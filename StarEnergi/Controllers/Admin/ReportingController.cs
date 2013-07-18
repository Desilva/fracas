using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class ReportingController : Controller
    {
        //
        // GET: /Reporting/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentReport()
        {
            return PartialView();
        }

        public ActionResult PartReport()
        {
            return PartialView();
        }

        public ActionResult AvailibilityReport()
        {
            return PartialView();
        }

    }
}
