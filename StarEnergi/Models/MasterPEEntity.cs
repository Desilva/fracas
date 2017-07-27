using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class MasterPEEntity
    {
        public int id { get; set; }
        [UIHint("Double")]
        public Nullable<double> pe_value { get; set; }
        public string pe_description { get; set; }
        [UIHint("Double")]
        public Nullable<double> pe_score { get; set; }
    }
}