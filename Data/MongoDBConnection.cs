using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using QLSinhVien.WinForms.Models;

namespace QLSinhVien.WinForms.Data
{
    public sealed class MongoDBConnection
    {
        private static readonly Lazy<MongoDBConnection> _instance =
            new(() => new MongoDBConnection());

        private readonly IMongoDatabase _database;
        private MongoDBConnection()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = config["MongoDB:ConnectionString"];
            var databaseName = config["MongoDB:DatabaseName"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
                throw new Exception("Không tìm thấy cấu hình MongoDB trong appsettings.json");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }
        public static MongoDBConnection Instance => _instance.Value;
        public IMongoDatabase Database => _database;
        public IMongoCollection<SinhVien> SinhViens =>
            _database.GetCollection<SinhVien>("sinhvien");
    }
}
