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
    public class MasterRCController : Controller
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
            rc data = new rc();
            if (TryUpdateModel(data))
            {
                db.rcs.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            rc data = db.rcs.Find(id);
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
            rc data = db.rcs.Find(id);
            db.rcs.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterRCEntity> entity = new List<MasterRCEntity>();
            List<rc> listData = db.rcs.ToList();
            foreach (rc a in listData)
            {
                var temp = new MasterRCEntity
                {
                    id = a.id,
                    rc_description = a.rc_description,
                    rc_score = a.rc_score,
                    rc_value = a.rc_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterRCEntity>
            {
                Data = entity
            });
        }
    }
}
