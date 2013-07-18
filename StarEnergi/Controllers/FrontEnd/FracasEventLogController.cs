using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using StarEnergi.Models;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;
using StarEnergi.Models.ExportExcel;

namespace StarEnergi.Controllers.FrontEnd
{
    public class FracasEventLogController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /FracasEventLog/

        public ActionResult Index()
        {
            ViewBag.Nama = "Fracas Event Log";
            return View();
        }


        public ActionResult FracasEventLogAsset(int id) {
            return PartialView();
        }

        public ActionResult DetailFracas(int id) {
            string type = Request.QueryString["type"];
            string part = Request.QueryString["part"];
            FracasEventModel details = new FracasEventModel();
            if (type == "eventLog")
            {
                ViewBag.type = "FracasEventLog";
                ViewBag.css = "eventlog";
            }
            else if (type == "history")
            {
                ViewBag.type = "FracasHistory";
                ViewBag.css = "history";
            }

            if (part.Equals("0"))
            {
                var model = from o in db.equipment_event
                            where o.id == id
                            select new FracasEventModel
                            {
                                idEquipment = o.id_equipment,
                                tagNumber = o.equipment.tag_num,
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                durasi = o.durasi,
                                downtime = o.downtime,
                                failureMode = o.failure_mode,
                                cause = o.failure_cause,
                                failureEffect = o.failure_effect,
                                secondaryEffect = o.secondary_effect,
                                failureSeverity = o.failure_severity,
                                failureClss = o.failure_clss,
                                immediateAction = o.immediate_act,
                                longTermAction = o.long_term_act,
                                engineer = o.engineer,
                                financialCost = o.financial_cost,
                                repairCost = o.repair_cost,
                                eventDesc = o.event_description
                            };
                details = model.ToList().First();
                ViewBag.nama = "Detail Event Equipment " + details.tagNumber;
                ViewBag.idEquipment = details.idEquipment;
            }
            else
            {
                var model = from o in db.part_unit_event
                            where o.id == id
                            select new FracasEventModel
                            {
                                idEquipment = o.equipment_part.id_equipment,
                                tagNumber = o.equipment_part.equipment.tag_num,
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                durasi = o.durasi,
                                downtime = o.downtime,
                                failureMode = o.failure_mode,
                                cause = o.failure_cause,
                                failureEffect = o.failure_effect,
                                secondaryEffect = o.secondary_effect,
                                failureSeverity = o.failure_severity,
                                failureClss = o.failure_clss,
                                immediateAction = o.immediate_act,
                                longTermAction = o.long_term_act,
                                engineer = o.engineer,
                                financialCost = o.financial_cost,
                                repairCost = o.repair_cost,
                                eventDesc = o.event_description,
                                relatedItem = o.equipment_part.part.tag_number
                            };
                details = model.ToList().First();
                ViewBag.nama = "Detail Event Equipment " + details.tagNumber;
                ViewBag.idEquipment = details.idEquipment;
            }

            return PartialView(details);
        }

        //
        //Select ajax binding
        [GridAction]
        public ActionResult _SelectAjaxEditingAsset(int id)
        {
            return bindingAsset(id);
        }

        //select equipment
        private ViewResult bindingAsset(int id)
        {
            var model = from o in db.equipment_event
                        where o.datetime_stop != null && o.status == 0 && o.id_equipment == id
                        select new FracasEventModel
                        {
                            id = o.id,
                            tagNumber = o.equipment.tag_num,
                            dateTimeStop = o.datetime_stop,
                            dateTimeStart = o.datetime_ops,
                            unitName = o.equipment.equipment_groups.system.unit.nama,
                            areaName = o.equipment.equipment_groups.system.unit.foc.nama,
                            durasi = o.durasi,
                            downtime = o.downtime,
                            part = 0
                        };
            List<FracasEventModel> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 0 && o.equipment_part.id_equipment == id
                            select new FracasEventModel
                            {
                                id = o.id,
                                tagNumber = o.equipment_part.equipment.tag_num,
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                unitName = o.equipment_part.equipment.equipment_groups.system.unit.nama,
                                areaName = o.equipment_part.equipment.equipment_groups.system.unit.foc.nama,
                                durasi = o.durasi,
                                downtime = o.downtime,
                                part = 1
                            };

            foreach (var item in modelPart)
            {
                result.Add(item);
            }

            return View(new GridModel<FracasEventModel>
            {
                Data = result
            });
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
            var model = from o in db.equipment_event
                        where o.datetime_stop != null && o.status == 0
                        select new FracasEventModel
                        {
                            id = o.id,
                            tagNumber = o.equipment.tag_num,
                            dateTimeStop = o.datetime_stop,
                            dateTimeStart = o.datetime_ops,
                            unitName = o.equipment.equipment_groups.system.unit.nama,
                            areaName = o.equipment.equipment_groups.system.unit.foc.nama,
                            durasi = o.durasi,
                            downtime = o.downtime,
                            part = 0
                        };
            List<FracasEventModel> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 0
                            select new FracasEventModel
                            {
                                id = o.id,
                                tagNumber = o.equipment_part.equipment.tag_num,
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                unitName = o.equipment_part.equipment.equipment_groups.system.unit.nama,
                                areaName = o.equipment_part.equipment.equipment_groups.system.unit.foc.nama,
                                durasi = o.durasi,
                                downtime = o.downtime,
                                part = 1
                            };

            foreach (var item in modelPart){
                result.Add(item);
            }

            return View(new GridModel<FracasEventModel>
            {
                Data = result
            });
        }

        //export excel
        public ActionResult importExcelEquipment(int id)
        {
            string equipment = db.equipments.Find(id).tag_num;
            var model = from o in db.equipment_event
                        where o.datetime_stop != null && o.status == 0 && o.id_equipment == id
                        select new FracasLogExport
                        {
                            dateTimeStop = o.datetime_stop,
                            dateTimeStart = o.datetime_ops,
                            unitName = o.equipment.equipment_groups.system.unit.nama,
                            areaName = o.equipment.equipment_groups.system.unit.foc.nama,
                            durasi = o.durasi,
                            downtime = o.downtime
                        };
            List<FracasLogExport> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 0 && o.equipment_part.id_equipment == id
                            select new FracasLogExport
                            {
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                unitName = o.equipment_part.equipment.equipment_groups.system.unit.nama,
                                areaName = o.equipment_part.equipment.equipment_groups.system.unit.foc.nama,
                                durasi = o.durasi,
                                downtime = o.downtime
                            };

            foreach (var item in modelPart)
            {
                result.Add(item);
            }

            GridView gv = new GridView();
            gv.Caption = "Fracas Event Log equipment " + equipment;
            gv.DataSource = result;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Date/Time Stop";
            gv.HeaderRow.Cells[1].Text = "Date/Time Ops";
            gv.HeaderRow.Cells[2].Text = "Unit";
            gv.HeaderRow.Cells[3].Text = "Area";
            gv.HeaderRow.Cells[4].Text = "Time To Repair";
            gv.HeaderRow.Cells[5].Text = "Dowtime";

            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "FracasLog.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        public ActionResult importExcelIndex()
        {
            var model = from o in db.equipment_event
                        where o.datetime_stop != null && o.status == 0
                        select new FracasLogExport
                        {
                            dateTimeStop = o.datetime_stop,
                            dateTimeStart = o.datetime_ops,
                            unitName = o.equipment.equipment_groups.system.unit.nama,
                            areaName = o.equipment.equipment_groups.system.unit.foc.nama,
                            durasi = o.durasi,
                            downtime = o.downtime
                        };
            List<FracasLogExport> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 0
                            select new FracasLogExport
                            {
                                dateTimeStop = o.datetime_stop,
                                dateTimeStart = o.datetime_ops,
                                unitName = o.equipment_part.equipment.equipment_groups.system.unit.nama,
                                areaName = o.equipment_part.equipment.equipment_groups.system.unit.foc.nama,
                                durasi = o.durasi,
                                downtime = o.downtime
                            };

            foreach (var item in modelPart)
            {
                result.Add(item);
            }

            if (result.Count > 0)
            {
                GridView gv = new GridView();
                gv.Caption = "Fracas Event Log";
                gv.DataSource = result;
                gv.DataBind();
                gv.HeaderRow.Cells[0].Text = "Date/Time Stop";
                gv.HeaderRow.Cells[1].Text = "Date/Time Ops";
                gv.HeaderRow.Cells[2].Text = "Unit";
                gv.HeaderRow.Cells[3].Text = "Area";
                gv.HeaderRow.Cells[4].Text = "Time To Repair";
                gv.HeaderRow.Cells[5].Text = "Dowtime";

                if (gv != null)
                {
                    return new DownloadFileActionResult(gv, "FracasLog.xls");
                }
                else
                {
                    return new JavaScriptResult();
                }
            }
            else {
                return View("Index");
            }
        }
    }
}
