using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using StarEnergi.Utilities;
using System.Data.Entity.Validation;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class SystemController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();

        //
        // GET: /System/

        public ActionResult Index()
        {
            var systems = db.systems.Include(s => s.unit);
            return PartialView(systems.ToList());
        }

        //
        // GET: /System/Details/5

        public ActionResult Details(int id)
        {
            system sys = db.systems.Find(id);
            if (sys.pt != null)
                ViewBag.pt = sys.pt.pt_value + "-" + sys.pt.pt_description;
            if (sys.pe != null)
                ViewBag.pe = sys.pe.pe_value + "-" + sys.pe.pe_description;           
            if (sys.oc != null)
                ViewBag.oc = sys.oc.oc_value + "-" + sys.oc.oc_description;
            if (sys.sf != null)
                ViewBag.sf = sys.sf.sf_value + "-" + sys.sf.sf_description;            
            if (sys.rc != null)
                ViewBag.rc = sys.rc.rc_value + "-" + sys.rc.rc_description;
            if (sys.scr != null)
                ViewBag.scr = sys.scr.Value;            
            return PartialView(sys);
        }

        private void ReCalculcateComponentValues(int idSystem)
        {
            system sys = db.systems.Where(x => x.id == idSystem).FirstOrDefault();
            if(sys!=null)
            {
                List<equipment_groups> listEquipmentGroup = db.equipment_groups.Where(x => x.id_system == idSystem).ToList();
                if (listEquipmentGroup.Count > 0)
                {
                    foreach (var a in listEquipmentGroup)
                    {
                        if(sys.scr.HasValue)
                        {
                            ProcessReCalculate(a.id, sys.scr);
                        }
                    }
                }
            }
        }

        private void ProcessReCalculate(int idEquipmentGroup,double? systemScr)
        {
            List<equipment> listEquipment = db.equipments.Where(x => x.id_equipment_group == idEquipmentGroup).ToList();
            if(listEquipment.Count > 0)
            {
                foreach(var a in listEquipment)
                {
                    bool saveChanges = false;
                    if(a.id_ocr.HasValue)
                    {
                        saveChanges = true;
                        a.acr = systemScr.Value * a.ocr.ocr_value;
                    }

                    if(a.id_afp.HasValue)
                    {
                        saveChanges = true;
                        a.mpi = a.acr * a.afp.afp_value;
                    }

                    if(saveChanges == true)
                    {
                        db.Entry(a).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        //
        // GET: /System/Create

        public ActionResult Create(int id)
        {
            ViewBag.id_unit = id;
            return PartialView();
        }

        //
        // POST: /System/Create

        [HttpPost]
        public ActionResult Create(system system)
        {                   
            if (ModelState.IsValid)
            {
                if (db.systems.Where(x => x.kode == system.kode).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                db.systems.Add(system);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    ///dummy data equipment group
                    ///
                    equipment_groups equipGroup = new equipment_groups();
                    equipGroup.id_system = system.id;
                    equipGroup.nama = string.Empty;

                    var hasGroup = db.equipment_groups.Any(n => n.id_system == system.id);
                    if (!hasGroup)
                        db.equipment_groups.Add(equipGroup);

                    system_paf systemPaf = new system_paf()
                    {
                        id_system = system.id
                    };
                    db.system_paf.Add(systemPaf);
                    db.SaveChanges();
                    ReCalculcateComponentValues(system.id);
                    return Json(e.Succes(system.id.ToString()));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /System/Edit/5

        public ActionResult Edit(int id)
        {
            system system = db.systems.Find(id);
            ViewBag.id_unit = system.id_unit;
            ViewBag.pt = system.id_pt;
            ViewBag.pe = system.id_pe;            
            ViewBag.oc = system.id_oc;
            ViewBag.sf = system.id_sf;            
            ViewBag.rc = system.id_rc;
            ViewBag.scr = system.scr;            
            return PartialView(system);
        }

        //
        // POST: /System/Edit/5

        [HttpPost]
        public ActionResult Edit(system system)
        {
            if (ModelState.IsValid)
            {
                db.Entry(system).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    ReCalculcateComponentValues(system.id);
                    string result = Config.TreeType.SYSTEM + ";" + system.id;
                    return Json(e.Succes(result));
                }
                else
                {
                    return Json(e.Fail(error));
                }
            }

            return Json(e.Fail(ModelState));
        }

        //
        // GET: /System/Delete/5

        public ActionResult Delete(int id)
        {
            system system = db.systems.Find(id);
            return PartialView(system);
        }

        //
        // POST: /System/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            system system = db.systems.Find(id);
            db.systems.Remove(system);
            db.SaveChanges();
            return Json(true);
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult GetPT()
        {
            var model = from o in db.pts
                        select new PTEntity
                        {
                            id = o.id,
                            nama = o.pt_description,
                            pt_value = o.pt_value,
                            pt_score = o.pt_score
                        };
            return Json(model.ToList());
        }

        public JsonResult GetOC()
        {
            var model = from o in db.ocs
                        select new OCEntity
                        {
                            id = o.id,
                            nama = o.oc_description,
                            oc_value = o.oc_value,
                            oc_score = o.oc_score
                        };
            return Json(model.ToList());
        }

        public JsonResult GetSF()
        {
            var model = from o in db.sfs
                        select new SFEntity
                        {
                            id = o.id,
                            nama = o.sf_description,
                            sf_value = o.sf_value,
                            sf_score = o.sf_score
                        };
            return Json(model.ToList());
        }

        public JsonResult GetRC()
        {
            var model = from o in db.rcs
                        select new RCEntity
                        {
                            id = o.id,
                            nama = o.rc_description,
                            rc_value = o.rc_value,
                            rc_score = o.rc_score
                        };
            return Json(model.ToList());
        }

        public JsonResult GetPE()
        {
            var model = from o in db.pes
                        select new PEEntity
                        {
                            id = o.id,
                            nama = o.pe_description,
                            pe_value = o.pe_value,
                            pe_score = o.pe_score
                        };
            return Json(model.ToList());
        }
       
    }
}