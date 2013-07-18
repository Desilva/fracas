using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class TargetPafEntity
    {
        public int id { get; set; }
        public int id_foc { get; set; }
        public Nullable<int> tahun { get; set; }
        public Nullable<int> bulan { get; set; }
        public Nullable<double> target_paf { get; set; }

        public string foc_name { get; set; }
    }
}