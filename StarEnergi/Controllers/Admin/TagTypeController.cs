using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class TagTypeController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /TagType/

        public ActionResult Index()
        {
            return PartialView();
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
            TagTypeEntity tagType = new TagTypeEntity();
            if (TryUpdateModel(tagType))
            {
                create(tagType);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            var editable = new TagTypeEntity
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
            var model = from o in db.tag_types
                        select new TagTypeEntity
                        {
                            id = o.id,
                            title = o.title
                        };
            return View(new GridModel<TagTypeEntity>
            {
                Data = model
            });
        }

        //insert data failure mode
        public void create(TagTypeEntity tagType)
        {
            tag_types tag_types = new tag_types();
            tag_types.title = tagType.title;
            db.tag_types.Add(tag_types);
            db.SaveChanges();
        }

        //update data failure mode
        private void update(TagTypeEntity tagTypeEntity)
        {
            tag_types tag_types = db.tag_types.Find(tagTypeEntity.id);
            tag_types.title = tagTypeEntity.title;
            db.Entry(tag_types).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            tag_types tag_types = db.tag_types.Find(id);
            db.tag_types.Remove(tag_types);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}