using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using StarEnergi.Utilities;
using System.Data.Entity.Validation;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class SystemController : Controller
    {
        
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();

        //
        // GET: /System/

        public ActionResult Index()
        {
            var systems = db.systems.Include(s => s.unit);
            return PartialView(systems.ToList());
        }

        //
        // GET: /System/Details/5

        public ActionResult Details(int id)
        {
            system system = db.systems.Find(id);
            return PartialView(system);
        }

        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            ViewBag.id_unit = id;
            return PartialView();
        } 

        //
        // POST: /System/Create

        [HttpPost]
        public ActionResult Create(system system)
        {
            if (ModelState.IsValid)
            {
                if (db.systems.Where(x => x.nama == system.nama).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.systems.Add(system);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    system_paf systemPaf = new system_paf()
                    {
                        id_system = system.id
                    };
                    db.system_paf.Add(systemPaf);
                    db.SaveChanges();
                    return Json(e.Succes(system.id.ToString()));
                }
                else
                {
                    return Json(e.Fail(error));
                }  
            }
            return Json(e.Fail(ModelState));
        }
        
        //
        // GET: /System/Edit/5
 
        public ActionResult Edit(int id)
        {
            system system = db.systems.Find(id);
            ViewBag.id_unit = system.id_unit;
            return PartialView(system);
        }

        //
        // POST: /System/Edit/5

        [HttpPost]
        public ActionResult Edit(system system)
        {
            if (ModelState.IsValid)
            {
                db.Entry(system).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.SYSTEM + ";" + system.id;
                    return Json(e.Succes(result));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }

            return Json(e.Fail(ModelState));
        }

        //
        // GET: /System/Delete/5
 
        public ActionResult Delete(int id)
        {
            system system = db.systems.Find(id);
            return PartialView(system);
        }

        //
        // POST: /System/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            system system = db.systems.Find(id);
            db.systems.Remove(system);
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