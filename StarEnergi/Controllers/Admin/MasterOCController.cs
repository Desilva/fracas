using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;

namespace StarEnergi.Controllers.Admin
{
    public class MasterOCController : Controller
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
            oc data = new oc();
            if (TryUpdateModel(data))
            {
                db.ocs.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            oc data = db.ocs.Find(id);
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
            oc data = db.ocs.Find(id);
            db.ocs.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterOCEntity> entity = new List<MasterOCEntity>();
            List<oc> listData = db.ocs.ToList();
            foreach (oc a in listData)
            {
                var temp = new MasterOCEntity
                {
                    id = a.id,
                    oc_description = a.oc_description,
                    oc_score = a.oc_score,
                    oc_value = a.oc_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterOCEntity>
            {
                Data = entity
            });
        }
    }
}
