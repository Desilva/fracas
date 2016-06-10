using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class DashboardChartEntity
    {
        public int year { get; set; }
        public double? value { get; set; }
        public double? target { get; set; }
    }
}