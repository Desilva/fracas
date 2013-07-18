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
    public class PartController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Part/

        public ActionResult Index(int id)
        {
            ViewBag.id_equipment = id;
            return PartialView();
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing(int id)
        {
            ViewBag.id_equipment = id;
            return binding();
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            part partEntity = new part();
            if (TryUpdateModel(partEntity))
            {
                create(partEntity);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var editable = new PartEntity
            {
                id = id
            };
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
            var model = from x in db.parts
                        select new PartEntity
                        {
                            id = x.id,
                            tag_number = x.tag_number,
                            nama = x.nama,
                            assurance_level = x.assurance_level,
                            vendor = x.vendor,
                            warranty = x.warranty,
                            key_map = x.key_map
                        };

            return View(new GridModel<PartEntity>
            {
                Data = model.ToList()
            });
        }

        //insert data failure mode
        public void create(part partEntity)
        {
            db.parts.Add(partEntity);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(PartEntity partEntity)
        {
            part part = db.parts.Find(partEntity.id);
            part.tag_number = partEntity.tag_number;
            part.nama = partEntity.nama;
            part.vendor = partEntity.vendor;
            part.warranty = partEntity.warranty;
            part.assurance_level = partEntity.assurance_level;
            part.key_map = partEntity.key_map;
            db.Entry(part).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            part part = db.parts.Find(id);
            db.parts.Remove(part);
            db.SaveChanges();
        }


        public JsonResult Exist(string tagNumber) {
            if(db.parts.Where(x => x.tag_number == tagNumber).ToList().Count() > 0){
                return Json(e.Fail());
            }
            return Json(e.Succes("Succes"));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}