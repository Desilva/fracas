using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class FacilityController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /Facility/

        public ActionResult Index()
        {
            return PartialView(db.rca_facility.ToList());
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
            RCAFacilityModel facility = new RCAFacilityModel();
            if (TryUpdateModel(facility))
            {
                create(facility);
            }
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            RCAFacilityModel editable = RCAFacilitySessionRepository.OneView(p=> p.id == id);
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
            List<rca_facility> d = db.rca_facility.ToList();
            foreach (var ds in d)
            {
                Debug.WriteLine(ds.name);
            }
            return View(new GridModel<RCAFacilityModel>
            {
                Data = RCAFacilitySessionRepository.AllView()
            });
        }

        //insert data failure mode
        public void create(RCAFacilityModel facility)
        {
            RCAFacilitySessionRepository.Insert(facility);
        }

        //update data failure mode
        private void update(RCAFacilityModel facility)
        {
            RCAFacilitySessionRepository.UpdateRCA(facility);
        }

        //delete data failure mode
        private void delete(int id)
        {
            RCAFacilityModel facility = RCAFacilitySessionRepository.OneView(p => p.id == id);
            RCAFacilitySessionRepository.Delete(facility);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }

    public static class RCAFacilitySessionRepository
    {
        public static relmon_star_energiEntities db = new relmon_star_energiEntities();

        public static IList<RCAFacilityModel> AllView()
        {
            IList<RCAFacilityModel> result =
                (from o in db.rca_facility
                 select new RCAFacilityModel
                 {
                     id = o.id,
                     name = o.name
                 }
                ).ToList();
            return result;
        }

        public static IList<rca_facility> AllDb()
        {
            IList<rca_facility> result = db.rca_facility.ToList();
            return result;
        }

        public static RCAFacilityModel OneView(Func<RCAFacilityModel, bool> predicate)
        {
            return AllView().Where(predicate).FirstOrDefault();
        }

        public static rca_facility OneDb(Func<rca_facility, bool> predicate)
        {
            return AllDb().Where(predicate).FirstOrDefault();
        }

        public static int Insert(RCAFacilityModel facility)
        {
            facility.id = AllDb().OrderByDescending(p => p.id).First().id + 1;

            rca_facility anal = new rca_facility()
            {
                id = facility.id,
                name = facility.name
            };

            db.rca_facility.Add(anal);
            db.SaveChanges();

            facility.id = db.rcas.Max(p => p.id);

            AllView().Add(facility);

            return facility.id;
        }
        public static void UpdateRCA(RCAFacilityModel facility)
        {
            RCAFacilityModel target = OneView(p => p.id == facility.id);
            rca_facility rca = OneDb(p => p.id == facility.id);
            if (target != null && rca != null)
            {
                target.name = facility.name;

                rca.name = facility.name;
                db.SaveChanges();
            }
        }
        
        public static void Delete(RCAFacilityModel facility)
        {
            rca_facility target = OneDb(p => p.id == facility.id);
            RCAFacilityModel hasil = OneView(p => p.id == facility.id);
            if (target != null)
            {
                Debug.WriteLine("hasil id = " + hasil.id);
                AllView().Remove(hasil);
                db.rca_facility.Remove(target);
                db.SaveChanges();
            }
        }
    }
}
