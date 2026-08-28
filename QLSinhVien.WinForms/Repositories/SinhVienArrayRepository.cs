using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using QLSinhVien.WinForms.Models;

namespace QLSinhVien.WinForms.Repositories
{
    public class SinhVienArrayRepository
    {
        private readonly IMongoCollection<SinhVien> _collection;

        public SinhVienArrayRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SinhVien>("sinhvien");
        }

        // 1. Thêm 1 ngoại ngữ mới cho SV ($addToSet chống trùng)
        public async Task<bool> AddLanguageAsync(string masv, string ngoaiNguMoi)
        {
            var filter = Builders<SinhVien>.Filter.Eq(s => s.Masv, masv);
            var update = Builders<SinhVien>.Update.AddToSet(s => s.Ngoaingu, ngoaiNguMoi);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // 2. Thêm 1 môn học mới cho SV ($push)
        public async Task<bool> AddSubjectAsync(string masv, MonHoc monHocMoi)
        {
            var filter = Builders<SinhVien>.Filter.Eq(s => s.Masv, masv);
            var update = Builders<SinhVien>.Update.Push(s => s.Monhoc, monHocMoi);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // 3. Cập nhật điểm của 1 môn học cụ thể (Dùng Positional Operator $)
        public async Task<bool> UpdateSubjectScoreAsync(string masv, string mamon, double diemMoi)
        {
            var filter = Builders<SinhVien>.Filter.And(
                Builders<SinhVien>.Filter.Eq(s => s.Masv, masv),
                Builders<SinhVien>.Filter.Eq("monhoc.mamon", mamon)
            );

            var update = Builders<SinhVien>.Update.Set("monhoc.$.diem", diemMoi);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // 4. Thay thế toàn bộ Document theo _id (replaceOne)
        public async Task<bool> ReplaceStudentAsync(SinhVien svMoi)
        {
            var filter = Builders<SinhVien>.Filter.Eq(s => s.Id, svMoi.Id);
            var result = await _collection.ReplaceOneAsync(filter, svMoi);
            return result.ModifiedCount > 0;
        }
    }
}
