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
    public class PirClauseController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /PirClause/

        public ActionResult Index()
        {
            var r = (from clause in db.pir_clause
                     select new PirClauseEntity
                     {
                         id = clause.id,
                         clause_no = clause.clause_no,
                         clause = clause.clause
                     }).ToList();
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
            pir_clause clause = new pir_clause();
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

            pir_clause editable = db.pir_clause.Find(id);
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
            var d = (from clause in db.pir_clause
                     select new PirClauseEntity
                     {
                         id = clause.id,
                         clause_no = clause.clause_no,
                         clause = clause.clause
                     }).ToList();
            return View(new GridModel<PirClauseEntity>
            {
                Data = d
            });
        }

        //insert data PirClause
        public void create(pir_clause clause)
        {
            db.pir_clause.Add(clause);
            db.SaveChanges();
        }

        //update data PirClause
        private void update(pir_clause clause)
        {
            db.Entry(clause).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data PirClause
        private void delete(int id)
        {
            pir_clause pirClause = db.pir_clause.Find(id);
            db.pir_clause.Remove(pirClause);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
