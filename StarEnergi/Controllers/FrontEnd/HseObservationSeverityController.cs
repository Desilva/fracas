using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class HseObservationSeverityController : Controller
    {
        //
        // GET: /HseObservationSeverity/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("Index", "FracasEventLog");
            }
            return View();
        }

    }
}
