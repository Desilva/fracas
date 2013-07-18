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
    public class SubComponentController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();

        //
        // GET: /System/Details/5

        public ActionResult Details(int id)
        {
            sub_component sc = db.sub_component.Find(id);
            return PartialView(sc);
        }

        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            ViewBag.id_component = id;
            return PartialView();
        }

        //
        // POST: /System/Create

        [HttpPost]
        public ActionResult Create(sub_component sub_component)
        {
            if (ModelState.IsValid)
            {
                if (db.sub_component.Where(x => x.tag_number == sub_component.tag_number).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.sub_component.Add(sub_component);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    return Json(e.Succes(sub_component.id.ToString()));
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
            sub_component sub_component = db.sub_component.Find(id);
            ViewBag.id_component = sub_component.id_component;
            return PartialView(sub_component);
        }

        //
        // POST: /System/Edit/5

        [HttpPost]
        public ActionResult Edit(sub_component sub_component)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sub_component).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    string result = Config.TreeType.SUBCOMPONENT + ";" + sub_component.id;
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
            sub_component sub_component = db.sub_component.Find(id);
            return PartialView(sub_component);
        }

        //
        // POST: /System/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            sub_component sub_component = db.sub_component.Find(id);
            db.sub_component.Remove(sub_component);
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