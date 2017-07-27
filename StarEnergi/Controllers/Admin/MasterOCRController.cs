using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;
using System.Threading;
using System.Globalization;

namespace StarEnergi.Controllers.Admin
{
    public class MasterOCRController : Controller
    {
        //
        // GET: /BuildOfMaterial/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
     
        public ActionResult Index()
        {
            return PartialView();
        }


        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            ocr data = new ocr();
            if (TryUpdateModel(data))
            {
                db.ocrs.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            ocr data = db.ocrs.Find(id);
            if (TryUpdateModel(data))
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Binding();
        }

        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            unit a;
            ocr data = db.ocrs.Find(id);
            db.ocrs.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterOCREntity> entity = new List<MasterOCREntity>();
            List<ocr> listData = db.ocrs.ToList();
            foreach (ocr a in listData)
            {
                var temp = new MasterOCREntity
                {
                    id = a.id,
                    ocr_description = a.ocr_description,
                    ocr_score = a.ocr_score,
                    ocr_value = a.ocr_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterOCREntity>
            {
                Data = entity
            });
        }
    }
}
