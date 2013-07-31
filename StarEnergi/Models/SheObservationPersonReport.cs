using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class SheObservationPersonReport
    {
        public string alpha_name { get; set; }
        public string department { get; set; }
        public int total_observation { get; set; }
        public int total_quality_obs { get; set; }
    }
}