using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Data;
using Telerik.Web.Mvc;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class FailureCauseController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /FailureCause/

        public ActionResult Index()
        {
            return PartialView(db.failure_causes.ToList());
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
            failure_causes failure_cause = new failure_causes();
            if (TryUpdateModel(failure_cause))
            {
                create(failure_cause);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            failure_causes editable = db.failure_causes.Find(id);
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
            return View(new GridModel<failure_causes>
            {
                Data = db.failure_causes.ToList()
            });
        }

        //insert data failure causes
        public void create(failure_causes failure_cause)
        {
            db.failure_causes.Add(failure_cause);
            db.SaveChanges();
        }

        //update data failure cause
        private void update(failure_causes failure_cause)
        {
            db.Entry(failure_cause).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            failure_causes failure_cause = db.failure_causes.Find(id);
            db.failure_causes.Remove(failure_cause);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel()
        {
            var model = db.failure_causes.Select(f => new { f.title, f.description });
            string[] title = { "Title", "Description" };

            return MyTools.importExcel(model.ToList(), "Failure Causes", title);
        }

    }
}
