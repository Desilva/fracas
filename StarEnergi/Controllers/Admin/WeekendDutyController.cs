using StarEnergi.Models;
using StarEnergi.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using StarEnergi.Extensions;

namespace StarEnergi.Controllers.Admin
{
    [AuthorizeUser("/WeekendDuty", AuthorizedRoles = new int[] { (int)Config.role.ADMIN, (int)Config.role.WEEKENDDUTY })]
    public class WeekendDutyController : Controller
    {
        //
        // GET: /BuildOfMaterial/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        public ActionResult Index()
        {
            return View();
        }

        private ActionResult bindingWeekendDuty(string department)
        {
            List<weekend_duty> weekendDuties = new List<weekend_duty>();
            if (department == null)
            {
                weekendDuties = db.weekend_duty.ToList();
            }
            else
            {
                weekendDuties = db.weekend_duty.Where(x => x.department == department).ToList();
            }
            weekendDuties = weekendDuties.Where(p => p.start_date.CompareTo(DateTime.Today) >= 0 || p.end_date.CompareTo(DateTime.Today) >= 0).OrderBy(p => p.start_date).ThenBy(p => p.end_date).ToList();
            foreach (weekend_duty weekendDuty in weekendDuties)
            {
                employee delegationEmployee = db.employees.Find(weekendDuty.delegate_id);
                weekendDuty.delegationName = delegationEmployee != null ? delegationEmployee.alpha_name : "";
            }
            return View(new GridModel<weekend_duty>
            {
                Data = weekendDuties
            });
        }

        [GridAction]
        public ActionResult _SelectAjaxWeekendDuty()
        {
            int? employeeId = Session["id"] as int?;
            List<user_per_role> userRoles = Session["roles"] as List<user_per_role>;
            if (employeeId != null)
            {
                employee employee = db.employees.Find(employeeId);
                if (userRoles.Exists(p => p.role == (int)Config.role.ADMIN)) {
                    return bindingWeekendDuty(null);
                } else {
                    return bindingWeekendDuty(employee.department);
                }
            }
            else
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Either you don't have the access to this module or you gave just logged out. Please try refreshing this page.";
                return View(new GridModel<weekend_duty>{});
            }
        }

        public ActionResult AddDuty()
        {
            int? employeeId = Session["id"] as int?;
            List<user_per_role> userRoles = Session["roles"] as List<user_per_role>;
            if (employeeId != null)
            {
                employee employee = db.employees.Find(employeeId);
                weekend_duty weekendDuty = new weekend_duty();
                weekendDuty.employee_id = employee.id;
                weekendDuty.department = employee.department == null ? "" : employee.department.ToUpper();


                List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null && p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                //if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN)) {
                    delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                //}
                ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                ViewBag.department = new SelectList(departments, weekendDuty.department);

                return View("Form", weekendDuty);
            }
            else
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Either you don't have the access to this module or you gave just logged out. Please try refreshing this page.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Save(weekend_duty weekendDuty)
        {
            NameValueCollection nvc = Request.Form;
            employee employee = db.employees.Find(weekendDuty.employee_id);
            List<user_per_role> userRoles = Session["roles"] as List<user_per_role>;

            if (nvc.Get("start_date") != "" && nvc.Get("end_date") != "")
            {
                if (weekendDuty.end_date.CompareTo(DateTime.Today) >= 0)
                {
                    if (weekendDuty.end_date.CompareTo(weekendDuty.start_date) >= 0)
                    {
                        if (weekendDuty.id == 0)
                        {
                            db.weekend_duty.Add(weekendDuty);
                        }
                        else
                        {
                            weekend_duty weekendDutyDb = db.weekend_duty.Find(weekendDuty.id);
                            weekendDutyDb.start_date = weekendDuty.start_date;
                            weekendDutyDb.end_date = weekendDuty.end_date;
                            weekendDutyDb.delegate_id = weekendDuty.delegate_id;
                            db.Entry(weekendDutyDb).State = EntityState.Modified;
                        }

                        IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                        if (error.Count() == 0)
                        {
                            db.SaveChanges();
                            employee employeeOnDuty = db.employees.Find(weekendDuty.delegate_id);
                            this.SetMessage(employeeOnDuty.alpha_name, "{0} has been succesfully saved.");
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            //return Json(error.First().ValidationErrors.ToArray());
                            foreach (DbEntityValidationResult errorDb in error)
                            {
                                foreach (DbValidationError errorDbEntity in errorDb.ValidationErrors) {
                                    ModelState.AddModelError(errorDbEntity.PropertyName, errorDbEntity.ErrorMessage);
                                }
                            }

                            List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null || p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                            if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN))
                            {
                                delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                            }
                            ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                            List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                            ViewBag.department = new SelectList(departments, weekendDuty.department);
                            return View("Form", weekendDuty);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("end_date", "Date end can not before date start.");
                        List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null || p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                        if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN))
                        {
                            delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                        }
                        ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                        List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                        ViewBag.department = new SelectList(departments, weekendDuty.department);
                        return View("Form", weekendDuty);
                    }
                }
                else
                {
                    ModelState.AddModelError("end_date", "Date end cannot before today.");
                    List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null || p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                    if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN))
                    {
                        delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                    }
                    ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                    List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                    ViewBag.department = new SelectList(departments, weekendDuty.department);
                    return View("Form", weekendDuty);
                }

            }
            else
            {
                ModelState.AddModelError("end_date", "Delegation Period cannot be empty.");
                List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null || p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN))
                {
                    delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                }
                ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                ViewBag.department = new SelectList(departments, weekendDuty.department);
                return View("Form", weekendDuty);
            }
        }

        public ActionResult EditDuty(int id)
        {
            int? employeeId = Session["id"] as int?;
            List<user_per_role> userRoles = Session["roles"] as List<user_per_role>;
            if (employeeId != null)
            {
                employee employee = db.employees.Find(employeeId);
                weekend_duty weekendDuty = db.weekend_duty.Find(id);

                List<employee> delegationTargets = db.employees.Where(p => p.employee_dept != null || p.employee_boss != null).OrderBy(p => p.alpha_name).ToList();
                //if (userRoles == null || !userRoles.Exists(p => p.role == (int)Config.role.ADMIN))
                //{
                    delegationTargets = db.employees.Where(p => p.department != null && p.department.ToUpper() == weekendDuty.department).ToList();
                //}
                ViewBag.delegate_id = new SelectList(delegationTargets, "id", "alpha_name", weekendDuty.delegate_id);


                List<string> departments = db.employees.Where(p => p.department != null).Select(p => p.department.ToUpper()).Distinct().ToList();
                ViewBag.department = new SelectList(departments, weekendDuty.department);

                return View("Form", weekendDuty);
            }
            else
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Either you don't have the access to this module or you gave just logged out. Please try refreshing this page.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public bool DeleteDuty(int id)
        {
            int? employeeId = Session["id"] as int?;
            List<user_per_role> userRoles = Session["roles"] as List<user_per_role>;
            if (employeeId != null)
            {
                weekend_duty duty = db.weekend_duty.Find(id);
                db.weekend_duty.Remove(duty);
                db.SaveChanges();
                return true;
            }
            else
            {
                Response.StatusCode = 500;
                Response.StatusDescription = "Either you don't have the access to this module or you gave just logged out. Please try refreshing this page.";
                return false;
            }
        }

        [HttpPost]
        public JsonResult ChangeDepartment(string department)
        {
            List<EmployeeEntity> employeesByDepartment = db.employees.Where(p => p.department.ToUpper() == department).Select(p => new EmployeeEntity {
                id = p.id,
                alpha_name = p.alpha_name
            }).ToList();
            return Json(employeesByDepartment);
        }
    }
}