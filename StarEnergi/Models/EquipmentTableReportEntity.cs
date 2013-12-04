﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class EquipmentTableReportEntity
    {
        public int id { get; set; }
        public Nullable<int> id_equipment_daily_report { get; set; }
        public Nullable<int> id_equipment { get; set; }
        public string tag_number { get; set; }
        public string description { get; set; }
        public string min_limit { get; set; }
        public string max_limit { get; set; }
        public string unit { get; set; }
        public string tag_value { get; set; }
        public DateTime? date { get; set; }
        public string time { get; set; }
        public string name_operator { get; set; }
        public string keterangan { get; set; }
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