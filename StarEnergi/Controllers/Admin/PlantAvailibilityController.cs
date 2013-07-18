using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Collections.Specialized;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class PlantAvailibilityController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private Dictionary<int, string> months = new Dictionary<int, string>();

        //
        // GET: /PlantAvailibility/
        public ActionResult Index()
        {
            ViewBag.edit_id_area = new SelectList(db.focs, "id", "nama");
            ViewBag.target_id_area = new SelectList(db.focs, "id", "nama");
            return PartialView();
        }


        #region add detail delete

        //
        // GET: /Paf/Create

        public JsonResult addPaf()
        {
            return Json(new { foc = new SelectList(db.focs, "id", "nama")});
        }

        public JsonResult addTarget()
        {
            return Json(new { foc = new SelectList(db.focs, "id", "nama") });
        }

        public JsonResult AddPafPost()
        {
            double tempSelisih, tempSelisihPaf, downtimePlaned, downtimeunPlaned, downtime, totalTime, newSelisih, totalTimeBulan
                  ,avail, availBulanan, newSelisihPaf, paf, pafBulanan;
            DateTime din = new DateTime(DateTime.Now.Year,1,1); 
            NameValueCollection posted = Request.Form;
            int id_area = int.Parse(posted["id_area"]);
            int bulan = int.Parse(posted["bulan"]);
            downtimePlaned = double.Parse(posted["planned"]);
		    downtimeunPlaned = double.Parse(posted["uplanned"]);
		    downtime = downtimePlaned + downtimeunPlaned;

            if (db.foc_op_avail.Where(a => a.id_foc == id_area).Count() > 0)
            {
                foc_op_avail focopavail = db.foc_op_avail.Where(a => a.id_foc == id_area).OrderByDescending(a => a.id).First();
                if ((focopavail.tahun == DateTime.Now.Year) && (focopavail != null))
                {
                    tempSelisih = double.Parse(focopavail.selisih.ToString());
                    tempSelisihPaf = double.Parse(focopavail.selisih_paf.ToString());
                }
                else {
                    tempSelisih = 0;
                    tempSelisihPaf = 0;
                }
            }
            else {
                tempSelisih = 0;
                tempSelisihPaf = 0;
            }

            newSelisih = downtime + tempSelisih;
            totalTime = DateTime.Now.Subtract(din).TotalHours;
            totalTimeBulan = Config.getDaysOfTheMonth(bulan) * 24;
            
            //pof
            avail = ((totalTime-newSelisih)/totalTime)*100;
            availBulanan = ((totalTimeBulan - downtime) / totalTimeBulan) * 100;

            //paf
            newSelisihPaf = tempSelisihPaf + downtimeunPlaned;
            paf = ((totalTime - newSelisihPaf)/totalTime)*100;
            pafBulanan = (((totalTimeBulan - downtimePlaned) - downtimeunPlaned) / (totalTimeBulan - downtimePlaned)) * 100;

            foc_op_avail insert = new foc_op_avail();
            insert.id_foc = id_area;
            insert.op_avail = avail;
            insert.pof_bulanan = availBulanan;
            insert.paf = paf;
            insert.paf_bulanan = pafBulanan;
            insert.selisih = int.Parse(newSelisih.ToString());
            insert.selisih_paf = int.Parse(newSelisihPaf.ToString());
            insert.bulan = bulan;
            insert.tahun = DateTime.Now.Year;

            db.foc_op_avail.Add(insert);
            db.SaveChanges();
            return Json(true);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddTargetPost(foc_target_paf target)
        {
            if (ModelState.IsValid)
            {
                db.foc_target_paf.Add(target);
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        } 

        //
        // GET: /PlantAvailibility/Delete/5
 
        public ActionResult Delete(int id)
        {
            foc_op_avail foc_op_avail = db.foc_op_avail.Find(id);
            return View(foc_op_avail);
        }

        //
        // POST: /PlantAvailibility/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            foc_op_avail foc_op_avail = db.foc_op_avail.Find(id);
            db.foc_op_avail.Remove(foc_op_avail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        #endregion

        
        #region table paf

        //blm dipakai
        public JsonResult GetDetail(int id)
        {
            var model = from o in db.foc_op_avail
                        where o.id == id
                        select new PlantAvailibilityEntity { 
                            foc_name = o.foc.nama,
                            tahun = o.tahun,
                            bulan = o.bulan,

                        };

            return Json(new { areaAvail = model.ToList() });
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
            var model = from o in db.foc_op_avail
                        select new PlantAvailibilityEntity{
                            id = o.id,
                            bulan = o.bulan,
                            foc_name = o.foc.nama,
                            tahun = o.tahun,
                            op_avail = o.op_avail,
                            pof_bulanan = o.pof_bulanan,
                            paf = o.paf,
                            paf_bulanan = o.paf_bulanan,
                            target = db.foc_target_paf.Where(a => a.id_foc == o.id_foc).Where(a => a.tahun == o.tahun).Where(a => a.bulan == o.bulan).Select(a => a.target_paf).FirstOrDefault()
                        };

            return View(new GridModel<PlantAvailibilityEntity>
            {
                Data = model.OrderByDescending(a => a.tahun).ToList()
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

        //delete data foc
        private void delete(int id)
        {
            foc_op_avail paf = db.foc_op_avail.Find(id);
            db.foc_op_avail.Remove(paf);
            db.SaveChanges();
        }
        
        public JsonResult GetMonth(int id_area) {
            setMonts();
            List<int> month = db.foc_op_avail.Where(a => (int)a.tahun == DateTime.Now.Year).Where(a => a.id_foc == id_area).Select(a => (int)a.bulan).ToList();
            Dictionary<int, string> newMonths = new Dictionary<int, string>();
            if (month.Count > 0)
            {
                foreach(int i in months.Keys){
                    if(!month.Contains(i)){
                        newMonths.Add(i,months[i]);
                    }
                }
                return Json(newMonths.ToArray());
            }
            else {
                return Json(months.ToArray());
            }
        }

        private void setMonts() {
            months.Add(1,"Januari");
            months.Add(2,"Februari");
            months.Add(3,"Maret");
            months.Add(4,"April");
            months.Add(5,"Mei");
            months.Add(6,"Juni");
            months.Add(7,"Juli");
            months.Add(8,"Agustus");
            months.Add(9,"Sepetember");
            months.Add(10,"Oktober");
            months.Add(11,"November");
            months.Add(12,"Desember");
        }

        public JsonResult GetMonthTarget(int id_area)
        {
            setMonts();
            List<int> month = db.foc_target_paf.Where(a => (int)a.tahun == DateTime.Now.Year).Where(a => a.id_foc == id_area).Select(a => (int)a.bulan).ToList();
            Dictionary<int, string> newMonths = new Dictionary<int, string>();
            if (month.Count > 0)
            {
                foreach (int i in months.Keys)
                {
                    if (!month.Contains(i))
                    {
                        newMonths.Add(i, months[i]);
                    }
                }
                return Json(newMonths.ToArray());
            }
            else
            {
                return Json(months.ToArray());
            }
        }
        #endregion

        #region table target
        //index target
        public ActionResult Targets()
        {
            List<TargetPafEntity> temp = new List<TargetPafEntity>();
            return PartialView(temp);
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectTargetAjaxEditing()
        {
            return bindingTarget();
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteTargetAjaxEditing(int id)
        {
            deleteTarget(id);
            return bindingTarget();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _EditTargetAjaxEditing(int id)
        {

            foc_target_paf editable = db.foc_target_paf.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingTarget();
        }

        //update data failure mode
        private void update(foc_target_paf targetPaf)
        {
            db.Entry(targetPaf).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data foc
        private void deleteTarget(int id)
        {
            foc_target_paf targetPaf = db.foc_target_paf.Find(id);
            db.foc_target_paf.Remove(targetPaf);
            db.SaveChanges();
        }

        //select data equipment read nav
        private ViewResult bindingTarget()
        {
            var targetPaf = from o in db.foc_target_paf
                            select new TargetPafEntity
                            {
                                id = o.id,
                                id_foc = o.id_foc,
                                target_paf = o.target_paf,
                                bulan = o.bulan,
                                tahun = o.tahun,
                                foc_name = o.foc.nama
                            };
            return View(new GridModel<TargetPafEntity>
            {
                Data = targetPaf.OrderByDescending(a => a.tahun).ToList()
            });
        }
        #endregion
    }
}