using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class FailureEffectController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /FailureEffect/

        public ActionResult Index()
        {
            return PartialView(db.failure_effects.ToList());
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
            failure_effects failure_effect = new failure_effects();
            if (TryUpdateModel(failure_effect))
            {
                create(failure_effect);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            failure_effects editable = db.failure_effects.Find(id);
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
            return View(new GridModel<failure_effects>
            {
                Data = db.failure_effects.ToList()
            });
        }

        //insert data failure mode
        public void create(failure_effects failure_effect)
        {
            db.failure_effects.Add(failure_effect);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(failure_effects failure_effect)
        {
            db.Entry(failure_effect).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            failure_effects failure_effect = db.failure_effects.Find(id);
            db.failure_effects.Remove(failure_effect);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel()
        {
            var model = db.failure_effects.Select(f => new { f.title, f.description });
            string[] title = { "Title", "Description" };

            return MyTools.importExcel(model.ToList(), "Failure Effect", title);
        }
    }
}
