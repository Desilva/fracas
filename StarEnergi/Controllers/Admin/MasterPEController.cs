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
    public class MasterPEController : Controller
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
            pe data = new pe();
            if (TryUpdateModel(data))
            {
                db.pes.Add(data);
                db.SaveChanges();
            }

            return Binding();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            pe data = db.pes.Find(id);
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
            pe data = db.pes.Find(id);
            db.pes.Remove(data);
            
            db.SaveChanges();
            return Binding();
        }

        //select equipment
        private ActionResult Binding()
        {
            List<MasterPEEntity> entity = new List<MasterPEEntity>();
            List<pe> listData = db.pes.ToList();
            foreach (pe a in listData)
            {
                var temp = new MasterPEEntity
                {
                    id = a.id,
                    pe_description = a.pe_description,
                    pe_score = a.pe_score,
                    pe_value = a.pe_value
                };
                entity.Add(temp);
            }
            return View(new GridModel<MasterPEEntity>
            {
                Data = entity
            });
        }
    }
}
