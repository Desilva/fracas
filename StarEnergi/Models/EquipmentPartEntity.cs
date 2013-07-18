using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class EquipmentPartEntity
    {
        [Key]
        public int id { get; set; }
        public int id_equipment { get; set; }
        [Required]
        public int id_parts { get; set; }
        public Nullable<double> mtbf { get; set; }
        public Nullable<double> mttr { get; set; }
        public Nullable<short> status { get; set; }
        public string sStatus { get; set; }
        public Nullable<double> lead_time { get; set; } //MDT

        [Required]
        [DataType(DataType.DateTime)]
        public Nullable<System.DateTime> installed_date { get; set; }
        public Nullable<System.DateTime> obsolete_date { get; set; }
        public string sInstalled_date { get; set; }
        public string sObsolete_date { get; set; }
        public double running_hours { get; set; }

        public string tag_num { get; set; }
        public string nama { get; set; }
        public string vendor { get; set; }
        public int? warranty { get; set; }

        public Nullable<double> assurance_level { get; set; }
        //public Nullable<double> avail_inherent { get; set; }
        //public Nullable<double> avail_measured { get; set; }
    }
}