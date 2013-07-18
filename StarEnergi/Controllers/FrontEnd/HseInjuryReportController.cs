using ReportManagement;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class HseInjuryReportController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /HseInjuryReport/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseInjuryReport" });
            }
            ViewBag.Nama = "SHE Injury Report";
            return View();
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addInjuryReport(int? id)
        {
            if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.she_injury_report.Find(id);
            }
            return PartialView();
        }

        //
        // Ajax select binding illness report
        [GridAction]
        public ActionResult _SelectAjaxInjuryReport()
        {
            return bindingInjuryReport();
        }

        //select data incident report
        private ViewResult bindingInjuryReport()
        {
            List<she_injury_report> f = new List<she_injury_report>();
            f = db.she_injury_report.ToList();

            return View(new GridModel<she_injury_report>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(she_injury_report injuryReport, string time_start_bef_acc, string time_survey, string time_trauma)
        {
            injuryReport.time_start_bef_acc = DateTime.Parse(time_start_bef_acc).TimeOfDay;
            injuryReport.time_survey = DateTime.Parse(time_survey).TimeOfDay;
            injuryReport.time_trauma = DateTime.Parse(time_trauma).TimeOfDay;
            db.she_injury_report.Add(injuryReport);
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(she_injury_report injuryReport, string time_start_bef_acc, string time_survey, string time_trauma)
        {
            injuryReport.time_start_bef_acc = DateTime.Parse(time_start_bef_acc).TimeOfDay;
            injuryReport.time_survey = DateTime.Parse(time_survey).TimeOfDay;
            injuryReport.time_trauma = DateTime.Parse(time_trauma).TimeOfDay;
            db.Entry(injuryReport).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        public ActionResult DetailInjuryReport(int id)
        {
            she_injury_report details = new she_injury_report();
            details = db.she_injury_report.Find(id);

            ViewBag.nama = "Detail Injury Report ";
            return PartialView(details);
        }

        public ActionResult printInjuryReport(int id)
        {
            she_injury_report dl = db.she_injury_report.Find(id);
            return this.ViewPdf("", "injuryReportPrint", dl);
        }

    }
}
