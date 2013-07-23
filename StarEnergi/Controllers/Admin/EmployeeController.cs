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
    public class EmployeeController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Employee/

        public ActionResult Index()
        {
            var employees = db.employees.Include(f => f.employee1);
            return PartialView(employees.ToList());
        }

        //
        // GET: /Employee/Details/5

        public ActionResult Details(int id)
        {
            employee employee = db.employees.Find(id);
            return PartialView(employee);
        }

        //
        // GET: /EmployeeDept/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.plant_id = new SelectList(db.plants, "id", "nama");
            ViewBag.employee_id = id;
            employee employee = db.employees.Find(id);
            ViewBag.dept_id = employee.dept_id;
            return PartialView();
        }

        //
        // POST: /EmployeeDept/Create

        [HttpPost]
        public ActionResult Create(employee employee)
        {
            Debug.WriteLine(employee.employee_no);
            if (ModelState.IsValid)
            {

                if (db.employees.Where(x => x.employee_no == employee.employee_no).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.employees.Add(employee);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    if (employee.signature != null)
                    {
                        employee es = db.employees.OrderBy(p => p.id).ToList().LastOrDefault();
                        string oldFilePath = Server.MapPath(employee.signature);
                        es.signature = "/Attachment/signatures/" + (es.id + ".") + es.signature.Substring(es.signature.LastIndexOf(".") + 1);
                        string newFilePath = Server.MapPath(es.signature);
                        System.IO.File.Move(oldFilePath, newFilePath);
                        db.Entry(es).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Json(e.Succes(employee.id.ToString()));
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
            employee employee = db.employees.Find(id);
            ViewBag.employee_id = employee.employee_boss;
            ViewBag.dept_id = employee.dept_id;
            return PartialView(employee);
        }

        //
        // POST: /EmployeeDept/Edit/5

        [HttpPost]
        public ActionResult Edit(employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = "EMPLOYEE" + ";" + employee.id;
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
            employee employee = db.employees.Find(id);
            return PartialView(employee);
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
            return Json(true);
            //return RedirectToAction("Index");
        }

        private List<employee> getAllDeletedData(int id)
        {
            employee e = db.employees.Find(id);

            List<employee> list_e = new List<employee>();

            list_e.Add(e);

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

        #region drag & drop

        public JsonResult DragDrop(int type, int value_id, int dest_id)
        {
            employee e = null;
            employee dest = null;
            switch (type)
            {
                case 1:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = dest.employee_dept;
                    e.employee_boss = null;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 2:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = dest.employee_dept;
                    e.employee_boss = dest.employee_boss;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 3:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = dest.employee_dept;
                    e.employee_boss = null;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 4:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = dest.employee_dept;
                    e.employee_boss = dest.employee_boss;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 5:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = null;
                    e.employee_boss = dest.id;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 6:
                    e = db.employees.Find(value_id);
                    employee_dept dept = db.employee_dept.Find(dest_id);

                    e.employee_dept = dept.id;
                    e.employee_boss = null;
                    e.dept_id = dept.id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 7:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = null;
                    e.employee_boss = dest.id;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 8:
                    e = db.employees.Find(value_id);
                    employee_dept dept2 = db.employee_dept.Find(dest_id);

                    e.employee_dept = dept2.id;
                    e.employee_boss = null;
                    e.dept_id = dept2.id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 9:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = null;
                    e.employee_boss = dest.id;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 10:
                    e = db.employees.Find(value_id);
                    dest = db.employees.Find(dest_id);

                    e.employee_dept = null;
                    e.employee_boss = dest.id;
                    e.dept_id = dest.dept_id;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
            }
            return Json(true);
        }

        #endregion

    }
}
