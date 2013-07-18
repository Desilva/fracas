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
    public class ImmediateCorController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /ImmediateCor/

        public ActionResult Index()
        {
            return PartialView(db.immediate_actions.ToList());
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
            immediate_actions immediateAction = new immediate_actions();
            if (TryUpdateModel(immediateAction))
            {
                create(immediateAction);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            immediate_actions editable = db.immediate_actions.Find(id);
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
            return View(new GridModel<immediate_actions>
            {
                Data = db.immediate_actions.ToList()
            });
        }

        //insert data failure mode
        public void create(immediate_actions immediateAction)
        {
            db.immediate_actions.Add(immediateAction);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(immediate_actions immediateAction)
        {
            db.Entry(immediateAction).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            immediate_actions immediate_actions = db.immediate_actions.Find(id);
            db.immediate_actions.Remove(immediate_actions);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel()
        {
            var model = db.immediate_actions.Select(f => new { f.action, f.description });
            string[] title = { "Action", "Description" };

            return MyTools.importExcel(model.ToList(), "Immidiate Corrective Action", title);
        }
    }
}