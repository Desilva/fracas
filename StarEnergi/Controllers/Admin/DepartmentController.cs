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
    [Authorize]
    public class DepartmentController : Controller
    {
        //
        // GET: /Division/

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            ViewData["rca_facility"] = db.rca_facility.ToList();
            return PartialView(db.rca_department.ToList());
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
            rca_department department = new rca_department();
            if (TryUpdateModel(department))
            {
                create(department);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id, int id_facility)
        {
            rca_facility fac = db.rca_facility.Find(id_facility);
            rca_department editable = db.rca_department.Find(id);
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
            return View(new GridModel<rca_department>
            {
                Data = db.rca_department.ToList()
            });
        }

        //insert data failure mode
        public void create(rca_department department)
        {
            db.rca_department.Add(department);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(rca_department department)
        {
            db.Entry(department).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            rca_department department = db.rca_department.Find(id);
            db.rca_department.Remove(department);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
