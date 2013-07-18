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
    [Authorize]
    public class FocController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Foc/

        public ActionResult Index()
        {
            var focs = db.focs.Include(f => f.plant);
            return PartialView(focs.ToList());
        }

        //
        // GET: /Foc/Details/5

        public ActionResult Details(int id)
        {
            foc foc = db.focs.Find(id);
            return PartialView(foc);
        }

        //
        // GET: /Foc/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.plant_id = new SelectList(db.plants, "id", "nama");
            ViewBag.plant_id = id;
            return PartialView();
        } 

        //
        // POST: /Foc/Create

        [HttpPost]
        public ActionResult Create(foc foc)
        {
            if (ModelState.IsValid)
            {

                if(db.focs.Where(x => x.nama == foc.nama).ToList().Count > 0){
                    return Json(e.Fail());  
                }

                db.focs.Add(foc);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    foc_paf focPaf = new foc_paf()
                    {
                        id_foc = foc.id
                    };
                    db.foc_paf.Add(focPaf);
                    db.SaveChanges();
                    return Json(e.Succes(foc.id.ToString()));
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
        // GET: /Foc/Edit/5
 
        public ActionResult Edit(int id)
        {
            foc foc = db.focs.Find(id);
            ViewBag.plant_id = foc.plant_id;
            return PartialView(foc);
        }

        //
        // POST: /Foc/Edit/5

        [HttpPost]
        public ActionResult Edit(foc foc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foc).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.AREA + ";" + foc.id;
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
        // GET: /Foc/Delete/5
 
        public ActionResult Delete(int id)
        {
            foc foc = db.focs.Find(id);
            return PartialView(foc);
        }

        //
        // POST: /Foc/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            foc foc = db.focs.Find(id);
            db.focs.Remove(foc);
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