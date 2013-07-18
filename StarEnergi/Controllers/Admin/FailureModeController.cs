using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class FailureModeController : Controller
    {
       
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /FailureMode/

        public ActionResult Index()
        {
            PopulateTagType();
            return PartialView();
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            PopulateTagType();
            return binding();
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            FailureModeEntity failureMode = new FailureModeEntity();
            if (TryUpdateModel(failureMode))
            {
                create(failureMode);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id, int id_tag_type)
        {

            var editable = new FailureModeEntity
            {
                    id = id,
                    tag_types_name = db.tag_types
                        .Where(e => e.id == id_tag_type)
                        .Select(e=>e.title).SingleOrDefault()                        
            };

            if (TryUpdateModel(editable, null, null, new[] { "Employee" }))
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

        //insert combo box tag type
        private void PopulateTagType()
        {
            ViewData["tagTypes"] = db.tag_types;
        }

        //select data failure mode
        private ViewResult binding()
        {
            var model = from o in db.failure_modes
                        select new FailureModeEntity
                        {
                            id = o.id,
                            title = o.title,
                            description = o.description,
                            tag_types_name = o.tag_types.title,
                            id_tag_type = o.id_tag_type
                        };
            return View(new GridModel<FailureModeEntity>
            {
                Data = model
            });
        }

        //insert data failure mode
        public void create(FailureModeEntity failureModeEntity)
        {
            failure_modes failureMode = new failure_modes();
            //failureMode.title = failureModeEntity.title;
            failureMode.description = failureModeEntity.description;
            failureMode.id_tag_type = failureModeEntity.id_tag_type;
            db.failure_modes.Add(failureMode);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(FailureModeEntity failureModesEntity)
        {
            failure_modes failure_modes = db.failure_modes.Find(failureModesEntity.id);
            failure_modes.title = failureModesEntity.title;
            failure_modes.description = failureModesEntity.description;
            failure_modes.id_tag_type = failureModesEntity.id_tag_type;
            db.Entry(failure_modes).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id) {
            failure_modes failure_modes = db.failure_modes.Find(id);
            db.failure_modes.Remove(failure_modes);
            db.SaveChanges();
        }
        

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel() {
            var model = db.failure_modes.Select(f => new{ f.description, f.tag_types.title });
            string[] title = { "Description", "Title" };

            return MyTools.importExcel(model.ToList(),"FailureMode", title);
        }
    }
}