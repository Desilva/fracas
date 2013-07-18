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
    public class EquipmentGroupController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /EquipmentGroup/

        public ActionResult Index()
        {
            var equipment_groups = db.equipment_groups.Include(e => e.system);
            return PartialView(equipment_groups.ToList());
        }

        //
        // GET: /EquipmentGroup/Details/5

        public ActionResult Details(int id)
        {
            equipment_groups equipment_groups = db.equipment_groups.Find(id);
            return PartialView(equipment_groups);
        }

        //
        // GET: /EquipmentGroup/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.id_system = new SelectList(db.systems, "id", "nama");
            ViewBag.id_system = id;
            return PartialView();
        } 

        //
        // POST: /EquipmentGroup/Create

        [HttpPost]
        public ActionResult Create(equipment_groups equipment_groups)
        {
            if (ModelState.IsValid)
            {
                if (db.equipment_groups.Where(x => x.nama == equipment_groups.nama).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.equipment_groups.Add(equipment_groups);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    //return RedirectToAction("Index");  
                    return Json(e.Succes(equipment_groups.id.ToString()));
                }
                else
                {
                    //return Json(error.First().ValidationErrors.ToArray());
                    return Json(e.Fail(error));
                }
            }

            //ViewBag.id_system = new SelectList(db.systems, "id", "nama", equipment_groups.id_system);
            //return PartialView(equipment_groups);
            return Json(e.Fail(ModelState));
        }
        
        //
        // GET: /EquipmentGroup/Edit/5
 
        public ActionResult Edit(int id)
        {
            equipment_groups equipment_groups = db.equipment_groups.Find(id);
            //ViewBag.id_system = new SelectList(db.systems, "id", "nama", equipment_groups.id_system);
            ViewBag.id_system = equipment_groups.id_system;
            return PartialView(equipment_groups);
        }

        //
        // POST: /EquipmentGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(equipment_groups equipment_groups)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipment_groups).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    string result = Config.TreeType.EQUIPMENT_GROUP + ";" + equipment_groups.id;
                    return Json(e.Succes(result));
                }
                else
                {
                    //return Json(error.First().ValidationErrors.ToArray());
                    return Json(e.Fail(error));
                }
            }
            //ViewBag.id_system = new SelectList(db.systems, "id", "nama", equipment_groups.id_system);
            //return PartialView(equipment_groups);
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /EquipmentGroup/Delete/5
 
        public ActionResult Delete(int id)
        {
            equipment_groups equipment_groups = db.equipment_groups.Find(id);
            return PartialView(equipment_groups);
        }

        //
        // POST: /EquipmentGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            equipment_groups equipment_groups = db.equipment_groups.Find(id);
            db.equipment_groups.Remove(equipment_groups);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}