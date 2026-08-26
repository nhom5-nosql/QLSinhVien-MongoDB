using MongoDB.Bson;
using MongoDB.Driver;
using QLSinhVien.WinForms.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QLSinhVien.WinForms.Repositories
{
    /// <summary>
    /// Repository tìm kiếm sinh viên theo mã SV / họ tên (chính xác hoặc gần đúng).
    /// </summary>
    public class SinhVienRepository
    {
        private readonly IMongoCollection<SinhVien> _collection;

        public SinhVienRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<SinhVien>("sinhvien");
        }

        // ── Lấy TOÀN BỘ danh sách sinh viên ────────────────────────────────────
        public async Task<List<SinhVien>> GetAllStudentsAsync()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        // ── Tìm CHÍNH XÁC theo mã SV (không phân biệt hoa/thường) ──────────────
        public async Task<SinhVien?> FindByMasvExactAsync(string masv)
        {
            var pattern = $"^{Regex.Escape(masv)}$";
            var filter  = Builders<SinhVien>.Filter.Regex(s => s.Masv, new BsonRegularExpression(pattern, "i"));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // ── Tìm CHÍNH XÁC theo họ tên đầy đủ (không phân biệt hoa/thường) ──────
        public async Task<SinhVien?> FindByHotenExactAsync(string hoten)
        {
            var pattern = $"^{Regex.Escape(hoten)}$";
            var filter  = Builders<SinhVien>.Filter.Regex(s => s.Hoten, new BsonRegularExpression(pattern, "i"));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // ── Tìm GẦN ĐÚNG (partial match) theo mã SV hoặc họ tên ────────────────
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
