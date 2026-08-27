using MongoDB.Driver;
using QLSinhVien.WinForms.Models;

namespace QLSinhVien.WinForms.Data
{
    public static class IndexInitializer
    {
        public static void EnsureIndexes(IMongoCollection<SinhVien> collection)
        {
            var uniqueIndex = new CreateIndexModel<SinhVien>(
                Builders<SinhVien>.IndexKeys.Ascending(sv => sv.Masv),
                new CreateIndexOptions
                {
                    Unique = true,           
                    Name = "idx_masv_unique" 
                });
            var compoundIndex = new CreateIndexModel<SinhVien>(
                Builders<SinhVien>.IndexKeys
                    .Ascending(sv => sv.Malop)   
                    .Ascending(sv => sv.Hoten), 
                new CreateIndexOptions { Name = "idx_malop_hoten" });
            collection.Indexes.CreateMany(new[] { uniqueIndex, compoundIndex });
        }
    }
}