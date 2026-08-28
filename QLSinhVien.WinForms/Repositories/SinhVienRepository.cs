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

        // Constructor phụ phòng trường hợp Duy gọi không tham số (fallback về Singleton)
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

        // 6. Delete: Xóa 1 sinh viên theo Mã SV
        public async Task DeleteAsync(string masv) =>
            await _collection.DeleteOneAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Masv, masv));

        // 7. Delete: Xóa toàn bộ sinh viên theo Mã Lớp
        public async Task DeleteByMaLopAsync(string malop) =>
            await _collection.DeleteManyAsync(Builders<SinhVien>.Filter.Eq(sv => sv.Malop, malop));


        // ==========================================
        // II. PHẦN C: TÌM KIẾM NÂNG CAO & REGEX (SANG)
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
    }
}