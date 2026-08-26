using MongoDB.Bson;
using MongoDB.Driver;
using QLSinhVien.WinForms.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLSinhVien.WinForms.Repositories
{
    public class DashboardRepository
    {
        private readonly IMongoCollection<SinhVien> _collection;

        public DashboardRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SinhVien>("sinhvien");
        }

        // 1. Lấy dữ liệu KPI Cards
        public async Task<KpiSummaryDto> GetKpiSummaryAsync()
        {
            var totalStudents = await _collection.CountDocumentsAsync(new BsonDocument());

            var distinctClasses = await _collection.DistinctAsync<string>("malop", new BsonDocument());
            var totalClasses = (await distinctClasses.ToListAsync()).Count;

            var maleCount = await _collection.CountDocumentsAsync(Builders<SinhVien>.Filter.Eq(s => s.Phai, "Nam"));
            var femaleCount = await _collection.CountDocumentsAsync(Builders<SinhVien>.Filter.Eq(s => s.Phai, "Nữ"));

            // Tính điểm TB toàn trường
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$unwind", "$monhoc"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "avgScore", new BsonDocument("$avg", "$monhoc.diem") }
                })
            };

            var avgResult = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            double avgScore = avgResult != null && avgResult.Contains("avgScore") ? Math.Round(avgResult["avgScore"].AsDouble, 2) : 0;

            return new KpiSummaryDto
            {
                TotalStudents = totalStudents,
                TotalClasses = totalClasses,
                AvgScore = avgScore,
                MaleCount = maleCount,
                FemaleCount = femaleCount
            };
        }

        // 2. Thống kê ngoại ngữ phổ biến ($unwind + $group)
        public async Task<List<LanguageStatDto>> GetPopularLanguagesAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$unwind", "$ngoaingu"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$ngoaingu" },
                    { "Count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("Count", -1))
            };

            return await _collection.Aggregate<LanguageStatDto>(pipeline).ToListAsync();
        }

        // 3. Phân loại học lực (Xuất sắc / Giỏi / Khá / TB-Yếu)
        public async Task<Dictionary<string, int>> GetAcademicRankStatsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "diemTB", new BsonDocument("$avg", "$monhoc.diem") }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "xepLoai", new BsonDocument("$switch", new BsonDocument
                        {
                            { "branches", new BsonArray
                                {
                                    new BsonDocument { { "case", new BsonDocument("$gte", new BsonArray { "$diemTB", 8.5 }) }, { "then", "Xuất sắc" } },
                                    new BsonDocument { { "case", new BsonDocument("$gte", new BsonArray { "$diemTB", 7.0 }) }, { "then", "Giỏi" } },
                                    new BsonDocument { { "case", new BsonDocument("$gte", new BsonArray { "$diemTB", 5.5 }) }, { "then", "Khá" } }
                                }
                            },
                            { "default", "TB/Yếu" }
                        })
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$xepLoai" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

            var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var result = new Dictionary<string, int>
            {
                ["Xuất sắc"] = 0,
                ["Giỏi"] = 0,
                ["Khá"] = 0,
                ["TB/Yếu"] = 0
            };
            foreach (var doc in docs)
            {
                string key = doc["_id"].AsString;
                if (result.ContainsKey(key))
                    result[key] = doc["count"].AsInt32;
            }
            return result;
        }

        // 4. Thống kê theo lớp: số SV, ĐTB max, ĐTB min
        public async Task<List<ClassStatDto>> GetClassStatsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "malop", "$malop" },
                    { "diemTB", new BsonDocument("$avg", "$monhoc.diem") }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$malop" },
                    { "TotalStudents", new BsonDocument("$sum", 1) },
                    { "MaxAvgScore", new BsonDocument("$max", "$diemTB") },
                    { "MinAvgScore", new BsonDocument("$min", "$diemTB") }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

            var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var list = new List<ClassStatDto>();
            foreach (var doc in docs)
            {
                list.Add(new ClassStatDto
                {
                    MaLop = doc["_id"].AsString,
                    TotalStudents = doc["TotalStudents"].AsInt32,
                    MaxAvgScore = Math.Round(doc["MaxAvgScore"].AsDouble, 2),
                    MinAvgScore = Math.Round(doc["MinAvgScore"].AsDouble, 2)
                });
            }
            return list;
        }

        // 5. Top 5 Sinh viên điểm TB cao nhất
        public async Task<List<StudentRankDto>> GetTop5StudentsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "masv", "$masv" },
                    { "hoten", "$hoten" },
                    { "malop", "$malop" },
                    { "diemTB", new BsonDocument("$avg", "$monhoc.diem") }
                }),
                new BsonDocument("$sort", new BsonDocument("diemTB", -1)),
                new BsonDocument("$limit", 5)
            };

            var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var list = new List<StudentRankDto>();

            foreach (var doc in docs)
            {
                double score = doc.Contains("diemTB") && !doc["diemTB"].IsBsonNull ? Math.Round(doc["diemTB"].AsDouble, 2) : 0;
                string rank = score >= 8.5 ? "Xuất sắc" : (score >= 7.0 ? "Giỏi" : (score >= 5.5 ? "Khá" : "TB/Yếu"));

                list.Add(new StudentRankDto
                {
                    Masv = doc["masv"].AsString,
                    Hoten = doc["hoten"].AsString,
                    Malop = doc["malop"].AsString,
                    DiemTB = score,
                    XepLoai = rank
                });
            }

            return list;
        }
    }
}
