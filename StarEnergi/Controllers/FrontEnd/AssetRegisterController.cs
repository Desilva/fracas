using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;
using StarEnergi.Models.ExportExcel;
using StarEnergi;

namespace StarEnergi.Controllers.FrontEnd
{
    public class AssetRegisterController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /AssetRegister/

        public ActionResult Index()
        {
            Config.menu = Config.MenuFrontEnd.ASSETREGISTER;
            ViewBag.Nama = "Asset Register";
            //var model = from o in db.equipments
            //            select new AssetRegisterEntity
            //            {
            //                id_equipment = o.id,
            //                tag_number = o.tag_num,
            //                system_name = o.equipment_groups.system.nama,
            //                unit_name = o.equipment_groups.system.unit.nama,
            //                area_name = o.equipment_groups.system.unit.foc.nama,
            //                id_discipline = o.id_discipline,
            //                mtbf = o.mtbf,
            //                mttr = o.mttr
            //            };
            //foreach(AssetRegisterEntity a in model ){
            //    a.discipline = a.id_discipline != null ? (db.disciplines.Find(a.id_discipline) != null ? db.disciplines.Find(a.id_discipline).title : "") : "";
            //}
            //return View(model.ToList());
            List<plant> plant = db.plants.ToList();
            plant = plant.OrderBy(a => a.nama).ToList();
            foreach (plant p in plant)
            {
                p.focs = p.focs.OrderBy(a => a.nama).ToList();
            }
            return View(plant);
        }

        public ActionResult Back()
        {
            Config.menu = Config.MenuFrontEnd.ASSETREGISTER;
            ViewBag.Nama = "Asset Register";
            var model = from o in db.equipments
                        select new AssetRegisterEntity
                        {
                            id_equipment = o.id,
                            tag_number = o.tag_num,
                            system_name = o.equipment_groups.system.nama,
                            unit_name = o.equipment_groups.system.unit.nama,
                            area_name = o.equipment_groups.system.unit.foc.nama,
                            id_discipline = o.id_discipline,
                            mtbf = o.mtbf,
                            mttr = o.mttr
                        };
            foreach (AssetRegisterEntity a in model)
            {
                a.discipline = a.id_discipline != null ? (db.disciplines.Find(a.id_discipline) != null ? db.disciplines.Find(a.id_discipline).title : "") : "";
            }
            return PartialView("Index",model.ToList());
        }

        //
        //Select ajax binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //select equipment
        private ViewResult binding()
        {
            var model = from o in db.equipments
                        select new AssetRegisterEntity
                        {
                            id_equipment = o.id,
                            tag_number = o.tag_num,
                            system_name = o.equipment_groups.system.nama,
                            unit_name = o.equipment_groups.system.unit.nama,
                            area_name = o.equipment_groups.system.unit.foc.nama,
                            id_discipline = o.id_discipline,
                            mtbf = o.mtbf,
                            mttr = o.mttr
                        };
            foreach (AssetRegisterEntity a in model)
            {
                a.discipline = a.id_discipline != null ? (db.disciplines.Find(a.id_discipline) != null ? db.disciplines.Find(a.id_discipline).title : "") : "";
            }

            return View(new GridModel<AssetRegisterEntity>
            {
                Data = model.ToList()
            });
        }

        public ActionResult DetailEquipment(int id) {
            ViewBag.nama = db.equipments.Find(id).tag_num;
            ViewBag.id = id;
            return PartialView();
        }

        public ActionResult AjaxView_Equipment(int id)
        {
            equipment e = db.equipments.Find(id);
            e.discipline = db.disciplines.Find(e.id_discipline);
            return PartialView(e);
        }

        public ActionResult AjaxView_PartList(int id)
        {
            //ViewBag.nama = db.equipments.Find(id).tag_num;
            //ViewBag.id = id;
            return PartialView();
        }

        [GridAction]
        public ActionResult _SelectAjaxEditingPart(int id)
        {
            return bindingPart(id);
        }

        //select equipment
        private ViewResult bindingPart(int id)
        {
            var model = from o in db.equipment_part
                        where o.id_equipment == id
                        select new EquipmentPartEntity
                        {
                            id = o.id,
                            tag_num = o.part.tag_number,
                            nama = o.part.nama
                        };

            return View(new GridModel<EquipmentPartEntity>
            {
                Data = model.ToList()
            });
        }

        public JsonResult GetDetail(int id)
        {
            var model = from o in db.equipment_part
                        where o.id == id
                        select new EquipmentPartEntity
                        {
                            id = o.id,
                            vendor = o.part.vendor,
                            warranty = o.part.warranty,
                            installed_date = o.installed_date,
                            obsolete_date = o.obsolete_date,
                            status = o.status,
                            mtbf = o.mtbf,
                            mttr = o.mttr,
                            lead_time = o.lead_time,
                            tag_num = o.part.tag_number,
                            nama = o.part.nama
                        };
            List<EquipmentPartEntity> temp = model.ToList();

            foreach (EquipmentPartEntity t in temp) {
                t.sInstalled_date = t.installed_date.ToString();
                t.sObsolete_date = t.obsolete_date.ToString();
                t.running_hours =  DateTime.Now.Subtract((DateTime)t.installed_date).TotalHours;
                if(t.status == 1){
                    t.sStatus = "Running";
                }else{
                    t.sStatus = "Down";
                }
            }

            return Json(new { partDetail = temp });
        }

        //export excel
        public ActionResult importExcel()
        {
            var model = from o in db.equipments
                        select new AssetRegisterExport
                        {
                            tag_number = o.tag_num,
                            system_name = o.equipment_groups.system.nama,
                            unit_name = o.equipment_groups.system.unit.nama,
                            area_name = o.equipment_groups.system.unit.foc.nama,
                            discipline = o.discipline.title,
                            mtbf = o.mtbf,
                            mttr = o.mttr
                        };

            GridView gv = new GridView();
            gv.Caption = "Asset Register ";
            gv.DataSource = model.ToList();
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Tag Number";
            gv.HeaderRow.Cells[1].Text = "System";
            gv.HeaderRow.Cells[2].Text = "Unit";
            gv.HeaderRow.Cells[3].Text = "Area";
            gv.HeaderRow.Cells[4].Text = "Discipline";
            gv.HeaderRow.Cells[5].Text = "MTBF";
            gv.HeaderRow.Cells[6].Text = "MTTR";

            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "AssetRegister.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }
    }
}
