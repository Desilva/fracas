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
    public class AnalysisTypeController : Controller
    {
        //
        // GET: /AnalysisType/

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            return PartialView(db.rca_analisys_type.ToList());
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
            rca_analisys_type analysisType = new rca_analisys_type();
            if (TryUpdateModel(analysisType))
            {
                create(analysisType);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            rca_analisys_type editable = db.rca_analisys_type.Find(id);
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
            return View(new GridModel<rca_analisys_type>
            {
                Data = db.rca_analisys_type.ToList()
            });
        }

        //insert data failure mode
        public void create(rca_analisys_type analysisType)
        {
            db.rca_analisys_type.Add(analysisType);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(rca_analisys_type analysisType)
        {
            db.Entry(analysisType).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            rca_analisys_type analysisType = db.rca_analisys_type.Find(id);
            db.rca_analisys_type.Remove(analysisType);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
