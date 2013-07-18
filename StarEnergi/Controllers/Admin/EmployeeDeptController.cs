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

namespace StarEnergi.Controllers.Admin
{
    public class EmployeeDeptController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /EmployeeDept/

        public ActionResult Index()
        {
            var employeeDept = db.employee_dept.Include(f => f.plant);
            return PartialView(employeeDept.ToList());
        }

        //
        // GET: /EmployeeDept/Details/5

        public ActionResult Details(int id)
        {
            employee_dept employeeDept = db.employee_dept.Find(id);
            return PartialView(employeeDept);
        }

        //
        // GET: /EmployeeDept/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.plant_id = new SelectList(db.plants, "id", "nama");
            ViewBag.plant_id = id;
            return PartialView();
        }

        //
        // POST: /EmployeeDept/Create

        [HttpPost]
        public ActionResult Create(employee_dept employeeDept)
        {
            if (ModelState.IsValid)
            {

                if (db.employee_dept.Where(x => x.dept_name == employeeDept.dept_name).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.employee_dept.Add(employeeDept);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    return Json(e.Succes(employeeDept.id.ToString()));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /EmployeeDept/Edit/5

        public ActionResult Edit(int id)
        {
            employee_dept employeeDept = db.employee_dept.Find(id);
            ViewBag.plant_id = employeeDept.plant_id;
            return PartialView(employeeDept);
        }

        //
        // POST: /EmployeeDept/Edit/5

        [HttpPost]
        public ActionResult Edit(employee_dept employeeDept)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeDept).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = "DEPARTMENT" + ";" + employeeDept.id;
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
            employee_dept employeeDept = db.employee_dept.Find(id);
            return PartialView(employeeDept);
        }

        //
        // POST: /EmployeeDept/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<employee> le = getAllDeletedData(id);
            foreach (employee e in le)
            {
                db.employees.Remove(e);
                db.SaveChanges();
            }
            employee_dept employeeDept = db.employee_dept.Find(id);
            db.employee_dept.Remove(employeeDept);
            db.SaveChanges();
            return Json(true);
            //return RedirectToAction("Index");
        }

        private List<employee> getAllDeletedData(int id)
        {
            employee_dept employeeDept = db.employee_dept.Find(id);
            List<employee> list_e = employeeDept.employees.ToList();

            for (int i = 0; i < list_e.Count; i++)
            {
                List<employee> le = list_e[i].employee1.ToList();
                foreach (employee eee in le)
                {
                    list_e.Add(eee);
                }
            }

            list_e.Reverse();

            return list_e;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
