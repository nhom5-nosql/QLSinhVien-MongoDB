using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace QLSinhVien.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // --- TEST KẾT NỐI MONGODB ---
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot config = builder.Build();
                string connectionString = config["MongoDB:ConnectionString"];
                var client = new MongoClient(connectionString);
                client.ListDatabaseNames().ToList();
                MessageBox.Show("Kết nối MongoDB thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: \n" + ex.Message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Application.Run(new FormMain());
        }
    }
}