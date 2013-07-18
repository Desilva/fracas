using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class FieldManagerSummaryController : Controller
    {
        //
        // GET: /FieldManagerSummary/

        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/FieldManagerSummary" });
            }
            var has = (from users in db.users
                       select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
            return View();
        }

        public ActionResult sheKPI()
        {
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
            return PartialView();
        }

        public ActionResult dailyLogKPI()
        {
            DateTime now = DateTime.Today.AddDays(-1);

            List<daily_log_weekly_target> list_wt_day = db.daily_log_weekly_target.Where(p => p.date == now).ToList();
            double today_target = 0;
            foreach (daily_log_weekly_target wt_day in list_wt_day)
            {
                today_target += Double.Parse(wt_day.target_unit_1) + Double.Parse(wt_day.target_unit_2);
            }
            ViewBag.today_target = today_target;
            List<daily_log> list_dl_day = db.daily_log.Where(p => p.date == now).ToList();
            double today_achievement = 0;
            foreach (daily_log dl_day in list_dl_day)
            {
                today_achievement += Double.Parse(dl_day.achievement_1 == null ? "0" : dl_day.achievement_1) + Double.Parse(dl_day.achievement_2 == null ? "0" : dl_day.achievement_2);
            }
            ViewBag.today_achievement = today_achievement;
            ViewBag.date_day = now;

            now = DateTime.Today;
            DateTime StartOfWeek = now.AddDays(-(int)now.DayOfWeek > 5 ? -9 : ((int)now.DayOfWeek < 5 ? -(int)now.DayOfWeek - 10 : -8));
            DateTime LastOfWeek = StartOfWeek.AddDays(6);

            List<daily_log_weekly_target> list_wt_week = db.daily_log_weekly_target.Where(p => p.date >= StartOfWeek && p.date <= LastOfWeek).ToList();
            double week_target = 0;
            foreach (daily_log_weekly_target wt_week in list_wt_week)
            {
                week_target += Double.Parse(wt_week.target_unit_1) + Double.Parse(wt_week.target_unit_2);
            }
            ViewBag.week_target = week_target;
            List<daily_log> list_dl_week = db.daily_log.Where(p => p.date >= StartOfWeek && p.date <= LastOfWeek).ToList();
            double week_achievement = 0;
            foreach (daily_log dl_week in list_dl_week)
            {
                week_achievement += Double.Parse(dl_week.achievement_1 == null ? "0" : dl_week.achievement_1) + Double.Parse(dl_week.achievement_2 == null ? "0" : dl_week.achievement_2);
            }
            ViewBag.week_achievement = week_achievement;
            ViewBag.date_week_start = StartOfWeek;
            ViewBag.date_week_last = LastOfWeek;

            now = DateTime.Today;
            int month = DateTime.Today.Month - 1;

            List<daily_log_weekly_target> list_wt_month = db.daily_log_weekly_target.Where(p => p.date.Value.Month == month).ToList();
            double month_target = 0;
            foreach (daily_log_weekly_target wt_month in list_wt_month)
            {
                month_target += Double.Parse(wt_month.target_unit_1 == null ? "0" : wt_month.target_unit_1) + Double.Parse(wt_month.target_unit_2 == null ? "0" : wt_month.target_unit_2);
            }
            ViewBag.month_target = month_target;
            List<daily_log> list_dl_month = db.daily_log.Where(p => p.date.Value.Month == month).ToList();
            double month_achievement = 0;
            foreach (daily_log dl_month in list_dl_month)
            {
                month_achievement += Double.Parse(dl_month.achievement_1 == null ? "0" : dl_month.achievement_1) + Double.Parse(dl_month.achievement_2 == null ? "0" : dl_month.achievement_2);
            }
            ViewBag.month_achievement = month_achievement;
            ViewBag.date_month = now.AddMonths(-1);

            daily_log_wpnb current = db.daily_log_wpnb.Where(p => p.month == month && p.year == now.Year).ToList().LastOrDefault();
            ViewBag.month_wpnb_target = current == null ? 0 : Double.Parse(current.target == null ? "0" : current.target);
            ViewBag.month_wpnb_achievement = current == null ? 0 : Double.Parse(current.achievement == null ? "0" : current.achievement);
            Debug.Write("a");
            return PartialView();
        }

        public ActionResult PirKPI()
        {
            List<pir> list_pir = db.pirs.Where(p => p.date_rise.Value.Year == DateTime.Today.Year).ToList();
            int totalPir = list_pir.Count;
            int totalOverdue = list_pir.Where(a => a.target_completion_init < DateTime.Today).ToList().Count;
            double pir_target = db.she_KPI_target.Find(2).target != null ? db.she_KPI_target.Find(2).target.Value : 0;

            Debug.WriteLine(((double)totalOverdue / (double)totalPir) * (double)100);
            if (((double)totalOverdue / (double)totalPir) * (double)100 <= pir_target)
            {
                ViewBag.success = true;
            }
            else
            {
                ViewBag.success = false;

            }
            ViewBag.totalPir = totalPir;
            ViewBag.totalOverdue = totalOverdue;
            ViewBag.pir_target = pir_target;
            return PartialView();
        }

        public ActionResult sheObsReport()
        {
            return PartialView();
        }

        public ActionResult pirReport()
        {
            return PartialView();
        }
    }
}
