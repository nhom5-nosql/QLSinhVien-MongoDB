using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace QLSinhVien.WinForms.Models
{
    [BsonIgnoreExtraElements]
    public class MonHoc
    {
        [BsonElement("mamon")]
        public string Mamon { get; set; } = string.Empty;

        [BsonElement("tenmon")]
        public string Tenmon { get; set; } = string.Empty;

        [BsonElement("diem")]
        public double Diem { get; set; }
    }
}
