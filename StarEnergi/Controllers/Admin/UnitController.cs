using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Data.Entity.Validation;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class UnitController : Controller
    {
        
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Unit/

        public ActionResult Index()
        {
            var units = db.units.Include(u => u.foc);
            return PartialView(units.ToList());
        }

        //
        // GET: /Unit/Details/5

        public ActionResult Details(int id)
        {
            unit unit = db.units.Find(id);
            return PartialView(unit);
        }

        //
        // GET: /Unit/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.id_foc = new SelectList(db.focs, "id", "nama");
            ViewBag.id_foc = id;
            return PartialView();
        } 

        //
        // POST: /Unit/Create

        [HttpPost]
        public ActionResult Create(unit unit)
        {
            if (ModelState.IsValid)
            {
                if(db.units.Where(x => x.nama == unit.nama).ToList().Count > 0){
                    return Json(e.Fail());
                }

                db.units.Add(unit);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    unit_paf unitPaf = new unit_paf()
                    {
                        id_unit = unit.id
                    };
                    db.unit_paf.Add(unitPaf);
                    db.SaveChanges();
                    return Json(e.Succes(unit.id.ToString()));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }
            return Json(e.Fail(ModelState));
        }
        
        //
        // GET: /Unit/Edit/5
 
        public ActionResult Edit(int id)
        {
            unit unit = db.units.Find(id);
            ViewBag.id_foc = unit.id_foc;
            return PartialView(unit);
        }

        //
        // POST: /Unit/Edit/5

        [HttpPost]
        public ActionResult Edit(unit unit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unit).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.UNIT + ";" + unit.id;
                    return Json(e.Succes(result));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }
            //ViewBag.id_foc = new SelectList(db.focs, "id", "nama", unit.id_foc);
            //return PartialView(unit);
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /Unit/Delete/5
 
        public ActionResult Delete(int id)
        {
            unit unit = db.units.Find(id);
            return PartialView(unit);
        }

        //
        // POST: /Unit/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            unit unit = db.units.Find(id);
            db.units.Remove(unit);
            db.SaveChanges();
            return Json(true);
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}