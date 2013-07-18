using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class UnitEntity
    {
        public int id { get; set; }
        public int id_foc { get; set; }
        public string nama { get; set; }
        public double ma { get; set; }
        public double masd { get; set; }
        public double avail_inherent { get; set; }
        public double avail_operational { get; set; }
    }
}