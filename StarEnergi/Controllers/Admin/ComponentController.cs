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
    public class ComponentController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();

        //
        // GET: /System/Details/5

        public ActionResult Details(int id)
        {
            component c = db.components.Find(id);
            return PartialView(c);
        }

        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            //ViewBag.id_equipment_part = id;            
            var listEPart = db.equipment_part.Where(n => n.id_equipment == id).ToList();
            if (listEPart.Count > 0)
                ViewBag.id_equipment_part = listEPart.FirstOrDefault().id;
            return PartialView();
        }

        //
        // POST: /System/Create

        [HttpPost]
        public ActionResult Create(component component)
        {
            if (ModelState.IsValid)
            {
                if (db.components.Where(x => x.tag_number == component.tag_number).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }
                equipment_part ePart = db.equipment_part.Find(component.id_equipment_part);
                component.equipment_part = ePart;
                db.components.Add(component);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();

                    return Json(e.Succes(component.id.ToString()));
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
            component component = db.components.Find(id);
            ViewBag.id_equipment_part = component.id_equipment_part;
            return PartialView(component);
        }

        //
        // POST: /System/Edit/5

        [HttpPost]
        public ActionResult Edit(component component)
        {
            if (ModelState.IsValid)
            {
                db.Entry(component).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.COMPONENT + ";" + component.id;
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
            component component = db.components.Find(id);
            return PartialView(component);
        }

        //
        // POST: /System/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            component component = db.components.Find(id);
            db.components.Remove(component);
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