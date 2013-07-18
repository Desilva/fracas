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
    public class SectionController : Controller
    {
        //
        // GET: /Department/

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            ViewData["rca_division"] = db.rca_department.ToList();
            return PartialView(db.rca_section.ToList());
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
            rca_section section = new rca_section();
            if (TryUpdateModel(section))
            {
                create(section);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            rca_section editable = db.rca_section.Find(id);
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
            return View(new GridModel<rca_section>
            {
                Data = db.rca_section.ToList()
            });
        }

        //insert data failure mode
        public void create(rca_section section)
        {
            db.rca_section.Add(section);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(rca_section section)
        {
            db.Entry(section).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            rca_section section = db.rca_section.Find(id);
            db.rca_section.Remove(section);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
