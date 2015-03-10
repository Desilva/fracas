using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using StarEnergi.Utilities;
using System.Diagnostics;
using System.IO;

namespace StarEnergi.Controllers.Admin
{
    public class EmployeeBossController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /EmployeeBoss/

        public ActionResult Index()
        {
            var employees = db.employees.Include(f => f.employee_dept1);
            return PartialView(employees.ToList());
        }

        //
        // GET: /EmployeeBoss/Details/5

        public ActionResult Details(int id)
        {
            employee employeeBoss = db.employees.Find(id);
            Dictionary<int, string> delegate_name = new Dictionary<int, string>();
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       orderby employees.alpha_name
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                       }).ToList();
            foreach (EmployeeEntity e in has)
            {
                delegate_name.Add(e.id, e.alpha_name);
            }
            ViewBag.delegate_name = new SelectList(delegate_name, "Key", "Value", employeeBoss.employee_delegate);
            return PartialView(employeeBoss);
        }

        //
        // GET: /EmployeeDept/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.plant_id = new SelectList(db.plants, "id", "nama");
            ViewBag.dept_id = id;
            return PartialView();
        }

        //
        // POST: /EmployeeDept/Create

        [HttpPost]
        public ActionResult Create(employee employeeBoss)
        {
            Debug.WriteLine(employeeBoss.employee_no);
            if (ModelState.IsValid)
            {

                if (db.employees.Where(x => x.employee_no == employeeBoss.employee_no).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.employees.Add(employeeBoss);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    if (employeeBoss.signature != null)
                    {
                        employee es = db.employees.OrderBy(p => p.id).ToList().LastOrDefault();
                        string oldFilePath = Server.MapPath(employeeBoss.signature);
                        es.signature = "/Attachment/signatures/" + (es.id + ".") + es.signature.Substring(es.signature.LastIndexOf(".") + 1);
                        string newFilePath = Server.MapPath(es.signature);
                        System.IO.File.Move(oldFilePath, newFilePath);
                        db.Entry(es).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Json(e.Succes(employeeBoss.id.ToString()));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /EmployeeBoss/Edit/5

        public ActionResult Edit(int id)
        {
            employee employeeBoss = db.employees.Find(id);
            ViewBag.dept_id = employeeBoss.employee_dept;
            Dictionary<int, string> delegate_name = new Dictionary<int, string>();
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       orderby employees.alpha_name
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                       }).ToList();
            foreach (EmployeeEntity e in has)
            {
                delegate_name.Add(e.id, e.alpha_name);
            }
            ViewBag.delegate_name = new SelectList(delegate_name, "Key", "Value", employeeBoss.employee_delegate);
            return PartialView(employeeBoss);
        }

        //
        // POST: /EmployeeDept/Edit/5

        [HttpPost]
        public ActionResult Edit(employee employeeBoss)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeBoss).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = "EMPLOYEEBOSS" + ";" + employeeBoss.id;
                    return Json(e.Succes(result));
                }
                else
                {
                    //return Json(error.First().ValidationErrors.ToArray());
                    return Json(e.Fail(error));
                }
            }
            //ViewBag.plant_id = new SelectList(db.plants, "id", "nama", foc.plant_id);
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /EmployeeDept/Delete/5

        public ActionResult Delete(int id)
        {
            employee employeeBoss = db.employees.Find(id);
            return PartialView(employeeBoss);
        }

        //
        // POST: /EmployeeDept/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            //employee employeeBoss = db.employees.Find(id);
            List<employee> le = getAllDeletedData(id);
            foreach (employee e in le)
            {
                e.employee_boss = null;
                e.employee_dept = null;
                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true);
            //return RedirectToAction("Index");
        }

        private List<employee> getAllDeletedData(int id)
        {
            employee e = db.employees.Find(id);

            List<employee> list_e = new List<employee>();

            list_e.Add(e);

            List<employee> le = e.employee1.ToList();

            foreach (employee ee in le)
            {
                ee.employee_boss = e.employee_boss;
                ee.employee_dept = e.employee_dept;

                db.Entry(ee).State = EntityState.Modified;
                db.SaveChanges();
            }

            return list_e;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult saveSignature(HttpPostedFileBase signature, int? id)
        {
            var currpath = "";
            if (signature != null && signature.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/signatures/"),
                    (id == null ? "0x0." : (id + ".")) + signature.FileName.Substring(signature.FileName.LastIndexOf(".") + 1)
                );
                signature.SaveAs(currpath);
                currpath = "/Attachment/signatures/" +
                    (id == null ? "0x0." : (id + ".")) + signature.FileName.Substring(signature.FileName.LastIndexOf(".") + 1);
            }
            return Json(new { success = true, path = currpath });
        }

    }
}
