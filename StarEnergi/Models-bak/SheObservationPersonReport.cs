using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class SheObservationPersonReport
    {
        public int id_employee { get; set; }
        public string alpha_name { get; set; }
        public string department { get; set; }
        public int total_observation { get; set; }
        public int total_quality_obs { get; set; }
    }

    public class SheObservationTextInput
    {
        public string datetime { get; set; }
        public string alpha_name { get; set; }
        public string department { get; set; }
        public string location { get; set; }
        public string activity { get; set; }
        public string safe_observed { get; set; }
        public string action_taken { get; set; }
        public string unsafe_observed { get; set; }
        public string immediate_action { get; set; }
        public string action_prevent { get; set; }
    }
}