using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class PrintIRModel
    {
        public int id { get; set; }
        public string facility { get; set; }
        public string incident_location { get; set; }
        public string reference_number { get; set; }
        public Nullable<byte> type_of_report { get; set; }
        public Nullable<System.DateTime> date_incident { get; set; }
        public string title { get; set; }
        public string incident_type { get; set; }
        public Nullable<byte> actual_loss_severity { get; set; }
        public Nullable<byte> potential_loss_severity { get; set; }
        public Nullable<byte> probability { get; set; }
        public string factual_information { get; set; }
        public string cost_estimate { get; set; }
        public string immediate_action { get; set; }
        public string prepared_by { get; set; }
        public string prepared_by_jabatan { get; set; }
        public string ack_supervisor { get; set; }
        public string ack_supervisor_jabatan { get; set; }
        public Nullable<System.DateTime> prepare_date { get; set; }
        public Nullable<System.DateTime> ack_date { get; set; }
        public string superintendent { get; set; }
        public string loss_control { get; set; }
        public string field_manager { get; set; }
        public string she_superintendent { get; set; }
        public Nullable<System.DateTime> superintendent_date { get; set; }
        public Nullable<System.DateTime> loss_date { get; set; }
        public Nullable<System.DateTime> field_manager_date { get; set; }
        public Nullable<System.DateTime> she_superintendent_date { get; set; }
        public Nullable<byte> investigation { get; set; }
        public string investigation_req { get; set; }
        public string superintendent_approve { get; set; }
        public string field_manager_approve { get; set; }
        public string loss_control_approve { get; set; }
        public string she_superintendent_approve { get; set; }
        public string lead_name { get; set; }
        public List<string> attach { get; set; }
    }

    public class PrintIIRModel
    {
        public int id { get; set; }
        public string facility { get; set; }
        public string title { get; set; }
        public string reference_number { get; set; }
        public Nullable<System.DateTime> date_incident { get; set; }
        public string incident_location { get; set; }
        public string incident_type { get; set; }
        public string actual_loss { get; set; }
        public string potential_loss { get; set; }
        public string probability { get; set; }
        public string factual_information { get; set; }
        public string immediate_action { get; set; }
        public string immediate_causes { get; set; }
        public string basic_causes { get; set; }
        public string additional_observation { get; set; }
        public string cost_estimate { get; set; }
        public List<string> investigator { get; set; }
        public Nullable<System.DateTime> investigator_date { get; set; }
        public string loss_control { get; set; }
        public Nullable<System.DateTime> loss_control_date { get; set; }
        public string field_manager { get; set; }
        public Nullable<System.DateTime> field_manager_date { get; set; }
        public string field_manager_approve { get; set; }
        public string loss_control_approve { get; set; }
        public List<string> investigator_approve { get; set; }
        public string safety_officer { get; set; }
        public Nullable<System.DateTime> safety_officer_date { get; set; }
        public string safety_officer_approve { get; set; }

        public List<iir_recommendations> iir_recommendations { get; set; }
    }
}