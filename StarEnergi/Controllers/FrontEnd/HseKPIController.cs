using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class HseKPIController : Controller
    {
        //
        // GET: /HseKPI/

        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseKPI" });
            }
            var has = (from users in db.users
                       select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
            ViewBag.obs_target = db.she_KPI_target.Find(1).target;
            ViewBag.ir_target = db.she_KPI_target.Find(3).target;

            DateTime prev = DateTime.Today.AddYears(-1);
            List<she_observation> list_she_obs_bef = db.she_observation.Where(p => p.date_time.Value.Year == prev.Year).ToList();
            List<she_observation> list_she_obs_aft = db.she_observation.Where(p => p.date_time.Value.Year == DateTime.Today.Year).ToList();
            List<incident_report> list_ir = db.incident_report.Where(p => p.date_incident.Value.Year == DateTime.Today.Year && p.investigation == 1).ToList();

            int totalAllCardBef = list_she_obs_bef.Count;
            int totalAllCardAft = totalAllCardBef + totalAllCardBef * (int)ViewBag.obs_target / 100;
            int totalIRBef = list_ir.Count;

            int totalCardBef = list_she_obs_bef.Where(p => p.date_time.Value.Month >= 1 && p.date_time.Value.Month <= prev.Month).ToList().Count;
            int totalCardAft = list_she_obs_aft.Where(p => p.date_time.Value.Month >= 1 && p.date_time.Value.Month <= prev.Month).ToList().Count;

            int averageBef = totalCardBef / prev.Month;
            int averageAft = totalCardAft / prev.Month;

            int totalIRHuman = 0;
            int totalIROps = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.id_rca != null)
                {
                    DateTime? complete_date = db.rcas.Find(ir.id_rca).publish_date;
                    Debug.WriteLine(complete_date);
                    if (complete_date != null)
                    {
                        if (ir.incident_type == "Injury / Illness")
                        {
                            totalIRHuman += complete_date.Value.CompareTo(ir.date_incident) <= 2 ? 1 : 0;
                        }
                        else
                        {
                            totalIROps += complete_date.Value.CompareTo(ir.date_incident) <= 16 ? 1 : 0;
                        }
                    }
                }
            }

            ViewBag.totalAllCardBef = totalAllCardBef;
            ViewBag.totalAllCardAft = totalAllCardAft;
            ViewBag.totalCardBef = totalCardBef;
            ViewBag.totalCardAft = totalCardAft;
            ViewBag.averageBef = averageBef;
            ViewBag.averageAft = averageAft;

            ViewBag.last_year = prev.Year;
            ViewBag.current_year = DateTime.Today.Year;
            ViewBag.start_month = 1;
            ViewBag.end_month = prev.Month;

            ViewBag.totalIRBef = totalIRBef;
            ViewBag.totalIRHuman = totalIRHuman;
            ViewBag.totalIROps = totalIROps;

            ViewBag.success = totalCardAft >= totalCardBef + totalCardBef * (int)ViewBag.obs_target / 100;
            ViewBag.successIR = totalIRBef == 0 ? true : (totalIRHuman + totalIROps) * 100 / totalIRBef >= (int)ViewBag.ir_target;
            return View();
        }

        [HttpPost]
        public ActionResult obsTarget()
        {
            return Json(new { success = true, target = db.she_KPI_target.Find(1).target });

        }

        [HttpPost]
        public ActionResult setObsTarget(int target)
        {
            she_KPI_target kp = db.she_KPI_target.Find(1);
            double? prev_target = kp.target;
            kp.target = target;
            db.Entry(kp).State = EntityState.Modified;
            db.SaveChanges();

            she_obs_KPI_log log = new she_obs_KPI_log
            {
                from = (int)prev_target,
                to = (int)target,
                username = Session["username"].ToString(),
                date = DateTime.Now
            };
            db.she_obs_KPI_log.Add(log);
            db.SaveChanges();
            DateTime prev = DateTime.Today.AddYears(-1);
            List<she_observation> list_she_obs_bef = db.she_observation.Where(p => p.date_time.Value.Year == prev.Year).ToList();
            List<she_observation> list_she_obs_aft = db.she_observation.Where(p => p.date_time.Value.Year == DateTime.Today.Year).ToList();

            int totalCardBef = list_she_obs_bef.Where(p => p.date_time.Value.Month >= 1 && p.date_time.Value.Month == prev.Month).ToList().Count;
            int totalCardAft = list_she_obs_aft.Where(p => p.date_time.Value.Month >= 1 && p.date_time.Value.Month == prev.Month).ToList().Count;

            bool success = totalCardAft >= totalCardBef + totalCardBef * (int)target / 100;
            return Json(new { success = true, successFail = success });

        }

        [HttpPost]
        public ActionResult IRTarget()
        {
            return Json(new { success = true, target = db.she_KPI_target.Find(3).target });

        }

        [HttpPost]
        public ActionResult setIRTarget(int target)
        {
            she_KPI_target kp = db.she_KPI_target.Find(3);
            double? prev_target = kp.target;
            kp.target = target;
            db.Entry(kp).State = EntityState.Modified;
            db.SaveChanges();

            ir_KPI_log log = new ir_KPI_log
            {
                from = (int)prev_target,
                to = (int)target,
                username = Session["username"].ToString(),
                date = DateTime.Now
            };
            db.ir_KPI_log.Add(log);
            db.SaveChanges();
            List<incident_report> list_ir = db.incident_report.Where(p => p.date_incident.Value.Year == DateTime.Today.Year && p.investigation == 1).ToList();
            int totalIRBef = list_ir.Count;
            int totalIRHuman = 0;
            int totalIROps = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.id_rca != null)
                {
                    DateTime? complete_date = db.rcas.Find(ir.id_rca).publish_date;
                    if (complete_date != null)
                    {
                        if (ir.incident_type == "Injury / Illness")
                        {
                            totalIRHuman += complete_date.Value.CompareTo(ir.date_incident) <= 2 ? 1 : 0;
                        }
                        else
                        {
                            totalIROps += complete_date.Value.CompareTo(ir.date_incident) <= 16 ? 1 : 0;
                        }
                    }
                }
            }
            bool success = totalIRBef == 0 ? true : (totalIRHuman + totalIROps) * 100 / totalIRBef >= target;
            return Json(new { success = true, successFail = success });

        }

        public ActionResult Log(int? type)
        {
            if (type == 1)
            {
                List<she_obs_KPI_log> log = db.she_obs_KPI_log.ToList();
                foreach (she_obs_KPI_log i in log)
                {
                    int? employee_id = db.users.Find(i.username).employee_id;
                    employee e = db.employees.Find(employee_id);
                    i.username = e.alpha_name;
                }
                ViewData["log"] = log;
            }
            else if (type == 2)
            {
                List<ir_KPI_log> log = db.ir_KPI_log.ToList();
                foreach (ir_KPI_log i in log)
                {
                    int? employee_id = db.users.Find(i.username).employee_id;
                    employee e = db.employees.Find(employee_id);
                    i.username = e.alpha_name;
                }
                ViewData["log"] = log;
            }
            return PartialView();
        }

    }
}
