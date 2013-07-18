using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class ProcessOwnerController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /ProcessOwner/

        public ActionResult Index()
        {
            return PartialView(db.process_owner.ToList());
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
            process_owner processowner = new process_owner();
            if (TryUpdateModel(processowner))
            {
                create(processowner);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            process_owner editable = db.process_owner.Find(id);
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

        //select data process owner
        private ViewResult binding()
        {
            return View(new GridModel<process_owner>
            {
                Data = db.process_owner.ToList()
            });
        }

        //insert data process owner
        public void create(process_owner processowner)
        {
            db.process_owner.Add(processowner);
            db.SaveChanges();
        }

        //update data process owner
        private void update(process_owner processowner)
        {
            db.Entry(processowner).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data process owner
        private void delete(int id)
        {
            process_owner processowner = db.process_owner.Find(id);
            db.process_owner.Remove(processowner);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}