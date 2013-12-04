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
    public class RCAPreserveCategoryController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /RCAPreserveCategory/

        public ActionResult Index()
        {
            var r = db.rca_preserve_category.Select(p => new CategoryEntity{ id = p.id, name = p.name}).ToList();
            return PartialView(r);
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
            rca_preserve_category clause = new rca_preserve_category();
            if (TryUpdateModel(clause))
            {
                create(clause);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            rca_preserve_category editable = db.rca_preserve_category.Find(id);
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

        //select data RCAPreserveCategory
        private ViewResult binding()
        {
            var d = db.rca_preserve_category.Select(p => new CategoryEntity { id = p.id, name = p.name }).ToList();
            return View(new GridModel<CategoryEntity>
            {
                Data = d
            });
        }

        //insert data RCAPreserveCategory
        public void create(rca_preserve_category clause)
        {
            db.rca_preserve_category.Add(clause);
            db.SaveChanges();
        }

        //update data RCAPreserveCategory
        private void update(rca_preserve_category clause)
        {
            db.Entry(clause).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data RCAPreserveCategory
        private void delete(int id)
        {
            rca_preserve_category pirClause = db.rca_preserve_category.Find(id);
            db.rca_preserve_category.Remove(pirClause);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
