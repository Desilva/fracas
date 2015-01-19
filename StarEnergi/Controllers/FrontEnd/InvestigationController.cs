using ReportManagement;
using StarEnergi.Controllers.Utilities;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

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

        public ActionResult investigationLog(int? id)
        {
            List<investigation_report_log> ir_log = db.investigation_report_log.Where(p => p.id_iir == id).ToList();
            foreach (investigation_report_log i in ir_log)
            {
                int? employee_id = db.users.Find(i.username).employee_id;
                employee e = db.employees.Find(employee_id);
                i.username = e.alpha_name;
            }
            ViewData["log"] = ir_log;
            ViewBag.ref_number = db.investigation_report.Find(id).reference_number;
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
                Data = f.OrderByDescending(p => p.reference_number)
            });
        }

        public ActionResult AddInvestigation(int? id_rca, int? id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       orderby employees.alpha_name
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
                //List<string> r = db.rca_implementation.Where(p => p.id_rca == id_rca).Select(p => p.next_action).ToList();
                incident_report ir = db.incident_report.Where(p => p.id_rca == id_rca).ToList().FirstOrDefault();
                rca rca = db.rcas.Find(id_rca);
                ViewBag.ir = ir;
                string dat = "";
                //foreach (string s in r)
                //{
                //    //dat += s + "\r";
                //}
                ViewBag.datas = dat;
                ViewBag.id_ir = id_rca;
                string id_user = Session["username"].ToString();
                List<string> team = db.rca_team_connector.Where(p => p.id_rca == id_rca && p.id_user != id_user).Join(db.users, q => q.id_user, p => p.username, (q, p) => new { id_user = SqlFunctions.StringConvert((double)p.employee_id).Trim() }).Select(p => p.id_user).Distinct().ToList();
                ViewBag.investigator_team = team;

                List<rca_implementation> imps = db.rca_implementation.Where(p => p.id_rca == id_rca).ToList();
                foreach (rca_implementation imp in imps)
                {
                    iir_recommendations rec = new iir_recommendations
                    {
                        id_iir = null,
                        PIC = imp.pic.ToString(),
                        description = imp.next_action,
                        completion_date = imp.due_date,
                        id_rca = id_rca
                    };
                    db.iir_recommendations.Add(rec);
                    db.SaveChanges();
                }

                ViewBag.immediate_cause = rca.immediate_cause;
                ViewBag.basic_cause = rca.basic_cause;
            } else if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.investigation_report.Find(id);
            }
            return View();
        }

        [HttpPost]
        public JsonResult Add(investigation_report investigationReport, int? id_rca, IList<string> investigators)
        {
            
            employee e;
            //e = db.employees.Find(Int32.Parse(investigationReport.investigator));

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
            string investigator = "";
            string investigator_sign = "";
            bool isFirst = true;
            List<String> s = new List<string>();
            foreach (string ss in investigators)
            {
                investigator += ss + ";";
                e = db.employees.Find(Int32.Parse(ss));
                if (isFirst)
                {
                    investigator_sign += e.signature + ";";
                    isFirst = false;
                }
                else
                {
                    investigator_sign += ";";
                }
                if (e.email != null)
                    s.Add(e.email);
            }
            investigator = investigator.Substring(0, investigator.Length - 1);
            investigator_sign = investigator_sign.Substring(0, investigator_sign.Length - 1);
            Debug.WriteLine(investigator_sign);
            investigationReport.investigator = investigator;
            investigationReport.reference_number = ir_ref;
            investigationReport.investigator_approve = investigator_sign;
            db.investigation_report.Add(investigationReport);
            db.SaveChanges();

            int id = investigationReport.id;
            investigation_report_log ir_log = new investigation_report_log
            {
                id_iir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Create new Incident Report",
                date = DateTime.Now
            };
            db.investigation_report_log.Add(ir_log);
            db.SaveChanges();


            //SEND TO NEXT LEVEL (TEAM)
            List<int> sendInvestigatorDataTemp = new List<int>();
            bool isFirstRecord = true;
            foreach (string investigatorData in investigators)
            {
                if(isFirstRecord == true)
                {
                    isFirstRecord = false;
                }
                else if (investigatorData != null && investigatorData != "")
                {
                    sendInvestigatorDataTemp.Add(Int32.Parse(investigatorData));
                    
                }
            }
            if (sendInvestigatorDataTemp.Count > 0)
            {
                this.SendUserNotification(investigationReport, sendInvestigatorDataTemp.ToArray(), "Please Approve "+investigationReport.reference_number);
            }



            
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
            if (investigationReport.safety_officer != null && investigationReport.safety_officer_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(investigationReport.safety_officer));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(investigationReport.safety_officer_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }
            //if (s.Count > 0)
            //    sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Investigation Report dengan nomor referensi " + investigationReport.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Investigation Report with reference number " + investigationReport.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(HttpContext.Session["id"].ToString())).alpha_name, "Approving Incident Investigation Report " + investigationReport.reference_number);

            id = investigationReport.id;
            if (id_rca != null)
            {
                rca ir = db.rcas.Find(id_rca);
                ir.id_iir = id;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<iir_recommendations> li = db.iir_recommendations.Where(p => p.id_rca == id_rca ).ToList();
            string recommendation = "";
            int i = 1;
            foreach (iir_recommendations l in li)
            {
                l.id_iir = id;
                if (l.has_pir == 1)
                {
                    recommendation += i + ". " + l.description + "<br />";
                    i++;
                }
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            e = db.employees.Where(p => p.position == "wwms coordinator").FirstOrDefault();
            s.Clear();
            if (e != null && e.email != null)
            {
                s.Add(e.email);
            }

            if (s.Count > 0 && recommendation != "")
                sendEmail.Send(s, @"Bapak/Ibu,<br />Mohon untuk membuat PIR dari Incident Investigation Report 
                                    dengan nomor referensi " + investigationReport.reference_number + 
                                    ".<br />Berikut adalah rekomendasi-rekomendasi yang perlu dibuatkan PIR:<br /><br />" + recommendation + 
                                    @"<br />Terima Kasih.<br /><br />
                                    <i>Dear Sir/Madam,<br />Please create PIR for Incident Investigation Report with reference number " + 
                                    investigationReport.reference_number + @".Below listed recommendations that need to be created PIR:
                                    <br /><br />" + recommendation + @"<br />Thank you.</i><br /><br />
                                    Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(HttpContext.Session["id"].ToString())).alpha_name, "Creating PIR");



            ////SEND TO NEXT LEVEL (TEAM)
            //if (ts.supervisor_approval_name != null && ts.supervisor_approval_name != "")
            //{
            //    this.SendUserNotification(ts, Int32.Parse(ts.supervisor_approval_name), comment);
            //}
            //if (ts.supervisor_delegate != null && ts.supervisor_delegate != "")
            //{
            //    this.SendUserNotification(ts, Int32.Parse(ts.supervisor_delegate), comment);
            //}




            return Json(new { ref_num = ir_ref });
        }

        [HttpPost]
        public JsonResult Edit(investigation_report investigationReport, IList<string> investigators)
        {
            string investigator = "";
            bool isFirst = true;
            List<String> s = new List<string>();
            foreach (string ss in investigators)
            {
                investigator += ss + ";";
                if (isFirst)
                {
                    isFirst = false;
                }
            }
            investigator = investigator.Substring(0, investigator.Length - 1);

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
            iir.investigator = investigator;
            iir.investigator_date = investigationReport.investigator_date;
            iir.loss_control = investigationReport.loss_control;
            iir.loss_control_date = investigationReport.loss_control_date;
            iir.field_manager = investigationReport.field_manager;
            iir.field_manager_date = investigationReport.field_manager_date;
            iir.loss_control_delegate = investigationReport.loss_control_delegate;
            iir.field_manager_delegate = investigationReport.field_manager_delegate;
            iir.safety_officer = investigationReport.safety_officer;
            iir.safety_officer_date = investigationReport.safety_officer_date;
            iir.safety_officer_delegate = investigationReport.safety_officer_delegate;

            db.Entry(iir).State = EntityState.Modified;
            db.SaveChanges();

            investigation_report_log ir_log = new investigation_report_log
            {
                id_iir = iir.id,
                username = HttpContext.Session["username"].ToString(),
                status = "Change Incident Report",
                date = DateTime.Now
            };
            db.investigation_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(true);
        }
        #endregion


        #region recommendation

        //
        // Ajax select binding recommendation
        [GridAction]
        public ActionResult _SelectAjaxRecommendation(int? id,int? id_rca)
        {
            return bindingRecommendation(id,id_rca);
        }

        //select data incident report
        private ViewResult bindingRecommendation(int? id, int? id_rca)
        {
            List<iir_recommendations> f = new List<iir_recommendations>();
            Debug.WriteLine(id == null);
            if (id == null)
            {
                if (id_rca != null)
                {
                    f = db.iir_recommendations.Where(p => p.id_rca == id_rca).ToList();
                    foreach (iir_recommendations iir in f)
                    {
                        employee e = db.employees.Find(Int32.Parse(iir.PIC != null && iir.PIC != "" ? iir.PIC : "0"));
                        iir.PIC = e != null ? e.alpha_name : "";
                    }
                }
            }
            else
            {
                f = db.iir_recommendations.Where(p => p.id_iir == id).ToList();
                foreach (iir_recommendations iir in f)
                {
                    employee e = db.employees.Find(Int32.Parse(iir.PIC != null && iir.PIC != "" ? iir.PIC : "0"));
                    iir.PIC = e != null ? e.alpha_name : "";
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
            return bindingRecommendation(id_iir,null);
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
                safety_officer_date = iir.safety_officer_date,
                safety_officer_approve = iir.safety_officer_approve,
                loss_control_approve = iir.loss_control_approve,
                field_manager_approve = iir.field_manager_approve
            };

            piim.iir_recommendations = db.iir_recommendations.Where(p => p.id_iir == id).ToList();
            piim.investigator = new List<string>();
            piim.investigator_approve = new List<string>();
            foreach (iir_recommendations iirs in piim.iir_recommendations)
            {
                iirs.PIC = db.employees.Find(Int32.Parse(iirs.PIC == null ? "0" : iirs.PIC)).alpha_name;
            }
            if (iir.investigator != null)
            {
                List<string> temp = iir.investigator.Split(';').ToList();
                List<string> temp2 = iir.investigator_approve.Split(';').ToList();
                foreach (string t in temp)
                {
                    string s = has.Find(p => p.id == Int32.Parse(t == "" ? "0" : t)).alpha_name;
                    piim.investigator.Add(s);
                    
                }

                foreach (string t in temp2)
                {
                    piim.investigator_approve.Add(t);
                }


            }
            if (iir.loss_control != null && (iir.loss_control_approve == null || iir.loss_control_approve.Substring(0, 1) == "a"))
                piim.loss_control = has.Find(p => p.id == Int32.Parse(iir.loss_control == null ? "0" : iir.loss_control)).alpha_name;
            else
                piim.loss_control = has.Find(p => p.id == Int32.Parse(iir.loss_control_delegate == null ? "0" : iir.loss_control_delegate)).alpha_name;
            if (iir.safety_officer != null && (iir.safety_officer_approve == null || iir.safety_officer_approve.Substring(0, 1) == "a"))
                piim.safety_officer = has.Find(p => p.id == Int32.Parse(iir.safety_officer == null ? "0" : iir.safety_officer)).alpha_name;
            else
                piim.safety_officer = has.Find(p => p.id == Int32.Parse(iir.safety_officer_delegate == null ? "0" : iir.safety_officer_delegate)).alpha_name;
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
                investigation_report_log ir_log = new investigation_report_log
                {
                    id_iir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Safety Supervisor",
                    date = DateTime.Now
                };
                db.investigation_report_log.Add(ir_log);
                db.SaveChanges();

                if (iir.safety_officer != null && iir.safety_officer != "")
                {
                    this.SendUserNotification(iir, Int32.Parse(iir.safety_officer), "Please Approve " + iir.reference_number);
                }
                if (iir.safety_officer_delegate != null && iir.safety_officer_delegate != "")
                {
                    this.SendUserNotification(iir, Int32.Parse(iir.safety_officer_delegate), "Please Approve " + iir.reference_number);
                }

                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveSafetyOfficer(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                investigation_report iir = db.investigation_report.Find(id);
                if (iir.safety_officer == employee_id.ToString())
                {
                    iir.safety_officer_approve = "a" + sign;
                }
                else
                {
                    iir.safety_officer_approve = "d" + sign;
                }
                db.Entry(iir).State = EntityState.Modified;
                db.SaveChanges();
                investigation_report_log ir_log = new investigation_report_log
                {
                    id_iir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by SHE Independent",
                    date = DateTime.Now
                };
                db.investigation_report_log.Add(ir_log);
                db.SaveChanges();

                if (iir.field_manager != null && iir.field_manager != "")
                {
                    this.SendUserNotification(iir, Int32.Parse(iir.field_manager), "Please Approve " + iir.reference_number);
                }
                if (iir.field_manager_delegate != null && iir.field_manager_delegate != "")
                {
                    this.SendUserNotification(iir, Int32.Parse(iir.field_manager_delegate), "Please Approve " + iir.reference_number);
                }

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
                string[] inves = iir.investigator.Split(';');
                int i = 0;
                foreach (string ins in inves)
                {
                    if (ins == employee_id.ToString())
                    {
                        break;
                    }
                    i++;
                }
                string inves_approve = "";
                int j = 0;
                string[] inves_appr = iir.investigator_approve.Split(';');
                foreach (string ins in inves_appr)
                {
                    if (i == j)
                    {
                        inves_approve += sign + ";";
                    }
                    else
                    {
                        inves_approve += ins + ";";
                    }
                    j++;
                }
                inves_approve = inves_approve.Substring(0, inves_approve.Length - 1);
                iir.investigator_approve = inves_approve;
                db.Entry(iir).State = EntityState.Modified;
                investigation_report_log ir_log = new investigation_report_log
                {
                    id_iir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Investigator",
                    date = DateTime.Now
                };
                db.investigation_report_log.Add(ir_log);
                db.SaveChanges();

                bool sendToLossControl = true;
                string[] temp = iir.investigator_approve.Split(';');
                foreach (string x in temp)
                {
                    if (x == "" || x == null)
                    {
                        sendToLossControl = false;
                    }
                }

                if (sendToLossControl == true)
                {
                    if (iir.loss_control != null && iir.loss_control != "")
                    {
                        this.SendUserNotification(iir, Int32.Parse(iir.loss_control), "Please Approve " + iir.reference_number);
                    }
                    if (iir.loss_control_delegate != null && iir.loss_control_delegate != "")
                    {
                        this.SendUserNotification(iir, Int32.Parse(iir.loss_control_delegate), "Please Approve " + iir.reference_number);
                    }

                    ir_log = new investigation_report_log
                    {
                        id_iir = id,
                        username = HttpContext.Session["username"].ToString(),
                        status = "Approved by all Investigator",
                        date = DateTime.Now
                    };
                    db.investigation_report_log.Add(ir_log);
                    db.SaveChanges();
                }

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
                investigation_report_log ir_log = new investigation_report_log
                {
                    id_iir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Field Manager",
                    date = DateTime.Now
                };
                db.investigation_report_log.Add(ir_log);
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
            List<string> investigator = investigationReport.investigator.Split(';').ToList();
            if (investigationReport.investigator != null)
            {
                employee e = db.employees.Find(Int32.Parse(investigator[0]));
                if (e.email != null)
                    s.Add(e.email);
            }
            List<string> investigator_appr = investigationReport.investigator_approve.Split(';').ToList();
            bool isFirst = true;
            string retAppr = "";
            foreach (string appr in investigator_appr)
            {
                if (isFirst)
                {
                    retAppr += appr + ";";
                    isFirst = false;
                }
                else
                {
                    retAppr += ";";
                }
            }
            retAppr.Substring(0, retAppr.Length - 1);
            investigationReport.investigator_approve = retAppr;
            db.Entry(investigationReport).State = EntityState.Modified;
            db.SaveChanges();

            investigation_report_log ir_log = new investigation_report_log
            {
                id_iir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Safety Supervisor",
                comment = comment,
                date = DateTime.Now
            };
            db.investigation_report_log.Add(ir_log);
            db.SaveChanges();

            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Investigation Report dengan Reference Number " + investigationReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Safety Supervisor.\n\nTerima Kasih.", "Rejected Incident Investigation Report " + investigationReport.reference_number);

            string[] inves = investigationReport.investigator.Split(';');
            if (inves[0] != null || inves[0] != "")
            {
                this.SendUserNotification(investigationReport, Int32.Parse(inves[0]), investigationReport.reference_number + " is rejected with comment: " + comment);
            }


            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult rejectSafetyOfficer(int id, string comment)
        {
            investigation_report investigationReport = db.investigation_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            List<string> investigator = investigationReport.investigator.Split(';').ToList();
            if (investigationReport.investigator != null)
            {
                employee e = db.employees.Find(Int32.Parse(investigator[0]));
                if (e.email != null)
                    s.Add(e.email);
            }

            investigationReport.loss_control_approve = null;
            db.Entry(investigationReport).State = EntityState.Modified;
            db.SaveChanges();

            investigation_report_log ir_log = new investigation_report_log
            {
                id_iir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by SHE Superintendent",
                comment = comment,
                date = DateTime.Now
            };
            db.investigation_report_log.Add(ir_log);
            db.SaveChanges();

            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Investigation Report dengan Reference Number " + investigationReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh SHE Superintendent.\n\nTerima Kasih.", "Rejected Incident Investigation Report " + investigationReport.reference_number);

            if (investigationReport.loss_control != null && investigationReport.loss_control != "")
            {
                this.SendUserNotification(investigationReport, Int32.Parse(investigationReport.loss_control), investigationReport.reference_number + " is rejected with comment: " + comment);
            }
            if (investigationReport.loss_control_delegate != null && investigationReport.loss_control_delegate != "")
            {
                this.SendUserNotification(investigationReport, Int32.Parse(investigationReport.loss_control_delegate), investigationReport.reference_number + " is rejected with comment: " + comment);
            }


            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult rejectFieldManager(int id, string comment)
        {
            investigation_report investigationReport = db.investigation_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            List<string> investigator = investigationReport.investigator.Split(';').ToList();
            if (investigationReport.investigator != null)
            {
                employee e = db.employees.Find(Int32.Parse(investigator[0]));
                if (e.email != null)
                    s.Add(e.email);
            }

            investigationReport.safety_officer_approve = null;
            db.Entry(investigationReport).State = EntityState.Modified;
            db.SaveChanges();

            investigation_report_log ir_log = new investigation_report_log
            {
                id_iir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Field Manager",
                comment = comment,
                date = DateTime.Now
            };
            db.investigation_report_log.Add(ir_log);
            db.SaveChanges();

            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Investigation Report dengan Reference Number " + investigationReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Field Manager.\n\nTerima Kasih.", "Rejected Incident Investigation Report " + investigationReport.reference_number);

            if (investigationReport.safety_officer != null && investigationReport.safety_officer != "")
            {
                this.SendUserNotification(investigationReport, Int32.Parse(investigationReport.safety_officer),investigationReport.reference_number + " is rejected with comment: " + comment);
            }
            if (investigationReport.safety_officer_delegate != null && investigationReport.safety_officer_delegate != "")
            {
                this.SendUserNotification(investigationReport, Int32.Parse(investigationReport.safety_officer_delegate), investigationReport.reference_number + " is rejected with comment: " + comment);
            }


            return Json(new { success = true });

        }
        #endregion


        [HttpPost]
        public JsonResult SendEmail(int id)
        {
            investigation_report investigationReport = db.investigation_report.Find(id);
            employee e;

            List<String> s = new List<string>();
            List<string> investigators = investigationReport.investigator.Split(';').ToList();
            foreach (string ss in investigators)
            {
                e = db.employees.Find(Int32.Parse(ss));
                if (e.email != null)
                    s.Add(e.email);
            }

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
            if (investigationReport.safety_officer != null && investigationReport.safety_officer_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(investigationReport.safety_officer));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(investigationReport.safety_officer_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Investigation Report dengan nomor referensi " + investigationReport.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Investigation Report with reference number " + investigationReport.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(HttpContext.Session["id"].ToString())).alpha_name, "Approving Incident Investigation Report " + investigationReport.reference_number);
            return Json(true);
        }

        private void SendUserNotification(investigation_report data, int sendUserId, string message)
        {
            WWService.UserServiceClient client = new WWService.UserServiceClient();
            WWService.ResponseModel response = client.CreateNotification(
            EncodeMd5("starenergyww"),
            sendUserId,
            System.Configuration.ConfigurationManager.AppSettings["ApplicationName"],
            "Incident Investigation Report",
            message,
            "#");

        }

        private void SendUserNotification(investigation_report data, int[] sendUserId, string message)
        {
            WWService.UserServiceClient client = new WWService.UserServiceClient();
            WWService.ResponseModel response = client.CreateNotificationList(
            EncodeMd5("starenergyww"),
            sendUserId,
            System.Configuration.ConfigurationManager.AppSettings["ApplicationName"],
            "Incident Investigation Report",
            message,
            "#");

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
    }
}
