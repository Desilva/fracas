using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class MasterPTEntity
    {
        public int id { get; set; }
        [UIHint("Double")]
        public Nullable<double> pt_value { get; set; }
        public string pt_description { get; set; }
        [UIHint("Double")]
        public Nullable<double> pt_score { get; set; }
    }
}