using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using StarEnergi.Models;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class HseEventLogController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /HseEventLog/

        public ActionResult Index()
        {
            ViewBag.Nama = "SHE Event Log";
            return View();
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

            List<HseEventEntity> ret = new List<HseEventEntity>();
            var r = (from hse in db.incident_report
                       select new HseEventEntity { id = hse.id, dateTimeIncident = hse.date_incident, 
                           incidentType = hse.incident_type, incidentLocation = hse.incident_location, 
                           factualInformation = hse.factual_information, immediateAction = hse.immediate_action }).ToList();
            ret = r;
            return View(new GridModel<HseEventEntity>
            {
                Data = ret
            });
        }

        public ActionResult importExcelIndex()
        {
            List<HseEventEntity> ret = new List<HseEventEntity>();
            var r = (from hse in db.incident_report
                     select new HseEventEntity
                     {
                         id = hse.id,
                         dateTimeIncident = hse.date_incident,
                         incidentType = hse.incident_type,
                         incidentLocation = hse.incident_location,
                         factualInformation = hse.factual_information,
                         immediateAction = hse.immediate_action
                     }).ToList();
            ret = r;

            GridView gv = new GridView();
            gv.Caption = "SHE Event Log";
            gv.DataSource = ret;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "ID";
            gv.HeaderRow.Cells[1].Text = "Date/Time Incident";
            gv.HeaderRow.Cells[2].Text = "Incident Type";
            gv.HeaderRow.Cells[3].Text = "Incident Location";
            gv.HeaderRow.Cells[4].Text = "Factual Information";
            gv.HeaderRow.Cells[5].Text = "Immediate Actions";

            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "SHE Event Log.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

    }
}
