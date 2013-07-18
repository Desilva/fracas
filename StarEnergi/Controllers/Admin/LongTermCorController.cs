using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using StarEnergi.Utilities;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class LongTermCorController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /LongTermCor/

        public ActionResult Index()
        {
            return PartialView(db.long_term_actions.ToList());
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
            long_term_actions longTermAction = new long_term_actions();
            if (TryUpdateModel(longTermAction))
            {
                create(longTermAction);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            long_term_actions editable = db.long_term_actions.Find(id);
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
            return View(new GridModel<long_term_actions>
            {
                Data = db.long_term_actions.ToList()
            });
        }

        //insert data failure mode
        public void create(long_term_actions longTermAction)
        {
            db.long_term_actions.Add(longTermAction);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(long_term_actions longTermAction)
        {
            db.Entry(longTermAction).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            long_term_actions longTermAction = db.long_term_actions.Find(id);
            db.long_term_actions.Remove(longTermAction);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel()
        {
            var model = db.long_term_actions.Select(f => new { f.action, f.description});
            string[] title = { "Action", "Description" };

            return MyTools.importExcel(model.ToList(), "Long Term Corrective Action", title);
        }
    }
}