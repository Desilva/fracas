using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class MasterAFPFEntity
    {
        public int id { get; set; }
        [UIHint("Double")]
        public Nullable<double> afp_value { get; set; }
        public string afp_description { get; set; }
        [UIHint("Double")]
        public Nullable<double> afp_score { get; set; }
    }
}