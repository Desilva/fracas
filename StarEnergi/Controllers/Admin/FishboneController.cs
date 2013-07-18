using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class FishboneController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        
        //
        // GET: /Fishbone/

        public ActionResult Index()
        {
            return PartialView(db.rca_fishbone_master.ToList());
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            rca_fishbone_master fishboneMaster = new rca_fishbone_master();
            if (TryUpdateModel(fishboneMaster))
            {
                create(fishboneMaster);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            rca_fishbone_master editable = db.rca_fishbone_master.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return binding();
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            delete(id);
            return binding();
        }

        //select data failure mode
        private ViewResult binding()
        {
            return View(new GridModel<rca_fishbone_master>
            {
                Data = db.rca_fishbone_master.ToList()
            });
        }

        //insert data failure mode
        public void create(rca_fishbone_master fishboneMaster)
        {
            db.rca_fishbone_master.Add(fishboneMaster);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(rca_fishbone_master fishboneMaster)
        {
            db.Entry(fishboneMaster).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            rca_fishbone_master fishboneMaster = db.rca_fishbone_master.Find(id);
            db.rca_fishbone_master.Remove(fishboneMaster);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}
