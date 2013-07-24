using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class BuildOfMaterialController : Controller
    {
        //
        // GET: /BuildOfMaterial/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RenderDetails(int id, int level) {
            ViewBag.id = id;
            ViewBag.level = level;
            return PartialView();
        }

        public ActionResult RenderUpdate(int id, int level)
        {
            ViewBag.id = id;
            ViewBag.level = level;
            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectAjaxEditing(int id_reference, int level)
        {
            return bindingBom(id_reference, level);
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing(int id_reference, int level)
        {
            bom bom = new bom();
            if (TryUpdateModel(bom))
            {
                bom.level_equip = level;
                bom.id_reference = id_reference;
                db.boms.Add(bom);
                db.SaveChanges();
            }

            return bindingBom(id_reference, level);
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id,int id_reference, int level)
        {
            bom editable = db.boms.Find(id);
            if (TryUpdateModel(editable))
            {
                bom target = db.boms.Where(p => p.id == editable.id).FirstOrDefault();
                target.no_key_map = editable.no_key_map;
                target.descrition = editable.descrition;
                db.SaveChanges();
            }
            return bindingBom(id_reference, level);
        }

        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id, int id_reference, int level)
        {
            bom remove = db.boms.Find(id);
            db.boms.Remove(remove);
            db.SaveChanges();
            return bindingBom(id_reference, level);
        }

        //select equipment
        private ActionResult bindingBom(int id, int level)
        {
            return View(new GridModel<bom>
            {
                Data = db.boms.Where(x => x.id_reference == id).Where(x =>x.level_equip == level).ToList()
            });
        }
    }
}
