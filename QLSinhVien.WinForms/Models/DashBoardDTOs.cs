using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace QLSinhVien.WinForms.Models
{
    // DTO cho các chỉ số KPI
    public class KpiSummaryDto
    {
        public long TotalStudents { get; set; }
        public long TotalClasses { get; set; }
        public double AvgScore { get; set; }
        public long MaleCount { get; set; }
        public long FemaleCount { get; set; }
    }

    // DTO cho Thống kê theo Lớp
    public class ClassStatDto
    {
        [BsonId]
        public string MaLop { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public double MaxAvgScore { get; set; }
        public double MinAvgScore { get; set; }
    }

    // DTO cho Ngoại ngữ phổ biến ($unwind)
    public class LanguageStatDto
    {
        [BsonId]
        public string NgoaiNgu { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    // DTO cho Top Sinh Viên & Phân loại
    public class StudentRankDto
    {
        public string Masv { get; set; } = string.Empty;
        public string Hoten { get; set; } = string.Empty;
        public string Malop { get; set; } = string.Empty;
        public double DiemTB { get; set; }
        public string XepLoai { get; set; } = string.Empty;
    }
}
