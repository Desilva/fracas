﻿using ReportManagement;
using StarEnergi.Models;
using StarEnergi.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Threading;


namespace StarEnergi.Controllers.FrontEnd
{
    public class DailyLogsController : PdfViewController
    {
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;
        #region others
        //
        // GET: /DailyLogs/

        public ActionResult Index()
        {
            ViewBag.Nama = "Daily Log";
            var has = (from users in db.users
                       select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            li = Session["roles"] as List<user_per_role>;
            if (HttpContext.Session["username"] != null)
            {
                string username = (String)HttpContext.Session["username"].ToString();
                Debug.WriteLine(username);
                ViewData["user_role"] = li;
            }
            return View();
        }

        public ActionResult part3View(int? id)
        {
            ViewBag.id = id;
            ViewBag.powerStation = db.daily_log_power_stations.Where(p => p.id_daily_log == id).OrderBy(p => p.power_station_unit).ThenBy(p => p.power_station_time).ToList();
            return PartialView();
        }

        public ActionResult part4View(int? id)
        {
            ViewBag.id = id;
            ViewBag.sags = db.daily_log_sags.Where(p => p.id_daily_log == id).OrderBy(p => p.sags_unit).ThenBy(p => p.sags_time).ToList();
            return PartialView();
        }

        public ActionResult part6View(int? id)
        {
            ViewBag.id = id;
            ViewBag.pro = db.daily_log_pro.Where(p => p.id_daily_log == id).ToList();
            return PartialView();
        }

        public ActionResult part7View(int? id)
        {
            ViewBag.id = id;
            ViewBag.maintenance = db.daily_log_maintainence.Where(p => p.id_daily_log == id).ToList();
            return PartialView();
        }

        public ActionResult part8View(int? id)
        {
            ViewBag.id = id;
            ViewBag.lastPlantStatus = db.daily_log_last_plant_status.Where(p => p.id_daily_log == id).ToList();
            
            if (db.daily_log.Find(id) != null)
            {
                daily_log dl = db.daily_log.Find(id);
                ViewBag.dl = dl;
                daily_log_weekly_target wt = db.daily_log_weekly_target.Where(p => p.date == dl.date && p.shift == dl.shift).ToList().FirstOrDefault();
                if (wt != null)
                {
                    ViewBag.target_1_sh1 = Double.Parse(wt.target_unit_1);
                    ViewBag.target_2_sh1 = Double.Parse(wt.target_unit_2);
                }
                ViewBag.lastPlantTime = dl.last_plant_time;
            }
            return PartialView();
        }

        public ActionResult part9View(int? id)
        {
            ViewBag.id = id;
            ViewBag.SAP = db.daily_log_sap.Where(p => p.id_daily_log == id).ToList();
            return PartialView();
        }

        public ActionResult report()
        {
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
            return PartialView();
        }

        public ActionResult operationalWarning()
        {
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
            return PartialView();
        }

        public ActionResult weeklyTarget()
        {
            ////Debug.WriteLine(DateTime.Now);
            DateTime now = DateTime.Now.Date;
            //DateTime d = new DateTime(now.Year, now.Month, now.Day);
            ViewBag.today = db.daily_log_weekly_target.Where(p => p.date == now).OrderBy(p => p.shift).ToList();
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
            return PartialView();
        }

        public ActionResult keyPerformanceIndicator()
        {
            DateTime now = DateTime.Today.AddDays(-1);

            List<daily_log_weekly_target> list_wt_day = db.daily_log_weekly_target.Where(p => p.date == now).ToList();
            double today_target = 0;
            foreach (daily_log_weekly_target wt_day in list_wt_day) {
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

            return PartialView();
        }

        [HttpPost]
        public ActionResult addWPNB(daily_log_wpnb wpnb)
        {
            db.daily_log_wpnb.Add(wpnb);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult part3(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }
            return PartialView();
        }

        public ActionResult part4(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }
            return PartialView();
        }

        public ActionResult part6(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }
            return PartialView();
        }

        public ActionResult part7(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }

            return PartialView();
        }

        public ActionResult part8(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }
            return PartialView();
        }

        public ActionResult part9(int? id)
        {
            ViewBag.id = id;
            if (id != null)
            {
                daily_log dl = db.daily_log.Find(id);
                string username = HttpContext.Session["username"] as string;
                ViewBag.isApprove = dl.is_approve;
            }

            return PartialView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id == null: create, else edit</param>
        /// <returns></returns>
        public ActionResult addDailyLog(int? id)
        {
            daily_log shift1 = null; bool isCreate = true;

            if (id != null) //edit
            {
                isCreate = false;
                ViewBag.mod = id;
                shift1 = db.daily_log.Find(id);
                ViewBag.datas = shift1;
                daily_log_weekly_target wt = db.daily_log_weekly_target.Where(p => p.date == shift1.date && p.shift == shift1.shift).ToList().FirstOrDefault();
                if (wt != null)
                {
                    ViewBag.target_1_sh1 = Double.Parse(wt.target_unit_1);
                    ViewBag.target_2_sh1 = Double.Parse(wt.target_unit_2);
                }
                if (shift1.id_shift2 != null)
                {
                    ViewBag.mod2 = shift1.id_shift2;
                    daily_log shift2 = db.daily_log.Find(shift1.id_shift2);
                    ViewBag.datas2 = shift2;
                    daily_log_weekly_target wt2 = db.daily_log_weekly_target.Where(p => p.date == shift2.date && p.shift == shift2.shift).ToList().FirstOrDefault();
                    if (wt2 != null)
                    {
                        ViewBag.target_1_sh2 = Double.Parse(wt2.target_unit_1);
                        ViewBag.target_2_sh2 = Double.Parse(wt2.target_unit_2);
                    }
                }

            }
            else //create
            {
                id = db.daily_log.Max(p => p.id);
                id++;
                string subPath = "~/Attachment/daily_log/" + id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                Debug.WriteLine(IsExists + "  " + id);
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
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

            //wells data
            List<daily_log_wells> wells = null;
            if (isCreate) //create
                wells = db.daily_log_wells.Where(m => (m.is_delete == null ? true : (m.is_delete.Value == true ? false : true))).OrderBy(m => m.name).ToList();
            else //edit
                wells = shift1.daily_log_to_wells.Select(m => m.daily_log_wells).OrderBy(m => m.name).ToList().ToList();

            //viewdata
            ViewData["users"] = has;
            ViewData["user_role"] = li;
            ViewData["wells"] = wells;

            return PartialView();
        }

        //
        // Ajax select binding incident report
        [GridAction]
        public ActionResult _SelectAjaxDailyLog()
        {
            return bindingDailyLog();
        }

        //select data incident report
        private ViewResult bindingDailyLog()
        {
            List<daily_log> f = db.daily_log.Where(p => p.shift == 1).OrderByDescending(p => p.date).ToList();
            List<DailyLogPresentationStub> list = new DailyLogPresentationStub().MapList(f);

            return View(new GridModel<DailyLogPresentationStub>
            {
                Data = list
            });
        }

        [HttpPost]
        public JsonResult Add(daily_log dailyLog, List<daily_log_to_wells> arrWell)
        {
            DateTime dt = dailyLog.date.Value.AddDays(-1);
            daily_log dl = db.daily_log.Where(p => p.date == dt && p.shift == 2).ToList().FirstOrDefault();
            if (dl != null)
            {
                if (dl.is_approve != 1)
                {
                    return Json(null);
                }
            }
            int id_before = db.daily_log.Max(p => p.id) + 1;
            db.daily_log.Add(dailyLog);
            db.SaveChanges();

            int id = db.daily_log.Max(p => p.id);

            if (id != id_before)
            {
                string subPath = "~/Attachment/daily_log/" + id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

                try
                {
                    string old_path = Server.MapPath("~/Attachment/daily_log/" + id_before);
                    string new_path = Server.MapPath("~/Attachment/daily_log/" + id);
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + id_before), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        System.IO.File.Move(old_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1), new_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }

            List<daily_log_power_stations> li = db.daily_log_power_stations.Where(p => p.id_daily_log == null).ToList();
            foreach (daily_log_power_stations l in li)
            {
                l.id_daily_log = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<daily_log_sags> sags = db.daily_log_sags.Where(p => p.id_daily_log == null).ToList();
            foreach (daily_log_sags l in sags)
            {
                l.id_daily_log = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<daily_log_pro> pro = db.daily_log_pro.Where(p => p.id_daily_log == null).ToList();
            foreach (daily_log_pro l in pro)
            {
                l.id_daily_log = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<daily_log_maintainence> main = db.daily_log_maintainence.Where(p => p.id_daily_log == null).ToList();
            foreach (daily_log_maintainence l in main)
            {
                l.id_daily_log = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<daily_log_last_plant_status> lps = db.daily_log_last_plant_status.Where(p => p.id_daily_log == null).ToList();
            foreach (daily_log_last_plant_status l in lps)
            {
                l.id_daily_log = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            //insert daily_log_to_wells
            daily_log_to_wells findWell;
            foreach (daily_log_to_wells row in arrWell)
            {
                findWell = dailyLog.daily_log_to_wells.Where(m => m.daily_log_id == row.daily_log_id && m.daily_log_wells_id == row.daily_log_wells_id).FirstOrDefault();
                if (findWell == null) //create daily log - well
                {
                    dailyLog.daily_log_to_wells.Add(row);
                }
                else //edit
                {
                    db.daily_log.Attach(dailyLog);

                    //update data
                    findWell.is_text = row.is_text;
                    findWell.fcv = row.fcv;
                    findWell.flow = row.flow;
                    findWell.whp = row.whp;

                    var entry = db.Entry(dailyLog);
                    entry.State = EntityState.Modified;
                }
            }
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(daily_log dailyLog, string time_check, List<daily_log_to_wells> arrWell)
        {
            daily_log ir = db.daily_log.Find(dailyLog.id);
            DateTime dt = DateTime.Parse(time_check == "" ? "00:00:00" : time_check);
            ir.date = dailyLog.date;
            ir.grup = dailyLog.grup;
            ir.production_foreman = dailyLog.production_foreman;
            ir.production_operator_1 = dailyLog.production_operator_1;
            ir.production_operator_2 = dailyLog.production_operator_2;
            ir.production_operator_3 = dailyLog.production_operator_3;
            ir.production_operator_4 = dailyLog.production_operator_4;
            ir.production_operator_5 = dailyLog.production_operator_5;
            ir.production_operator_6 = dailyLog.production_operator_6;
            ir.production_operator_7 = dailyLog.production_operator_7;
            ir.production_operator_8 = dailyLog.production_operator_8;
            ir.time_check = dt.TimeOfDay;
            ir.generator_output_1 = dailyLog.generator_output_1;
            ir.gross_1 = dailyLog.gross_1;
            ir.generator_output_counter_1 = dailyLog.generator_output_counter_1;
            ir.power_factor_1 = dailyLog.power_factor_1;
            ir.tap_charger_1 = dailyLog.tap_charger_1;
            ir.pln_grid_voltage_1 = dailyLog.pln_grid_voltage_1;
            ir.valve_limiter_1 = dailyLog.valve_limiter_1;
            ir.governor_output_1 = dailyLog.governor_output_1;
            ir.wcp_counter_1 = dailyLog.wcp_counter_1;
            ir.condenser_pressure_1 = dailyLog.condenser_pressure_1;
            ir.main_cw_flow_1 = dailyLog.main_cw_flow_1;
            ir.ppc_g_co_1 = dailyLog.ppc_g_co_1;
            ir.interface_pressure_1 = dailyLog.interface_pressure_1;
            ir.vent_bias_1 = dailyLog.vent_bias_1;
            ir.main_cw_pressure_1 = dailyLog.main_cw_pressure_1;
            ir.ct_basin_ph_1 = dailyLog.ct_basin_ph_1;
            ir.condenser_cw_inlet_a_1 = dailyLog.condenser_cw_inlet_a_1;
            ir.condenser_cw_inlet_b_1 = dailyLog.condenser_cw_inlet_b_1;
            ir.gen_trans_winding_temp_1 = dailyLog.gen_trans_winding_temp_1;
            ir.unit_trans_winding_temp_1 = dailyLog.unit_trans_winding_temp_1;
            ir.wheel_case_pressure_1 = dailyLog.wheel_case_pressure_1;
            ir.generator_output_2 = dailyLog.generator_output_2;
            ir.gross_2 = dailyLog.gross_2;
            ir.generator_output_counter_2 = dailyLog.generator_output_counter_2;
            ir.power_factor_2 = dailyLog.power_factor_2;
            ir.tap_charger_2 = dailyLog.tap_charger_2;
            ir.pln_grid_voltage_2 = dailyLog.pln_grid_voltage_2;
            ir.valve_limiter_2 = dailyLog.valve_limiter_2;
            ir.governor_output_2 = dailyLog.governor_output_2;
            ir.wcp_counter_2 = dailyLog.wcp_counter_2;
            ir.condenser_pressure_2 = dailyLog.condenser_pressure_2;
            ir.main_cw_flow_2 = dailyLog.main_cw_flow_2;
            ir.ppc_g_co_2 = dailyLog.ppc_g_co_2;
            ir.interface_pressure_2 = dailyLog.interface_pressure_2;
            ir.vent_bias_2 = dailyLog.vent_bias_2;
            ir.main_cw_pressure_2 = dailyLog.main_cw_pressure_2;
            ir.ct_basin_ph_2 = dailyLog.ct_basin_ph_2;
            ir.condenser_cw_inlet_a_2 = dailyLog.condenser_cw_inlet_a_2;
            ir.condenser_cw_inlet_b_2 = dailyLog.condenser_cw_inlet_b_2;
            ir.gen_trans_winding_temp_2 = dailyLog.gen_trans_winding_temp_2;
            ir.unit_trans_winding_temp_2 = dailyLog.unit_trans_winding_temp_2;
            ir.wheel_case_pressure_2 = dailyLog.wheel_case_pressure_2;
            ir.ncg_1 = dailyLog.ncg_1;
            ir.ncg_2 = dailyLog.ncg_2;
            ir.turbine_1 = dailyLog.turbine_1;
            ir.turbine_2 = dailyLog.turbine_2;
            ir.ct_temp_1 = dailyLog.ct_temp_1;
            ir.ct_temp_2 = dailyLog.ct_temp_2;
            ir.exhaust_1 = dailyLog.exhaust_1;
            ir.exhaust_2 = dailyLog.exhaust_2;
            ir.upper_tp_level = dailyLog.upper_tp_level;
            ir.lower_tp_level = dailyLog.lower_tp_level;
            ir.mv_333 = dailyLog.mv_333;
            ir.mv_334 = dailyLog.mv_334;
            ir.brine_level = dailyLog.brine_level;
            ir.condensate_level = dailyLog.condensate_level;
            ir.naoh_level = dailyLog.naoh_level;
            ir.wwd_pond_level = dailyLog.wwd_pond_level;
            ir.uti_active_1 = dailyLog.uti_active_1;
            ir.uti_reactive_1 = dailyLog.uti_reactive_1;
            ir.sc_main_1 = dailyLog.sc_main_1;
            ir.sc_auxiliary_1 = dailyLog.sc_auxiliary_1;
            ir.ge_active_1 = dailyLog.ge_active_1;
            ir.ge_reactive_1 = dailyLog.ge_reactive_1;
            ir.metering_segwwl_1 = dailyLog.metering_segwwl_1;
            ir.metering_pln_1 = dailyLog.metering_pln_1;
            ir.condensate_ps_1 = dailyLog.condensate_ps_1;
            ir.segwwl_availability_1 = dailyLog.segwwl_availability_1;
            ir.pln_dispatch_1 = dailyLog.pln_dispatch_1;
            ir.pln_meter_1 = dailyLog.pln_meter_1;
            ir.segwwl_export_1 = dailyLog.segwwl_export_1;
            ir.actual_export_1 = dailyLog.actual_export_1;
            ir.production_excess_1 = dailyLog.production_excess_1;
            ir.rpf_1 = dailyLog.rpf_1;
            ir.pgf_1 = dailyLog.pgf_1;
            ir.pln_1 = dailyLog.pln_1;
            ir.uti_active_2 = dailyLog.uti_active_2;
            ir.uti_reactive_2 = dailyLog.uti_reactive_2;
            ir.sc_main_2 = dailyLog.sc_main_2;
            ir.sc_auxiliary_2 = dailyLog.sc_auxiliary_2;
            ir.ge_active_2 = dailyLog.ge_active_2;
            ir.ge_reactive_2 = dailyLog.ge_reactive_2;
            ir.metering_segwwl_2 = dailyLog.metering_segwwl_2;
            ir.metering_pln_2 = dailyLog.metering_pln_2;
            ir.condensate_ps_2 = dailyLog.condensate_ps_2;
            ir.segwwl_availability_2 = dailyLog.segwwl_availability_2;
            ir.pln_dispatch_2 = dailyLog.pln_dispatch_2;
            ir.pln_meter_2 = dailyLog.pln_meter_2;
            ir.segwwl_export_2 = dailyLog.segwwl_export_2;
            ir.actual_export_2 = dailyLog.actual_export_2;
            ir.production_excess_2 = dailyLog.production_excess_2;
            ir.rpf_2 = dailyLog.rpf_2;
            ir.pgf_2 = dailyLog.pgf_2;
            ir.pln_2 = dailyLog.pln_2;
            ir.condensate_total = dailyLog.condensate_total;
            ir.brine_total = dailyLog.brine_total;
            ir.note = dailyLog.note;
            ir.last_plant_time = dailyLog.last_plant_time;
            ir.user_shift = dailyLog.user_shift;
            ir.shift = dailyLog.shift;
            ir.achievement_1 = dailyLog.achievement_1;
            ir.achievement_2 = dailyLog.achievement_2;
            ir.remark_1 = dailyLog.remark_1;
            ir.remark_2 = dailyLog.remark_2;
            
            //insert daily_log_to_wells
            if (arrWell != null)
            {
                daily_log_to_wells findWell;
                foreach (daily_log_to_wells row in arrWell)
                {
                    findWell = ir.daily_log_to_wells.Where(m => m.daily_log_wells_id == row.daily_log_wells_id).FirstOrDefault();
                    if (findWell == null) //create daily log - well
                    {
                        ir.daily_log_to_wells.Add(row);
                    }
                    else //edit
                    {
                        db.daily_log.Attach(ir);

                        //update data
                        findWell.is_text = row.is_text;
                        findWell.fcv = row.fcv;
                        findWell.flow = row.flow;
                        findWell.whp = row.whp;

                        var entry = db.Entry(ir);
                        entry.State = EntityState.Modified;
                    }
                }
            }

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult Approve(daily_log dailyLog)
        {
            daily_log ir = db.daily_log.Find(dailyLog.id);
            daily_log_weekly_target wt = db.daily_log_weekly_target.Where(p => p.date == ir.date && p.shift == ir.shift).ToList().FirstOrDefault();
            if (wt != null)
            {
                if (Double.Parse(wt.target_unit_1).CompareTo(Double.Parse(ir.achievement_1 == null ? "0" : ir.achievement_1)) > 0)
                {
                    if (ir.remark_1 == null)
                    {
                        return Json(null);
                    }
                }
                if (Double.Parse(wt.target_unit_2).CompareTo(Double.Parse(ir.achievement_2 == null ? "0" : ir.achievement_2)) > 0)
                {
                    if (ir.remark_2 == null)
                    {
                        return Json(null);
                    }
                }
            }

            if (ir.shift == 1)
            {
                daily_log dl = new daily_log();
                dl.date = ir.date;
                dl.shift = 2;
                db.daily_log.Add(dl);
                db.SaveChanges();
                int id = db.daily_log.Max(p => p.id);

                ir.id_shift2 = id;

                string subPath = "~/Attachment/daily_log/" + id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            }

            
            ir.is_approve = dailyLog.is_approve;
            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();

            
            return Json(true);
        }

        //
        // Ajax delete binding daily log
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxDailyLog(int id)
        {
            deleteDailyLog(id);
            return bindingDailyLog();
        }

        //delete data daily log
        private void deleteDailyLog(int id)
        {
            daily_log ir = db.daily_log.Find(id);
            db.daily_log.Remove(ir);
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult attachment1(IEnumerable<HttpPostedFileBase> attachment1, int? id)
        {
            var currpath = "";
            string st = "";
            id = id == null ? db.daily_log.Max(p => p.id) + 1 : id;
            if (attachment1 != null)
            {
                foreach (var file in attachment1)
                {
                    currpath = Path.Combine(
                        Server.MapPath("~/Attachment/daily_log/" + id),
                        file.FileName
                    );
                    file.SaveAs(currpath);
                }
                currpath = "/Attachment/daily_log/" + id + "/";
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + id), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        st += f.File.Substring(f.File.LastIndexOf("\\") + 1) + ";;";
                        Debug.WriteLine(f.File);
                    }
                    Debug.WriteLine(files.Count().ToString());
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }
            return Json(new { success = true, path = currpath, files = st });
        }

        [HttpPost]
        public ActionResult attachment2(IEnumerable<HttpPostedFileBase> attachment2, int? id)
        {
            var currpath = "";
            string st = "";
            id = id == null ? db.daily_log.Max(p => p.id) + 1 : id;
            if (attachment2 != null)
            {
                foreach (var file in attachment2)
                {
                    currpath = Path.Combine(
                        Server.MapPath("~/Attachment/daily_log/" + id),
                        file.FileName
                    );
                    file.SaveAs(currpath);
                }
                currpath = "/Attachment/daily_log/" + id + "/";
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + id), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        st += f.File.Substring(f.File.LastIndexOf("\\") + 1) + ";;";
                        Debug.WriteLine(f.File);
                    }
                    Debug.WriteLine(files.Count().ToString());
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }
            return Json(new { success = true, path = currpath, files = st });
        }

        [HttpPost]
        public ActionResult Atch(int? id)
        {
            id = id == null ? db.daily_log.Max(p => p.id) + 1 : id;
            var currpath = "/Attachment/daily_log/" + id + "/";
            string st = "";
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + id), "*.*", SearchOption.TopDirectoryOnly)
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    st += f.File.Substring(f.File.LastIndexOf("\\") + 1) + ";;";
                    Debug.WriteLine(f.File);
                }
                Debug.WriteLine(files.Count().ToString());
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Debug.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Debug.WriteLine(PathEx.Message);
            }
            return Json(new { success = true, path = currpath, files = st });
        }

        //==========================================================

        //
        // Ajax select binding power station activity
        [GridAction]
        public ActionResult _SelectAjaxPowerStation(int? id)
        {
            return bindingPowerStation(id);
        }

        //select data power station activity
        private ViewResult bindingPowerStation(int? id)
        {
            List<daily_log_power_stations> f = new List<daily_log_power_stations>();
            if (id == null)
            {
                f = db.daily_log_power_stations.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_power_stations.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_power_stations>
            {
                Data = f.OrderBy(p => p.power_station_unit)
            });
        }

        //
        // Ajax delete binding power station activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxPowerStation(int id, int? id_daily_log)
        {
            deletePowerStation(id);
            return bindingPowerStation(id_daily_log);
        }

        //delete data power station activity
        private void deletePowerStation(int id)
        {
            daily_log_power_stations powerStation = db.daily_log_power_stations.Find(id);
            db.daily_log_power_stations.Remove(powerStation);
            db.SaveChanges();
        }

        //
        // Ajax insert binding power station activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxPowerStation(int? id_daily_log)
        {
            daily_log_power_stations ps = new daily_log_power_stations();
            ps.id_daily_log = id_daily_log;
            ps.username = HttpContext.Session["username"] as string;
            if (TryUpdateModel(ps))
            {
                if (ps.power_station_unit == null || ps.power_station_unit == "")
                {
                    ModelState.AddModelError("power_station_unit", "Unit required");
                }
                else if (ps.power_station_activity == null || ps.power_station_activity == "")
                {
                    ModelState.AddModelError("power_station_activity", "Activity required");
                } 
                else
                {
                    create(ps);
                }
            }
            return bindingPowerStation(id_daily_log);
        }

        //insert data power station activity
        public void create(daily_log_power_stations ps)
        {
            db.daily_log_power_stations.Add(ps);
            db.SaveChanges();
        }

        //
        // Ajax update binding power station activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxPowerStation(int? id_daily_log, int id)
        {
            daily_log_power_stations editable = db.daily_log_power_stations.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingPowerStation(id_daily_log);
        }

        //update data power station activity
        private void update(daily_log_power_stations ps)
        {
            db.Entry(ps).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllPowerStation()
        {
            List<daily_log_power_stations> li = db.daily_log_power_stations.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_power_stations ir in li)
            {
                db.daily_log_power_stations.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================

        //
        // Ajax select binding SAGS activity
        [GridAction]
        public ActionResult _SelectAjaxSAGS(int? id)
        {
            return bindingSAGS(id);
        }

        //select data SAGS activity
        private ViewResult bindingSAGS(int? id)
        {
            List<daily_log_sags> f = new List<daily_log_sags>();
            if (id == null)
            {
                f = db.daily_log_sags.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_sags.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_sags>
            {
                Data = f.OrderBy(p => p.sags_unit)
            });
        }

        //
        // Ajax delete binding SAGS activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxSAGS(int id, int? id_daily_log)
        {
            deleteSAGS(id);
            return bindingSAGS(id_daily_log);
        }

        //delete data SAGS activity
        private void deleteSAGS(int id)
        {
            daily_log_sags sags = db.daily_log_sags.Find(id);
            db.daily_log_sags.Remove(sags);
            db.SaveChanges();
        }

        //
        // Ajax insert binding SAGS activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxSAGS(int? id_daily_log)
        {
            daily_log_sags sags = new daily_log_sags();
            sags.id_daily_log = id_daily_log;
            sags.username = HttpContext.Session["username"] as string;
            if (TryUpdateModel(sags))
            {
                if (sags.sags_unit == null || sags.sags_unit == "")
                {
                    ModelState.AddModelError("sags_unit", "Unit required");
                }
                else if (sags.sags_activity == null || sags.sags_activity == "")
                {
                    ModelState.AddModelError("sags_activity", "Activity required");
                }
                else
                {
                    create(sags);
                }
            }
            return bindingSAGS(id_daily_log);
        }

        //insert data SAGS activity
        public void create(daily_log_sags sags)
        {
            db.daily_log_sags.Add(sags);
            db.SaveChanges();
        }

        //
        // Ajax update binding SAGS activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxSAGS(int? id_daily_log, int id)
        {
            daily_log_sags editable = db.daily_log_sags.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingSAGS(id_daily_log);
        }

        //update data SAGS activity
        private void update(daily_log_sags sags)
        {
            db.Entry(sags).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllSAGS()
        {
            List<daily_log_sags> li = db.daily_log_sags.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_sags ir in li)
            {
                db.daily_log_sags.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================

        //
        // Ajax select binding PRO
        [GridAction]
        public ActionResult _SelectAjaxPRO(int? id)
        {
            return bindingPRO(id);
        }

        //select data PRO
        private ViewResult bindingPRO(int? id)
        {
            List<daily_log_pro> f = new List<daily_log_pro>();
            if (id == null)
            {
                f = db.daily_log_pro.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_pro.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_pro>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        //
        // Ajax delete binding PRO
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxPRO(int id, int? id_daily_log)
        {
            deletePRO(id);
            return bindingPRO(id_daily_log);
        }

        //delete data PRO
        private void deletePRO(int id)
        {
            daily_log_pro pro = db.daily_log_pro.Find(id);
            db.daily_log_pro.Remove(pro);
            db.SaveChanges();
        }

        //
        // Ajax insert binding PRO
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxPRO(int? id_daily_log)
        {
            daily_log_pro pro = new daily_log_pro();
            pro.id_daily_log = id_daily_log;
            pro.username = HttpContext.Session["username"] as string;
            if (TryUpdateModel(pro))
            {
                if (pro.work_to_be_performed == null || pro.work_to_be_performed == "")
                {
                    ModelState.AddModelError("work_to_be_performed", "Work to be performed required");
                }
                else
                {
                    create(pro);
                }
            }
            return bindingPRO(id_daily_log);
        }

        //insert data PRO
        public void create(daily_log_pro pro)
        {
            db.daily_log_pro.Add(pro);
            db.SaveChanges();
        }

        //
        // Ajax update binding PRO
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxPRO(int? id_daily_log, int id)
        {
            daily_log_pro editable = db.daily_log_pro.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingPRO(id_daily_log);
        }

        //update data SAGS activity
        private void update(daily_log_pro pro)
        {
            db.Entry(pro).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllPRO()
        {
            List<daily_log_pro> li = db.daily_log_pro.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_pro ir in li)
            {
                db.daily_log_pro.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================

        //
        // Ajax select binding Maintenance Activity
        [GridAction]
        public ActionResult _SelectAjaxMaintenance(int? id)
        {
            return bindingMaintenance(id);
        }

        //select data PRO
        private ViewResult bindingMaintenance(int? id)
        {
            List<daily_log_maintainence> f = new List<daily_log_maintainence>();
            if (id == null)
            {
                f = db.daily_log_maintainence.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_maintainence.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_maintainence>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        //
        // Ajax delete binding Maintenance Activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxMaintenance(int id, int? id_daily_log)
        {
            deleteMaintenance(id);
            return bindingMaintenance(id_daily_log);
        }

        //delete data Maintenance Activity
        private void deleteMaintenance(int id)
        {
            daily_log_maintainence maintenance = db.daily_log_maintainence.Find(id);
            db.daily_log_maintainence.Remove(maintenance);
            db.SaveChanges();
        }

        //
        // Ajax insert binding Maintenance Activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxMaintenance(int? id_daily_log)
        {
            daily_log_maintainence maintenance = new daily_log_maintainence();
            maintenance.id_daily_log = id_daily_log;
            maintenance.username = HttpContext.Session["username"] as string;
            if (TryUpdateModel(maintenance))
            {
                if (maintenance.work_to_be_performed == null || maintenance.work_to_be_performed == "")
                {
                    ModelState.AddModelError("work_to_be_performed", "Work to be performed required");
                }
                else
                {
                    create(maintenance);
                }
            }
            return bindingMaintenance(id_daily_log);
        }

        //insert data Maintenance Activity
        public void create(daily_log_maintainence maintenance)
        {
            db.daily_log_maintainence.Add(maintenance);
            db.SaveChanges();
        }

        //
        // Ajax update binding Maintenance Activity
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxMaintenance(int? id_daily_log, int id)
        {
            daily_log_maintainence editable = db.daily_log_maintainence.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingMaintenance(id_daily_log);
        }

        //update data Maintenance Activity
        private void update(daily_log_maintainence maintenance)
        {
            db.Entry(maintenance).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllMaintenance()
        {
            List<daily_log_maintainence> li = db.daily_log_maintainence.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_maintainence ir in li)
            {
                db.daily_log_maintainence.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================

        //
        // Ajax select binding SAP
        [GridAction]
        public ActionResult _SelectAjaxSAP(int? id)
        {
            return bindingSAP(id);
        }

        //select data SAP
        private ViewResult bindingSAP(int? id)
        {
            List<daily_log_sap> f = new List<daily_log_sap>();
            if (id == null)
            {
                f = db.daily_log_sap.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_sap.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_sap>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        //
        // Ajax delete binding SAP
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxSAP(int id, int? id_daily_log)
        {
            deleteSAP(id);
            return bindingSAP(id_daily_log);
        }

        //delete data PRO
        private void deleteSAP(int id)
        {
            daily_log_sap sap = db.daily_log_sap.Find(id);
            db.daily_log_sap.Remove(sap);
            db.SaveChanges();
        }

        //
        // Ajax insert binding SAP
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxSAP(int? id_daily_log)
        {
            daily_log_sap sap = new daily_log_sap();
            sap.id_daily_log = id_daily_log;
            if (TryUpdateModel(sap))
            {
                if (sap.description == null || sap.description == "")
                {
                    ModelState.AddModelError("description", "Description required");
                }
                else
                {
                    create(sap);
                }
            }
            return bindingSAP(id_daily_log);
        }

        //insert data SAP
        public void create(daily_log_sap sap)
        {
            db.daily_log_sap.Add(sap);
            db.SaveChanges();
        }

        //
        // Ajax update binding SAP
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxSAP(int? id_daily_log, int id)
        {
            daily_log_sap editable = db.daily_log_sap.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingSAP(id_daily_log);
        }

        //update data SAP activity
        private void update(daily_log_sap sap)
        {
            db.Entry(sap).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllSAP()
        {
            List<daily_log_sap> li = db.daily_log_sap.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_sap sap in li)
            {
                db.daily_log_sap.Remove(sap);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================

        //
        // Ajax select binding Last Plant Status
        [GridAction]
        public ActionResult _SelectAjaxLastPlantStatus(int? id)
        {
            return bindingLastPlantStatus(id);
        }

        //select data Last Plant Status
        private ViewResult bindingLastPlantStatus(int? id)
        {
            List<daily_log_last_plant_status> f = new List<daily_log_last_plant_status>();
            if (id == null)
            {
                f = db.daily_log_last_plant_status.Where(p => p.id_daily_log == null).ToList();
            }
            else
            {
                f = db.daily_log_last_plant_status.Where(p => p.id_daily_log == id).ToList();
            }

            return View(new GridModel<daily_log_last_plant_status>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        //
        // Ajax delete binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxLastPlantStatus(int id, int? id_daily_log)
        {
            deleteLastPlantStatus(id);
            return bindingLastPlantStatus(id_daily_log);
        }

        //delete data Last Plant Status
        private void deleteLastPlantStatus(int id)
        {
            daily_log_last_plant_status lastPlantStatus = db.daily_log_last_plant_status.Find(id);
            db.daily_log_last_plant_status.Remove(lastPlantStatus);
            db.SaveChanges();
        }

        //
        // Ajax insert binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxLastPlantStatus(int? id_daily_log)
        {
            daily_log_last_plant_status lps = new daily_log_last_plant_status();
            lps.id_daily_log = id_daily_log;
            lps.username = HttpContext.Session["username"] as string;
            if (TryUpdateModel(lps))
            {
                if (lps.description == null || lps.description == "")
                {
                    ModelState.AddModelError("description", "Status required");
                }
                else
                {
                    create(lps);
                }
            }
            return bindingLastPlantStatus(id_daily_log);
        }

        //insert data Last Plant Status
        public void create(daily_log_last_plant_status lps)
        {
            db.daily_log_last_plant_status.Add(lps);
            db.SaveChanges();
        }

        //
        // Ajax update binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxLastPlantStatus(int? id_daily_log, int id)
        {
            daily_log_last_plant_status editable = db.daily_log_last_plant_status.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingLastPlantStatus(id_daily_log);
        }

        //update data Last Plant Status
        private void update(daily_log_last_plant_status lps)
        {
            db.Entry(lps).State = EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult DeleteAllLastPlantStatus()
        {
            List<daily_log_last_plant_status> li = db.daily_log_last_plant_status.Where(p => p.id_daily_log == null).ToList();

            foreach (daily_log_last_plant_status ir in li)
            {
                db.daily_log_last_plant_status.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //==========================================================


        //
        // Ajax select binding Last Plant Status
        [GridAction]
        public ActionResult _SelectAjaxOperationWarningHis()
        {
            return bindingOperationWarningHis();
        }

        //select data Last Plant Status
        private ViewResult bindingOperationWarningHis()
        {
            List<daily_log_operation_warning> f = new List<daily_log_operation_warning>();
            f = db.daily_log_operation_warning.ToList();
            return View(new GridModel<daily_log_operation_warning>
            {
                Data = f.OrderByDescending(p => p.end_date)
            });
        }

        //
        // Ajax select binding Last Plant Status
        [GridAction]
        public ActionResult _SelectAjaxOperationWarning()
        {
            return bindingOperationWarning();
        }

        //select data Last Plant Status
        private ViewResult bindingOperationWarning()
        {
            List<daily_log_operation_warning> f = new List<daily_log_operation_warning>();
            f = db.daily_log_operation_warning.Where(p => p.end_date.Value.CompareTo(DateTime.Today) >= 0).ToList();
            foreach (daily_log_operation_warning ow in f)
            {
                if (ow.initiator != null)
                {
                    ow.initiator_name = db.employees.Find(ow.initiator).alpha_name;
                }
            }
            return View(new GridModel<daily_log_operation_warning>
            {
                Data = f.OrderByDescending(p => p.end_date)
            });
        }

        //
        // Ajax insert binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxOperationWarning()
        {
            daily_log_operation_warning lps = new daily_log_operation_warning();
            if (TryUpdateModel(lps))
            {
                if (lps.start_date == null )
                {
                    ModelState.AddModelError("start_date", "Start date required");
                }
                else if (lps.end_date == null)
                {
                    ModelState.AddModelError("end_date", "End date required");
                }
                else if (lps.memo_direction == null || lps.memo_direction == "")
                {
                    ModelState.AddModelError("memo_direction", "Memo direction required");
                }
                else if (lps.action_required_by == null || lps.action_required_by == "")
                {
                    ModelState.AddModelError("action_required_by", "Action required by required");
                }
                {
                    create(lps);
                }
            }
            return bindingOperationWarning();
        }

        //insert data Last Plant Status
        public void create(daily_log_operation_warning lps)
        {
            lps.initiator = int.Parse(Session["id"].ToString());
            db.daily_log_operation_warning.Add(lps);
            db.SaveChanges();
        }

        //
        // Ajax update binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxOperationWarning(int id)
        {
            daily_log_operation_warning editable = db.daily_log_operation_warning.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingOperationWarning();
        }

        //update data Last Plant Status
        private void update(daily_log_operation_warning lps)
        {
            db.Entry(lps).State = EntityState.Modified;
            db.SaveChanges();
        }

        //==========================================================


        //
        // Ajax select binding Last Plant Status
        [GridAction]
        public ActionResult _SelectAjaxWeeklyTarget()
        {
            return bindingWeeklyTarget();
        }

        //select data Last Plant Status
        private ViewResult bindingWeeklyTarget()
        {
            List<daily_log_weekly_target> f = new List<daily_log_weekly_target>();
            DateTime now = DateTime.Today;
            DateTime StartOfWeek = now.AddDays((int)now.DayOfWeek > 5 ? -1 : ((int)now.DayOfWeek < 5 ? -(int)now.DayOfWeek-2 : 0));
            DateTime LastOfWeek = StartOfWeek.AddDays(6);
            f = db.daily_log_weekly_target.Where(p => p.date.Value.CompareTo(StartOfWeek) >= 0 && p.date.Value.CompareTo(LastOfWeek) <= 0).ToList();
            return View(new GridModel<daily_log_weekly_target>
            {
                Data = f.OrderByDescending(p => p.date).ThenBy(p => p.shift)
            });
        }

        //
        // Ajax select binding Last Plant Status
        [GridAction]
        public ActionResult _SelectAjaxHistoryTarget()
        {
            return bindingHistoryTarget();
        }

        //select data Last Plant Status
        private ViewResult bindingHistoryTarget()
        {
            List<daily_log_weekly_target> f = new List<daily_log_weekly_target>();
            f = db.daily_log_weekly_target.ToList();
            return View(new GridModel<daily_log_weekly_target>
            {
                Data = f.OrderByDescending(p => p.date).ThenBy(p => p.shift)
            });
        }

        //
        // Ajax insert binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxWeeklyTarget()
        {
            daily_log_weekly_target lps = new daily_log_weekly_target();
            if (TryUpdateModel(lps))
            {
                if (lps.date == null)
                {
                    ModelState.AddModelError("date", "Date required");
                }
                else if (lps.target_unit_1 == null || lps.target_unit_1 == "")
                {
                    ModelState.AddModelError("target_unit_1", "Target Unit 1 required");
                }
                else if (lps.target_unit_2 == null || lps.target_unit_2 == "")
                {
                    ModelState.AddModelError("target_unit_1", "Target Unit 2 required");
                }
                else
                {
                    create(lps);
                }
            }
            return bindingHistoryTarget();
        }

        //insert data Last Plant Status
        public void create(daily_log_weekly_target lps)
        {
            db.daily_log_weekly_target.Add(lps);
            db.SaveChanges();
        }

        //
        // Ajax update binding Last Plant Status
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxWeeklyTarget(int id)
        {
            daily_log_weekly_target editable = db.daily_log_weekly_target.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingHistoryTarget();
        }

        //update data Last Plant Status
        private void update(daily_log_weekly_target lps)
        {
            db.Entry(lps).State = EntityState.Modified;
            db.SaveChanges();
        }

        //==========================================================

        public ActionResult DetailDailyLog(int id)
        {
            daily_log details = new daily_log();
            details = db.daily_log.Find(id);

            ViewBag.nama = "Detail Daily Log " + details.date;

            ViewBag.mod = details.id_shift2;
            if (details.id_shift2 != null)
            {
                daily_log shift1 = db.daily_log.Find(details.id_shift2);
                ViewBag.datas = shift1;
            }
            return PartialView(details);
        }

        public ActionResult printDL(int id)
        {
            daily_log dl = db.daily_log.Find(id);
            dl.list_daily_log_last_plant_status = db.daily_log_last_plant_status.Where(p => p.id_daily_log == id).ToList();
            dl.list_daily_log_maintainence = db.daily_log_maintainence.Where(p => p.id_daily_log == id).ToList();
            dl.list_daily_log_power_stations = db.daily_log_power_stations.Where(p => p.id_daily_log == id).OrderBy(p => p.power_station_unit).ThenBy(p => p.power_station_time).ToList();
            dl.list_daily_log_pro = db.daily_log_pro.Where(p => p.id_daily_log == id).ToList();
            dl.list_daily_log_sags = db.daily_log_sags.Where(p => p.id_daily_log == id).OrderBy(p => p.sags_unit).ThenBy(p => p.sags_time).ToList();
            dl.list_daily_log_sap = db.daily_log_sap.Where(p => p.id_daily_log == id).ToList();
            dl.list_daily_log_operation_warning = db.daily_log_operation_warning.Where(p => p.end_date.Value.CompareTo(DateTime.Today) >= 0).ToList();

            dl.attach = new List<string>();
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + id), "*.*", SearchOption.TopDirectoryOnly)
                            where file.Substring(file.LastIndexOf(".") + 1).ToLower() == "png" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpeg"
                             || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpg" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "bmp" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "pdf"
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    dl.attach.Add(f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    Debug.WriteLine(f.File);
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Debug.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Debug.WriteLine(PathEx.Message);
            }

            daily_log_weekly_target wt = db.daily_log_weekly_target.Where(p => p.date == dl.date && p.shift == dl.shift).ToList().FirstOrDefault();
            if (wt != null)
            {
                dl.target_sh1 = Double.Parse(wt.target_unit_1).ToString("f2");
                dl.target_sh2 = Double.Parse(wt.target_unit_2).ToString("f2");
            }

            dl.daily_log_shift2 = db.daily_log.Find(dl.id_shift2 == null ? 0 : dl.id_shift2);
            if (dl.daily_log_shift2 != null)
            {
                dl.daily_log_shift2.list_daily_log_last_plant_status = db.daily_log_last_plant_status.Where(p => p.id_daily_log == dl.id_shift2).ToList();
                dl.daily_log_shift2.list_daily_log_maintainence = db.daily_log_maintainence.Where(p => p.id_daily_log == dl.id_shift2).ToList();
                dl.daily_log_shift2.list_daily_log_power_stations = db.daily_log_power_stations.Where(p => p.id_daily_log == dl.id_shift2).OrderBy(p => p.power_station_unit).ThenBy(p => p.power_station_time).ToList();
                dl.daily_log_shift2.list_daily_log_pro = db.daily_log_pro.Where(p => p.id_daily_log == dl.id_shift2).ToList();
                dl.daily_log_shift2.list_daily_log_sags = db.daily_log_sags.Where(p => p.id_daily_log == dl.id_shift2).OrderBy(p => p.sags_unit).ThenBy(p => p.sags_time).ToList();
                dl.daily_log_shift2.list_daily_log_sap = db.daily_log_sap.Where(p => p.id_daily_log == dl.id_shift2).ToList();
                dl.daily_log_shift2.list_daily_log_operation_warning = db.daily_log_operation_warning.Where(p => p.end_date.Value.CompareTo(DateTime.Today) >= 0).ToList();
                dl.daily_log_shift2.attach = new List<string>();
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/daily_log/" + dl.id_shift2), "*.*", SearchOption.TopDirectoryOnly)
                                where file.Substring(file.LastIndexOf(".") + 1).ToLower() == "png" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpeg"
                                 || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpg" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "bmp" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "pdf"
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        dl.daily_log_shift2.attach.Add(f.File.Substring(f.File.LastIndexOf("\\") + 1));
                        Debug.WriteLine(f.File);
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }

                daily_log_weekly_target wt2 = db.daily_log_weekly_target.Where(p => p.date == dl.daily_log_shift2.date && p.shift == dl.daily_log_shift2.shift).ToList().FirstOrDefault();
                if (wt2 != null)
                {
                    dl.daily_log_shift2.target_sh1 = Double.Parse(wt2.target_unit_1).ToString("f2");
                    dl.daily_log_shift2.target_sh2 = Double.Parse(wt2.target_unit_2).ToString("f2");
                }
            }
            return this.ViewPdf("", "DailyLogPrint", dl);
        }
        
        public ActionResult printReport(DateTime fromD, DateTime toD, string sections)
        {
            IList<string> section = sections.Split(',');
            List<DailyLogReportEntity> report = new List<DailyLogReportEntity>();
            List<daily_log> list_dl = db.daily_log.Where(p => p.date >= fromD.Date && p.date <= toD.Date && p.shift == 1).ToList();
            foreach (daily_log dl in list_dl)
            {
                DailyLogReportEntity dl_report = new DailyLogReportEntity();
                dl.daily_log_shift2 = db.daily_log.Find(dl.id_shift2);
                dl_report.date = dl.date;
                if (section.Contains("1"))
                {
                    dl_report.section1 = true;
                    dl_report.grup_1 = dl.grup;
                    dl_report.production_foreman_1 = dl.production_foreman;
                    dl_report.production_operator_1_1 = dl.production_operator_1;
                    dl_report.production_operator_2_1 = dl.production_operator_2;
                    dl_report.production_operator_3_1 = dl.production_operator_3;
                    dl_report.production_operator_4_1 = dl.production_operator_4;
                    dl_report.production_operator_5_1 = dl.production_operator_5;
                    dl_report.production_operator_6_1 = dl.production_operator_6;
                    dl_report.production_operator_7_1 = dl.production_operator_7;
                    dl_report.production_operator_8_1 = dl.production_operator_8;

                    if (dl.daily_log_shift2 != null)
                    {
                        dl_report.grup_2 = dl.daily_log_shift2.grup;
                        dl_report.production_foreman_2 = dl.daily_log_shift2.production_foreman;
                        dl_report.production_operator_1_2 = dl.daily_log_shift2.production_operator_1;
                        dl_report.production_operator_2_2 = dl.daily_log_shift2.production_operator_2;
                        dl_report.production_operator_3_2 = dl.daily_log_shift2.production_operator_3;
                        dl_report.production_operator_4_2 = dl.daily_log_shift2.production_operator_4;
                        dl_report.production_operator_5_2 = dl.daily_log_shift2.production_operator_5;
                        dl_report.production_operator_6_2 = dl.daily_log_shift2.production_operator_6;
                        dl_report.production_operator_7_2 = dl.daily_log_shift2.production_operator_7;
                        dl_report.production_operator_8_2 = dl.daily_log_shift2.production_operator_8;
                    }
                }

                if (section.Contains("2"))
                {
                    dl_report.section2 = true;
                    dl_report.list_daily_log_power_stations = db.daily_log_power_stations.Where(p => p.id_daily_log == dl.id || p.id_daily_log == dl.id_shift2).OrderBy(p => p.power_station_unit).ThenBy(p => p.power_station_time).ToList();
                    dl_report.list_daily_log_sags = db.daily_log_sags.Where(p => p.id_daily_log == dl.id || p.id_daily_log == dl.id_shift2).OrderBy(p => p.sags_unit).ThenBy(p => p.sags_time).ToList();
                }

                if (section.Contains("3"))
                {
                    dl_report.section3 = true;
                    dl_report.uti_active_1 = dl.uti_active_1;
                    dl_report.uti_reactive_1 = dl.uti_reactive_1;
                    dl_report.sc_main_1 = dl.sc_main_1;
                    dl_report.sc_auxiliary_1 = dl.sc_auxiliary_1;
                    dl_report.ge_active_1 = dl.ge_active_1;
                    dl_report.ge_reactive_1 = dl.ge_reactive_1;
                    dl_report.metering_segwwl_1 = dl.metering_segwwl_1;
                    dl_report.metering_pln_1 = dl.metering_pln_1;
                    dl_report.condensate_ps_1 = dl.condensate_ps_1;
                    dl_report.segwwl_availability_1 = dl.segwwl_availability_1;
                    dl_report.pln_dispatch_1 = dl.pln_dispatch_1;
                    dl_report.pln_meter_1 = dl.pln_meter_1;
                    dl_report.segwwl_export_1 = dl.segwwl_export_1;
                    dl_report.actual_export_1 = dl.actual_export_1;
                    dl_report.production_excess_1 = dl.production_excess_1;
                    dl_report.rpf_1 = dl.rpf_1;
                    dl_report.pgf_1 = dl.pgf_1;
                    dl_report.pln_1 = dl.pln_1;
                    dl_report.uti_active_2 = dl.uti_active_2;
                    dl_report.uti_reactive_2 = dl.uti_reactive_2;
                    dl_report.sc_main_2 = dl.sc_main_2;
                    dl_report.sc_auxiliary_2 = dl.sc_auxiliary_2;
                    dl_report.ge_active_2 = dl.ge_active_2;
                    dl_report.ge_reactive_2 = dl.ge_reactive_2;
                    dl_report.metering_segwwl_2 = dl.metering_segwwl_2;
                    dl_report.metering_pln_2 = dl.metering_pln_2;
                    dl_report.condensate_ps_2 = dl.condensate_ps_2;
                    dl_report.segwwl_availability_2 = dl.segwwl_availability_2;
                    dl_report.pln_dispatch_2 = dl.pln_dispatch_2;
                    dl_report.pln_meter_2 = dl.pln_meter_2;
                    dl_report.segwwl_export_2 = dl.segwwl_export_2;
                    dl_report.actual_export_2 = dl.actual_export_2;
                    dl_report.production_excess_2 = dl.production_excess_2;
                    dl_report.rpf_2 = dl.rpf_2;
                    dl_report.pgf_2 = dl.pgf_2;
                    dl_report.pln_2 = dl.pln_2;
                    dl_report.condensate_total = dl.condensate_total;
                    dl_report.brine_total = dl.brine_total;
                    dl_report.note = dl.note;
                }

                if (section.Contains("4"))
                {
                    dl_report.section4 = true;
                    double target_1 = 0;
                    double target_2 = 0;
                    List<daily_log_weekly_target> wt = db.daily_log_weekly_target.Where(p => p.date == dl.date).ToList();
                    foreach (daily_log_weekly_target w in wt)
                    {
                        target_1 += Double.Parse(w.target_unit_1);
                        target_2 += Double.Parse(w.target_unit_2);
                    }
                    dl_report.target_1 = target_1.ToString("f2");
                    dl_report.target_2 = target_2.ToString("f2");
                    dl_report.achievement_1_1 = dl.achievement_1;
                    dl_report.achievement_2_1 = dl.achievement_2;
                    dl_report.remark_1_1 = dl.remark_1;
                    dl_report.remark_2_1 = dl.remark_2;

                    if (dl.daily_log_shift2 != null)
                    {
                        dl_report.achievement_1_2 = dl.daily_log_shift2.achievement_1;
                        dl_report.achievement_2_2 = dl.daily_log_shift2.achievement_2;
                        dl_report.remark_1_2 = dl.daily_log_shift2.remark_1;
                        dl_report.remark_2_2 = dl.daily_log_shift2.remark_2;
                    }
                }

                if (section.Contains("5"))
                {
                    dl_report.section5 = true;
                    dl_report.list_daily_log_last_plant_status = db.daily_log_last_plant_status.Where(p => p.id_daily_log == dl.id || p.id_daily_log == dl.id_shift2).ToList();
                }
                report.Add(dl_report);
            }

            DailyLogReport dlr = new DailyLogReport
            {
                list_entity = report,
                from_date = fromD,
                to_date = toD
            };

            return this.ViewPdf("", "DailyLogReportPrint", dlr);
        }

        // ==============================================

        // load excel

        public string LoadExcel(HttpPostedFileBase userfile)
        {
            // extract only the fielname
            var fileName = Path.GetFileName(userfile.FileName);
            string err = "";
            // store the file inside ~/App_Data/uploads folder
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);           
            userfile.SaveAs(path);
            err = SaveDailyLog(path);
            return err;
        }

        private string SaveDailyLog(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadDailyLogNew(path);
            return excel.generateError(err);
        }
#endregion


        private void FillPowerStation(ref XSSFSheet sheet)
        {
            int startRowIndex = 13;
            int colIndexLeft = 9;
            int colIndexRight = 13;

            Dictionary<string, string> listPowerStation = new Dictionary<string, string>();
            listPowerStation.Add("Generator Output", " MW");
            listPowerStation.Add("(gross)", " Mvar");
            listPowerStation.Add("Generator Output Counter", " minute");
            listPowerStation.Add("Power Factor", "");
            listPowerStation.Add("Tap Changer", " step");
            listPowerStation.Add("PLN Grid Voltage", " kV");
            listPowerStation.Add("Valve Limiter", " %");
            listPowerStation.Add("Governor Output", " %");
            listPowerStation.Add("WCP Counter", " minute");
            listPowerStation.Add("Condenser Pressure", " mBar");
            listPowerStation.Add("Main C/W Flow", " kg/s");
            listPowerStation.Add("PPC-G(CO)", " %");
            listPowerStation.Add("Interface Pressure(SP)", " Bar");
            listPowerStation.Add("Vent Bias(SP)", " Bar");
            listPowerStation.Add("Main C/W Pressure", " Bar");
            listPowerStation.Add("CT Basin pH", "");
            listPowerStation.Add("Condenser C/W Inlet-A", " %");
            listPowerStation.Add("Condenser C/W Inlet-B", " %");
            listPowerStation.Add("Gen. Trans. Winding Temp.", " °C");
            listPowerStation.Add("Unit Trans. Winding Temp.", " °C");
            listPowerStation.Add("Wheel Case Pressure", " Bar");

            foreach (var pair in listPowerStation)
            {
                XSSFCell cellLeft = (XSSFCell)sheet.GetRow(startRowIndex).GetCell(colIndexLeft);
                cellLeft.SetCellValue(pair.Key);
                XSSFCell cellRight = (XSSFCell)sheet.GetRow(startRowIndex).GetCell(colIndexRight);
                cellRight.SetCellValue(pair.Value);
                startRowIndex++;
            }

        }

        public ActionResult GenerateExcelDayShift()
        {
            string filename = "DailyLogDay_BaseTemplate.xlsx";
            //string filename = "DailyLogDay_BaseTemplate2.xls";

            var excel = this.ProcessExcel(filename);
            //var excel = this.ProcessExcel3(filename);

            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", String.Format("DailyLogDayShift_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy")));
            //return File(excel, "application/octet-stream", "DailyLogDayShift_" + DateTime.Now.ToString("ddMMyyyy"));
        }

        public ActionResult GenerateExcelNightShift()
        {
            string filename = "DailyLogNight_BaseTemplate.xlsx";

            var excel = this.ProcessExcel(filename,false);

            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", String.Format("DailyLogNightShift_{0}.xlsx", DateTime.Now.ToString("ddMMyyyy")));
            //return File(excel, "application/octet-stream", "DailyLogNightShift_" + DateTime.Now.ToString("ddMMyyyy"));
        }

        private byte[] ProcessExcel(string filename, bool isDay=true)
        {
            //culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //supaya file tidak corrupt
            int parseRecordNumber = 1000;
            int startRowIndex = 4;

            XSSFCellStyle styleCurrency;
            XSSFCellStyle styleDate;
            XSSFCellStyle styleNumeric;

            //kamus
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet; XSSFRow row; XSSFCell cell;

            XSSFCellStyle style; XSSFFont font;
            XSSFCellStyle style2;
            XSSFCellStyle style3;

            CellRangeAddressList addressList; XSSFDataValidationHelper dvHelper; XSSFDataValidationConstraint dvConstraint; XSSFDataValidation validation;

            sheet = (XSSFSheet)workbook.CreateSheet("Data");
            sheet.DisplayGridlines = false;
            int col = 0;
            int rowNumber = 0;

            var numericFormat = workbook.CreateDataFormat().GetFormat("#.00#"); 


            #region Title
            //PICTURE
                FileStream picFile = new FileStream(Server.MapPath("~/Content/image/excel-logo.jpg"), FileMode.Open, FileAccess.Read);
                var memoryStream = new MemoryStream();
                picFile.CopyTo(memoryStream);
                byte[] picBytes = memoryStream.ToArray();
                int pictureIdx = workbook.AddPicture(picBytes, PictureType.JPEG);
                picFile.Close();
                memoryStream.Close();

                XSSFCreationHelper helper = (XSSFCreationHelper)workbook.GetCreationHelper();

                // Create the drawing patriarch.  This is the top level container for all shapes. 
                XSSFDrawing drawing = (XSSFDrawing)sheet.CreateDrawingPatriarch();

                //add a picture shape
                XSSFClientAnchor anchor = (XSSFClientAnchor)helper.CreateClientAnchor();
                //set top-left corner of the picture,
                //subsequent call of Picture#resize() will operate relative to it
                anchor.Col1 = 0;
                anchor.Row1 = 0;

                XSSFPicture pict = (XSSFPicture)drawing.CreatePicture(anchor, pictureIdx);

                //auto-size picture relative to its top-left corner
                pict.Resize();

                //TITLE
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                dvHelper = new XSSFDataValidationHelper(sheet);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                cell = (XSSFCell)row.CreateCell(2);
                cell.SetCellValue("STAR ENERGY GEOTHERMAL (WAYANG WINDU) LTD.");
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 16;
                font.FontName = "Times New Roman";
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.IsItalic = true;
                style.SetFont(font);
                cell.CellStyle = style;
                cell.CellStyle.IsLocked = true;
                sheet.AddMergedRegion(new CellRangeAddress(
                                        rowNumber, //first row (0-based)
                                        rowNumber, //last row  (0-based)
                                        2, //first column (0-based)
                                        13  //last column  (0-based)
                                ));

                //SubTitle
                rowNumber += 3;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                dvHelper = new XSSFDataValidationHelper(sheet);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                cell = (XSSFCell)row.CreateCell(0);
                cell.SetCellValue("Wayang Windu Geothermal Power Plant");
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 10;
                font.FontName = "Times New Roman";
                font.Boldweight = (short)FontBoldWeight.Bold;
                font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                cell.CellStyle = style;
                cell.CellStyle.IsLocked = true;

                //SubTitle 2
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                dvHelper = new XSSFDataValidationHelper(sheet);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                cell = (XSSFCell)row.CreateCell(0);
                cell.SetCellValue("Control Room Log Book");
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 9;
                font.FontName = "Times New Roman";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                cell.CellStyle = style;
                cell.CellStyle.IsLocked = true;
            #endregion

            #region Header1
                //STYLING
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                for (int i = 0; i < 14; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                    cell.CellStyle.IsLocked = true;
                }

                //DATE
                cell = (XSSFCell)row.GetCell(0);
                cell.SetCellValue("Date");
                sheet.AddMergedRegion(new CellRangeAddress(
                                        rowNumber, //first row (0-based)
                                        rowNumber, //last row  (0-based)
                                        0, //first column (0-based)
                                        2  //last column  (0-based)
                                ));

                //GROUP
                cell = (XSSFCell)row.GetCell(3);
                cell.SetCellValue("Group");
                cell.CellStyle.BorderLeft = BorderStyle.Thin;

                //PRODUCTION FOREMAN
                cell = (XSSFCell)row.GetCell(4);
                cell.SetCellValue("Production Foreman");
                cell.CellStyle.BorderLeft = BorderStyle.Thin;
                sheet.AddMergedRegion(new CellRangeAddress(
                                        rowNumber, //first row (0-based)
                                        rowNumber, //last row  (0-based)
                                        4, //first column (0-based)
                                        5  //last column  (0-based)
                                ));

                //PRODUCTION OPERATORS
                cell = (XSSFCell)row.GetCell(6);
                cell.SetCellValue("Production Operators");
                cell.CellStyle.BorderLeft = BorderStyle.Thin;
                sheet.AddMergedRegion(new CellRangeAddress(
                                        rowNumber, //first row (0-based)
                                        rowNumber, //last row  (0-based)
                                        6, //first column (0-based)
                                        13  //last column  (0-based)
                                ));
            #endregion

            #region Header1Contents

            //STYLING
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);

            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            //font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Medium;
            style.IsLocked = false;
            for (int i = 0; i < 14; i++)
            {
                for (int j = 1; j >= 0; j--)
                {
                    row = (XSSFRow)sheet.GetRow(rowNumber - j);
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                    cell.CellStyle.IsLocked = false;
                }
                
            }

            col = 0;

            //DATE CONTENTS
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               0, //first column (0-based)
               2  //last column  (0-based)
            ));
            row = (XSSFRow)sheet.GetRow((short)rowNumber-1);
            cell = (XSSFCell)row.GetCell(col);
            cell.SetCellValue(DateTime.Now.Date);
            cell.CellStyle = style;
            cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("MMMM d, yyyy");

            //GROUP CONTENTS
            col += 3;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               3, //first column (0-based)
               3  //last column  (0-based)
            ));
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Center;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Medium;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Medium;
            style2.IsLocked = false;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.GetCell(col);
            cell.CellStyle = style2;
            cell.CellStyle.SetFont(font);
            cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("MMMM d, yyyy");

            //PRODUCTION FOREMAN CONTENTS
            col ++;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               4, //first column (0-based)
               5  //last column  (0-based)
            ));
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.GetCell(col);
            cell.CellStyle = style;


            //OPERATOR 1 CONTENT
            col+=2;
            row = (XSSFRow)sheet.GetRow((short)rowNumber -1);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Medium;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Thin;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col+1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber - 1, //last row  (0-based)
               col, //first column (0-based)
               col+1  //last column  (0-based)
            ));

            //OPERATOR 2 CONTENT
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Medium;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 3 CONTENT
            col +=2;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Medium;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Thin;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber - 1, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 4 CONTENT
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Medium;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 5 CONTENT
            col+=2;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Medium;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Thin;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber - 1, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 6 CONTENT
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Medium;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 7 CONTENT
            col+=2;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Medium;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Thin;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber - 1, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            //OPERATOR 8 CONTENT
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.GetCell(col);
            style2 = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style2.SetFont(font);
            style2.Alignment = HorizontalAlignment.Left;
            style2.VerticalAlignment = VerticalAlignment.Center;
            style2.BorderTop = BorderStyle.Thin;
            style2.BorderLeft = BorderStyle.Thin;
            style2.BorderRight = BorderStyle.Thin;
            style2.BorderBottom = BorderStyle.Medium;
            style2.IsLocked = false;
            cell.CellStyle = style2;
            cell = (XSSFCell)row.GetCell(col + 1);
            cell.CellStyle = style2;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 1  //last column  (0-based)
            ));

            #endregion

            #region Header2

            //PRELIMINARY
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);

            //TIME
            col = 0;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Medium;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("TIME");
            cell.CellStyle = style;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col  //last column  (0-based)
            ));

            //DESCRIPTION
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("DESCRIPTION");
            cell.CellStyle = style;
            for (int i = col+1; i <= 13; i++)
            {
                cell = (XSSFCell)row.CreateCell(i);
                cell.CellStyle = style;
            }
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber -1, //first row (0-based)
               rowNumber -1, //last row  (0-based)
               col, //first column (0-based)
               col+ 12  //last column  (0-based)
            ));

            //PRODUCTION AND INJECTION WELLS
            col = 1;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Medium;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("PRODUCTION AND INJECTION WELLS");
            cell.CellStyle = style;
            for (int i = col + 1; i <= 8; i++)
            {
                cell = (XSSFCell)row.CreateCell(i);
                cell.CellStyle = style;
            }
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 7  //last column  (0-based)
            ));

            //POWER STATION
            col = 9;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Medium;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("POWER STATION");
            cell.CellStyle = style;
            for (int i = col + 1; i <= 13; i++)
            {
                cell = (XSSFCell)row.CreateCell(i);
                cell.CellStyle = style;
            }
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col + 4  //last column  (0-based)
            ));
            #endregion

            #region Row-13-14
            //PRELIMINARY
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);
            rowNumber++;
            row = (XSSFRow)sheet.CreateRow((short)rowNumber);

            //TIME
            col = 0;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            //style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            if (isDay == true)
            {
                cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 7, 0, 0));
            }
            else
            {
                cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 19, 0, 0));
            }
            cell.CellStyle = style;
            cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("h:mm");
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
            //style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;

            //Wells 1
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255,0,255,255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Wells");
            cell.CellStyle = style;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col  //last column  (0-based)
            ));


            //FCV 1
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("FCV");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("%");
            cell.CellStyle = style;

            //Flow 1
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Flow");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("(kg/s)");
            cell.CellStyle = style;

            //WHP 1
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("WHP");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("(Bar)");
            cell.CellStyle = style;

            //Wells 2
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Wells");
            cell.CellStyle = style;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col  //last column  (0-based)
            ));

            //FCV 2
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("FCV");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("%");
            cell.CellStyle = style;

            //Flow 2
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Flow");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("(kg/s)");
            cell.CellStyle = style;

            //WHP 2
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Double;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("WHP");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Double;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("(Bar)");
            cell.CellStyle = style;


            //POWER STATION NAME
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Double;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            cell = (XSSFCell)row.CreateCell(col + 1);
            cell.CellStyle = style;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            cell = (XSSFCell)row.CreateCell(col + 1);
            cell.CellStyle = style;
           
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col +1  //last column  (0-based)
            ));

            //TG UNIT 1
            col+=2;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("T/G");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Unit-1");
            cell.CellStyle = style;

            //TG UNIT 2
            col ++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.None;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("T/G");
            cell.CellStyle = style;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.None;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.SetCellValue("Unit-2");
            cell.CellStyle = style;

            //POWERSTATION MEASUREMENTS
            col++;
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.FontHeight = 8;
            font.FontName = "Arial";
            font.Boldweight = (short)FontBoldWeight.Bold;
            //font.Underline = FontUnderlineType.Single;
            style.SetFont(font);
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderTop = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Double;
            row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            row = (XSSFRow)sheet.GetRow((short)rowNumber);
            cell = (XSSFCell)row.CreateCell(col);
            cell.CellStyle = style;
            sheet.AddMergedRegion(new CellRangeAddress(
               rowNumber - 1, //first row (0-based)
               rowNumber, //last row  (0-based)
               col, //first column (0-based)
               col  //last column  (0-based)
            ));

            #endregion

            #region Well and Powerstation
            var dataWell = (from a in db.daily_log_wells
                            where a.is_delete != true
                            select a).OrderBy(x => x.name).ToList();
            startRowIndex = 13;
            int colIndexLeft = 1;
            int colIndexRight = 5;
            int maxWellCount = 30;
            int halfMaxWellCount = maxWellCount / 2;


            if (dataWell != null)
            {
                //sheet.ProtectSheet("starenergy");
                //style = (XSSFCellStyle)workbook.CreateCellStyle();
                //style.IsLocked = false;
                //cell = (XSSFCell)sheet.GetRow(5).GetCell(0);
                //cell.CellStyle = style;



                if (dataWell.Count <= maxWellCount)
                {
                    for (int i = 0; i < halfMaxWellCount; i++)
                    {
                        //TIME
                        row = (XSSFRow)sheet.CreateRow(startRowIndex + i);
                        cell = (XSSFCell)row.CreateCell(0);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.None;
                        style.BorderBottom = BorderStyle.None;
                        cell.CellStyle = style;

                        //WELLS
                        cell = (XSSFCell)row.CreateCell(1);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        cell.CellStyle = style;

                        //FCV
                        cell = (XSSFCell)row.CreateCell(2);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //FLOW
                        cell = (XSSFCell)row.CreateCell(3);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WHP
                        cell = (XSSFCell)row.CreateCell(4);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WELLS 2nd
                        cell = (XSSFCell)row.CreateCell(5);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        cell.CellStyle = style;

                        //FCV 2nd
                        cell = (XSSFCell)row.CreateCell(6);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //FLOW 2nd
                        cell = (XSSFCell)row.CreateCell(7);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WHP 2nd
                        cell = (XSSFCell)row.CreateCell(8);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Double;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;


                        //Power Station Name?
                        cell = (XSSFCell)row.CreateCell(9);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFont(font);
                        cell.CellStyle = style;
                        cell = (XSSFCell)row.CreateCell(10);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderBottom = BorderStyle.Hair;
                        cell.CellStyle = style;
                        sheet.AddMergedRegion(new CellRangeAddress(
                                startRowIndex + i, //first row (0-based)
                                startRowIndex + i, //last row  (0-based)
                                9, //first column (0-based)
                                10  //last column  (0-based)
                        ));

                        //TG UNIT 1
                        cell = (XSSFCell)row.CreateCell(11);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //TG UNIT 2
                        cell = (XSSFCell)row.CreateCell(12);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //Powerstation Measurement Unit?
                        cell = (XSSFCell)row.CreateCell(13);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        cell.CellStyle = style;
                    }

                    int columnOnLeft = 0;
                    int columnOnRight = 0;
                    if (dataWell.Count % 2 == 1)
                    {
                        columnOnLeft = (dataWell.Count / 2) + 1;
                        columnOnRight = dataWell.Count - columnOnLeft;
                    }
                    else
                    {
                        columnOnLeft = (dataWell.Count / 2);
                        columnOnRight = dataWell.Count - columnOnLeft;
                    }
                    List<daily_log_wells> dataOnLeft = new List<daily_log_wells>();
                    List<daily_log_wells> dataOnRight = new List<daily_log_wells>();

                    dataOnLeft = dataWell.Take(columnOnLeft).ToList();
                    if (columnOnRight > 0)
                    {
                        dataOnRight = dataWell.Skip(columnOnLeft).Take(columnOnRight).ToList();
                    }

                    if (dataOnLeft.Count > 0)
                    {
                        int incrementIndex = 0;
                        foreach (daily_log_wells well in dataOnLeft)
                        {
                            cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft);
                            cell.SetCellValue(well.name);
                            incrementIndex++;
                        }
                    }
                    if (dataOnRight.Count > 0)
                    {
                        int incrementIndex = 0;
                        foreach (daily_log_wells well in dataOnRight)
                        {
                            cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight);
                            cell.SetCellValue(well.name);
                            incrementIndex++;
                        }
                    }
                    
                    
                }
                else
                {
                    int remainder = dataWell.Count - maxWellCount;
                    int rowsNeedToBeCreated;
                    int lastWellRowIndex = 28;
                    if (remainder % 2 == 1)
                    {
                        rowsNeedToBeCreated = (remainder / 2) + 1;
                    }
                    else
                    {
                        rowsNeedToBeCreated = remainder / 2;
                    }


                    for (int i = 0; i < halfMaxWellCount + rowsNeedToBeCreated; i++)
                    {
                        //TIME
                        row = (XSSFRow)sheet.CreateRow(startRowIndex + i);
                        cell = (XSSFCell)row.CreateCell(0);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.None;
                        style.BorderBottom = BorderStyle.None;
                        cell.CellStyle = style;

                        //WELLS
                        cell = (XSSFCell)row.CreateCell(1);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        cell.CellStyle = style;

                        //FCV
                        cell = (XSSFCell)row.CreateCell(2);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //FLOW
                        cell = (XSSFCell)row.CreateCell(3);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WHP
                        cell = (XSSFCell)row.CreateCell(4);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WELLS 2nd
                        cell = (XSSFCell)row.CreateCell(5);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        cell.CellStyle = style;

                        //FCV 2nd
                        cell = (XSSFCell)row.CreateCell(6);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //FLOW 2nd
                        cell = (XSSFCell)row.CreateCell(7);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //WHP 2nd
                        cell = (XSSFCell)row.CreateCell(8);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Double;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;


                        //Power Station Name?
                        cell = (XSSFCell)row.CreateCell(9);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFont(font);
                        cell.CellStyle = style;
                        cell = (XSSFCell)row.CreateCell(10);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderBottom = BorderStyle.Hair;
                        cell.CellStyle = style;
                        sheet.AddMergedRegion(new CellRangeAddress(
                                startRowIndex + i, //first row (0-based)
                                startRowIndex + i, //last row  (0-based)
                                9, //first column (0-based)
                                10  //last column  (0-based)
                        ));

                        //TG UNIT 1
                        cell = (XSSFCell)row.CreateCell(11);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //TG UNIT 2
                        cell = (XSSFCell)row.CreateCell(12);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //Powerstation Measurement Unit?
                        cell = (XSSFCell)row.CreateCell(13);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        cell.CellStyle = style;
                    }

                    int incrementIndex = 0;
                    bool isOnLeft = true;
                    foreach (daily_log_wells well in dataWell)
                    {
                        if (isOnLeft)
                        {
                            cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft);
                            cell.SetCellValue(well.name);
                            incrementIndex++;
                            if (incrementIndex == (halfMaxWellCount + rowsNeedToBeCreated))
                            {
                                isOnLeft = false;
                                incrementIndex = 0;
                            }
                        }
                        else
                        {
                            cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight);
                            cell.SetCellValue(well.name);
                            incrementIndex++;
                        }

                    }
                    //this.FillPowerStation(ref sheet);
                }

                //BELOW WELL
                rowNumber = sheet.LastRowNum;

                rowNumber++;
                int belowWellStartRow = rowNumber;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);

                //TIME
                col = 0;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                //if (isDay == true)
                //{
                //    cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 7, 0, 0));
                //}
                //else
                //{
                //    cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 19, 0, 0));
                //}
                
                //cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("h:mm");


                //U1 NCG
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U1 NCG");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Flow(kg/s)");
                cell.CellStyle = style;


                //U2 NCG
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U2 NCG");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Flow(kg/s)");
                cell.CellStyle = style;

                //U1 Turbine
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U1 Turbine");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Exp.(mm)");
                cell.CellStyle = style;

                //U2 Turbine
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U2 Turbine");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Exp.(mm)");
                cell.CellStyle = style;

                //U1 CT
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U1 CT");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Temp.(°C)");
                cell.CellStyle = style;

                //U2 CT
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("U2 CT");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Temp.(°C)");
                cell.CellStyle = style;

                //T1 Exhaust
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("T1 Exhaust");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Temp.(°C)");
                cell.CellStyle = style;

                //T2 Exhaust
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("T2 Exhaust");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Temp.(°C)");
                cell.CellStyle = style;

               
                //Content Placeholder 1
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);

                //0
                cell = (XSSFCell)row.CreateCell(0);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //1
                cell = (XSSFCell)row.CreateCell(1);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //2
                cell = (XSSFCell)row.CreateCell(2);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //3
                cell = (XSSFCell)row.CreateCell(3);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //4
                cell = (XSSFCell)row.CreateCell(4);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //5
                cell = (XSSFCell)row.CreateCell(5);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //6
                cell = (XSSFCell)row.CreateCell(6);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //7
                cell = (XSSFCell)row.CreateCell(7);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //8
                cell = (XSSFCell)row.CreateCell(8);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);

                //TIME
                col = 0;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                //if (isDay == true)
                //{
                //    cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 7, 0, 0));
                //}
                //else
                //{
                //    cell.SetCellValue(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 19, 0, 0));
                //}
                
                //cell.CellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("h:mm");


                //Upper TP
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Upper TP");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Level(m)");
                cell.CellStyle = style;


                //Lower TP
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Lower TP");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Level(m)");
                cell.CellStyle = style;

                //MV-333
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MV-333");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Position(%)");
                cell.CellStyle = style;

                //MV-334
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MV-334");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Position(%)");
                cell.CellStyle = style;

                //Brine Level
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Brine Level");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("(mm)");
                cell.CellStyle = style;

                //Condensate
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Condensate");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Level(mm)");
                cell.CellStyle = style;

                //NaOH 48%
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("NaOH 48%");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Level(mm)");
                cell.CellStyle = style;

                //WWD Pond
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Medium;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.None;
                row = (XSSFRow)sheet.GetRow((short)rowNumber - 1);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("WWD Pond");
                cell.CellStyle = style;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundXSSFColor = new XSSFColor(System.Drawing.Color.FromArgb(255, 0, 255, 255));
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.Double;
                row = (XSSFRow)sheet.GetRow((short)rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Level(m)");
                cell.CellStyle = style;


                //Content Placeholder 2
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);

                //0
                cell = (XSSFCell)row.CreateCell(0);
                if (isDay == false)
                {
                    style = (XSSFCellStyle)workbook.CreateCellStyle();
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderBottom = BorderStyle.Thin;
                    cell.CellStyle = style;
                }
                else
                {
                    style = (XSSFCellStyle)workbook.CreateCellStyle();
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderBottom = BorderStyle.None;
                    cell.CellStyle = style;
                }
                

                //1
                cell = (XSSFCell)row.CreateCell(1);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //2
                cell = (XSSFCell)row.CreateCell(2);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //3
                cell = (XSSFCell)row.CreateCell(3);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //4
                cell = (XSSFCell)row.CreateCell(4);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //5
                cell = (XSSFCell)row.CreateCell(5);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //6
                cell = (XSSFCell)row.CreateCell(6);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //7
                cell = (XSSFCell)row.CreateCell(7);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //8
                cell = (XSSFCell)row.CreateCell(8);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Double;
                style.BorderRight = BorderStyle.Double;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                for (int i = belowWellStartRow; i <= rowNumber; i++)
                {
                    if (i == rowNumber && isDay == false)
                    {
                        row = (XSSFRow)sheet.GetRow((short)i);
                        //Power Station Name?
                        cell = (XSSFCell)row.CreateCell(9);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.BorderBottom = BorderStyle.Thin;
                        style.SetFont(font);
                        cell.CellStyle = style;
                        cell = (XSSFCell)row.CreateCell(10);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle = style;
                        sheet.AddMergedRegion(new CellRangeAddress(
                                i, //first row (0-based)
                                i, //last row  (0-based)
                                9, //first column (0-based)
                                10  //last column  (0-based)
                        ));

                        //TG UNIT 1
                        cell = (XSSFCell)row.CreateCell(11);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Thin;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //TG UNIT 2
                        cell = (XSSFCell)row.CreateCell(12);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Thin;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //Powerstation Measurement Unit?
                        cell = (XSSFCell)row.CreateCell(13);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Thin;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        cell.CellStyle = style;
                    }
                    else
                    {
                        row = (XSSFRow)sheet.GetRow((short)i);
                        //Power Station Name?
                        cell = (XSSFCell)row.CreateCell(9);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFont(font);
                        cell.CellStyle = style;
                        cell = (XSSFCell)row.CreateCell(10);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderBottom = BorderStyle.Hair;
                        cell.CellStyle = style;
                        sheet.AddMergedRegion(new CellRangeAddress(
                                i, //first row (0-based)
                                i, //last row  (0-based)
                                9, //first column (0-based)
                                10  //last column  (0-based)
                        ));

                        //TG UNIT 1
                        cell = (XSSFCell)row.CreateCell(11);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //TG UNIT 2
                        cell = (XSSFCell)row.CreateCell(12);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                        style.FillPattern = FillPattern.SolidForeground;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        style.IsLocked = false;
                        cell.CellStyle = style;

                        //Powerstation Measurement Unit?
                        cell = (XSSFCell)row.CreateCell(13);
                        style = (XSSFCellStyle)workbook.CreateCellStyle();
                        style.BorderLeft = BorderStyle.Thin;
                        style.BorderTop = BorderStyle.Hair;
                        style.BorderRight = BorderStyle.Thin;
                        style.BorderBottom = BorderStyle.Hair;
                        font = (XSSFFont)workbook.CreateFont();
                        font.FontName = "Arial";
                        font.FontHeight = 8;
                        style.SetFont(font);
                        style.DataFormat = numericFormat;
                        cell.CellStyle = style;
                    }
                    
                }

                this.FillPowerStation(ref sheet);

            }


            #endregion

            if (isDay == true)
            {
                #region MeteringAndDispatch

                #region Row1
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;

                //TIME
                row = (XSSFRow)sheet.GetRow(rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                cell.CellStyle = style;

                //Title Metering and Dispatch
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= 13; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("METERING AND DISPATCH");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 12  //last column  (0-based)
                 ));
                #endregion

                #region Row2

                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                row = (XSSFRow)sheet.GetRow(rowNumber);
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                cell.CellStyle = style;

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                for (int i = col; i <= 3; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 2  //last column  (0-based)
                 ));

                //UNIT 1
                col += 3;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Unit-1");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;

                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Unit-2");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                for (int i = col; i <= col + 2; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 2  //last column  (0-based)
                 ));


                //UNIT 1
                col += 3;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Unit-1");
                cell.CellStyle = style;

                //UNIT 2
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Unit-2");
                cell.CellStyle = style;

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Double;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;

                #endregion

                #region Row3
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Unit Transformer
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Unit Transformer");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Active
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Active");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                //SEGWWL Availability
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("SEGWWL Availability");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row4
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Import
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Import");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Reactive
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Reactive");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //MVarh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MVarh");
                cell.CellStyle = style;

                //PLN Dispatch
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("PLN Dispatch");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row5
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Steam
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Steam");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Active
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Active");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Ton");
                cell.CellStyle = style;

                //PLN Meter
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("PLN Meter");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row6
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Consumption
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Consumption");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Auxiliary
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Auxiliary");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Ton");
                cell.CellStyle = style;

                //SEGWWL Export
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("SEGWWL Export");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row7
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Production
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 7; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Production");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 7  //last column  (0-based)
                 ));

                //Actual Export
                col+=8;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Actual Export");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;


                #endregion

                #region Row8
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Generator Export
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Generator Export");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Active
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Active");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                //Production Excess
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Production Excess");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row9
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //(gross)
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("(gross)");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Reactive
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Reactive");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //Mvarh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Mvarh");
                cell.CellStyle = style;

                //Losses
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 4; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Losses");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 4  //last column  (0-based)
                 ));

                #endregion

                #region Row10
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Metering at 10:00
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Metering at 10:00");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //SEGWWL
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("SEGWWL");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                //RPF
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("RPF");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row11
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //PLN
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("PLN");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                //PGF
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("PGF");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row12
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //Condensate
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Condensate");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //P/S
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("P/S");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Hair;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Ton");
                cell.CellStyle = style;

                //PLN
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Production Excess");
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 1, //first column (0-based)
                col  //last column  (0-based)
                 ));

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;

                //MWh
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("MWh");
                cell.CellStyle = style;

                #endregion

                #region Row13
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.None;
                cell.CellStyle = style;

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Total
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Total");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 3, //first column (0-based)
                col //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Ton");
                cell.CellStyle = style;

                //Note
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.None;
                for (int i = col; i <= col + 4; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Note:");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 4  //last column  (0-based)
                 ));

                #endregion

                #region Row14
                rowNumber++;
                row = (XSSFRow)sheet.CreateRow((short)rowNumber);
                col = 0;


                //TIME
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.None;
                style.BorderBottom = BorderStyle.Thin;
                cell.CellStyle = style;

                //Brine
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                for (int i = col; i <= col + 1; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    if (i == col)
                    {
                        cell.SetCellValue("Brine");
                    }
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 1  //last column  (0-based)
                 ));

                //Total
                col += 2;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Total");
                cell.CellStyle = style;

                //UNIT 1
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                //UNIT 2
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                style.BorderLeft = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Hair;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                style.FillPattern = FillPattern.SolidForeground;
                font = (XSSFFont)workbook.CreateFont();
                font.FontName = "Arial";
                font.FontHeight = 8;
                style.SetFont(font);
                style.DataFormat = numericFormat;
                style.IsLocked = false;
                cell.CellStyle = style;
                col++;
                cell = (XSSFCell)row.CreateCell(col);
                cell.CellStyle = style;
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col - 3, //first column (0-based)
                col //last column  (0-based)
                 ));

                //Ton
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                //font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                cell = (XSSFCell)row.CreateCell(col);
                cell.SetCellValue("Ton");
                cell.CellStyle = style;

                //
                col++;
                style = (XSSFCellStyle)workbook.CreateCellStyle();
                font = (XSSFFont)workbook.CreateFont();
                font.FontHeight = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)FontBoldWeight.Bold;
                //font.Underline = FontUnderlineType.Single;
                style.SetFont(font);
                style.Alignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Center;
                //style.FillForegroundColor = (short)IndexedColors.Yellow.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                style.BorderTop = BorderStyle.None;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderBottom = BorderStyle.Thin;
                style.IsLocked = false;
                for (int i = col; i <= col + 4; i++)
                {
                    cell = (XSSFCell)row.CreateCell(i);
                    cell.CellStyle = style;
                }
                sheet.AddMergedRegion(new CellRangeAddress(
                rowNumber, //first row (0-based)
                rowNumber, //last row  (0-based)
                col, //first column (0-based)
                col + 4  //last column  (0-based)
                 ));

                #endregion

                #endregion
            }
            sheet.ProtectSheet("starenergy");
            //write to byte[]
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);

            return ms.ToArray();
        }

        private byte[] ProcessExcelOLD(string filename)
        {
            byte[] excel = null;
            XSSFSheet sheet;
            XSSFCellStyle style;
            XSSFRow row;
            XSSFCell cell;
            XSSFFont font;

            string currentFilePathInServer = Server.MapPath("~/App_Data/daily_log/" + filename);

            XSSFWorkbook book;
            try
            {


                using (FileStream file = new FileStream(currentFilePathInServer, FileMode.Open, FileAccess.Read))
                {
                    book = new XSSFWorkbook(file);
                }

                sheet = (XSSFSheet)book.GetSheet("FormFracas");
                

                var dataWell = (from a in db.daily_log_wells
                                where a.is_delete != true
                                select a).OrderBy(x => x.name).ToList();
                int startRowIndex = 13;
                int colIndexLeft = 1;
                int colIndexRight = 5;
                int maxWellCount = 30;
                int halfMaxWellCount = maxWellCount / 2;


                if (dataWell != null)
                {
                    
                    //SET DATE
                    XSSFCell cellDate = (XSSFCell)sheet.GetRow(7).GetCell(0);
                    cellDate.SetCellValue(DateTime.Now);

                    //sheet.ProtectSheet("starenergy");
                    //style = (XSSFCellStyle)book.CreateCellStyle();
                    //style.IsLocked = false;
                    //cell = (XSSFCell)sheet.GetRow(5).GetCell(0);
                    //cell.CellStyle = style;

                    if (dataWell.Count <= halfMaxWellCount)
                    {
                        int incrementIndex = 0;
                        foreach (daily_log_wells well in dataWell)
                        {
                            cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft);
                            cell.SetCellValue(well.name);


                            ////UNLOCK FCV
                            //style = (XSSFCellStyle)book.CreateCellStyle();
                            //style.IsLocked = false;
                            //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 1);
                            //cell.CellStyle = style;

                            ////UNLOCK Flow
                            //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 2);
                            //cell.CellStyle = style;

                            ////UNLOCK WHP
                            //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 3);
                            //cell.CellStyle = style;

                            incrementIndex++;
                        }
                        this.FillPowerStation(ref sheet);
                    }
                    else if (dataWell.Count > halfMaxWellCount && dataWell.Count <= maxWellCount)
                    {
                        int incrementIndex = 0;
                        bool isOnLeft = true;
                        foreach (daily_log_wells well in dataWell)
                        {
                            if (isOnLeft)
                            {
                                cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft);
                                cell.SetCellValue(well.name);
                                incrementIndex++;

                                ////UNLOCK FCV
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 1);
                                //cell.CellStyle = style;

                                ////UNLOCK Flow
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 2);
                                //cell.CellStyle = style;

                                ////UNLOCK WHP
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft + 3);
                                //cell.CellStyle = style;

                                if (incrementIndex == halfMaxWellCount)
                                {
                                    isOnLeft = false;
                                    incrementIndex = 0;
                                }
                            }
                            else
                            {
                                cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight);
                                cell.SetCellValue(well.name);

                                ////UNLOCK FCV
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight + 1);
                                //cell.CellStyle = style;

                                ////UNLOCK Flow
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight + 2);
                                //cell.CellStyle = style;

                                ////UNLOCK WHP
                                //style = (XSSFCellStyle)book.CreateCellStyle();
                                //style.IsLocked = false;
                                //cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight + 3);
                                //cell.CellStyle = style;

                                incrementIndex++;
                            }

                        }
                        this.FillPowerStation(ref sheet);
                    }
                    else
                    {
                        int remainder = dataWell.Count - maxWellCount;
                        int rowsNeedToBeCreated;
                        int lastWellRowIndex = 28;
                        if (remainder % 2 == 1)
                        {
                            rowsNeedToBeCreated = (remainder / 2) + 1;
                        }
                        else
                        {
                            rowsNeedToBeCreated = remainder / 2;
                        }

                        var numericFormat = book.CreateDataFormat().GetFormat("#.00#"); 
                        //Index,Total Rows,Total to Insert 
                        sheet.ShiftRows(lastWellRowIndex, sheet.LastRowNum, rowsNeedToBeCreated);

                        for (int i = 0; i < rowsNeedToBeCreated; i++)
                        {
                            //TIME
                            row = (XSSFRow)sheet.CreateRow((short)28 + i);
                            cell = (XSSFCell)row.CreateCell(0);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Thick;
                            style.BorderTop = BorderStyle.Thick;
                            cell.CellStyle = style;

                            //WELLS
                            cell = (XSSFCell)row.CreateCell(1);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            cell.CellStyle = style;

                            //FCV
                            cell = (XSSFCell)row.CreateCell(2);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //FLOW
                            cell = (XSSFCell)row.CreateCell(3);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //WHP
                            cell = (XSSFCell)row.CreateCell(4);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //WELLS 2nd
                            cell = (XSSFCell)row.CreateCell(5);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            cell.CellStyle = style;

                            //FCV 2nd
                            cell = (XSSFCell)row.CreateCell(6);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //FLOW 2nd
                            cell = (XSSFCell)row.CreateCell(7);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //WHP 2nd
                            cell = (XSSFCell)row.CreateCell(8);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Double;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;


                            //Power Station Name?
                            cell = (XSSFCell)row.CreateCell(9);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            cell.CellStyle = style;
                            cell = (XSSFCell)row.CreateCell(10);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            cell.CellStyle = style;
                            sheet.AddMergedRegion(new CellRangeAddress(
                                    28 + i, //first row (0-based)
                                    28 + i, //last row  (0-based)
                                    9, //first column (0-based)
                                    10  //last column  (0-based)
                            ));

                            //TG UNIT 1
                            cell = (XSSFCell)row.CreateCell(11);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //TG UNIT 2
                            cell = (XSSFCell)row.CreateCell(12);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thin;
                            style.BorderBottom = BorderStyle.Hair;
                            style.SetFillForegroundColor(new XSSFColor(System.Drawing.Color.FromArgb(255, 192, 192, 192)));
                            style.FillPattern = FillPattern.SolidForeground;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;

                            //Powerstation Measurement Unit?
                            cell = (XSSFCell)row.CreateCell(13);
                            style = (XSSFCellStyle)book.CreateCellStyle();
                            style.BorderLeft = BorderStyle.Thin;
                            style.BorderTop = BorderStyle.Hair;
                            style.BorderRight = BorderStyle.Thick;
                            style.BorderBottom = BorderStyle.Hair;
                            font = (XSSFFont)book.CreateFont();
                            font.FontName = "Arial";
                            font.FontHeight = 8;
                            style.SetFont(font);
                            style.DataFormat = numericFormat;
                            cell.CellStyle = style;
                        }

                        int incrementIndex = 0;
                        bool isOnLeft = true;
                        foreach (daily_log_wells well in dataWell)
                        {
                            if (isOnLeft)
                            {
                                cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexLeft);
                                cell.SetCellValue(well.name);
                                incrementIndex++;
                                if (incrementIndex == (halfMaxWellCount + rowsNeedToBeCreated))
                                {
                                    isOnLeft = false;
                                    incrementIndex = 0;
                                }
                            }
                            else
                            {
                                cell = (XSSFCell)sheet.GetRow(startRowIndex + incrementIndex).GetCell(colIndexRight);
                                cell.SetCellValue(well.name);
                                incrementIndex++;
                            }

                        }
                        this.FillPowerStation(ref sheet);
                        //sheet.AddMergedRegion(new CellRangeAddress(
                        //            12, //first row (0-based)
                        //            sheet.LastRowNum - 1, //last row  (0-based)
                        //            0, //first column (0-based)
                        //            0  //last column  (0-based)
                        //));
                    }


                }

                

                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                excel = ms.ToArray();

            }
            catch (Exception e)
            {
                Response.Write("Internal Server Error. Please Contact Administrator <br/>");
                Response.Write(e.ToString());
                Response.End();
            }

            return excel;
        }

       
    }
}
