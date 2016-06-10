using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class DailyLogReportEntity
    {
        public int id { get; set; }
        public Nullable<System.DateTime> date { get; set; }

        // section 1
        public bool section1 { get; set; }
        public string grup_1 { get; set; }
        public string production_foreman_1 { get; set; }
        public string production_operator_1_1 { get; set; }
        public string production_operator_2_1 { get; set; }
        public string production_operator_3_1 { get; set; }
        public string production_operator_4_1 { get; set; }
        public string production_operator_5_1 { get; set; }
        public string production_operator_6_1 { get; set; }
        public string production_operator_7_1 { get; set; }
        public string production_operator_8_1 { get; set; }
        public string grup_2 { get; set; }
        public string production_foreman_2 { get; set; }
        public string production_operator_1_2 { get; set; }
        public string production_operator_2_2 { get; set; }
        public string production_operator_3_2 { get; set; }
        public string production_operator_4_2 { get; set; }
        public string production_operator_5_2 { get; set; }
        public string production_operator_6_2 { get; set; }
        public string production_operator_7_2 { get; set; }
        public string production_operator_8_2 { get; set; }

        // section 2
        public bool section2 { get; set; }
        public virtual List<daily_log_power_stations> list_daily_log_power_stations { get; set; }
        public virtual List<daily_log_sags> list_daily_log_sags { get; set; }

        // section 3
        public bool section3 { get; set; }
        public string uti_active_1 { get; set; }
        public string uti_reactive_1 { get; set; }
        public string sc_main_1 { get; set; }
        public string sc_auxiliary_1 { get; set; }
        public string ge_active_1 { get; set; }
        public string ge_reactive_1 { get; set; }
        public string metering_segwwl_1 { get; set; }
        public string metering_pln_1 { get; set; }
        public string condensate_ps_1 { get; set; }
        public string segwwl_availability_1 { get; set; }
        public string pln_dispatch_1 { get; set; }
        public string pln_meter_1 { get; set; }
        public string segwwl_export_1 { get; set; }
        public string actual_export_1 { get; set; }
        public string production_excess_1 { get; set; }
        public string rpf_1 { get; set; }
        public string pgf_1 { get; set; }
        public string pln_1 { get; set; }
        public string uti_active_2 { get; set; }
        public string uti_reactive_2 { get; set; }
        public string sc_main_2 { get; set; }
        public string sc_auxiliary_2 { get; set; }
        public string ge_active_2 { get; set; }
        public string ge_reactive_2 { get; set; }
        public string metering_segwwl_2 { get; set; }
        public string metering_pln_2 { get; set; }
        public string condensate_ps_2 { get; set; }
        public string segwwl_availability_2 { get; set; }
        public string pln_dispatch_2 { get; set; }
        public string pln_meter_2 { get; set; }
        public string segwwl_export_2 { get; set; }
        public string actual_export_2 { get; set; }
        public string production_excess_2 { get; set; }
        public string rpf_2 { get; set; }
        public string pgf_2 { get; set; }
        public string pln_2 { get; set; }
        public string condensate_total { get; set; }
        public string brine_total { get; set; }
        public string note { get; set; }

        //section 4
        public bool section4 { get; set; }
        public string target_1 { get; set; }
        public string target_2 { get; set; }
        public string achievement_1_1 { get; set; }
        public string achievement_2_1 { get; set; }
        public string remark_1_1 { get; set; }
        public string remark_2_1 { get; set; }
        public string achievement_1_2 { get; set; }
        public string achievement_2_2 { get; set; }
        public string remark_1_2 { get; set; }
        public string remark_2_2 { get; set; }

        //section 5
        public bool section5 { get; set; }
        public virtual List<daily_log_last_plant_status> list_daily_log_last_plant_status { get; set; }

    }

    public class DailyLogReport
    {
        public List<DailyLogReportEntity> list_entity {get;set;}
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
    }
}
