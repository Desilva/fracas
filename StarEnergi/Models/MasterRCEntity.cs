using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class MasterRCEntity
    {
        public int id { get; set; }
        [UIHint("Double")]
        public Nullable<double> rc_value { get; set; }
        public string rc_description { get; set; }
        [UIHint("Double")]
        public Nullable<double> rc_score { get; set; }
    }
}