using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using StarEnergyServiceGridHelper;
using System.Web.Script.Serialization;
using System.Text;
using System.Security.Cryptography;

namespace StarEnergi.Controllers.FrontEnd
{
    public class DashboardController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        //
        // GET: /Dasboard/

        public ActionResult Index()
        {
            //Config.menu = Config.MenuFrontEnd.DASHBOARD;
            //ViewBag.nama = "Dashboard";
            //return View(db.plants);
            she_observation_undian undian = db.she_observation_undian.OrderByDescending(p => p.from).FirstOrDefault();
            WinnerReport winner = null;
            if (undian != null)
            {
                winner = new WinnerReport
                {
                    from = undian.from,
                    to = undian.to
                };
                List<she_observation_undian_winner> f = db.she_observation_undian_winner.Where(p => p.id_undian == undian.id).ToList();
                List<WinnerEntity> g = new List<WinnerEntity>();
                int count_q = 0;
                int count = 0;
                foreach (she_observation_undian_winner she in f)
                {
                    WinnerEntity w = new WinnerEntity
                    {
                        id = she.id,
                        winner = db.she_observation.Find(she.winner_observation).observer.Split('#')[0].ToString(),
                        category = she.category == 0 ? "Quality" : "All"
                    };
                    g.Add(w);
                    if (she.category == 0)
                        count_q++;
                    else
                        count++;
                }

                winner.winners = g;
            }
            ViewBag.winner = winner;
            return View("DashboardNew");
        }

        #region speedometer
        public ActionResult SpeedoMa()
        {
            List<ma> result = new List<ma>();

            foreach (foc f in db.focs)
            {
                if (f.mas.Where(a => a.category == 1).Count() > 0)
                {
                    result.Add(f.mas.Where(a => a.category == 1).OrderByDescending(a => a.last_update).First());
                }
            }
            return PartialView(result);
        }

        public ActionResult SpeedoPaf()
        {
            List<foc_op_avail> result = new List<foc_op_avail>();

            foreach(foc f in db.focs){
                if (f.foc_op_avail.Count > 0){
                    result.Add(f.foc_op_avail.Last());    
                }
            }
            
            return PartialView(result);
        }

        public ActionResult SpeedoPof()
        {
            List<foc_op_avail> result = new List<foc_op_avail>();

            foreach (foc f in db.focs)
            {
                if (f.foc_op_avail.Count > 0)
                {
                    result.Add(f.foc_op_avail.Last());
                }
            }
            return PartialView(result);
        }
        #endregion

        #region sidebar

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetMaUnit(int idUnit) {
            string[] result = new string[2];

            result[0] = string.Format("{0:0.00}", db.units.Find(idUnit).ma);
            result[1] = string.Format("{0:0.00}", db.units.Find(idUnit).masd);
            
            return Json(result);    
        }

        public ActionResult SideBarMa(int id) {
            ViewBag.idUnit = new SelectList(db.units.Where(a => a.id_foc == id).ToList(), "id", "nama");

            int maxYear = db.mas.Where(a => a.id_foc == id).OrderByDescending(a => a.last_update).ToList().First().last_update.Year;
            int minYear = db.mas.Where(a => a.id_foc == id).OrderBy(a => a.last_update).ToList().First().last_update.Year;

            List<YearFilterForm> listAvailYear = new List<YearFilterForm>();
            for (int i = minYear; i <= maxYear; i++)
            {
                YearFilterForm year = new YearFilterForm();
                year.year = i;
                listAvailYear.Add(year);
            }


            List<MonthFilterForm> listMonth = new List<MonthFilterForm>();
            int iterator = 1;
            foreach (string s in Config.Month)
            {
                MonthFilterForm month = new MonthFilterForm();
                month.id = iterator;
                month.name = Config.Month[iterator-1];
                listMonth.Add(month);
                iterator++;
            }

            ViewBag.fromYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.toYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.year = new SelectList(listAvailYear, "year", "year");

            ViewBag.fromMonth = new SelectList(listMonth, "id", "name");
            ViewBag.toMonth = new SelectList(listMonth, "id", "name");
            ViewBag.id = id;
            return PartialView();
        }

        public ActionResult SideBarPaf(int id)
        {
            int maxYear = (int)db.foc_op_avail.Where(a => a.id_foc == id).OrderByDescending(a => a.tahun).ToList().First().tahun;
            int minYear = (int)db.foc_op_avail.Where(a => a.id_foc == id).OrderBy(a => a.tahun).ToList().First().tahun;

            List<YearFilterForm> listAvailYear = new List<YearFilterForm>();
            for (int i = minYear; i <= maxYear; i++)
            {
                YearFilterForm year = new YearFilterForm();
                year.year = i;
                listAvailYear.Add(year);
            }


            List<MonthFilterForm> listMonth = new List<MonthFilterForm>();
            int iterator = 1;
            foreach (string s in Config.Month)
            {
                MonthFilterForm month = new MonthFilterForm();
                month.id = iterator;
                month.name = Config.Month[iterator - 1];
                listMonth.Add(month);
                iterator++;
            }

            ViewBag.fromYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.toYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.year = new SelectList(listAvailYear, "year", "year");

            ViewBag.fromMonth = new SelectList(listMonth, "id", "name");
            ViewBag.toMonth = new SelectList(listMonth, "id", "name");
            ViewBag.id = id;
            return PartialView();
        }

        public ActionResult SideBarPof(int id)
        {
            int maxYear = (int)db.foc_op_avail.Where(a => a.id_foc == id).OrderByDescending(a => a.tahun).ToList().First().tahun;
            int minYear = (int)db.foc_op_avail.Where(a => a.id_foc == id).OrderBy(a => a.tahun).ToList().First().tahun;

            List<YearFilterForm> listAvailYear = new List<YearFilterForm>();
            for (int i = minYear; i <= maxYear; i++)
            {
                YearFilterForm year = new YearFilterForm();
                year.year = i;
                listAvailYear.Add(year);
            }


            List<MonthFilterForm> listMonth = new List<MonthFilterForm>();
            int iterator = 1;
            foreach (string s in Config.Month)
            {
                MonthFilterForm month = new MonthFilterForm();
                month.id = iterator;
                month.name = Config.Month[iterator - 1];
                listMonth.Add(month);
                iterator++;
            }

            ViewBag.fromYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.toYear = new SelectList(listAvailYear, "year", "year");
            ViewBag.year = new SelectList(listAvailYear, "year", "year");

            ViewBag.fromMonth = new SelectList(listMonth, "id", "name");
            ViewBag.toMonth = new SelectList(listMonth, "id", "name");
            ViewBag.id = id;
            return PartialView();
        }

        #endregion

        #region chart
        public ActionResult ChartSingle() {
            return PartialView();
        }

        public ActionResult ChartDouble()
        {
            return PartialView();
        }

        public ActionResult _SelectAjaxBinding()
        {
            NameValueCollection criteria = Request.Form;
            if (criteria.Count > 0)
            {
                if (criteria["filter"] == "year")
                {
                    return bindingYear(criteria);
                }
                else {
                    return bindingMonth(criteria);
                }
            }
            else {
                return Json(false);
            }
            
        }

        public ActionResult _SelectAjaxBindingDouble()
        {
            NameValueCollection criteria = Request.Form;
            if (criteria.Count > 0)
            {
                if (criteria["filter"] == "year")
                {
                    return bindingYearDouble(criteria);
                }
                else
                {
                    return bindingMonthDouble(criteria);
                }
            }
            else
            {
                return Json(false);
            }

        }

        private ActionResult bindingMonth(NameValueCollection criteria)
        {
            int id = int.Parse(criteria["id_area"]);
            int year = int.Parse(criteria["year"]);
            int fromMonth = int.Parse(criteria["from"]);
            int toMonth = int.Parse(criteria["to"]);
            string type = criteria["type"];

            List<DashboardChartEntity> temp = new List<DashboardChartEntity>();
            if (type == "ma")
            {
                for (int i = fromMonth; i <= toMonth; i++)
                {
                    var model = from o in db.mas
                                where (o.id_foc == id) && (o.last_update.Year == year) && (o.category == 0) && (o.last_update.Month == i) 
                                orderby (o.last_update) descending
                                select new DashboardChartEntity
                                {
                                    year = o.last_update.Month,
                                    value = o.ma1
                                };

                    if (model.ToList().Count > 0)
                    {
                        temp.Add(model.First());
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0
                        };
                        temp.Add(dummy);
                    }
                }
                return Json(temp);
            }
            else if (type == "pof")
            {

                for (int i = fromMonth; i <= toMonth; i++)
                {
                    var model = from o in db.foc_op_avail
                                where (o.id_foc == id) && (o.tahun == year) && (o.bulan == i)
                                select new DashboardChartEntity
                                {
                                    year = (int)o.bulan,
                                    value = o.pof_bulanan
                                };
                    if (model.ToList().Count > 0)
                    {
                        temp.Add(model.First());
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0
                        };
                        temp.Add(dummy);
                    }

                }
                return Json(temp);
            }
            else
            {
                return Json(false);
            } 
        }

        private JsonResult bindingYear(NameValueCollection criteria)
        {
            int id = int.Parse(criteria["id_area"]);
            int fromYear = int.Parse(criteria["from"]);
            int toYear = int.Parse(criteria["to"]);
            string type = criteria["type"];

            List<DashboardChartEntity> temp = new List<DashboardChartEntity>();
            if (type == "ma")
            {
                for (int i = fromYear; i <= toYear; i++)
                {
                    var model = from o in db.mas
                                where (o.id_foc == id) && (o.last_update.Year == i) && (o.category == 1)
                                orderby (o.last_update) descending
                                select new DashboardChartEntity
                                {
                                    year = o.last_update.Year,
                                    value = o.ma1
                                };
                    if (model.ToList().Count > 0)
                    {
                        temp.Add(model.First());
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0
                        };
                        temp.Add(dummy);
                    }
                }
                return Json(temp);
            }
            else if (type == "pof")
            {
                for (int i = fromYear; i <= toYear; i++)
                {
                    var model = from o in db.foc_op_avail
                                where (o.id_foc == id) && (o.tahun == i)
                                orderby (o.bulan) descending
                                select new DashboardChartEntity
                                {
                                    year = (int)o.tahun,
                                    value = o.op_avail
                                };
                    if (model.ToList().Count > 0)
                    {
                        temp.Add(model.First());
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0
                        };
                        temp.Add(dummy);
                    }

                }
                return Json(temp);
            }
            else
            {
                return Json(false);
            }          
        }

        private ActionResult bindingMonthDouble(NameValueCollection criteria)
        {
            int id = int.Parse(criteria["id_area"]);
            int year = int.Parse(criteria["year"]);
            int fromMonth = int.Parse(criteria["from"]);
            int toMonth = int.Parse(criteria["to"]);
            string type = criteria["type"];

            List<DashboardChartEntity> temp = new List<DashboardChartEntity>();
            if (type == "paf")
            {
                for (int i = fromMonth; i <= toMonth; i++)
                {
                    var model = from o in db.foc_op_avail
                                where (o.id_foc == id) && (o.tahun == year) && (o.bulan == i)
                                select new DashboardChartEntity
                                {
                                    year = (int)o.bulan,
                                    value = o.paf_bulanan
                                };
                    List<DashboardChartEntity> result = model.ToList();
                    if (result.Count > 0)
                    {
                        DashboardChartEntity add = result.First();
                        List<foc_target_paf> c = db.foc_target_paf.Where(a => a.tahun == year).Where(a => a.bulan == add.year).ToList();
                        if (c.Count > 0)
                        {
                            add.target = db.foc_target_paf.Where(a => a.tahun == year).Where(a => a.bulan == add.year).First().target_paf;
                        }
                        else
                        {
                            add.target = null;
                        }

                        temp.Add(add);
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0,
                            target = null
                        };
                        temp.Add(dummy);
                    }

                }
                return Json(temp);
            }
            else
            {
                return Json(false);
            }
        }

        private JsonResult bindingYearDouble(NameValueCollection criteria)
        {
            int id = int.Parse(criteria["id_area"]);
            int fromYear = int.Parse(criteria["from"]);
            int toYear = int.Parse(criteria["to"]);
            string type = criteria["type"];

            List<DashboardChartEntity> temp = new List<DashboardChartEntity>();
            if (type == "paf")
            {
                for (int i = fromYear; i <= toYear; i++)
                {
                    var model = from o in db.foc_op_avail
                                where (o.id_foc == id) && (o.tahun == i)
                                orderby (o.bulan) descending
                                select new DashboardChartEntity
                                {
                                    year = (int)o.tahun,
                                    value = o.paf
                                };
                    List<DashboardChartEntity> result = model.ToList();
                    if (result.Count > 0)
                    {
                        DashboardChartEntity add = result.First();
                        List<foc_target_paf> c = db.foc_target_paf.Where(a => a.tahun == add.year).ToList();
                        if (c.Count > 0)
                        {
                            add.target = db.foc_target_paf.Where(a => a.tahun == add.year).OrderByDescending(a => a.bulan).First().target_paf;
                        }
                        else {
                            add.target = null;
                        }

                        temp.Add(add);
                    }
                    else
                    {
                        DashboardChartEntity dummy = new DashboardChartEntity()
                        {
                            year = i,
                            value = 0,
                            target = null
                        };
                        temp.Add(dummy);
                    }
                }
                return Json(temp);
            }
            else
            {
                return Json(false);
            }
        }

        #endregion


        #region notificationGrid
        public string Binding()
        {
            GridRequestParameters param = GridRequestParameters.Current;

            WWService.UserServiceClient client = new WWService.UserServiceClient();
            string serviceRequestData = new JavaScriptSerializer().Serialize(param);
            int userId;
            if (HttpContext.Session["id"] != null)
            {
                userId = Int32.Parse(HttpContext.Session["id"].ToString());
            }
            else
            {
                userId = 0;
            }
            
            string serviceResponse = client.GetUserNotification(serviceRequestData, userId,EncodeMd5("starenergyww"));

            return serviceResponse;
            
        }

        public JsonResult NotificationIsSeen(int id)
        {
            WWService.UserServiceClient client = new WWService.UserServiceClient();


            int userId;
            if (HttpContext.Session["id"] != null)
            {
                userId = Int32.Parse(HttpContext.Session["id"].ToString());
            }
            else
            {
                userId = 0;
            }
            client.NotifactionIsSeen(EncodeMd5("starenergyww"), id, userId);

            return Json(true);
        }

        private string EncodeMd5(string originalText)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalText);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }

        #endregion

    }
}
