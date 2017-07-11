using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;

namespace StarEnergi.Controllers.Admin
{
    public class BuildOfMaterialController : Controller
    {
        //
        // GET: /BuildOfMaterial/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private string equip = "equip";
        private string comp = "comp";
        public ActionResult Index()
        {
            return PartialView();
        }

        //public ActionResult RenderDetails(int id, int level)
        //{
        //    ViewBag.id = id;
        //    ViewBag.level = level;
        //    return PartialView();
        //}

        //public ActionResult RenderUpdate(int id, int level)
        //{
        //    ViewBag.id = id;
        //    ViewBag.level = level;
        //    ViewData["build_of_material"] = db.build_of_materials.ToList();
        //    return PartialView();
        //}

        //[GridAction]
        //public ActionResult _SelectAjaxEditing(int id_reference, int level)
        //{
        //    return bindingBom(id_reference, level);
        //}

        //// Ajax insert binding
        //[AcceptVerbs(HttpVerbs.Post)]
        //[GridAction]
        //public ActionResult _InsertAjaxEditing(int id_reference, int level)
        //{
        //    bom bom = new bom();
        //    if (TryUpdateModel(bom))
        //    {
        //        bom.level_equip = level;
        //        bom.id_reference = id_reference;
        //        db.boms.Add(bom);
        //        db.SaveChanges();
        //    }

        //    return bindingBom(id_reference, level);
        //}

        //// Ajax insert binding
        //[AcceptVerbs(HttpVerbs.Post)]
        //[GridAction]
        //public ActionResult _SaveAjaxEditing(int id, int id_reference, int level)
        //{
        //    bom editable = db.boms.Find(id);
        //    if (TryUpdateModel(editable))
        //    {
        //        bom target = db.boms.Where(p => p.id == editable.id).FirstOrDefault();
        //        target.no_key_map = editable.no_key_map;
        //        target.description = editable.description;
        //        db.SaveChanges();
        //    }
        //    return bindingBom(id_reference, level);
        //}

        //[GridAction]
        //public ActionResult _DeleteAjaxEditing(int id, int id_reference, int level)
        //{
        //    bom remove = db.boms.Find(id);
        //    db.boms.Remove(remove);
        //    db.SaveChanges();
        //    return bindingBom(id_reference, level);
        //}

        ////select equipment
        //private ActionResult bindingBom(int id, int level)
        //{
        //    return View(new GridModel<bom>
        //    {
        //        Data = db.boms.Where(x => x.id_reference == id).Where(x => x.level_equip == level).ToList()
        //    });
        //}


        public ActionResult RenderDetails(int id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;            
            ViewData["build_of_material"] = db.build_of_materials.Where(n => n.is_delete == false).ToList();
            return PartialView();
        }

        public ActionResult RenderUpdate(int id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;            
            var listData = db.build_of_materials.Where(n => n.is_delete == false).ToList();
            ViewData["build_of_material"] = listData;
            return PartialView();
        }

        public ActionResult RenderDetailsComponent(int id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;
            ViewData["build_of_material"] = db.build_of_materials.Where(n => n.is_delete == false).ToList();
            return PartialView();
        }

        public ActionResult RenderUpdateComponent(int id, string type)
        {
            ViewBag.id = id;
            ViewBag.type = type;
            ViewData["build_of_material"] = db.build_of_materials.Where(n => n.is_delete == false).ToList();
            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectAjaxEditing(int id_reference, string type)
        {
            return bindingBom(id_reference, type);
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing(int id_reference, string type, int? idbom)
        {
            if (idbom != null)
            {
                if (type == equip)
                {
                    bom_equipment be = new bom_equipment();
                    if (TryUpdateModel(be))
                    {
                        be.id_equipment = id_reference;
                        be.id_bom = idbom.Value;
                        db.bom_equipments.Add(be);
                        db.SaveChanges();
                    }
                }
                else if (type == comp)
                {
                    bom_component bc = new bom_component();
                    if (TryUpdateModel(bc))
                    {
                        bc.id_component = id_reference;
                        bc.id_bom = idbom.Value;
                        db.bom_components.Add(bc);
                        db.SaveChanges();
                    }
                }
            }
            return bindingBom(id_reference, type);
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id, int id_reference, string type, int? idbom)
        {
            if (idbom != null)
            {
                if (type == equip)
                {
                    bom_equipment be = db.bom_equipments.Find(id);
                    if (TryUpdateModel(be))
                    {
                        bom_equipment target = db.bom_equipments.Where(p => p.id == be.id).FirstOrDefault();
                        target.id_bom = be.id_bom;
                        db.SaveChanges();
                    }
                }
                else if (type == comp)
                {
                    bom_component bc = db.bom_components.Find(id);
                    if (TryUpdateModel(bc))
                    {
                        bom_component target = db.bom_components.Where(p => p.id == bc.id).FirstOrDefault();
                        target.id_bom = bc.id_bom;
                        db.SaveChanges();
                    }
                }
            }
            return bindingBom(id_reference, type);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateAjaxEditing(int id, int id_reference, string type, int idBom)
        {
            if (type == equip)
            {
                bom_equipment be = db.bom_equipments.Find(id);
                if (TryUpdateModel(be))
                {
                    bom_equipment target = db.bom_equipments.Where(p => p.id == be.id).FirstOrDefault();
                    target.id_bom = idBom;
                    db.SaveChanges();
                }
            }
            else if (type == comp)
            {
                bom_component bc = db.bom_components.Find(id);
                if (TryUpdateModel(bc))
                {
                    bom_component target = db.bom_components.Where(p => p.id == bc.id).FirstOrDefault();
                    target.id_bom = idBom;
                    db.SaveChanges();
                }
            }
            return bindingBom(id_reference, type);
        }

        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id, int id_reference, string type)
        {
            if (type == equip)
            {
                bom_equipment be = db.bom_equipments.Find(id);
                db.bom_equipments.Remove(be);
                db.SaveChanges();
            }
            else if (type == comp)
            {
                bom_component bc = db.bom_components.Find(id);
                db.bom_components.Remove(bc);
                db.SaveChanges();
            }
            return bindingBom(id_reference, type);
        }

        //select equipment
        private ActionResult bindingBom(int id, string type)
        {
            if (type == equip)
            {
                List<BomEquipmentEntity> beEntity = new List<BomEquipmentEntity>();
                List<bom_equipment> listData = db.bom_equipments.Where(x => x.build_of_material.is_delete == false && x.id_equipment == id).ToList();
                foreach (bom_equipment be in listData)
                {
                    List<build_of_material> listBom = db.build_of_materials.Where(n => n.id == be.id_bom).ToList();
                    string keymap = ""; string location="";
                    if (listBom.Count > 0)
                    {
                        keymap = listBom.FirstOrDefault().no_keymap;
                        location = listBom.FirstOrDefault().functional_location;
                    }
                    BomEquipmentEntity temp = new BomEquipmentEntity
                    {
                        id = be.id,
                        id_bom = be.id_bom,
                        no_keymap = keymap,
                        functional_location = location,
                    };
                    beEntity.Add(temp);
                }
                return View(new GridModel<BomEquipmentEntity>
                {
                    Data = beEntity
                });
            }
            else
            {
                List<BomEquipmentEntity> bcEntity = new List<BomEquipmentEntity>();
                List<bom_component> listData = db.bom_components.Where(x => x.build_of_material.is_delete == false && x.id_component == id).ToList();
                component component = db.components.Find(id);
                foreach (bom_component bc in listData)
                {
                    List<build_of_material> listBom = db.build_of_materials.Where(n => n.id == bc.id_bom).ToList();
                    string keymap = ""; string location = "";
                    if (listBom.Count > 0)
                    {
                        keymap = listBom.FirstOrDefault().no_keymap;
                        location = listBom.FirstOrDefault().functional_location;
                    }
                    BomEquipmentEntity temp = new BomEquipmentEntity
                    {
                        id = bc.id,
                        id_bom = bc.id_bom,
                        no_keymap = keymap,
                        functional_location = location,
                    };
                    bcEntity.Add(temp);
                }
                return View(new GridModel<BomEquipmentEntity>
                {
                    Data = bcEntity
                });
            }
        }


        [GridAction]
        public ActionResult _SelectGridEditing()
        {
            return BindingBuildOfMaterial();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertGridEditing()
        {
            build_of_material bom = new build_of_material();
            if (TryUpdateModel(bom))
            {
                db.build_of_materials.Add(bom);
                db.SaveChanges();
            }

            return BindingBuildOfMaterial();
        }

        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveGridEditing(int id)
        {
            build_of_material editable = db.build_of_materials.Find(id);
            if (TryUpdateModel(editable))
            {
                db.Entry(editable).State = EntityState.Modified;
                db.SaveChanges();
            }
            return BindingBuildOfMaterial();
        }

        [GridAction]
        public ActionResult _DeleteGridEditing(int id)
        {
            build_of_material bom = db.build_of_materials.Find(id);
            bom.is_delete = true;
            db.Entry(bom).State = EntityState.Modified;
            db.SaveChanges();
            return BindingBuildOfMaterial();
        }

        private ActionResult BindingBuildOfMaterial()
        {
            List<BomEntity> bomEntity = new List<BomEntity>();
            List<build_of_material> listData = db.build_of_materials.Where(n => n.is_delete == false).ToList();
            foreach (build_of_material bm in listData)
            {
                BomEntity temp = new BomEntity
                {
                    id = bm.id,
                    is_delete = bm.is_delete,
                    functional_location = bm.functional_location,
                    no_keymap = bm.no_keymap
                };
                bomEntity.Add(temp);
            }
            return View(new GridModel<BomEntity>
            {
                Data = bomEntity
            });
        }
        public string GetKeyMap(int id)
        {
            var result = "";
            if (id > 0)
            {
                build_of_material data = db.build_of_materials.Find(id);
                result = data.no_keymap;
            }
            ViewBag.idBom = id;
            return result;
        }
    }
}
