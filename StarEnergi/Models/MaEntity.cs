using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class MaEntity
    {
        public int id { get; set; }
        public int id_foc { get; set; }
        public double tadd { get; set; }
        public double tadt_hours { get; set; }
        public double ta_interval { get; set; }
        public double ntamdd { get; set; }
        public double study_year_ntamd { get; set; }
        public double prior_year_ntamd { get; set; }
        public double ntamsd { get; set; }
        public double study_year_ms { get; set; }
        public double prior_year_ms { get; set; }
        public double days_in_study_year { get; set; }
        public System.DateTime last_update { get; set; }
        public Nullable<double> masd { get; set; }
        public Nullable<double> ma { get; set; }
        public Nullable<byte> category { get; set; }
        public string type { get; set; }

        public string foc_name { get; set; }
    }
}