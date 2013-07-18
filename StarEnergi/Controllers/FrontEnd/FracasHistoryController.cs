using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using StarEnergi.Models.ExportExcel;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class FracasHistoryController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /FracasHistory/

        public ActionResult Index()
        {
            ViewBag.message = "Fracas History";
            Config.menu = Config.MenuFrontEnd.FRACAS;
            return View(db.plants);
        }

        public ActionResult DetailsEquipment(int id) {
            ViewBag.nama = db.equipments.Find(id).tag_num;
            ViewBag.id = id;
            return PartialView();
        }

        //
        //Select ajax binding
        [GridAction]
        public ActionResult _SelectAjaxEditingEquipment(int id)
        {
            return bindingEquipment(id);
        }

        //select equipment
        private ViewResult bindingEquipment(int id)
        {
            var model = from o in db.equipment_event
                        where o.id_equipment == id && o.status == 1 && o.datetime_stop != null
                        select new FracasEventModel
                        {
                            id = o.id,
                            dateTimeStart = o.datetime_ops,
                            dateTimeStop = o.datetime_stop,
                            eventDesc = o.event_description,
                            cause = o.failure_cause,
                            part = 0,
                            relatedItem = "-"
                        };
            List<FracasEventModel> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 1 && o.equipment_part.id_equipment == id
                            select new FracasEventModel
                            {
                                id = o.id,
                                dateTimeStart = o.datetime_ops,
                                dateTimeStop = o.datetime_stop,
                                eventDesc = o.event_description,
                                cause = o.failure_cause,
                                part = 1,
                                relatedItem = o.equipment_part.part.tag_number
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

        //export excel
        public ActionResult importExcelEquipment(int id)
        {
            string equipment = db.equipments.Find(id).tag_num;
            var model = from o in db.equipment_event
                        where o.id_equipment == id && o.status == 1 && o.datetime_stop != null
                        select new FracasExport
                        {
                            dateTimeStop = o.datetime_stop,
                            eventDesc = o.event_description,
                            cause = o.failure_cause,
                            relatedItem = "-"
                        };
            List<FracasExport> result = model.ToList();

            var modelPart = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 1 && o.equipment_part.id_equipment == id
                            select new FracasExport
                            {
                                dateTimeStop = o.datetime_stop,
                                eventDesc = o.event_description,
                                cause = o.failure_cause,
                                relatedItem = o.equipment_part.part.tag_number
                            };

            foreach (var item in modelPart)
            {
                result.Add(item);
            }
            if (result.Count > 0)
            {
                GridView gv = new GridView();
                gv.Caption = "Fracas equipment " + equipment;
                gv.DataSource = result;
                gv.DataBind();
                gv.HeaderRow.Cells[0].Text = "Date";
                gv.HeaderRow.Cells[1].Text = "Event";
                gv.HeaderRow.Cells[2].Text = "Failure Cause";
                gv.HeaderRow.Cells[3].Text = "Related Item";

                if (gv != null)
                {
                    return new DownloadFileActionResult(gv, "FracasHistory.xls");
                }
                else
                {
                    return new JavaScriptResult();
                }
            }
            else {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
        }

    }
}
