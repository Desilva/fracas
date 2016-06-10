using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class PlantAvailibilityEntity
    {
        public int id { get; set; }
        public int id_foc { get; set; }
        public Nullable<double> op_avail { get; set; }
        public Nullable<System.DateTime> plant_down { get; set; }
        public Nullable<System.DateTime> plant_up { get; set; }
        public Nullable<int> selisih { get; set; }
        public Nullable<int> selisih_paf { get; set; }
        public Nullable<double> pof_bulanan { get; set; }
        public Nullable<double> paf_bulanan { get; set; }
        public Nullable<double> paf { get; set; }
        public Nullable<int> bulan { get; set; }
        public Nullable<int> tahun { get; set; }
        public string tipe { get; set; }
        public Nullable<double> target { get; set; }

        public string foc_name { get; set; }
    }
}