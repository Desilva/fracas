using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class RCEntity
    {
        public int id { get; set; }        
        public double? rc_value { get; set; }
        public string rc_description { get; set; }
        public double? rc_score { get; set; }
        public string nama { get; set; }
    }
}