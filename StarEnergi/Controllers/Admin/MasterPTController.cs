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
    public class MasterPTController : Controller
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
            pt data = new pt();
            if (TryUpdateModel(data))
            {
                db.pts.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            pt data = db.pts.Find(id);
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
            
            pt data = db.pts.Find(id);
            db.pts.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterPTEntity> entity = new List<MasterPTEntity>();
            List<pt> listData = db.pts.ToList();
            foreach (pt a in listData)
            {
                var temp = new MasterPTEntity
                {
                    id = a.id,
                    pt_description = a.pt_description,
                    pt_score = a.pt_score,
                    pt_value = a.pt_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterPTEntity>
            {
                Data = entity
            });
        }
    }
}
