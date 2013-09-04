using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class SubClassController : Controller
    {
        //
        // GET: /Division/

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            ViewData["equipment_class"] = db.tag_types.ToList();
            return PartialView(db.disciplines.ToList());
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
            discipline discipline = new discipline();
            if (TryUpdateModel(discipline))
            {
                create(discipline);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id){

            discipline editable = db.disciplines.Find(id);
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
            List<DisciplineEntity> temp = new List<DisciplineEntity>();
            foreach (discipline d in db.disciplines.ToList()){
                DisciplineEntity t = new DisciplineEntity
                {
                    id = d.id,
                    id_tag_type = d.id_tag_type,
                    title = d.title
                };
                temp.Add(t);
            }

            return View(new GridModel<DisciplineEntity>
            {
                Data = temp
            });
        }

        //insert data failure mode
        public void create(discipline discipline)
        {
            db.disciplines.Add(discipline);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(discipline discipline)
        {
            db.Entry(discipline).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            discipline discipline = db.disciplines.Find(id);
            db.disciplines.Remove(discipline);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
