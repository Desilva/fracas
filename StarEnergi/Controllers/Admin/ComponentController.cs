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

            ViewBag.areaFuncCode = c.equipment_part.equipment.equipment_groups.system.unit.foc.nama;
            ViewBag.equipmentFuncCode = c.equipment_part.equipment.functional_code;
            ViewBag.systemFuncCode = c.equipment_part.equipment.equipment_groups.system.kode;
            ViewBag.unitFuncCode = c.equipment_part.equipment.equipment_groups.system.unit.functional_code;
            return PartialView(c);
        }

        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            //ViewBag.id_equipment_part = id;         
            equipment equipment = db.equipments.Find(id);
            var listEPart = db.equipment_part.Where(n => n.id_equipment == id).ToList();
            if (listEPart.Count > 0)
                ViewBag.id_equipment_part = listEPart.FirstOrDefault().id;
            else
            {
                ///dummy data part and equipmeent part
                ///
                part part = new part();
                equipment_part ePart = new equipment_part();

                part.tag_number = equipment.tag_num;
                db.parts.Add(part);
                db.SaveChanges();

                ePart.id_equipment = equipment.id;
                ePart.id_parts = part.id;

                var hasEPart = db.equipment_part.Any(n => n.id_equipment == equipment.id && n.id_parts == part.id);
                if (!hasEPart)
                {
                    db.equipment_part.Add(ePart);
                    db.SaveChanges();
                }

                ViewBag.id_equipment_part = ePart.id;
            }

            ViewBag.areaFuncCode = equipment.equipment_groups.system.unit.foc.nama;
            ViewBag.equipmentFuncCode = equipment.functional_code;
            ViewBag.systemFuncCode = equipment.equipment_groups.system.kode;
            ViewBag.unitFuncCode = equipment.equipment_groups.system.unit.functional_code;

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

            ViewBag.areaFuncCode = component.equipment_part.equipment.equipment_groups.system.unit.foc.nama;
            ViewBag.equipmentFuncCode = component.equipment_part.equipment.functional_code;
            ViewBag.systemFuncCode = component.equipment_part.equipment.equipment_groups.system.kode;
            ViewBag.unitFuncCode = component.equipment_part.equipment.equipment_groups.system.unit.functional_code;
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