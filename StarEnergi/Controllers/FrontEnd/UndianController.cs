using ReportManagement;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class UndianController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /Undian/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Undian" });
            }
            string username = Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            if (!li.Exists(p => p.role == (int)Config.role.ADMINMASTERSHE))
            {
                return RedirectToAction("Index", "SheObservation");
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
                           dept_name = employees.department != null ? employees.department : dept.dept_name,
                           dept_id = employees.dept_id,
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewBag.user = has;
            ViewBag.ex = db.she_observation_undian_exception.ToList();

            return View();
        }

        #region reward master

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxReward()
        {
            return binding();
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxReward()
        {
            she_observation_undian_reward reward = new she_observation_undian_reward();
            if (TryUpdateModel(reward))
            {
                create(reward);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxReward(int id)
        {

            she_observation_undian_reward reward = db.she_observation_undian_reward.Find(id);

            if (TryUpdateModel(reward))
            {
                update(reward);
            }
            return binding();
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxReward(int id)
        {
            delete(id);
            return binding();
        }

        //select data reward
        private ViewResult binding()
        {
            
            var model = db.she_observation_undian_reward.ToList();
            return View(new GridModel<she_observation_undian_reward>
            {
                Data = model
            });
        }

        //insert data reward
        public void create(she_observation_undian_reward reward)
        {
            db.she_observation_undian_reward.Add(reward);
            db.SaveChanges();
        }

        //update data reward
        private void update(she_observation_undian_reward reward)
        {
            db.Entry(reward).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data reward
        private void delete(int id)
        {
            she_observation_undian_reward reward = db.she_observation_undian_reward.Find(id);
            db.she_observation_undian_reward.Remove(reward);
            db.SaveChanges();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        [HttpPost]
        public JsonResult startDraw(int[] employees, DateTime? from, DateTime? to, int percentage)
        {
            
            foreach (int i in employees)
            {
                if (db.she_observation_undian_exception.Where(p => p.id_employee == i).Count() == 0)
                {
                    she_observation_undian_exception she_obs = new she_observation_undian_exception
                    {
                        id_employee = i
                    };
                    db.she_observation_undian_exception.Add(she_obs);
                    db.SaveChanges();
                }
            }

            if (db.she_observation_undian.Where(p => p.from <= from && p.to >= from).Count() > 0)
            {
                return Json(null);
            }
            else
            {
                she_observation_undian undian = new she_observation_undian
                {
                    from = from,
                    to = to,
                    percentage = percentage,
                    drawing_date = DateTime.Today
                };
                db.she_observation_undian.Add(undian);
                db.SaveChanges();
                return Json(new { id = undian.id });
            }


            
        }

        public ActionResult draw(int id)
        {
            she_observation_undian undian = db.she_observation_undian.Find(id);
            DateTime froms = undian.from.Value;
            DateTime tos = undian.to.Value.AddDays(1);
            //var data = (from she_obs in db.she_observation
            //            join emp in db.employees
            //            on she_obs.observer. equals SqlFunctions.StringConvert((double)emp.id)
            //            where she_obs.date_time >= froms.Date && she_obs.date_time <= tos.Date
            //            select new she_observation
            //            {
            //                observer = she_obs.observer,
            //                is_quality = she_obs.is_quality,
            //                id = she_obs.id,
            //                is_contractor = emp.employee_dept == 25 ? 1 : 0,
            //            }).ToList();
            List<she_observation> list_she_obs = db.she_observation.Where(p => p.date_time >= froms.Date && p.date_time <= tos.Date).ToList();

            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       orderby employees.dept_id
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           dept_id = employees.employee_dept,
                       }).ToList();
            List<EmployeeEntity> bind = has;
            //List<she_observation> list_she_obs = data;
            List<she_observation_undian_exception> list_ex = db.she_observation_undian_exception.ToList();
            for (int i = 0; i < list_she_obs.Count; i++)
            {
                she_observation she_obs = list_she_obs.ElementAt(i);
                string[] a = she_obs.observer.Split('#');
                if (a.Length > 1)
                {
                    int emp_id = Int32.Parse(a.ElementAt(1));
                    if (list_ex.Exists(p => p.id_employee == Int32.Parse(a.ElementAt(1))))
                    {
                        list_she_obs.Remove(she_obs);
                        i--;
                    }
                    else
                    {
                        EmployeeEntity emp = bind.Where(p => p.id == emp_id).FirstOrDefault();
                        if (emp != null && emp.dept_id == 25)
                        {
                            she_obs.is_contractor = 1;
                        }
                        else
                        {
                            she_obs.is_contractor = 0;
                        }
                    }
                    
                }
                else
                {
                    string emp_name = a.ElementAt(0);
                    EmployeeEntity emp = bind.Where(p => p.alpha_name == emp_name).FirstOrDefault();
                    int id_emp = emp != null ? emp.id : 0;
                    if (list_ex.Exists(p => p.id_employee == id_emp))
                    {
                        list_she_obs.Remove(she_obs);
                        i--;
                    }
                }
            }
            ViewBag.list_she_obs = list_she_obs;
            ViewBag.date_from = froms;
            ViewBag.date_to = tos;
            ViewBag.id = id;
            ViewBag.percentage = undian.percentage;
            return PartialView();
        }

        [HttpPost]
        public JsonResult saveResult(int she_obs, int id_undian, int reward, byte category)
        {
            CultureInfo ci = new CultureInfo("id-ID");
            she_observation_undian_winner undian = new she_observation_undian_winner
            {
                id_reward = reward,
                category = category,
                winner_observation = she_obs,
                id_undian = id_undian
            };
            db.she_observation_undian_winner.Add(undian);
            db.SaveChanges();

            var a = (from undians in db.she_observation_undian_winner
                     join obs in db.she_observation 
                     on undians.winner_observation equals obs.id
                     join rewards in db.she_observation_undian_reward
                     on undians.id_reward equals rewards.id
                     where undians.id_undian == id_undian
                     select new WinnerEntity
                     {
                         reward = rewards.reward.Value,
                         category = undians.category == 0 ? "Quality" : "All",
                         winner = obs.observer
                     }).ToList();
            foreach (WinnerEntity win in a)
            {
                win.reward_string = win.reward.ToString("c", ci);
            }
            return Json(new {list_winner = a});
        }

        [HttpPost]
        public JsonResult getReward(byte category, int id_undian)
        {
            List<she_observation_undian_reward> list_reward = db.she_observation_undian_reward.OrderBy(p => p.id).ToList();
            List<she_observation_undian_winner> list_winner = db.she_observation_undian_winner.Where(p => p.id_undian == id_undian).ToList();
            CultureInfo ci = new CultureInfo("id-ID");
            foreach (she_observation_undian_reward reward in list_reward)
            {
                if (list_winner.Where(p => p.id_reward == reward.id).FirstOrDefault() == null)
                {
                    return Json(new { id = reward.id, reward = reward.reward.Value.ToString("c",ci)});
                }
            }
            return Json(false);

        }

        #region winner
        public ActionResult winner()
        {
            List<she_observation_undian> undian = db.she_observation_undian.OrderByDescending(p => p.from).ToList();
            ViewBag.undian = undian;
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       orderby employees.alpha_name
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name
                       }).ToList();
            ViewBag.user = has;
            return PartialView();
        }

        [HttpPost]
        public JsonResult signer(int signer, int id_undian)
        {
            she_observation_undian undian = db.she_observation_undian.Find(id_undian);
            undian.signature = signer;
            db.Entry(undian).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult changePeriod(int id)
        {
            she_observation_undian undian = db.she_observation_undian.Find(id);
            return Json(undian.signature);
        }

        //
        // Ajax select binding she observation report
        [GridAction]
        public ActionResult _SelectAjaxUndian(int? id)
        {
            return bindingUndian(id);
        }

        //select data incident she observation report
        private ViewResult bindingUndian(int? id)
        {
            List<she_observation_undian_winner> f = null;
            List<WinnerEntity> g = new List<WinnerEntity>();
            CultureInfo ci = new CultureInfo("id-ID");
            if (id == null)
            {
                she_observation_undian undian = db.she_observation_undian.OrderBy(p => p.from).ToList().LastOrDefault();
                if (undian != null)
                {
                    f = db.she_observation_undian_winner.Where(p => p.id_undian == undian.id).ToList();
                }
                else
                {
                    f = db.she_observation_undian_winner.ToList();
                }
            }
            else
            {
                f = db.she_observation_undian_winner.Where(p => p.id_undian == id).ToList();
            }

            foreach (she_observation_undian_winner she in f)
            {
                WinnerEntity w = new WinnerEntity
                {
                    id = she.id,
                    reward_string = (db.she_observation_undian_reward.Find(she.id_reward) == null ? "" : db.she_observation_undian_reward.Find(she.id_reward).reward.Value.ToString("c",ci)),
                    winner = db.she_observation.Find(she.winner_observation).observer.Split('#')[0].ToString(),
                    category = she.category == 0 ? "Quality SHE Observation Report" : "SHE Observation Report"
                };
                g.Add(w);
            }
            return View(new GridModel<WinnerEntity>
            {
                Data = g
            });
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _BindGrid(GridCommand command, string mode, int? id)
        {
            return bindingUndian(id);
        }

        public ActionResult Print(int id)
        {
            WinnerReport report = new WinnerReport();
            CultureInfo ci = new CultureInfo("id-ID");
            she_observation_undian undian = db.she_observation_undian.Find(id);
            report.from = undian.from;
            report.to = undian.to;

            List<she_observation_undian_winner> f = db.she_observation_undian_winner.Where(p => p.id_undian == id).ToList();
            List<WinnerEntity> g = new List<WinnerEntity>();
            int count_q = 0;
            int count = 0;
            foreach (she_observation_undian_winner she in f)
            {
                WinnerEntity w = new WinnerEntity
                {
                    id = she.id,
                    reward_string = db.she_observation_undian_reward.Find(she.id_reward).reward.Value.ToString("c", ci),
                    winner = db.she_observation.Find(she.winner_observation).observer.Split('#')[0].ToString(),
                    category = she.category == 0 ? "Quality SHE Observation Report" : "SHE Observation Report"
                };
                g.Add(w);
                if (she.category == 0)
                    count_q++;
                else
                    count++;
            }

            report.winners = g;
            report.count = count;
            report.count_q = count_q;

            return this.ViewPdf("", "Print", report);
        }

        public ActionResult Certificate(int id)
        {
            ViewBag.id = id;
            return PartialView();
        }

        #endregion
    }
}
