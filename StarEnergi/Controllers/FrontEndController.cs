using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using StarEnergi.Utilities.Statistical_Engine;

namespace StarEnergi.Controllers
{
    public class FrontEndController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /FrontEnd/

        public ActionResult Index()
        {         
            ViewBag.Message = "Welcome to Admin Reliability Monitoring!";
            //CalculateEventData ced = new CalculateEventData(185);
            return View(db.plants);
        }

    }
}
