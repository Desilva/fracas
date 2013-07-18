using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Data;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class MaUnitController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /MaUnit/
        public ActionResult Index()
        {
            ViewBag.id_area = new SelectList(db.focs, "id", "nama");
            return PartialView();
        }

        public JsonResult GetUnit(int id_area)
        {
            var model = from o in db.units
                        where o.id_foc == id_area
                        select new UnitEntity
                        {
                            id = o.id,
                            nama = o.nama
                        };
            return Json(model.ToList());
        }

        public void UpdateMaUnit(int id,double ma, double masd)
        {
            unit unit = db.units.Find(id);
            unit.ma = ma;
            unit.masd = masd;
            db.Entry(unit).State = EntityState.Modified;
            db.SaveChanges();
        }

    }
}
