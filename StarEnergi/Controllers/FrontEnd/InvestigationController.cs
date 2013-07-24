using ReportManagement;
using StarEnergi.Controllers.Utilities;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class InvestigationController : PdfViewController
    {
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;
        //
        // GET: /Investigation/
        #region iir
        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Investigation" });
            }
            else
            {
                var has = (from employees in db.employees
                           join dept in db.employee_dept on employees.dept_id equals dept.id
                           join users in db.users on employees.id equals users.employee_id into user_employee
                           from ue in user_employee.DefaultIfEmpty()
                           select new EmployeeEntity
                           {
                               id = employees.id,
                               alpha_name = employees.alpha_name,
                               employee_no = employees.employee_no,
                               position = employees.position,
                               work_location = employees.work_location,
                               dob = employees.dob,
                               dept_name = dept.dept_name,
                               username = (ue.username == null ? String.Empty : ue.username)
                           }).ToList();
                ViewData["users"] = has;
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!li.Exists(p => p.role == (int)Config.role.IIR))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/Investigation" });
                }
                ViewData["user_role"] = li;
                return View();
            }
            
        }

        public ActionResult report()
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewData["users"] = has;
            ViewData["user_role"] = li;
            return PartialView();
        }

        //
        // Ajax select binding Investigation report
        [GridAction]
        public ActionResult _SelectAjaxInvestigationReport()
        {
            return bindingInvestigationReport();
        }

        //select data Investigation report
        private ViewResult bindingInvestigationReport()
        {
            List<investigation_report> f = new List<investigation_report>();
            f = db.investigation_report.ToList();

            return View(new GridModel<investigation_report>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        public ActionResult AddInvestigation(int? id_rca, int? id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate,
                           approval_level = employees.approval_level
                       }).ToList();
            ViewData["users"] = has;
            var pic = (from user_per_roles in db.user_per_role
                       join users in db.users on user_per_roles.username equals users.username
                       join employees in db.employees on users.employee_id equals employees.id
                       where user_per_roles.role == 4 || user_per_roles.role == 11
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           username = users.username,
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).Distinct().ToList();
            ViewData["pic"] = pic;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;

            investigation_report inc = db.investigation_report.OrderBy(p => p.reference_number).ToList().LastOrDefault();

            if (inc == null || inc.reference_number == null || inc.reference_number.Length != 21)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ViewBag.ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(inc.reference_number.Substring(17));
                refs++;
                int reference_year = Int32.Parse(inc.reference_number.Substring(12,4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ViewBag.ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else {
                    refs = 1;
                    ViewBag.ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }
            if (id_rca != null)
            {
                List<string> r = db.rca_implementation.Where(p => p.id_rca == id_rca).Select(p => p.next_action).ToList();
                string dat = "";
                foreach (string s in r)
                {
                    dat += s + "\r";
                }
                ViewBag.datas = dat;
                ViewBag.id_ir = id_rca;
            } else if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.investigation_report.Find(id);
            }
            return View();
        }

        [HttpPost]
        public JsonResult Add(investigation_report investigationReport, int? id_rca)
        {
            employee e;
            e = db.employees.Find(Int32.Parse(investigationReport.investigator));

            investigation_report inc = db.investigation_report.OrderBy(p => p.reference_number).ToList().LastOrDefault();
            string ir_ref = "";
            if (inc == null || inc.reference_number == null || inc.reference_number.Length != 21)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(inc.reference_number.Substring(17));
                refs++;
                int reference_year = Int32.Parse(inc.reference_number.Substring(12, 4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else
                {
                    refs = 1;
                    ir_ref = "W-O-SPE-IIR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }

            investigationReport.reference_number = ir_ref;
            investigationReport.investigator_approve = e.signature;
            db.investigation_report.Add(investigationReport);
            db.SaveChanges();
            
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (investigationReport.field_manager != null && investigationReport.field_manager_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(investigationReport.field_manager));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(investigationReport.field_manager_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (investigationReport.loss_control != null && investigationReport.loss_control_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(investigationReport.loss_control));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(investigationReport.loss_control_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Investigation Report dengan nomor referensi " + investigationReport.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Investigation Report with reference number " + investigationReport.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(investigationReport.investigator)).alpha_name, "Approving Incident Investigation Report " + investigationReport.reference_number);

            int id = db.investigation_report.Max(p => p.id);
            if (id_rca != null)
            {
                rca ir = db.rcas.Find(id_rca);
                ir.id_iir = id;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<iir_recommendations> li = db.iir_recommendations.Where(p => p.id_iir == null).ToList();
            foreach (iir_recommendations l in li)
            {
                l.id_iir = id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { ref_num = ir_ref });
        }

        [HttpPost]
        public JsonResult Edit(investigation_report investigationReport)
        {
            investigation_report iir = db.investigation_report.Find(investigationReport.id);

            iir.facility = investigationReport.facility;
            iir.incident_location = investigationReport.incident_location;
            iir.reference_number = investigationReport.reference_number;
            iir.incident_type = investigationReport.incident_type;
            iir.title = investigationReport.title;
            iir.date_incident = investigationReport.date_incident;
            iir.actual_loss = investigationReport.actual_loss;
            iir.potential_loss = investigationReport.potential_loss;
            iir.probability = investigationReport.probability;
            iir.factual_information = investigationReport.factual_information;
            iir.cost_estimate = investigationReport.cost_estimate;
            iir.immediate_action = investigationReport.immediate_action;
            iir.immediate_causes = investigationReport.immediate_causes;
            iir.basic_causes = investigationReport.basic_causes;
            iir.additional_observation = investigationReport.additional_observation;
            iir.investigator = investigationReport.investigator;
            iir.investigator_date = investigationReport.investigator_date;
            iir.loss_control = investigationReport.loss_control;
            iir.loss_control_date = investigationReport.loss_control_date;
            iir.field_manager = investigationReport.field_manager;
            iir.field_manager_date = investigationReport.field_manager_date;
            iir.loss_control_delegate = investigationReport.loss_control_delegate;
            iir.field_manager_delegate = investigationReport.field_manager_delegate;

            db.Entry(iir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }
        #endregion


        #region recommendation

        //
        // Ajax select binding recommendation
        [GridAction]
        public ActionResult _SelectAjaxRecommendation(int? id)
        {
            return bindingRecommendation(id);
        }

        //select data incident report
        private ViewResult bindingRecommendation(int? id)
        {
            List<iir_recommendations> f = new List<iir_recommendations>();
            Debug.WriteLine(id == null);
            if (id == null)
            {
                f = db.iir_recommendations.Where(p => p.id_iir == null).ToList();
                foreach (iir_recommendations iir in f)
                {
                    iir.PIC = db.employees.Find(Int32.Parse(iir.PIC)).alpha_name;
                }
            }
            else
            {
                f = db.iir_recommendations.Where(p => p.id_iir == id).ToList();
                foreach (iir_recommendations iir in f)
                {
                    iir.PIC = db.employees.Find(Int32.Parse(iir.PIC)).alpha_name;
                }
            }

            return View(new GridModel<iir_recommendations>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        //
        // Ajax delete binding incident report
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxRecommendation(int id, int? id_iir)
        {
            deleteRecommendation(id);
            return bindingRecommendation(id_iir);
        }

        //delete data fracas equipment
        private void deleteRecommendation(int id)
        {
            iir_recommendations recommend = db.iir_recommendations.Find(id);
            db.iir_recommendations.Remove(recommend);
            db.SaveChanges();
        }

        public JsonResult addRecommendation()
        {
            return Json(true);
        }

        [HttpPost]
        [GridAction]
        public JsonResult addRecommend(iir_recommendations ir_recommend, int addPIR)
        {
            //if (addPIR == 1)
            //{
            //    //generate pir_number first
            //    ir_recommend.pir_number = "12";
            //}
            ir_recommend.has_pir = (byte)addPIR;
            db.iir_recommendations.Add(ir_recommend);
            db.SaveChanges();
            return Json(true);
        }

        public JsonResult GetRecommends(int id)
        {
            iir_recommendations u = db.iir_recommendations.Find(id);
            return Json(u, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult editRecommend(iir_recommendations ir_recommend, int addPIR)
        {
            iir_recommendations ir = db.iir_recommendations.Find(ir_recommend.id);

            ir.description = ir_recommend.description;
            ir.PIC = ir_recommend.PIC;
            ir.completion_date = ir_recommend.completion_date;

            if (addPIR == 1)
            {
                ir.has_pir = 1;
            }
            else
            {
                ir.has_pir = null;
                ir.pir_number = null;
            }

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteAllRecommend()
        {
            List<iir_recommendations> li = db.iir_recommendations.Where(p => p.id_iir == null).ToList();

            foreach (iir_recommendations ir in li)
            {
                db.iir_recommendations.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }
        #endregion

        #region print
        public ActionResult printIIR(int id)
        {
            Debug.WriteLine("testing unit");
            return this.ViewPdf("", "InvestigationPrint", createIIRModel(id));
        }

        private PrintIIRModel createIIRModel(int id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            investigation_report iir = db.investigation_report.Find(id);
            PrintIIRModel piim = new PrintIIRModel
            {
                id = iir.id,
                facility = iir.facility,
                incident_location = iir.incident_location,
                reference_number = iir.reference_number,
                date_incident = iir.date_incident,
                title = iir.title,
                incident_type = iir.incident_type,
                actual_loss = iir.actual_loss,
                potential_loss = iir.potential_loss,
                probability = iir.probability,
                factual_information = iir.factual_information,
                immediate_action = iir.immediate_action,
                immediate_causes = iir.immediate_causes,
                basic_causes = iir.basic_causes,
                additional_observation = iir.additional_observation,
                cost_estimate = iir.cost_estimate,
                investigator_date =iir.investigator_date,
                loss_control_date = iir.loss_control_date,
                field_manager_date = iir.field_manager_date,
                investigator_approve = iir.investigator_approve,
                loss_control_approve = iir.loss_control_approve,
                field_manager_approve = iir.field_manager_approve
            };

            piim.iir_recommendations = db.iir_recommendations.Where(p => p.id_iir == id).ToList();
            foreach (iir_recommendations iirs in piim.iir_recommendations)
            {
                iirs.PIC = db.employees.Find(Int32.Parse(iirs.PIC == null ? "0" : iirs.PIC)).alpha_name;
            }
            if (iir.investigator != null)
                piim.investigator = has.Find(p => p.id == Int32.Parse(iir.investigator == null ? "0" : iir.investigator)).alpha_name;
            if (iir.loss_control != null && (iir.loss_control_approve == null || iir.loss_control_approve.Substring(0, 1) == "a"))
                piim.loss_control = has.Find(p => p.id == Int32.Parse(iir.loss_control == null ? "0" : iir.loss_control)).alpha_name;
            else
                piim.loss_control = has.Find(p => p.id == Int32.Parse(iir.loss_control_delegate == null ? "0" : iir.loss_control_delegate)).alpha_name;
            if (iir.field_manager != null && (iir.field_manager_approve == null || iir.field_manager_approve.Substring(0, 1) == "a"))
                piim.field_manager = has.Find(p => p.id == Int32.Parse(iir.field_manager == null ? "0" : iir.field_manager)).alpha_name;
            else
                piim.field_manager = has.Find(p => p.id == Int32.Parse(iir.field_manager_delegate == null ? "0" : iir.field_manager_delegate)).alpha_name;

            return piim;
        }
        #endregion

        #region approval rejection
        [HttpPost]
        public ActionResult approveLossControl(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                investigation_report iir = db.investigation_report.Find(id);
                if (iir.loss_control == employee_id.ToString())
                {
                    iir.loss_control_approve = "a" + sign;
                }
                else
                {
                    iir.loss_control_approve = "d" + sign;
                }
                db.Entry(iir).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveInvestigator(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                investigation_report iir = db.investigation_report.Find(id);
                iir.investigator_approve = sign;
                db.Entry(iir).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        public ActionResult approveFieldManager(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                investigation_report iir = db.investigation_report.Find(id);
                if (iir.field_manager == employee_id.ToString())
                {
                    iir.field_manager_approve = "a" + sign;
                }
                else
                {
                    iir.field_manager_approve = "d" + sign;
                }
                db.Entry(iir).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult rejectLossControl(int id, string comment)
        {
            investigation_report investigationReport = db.investigation_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (investigationReport.investigator != null)
            {
                employee e = db.employees.Find(Int32.Parse(investigationReport.investigator));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Investigation Report dengan Reference Number " + investigationReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Safety Supervisor.\n\nTerima Kasih.", "Rejected Incident Investigation Report " + investigationReport.reference_number);
            
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult rejectFieldManager(int id, string comment)
        {
            investigation_report investigationReport = db.investigation_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (investigationReport.investigator != null)
            {
                employee e = db.employees.Find(Int32.Parse(investigationReport.investigator));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Investigation Report dengan Reference Number " + investigationReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Field Manager.\n\nTerima Kasih.", "Rejected Incident Investigation Report " + investigationReport.reference_number);
            
            return Json(new { success = true });

        }
        #endregion
    }
}
