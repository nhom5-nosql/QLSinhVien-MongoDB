using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using QLSinhVien.WinForms.Forms;
using QLSinhVien.WinForms.Repositories;

namespace QLSinhVien.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Đọc cấu hình từ appsettings.json
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config["MongoDB:ConnectionString"]!;
            string databaseName     = config["MongoDB:DatabaseName"]!;

            // 2. Khởi tạo MongoClient và Database (1 lần duy nhất cho toàn app)
            IMongoClient mongoClient;
            IMongoDatabase database;

            try
            {
                mongoClient = new MongoClient(connectionString);
                database    = mongoClient.GetDatabase(databaseName);

                // Test kết nối: liệt kê database names
                mongoClient.ListDatabaseNames().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Không thể kết nối MongoDB!\n\n{ex.Message}",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return; // Thoát nếu không kết nối được
            }

            // 3. Khởi tạo các Repository và chạy FormDashBoard
            var dashboardRepo = new DashboardRepository(database);
            var svRepo        = new SinhVienRepository(database);
            Application.Run(new FormDashBoard(dashboardRepo, svRepo));
        }
    }
}