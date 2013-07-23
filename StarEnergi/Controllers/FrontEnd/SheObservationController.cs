using ReportManagement;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class SheObservationController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;
        //
        // GET: /HseObservationForm/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/SheObservation" });
            }
            ViewBag.Nama = "SHE Observation Form";
            return View();
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addSheObservation(int? id)
        {
            string username = Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList(); 
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
                           dept_name = employees.department != null ? employees.department : dept.dept_name,
                           dept_id = employees.dept_id,
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewBag.user = has;

            ViewBag.list_equipment = db.equipments.ToList();
            if (id != null)
            {
                if (li.Exists(p => p.role == (int)Config.role.ADMINMASTERSHE))
                {
                    ViewBag.mod = id;
                    ViewBag.datas = db.she_observation.Find(id);
                }
                else
                {
                    return DetailSheObservation(id.Value);
                }
            }
            return PartialView();
        }

        //
        // Ajax select binding she observation report
        [GridAction]
        public ActionResult _SelectAjaxSheObservation()
        {
            return bindingSheObservation();
        }

        //select data incident she observation report
        private ViewResult bindingSheObservation()
        {
            string username = Session["username"].ToString();
            int id = int.Parse(Session["id"].ToString());
            li = db.user_per_role.Where(p => p.username == username).ToList();
            string name = db.employees.Find(id).alpha_name;
            List<she_observation> f = new List<she_observation>();
            if (li.Exists(p => p.role == (int)Config.role.ADMINMASTERSHE))
            {
                f = db.she_observation.ToList();
            }
            else
            {
                f = db.she_observation.Where(p => p.observer == name).ToList();
            }
            

            return View(new GridModel<she_observation>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(she_observation sheObservation)
        {
            db.she_observation.Add(sheObservation);
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(she_observation sheObservation)
        {
            she_observation so = db.she_observation.Find(sheObservation.id);

            so.she_obs = sheObservation.she_obs;
            so.she_ins = sheObservation.she_ins;
            so.accident_unsafe = sheObservation.accident_unsafe;
            so.accident_safe = sheObservation.accident_safe;
            so.fire_unsafe = sheObservation.fire_unsafe;
            so.fire_safe = sheObservation.fire_safe;
            so.prevention_unsafe = sheObservation.prevention_unsafe;
            so.prevention_safe = sheObservation.prevention_safe;
            so.supervisor_unsafe = sheObservation.supervisor_unsafe;
            so.supervisor_safe = sheObservation.supervisor_safe;
            so.fit_work_unsafe = sheObservation.fit_work_unsafe;
            so.fit_work_safe = sheObservation.fit_work_safe;
            so.psychological_unsafe = sheObservation.psychological_unsafe;
            so.psychological_safe = sheObservation.psychological_safe;
            so.posture_unsafe = sheObservation.posture_unsafe;
            so.posture_safe = sheObservation.posture_safe;
            so.substance_unsafe = sheObservation.substance_unsafe;
            so.substance_safe = sheObservation.substance_safe;
            so.hygiene_unsafe = sheObservation.hygiene_unsafe;
            so.hygiene_safe = sheObservation.hygiene_safe;
            so.house_unsafe = sheObservation.house_unsafe;
            so.house_safe = sheObservation.house_safe;
            so.standard_unsafe = sheObservation.standard_unsafe;
            so.standard_safe = sheObservation.standard_safe;
            so.spill_unsafe = sheObservation.spill_unsafe;
            so.spill_safe = sheObservation.spill_safe;
            so.waste_energy_unsafe = sheObservation.waste_energy_unsafe;
            so.waste_energy_safe = sheObservation.waste_energy_safe;
            so.containment_unsafe = sheObservation.containment_unsafe;
            so.containment_safe = sheObservation.containment_safe;
            so.absorbent_unsafe = sheObservation.absorbent_unsafe;
            so.absorbent_safe = sheObservation.absorbent_safe;
            so.hira_unsafe = sheObservation.hira_unsafe;
            so.hira_safe = sheObservation.hira_safe;
            so.ptw_unsafe = sheObservation.ptw_unsafe;
            so.ptw_safe = sheObservation.ptw_safe;
            so.sop_unsafe = sheObservation.sop_unsafe;
            so.sop_safe = sheObservation.sop_safe;
            so.msds_unsafe = sheObservation.msds_unsafe;
            so.msds_safe = sheObservation.msds_safe;
            so.emergency_unsafe = sheObservation.emergency_unsafe;
            so.emergency_safe = sheObservation.emergency_safe;
            so.certificates_unsafe = sheObservation.certificates_unsafe;
            so.certificates_safe = sheObservation.certificates_safe;
            so.ppe_unsafe = sheObservation.ppe_unsafe;
            so.ppe_safe = sheObservation.ppe_safe;
            so.hand_unsafe = sheObservation.hand_unsafe;
            so.hand_safe = sheObservation.hand_safe;
            so.mechanical_unsafe = sheObservation.mechanical_unsafe;
            so.mechanical_safe = sheObservation.mechanical_safe;
            so.electrical_unsafe = sheObservation.electrical_unsafe;
            so.electrical_safe = sheObservation.electrical_safe;
            so.vehicular_unsafe = sheObservation.vehicular_unsafe;
            so.vehicular_safe = sheObservation.vehicular_safe;
            so.substandard_unsafe = sheObservation.substandard_unsafe;
            so.substandard_safe = sheObservation.substandard_safe;
            so.h2s_unsafe = sheObservation.h2s_unsafe;
            so.h2s_safe = sheObservation.h2s_safe;
            so.workplace_health_unsafe = sheObservation.workplace_health_unsafe;
            so.workplace_health_safe = sheObservation.workplace_health_safe;
            so.date_time = sheObservation.date_time;
            so.observer = sheObservation.observer;
            so.department = sheObservation.department;
            so.location = sheObservation.location;
            so.activity = sheObservation.activity;
            so.safe_observed = sheObservation.safe_observed;
            so.safe_observeds = sheObservation.safe_observeds;
            so.action_encourages = sheObservation.action_encourages;
            so.action_encourage = sheObservation.action_encourage;
            so.unsafe_observed = sheObservation.unsafe_observed;
            so.unsafe_observeds = sheObservation.unsafe_observeds;
            so.immediate_corrective_act = sheObservation.immediate_corrective_act;
            so.immediate_corrective_acts = sheObservation.immediate_corrective_acts;
            so.action_prevents = sheObservation.action_prevents;
            so.action_prevent = sheObservation.action_prevent;
            so.type_equipment = sheObservation.type_equipment;
            so.equipment_employee = sheObservation.equipment_employee;
            so.equipment_id = sheObservation.equipment_id;
            so.employee_id = sheObservation.employee_id;

            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        //
        // Ajax delete binding she observation report
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxSheObservation(int id)
        {
            deleteSheObservation(id);
            return bindingSheObservation();
        }

        //delete data she observation report
        private void deleteSheObservation(int id)
        {
            she_observation ir = db.she_observation.Find(id);
            db.she_observation.Remove(ir);
            db.SaveChanges();
        }

        public ActionResult DetailSheObservation(int id)
        {
            she_observation details = new she_observation();
            details = db.she_observation.Find(id);
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
                           dept_name = employees.department != null ? employees.department : dept.dept_name,
                           dept_id = employees.dept_id,
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewBag.user = has;
            string username = Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();

            ViewBag.list_equipment = db.equipments.ToList();
            ViewBag.nama = "Detail She Observation";
            return PartialView(details);
        }

        public ActionResult printSheObservation(int id)
        {
            she_observation so = db.she_observation.Find(id);
            return this.ViewPdf("", "sheObservationPrint", so);
        }

        [HttpPost]
        public JsonResult QualityReview(int id, byte is_quality)
        {
            she_observation so = db.she_observation.Find(id);

            so.is_quality = is_quality;
            so.is_review = 1;

            db.Entry(so).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

    }
}
