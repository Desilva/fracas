using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class SheProjectController : Controller
    {
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /SheProject/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/SheProject" });
            }

            var has = (from employees in db.employees
                        join dept in db.employee_dept on employees.dept_id equals dept.id
                        join users in db.users on employees.id equals users.employee_id into user_employee
                        from ue in user_employee.DefaultIfEmpty()
                        select new EmployeeEntity
                        {
                            id = employees.id,
                            alpha_name = employees.alpha_name,
                            employee_no = employees.employee_no,
                            position = employees.position,
                            work_location = employees.work_location,
                            dob = employees.dob,
                            dept_name = dept.dept_name,
                            username = (ue.username == null ? String.Empty : ue.username)
                        }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
            return View();
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addSheProject(int? id)
        {
            var has = db.monthly_she_contractor.ToList();
            ViewData["users"] = has;

            if (id != null)
            {
                ViewBag.model = db.monthly_project_she_report.Find(id);
                ViewBag.activities = db.monthly_project_activity.Where(p => p.id_monthly_project == id).ToList();
                ViewBag.outstandings = db.monthly_project_outstanding_activity.Where(p => p.id_monthly_project == id).ToList();
            }

            return PartialView();
        }

        public JsonResult Add(monthly_project_she_report mpsr, IList<string> activities, IList<string> outstanding)
        {
            List<monthly_project_she_report> prev_mpsr = db.monthly_project_she_report.Where(p => p.month_year < mpsr.month_year && p.month_year.Value.Year == mpsr.month_year.Value.Year && p.contractor_id == mpsr.contractor_id && p.no_contract == mpsr.no_contract).ToList();

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

            foreach (string s in activities)
            {
                monthly_project_activity mpa = new monthly_project_activity
                {
                    id_monthly_project = mpsr.id,
                    activity = s
                };

                db.monthly_project_activity.Add(mpa);
                db.SaveChanges();
            }

            foreach (string s in outstanding)
            {
                monthly_project_outstanding_activity mpa = new monthly_project_outstanding_activity
                {
                    id_monthly_project = mpsr.id,
                    activity = s
                };

                db.monthly_project_outstanding_activity.Add(mpa);
                db.SaveChanges();
            }

            return Json(true);
        }

        public JsonResult Edit(monthly_project_she_report mpsr, IList<string> activities, IList<string> outstanding)
        {
            List<monthly_project_she_report> prev_mpsr = db.monthly_project_she_report.Where(p => p.month_year < mpsr.month_year && p.month_year.Value.Year == mpsr.month_year.Value.Year && p.contractor_id == mpsr.contractor_id && p.no_contract == mpsr.no_contract).ToList();
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
            db.Entry(mpsr).State = EntityState.Modified;
            db.SaveChanges();

            List<monthly_project_activity> lmpa = db.monthly_project_activity.Where(p => p.id_monthly_project == mpsr.id).ToList();
            foreach (monthly_project_activity mpa in lmpa)
            {
                db.monthly_project_activity.Remove(mpa);
                db.SaveChanges();
            }

            foreach (string s in activities)
            {
                monthly_project_activity mpa = new monthly_project_activity
                {
                    id_monthly_project = mpsr.id,
                    activity = s
                };

                db.monthly_project_activity.Add(mpa);
                db.SaveChanges();
            }

            foreach (string s in outstanding)
            {
                monthly_project_outstanding_activity mpa = new monthly_project_outstanding_activity
                {
                    id_monthly_project = mpsr.id,
                    activity = s
                };

                db.monthly_project_outstanding_activity.Add(mpa);
                db.SaveChanges();
            }

            List<monthly_project_she_report> next_mpsr = db.monthly_project_she_report.Where(p => p.month_year > mpsr.month_year && p.month_year.Value.Year == mpsr.month_year.Value.Year && p.contractor_id == mpsr.contractor_id && p.no_contract == mpsr.no_contract).ToList();
            foreach (monthly_project_she_report mm in next_mpsr)
            {
                mm.incident_minor_ytd = mpsr.incident_minor_ytd + mm.incident_minor_total;
                mm.incident_minor_ytd_cost = mpsr.incident_minor_ytd_cost + mm.incident_minor_cost;
                mm.incident_moderate_ytd = mpsr.incident_moderate_ytd + mm.incident_moderate_total;
                mm.incident_moderate_ytd_cost = mpsr.incident_moderate_ytd_cost + mm.incident_moderate_cost;
                mm.incident_serious_ytd = mpsr.incident_serious_ytd + mm.incident_serious_total;
                mm.incident_serious_ytd_cost = mpsr.incident_serious_ytd_cost + mm.incident_serious_cost;
                mm.incident_major_ytd = mpsr.incident_major_ytd + mm.incident_major_total;
                mm.incident_major_ytd_cost = mpsr.incident_major_ytd_cost + mm.incident_major_cost;
                mm.toolbox_meeting_ytd = mpsr.toolbox_meeting_ytd + mm.toolbox_meeting_total;
                mm.weekly_she_meeting_ytd = mpsr.weekly_she_meeting_ytd + mm.weekly_she_meeting_total;
                mm.monthly_contr_mig_ytd = mpsr.monthly_contr_mig_ytd + mm.monthly_contr_mig_total;
                mm.environmental_loss_ytd = mpsr.environmental_loss_ytd + mm.environmental_loss_total;
                mm.environmental_loss_ytd_cost = mpsr.environmental_loss_ytd_cost + mm.environmental_loss_cost;
                mm.she_observation_card_ytd = mpsr.she_observation_card_ytd + mm.she_observation_card_total;
                mm.property_damage_ytd = mpsr.property_damage_ytd + mm.property_damage_total;
                mm.property_damage_ytd_cost = mpsr.property_damage_ytd_cost + mm.property_damage_cost;
                mm.new_jsa_hira_ytd = mpsr.new_jsa_hira_ytd + mm.new_jsa_hira_total;
                mm.process_loss_ytd = mpsr.process_loss_ytd + mm.process_loss_total;
                mm.process_loss_ytd_cost = mpsr.process_loss_ytd_cost + mm.process_loss_cost;
                mm.ptw_issued_ytd = mpsr.ptw_issued_ytd + mm.ptw_issued_total;
                mm.external_relation_ytd = mpsr.external_relation_ytd + mm.external_relation_total;
                mm.external_relation_ytd_cost = mpsr.external_relation_ytd_cost + mm.external_relation_cost;
                mm.theft_crime_ytd = mpsr.theft_crime_ytd + mm.theft_crime_total;
                mm.theft_crime_ytd_cost = mpsr.theft_crime_ytd_cost + mm.theft_crime_cost;
                mm.facility_inspection_ytd = mpsr.facility_inspection_ytd + mm.facility_inspection_total;
                mm.vehicular_ytd = mpsr.vehicular_ytd + mm.vehicular_total;
                mm.vehicular_ytd_cost = mpsr.vehicular_ytd_cost + mm.vehicular_cost;
                mm.vehicular_inspection_ytd = mpsr.vehicular_inspection_ytd + mm.vehicular_inspection_total;
                mm.near_miss_ytd = mpsr.near_miss_ytd + mm.near_miss_total;
                mm.near_miss_ytd_cost = mpsr.near_miss_ytd_cost + mm.near_miss_cost;
                mm.ppe_inspection_ytd = mpsr.ppe_inspection_ytd + mm.ppe_inspection_total;
                mm.lifting_eq_inspection_ytd = mpsr.lifting_eq_inspection_ytd + mm.lifting_eq_inspection_total;
                mm.man_hours_ytd = mpsr.man_hours_ytd + mm.man_hours_mh;
                mm.fire_inspection_ytd = mpsr.fire_inspection_ytd + mm.fire_inspection_total;
                mm.days_ytd = mpsr.days_ytd + mm.days_mh;
                mm.vehicle_emission_ytd = mpsr.vehicle_emission_ytd + mm.vehicle_emission_total;
                mm.incident_frequency_rate_ytd = (double)(mm.incident_major_ytd + mm.incident_minor_ytd + mm.incident_moderate_ytd + mm.incident_serious_ytd) * 200000 / (double)(mm.man_hours_ytd);
                mm.welding_eq_inspection_ytd = mpsr.welding_eq_inspection_ytd + mm.welding_eq_inspection_total;
                mm.incident_severity_rate_ytd = (double)(mm.incident_major_ytd + mm.incident_moderate_ytd + mm.incident_serious_ytd) * 200000 / (double)(mm.man_hours_ytd);
                mm.hde_inspection_ytd = mpsr.hde_inspection_ytd + mm.hde_inspection_total;
                mm.light_vehicle_travel_ytd = mpsr.light_vehicle_travel_ytd + mm.light_vehicle_travel_mh;
                mm.fire_emergency_ytd = mpsr.fire_emergency_ytd + mm.fire_emergency_total;
                mm.h2s_emergency_ytd = mpsr.h2s_emergency_ytd + mm.h2s_emergency_total;
                mm.environmental_spill_ytd = mpsr.environmental_spill_ytd + mm.environmental_spill_total;
                mm.medical_evacuation_ytd = mpsr.medical_evacuation_ytd + mm.medical_evacuation_total;
                mm.fit_for_day_ytd = mpsr.fit_for_day_ytd + mm.fit_for_day_total;
                mm.domestic_waste_ytd = mpsr.domestic_waste_ytd + mm.domestic_waste_total;
                mm.clinic_visit_ytd = mpsr.clinic_visit_ytd + mm.clinic_visit_total;
                mm.hazardous_waste_ytd = mpsr.hazardous_waste_ytd + mm.hazardous_waste_total;
                mm.no_work_illness_ytd = mpsr.no_work_illness_ytd + mm.no_work_illness_total;
                mm.new_msds_ytd = mpsr.new_msds_ytd + mm.new_msds_total;
                mm.ill_monitoring_ytd = mpsr.ill_monitoring_ytd + mm.ill_monitoring_total;
                mm.lti_ytd = mpsr.lti_ytd + (mm.last_date_time_lti != null ? 1 : 0);

                db.Entry(mm).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(true);
        }

        //
        // Ajax select binding she observation report
        [GridAction]
        public ActionResult _SelectAjaxSheProject()
        {
            return bindingSheProject();
        }

        //select data incident she observation report
        private ViewResult bindingSheProject()
        {
            List<monthly_project_she_report> f = db.monthly_project_she_report.ToList();
            foreach (monthly_project_she_report mpsr in f)
            {
                monthly_she_contractor e = db.monthly_she_contractor.Find(mpsr.contractor_id != null ? mpsr.contractor_id : 0);
                mpsr.contractor_name = e != null ? e.name : "";
            }
            return View(new GridModel<monthly_project_she_report>
            {
                Data = f.OrderBy(p => p.contractor_id).ThenByDescending(p => p.month_year)
            });
        }

        public JsonResult FindContract(string no_contract)
        {
            monthly_project_she_report mpsr = db.monthly_project_she_report.Where(p => p.no_contract == no_contract).FirstOrDefault();

            if (mpsr == null)
            {
                return Json(null);
            }
            else
            {
                return Json(new { period_start = mpsr.period_start.Value.ToShortDateString(), period_end = mpsr.period_end.Value.ToShortDateString(), project_name = mpsr.project_name, project_location = mpsr.project_location, project_manager = mpsr.project_manager, contract_supervisor = mpsr.contract_supervisor, project_she_representative = mpsr.project_she_representative, se_she_representative = mpsr.se_she_representative });
            }
        }

        public ActionResult ProjectReport()
        {
            return View();
        }

    }
}
