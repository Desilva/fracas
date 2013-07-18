using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class PIREntity
    {
        public int id { get; set; }
        public string no { get; set; }
        public string improvement_request { get; set; }
        public Nullable<System.DateTime> date_rise { get; set; }
        public string initiate_by { get; set; }
        public string title { get; set; }
        public string reference { get; set; }
        public string procedure_reference { get; set; }
        public Nullable<System.DateTime> initiator_sign_date { get; set; }
        public Nullable<System.DateTime> kpbo_sign_date_init { get; set; }
        public Nullable<System.DateTime> target_completion_init { get; set; }
        public string desc_prob { get; set; }
        public string investigator { get; set; }
        public Nullable<System.DateTime> investigator_date { get; set; }
        public string improvement_plant { get; set; }
        public Nullable<System.DateTime> start_implement_date { get; set; }
        public string process_owner { get; set; }
        public Nullable<System.DateTime> target_completion_process { get; set; }
        public string action_by { get; set; }
        public string require_dokument { get; set; }
        public string hira_require { get; set; }
        public Nullable<System.DateTime> kpbo_sign_date_process { get; set; }
        public Nullable<System.DateTime> review_date { get; set; }
        public string result_of_action { get; set; }
        public Nullable<System.DateTime> kpbo_sign_date_process_result { get; set; }
        public Nullable<System.DateTime> sign_date_verified { get; set; }
        public string verified_desc { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }
}