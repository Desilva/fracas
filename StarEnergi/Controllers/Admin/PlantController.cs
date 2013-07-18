using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class PlantController : Controller
    {
        
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /Plant/

        public ActionResult Index()
        {
            return PartialView(db.plants.ToList());
        }

        //
        // GET: /Plant/Details/5

        public ActionResult Details(int id)
        {
            plant plant = db.plants.Find(id);
            return PartialView(plant);
        }

        //
        // GET: /Plant/Create

        public ActionResult Create()
        {
            return PartialView();
        } 

        //
        // POST: /Plant/Create

        [HttpPost]
        public ActionResult Create(plant plant)
        {
            if (ModelState.IsValid)
            {
                db.plants.Add(plant);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return PartialView(plant);
        }
        
        //
        // GET: /Plant/Edit/5
 
        public ActionResult Edit(int id)
        {
            plant plant = db.plants.Find(id);
            return PartialView(plant);
        }

        //
        // POST: /Plant/Edit/5

        [HttpPost]
        public ActionResult Edit(plant plant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView(plant);
        }

        //
        // GET: /Plant/Delete/5
 
        public ActionResult Delete(int id)
        {
            plant plant = db.plants.Find(id);
            return PartialView(plant);
        }

        //
        // POST: /Plant/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            plant plant = db.plants.Find(id);
            db.plants.Remove(plant);
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