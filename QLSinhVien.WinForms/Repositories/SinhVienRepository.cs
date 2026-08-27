using MongoDB.Driver;
using QLSinhVien.WinForms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLSinhVien.WinForms.Repositories
{
    public class SinhVienRepository
    {
        private readonly IMongoCollection<SinhVien> _collection;

        public SinhVienRepository()
        {
            var database = Data.MongoDBConnection.Instance.Database;
            _collection = database.GetCollection<SinhVien>("sinhvien");
        }

        // 1. Read: Lấy toàn bộ danh sách sinh viên
        public async Task<List<SinhVien>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        // 2. Read: Tìm kiếm theo mã sinh viên (Masv)
        public async Task<SinhVien> GetByMaSVAsync(string masv) =>
            await _collection.Find(sv => sv.Masv == masv).FirstOrDefaultAsync();

        // 3. Read: Lọc theo mã lớp (Malop)
        public async Task<List<SinhVien>> GetByMaLopAsync(string malop) =>
            await _collection.Find(sv => sv.Malop == malop).ToListAsync();

        // 4. Create: Thêm mới 1 sinh viên
        public async Task InsertAsync(SinhVien sv) =>
            await _collection.InsertOneAsync(sv);

        // 5. Update: Cập nhật thông tin cơ bản
        public async Task UpdateBasicInfoAsync(string masv, SinhVien updated)
        {
            var updateDef = Builders<SinhVien>.Update
                .Set(sv => sv.Hoten, updated.Hoten)
                .Set(sv => sv.Tuoi, updated.Tuoi)
                .Set(sv => sv.Phai, updated.Phai)
                .Set(sv => sv.Malop, updated.Malop);

            await _collection.UpdateOneAsync(sv => sv.Masv == masv, updateDef);
        }

        // 6. Delete: Xóa 1 sinh viên theo mã (Masv)
        public async Task DeleteAsync(string masv) =>
            await _collection.DeleteOneAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Masv, masv));

        // 7. Delete: Xóa toàn bộ sinh viên theo lớp (Malop)
        public async Task DeleteByMaLopAsync(string malop) =>
            await _collection.DeleteManyAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Malop, malop));
    }
}