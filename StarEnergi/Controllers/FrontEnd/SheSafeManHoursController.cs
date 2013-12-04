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
    public class SheSafeManHoursController : Controller
    {
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /SheSafeManHours/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/SheSafeManHours" });
            }

            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();

            if (!li.Exists(p => p.role == (int)Config.role.ADMINMASTERSHE))
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/SheSafeManHours" });
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
            ViewData["user_role"] = li;

            safe_man_hours_incident smhi = db.safe_man_hours_incident.OrderBy(p => p.date).ToList().LastOrDefault();

            ViewBag.date_incident = (smhi != null ? smhi.date : null);
            return View();
        }

        public ActionResult report()
        {
            return PartialView();
        }

        //
        // Ajax select binding safe man hours
        [GridAction]
        public ActionResult _SelectAjaxSheSafeManHours()
        {
            return bindingSheSafeManHours();
        }

        //select data safe man hours
        private ViewResult bindingSheSafeManHours()
        {
            List<safe_man_hours> f = db.safe_man_hours.ToList();
            return View(new GridModel<safe_man_hours>
            {
                Data = f.OrderByDescending(p => p.month)
            });
        }

        public ActionResult addSheSafeManHours(int? id, int? month, int? year, int? cont_total, int? cont_mh)
        {
            safe_man_hours_incident smhi = db.safe_man_hours_incident.OrderBy(p => p.date).ToList().LastOrDefault();
            if (smhi != null)
            {
                ViewBag.incident = smhi.date;
            }

            ViewBag.month = month;
            ViewBag.year = year;
            ViewBag.cont_total = cont_total;
            ViewBag.cont_mh = cont_mh;

            if (id != null)
            {
                ViewBag.model = db.safe_man_hours.Find(id);
            }

            return PartialView();
        }

        public JsonResult Add(safe_man_hours smh)
        {
            int month = smh.month.Value.Month;
            int year = smh.month.Value.Year;
            int day = 0;
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12) {
                day = 31;
            } else if (month == 4 || month == 6 || month == 9 || month == 11) {
                day = 30;
            } else {
                if ((year % 400 == 0) || (year % 4 == 0 && year % 100 != 0)) {
                    day = 29;
                } else {
                    day = 28;
                }
            }


            List<safe_man_hours_incident> list_smhi = db.safe_man_hours_incident.Where(p => p.date.Value.Month == smh.month.Value.Month && p.date.Value.Year == smh.month.Value.Year).ToList();
            if (list_smhi.Count > 0)
            {
                DateTime last_day = new DateTime(year, month, day);
                smh.days_smh = (int)((last_day - list_smhi.LastOrDefault().date.Value).TotalDays);
                smh.seg_emp_smh = smh.seg_total;
                smh.cont_emp_smh = smh.cont_total;
                smh.seg_mh_smh = smh.seg_total_work_hr * smh.days_smh / day;
                smh.cont_mh_smh = smh.cont_total_work_hr * smh.days_smh / day;
                smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
            }
            else
            {
                long smh_emp_seg = 0;
                long smh_emp_cont = 0;
                long smh_mh_seg = 0;
                long smh_mh_cont = 0;
                safe_man_hours_incident smhi = db.safe_man_hours_incident.Where(p => ((p.date.Value.Month < smh.month.Value.Month && p.date.Value.Year == smh.month.Value.Year) || p.date.Value.Year < smh.month.Value.Year)).OrderBy(p => p.date).ToList().LastOrDefault();
                if (smhi != null)
                {
                    List<safe_man_hours> list_smh = db.safe_man_hours.Where(p => ((p.month.Value.Month >= smhi.date.Value.Month && p.month.Value.Year == smhi.date.Value.Year) || p.month.Value.Year > smhi.date.Value.Year) && ((p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year < smh.month.Value.Year)).OrderBy(p => p.month).ToList();
                    foreach (safe_man_hours smhs in list_smh)
                    {
                        if (smhs.month.Value.Month == smhi.date.Value.Month && smhs.month.Value.Year == smhi.date.Value.Year)
                        {
                            smh_emp_seg += smhs.seg_emp_smh.Value;
                            smh_emp_cont += smhs.cont_emp_smh.Value;
                            smh_mh_seg += smhs.seg_mh_smh.Value;
                            smh_mh_cont += smhs.cont_mh_smh.Value;
                        }
                        else
                        {
                            smh_emp_seg += smhs.seg_total.Value;
                            smh_emp_cont += smhs.cont_total.Value;
                            smh_mh_seg += smhs.seg_total_work_hr.Value;
                            smh_mh_cont += smhs.cont_total_work_hr.Value;
                        }
                    }

                    smh.seg_emp_smh = smh_emp_seg + smh.seg_total;
                    smh.cont_emp_smh = smh_emp_cont + smh.cont_total;
                    smh.seg_mh_smh = smh_mh_seg + smh.seg_total_work_hr;
                    smh.cont_mh_smh = smh_mh_cont + smh.cont_total_work_hr;
                    smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                    smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
                    DateTime last_day = new DateTime(year, month, day);
                    smh.days_smh = (int)((last_day - smhi.date.Value).TotalDays);
                }
                else
                {
                    List<safe_man_hours> list_smh = db.safe_man_hours.Where(p => ((p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year < smh.month.Value.Year)).OrderBy(p => p.month).ToList();
                    foreach (safe_man_hours smhs in list_smh)
                    {
                        smh_emp_seg += smhs.seg_total.Value;
                        smh_emp_cont += smhs.cont_total.Value;
                        smh_mh_seg += smhs.seg_total_work_hr.Value;
                        smh_mh_cont += smhs.cont_total_work_hr.Value;
                    }

                    Debug.WriteLine(list_smh.Count);

                    smh.seg_emp_smh = smh_emp_seg + smh.seg_total;
                    smh.cont_emp_smh = smh_emp_cont + smh.cont_total;
                    smh.seg_mh_smh = smh_mh_seg + smh.seg_total_work_hr;
                    smh.cont_mh_smh = smh_mh_cont + smh.cont_total_work_hr;
                    smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                    smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
                    DateTime last_day = new DateTime(year, month, day);
                    smh.days_smh = (int)((last_day - (list_smh.Count == 0 ? smh.month.Value.AddDays(-1) : list_smh.FirstOrDefault().month.Value.AddDays(-1))).TotalDays);
                }
            }

            long ytd_emp_seg = 0;
            long ytd_emp_cont = 0;
            long ytd_mh_seg = 0;
            long ytd_mh_cont = 0;
            List<safe_man_hours> list_smhs = db.safe_man_hours.Where(p => p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year).OrderBy(p => p.month).ToList();
            foreach (safe_man_hours smhs in list_smhs)
            {
                ytd_emp_seg += smhs.seg_total.Value;
                ytd_emp_cont += smhs.cont_total.Value;
                ytd_mh_seg += smhs.seg_total_work_hr.Value;
                ytd_mh_cont += smhs.cont_total_work_hr.Value;
            }

            smh.seg_emp_ytd = ytd_emp_seg + smh.seg_total;
            smh.cont_emp_ytd = ytd_emp_cont + smh.cont_total;
            smh.seg_mh_ytd = ytd_mh_seg + smh.seg_total_work_hr;
            smh.cont_mh_ytd = ytd_mh_cont + smh.cont_total_work_hr;
            smh.emp_total_ytd = smh.seg_emp_ytd + smh.cont_emp_ytd;
            smh.mh_total_ytd = smh.seg_mh_ytd + smh.cont_mh_ytd;

            bool isCount = true;
            long seg_emp_smh = smh.seg_emp_smh.Value;
            long cont_emp_smh = smh.cont_emp_smh.Value;
            long seg_mh_smh = smh.seg_mh_smh.Value;
            long cont_mh_smh = smh.cont_mh_smh.Value;
            long seg_emp_ytd = smh.seg_emp_ytd.Value;
            long cont_emp_ytd = smh.cont_emp_ytd.Value;
            long seg_mh_ytd = smh.seg_mh_ytd.Value;
            long cont_mh_ytd = smh.cont_mh_ytd.Value;
            long days_smh = smh.days_smh.Value;
            list_smhs = db.safe_man_hours.Where(p => (p.month.Value.Month > smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year > smh.month.Value.Year).OrderBy(p => p.month).ToList();
            foreach (safe_man_hours smhs in list_smhs)
            {
                if (smhs.month.Value.Year == smh.month.Value.Year)
                {
                    smhs.seg_emp_ytd = smh.seg_emp_ytd + smhs.seg_total;
                    smhs.cont_emp_ytd = smh.cont_emp_ytd + smhs.cont_total;
                    smhs.seg_mh_ytd = smh.seg_mh_ytd + smhs.seg_total_work_hr;
                    smhs.cont_mh_ytd = smh.cont_mh_ytd + smhs.cont_total_work_hr;
                    smhs.emp_total_ytd = smhs.seg_emp_ytd + smhs.cont_emp_ytd;
                    smhs.mh_total_ytd = smhs.seg_mh_ytd + smhs.cont_mh_ytd;
                }

                if (db.safe_man_hours_incident.Where(p => p.date.Value.Month == smhs.month.Value.Month && p.date.Value.Year == smhs.month.Value.Year).Count() > 0)
                {
                    isCount = false;
                }

                if (isCount)
                {
                    smhs.seg_emp_smh = smh.seg_emp_smh + smhs.seg_total;
                    smhs.cont_emp_smh = smh.cont_emp_smh + smhs.cont_total;
                    smhs.seg_mh_smh = smh.seg_mh_smh + smhs.seg_total_work_hr;
                    smhs.cont_mh_smh = smh.cont_mh_smh + smhs.cont_total_work_hr;
                    smhs.emp_total_smh = smhs.seg_emp_smh + smhs.cont_emp_smh;
                    smhs.mh_total_smh = smhs.seg_mh_smh + smhs.cont_mh_smh;
                    DateTime last_day = new DateTime(smhs.month.Value.Year, smhs.month.Value.Month, getDay(smhs.month.Value.Year, smhs.month.Value.Month));
                    smhs.days_smh = smh.days_smh + (int)((last_day - smhs.month.Value.AddDays(-1)).TotalDays);
                }

                db.Entry(smhs).State = EntityState.Modified;
                db.SaveChanges();

                smh.seg_emp_smh = smhs.seg_emp_smh;
                smh.cont_emp_smh = smhs.cont_emp_smh;
                smh.seg_mh_smh = smhs.seg_mh_smh;
                smh.cont_mh_smh = smhs.cont_mh_smh;
                smh.seg_emp_ytd = smhs.seg_emp_ytd;
                smh.cont_emp_ytd = smhs.cont_emp_ytd;
                smh.seg_mh_ytd = smhs.seg_mh_ytd;
                smh.cont_mh_ytd = smhs.cont_mh_ytd;
                smh.days_smh = smhs.days_smh;
            }

            smh.seg_emp_smh = seg_emp_smh;
            smh.cont_emp_smh = cont_emp_smh;
            smh.seg_mh_smh = seg_mh_smh;
            smh.cont_mh_smh = cont_mh_smh;
            smh.seg_emp_ytd = seg_emp_ytd;
            smh.cont_emp_ytd = cont_emp_ytd;
            smh.seg_mh_ytd = seg_mh_ytd;
            smh.cont_mh_ytd = cont_mh_ytd;
            smh.days_smh = days_smh;

            db.safe_man_hours.Add(smh);
            db.SaveChanges();

            return Json(new { id = smh.id }); 
        }

        public JsonResult Edit(safe_man_hours smh)
        {
            int month = smh.month.Value.Month;
            int year = smh.month.Value.Year;
            int day = 0;
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                day = 31;
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                day = 30;
            }
            else
            {
                if ((year % 400 == 0) || (year % 4 == 0 && year % 100 != 0))
                {
                    day = 29;
                }
                else
                {
                    day = 28;
                }
            }




            db.Entry(smh).State = EntityState.Modified;
            db.SaveChanges();

            List<safe_man_hours_incident> list_smhi = db.safe_man_hours_incident.Where(p => p.date.Value.Month == smh.month.Value.Month && p.date.Value.Year == smh.month.Value.Year).ToList();
            if (list_smhi.Count > 0)
            {
                DateTime last_day = new DateTime(year, month, day);
                smh.days_smh = (int)((last_day - list_smhi.LastOrDefault().date.Value).TotalDays);
                smh.seg_emp_smh = smh.seg_total;
                smh.cont_emp_smh = smh.cont_total;
                smh.seg_mh_smh = smh.seg_total_work_hr * smh.days_smh / day;
                smh.cont_mh_smh = smh.cont_total_work_hr * smh.days_smh / day;
                smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
            }
            else
            {
                long smh_emp_seg = 0;
                long smh_emp_cont = 0;
                long smh_mh_seg = 0;
                long smh_mh_cont = 0;
                safe_man_hours_incident smhi = db.safe_man_hours_incident.Where(p => ((p.date.Value.Month < smh.month.Value.Month && p.date.Value.Year == smh.month.Value.Year) || p.date.Value.Year < smh.month.Value.Year)).OrderBy(p => p.date).ToList().LastOrDefault();
                if (smhi != null)
                {
                    List<safe_man_hours> list_smh = db.safe_man_hours.Where(p => ((p.month.Value.Month >= smhi.date.Value.Month && p.month.Value.Year == smhi.date.Value.Year) || p.month.Value.Year > smhi.date.Value.Year) && ((p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year < smh.month.Value.Year)).OrderBy(p => p.month).ToList();
                    foreach (safe_man_hours smhs in list_smh)
                    {
                        if (smhs.month.Value.Month == smhi.date.Value.Month && smhs.month.Value.Year == smhi.date.Value.Year)
                        {
                            smh_emp_seg += smhs.seg_emp_smh.Value;
                            smh_emp_cont += smhs.cont_emp_smh.Value;
                            smh_mh_seg += smhs.seg_mh_smh.Value;
                            smh_mh_cont += smhs.cont_mh_smh.Value;
                        }
                        else
                        {
                            smh_emp_seg += smhs.seg_total.Value;
                            smh_emp_cont += smhs.cont_total.Value;
                            smh_mh_seg += smhs.seg_total_work_hr.Value;
                            smh_mh_cont += smhs.cont_total_work_hr.Value;
                        }
                    }

                    smh.seg_emp_smh = smh_emp_seg + smh.seg_total;
                    smh.cont_emp_smh = smh_emp_cont + smh.cont_total;
                    smh.seg_mh_smh = smh_mh_seg + smh.seg_total_work_hr;
                    smh.cont_mh_smh = smh_mh_cont + smh.cont_total_work_hr;
                    smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                    smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
                    DateTime last_day = new DateTime(year, month, day);
                    smh.days_smh = (int)((last_day - smhi.date.Value).TotalDays);
                }
                else
                {
                    List<safe_man_hours> list_smh = db.safe_man_hours.Where(p => ((p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year < smh.month.Value.Year)).OrderBy(p => p.month).ToList();
                    foreach (safe_man_hours smhs in list_smh)
                    {
                        smh_emp_seg += smhs.seg_total.Value;
                        smh_emp_cont += smhs.cont_total.Value;
                        smh_mh_seg += smhs.seg_total_work_hr.Value;
                        smh_mh_cont += smhs.cont_total_work_hr.Value;
                    }

                    smh.seg_emp_smh = smh_emp_seg + smh.seg_total;
                    smh.cont_emp_smh = smh_emp_cont + smh.cont_total;
                    smh.seg_mh_smh = smh_mh_seg + smh.seg_total_work_hr;
                    smh.cont_mh_smh = smh_mh_cont + smh.cont_total_work_hr;
                    smh.emp_total_smh = smh.seg_emp_smh + smh.cont_emp_smh;
                    smh.mh_total_smh = smh.seg_mh_smh + smh.cont_mh_smh;
                    DateTime last_day = new DateTime(year, month, day);
                    smh.days_smh = (int)((last_day - (list_smh.Count == 0 ? smh.month.Value.AddDays(-1) : list_smh.FirstOrDefault().month.Value.AddDays(-1))).TotalDays);
                }
            }

            long ytd_emp_seg = 0;
            long ytd_emp_cont = 0;
            long ytd_mh_seg = 0;
            long ytd_mh_cont = 0;
            List<safe_man_hours> list_smhs = db.safe_man_hours.Where(p => p.month.Value.Month < smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year).OrderBy(p => p.month).ToList();
            foreach (safe_man_hours smhs in list_smhs)
            {
                ytd_emp_seg += smhs.seg_total.Value;
                ytd_emp_cont += smhs.cont_total.Value;
                ytd_mh_seg += smhs.seg_total_work_hr.Value;
                ytd_mh_cont += smhs.cont_total_work_hr.Value;
            }

            smh.seg_emp_ytd = ytd_emp_seg + smh.seg_total;
            smh.cont_emp_ytd = ytd_emp_cont + smh.cont_total;
            smh.seg_mh_ytd = ytd_mh_seg + smh.seg_total_work_hr;
            smh.cont_mh_ytd = ytd_mh_cont + smh.cont_total_work_hr;
            smh.emp_total_ytd = smh.seg_emp_ytd + smh.cont_emp_ytd;
            smh.mh_total_ytd = smh.seg_mh_ytd + smh.cont_mh_ytd;

            
            bool isCount = true;
            long seg_emp_smh = smh.seg_emp_smh.Value;
            long cont_emp_smh = smh.cont_emp_smh.Value;
            long seg_mh_smh = smh.seg_mh_smh.Value;
            long cont_mh_smh = smh.cont_mh_smh.Value;
            long seg_emp_ytd = smh.seg_emp_ytd.Value;
            long cont_emp_ytd = smh.cont_emp_ytd.Value;
            long seg_mh_ytd = smh.seg_mh_ytd.Value;
            long cont_mh_ytd = smh.cont_mh_ytd.Value;
            long days_smh = smh.days_smh.Value;
            list_smhs = db.safe_man_hours.Where(p => (p.month.Value.Month > smh.month.Value.Month && p.month.Value.Year == smh.month.Value.Year) || p.month.Value.Year > smh.month.Value.Year).OrderBy(p => p.month).ToList();
            foreach (safe_man_hours smhs in list_smhs)
            {
                if (smhs.month.Value.Year == smh.month.Value.Year)
                {
                    smhs.seg_emp_ytd = smh.seg_emp_ytd + smhs.seg_total;
                    smhs.cont_emp_ytd = smh.cont_emp_ytd + smhs.cont_total;
                    smhs.seg_mh_ytd = smh.seg_mh_ytd + smhs.seg_total_work_hr;
                    smhs.cont_mh_ytd = smh.cont_mh_ytd + smhs.cont_total_work_hr;
                    smhs.emp_total_ytd = smhs.seg_emp_ytd + smhs.cont_emp_ytd;
                    smhs.mh_total_ytd = smhs.seg_mh_ytd + smhs.cont_mh_ytd;
                }

                if (db.safe_man_hours_incident.Where(p => p.date.Value.Month == smhs.month.Value.Month && p.date.Value.Year == smhs.month.Value.Year).Count() > 0)
                {
                    isCount = false;
                }

                if (isCount)
                {
                    smhs.seg_emp_smh = smh.seg_emp_smh + smhs.seg_total;
                    smhs.cont_emp_smh = smh.cont_emp_smh + smhs.cont_total;
                    smhs.seg_mh_smh = smh.seg_mh_smh + smhs.seg_total_work_hr;
                    smhs.cont_mh_smh = smh.cont_mh_smh + smhs.cont_total_work_hr;
                    smhs.emp_total_smh = smhs.seg_emp_smh + smhs.cont_emp_smh;
                    smhs.mh_total_smh = smhs.seg_mh_smh + smhs.cont_mh_smh;
                    DateTime last_day = new DateTime(smhs.month.Value.Year, smhs.month.Value.Month, getDay(smhs.month.Value.Year, smhs.month.Value.Month));
                    smhs.days_smh = smh.days_smh + (int)((last_day - smhs.month.Value.AddDays(-1)).TotalDays);
                }

                db.Entry(smhs).State = EntityState.Modified;
                db.SaveChanges();

                smh.seg_emp_smh = smhs.seg_emp_smh;
                smh.cont_emp_smh = smhs.cont_emp_smh;
                smh.seg_mh_smh = smhs.seg_mh_smh;
                smh.cont_mh_smh = smhs.cont_mh_smh;
                smh.seg_emp_ytd = smhs.seg_emp_ytd;
                smh.cont_emp_ytd = smhs.cont_emp_ytd;
                smh.seg_mh_ytd = smhs.seg_mh_ytd;
                smh.cont_mh_ytd = smhs.cont_mh_ytd;
                smh.days_smh = smhs.days_smh;
            }

            smh.seg_emp_smh = seg_emp_smh;
            smh.cont_emp_smh = cont_emp_smh;
            smh.seg_mh_smh = seg_mh_smh;
            smh.cont_mh_smh = cont_mh_smh;
            smh.seg_emp_ytd = seg_emp_ytd;
            smh.cont_emp_ytd = cont_emp_ytd;
            smh.seg_mh_ytd = seg_mh_ytd;
            smh.cont_mh_ytd = cont_mh_ytd;
            smh.days_smh = days_smh;

            db.Entry(smh).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { id = smh.id });
        }

        public JsonResult CheckMonthYear(int month, int year)
        {
            safe_man_hours smh = db.safe_man_hours.Where(p => p.month.Value.Month == month && p.month.Value.Year == year).ToList().FirstOrDefault();

            if (smh != null)
            {
                return Json(new { id = smh.id });
            }
            else
            {
                long cont_emp = 0;
                long cont_mh = 0;
                List<monthly_project_she_report> list_mpsr = db.monthly_project_she_report.Where(p => p.month_year.Value.Month == month && p.month_year.Value.Year == year).ToList();
                foreach (monthly_project_she_report mpsr in list_mpsr)
                {
                    cont_emp += mpsr.local_total.Value + mpsr.non_local_total.Value + mpsr.expatriates_total.Value;
                    cont_mh += mpsr.man_hours_mh.Value;
                }

                return Json(new { cont_emp = cont_emp, cont_mh = cont_mh});
            }
        }

        public JsonResult AddIncident()
        {
            safe_man_hours_incident smhi = new safe_man_hours_incident
            {
                date = DateTime.Today
            };

            db.safe_man_hours_incident.Add(smhi);
            db.SaveChanges();

            return Json(true);
        }

        private int getDay(int year, int month)
        {
            int day = 0;
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                day = 31;
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                day = 30;
            }
            else
            {
                if ((year % 400 == 0) || (year % 4 == 0 && year % 100 != 0))
                {
                    day = 29;
                }
                else
                {
                    day = 28;
                }
            }

            return day;
        }

    }
}
