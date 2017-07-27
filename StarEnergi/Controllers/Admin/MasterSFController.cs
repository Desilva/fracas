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
    public class MasterSFController : Controller
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
            sf data = new sf();
            if (TryUpdateModel(data))
            {
                db.sfs.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            sf data = db.sfs.Find(id);
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
            sf data = db.sfs.Find(id);
            db.sfs.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterSFEntity> entity = new List<MasterSFEntity>();
            List<sf> listData = db.sfs.ToList();
            foreach (sf a in listData)
            {
                var temp = new MasterSFEntity
                {
                    id = a.id,
                    sf_description = a.sf_description,
                    sf_score = a.sf_score,
                    sf_value = a.sf_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterSFEntity>
            {
                Data = entity
            });
        }
    }
}
