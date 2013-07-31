using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class IncidentReportExcel
    {
    }

    public class incident_report_r
    {
        public string type { get; set; }
        public int cases { get; set; }
        public double cost_seg { get; set; }
        public double cost_kon { get; set; }
        public double total_cost { get; set; }

    }

    public class incident_report_e
    {
        public string reference_number { get; set; }
        public Nullable<System.DateTime> prepare_date { get; set; }
        public Nullable<System.DateTime> date_incident { get; set; }
        public string title { get; set; }
        public string incident_type { get; set; }
        public string cost_estimate { get; set; }
        public string prepared_by { get; set; }
    }
}