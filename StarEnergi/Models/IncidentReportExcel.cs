﻿using System;
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

    public class IREntity
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
        public string ack_supervisor { get; set; }
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
        public Nullable<int> id_rca { get; set; }
        public string superintendent_approve { get; set; }
        public string field_manager_approve { get; set; }
        public string loss_control_approve { get; set; }
        public string she_superintendent_approve { get; set; }
        public string lead_name { get; set; }
        public string superintendent_delegate { get; set; }
        public string field_manager_delegate { get; set; }
        public string loss_control_delegate { get; set; }
        public string she_superintendent_delegate { get; set; }
        public string supervisor_approve { get; set; }
        public string supervisor_delegate { get; set; }
        public Nullable<byte> kontraktor_seg { get; set; }

        public string prepared_by_name { get; set; }
        public string tsr_number { get; set; }
        public string fracas_number { get; set; }
        public string rca_number { get; set; }
        public string type_report { get; set; }
        public string actual_loss { get; set; }
        public string potential_loss { get; set; }
        public string probability_str { get; set; }
        public string inves { get; set; }

        public bool isCanEdit { get; set; }
        public bool isSuspend { get; set; }
    }
}