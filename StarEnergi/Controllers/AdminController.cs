using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;


namespace StarEnergi.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/admin" });
            }
            else {
                string username = (String)Session["username"].ToString();
                List<user_per_role> li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!(li.Exists(p => p.role == (int)Config.role.ADMIN)))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/admin" });
                }
                ViewData["user_role"] = li;
            }
            ViewBag.Message = "Welcome to Admin Reliability Monitoring!";
            ViewBag.stat = "admin";
            List<plant> plant = db.plants.ToList();
            plant = plant.OrderBy(a => a.nama).ToList();
            foreach (plant p in plant) {
                p.focs = p.focs.OrderBy(a => a.nama).ToList();
            }
            
            return View(plant);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
