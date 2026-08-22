using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QLSinhVien.WinForms.Models
{
    public class SinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("masv")]
        public string Masv { get; set; } = string.Empty;

        [BsonElement("hoten")]
        public string Hoten { get; set; } = string.Empty;

        [BsonElement("tuoi")]
        public int Tuoi { get; set; }

        [BsonElement("phai")]
        public string Phai { get; set; } = string.Empty;  

        [BsonElement("malop")]
        public string Malop { get; set; } = string.Empty;

        [BsonElement("ngoaingu")]
        public List<string> Ngoaingu { get; set; } = new();
        
        [BsonElement("monhoc")]
        public List<MonHoc> Monhoc { get; set; } = new();
    }
}

