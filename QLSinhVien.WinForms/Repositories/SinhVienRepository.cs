using MongoDB.Bson;
using MongoDB.Driver;
using QLSinhVien.WinForms.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QLSinhVien.WinForms.Repositories
{
    public class SinhVienRepository
    {
        private readonly IMongoCollection<SinhVien> _collection;

        // Constructor khởi tạo nhận IMongoDatabase từ Singleton/DI
        public SinhVienRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SinhVien>("sinhvien");
        }

        public SinhVienRepository() 
            : this(Data.MongoDBConnection.Instance.Database)
        {
        }

        // ==========================================
        // I. PHẦN B: CÁC THAO TÁC CRUD CƠ BẢN
        // ==========================================

        // 1. Read: Lấy toàn bộ danh sách sinh viên
        public async Task<List<SinhVien>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        // 2. Read: Tìm kiếm chính xác theo Mã SV
        public async Task<SinhVien> GetByMaSVAsync(string masv) =>
            await _collection.Find(sv => sv.Masv == masv).FirstOrDefaultAsync();

        // 3. Read: Lọc danh sách theo Mã Lớp
        public async Task<List<SinhVien>> GetByMaLopAsync(string malop) =>
            await _collection.Find(sv => sv.Malop == malop).ToListAsync();

        // 4. Create: Thêm mới 1 sinh viên
        public async Task InsertAsync(SinhVien sv) =>
            await _collection.InsertOneAsync(sv);

        // 5. Update: Cập nhật thông tin cơ bản (Họ tên, Tuổi, Phái, Mã lớp)
        public async Task UpdateBasicInfoAsync(string masv, SinhVien updated)
        {
            var updateDef = Builders<SinhVien>.Update
                .Set(sv => sv.Hoten, updated.Hoten)
                .Set(sv => sv.Tuoi, updated.Tuoi)
                .Set(sv => sv.Phai, updated.Phai)
                .Set(sv => sv.Malop, updated.Malop);

            await _collection.UpdateOneAsync(sv => sv.Masv == masv, updateDef);
        }

        // Update toàn bộ thông tin sinh viên (bao gồm mảng Môn học và Ngoại ngữ)
        public async Task UpdateFullAsync(string masv, SinhVien updated)
        {
            var updateDef = Builders<SinhVien>.Update
                .Set(sv => sv.Hoten, updated.Hoten)
                .Set(sv => sv.Tuoi, updated.Tuoi)
                .Set(sv => sv.Phai, updated.Phai)
                .Set(sv => sv.Malop, updated.Malop)
                .Set(sv => sv.Ngoaingu, updated.Ngoaingu)
                .Set(sv => sv.Monhoc, updated.Monhoc);

            await _collection.UpdateOneAsync(sv => sv.Masv == masv, updateDef);
        }

        // 6. Delete: Xóa 1 sinh viên theo Mã SV
        public async Task DeleteAsync(string masv) =>
            await _collection.DeleteOneAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Masv, masv));

        // 7. Delete: Xóa toàn bộ sinh viên theo Mã Lớp
        public async Task DeleteByMaLopAsync(string malop) =>
            await _collection.DeleteManyAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Malop, malop));


        // ==========================================
        // II. PHẦN C: TÌM KIẾM NÂNG CAO & REGEX
        // ==========================================

        // 1. Lấy toàn bộ danh sách (phiên bản alias)
        public async Task<List<SinhVien>> GetAllStudentsAsync() =>
            await GetAllAsync();

        // 2. Tìm chính xác Mã SV (Regex Không phân biệt hoa/thường)
        public async Task<SinhVien?> FindByMasvExactAsync(string masv)
        {
            var pattern = $"^{Regex.Escape(masv)}$";
            var filter  = Builders<SinhVien>.Filter.Regex(s => s.Masv, new BsonRegularExpression(pattern, "i"));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // 3. Tìm chính xác Họ Tên (Regex Không phân biệt hoa/thường)
        public async Task<SinhVien?> FindByHotenExactAsync(string hoten)
        {
            var pattern = $"^{Regex.Escape(hoten)}$";
            var filter  = Builders<SinhVien>.Filter.Regex(s => s.Hoten, new BsonRegularExpression(pattern, "i"));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // 4. Tìm gần đúng (Partial Match) theo Mã SV hoặc Họ Tên cho SearchBar
        public async Task<List<SinhVien>> SearchPartialAsync(string keyword, int? limit = null)
        {
            var safePattern = Regex.Escape(keyword);
            var filter = Builders<SinhVien>.Filter.Or(
                Builders<SinhVien>.Filter.Regex(s => s.Masv,  new BsonRegularExpression(safePattern, "i")),
                Builders<SinhVien>.Filter.Regex(s => s.Hoten, new BsonRegularExpression(safePattern, "i"))
            );
            
            var query = _collection.Find(filter);
            if (limit.HasValue && limit.Value > 0)
            {
                query = query.Limit(limit.Value);
            }
            return await query.ToListAsync();
        }

        // ==========================================
        // III. GỢI Ý (SUGGESTIONS) CHO NGOẠI NGỮ & MÔN HỌC
        // ==========================================

        // Lấy tất cả ngoại ngữ duy nhất đã tồn tại trong CSDL
        public async Task<List<string>> GetAllDistinctLanguagesAsync()
        {
            var distinct = await _collection.DistinctAsync<string>("ngoaingu", new BsonDocument());
            var list = await distinct.ToListAsync();
            return list.FindAll(s => !string.IsNullOrWhiteSpace(s))
                       .Distinct(System.StringComparer.OrdinalIgnoreCase)
                       .OrderBy(s => s)
                       .ToList();
        }

        // Lấy tất cả môn học (Mã môn, Tên môn) duy nhất đã tồn tại trong CSDL qua Aggregation
        public async Task<List<MonHoc>> GetAllDistinctSubjectsAsync()
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$unwind", "$monhoc"),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "mamon", "$monhoc.mamon" },
                            { "tenmon", "$monhoc.tenmon" }
                        }
                    }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "mamon", "$_id.mamon" },
                    { "tenmon", "$_id.tenmon" }
                }),
                new BsonDocument("$sort", new BsonDocument("tenmon", 1))
            };

            var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            var list = new List<MonHoc>();
            foreach (var doc in docs)
            {
                string mamon = doc.Contains("mamon") && !doc["mamon"].IsBsonNull ? doc["mamon"].AsString : "";
                string tenmon = doc.Contains("tenmon") && !doc["tenmon"].IsBsonNull ? doc["tenmon"].AsString : "";
                if (!string.IsNullOrWhiteSpace(mamon) || !string.IsNullOrWhiteSpace(tenmon))
                {
                    list.Add(new MonHoc { Mamon = mamon, Tenmon = tenmon, Diem = 0 });
                }
            }
            return list;
        }

        // ==========================================
        // IV. TỰ ĐỘNG SINH MÃ SINH VIÊN (AUTO-GENERATE MASV)
        // ==========================================
        public async Task<string> GenerateNextMasvAsync()
        {
            var listMasv = await _collection.Find(_ => true)
                                            .Project(s => s.Masv)
                                            .ToListAsync();

            int maxNumber = 0;
            string prefix = "SV";
            int digitCount = 3;

            var regex = new Regex(@"^(?<prefix>[a-zA-Z]+)(?<num>\d+)$");

            foreach (var masv in listMasv)
            {
                if (string.IsNullOrWhiteSpace(masv)) continue;

                var match = regex.Match(masv.Trim());
                if (match.Success)
                {
                    string p = match.Groups["prefix"].Value;
                    string numStr = match.Groups["num"].Value;

                    if (numStr.Length > digitCount)
                        digitCount = numStr.Length;

                    if (int.TryParse(numStr, out int num))
                    {
                        if (num > maxNumber)
                        {
                            maxNumber = num;
                            prefix = p;
                        }
                    }
                }
            }

            int nextNum = maxNumber + 1;
            string candidate;
            do
            {
                candidate = $"{prefix}{nextNum.ToString().PadLeft(digitCount, '0')}";
                nextNum++;
            }
            while (listMasv.Exists(m => string.Equals(m, candidate, StringComparison.OrdinalIgnoreCase)));

            return candidate;
        }
    }
}