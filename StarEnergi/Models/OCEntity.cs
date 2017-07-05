using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class OCEntity
    {
        public int id { get; set; }        
        public double? oc_value { get; set; }
        public string oc_description { get; set; }
        public double? oc_score { get; set; }
        public string nama { get; set; }
    }
}