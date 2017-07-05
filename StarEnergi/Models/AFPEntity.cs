using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class AFPEntity
    {
        public int id { get; set; }        
        public double? afp_value { get; set; }
        public string afp_description { get; set; }
        public double? afp_score { get; set; }
        public string nama { get; set; }
    }
}