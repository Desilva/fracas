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
    public class MasterAFPFController : Controller
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
            afp data = new afp();
            if (TryUpdateModel(data))
            {
                db.afps.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            afp data = db.afps.Find(id);
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
            afp data = db.afps.Find(id);
            db.afps.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterAFPFEntity> entity = new List<MasterAFPFEntity>();
            List<afp> listData = db.afps.ToList();
            foreach (afp a in listData)
            {
                var temp = new MasterAFPFEntity
                {
                    id = a.id,
                    afp_description = a.afp_description,
                    afp_score = a.afp_score,
                    afp_value = a.afp_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterAFPFEntity>
            {
                Data = entity
            });
        }
    }
}
