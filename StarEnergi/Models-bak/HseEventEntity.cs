using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class HseEventEntity
    {
        public int id { get; set; }
        public DateTime? dateTimeIncident { get; set; }
        public string incidentType { get; set; }
        public string incidentLocation { get; set; }
        public string factualInformation { get; set; }

        public string immediateAction { get; set; }

    }
}