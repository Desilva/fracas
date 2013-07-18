using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class EventDescriptionController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /EventDescription/

        public ActionResult Index()
        {
            return PartialView(db.event_descriptions.ToList());
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
            event_descriptions eventDescription = new event_descriptions();
            if (TryUpdateModel(eventDescription))
            {
                create(eventDescription);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            event_descriptions editable = db.event_descriptions.Find(id);
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
            return View(new GridModel<event_descriptions>
            {
                Data = db.event_descriptions.ToList()
            });
        }

        //insert data failure mode
        public void create(event_descriptions eventDescription)
        {
            db.event_descriptions.Add(eventDescription);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(event_descriptions eventDescription)
        {
            db.Entry(eventDescription).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            event_descriptions eventDescription = db.event_descriptions.Find(id);
            db.event_descriptions.Remove(eventDescription);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}