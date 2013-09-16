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
    public class UndianController : Controller
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

        [HttpPost]
        public JsonResult startDraw(int[] employees, DateTime from, DateTime to)
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
                return Json(true);
            }


            
        }

        public ActionResult draw(DateTime from, DateTime to)
        {
            to = to.AddDays(1);
            List<she_observation> list_she_obs = db.she_observation.Where(p => p.date_time >= from.Date && p.date_time <= to.Date && p.is_quality == 1).ToList();
            List<she_observation_undian_exception> list_ex = db.she_observation_undian_exception.ToList();
            for (int i = 0; i < list_she_obs.Count; i++)
            {
                she_observation she_obs = list_she_obs.ElementAt(i);
                string[] a = she_obs.observer.Split('#');
                if (a.Length > 1)
                {
                    if (list_ex.Exists(p => p.id_employee == Int32.Parse(a.ElementAt(1))))
                    {
                        list_she_obs.Remove(she_obs);
                        i--;
                    }
                }
                else
                {
                    string emp_name = a.ElementAt(0);
                    employee emp = db.employees.Where(p => p.alpha_name == emp_name).FirstOrDefault();
                    int id_emp = emp != null ? emp.id : 0;
                    if (list_ex.Exists(p => p.id_employee == id_emp))
                    {
                        list_she_obs.Remove(she_obs);
                        i--;
                    }
                }
            }
            ViewBag.list_she_obs = list_she_obs;
            ViewBag.date_from = from;
            ViewBag.date_to = to;
            return PartialView();
        }

        [HttpPost]
        public JsonResult saveResult(int she_obs, int winner_id, DateTime? from, DateTime? to, int? id_undian)
        {
            she_observation_undian undian = null;
            if (winner_id == 1)
            {
                undian = new she_observation_undian
                {
                    from = from.Value,
                    to = to.Value.AddDays(-1),
                    winner_id_she_observation_1 = she_obs
                };

                db.she_observation_undian.Add(undian);
                db.SaveChanges();
            }
            else if (winner_id == 2)
            {
                undian = db.she_observation_undian.Find(id_undian);
                undian.winner_id_she_observation_2 = she_obs;

                db.Entry(undian).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (winner_id == 3)
            {
                undian = db.she_observation_undian.Find(id_undian);
                undian.winner_id_she_observation_3 = she_obs;

                db.Entry(undian).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new {id = undian.id});
        }

        public ActionResult winner()
        {
            return PartialView();
        }

        //
        // Ajax select binding she observation report
        [GridAction]
        public ActionResult _SelectAjaxUndian()
        {
            return bindingUndian();
        }

        //select data incident she observation report
        private ViewResult bindingUndian()
        {
            List<she_observation_undian> f = db.she_observation_undian.ToList();
            foreach (she_observation_undian she in f)
            {
                she.from_period = she.from.Value.ToString("MMM yyyy");
                she.to_period = she.to.Value.ToString("MMM yyyy");
                she.winner_name_1 = db.she_observation.Find(she.winner_id_she_observation_1).observer.Split('#').FirstOrDefault();
                she.winner_name_2 = db.she_observation.Find(she.winner_id_she_observation_2).observer.Split('#').FirstOrDefault();
                she.winner_name_3 = db.she_observation.Find(she.winner_id_she_observation_3).observer.Split('#').FirstOrDefault();
            }
            return View(new GridModel<she_observation_undian>
            {
                Data = f.OrderByDescending(x => x.from)
            });
        }
    }
}
