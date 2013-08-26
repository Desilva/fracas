using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Data.Entity.Validation;
using StarEnergi.Utilities;
using Telerik.Web.Mvc;
using System.Collections.Specialized;
using StarEnergi.Utilities.Statistical_Engine;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Equipment/

        public ActionResult Index()
        {
            var equipments = db.equipments.Include(e => e.equipment_groups);
            return PartialView(equipments.ToList());
        }

        //
        // GET: /Equipment/Details/5

        public ActionResult Details(int id)
        {
            equipment equipment = db.equipments.Find(id);
            if (equipment.status == 1)
            {
                ViewBag.status = "running";
            }
            else {
                ViewBag.status = "down";
            }
            EquipmentEntity epe = new EquipmentEntity(equipment);
            PopulateTagNumberEquipment();
            return PartialView(epe);
        }

        //
        // GET: /Equipment/Create/2

        public ActionResult Create(int id)
        {
            //ViewBag.id_equipment_group = new SelectList(db.equipment_groups, "id", "nama");
            ViewBag.id_discipline = new SelectList(db.disciplines, "id","title");
            ViewBag.id_tag_type = new SelectList(db.tag_types, "id", "title");
            ViewBag.id_equipment_group = id;
            return PartialView();
        }

        //
        // POST: /Equipment/Create

        [HttpPost]
        public ActionResult Create(equipment equipment)
        {        
            if (ModelState.IsValid)
            {

                if (db.equipments.Where(x => x.tag_num == equipment.tag_num).ToList().Count > 0)
                {
                    return Json(e.Fail());
                }

                DateTime installedDate =(DateTime)equipment.installed_date;
                equipment.obsolete_date = installedDate.Add(new TimeSpan((int)equipment.warranty, 0, 0));
                db.equipments.Add(equipment);
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();
                    equipment_readiness_nav eqReadNav = new equipment_readiness_nav()
                    {
                        id_equipment = equipment.id
                    };
                    equipment_paf eqPaf = new equipment_paf()
                    {
                        id_equipment = equipment.id
                    };
                    equipment_event eqEvent = new equipment_event()
                    {
                        id_equipment = equipment.id,
                        datetime_ops = installedDate
                    };

                    db.equipment_readiness_nav.Add(eqReadNav);
                    db.equipment_paf.Add(eqPaf);
                    db.equipment_event.Add(eqEvent);

                    db.SaveChanges();
                    return Json(e.Succes(equipment.id.ToString()));
                }
                else {
                    //return Json(error.First().ValidationErrors.ToArray());
                    return Json(e.Fail(error));
                }
                
                //return RedirectToAction("Index");  
                
            }
            ViewBag.id_discipline = new SelectList(db.disciplines, "id", "title");
            ViewBag.id_tag_type = new SelectList(db.tag_types, "id", "title");
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /Equipment/Edit/5

        public ActionResult Edit(int id)
        {
            equipment equipment = db.equipments.Find(id);
            ViewBag.id_discipline = new SelectList(db.disciplines, "id", "title", equipment.id_discipline);
            ViewBag.id_tag_type = new SelectList(db.tag_types, "id", "title", equipment.id_tag_type);
            ViewBag.id_equipment_group = equipment.id_equipment_group;

            EquipmentEntity epe = new EquipmentEntity(equipment);
            PopulateTagNumberEquipment();
            return PartialView(epe);
        }

        //
        // POST: /Equipment/Edit/5

        [HttpPost]
        public ActionResult Edit(equipment equipment)
        {

            equipment edited = db.equipments.Find(equipment.id);
            edited.tag_num = equipment.tag_num;
            edited.nama = equipment.nama;
            edited.econ = equipment.econ;
            edited.ram_crit = equipment.ram_crit;
            edited.id_discipline = equipment.id_discipline;
            edited.installed_date = equipment.installed_date;
            edited.sertifikasi = equipment.sertifikasi;
            edited.warranty = equipment.warranty;
            edited.vendor = equipment.vendor;
            edited.id_tag_type = equipment.id_tag_type;
            edited.data_sheet_path = equipment.data_sheet_path;
            edited.id_equipment_group = equipment.id_equipment_group;

            if (ModelState.IsValid)
            {
                db.Entry(edited).State = EntityState.Modified;
                IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
                if (error.Count() == 0)
                {
                    db.SaveChanges();

                    CalculateSertificate cs = new CalculateSertificate(edited.id);

                    //return RedirectToAction("Index");
                    string result = Config.TreeType.EQUIPMENTS + ";" + equipment.id;
                    return Json(e.Succes(result));
                }else {
                    //return Json(error.First().ValidationErrors.ToArray());
                    return Json(e.Fail(error));
                }
            }
            ViewBag.id_discipline = new SelectList(db.disciplines, "id", "title");
            ViewBag.id_tag_type = new SelectList(db.tag_types, "id", "title");
            
            return Json(e.Fail(ModelState));
        }

        //
        // GET: /Equipment/Delete/5

        public ActionResult Delete(int id)
        {
            equipment equipment = db.equipments.Find(id);
            return PartialView(equipment);
        }

        //
        // POST: /Equipment/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            equipment equipment = db.equipments.Find(id);
            db.equipments.Remove(equipment);
            db.SaveChanges();
            return Json(true);
            //return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /*public ActionResult Download(string id)
        {
            return File(Config.EquipmentDataSheetFolder+"\\" + id, "application/pdf", id);
        }*/

        

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing(int id)
        {
            PopulateTagNumberEquipment();
            return binding(id);
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            string[] path = Request.RawUrl.Split('/');
            int id_equipment = int.Parse(path.Last());

            EquipmentPartEntity equipmentPartEntity = new EquipmentPartEntity();
            equipmentPartEntity.id_equipment = id_equipment;
            if (TryUpdateModel(equipmentPartEntity))
            {
                //cek part exist
                if (ListPart(id_equipment, equipmentPartEntity.id_parts).Count == 0)
                {
                    create_part(equipmentPartEntity);
                }
                else {
                    return binding(id_equipment);
                }
            }
            return binding(id_equipment);
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            string[] path = Request.RawUrl.Split('/');
            int id_equipment = int.Parse(path.Last());
            var editable = new EquipmentPartEntity
            {
                id = id,
                id_equipment = id_equipment
            };

            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return binding(id_equipment);
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            string[] path = Request.RawUrl.Split('/');
            int id_equipment = int.Parse(path.Last());
            delete(id);
            return binding(id_equipment);
        }

        //insert combo box tag type
        private void PopulateTagNumberEquipment()
        {
            ViewData["parts"] = db.parts;
        }

        //select data part
        private ViewResult binding(int id)
        {
            EquipmentEntity ep = new EquipmentEntity(id);
            return View(new GridModel<EquipmentPartEntity>
            {
                Data = ep.parts
            });
        }

        //insert data equipment part
        public void create_part(EquipmentPartEntity equipmentPartEntity)
        {
            equipment_part ep = new equipment_part();
            ep.id_parts = equipmentPartEntity.id_parts;
            ep.id_equipment = equipmentPartEntity.id_equipment;
            ep.installed_date = equipmentPartEntity.installed_date;
            db.equipment_part.Add(ep);
            db.SaveChanges();

            part_unit_event pEvent = new part_unit_event()
            {
                id_equipment_part = ep.id,
                datetime_ops = ep.installed_date
            };
            db.part_unit_event.Add(pEvent);
            db.SaveChanges();
        }

        //update data equpment part
        private void update(EquipmentPartEntity equipmentPartEntity)
        {
            equipment_part ep = db.equipment_part.Find(equipmentPartEntity.id);
            ep.id_equipment = equipmentPartEntity.id_equipment;
            ep.installed_date = equipmentPartEntity.installed_date;
            db.Entry(ep).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data equipment part
        private void delete(int id)
        {
            equipment_part equipment_part = db.equipment_part.Find(id);
            db.equipment_part.Remove(equipment_part);
            db.SaveChanges();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CheckPart(int id_eq, int id_part) {

            if (ListPart(id_eq, id_part).Count == 0)
            {
                return Json(true);
            }
            else {
                return Json(false);
            }

        }

        private List<equipment_part> ListPart(int id_eq, int id_part)
        {
            return db.equipment_part.Where(a => a.id_equipment == id_eq).Where(a => a.id_parts == id_part).ToList();
        
        }

        [GridAction]
        public ActionResult _PartsForComponentHierarchyAjax(int idEquipment, int idPart)
        {
            return bindingPartsForComponent(idEquipment,idPart);
        }

        private ViewResult bindingPartsForComponent(int idEquipment, int idPart)
        {
            int idEquipmentPart = db.equipment_part.Where(a => a.id_equipment == idEquipment).Where(a => a.id_parts == idPart).Select(a => a.id).FirstOrDefault();
            List<ComponentEntity> ce = new List<ComponentEntity>();
            if (idEquipmentPart != null)
            {
                List<component> c = db.components.Where(a => a.id_equipment_part == idEquipmentPart).ToList();
                foreach (component x in c)
                {
                    ComponentEntity temp = new ComponentEntity()
                    {
                        id_component = x.id,
                        id_equipment_part = x.id_equipment_part,
                        tag_number = x.tag_number,
                        description = x.description
                    };
                    ce.Add(temp);
                }
            }
            return View(new GridModel(ce));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertComponentAjaxEditing(int idEquipment, int idPart)
        {
            int idEquipmentPart = db.equipment_part.Where(a => a.id_equipment == idEquipment).Where(a => a.id_parts == idPart).Select(a => a.id).FirstOrDefault();
            ComponentEntity Component = new ComponentEntity();
            if (TryUpdateModel(Component))
            {
                createComponent(Component, idEquipmentPart);
            }
            return bindingPartsForComponent(idEquipment, idPart);
        }

        //insert data failure mode
        public void createComponent(ComponentEntity ComponentEntity, int idEquipmentPart)
        {
            component component = new component();
            //failureMode.title = failureModeEntity.title;
            component.tag_number = ComponentEntity.tag_number;
            component.description = ComponentEntity.description;
            component.id_equipment_part = idEquipmentPart;
            db.components.Add(component);
            db.SaveChanges();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteComponentAjaxEditing(int id, int idEquipment, int idPart)
        {
            component component = db.components.Find(id);
            db.components.Remove(component);
            db.SaveChanges();
            return bindingPartsForComponent(idEquipment, idPart);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveComponentAjaxEditing(int id, int idEquipment, int idPart)
        {
            ComponentEntity ce = new ComponentEntity();
            if (TryUpdateModel(ce))
            {
                updateComponent(ce);
            }
            return bindingPartsForComponent(idEquipment,idPart);
        }

        private void updateComponent(ComponentEntity ce)
        {
            component c = db.components.Find(ce.id_component);
            c.tag_number = ce.tag_number;
            c.description = ce.description;
            c.id_equipment_part = ce.id_equipment_part;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();
        }

        #region subcomponent
        [GridAction]
        public ActionResult _ComponentsForSubHierarchyAjax(int idComponent)
        {
            return bindingComponentForSubComponent(idComponent);
        }

        private ViewResult bindingComponentForSubComponent(int idComponent)
        {
            List<SubComponentEntity> se = new List<SubComponentEntity>();
            List<sub_component> sc = db.sub_component.Where(a => a.id_component == idComponent).ToList();
            foreach (sub_component x in sc)
            {
                SubComponentEntity temp = new SubComponentEntity()
                {
                    id_sub_component = x.id,
                    id_component = x.id_component,
                    tag_number = x.tag_number,
                    description = x.description
                };
                se.Add(temp);
            }
            return View(new GridModel(se));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertSubComponentAjaxEditing(int idComponent)
        {
            SubComponentEntity SubComponent = new SubComponentEntity();
            if (TryUpdateModel(SubComponent))
            {
                createSubComponent(SubComponent, idComponent);
            }
            return bindingComponentForSubComponent(idComponent);
        }

        //insert data failure mode
        public void createSubComponent(SubComponentEntity SubComponentEntity, int idComponent)
        {
            sub_component subComponent = new sub_component();
            //failureMode.title = failureModeEntity.title;
            subComponent.tag_number = SubComponentEntity.tag_number;
            subComponent.description = SubComponentEntity.description;
            subComponent.id_component = idComponent;
            db.sub_component.Add(subComponent);
            db.SaveChanges();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteSubComponentAjaxEditing(int id, int idComponent)
        {
            sub_component sc = db.sub_component.Find(id);
            db.sub_component.Remove(sc);
            db.SaveChanges();
            return bindingComponentForSubComponent(idComponent);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveSubComponentAjaxEditing(int id, int idComponent)
        {
            SubComponentEntity sce = new SubComponentEntity();
            if (TryUpdateModel(sce))
            {
                updateSubComponent(sce);
            }
            return bindingComponentForSubComponent(idComponent);
        }

        private void updateSubComponent(SubComponentEntity sce)
        {
            sub_component sc = db.sub_component.Find(sce.id_sub_component);
            sc.tag_number = sce.tag_number;
            sc.description = sce.description;
            sc.id_component = sce.id_component;
            db.Entry(sc).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion

        public JsonResult GetSubClass(int id_class)
        {
            var subClass = db.disciplines.Where(x => x.id_tag_type == id_class);

            List<DisciplineEntity> send = new List<DisciplineEntity>();
            foreach (discipline d in subClass) {
                DisciplineEntity dt = new DisciplineEntity
                {
                    id = d.id,
                    id_tag_type = d.id_tag_type,
                    title = d.title
                };
                send.Add(dt);
            }

            return Json(send);
        }
    }
}