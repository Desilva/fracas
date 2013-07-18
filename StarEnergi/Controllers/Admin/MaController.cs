using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class MaController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /Ma/

        public ActionResult Index()
        {
            //var mas = db.mas.Include(m => m.foc);
            //return PartialView(mas.ToList());
            ViewBag.id_area = new SelectList(db.focs, "id", "nama",db.focs.First().id);
            return PartialView();
        }

        #region deprecated

        public JsonResult GetDetail(int id)
        {
            var model = from o in db.mas
                        where o.category == 0
                        select o;
            List<MaEntity> ma = new List<MaEntity>();
            foreach (var item in model)
            {
                MaEntity temp = new MaEntity();
                temp.id = item.id;
                temp.foc_name = item.foc.nama;
                temp.id_foc = item.id_foc;
                temp.days_in_study_year = item.days_in_study_year;
                temp.ntamdd = item.ntamdd;
                temp.ntamsd = item.ntamsd;
                temp.prior_year_ms = item.prior_year_ms;
                temp.prior_year_ntamd = item.prior_year_ntamd;
                temp.study_year_ms = item.study_year_ms;
                temp.study_year_ntamd = item.study_year_ntamd;
                temp.ta_interval = item.ta_interval;
                temp.tadd = item.tadd;
                temp.tadt_hours = item.tadt_hours;
                temp.ma = item.ma1;
                temp.masd = item.masd;
                temp.last_update = item.last_update;

                if (item.type == 0)
                {
                    temp.type = "Primary";
                }
                else
                {
                    temp.type = "Secondary";
                }
                ma.Add(temp);
            }

            return Json(new { ma = ma });
        }

        //
        // GET: /Ma/AddMa
        public JsonResult addMa()
        {
            return Json(new {foc = new SelectList(db.focs,"id","nama")});
        } 

        //
        // POST: /Ma/Create

        [HttpPost]
        public ActionResult Create(ma ma)
        {
            if (ModelState.IsValid)
            {
                ma.last_update = DateTime.Now;
                ma.category = 0;
                db.mas.Add(ma);
                db.SaveChanges();

                DateTime start = new DateTime(DateTime.Now.Year,1,1);
                IEnumerable<ma> data = db.mas.Where(a => a.last_update >= start).Where(a => a.last_update <= DateTime.Now).Where(a => a.category != 1).Where(a => a.id_foc == ma.id_foc);
                ma result = calculateMaYear(data, ma.type);
                db.mas.Add(result);
                db.SaveChanges();

                return RedirectToAction("Index");  
            }

            ViewBag.id_foc = new SelectList(db.focs, "id", "nama", ma.id_foc);
            return View(ma);
        }
        
        private ma calculateMaYear(IEnumerable<ma> data, byte? type){
            ma result = new ma();
            double tadthours=0, tainterval = 0, studyyearntamdd = 
                0, prioryearntamdd = 0, daysinstudyyear = 0,
                studyyearms = 0, prioryearms = 0, tadd=0,
                ntamdd = 0, ntamsd = 0, temp = 0, ma = 0, masd = 0;
         
            foreach (ma item in data) { 
                tadthours += item.tadt_hours;
				tainterval += item.ta_interval;
				studyyearntamdd += item.study_year_ntamd;
				prioryearntamdd += item.prior_year_ntamd;
				daysinstudyyear += item.days_in_study_year;
				studyyearms += item.study_year_ms;
                prioryearms += item.prior_year_ms;
            }

            if((tadthours == 0)||(tainterval==0)){
				tadd = 0;
			}else{
				tadd = tadthours/(tainterval*24);
			}
			
			if((studyyearntamdd == 0)||(prioryearntamdd==0)){
				ntamdd = 0;
			}else{
				ntamdd = (studyyearntamdd+prioryearntamdd)/(2*24);
			}
			
			if((studyyearms == 0)||(prioryearms==0)){
				ntamsd = 0;
			}else{
				ntamsd = (studyyearms+prioryearms)/(2*24);
			}

            temp = tadd + ntamdd;
            ma = (1-(temp/daysinstudyyear))*100;
            temp += ntamsd;
            masd = (1-(temp/daysinstudyyear))*100;
 
            result.category = 1;
            result.days_in_study_year = daysinstudyyear;
            result.id_foc = data.First().id_foc;
            result.last_update = DateTime.Now;
            result.ma1 = ma;
            result.masd = masd;
            result.ntamdd = ntamdd;
            result.ntamsd = ntamsd;
            result.prior_year_ms = prioryearms;
            result.prior_year_ntamd = prioryearntamdd;
            result.study_year_ms = studyyearms;
            result.study_year_ntamd = studyyearntamdd;
            result.ta_interval = tainterval;
            result.tadd = tadd;
            result.tadt_hours = tadthours;
            result.type = type;

            return result;
        }

#endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //select data equipment read nav
        private ViewResult binding()
        {
            var model = from o in db.mas
                        where o.category == 0
                        select o;
            List<MaEntity> ma = new List<MaEntity>();
            foreach (var item in model)
            {
                MaEntity temp = new MaEntity();
                temp.id = item.id;
                temp.foc_name = item.foc.nama;
                temp.ma = item.ma1;
                temp.masd = item.masd;
                temp.last_update = item.last_update;

                if (item.type == 0)
                {
                    temp.type = "Primary";
                }
                else
                {
                    temp.type = "Secondary";
                }
                ma.Add(temp);
            }

            return View(new GridModel<MaEntity>
            {
                Data = ma
            });
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            delete(id);
            return binding();
        }

        //delete data ma
        private void delete(int id)
        {
            ma ma = db.mas.Find(id);
            db.mas.Remove(ma);
            db.SaveChanges();
        }

        //set type ma
        private string type(int id) {
            if(id == 0){
                return "Primary";
            }else{
                return "Secondary";
            }
        }

        
    }
}