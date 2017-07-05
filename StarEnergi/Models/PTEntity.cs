using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class PTEntity
    {
        public int id { get; set; }        
        public double? pt_value { get; set; }
        public string pt_description { get; set; }
        public double? pt_score { get; set; }
        public string nama { get; set; }
    }
}