using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class EquipmentTableReportEntity
    {
        public int id { get; set; }
        public Nullable<int> id_equipment_daily_report { get; set; }
        public Nullable<int> id_equipment { get; set; }
        [StringLength(255)]
        public string tag_number { get; set; }
        [StringLength(255)]
        public string description { get; set; }
        [StringLength(20)]
        public string min_limit { get; set; }
        [StringLength(20)]
        public string max_limit { get; set; }
        [StringLength(20)]
        public string unit { get; set; }
        [StringLength(50)]
        public string tag_value { get; set; }
        public DateTime? date { get; set; }
        [StringLength(10)]
        public string time { get; set; }
        public string name_operator { get; set; }
        [StringLength(255)]
        public string keterangan { get; set; }
        [StringLength(10)]
        public string barcode { get; set; }
    }

    public class EquipmentReportEntity
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string operator_name { get; set; }
        public List<EquipmentTableReportEntity> table { get; set; }
    }
}