using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class SecondaryEffectController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /SecondaryEffect/

        public ActionResult Index()
        {
            return PartialView(db.secondary_effects.ToList());
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
            secondary_effects secondary_effect = new secondary_effects();
            if (TryUpdateModel(secondary_effect))
            {
                create(secondary_effect);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            secondary_effects editable = db.secondary_effects.Find(id);
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

        //select data secondary effect
        private ViewResult binding()
        {
            return View(new GridModel<secondary_effects>
            {
                Data = db.secondary_effects.ToList()
            });
        }

        //insert data secondary effect
        public void create(secondary_effects secondary_effect)
        {
            db.secondary_effects.Add(secondary_effect);
            db.SaveChanges();
        }

        //update data secondary effect
        private void update(secondary_effects secondary_effect)
        {
            db.Entry(secondary_effect).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data secondary effect
        private void delete(int id)
        {
            secondary_effects secondary_effect = db.secondary_effects.Find(id);
            db.secondary_effects.Remove(secondary_effect);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult importExcel()
        {
            var model = db.secondary_effects.Select(f => new { f.title, f.description });
            string[] title = { "Title", "Description" };

            return MyTools.importExcel(model.ToList(), "Secondary Effect", title);
        }

    }
}
