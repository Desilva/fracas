using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class MPSRModel
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public int addMPSR(monthly_project_she_report mpsr)
        {
            List<monthly_project_she_report> prev_mpsr = this.db.monthly_project_she_report.Where(p => p.month_year < mpsr.month_year && p.month_year.Value.Year == mpsr.month_year.Value.Year && p.contractor_id == mpsr.contractor_id && p.no_contract == mpsr.no_contract).ToList();

            mpsr.incident_minor_ytd = mpsr.incident_minor_total;
            mpsr.incident_minor_ytd_cost = mpsr.incident_minor_cost;
            mpsr.incident_moderate_ytd = mpsr.incident_moderate_total;
            mpsr.incident_moderate_ytd_cost = mpsr.incident_moderate_cost;
            mpsr.incident_serious_ytd = mpsr.incident_serious_total;
            mpsr.incident_serious_ytd_cost = mpsr.incident_serious_cost;
            mpsr.incident_major_ytd = mpsr.incident_major_total;
            mpsr.incident_major_ytd_cost = mpsr.incident_major_cost;
            mpsr.toolbox_meeting_ytd = mpsr.toolbox_meeting_total;
            mpsr.weekly_she_meeting_ytd = mpsr.weekly_she_meeting_total;
            mpsr.monthly_contr_mig_ytd = mpsr.monthly_contr_mig_total;
            mpsr.environmental_loss_ytd = mpsr.environmental_loss_total;
            mpsr.environmental_loss_ytd_cost = mpsr.environmental_loss_cost;
            mpsr.she_observation_card_ytd = mpsr.she_observation_card_total;
            mpsr.property_damage_ytd = mpsr.property_damage_total;
            mpsr.property_damage_ytd_cost = mpsr.property_damage_cost;
            mpsr.new_jsa_hira_ytd = mpsr.new_jsa_hira_total;
            mpsr.process_loss_ytd = mpsr.process_loss_total;
            mpsr.process_loss_ytd_cost = mpsr.process_loss_cost;
            mpsr.ptw_issued_ytd = mpsr.ptw_issued_total;
            mpsr.external_relation_ytd = mpsr.external_relation_total;
            mpsr.external_relation_ytd_cost = mpsr.external_relation_cost;
            mpsr.theft_crime_ytd = mpsr.theft_crime_total;
            mpsr.theft_crime_ytd_cost = mpsr.theft_crime_cost;
            mpsr.facility_inspection_ytd = mpsr.facility_inspection_total;
            mpsr.vehicular_ytd = mpsr.vehicular_total;
            mpsr.vehicular_ytd_cost = mpsr.vehicular_cost;
            mpsr.vehicular_inspection_ytd = mpsr.vehicular_inspection_total;
            mpsr.near_miss_ytd = mpsr.near_miss_total;
            mpsr.near_miss_ytd_cost = mpsr.near_miss_cost;
            mpsr.ppe_inspection_ytd = mpsr.ppe_inspection_total;
            mpsr.lifting_eq_inspection_ytd = mpsr.lifting_eq_inspection_total;
            mpsr.man_hours_ytd = mpsr.man_hours_mh;
            mpsr.fire_inspection_ytd = mpsr.fire_inspection_total;
            mpsr.days_ytd = mpsr.days_mh;
            mpsr.vehicle_emission_ytd = mpsr.vehicle_emission_total;
            mpsr.welding_eq_inspection_ytd = mpsr.welding_eq_inspection_total;
            mpsr.hde_inspection_ytd = mpsr.hde_inspection_total;
            mpsr.light_vehicle_travel_ytd = mpsr.light_vehicle_travel_mh;
            mpsr.fire_emergency_ytd = mpsr.fire_emergency_total;
            mpsr.h2s_emergency_ytd = mpsr.h2s_emergency_total;
            mpsr.environmental_spill_ytd = mpsr.environmental_spill_total;
            mpsr.medical_evacuation_ytd = mpsr.medical_evacuation_total;
            mpsr.fit_for_day_ytd = mpsr.fit_for_day_total;
            mpsr.domestic_waste_ytd = mpsr.domestic_waste_total;
            mpsr.clinic_visit_ytd = mpsr.clinic_visit_total;
            mpsr.hazardous_waste_ytd = mpsr.hazardous_waste_total;
            mpsr.no_work_illness_ytd = mpsr.no_work_illness_total;
            mpsr.new_msds_ytd = mpsr.new_msds_total;
            mpsr.ill_monitoring_ytd = mpsr.ill_monitoring_total;
            mpsr.local_total = mpsr.local_workers + mpsr.local_lead + mpsr.local_spv;
            mpsr.non_local_total = mpsr.non_local_workers + mpsr.non_local_lead + mpsr.non_local_spv;
            mpsr.expatriates_total = mpsr.expatriates_workers + mpsr.expatriates_lead + mpsr.expatriates_spv;
            mpsr.incident_frequency_rate_mh = (double)(mpsr.incident_major_total + mpsr.incident_minor_total + mpsr.incident_moderate_total + mpsr.incident_serious_total) * 200000 / (double)(mpsr.man_hours_mh);
            mpsr.incident_severity_rate_mh = (double)(mpsr.incident_major_total + mpsr.incident_moderate_total + mpsr.incident_serious_total) * 200000 / (double)(mpsr.man_hours_mh);
            mpsr.lti_ytd = 0;

            foreach (monthly_project_she_report m in prev_mpsr)
            {
                mpsr.incident_minor_ytd += m.incident_minor_total;
                mpsr.incident_minor_ytd_cost += m.incident_minor_cost;
                mpsr.incident_moderate_ytd += m.incident_moderate_total;
                mpsr.incident_moderate_ytd_cost += m.incident_moderate_cost;
                mpsr.incident_serious_ytd += m.incident_serious_total;
                mpsr.incident_serious_ytd_cost += m.incident_serious_cost;
                mpsr.incident_major_ytd += m.incident_major_total;
                mpsr.incident_major_ytd_cost += m.incident_major_cost;
                mpsr.toolbox_meeting_ytd += m.toolbox_meeting_total;
                mpsr.weekly_she_meeting_ytd += m.weekly_she_meeting_total;
                mpsr.monthly_contr_mig_ytd += m.monthly_contr_mig_total;
                mpsr.environmental_loss_ytd += m.environmental_loss_total;
                mpsr.environmental_loss_ytd_cost += m.environmental_loss_cost;
                mpsr.she_observation_card_ytd += m.she_observation_card_total;
                mpsr.property_damage_ytd += m.property_damage_total;
                mpsr.property_damage_ytd_cost += m.property_damage_cost;
                mpsr.new_jsa_hira_ytd += m.new_jsa_hira_total;
                mpsr.process_loss_ytd += m.process_loss_total;
                mpsr.process_loss_ytd_cost += m.process_loss_cost;
                mpsr.ptw_issued_ytd += m.ptw_issued_total;
                mpsr.external_relation_ytd += m.external_relation_total;
                mpsr.external_relation_ytd_cost += m.external_relation_cost;
                mpsr.theft_crime_ytd += m.theft_crime_total;
                mpsr.theft_crime_ytd_cost += m.theft_crime_cost;
                mpsr.facility_inspection_ytd += m.facility_inspection_total;
                mpsr.vehicular_ytd += m.vehicular_total;
                mpsr.vehicular_ytd_cost += m.vehicular_cost;
                mpsr.vehicular_inspection_ytd += m.vehicular_inspection_total;
                mpsr.near_miss_ytd += m.near_miss_total;
                mpsr.near_miss_ytd_cost += m.near_miss_cost;
                mpsr.ppe_inspection_ytd += m.ppe_inspection_total;
                mpsr.lifting_eq_inspection_ytd += m.lifting_eq_inspection_total;
                mpsr.man_hours_ytd += m.man_hours_mh;
                mpsr.fire_inspection_ytd += m.fire_inspection_total;
                mpsr.days_ytd += m.days_mh;
                mpsr.vehicle_emission_ytd += m.vehicle_emission_total;
                mpsr.welding_eq_inspection_ytd += m.welding_eq_inspection_total;
                mpsr.hde_inspection_ytd += m.hde_inspection_total;
                mpsr.light_vehicle_travel_ytd += m.light_vehicle_travel_mh;
                mpsr.fire_emergency_ytd += m.fire_emergency_total;
                mpsr.h2s_emergency_ytd += m.h2s_emergency_total;
                mpsr.environmental_spill_ytd += m.environmental_spill_total;
                mpsr.medical_evacuation_ytd += m.medical_evacuation_total;
                mpsr.fit_for_day_ytd += m.fit_for_day_total;
                mpsr.domestic_waste_ytd += m.domestic_waste_total;
                mpsr.clinic_visit_ytd += m.clinic_visit_total;
                mpsr.hazardous_waste_ytd += m.hazardous_waste_total;
                mpsr.no_work_illness_ytd += m.no_work_illness_total;
                mpsr.new_msds_ytd += m.new_msds_total;
                mpsr.ill_monitoring_ytd += m.ill_monitoring_total;
                mpsr.lti_ytd += (m.last_date_time_lti != null ? 1 : 0);
            }

            mpsr.incident_frequency_rate_ytd = (double)(mpsr.incident_major_ytd + mpsr.incident_minor_ytd + mpsr.incident_moderate_ytd + mpsr.incident_serious_ytd) * 200000 / (double)(mpsr.man_hours_ytd);
            mpsr.incident_severity_rate_ytd = (double)(mpsr.incident_major_ytd + mpsr.incident_moderate_ytd + mpsr.incident_serious_ytd) * 200000 / (double)(mpsr.man_hours_ytd);

            db.monthly_project_she_report.Add(mpsr);
            db.SaveChanges();

            return mpsr.id;
        }
    }
}