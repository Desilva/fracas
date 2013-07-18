using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class DailyLogController : Controller
    {
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        
        //
        // GET: /DailyLog/

        public ActionResult Index()
        {
            var has = (from users in db.users
                       select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
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
            return PartialView();
        }

        public ActionResult weeklyTarget()
        {
            ////Debug.WriteLine(DateTime.Now);
            DateTime now = DateTime.Now.Date;
            //DateTime d = new DateTime(now.Year, now.Month, now.Day);
            ViewBag.today = db.daily_log_weekly_target.Where(p => p.date == now).OrderBy(p => p.shift).ToList();
            return PartialView();
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

        public ActionResult addDailyLog(int? id)
        {
            if (id != null)
            {
                ViewBag.mod = id;
                daily_log shift1 = db.daily_log.Find(id);
                ViewBag.datas = shift1;
                if (shift1.id_shift2 != null)
                {
                    ViewBag.mod2 = shift1.id_shift2;
                    ViewBag.datas2 = db.daily_log.Find(shift1.id_shift2);
                }

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
            List<daily_log> f = new List<daily_log>();
            f = db.daily_log.Where(p => p.shift == 1).ToList();

            return View(new GridModel<daily_log>
            {
                Data = f.OrderByDescending(p => p.date)
            });
        }

        [HttpPost]
        public JsonResult Add(daily_log dailyLog)
        {
            db.daily_log.Add(dailyLog);
            db.SaveChanges();

            int id = db.daily_log.Max(p => p.id);

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

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(daily_log dailyLog)
        {
            daily_log ir = db.daily_log.Find(dailyLog.id);

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
            ir.time_check = dailyLog.time_check;
            ir.wma_2_is_text = dailyLog.wma_2_is_text;
            ir.wma_2_fcv = dailyLog.wma_2_fcv;
            ir.wma_2_flow = dailyLog.wma_2_flow;
            ir.wma_2_whp = dailyLog.wma_2_whp;
            ir.wma_4_is_text = dailyLog.wma_4_is_text;
            ir.wma_4_fcv = dailyLog.wma_4_fcv;
            ir.wma_4_flow = dailyLog.wma_4_flow;
            ir.wma_4_whp = dailyLog.wma_4_whp;
            ir.wma_6_is_text = dailyLog.wma_6_is_text;
            ir.wma_6_fcv = dailyLog.wma_6_fcv;
            ir.wma_6_flow = dailyLog.wma_6_flow;
            ir.wma_6_whp = dailyLog.wma_6_whp;
            ir.mbd_1_is_text = dailyLog.mbd_1_is_text;
            ir.mbd_1_fcv = dailyLog.mbd_1_fcv;
            ir.mbd_1_flow = dailyLog.mbd_1_flow;
            ir.mbd_1_whp = dailyLog.mbd_1_whp;
            ir.mbd_2_is_text = dailyLog.mbd_2_is_text;
            ir.mbd_2_fcv = dailyLog.mbd_2_fcv;
            ir.mbd_2_flow = dailyLog.mbd_2_flow;
            ir.mbd_2_whp = dailyLog.mbd_2_whp;
            ir.mbd_3_is_text = dailyLog.mbd_3_is_text;
            ir.mbd_3_fcv = dailyLog.mbd_3_fcv;
            ir.mbd_3_flow = dailyLog.mbd_3_flow;
            ir.mbd_3_whp = dailyLog.mbd_3_whp;
            ir.mbd_4_is_text = dailyLog.mbd_4_is_text;
            ir.mbd_4_fcv = dailyLog.mbd_4_fcv;
            ir.mbd_4_flow = dailyLog.mbd_4_flow;
            ir.mbd_4_whp = dailyLog.mbd_4_whp;
            ir.mbd_5_is_text = dailyLog.mbd_5_is_text;
            ir.mbd_5_fcv = dailyLog.mbd_5_fcv;
            ir.mbd_5_flow = dailyLog.mbd_5_flow;
            ir.mbd_5_whp = dailyLog.mbd_5_whp;
            ir.wwq_1_is_text = dailyLog.wwq_1_is_text;
            ir.wwq_1_fcv = dailyLog.wwq_1_fcv;
            ir.wwq_1_flow = dailyLog.wwq_1_flow;
            ir.wwq_1_whp = dailyLog.wwq_1_whp;
            ir.wwq_2_is_text = dailyLog.wwq_2_is_text;
            ir.wwq_2_fcv = dailyLog.wwq_2_fcv;
            ir.wwq_2_flow = dailyLog.wwq_2_flow;
            ir.wwq_2_whp = dailyLog.wwq_2_whp;
            ir.wwq_3_is_text = dailyLog.wwq_3_is_text;
            ir.wwq_3_fcv = dailyLog.wwq_3_fcv;
            ir.wwq_3_flow = dailyLog.wwq_3_flow;
            ir.wwq_3_whp = dailyLog.wwq_3_whp;
            ir.wwq_4_is_text = dailyLog.wwq_4_is_text;
            ir.wwq_4_fcv = dailyLog.wwq_4_fcv;
            ir.wwq_4_flow = dailyLog.wwq_4_flow;
            ir.wwq_4_whp = dailyLog.wwq_4_whp;
            ir.wwq_5_is_text = dailyLog.wwq_5_is_text;
            ir.wwq_5_fcv = dailyLog.wwq_5_fcv;
            ir.wwq_5_flow = dailyLog.wwq_5_flow;
            ir.wwq_5_whp = dailyLog.wwq_5_whp;
            ir.mbe_3_is_text = dailyLog.mbe_3_is_text;
            ir.mbe_3_fcv = dailyLog.mbe_3_fcv;
            ir.mbe_3_flow = dailyLog.mbe_3_flow;
            ir.mbe_3_whp = dailyLog.mbe_3_whp;
            ir.mbe_4_is_text = dailyLog.mbe_4_is_text;
            ir.mbe_4_fcv = dailyLog.mbe_4_fcv;
            ir.mbe_4_flow = dailyLog.mbe_4_flow;
            ir.mbe_4_whp = dailyLog.mbe_4_whp;
            ir.mba_1_is_text = dailyLog.mba_1_is_text;
            ir.mba_1_fcv = dailyLog.mba_1_fcv;
            ir.mba_1_flow = dailyLog.mba_1_flow;
            ir.mba_1_whp = dailyLog.mba_1_whp;
            ir.mba_2_is_text = dailyLog.mba_2_is_text;
            ir.mba_2_fcv = dailyLog.mba_2_fcv;
            ir.mba_2_flow = dailyLog.mba_2_flow;
            ir.mba_2_whp = dailyLog.mba_2_whp;
            ir.mba_3_is_text = dailyLog.mba_3_is_text;
            ir.mba_3_fcv = dailyLog.mba_3_fcv;
            ir.mba_3_flow = dailyLog.mba_3_flow;
            ir.mba_3_whp = dailyLog.mba_3_whp;
            ir.mba_4_is_text = dailyLog.mba_4_is_text;
            ir.mba_4_fcv = dailyLog.mba_4_fcv;
            ir.mba_4_flow = dailyLog.mba_4_flow;
            ir.mba_4_whp = dailyLog.mba_4_whp;
            ir.mba_5_is_text = dailyLog.mba_5_is_text;
            ir.mba_5_fcv = dailyLog.mba_5_fcv;
            ir.mba_5_flow = dailyLog.mba_5_flow;
            ir.mba_5_whp = dailyLog.mba_5_whp;
            ir.mbb_1_is_text = dailyLog.mbb_1_is_text;
            ir.mbb_1_fcv = dailyLog.mbb_1_fcv;
            ir.mbb_1_flow = dailyLog.mbb_1_flow;
            ir.mbb_1_whp = dailyLog.mbb_1_whp;
            ir.mbb_2_is_text = dailyLog.mbb_2_is_text;
            ir.mbb_2_fcv = dailyLog.mbb_2_fcv;
            ir.mbb_2_flow = dailyLog.mbb_2_flow;
            ir.mbb_2_whp = dailyLog.mbb_2_whp;
            ir.mbb_3_is_text = dailyLog.mbb_3_is_text;
            ir.mbb_3_fcv = dailyLog.mbb_3_fcv;
            ir.mbb_3_flow = dailyLog.mbb_3_flow;
            ir.mbb_3_whp = dailyLog.mbb_3_whp;
            ir.mbb_4_is_text = dailyLog.mbb_4_is_text;
            ir.mbb_4_fcv = dailyLog.mbb_4_fcv;
            ir.mbb_4_flow = dailyLog.mbb_4_flow;
            ir.mbb_4_whp = dailyLog.mbb_4_whp;
            ir.mbb_5_is_text = dailyLog.mbb_5_is_text;
            ir.mbb_5_fcv = dailyLog.mbb_5_fcv;
            ir.mbb_5_flow = dailyLog.mbb_5_flow;
            ir.mbb_5_whp = dailyLog.mbb_5_whp;
            ir.mbb_6_is_text = dailyLog.mbb_6_is_text;
            ir.mbb_6_fcv = dailyLog.mbb_6_fcv;
            ir.mbb_6_flow = dailyLog.mbb_6_flow;
            ir.mbb_6_whp = dailyLog.mbb_6_whp;
            ir.wwf_1_is_text = dailyLog.wwf_1_is_text;
            ir.wwf_1_fcv = dailyLog.wwf_1_fcv;
            ir.wwf_1_flow = dailyLog.wwf_1_flow;
            ir.wwf_1_whp = dailyLog.wwf_1_whp;
            ir.wwf_3_is_text = dailyLog.wwf_3_is_text;
            ir.wwf_3_fcv = dailyLog.wwf_3_fcv;
            ir.wwf_3_flow = dailyLog.wwf_3_flow;
            ir.wwf_3_whp = dailyLog.wwf_3_whp;
            ir.www_1_is_text = dailyLog.www_1_is_text;
            ir.www_1_fcv = dailyLog.www_1_fcv;
            ir.www_1_flow = dailyLog.www_1_flow;
            ir.www_1_whp = dailyLog.www_1_whp;
            ir.wwp_1_is_text = dailyLog.wwp_1_is_text;
            ir.wwp_1_fcv = dailyLog.wwp_1_fcv;
            ir.wwp_1_flow = dailyLog.wwp_1_flow;
            ir.wwp_1_whp = dailyLog.wwp_1_whp;
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

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult Approve(daily_log dailyLog)
        {
            daily_log ir = db.daily_log.Find(dailyLog.id);
            daily_log dl = new daily_log();
            dl.date = ir.date;
            dl.shift = 2;
            db.daily_log.Add(dl);
            db.SaveChanges();
            int id = db.daily_log.Max(p => p.id);

            
            ir.is_approve = dailyLog.is_approve;
            ir.id_shift2 = id;
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
        public ActionResult _SelectAjaxOperationWarning()
        {
            return bindingOperationWarning();
        }

        //select data Last Plant Status
        private ViewResult bindingOperationWarning()
        {
            List<daily_log_operation_warning> f = new List<daily_log_operation_warning>();
            f = db.daily_log_operation_warning.ToList();
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
                if (lps.warning == null || lps.warning == "")
                {
                    ModelState.AddModelError("description", "Operation Warning required");
                }
                else
                {
                    create(lps);
                }
            }
            return bindingOperationWarning();
        }

        //insert data Last Plant Status
        public void create(daily_log_operation_warning lps)
        {
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
            f = db.daily_log_weekly_target.ToList();
            return View(new GridModel<daily_log_weekly_target>
            {
                Data = f.OrderByDescending(p => p.date).OrderBy(p => p.shift)
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
                if (lps.target == null || lps.target == "")
                {
                    ModelState.AddModelError("target", "Target required");
                }
                else
                {
                    create(lps);
                }
            }
            return bindingWeeklyTarget();
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
            return bindingWeeklyTarget();
        }

        //update data Last Plant Status
        private void update(daily_log_weekly_target lps)
        {
            db.Entry(lps).State = EntityState.Modified;
            db.SaveChanges();
        }

        //==========================================================

    }
}
