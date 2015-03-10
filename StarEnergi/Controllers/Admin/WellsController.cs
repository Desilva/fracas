using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using StarEnergi.Utilities;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class WellsController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /Wells/

        public ActionResult Index()
        {
            return PartialView(db.daily_log_wells.ToList());
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
            daily_log_wells wells = new daily_log_wells();
            if (TryUpdateModel(wells))
            {
                create(wells);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            daily_log_wells editable = db.daily_log_wells.Find(id);
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
            return View(new GridModel<daily_log_wells>
            {
                Data = db.daily_log_wells.Where(p => p.is_delete == false).OrderBy(p => p.name).ToList()
            });
        }

        //insert data failure causes
        public void create(daily_log_wells wells)
        {
            wells.is_delete = false;
            db.daily_log_wells.Add(wells);
            db.SaveChanges();
        }

        //update data failure cause
        private void update(daily_log_wells wells)
        {
            wells.is_delete = false;
            db.Entry(wells).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            daily_log_wells wells = db.daily_log_wells.Find(id);
            wells.is_delete = true;
            db.Entry(wells).State = EntityState.Modified;
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //public ActionResult importExcel()
        //{
        //    var model = db.failure_causes.Select(f => new { f.title, f.description });
        //    string[] title = { "Title", "Description" };

        //    return MyTools.importExcel(model.ToList(), "Failure Causes", title);
        //}

    }
}
