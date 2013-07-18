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
    public class SubEquipmentController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();

        //
        // GET: /System/Details/5

        public ActionResult Details(int id)
        {
            equipment_part ep = db.equipment_part.Find(id);
            if (ep.status == 1)
            {
                ViewBag.status = "running";
            }
            else
            {
                ViewBag.status = "down";
            }
            return PartialView(ep);
        }

        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            ViewBag.id_equipment = id;
            ViewBag.id_part = new SelectList(db.parts, "id", "tag_number");
            return PartialView();
        }
        

        //
        // POST: /System/Create

        [HttpPost]
        public ActionResult Create(equipment_part eqp)
        {
            if (ModelState.IsValid)
            {
                if (db.equipment_part.Where(x => x.id_parts == eqp.id_parts).Where(x => x.id_equipment == eqp.id_equipment).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.equipment_part.Add(eqp);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    part_unit_event pEvent = new part_unit_event()
                    {
                        id_equipment_part = eqp.id,
                        datetime_ops = eqp.installed_date
                    };
                    db.part_unit_event.Add(pEvent);
                    return Json(e.Succes(eqp.id.ToString()));
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
            equipment_part ep = db.equipment_part.Find(id);
            ViewBag.id_equipment = ep.id_equipment;
            return PartialView(ep);
        }

        //
        // POST: /System/Edit/5

        [HttpPost]
        public ActionResult Edit(equipment_part ep)
        {
            equipment_part update = db.equipment_part.Find(ep.id);
            update.id_equipment = ep.id_equipment;
            update.installed_date = ep.installed_date;
           
            if (ModelState.IsValid)
            {
                db.Entry(update).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.PART + ";" + ep.id;
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
            equipment_part ep = db.equipment_part.Find(id);
            return PartialView(ep);
        }

        //
        // POST: /System/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            equipment_part ep = db.equipment_part.Find(id);
            db.equipment_part.Remove(ep);
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