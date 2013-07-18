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
    public class HseIllnessReportController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /HseIllnessReport/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseIllnessReport" });
            }
            ViewBag.Nama = "SHE Illness Report";
            return View();
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addIllnessReport(int? id)
        {
            if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.she_illness_report.Find(id);
            }
            return PartialView();
        }

        //
        // Ajax select binding illness report
        [GridAction]
        public ActionResult _SelectAjaxIllnessReport()
        {
            return bindingIllnessReport();
        }

        //select data incident report
        private ViewResult bindingIllnessReport()
        {
            List<she_illness_report> f = new List<she_illness_report>();
            f = db.she_illness_report.ToList();

            return View(new GridModel<she_illness_report>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(she_illness_report illnessReport)
        {
            db.she_illness_report.Add(illnessReport);
            db.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(she_illness_report illnessReport)
        {
            she_illness_report ir = db.she_illness_report.Find(illnessReport.id);

            ir.chest_injury = illnessReport.chest_injury;
            ir.chest_tender_chest_wall = illnessReport.chest_tender_chest_wall;
            ir.chest_position_windpipe_neck = illnessReport.chest_position_windpipe_neck;
            ir.chest_movement_breathing = illnessReport.chest_movement_breathing;
            ir.chest_breathing_sound = illnessReport.chest_breathing_sound;
            ir.chest_air_can_hear = illnessReport.chest_air_can_hear;
            ir.abdo_size = illnessReport.abdo_size;
            ir.abdo_pain_coughing = illnessReport.abdo_pain_coughing;
            ir.abdo_pain_moving = illnessReport.abdo_pain_moving;
            ir.abdo_pain_puffing = illnessReport.abdo_pain_puffing;
            ir.abdo_area_tenderness_found = illnessReport.abdo_area_tenderness_found;
            ir.abdo_lumps_swelling_found = illnessReport.abdo_lumps_swelling_found;
            ir.abdo_bowel_sound = illnessReport.abdo_bowel_sound;
            ir.ge_gland_neck_found = illnessReport.ge_gland_neck_found;
            ir.ge_gland_neck_r = illnessReport.ge_gland_neck_r;
            ir.ge_gland_neck_l = illnessReport.ge_gland_neck_l;
            ir.ge_gland_armpit_found = illnessReport.ge_gland_armpit_found;
            ir.ge_gland_armpit_r = illnessReport.ge_gland_armpit_r;
            ir.ge_gland_armpit_l = illnessReport.ge_gland_armpit_l;
            ir.ge_gland_groin_found = illnessReport.ge_gland_groin_found;
            ir.ge_gland_groin_r = illnessReport.ge_gland_groin_r;
            ir.ge_gland_groin_l = illnessReport.ge_gland_groin_l;
            ir.ge_ears = illnessReport.ge_ears;
            ir.ge_ears_r = illnessReport.ge_ears_r;
            ir.ge_ears_l = illnessReport.ge_ears_l;
            ir.ge_colour = illnessReport.ge_colour;
            ir.ge_eardrum_normal = illnessReport.ge_eardrum_normal;
            ir.ge_eardrum_normal_r = illnessReport.ge_eardrum_normal_r;
            ir.ge_eardrum_normal_l = illnessReport.ge_eardrum_normal_l;
            ir.ge_eardrum_wax = illnessReport.ge_eardrum_wax;
            ir.ge_eardrum_wax_r = illnessReport.ge_eardrum_wax_r;
            ir.ge_eardrum_wax_l = illnessReport.ge_eardrum_wax_l;
            ir.ge_eardrum_red = illnessReport.ge_eardrum_red;
            ir.ge_eardrum_red_r = illnessReport.ge_eardrum_red_r;
            ir.ge_eardrum_red_l = illnessReport.ge_eardrum_red_l;
            ir.ge_eardrum_perforated = illnessReport.ge_eardrum_perforated;
            ir.ge_eardrum_perforated_r = illnessReport.ge_eardrum_perforated_r;
            ir.ge_eardrum_perforated_l = illnessReport.ge_eardrum_perforated_l;
            ir.ge_throat_colour = illnessReport.ge_throat_colour;
            ir.ge_tonsil_size = illnessReport.ge_tonsil_size;
            ir.ge_skin_rush = illnessReport.ge_skin_rush;
            ir.ge_skin_rush_size = illnessReport.ge_skin_rush_size;
            ir.ge_skin_rush_colour = illnessReport.ge_skin_rush_colour;
            ir.ge_skin_rush_surface = illnessReport.ge_skin_rush_surface;
            ir.inves_peak = illnessReport.inves_peak;
            ir.urine_testing_blood = illnessReport.urine_testing_blood;
            ir.urine_testing_glucose = illnessReport.urine_testing_glucose;
            ir.urine_testing_ph = illnessReport.urine_testing_ph;
            ir.urine_testing_protein = illnessReport.urine_testing_protein;
            ir.urine_testing_sg = illnessReport.urine_testing_sg;
            ir.urine_testing_other = illnessReport.urine_testing_other;
            ir.comment_finding = illnessReport.comment_finding;
            ir.comment_finding_no = illnessReport.comment_finding_no;
            ir.possible_diagnosis = illnessReport.possible_diagnosis;
            ir.diag_name = illnessReport.diag_name;
            ir.post_designation = illnessReport.post_designation;
            ir.treatment_date = illnessReport.treatment_date;

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        //
        // Ajax delete binding illness report
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxIllnessReport(int id)
        {
            deleteIllnessReport(id);
            return bindingIllnessReport();
        }

        //delete data illness report
        private void deleteIllnessReport(int id)
        {
            she_illness_report ir = db.she_illness_report.Find(id);
            db.she_illness_report.Remove(ir);
            db.SaveChanges();
        }

        public ActionResult DetailIllnessReport(int id)
        {
            she_illness_report details = new she_illness_report();
            details = db.she_illness_report.Find(id);

            ViewBag.nama = "Detail Illness Report " + details.patient_name;
            return PartialView(details);
        }

        public ActionResult printIllnessReport(int id)
        {
            she_illness_report dl = db.she_illness_report.Find(id);
            return this.ViewPdf("", "illnessReportPrint", dl);
        }

    }
}
