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
    public class SheContractorController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /SheContractor/

        public ActionResult Index()
        {
            var r = db.monthly_she_contractor.ToList();
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
            monthly_she_contractor clause = new monthly_she_contractor();
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

            monthly_she_contractor editable = db.monthly_she_contractor.Find(id);
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

        //select data PirClause
        private ViewResult binding()
        {
            var d = db.monthly_she_contractor.ToList();
            return View(new GridModel<monthly_she_contractor>
            {
                Data = d
            });
        }

        //insert data PirClause
        public void create(monthly_she_contractor clause)
        {
            db.monthly_she_contractor.Add(clause);
            db.SaveChanges();
        }

        //update data PirClause
        private void update(monthly_she_contractor clause)
        {
            db.Entry(clause).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data PirClause
        private void delete(int id)
        {
            monthly_she_contractor pirClause = db.monthly_she_contractor.Find(id);
            db.monthly_she_contractor.Remove(pirClause);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
