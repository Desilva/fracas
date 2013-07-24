using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using ReportManagement;
using StarEnergi.Controllers.Utilities;
using System.IO;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class IncidentController : PdfViewController
    {
        //
        // GET: /Incident/
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;


        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Incident" });
            }
            var has = (from users in db.users
                      select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
            return View();
        }

        public ActionResult addIncident(int? id) {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id 
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       orderby employees.dept_id
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           dept_id = employees.dept_id,
                           username = (ue.username == null ? String.Empty : ue.username),
                           employee = employees.employee2,
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate,
                           approval_level = employees.approval_level
                       }).ToList();
            List<EmployeeEntity> bind = has;
            foreach (EmployeeEntity ee in bind)
            {
                int level = 0;
                ee.role = db.user_per_role.Where(p => p.username == (ee.username != null ? ee.username : "")).ToList();
                if (ee.employee != null)
                {
                    employee temp = ee.employee;
                    level = 1;

                    while (temp.employee2 != null)
                    {
                        temp = temp.employee2;
                        level++;
                    }
                }
                ee.level = level;
            }
            ViewBag.facility = db.rca_facility.ToList();

            incident_report inc = db.incident_report.OrderBy(p => p.reference_number).ToList().LastOrDefault();

            if (inc == null || inc.reference_number == null || inc.reference_number.Length != 20)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ViewBag.ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(inc.reference_number.Substring(16));
                refs++;
                int reference_year = Int32.Parse(inc.reference_number.Substring(11, 4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ViewBag.ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else
                {
                    refs = 1;
                    ViewBag.ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }
            ViewData["users"] = bind.OrderBy(p => p.alpha_name).ToList();
            ViewData["user_role"] = li;
            if (id != null)
            {
                ViewBag.mod = id;
                incident_report ir = db.incident_report.Find(id);
                ViewBag.datas = ir;
                ViewBag.superintendent_del = ir.superintendent_approve == null ? (ir.superintendent == null ? null : db.employees.Find(Int32.Parse(ir.superintendent == null ? "0" : ir.superintendent)).employee_delegate) : null;
                ViewBag.she_superintendent_del = ir.she_superintendent_approve == null ? db.employees.Find(Int32.Parse(ir.she_superintendent == null ? "0" : ir.she_superintendent)).employee_delegate : null;
                ViewBag.loss_control_del = ir.loss_control_approve == null ? db.employees.Find(Int32.Parse(ir.loss_control == null ? "0" : ir.loss_control)).employee_delegate : null;
                ViewBag.field_manager_del = ir.field_manager_approve == null ? db.employees.Find(Int32.Parse(ir.field_manager == null ? "0" : ir.field_manager)).employee_delegate : null;
            }
            else
            {
                int cur_user_id = Int32.Parse(HttpContext.Session["id"].ToString());
                int? cur_dept_id = db.employees.Find(cur_user_id).dept_id;
                employee cur_user_boss = db.employees.Find(cur_user_id);
                int? superintendent_id = null;
                int? superintendent_id_del = null;
                int? supervisor_id = null;
                int? supervisor_id_del = null;
                while (cur_user_boss != null)
                {
                    if (cur_user_boss.approval_level == 1)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            supervisor_id = cur_user_boss.id;
                            supervisor_id_del = cur_user_boss.employee_delegate;
                        }
                        else
                        {
                            supervisor_id = cur_user_boss.id;
                        }
                    }
                    if (cur_user_boss.approval_level == 2)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            superintendent_id = cur_user_boss.id;
                            superintendent_id_del = cur_user_boss.employee_delegate;
                        } else {
                            superintendent_id = cur_user_boss.id;
                        }
                    }
                    cur_user_boss = cur_user_boss.employee2;
                }
                ViewBag.superintendent_id = superintendent_id;
                ViewBag.supervisor_id = supervisor_id;
                ViewBag.superintendent_id_del = superintendent_id_del;
                ViewBag.supervisor_id_del = supervisor_id_del;
                int last_id = db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id);
                last_id++;
                string subPath = "~/Attachment/incident_report/" + last_id + "/signatures"; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

                subPath = "~/Attachment/incident_report/" + last_id + "/atch"; // your code goes here
                IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            }
            return PartialView();
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
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewData["users"] = has;
            ViewData["user_role"] = li;
            return PartialView();
        }

        public ActionResult incidentLog(int? id)
        {
            List<incident_report_log> ir_log = db.incident_report_log.Where(p => p.id_ir == id).ToList();
            foreach (incident_report_log i in ir_log)
            {
                int? employee_id = db.users.Find(i.username).employee_id;
                employee e = db.employees.Find(employee_id);
                i.username = e.alpha_name;
            }
            ViewData["log"] = ir_log;
            ViewBag.ref_number = db.incident_report.Find(id).reference_number;
            return PartialView();
        }

        //
        // Ajax select binding incident report
        [GridAction]
        public ActionResult _SelectAjaxIncidentReport()
        {
            return bindingIncidentReport();
        }

        //select data incident report
        private ViewResult bindingIncidentReport()
        {
            List<incident_report> f = new List<incident_report>();
            f = db.incident_report.ToList();

            foreach(incident_report i in f){
                if (i.id_rca != null)
                {
                    var r = (from rcas in db.rcas
                             where rcas.id == i.id_rca
                             select new
                             {
                                 rca_number = rcas.rca_code
                             }).ToList();
                    i.rca_number = r.FirstOrDefault().rca_number;
                }
                i.type_report = i.type_of_report == 1 ? "On the job" : i.type_of_report == 0 ? "Off the job" : "";
                i.actual_loss = i.actual_loss_severity == 1 ? "Major" : i.actual_loss_severity == 2 ? "Serious" : i.actual_loss_severity == 3 ? "Moderate" : i.actual_loss_severity == 4 ? "Minor" : "";
                i.potential_loss = i.potential_loss_severity == 1 ? "Major" : i.potential_loss_severity == 2 ? "Serious" : i.potential_loss_severity == 3 ? "Moderate" : i.potential_loss_severity == 4 ? "Minor" : "";
                i.probability_str = i.probability == 1 ? "Frequent" : i.probability == 2 ? "Occasional" : i.probability == 3 ? "Seldom" : i.probability == 4 ? "Rare" : "";
                i.prepared_by = i.prepared_by != "" ? db.employees.Find(int.Parse((i.prepared_by == "" || i.prepared_by == null) ? "0" : i.prepared_by)).alpha_name : "";
            }

            return View(new GridModel<incident_report>
            {
                Data = f.OrderByDescending(p => p.reference_number)
            });
        }

        [HttpPost]
        public JsonResult Add(incident_report incidentReport)
        {
            int id_before = (db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id)) + 1;

            incident_report inc = db.incident_report.OrderBy(p => p.reference_number).ToList().LastOrDefault();
            string ir_ref = "";
            if (inc == null || inc.reference_number == null || inc.reference_number.Length != 20)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(inc.reference_number.Substring(16));
                refs++;
                int reference_year = Int32.Parse(inc.reference_number.Substring(11, 4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else
                {
                    refs = 1;
                    ir_ref = "W-O-SPE-IR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }
            incidentReport.reference_number = ir_ref;
            db.incident_report.Add(incidentReport);
            db.SaveChanges();
            employee e;
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.superintendent != null && incidentReport.superintendent_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.superintendent));
                if (e.email != null)
                    s.Add(e.email);

            }
            else
            {
                e = db.employees.Find(Int32.Parse(incidentReport.superintendent_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (incidentReport.field_manager != null && incidentReport.field_manager_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.field_manager));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(incidentReport.field_manager_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (incidentReport.loss_control != null && incidentReport.loss_control_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.loss_control));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(incidentReport.loss_control_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (incidentReport.she_superintendent != null && incidentReport.she_superintendent_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.she_superintendent));
                if (e.email != null)
                    s.Add(e.email);
            }
            else
            {
                e = db.employees.Find(Int32.Parse(incidentReport.she_superintendent_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (incidentReport.ack_supervisor != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.ack_supervisor));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (incidentReport.lead_name != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.lead_name));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Report dengan nomor referensi " + incidentReport.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Report with reference number " + incidentReport.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(incidentReport.prepared_by)).alpha_name, "Approving Incident Report " + incidentReport.reference_number);

            int id = db.incident_report.Max(p => p.id);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Create new Incident Report"
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            if (id != id_before)
            {
                string subPathSign = "~/Attachment/incident_report/" + id + "/signatures"; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPathSign));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPathSign));

                string subPath = "~/Attachment/incident_report/" + id + "/atch"; // your code goes here
                IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

                try
                {
                    string old_path = Server.MapPath("~/Attachment/incident_report/" + id_before + "/atch");
                    string new_path = Server.MapPath("~/Attachment/incident_report/" + id + "/atch");
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/incident_report/" + id_before + "/atch"), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        System.IO.File.Move(old_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1), new_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }

            return Json(new { ref_num = incidentReport.reference_number });
        }

        [HttpPost]
        public JsonResult Edit(incident_report incidentReport)
        {
            incident_report ir = db.incident_report.Find(incidentReport.id);

            Debug.WriteLine(incidentReport.prepare_date);
            ir.facility = incidentReport.facility;
            ir.incident_location = incidentReport.incident_location;
            ir.reference_number = incidentReport.reference_number;
            ir.incident_type = incidentReport.incident_type;
            ir.type_of_report = incidentReport.type_of_report;
            ir.title = incidentReport.title;
            ir.date_incident = incidentReport.date_incident;
            ir.actual_loss_severity = incidentReport.actual_loss_severity;
            ir.potential_loss_severity = incidentReport.potential_loss_severity;
            ir.probability = incidentReport.probability;
            ir.factual_information = incidentReport.factual_information;
            ir.cost_estimate = incidentReport.cost_estimate;
            ir.immediate_action = incidentReport.immediate_action;
            ir.prepared_by = incidentReport.prepared_by;
            ir.prepare_date = incidentReport.prepare_date;
            ir.ack_supervisor = incidentReport.ack_supervisor;
            ir.ack_date = incidentReport.ack_date;
            ir.superintendent = incidentReport.superintendent;
            ir.superintendent_date = incidentReport.superintendent_date;
            ir.loss_control = incidentReport.loss_control;
            ir.loss_date = incidentReport.loss_date;
            ir.field_manager = incidentReport.field_manager;
            ir.field_manager_date = incidentReport.field_manager_date;
            ir.she_superintendent = incidentReport.she_superintendent;
            ir.she_superintendent_date = incidentReport.she_superintendent_date;
            ir.investigation_req = incidentReport.investigation_req;
            ir.investigation = incidentReport.investigation;
            ir.lead_name = incidentReport.lead_name;
            ir.superintendent_delegate = incidentReport.superintendent_delegate;
            ir.she_superintendent_delegate = incidentReport.she_superintendent_delegate;
            ir.loss_control_delegate = incidentReport.loss_control_delegate;
            ir.field_manager_delegate = incidentReport.field_manager_delegate;

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();

            incident_report_log ir_log = new incident_report_log
            {
                id_ir = incidentReport.id,
                username = HttpContext.Session["username"].ToString(),
                status = "Change Incident Report",
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult selectSup(int id, int employee_id)
        {
            employee emp = db.employees.Find(employee_id);
            bool delegates = emp.delagate == 1;

            return Json(new { success = true, delegates = delegates, delegate_name = emp.employee_delegate });
        }

        #region approval

        [HttpPost]
        public ActionResult approveLossControl(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                incident_report ir = db.incident_report.Find(id);
                if (ir.loss_control == employee_id.ToString())
                {
                    ir.loss_control_approve = "a" + sign;
                }
                else
                {
                    ir.loss_control_approve = "d" + sign;
                }
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                incident_report_log ir_log = new incident_report_log
                {
                    id_ir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Safety Supervisor",
                    date = DateTime.Now
                };
                db.incident_report_log.Add(ir_log);
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveSuperintendent(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                incident_report ir = db.incident_report.Find(id);
                if (ir.superintendent == employee_id.ToString())
                {
                    ir.superintendent_approve = "a" + sign;
                }
                else
                {
                    ir.superintendent_approve = "d" + sign;
                }
                
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                incident_report_log ir_log = new incident_report_log
                {
                    id_ir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Initiator Superintendent",
                    date = DateTime.Now
                };
                db.incident_report_log.Add(ir_log);
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false});
            }
            
        }

        [HttpPost]
        public ActionResult approveFieldManager(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                incident_report ir = db.incident_report.Find(id);
                if (ir.field_manager == employee_id.ToString())
                {
                    ir.field_manager_approve = "a" + sign;
                }
                else
                {
                    ir.field_manager_approve = "d" + sign;
                }
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                incident_report_log ir_log = new incident_report_log
                {
                    id_ir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Field Manager",
                    date = DateTime.Now
                };
                db.incident_report_log.Add(ir_log);
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveSheSuperintendent(int id, int employee_id)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                incident_report ir = db.incident_report.Find(id);
                if (ir.she_superintendent == employee_id.ToString())
                {
                    ir.she_superintendent_approve = "a" + sign;
                }
                else
                {
                    ir.she_superintendent_approve = "d" + sign;
                }
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                incident_report_log ir_log = new incident_report_log
                {
                    id_ir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by SHE Superintendent",
                    date = DateTime.Now
                };
                db.incident_report_log.Add(ir_log);
                db.SaveChanges();
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult rejectSuperintendent(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.prepared_by != null)
            {
                employee e = db.employees.Find(Int32.Parse(incidentReport.prepared_by));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Report dengan Reference Number " + incidentReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Initiator Superintendent.\n\nTerima Kasih.", "Rejected Incident Report " + incidentReport.reference_number);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Initiator Superintendent",
                comment = comment,
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectLossControl(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.prepared_by != null)
            {
                employee e = db.employees.Find(Int32.Parse(incidentReport.prepared_by));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Report dengan Reference Number " + incidentReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Safety Supervisor.\n\nTerima Kasih.", "Rejected Incident Report " + incidentReport.reference_number);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Safety Supervisor",
                comment = comment,
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectSheSuperintendent(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.prepared_by != null)
            {
                employee e = db.employees.Find(Int32.Parse(incidentReport.prepared_by));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Report dengan Reference Number " + incidentReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh SHE Superintendent.\n\nTerima Kasih.", "Rejected Incident Report " + incidentReport.reference_number);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by SHE Superintendent",
                comment = comment,
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectFieldManager(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.prepared_by != null)
            {
                employee e = db.employees.Find(Int32.Parse(incidentReport.prepared_by));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
                sendEmail.Send(s, "Salam,\n\nIncident Report dengan Reference Number " + incidentReport.reference_number + " perlu diperbaiki dengan komentar\n\"" + comment + "\" oleh Field Manager.\n\nTerima Kasih.", "Rejected Incident Report " + incidentReport.reference_number);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Field Manager",
                comment = comment,
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();
            return Json(new { success = true });

        }

        #endregion



        [HttpPost]
        public JsonResult Attach(int type_approve)
        {
            return Json(true);
        }
        
        [HttpPost]
        public ActionResult attachment(IEnumerable<HttpPostedFileBase> attachment, int? id)
        {
            var currpath = "";
            string st = "";
            if (id == null) id = (db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id)) + 1;
            if (attachment != null)
            {
                foreach (var file in attachment)
                {
                    currpath = Path.Combine(
                    Server.MapPath("~/Attachment/incident_report/" + id + "/atch"),
                    file.FileName);
                    file.SaveAs(currpath);
                }
                currpath = "/Attachment/incident_report/" + id + "/atch/";
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/incident_report/" + id + "/atch"), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        st += f.File.Substring(f.File.LastIndexOf("\\") + 1) + ";;";
                        Debug.WriteLine(f.File);
                    }
                    Debug.WriteLine(files.Count().ToString());
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }
            return Json(new { success = true, path = currpath, files=st });
        }

        [HttpPost]
        public ActionResult Atch(int? id)
        {
            if (id == null) id = (db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id)) + 1;
            var currpath = "/Attachment/incident_report/" + id + "/atch/";
            string st = "";
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/incident_report/" + id + "/atch"), "*.*", SearchOption.TopDirectoryOnly)
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    st += f.File.Substring(f.File.LastIndexOf("\\") + 1) + ";;";
                    Debug.WriteLine(f.File);
                }
                Debug.WriteLine(files.Count().ToString());
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Debug.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Debug.WriteLine(PathEx.Message);
            }
            return Json(new { success = true, path = currpath, files = st });
        }

        [HttpPost]
        public JsonResult DeleteAtch(int? id)
        {
            if (id == null)
            {
                id = (db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id)) + 1;
                var currpath = "/Attachment/incident_report/" + id + "/atch/";
                try
                {
                    string new_path = Server.MapPath("~/Attachment/incident_report/" + id + "/atch");
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/incident_report/" + id + "/atch"), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        System.IO.File.Delete(new_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    Debug.WriteLine(UAEx.Message);
                }
                catch (PathTooLongException PathEx)
                {
                    Debug.WriteLine(PathEx.Message);
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditInv(incident_report incidentReport)
        {
            incident_report ir = db.incident_report.Find(incidentReport.id);

            ir.investigation = incidentReport.investigation;
            ir.investigation_req = db.users.Find(HttpContext.Session["username"].ToString()).employee_id.ToString();

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        //
        // Ajax delete binding incident report
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxIncidentReport(int id)
        {
            deleteIncidentReport(id);
            return bindingIncidentReport();
        }

        //delete data fracas equipment
        private void deleteIncidentReport(int id)
        {
            incident_report ir = db.incident_report.Find(id);
            db.incident_report.Remove(ir);
            db.SaveChanges();
        }

        public ActionResult printIR(int id)
        {
            return this.ViewPdf("", "IncidentPrint", createIRModel(id));
        }

        private PrintIRModel createIRModel(int id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           dept_id = employees.dept_id
                       }).ToList();
            incident_report ir = db.incident_report.Find(id);
            PrintIRModel pim = new PrintIRModel
            {
                id = ir.id,
                facility = ir.facility,
                incident_location = ir.incident_location,
                reference_number = ir.reference_number,
                date_incident = ir.date_incident,
                title = ir.title,
                incident_type = ir.incident_type,
                factual_information = ir.factual_information,
                immediate_action = ir.immediate_action,
                cost_estimate = ir.cost_estimate,
                prepare_date = ir.prepare_date,
                ack_date = ir.ack_date,
                superintendent_date = ir.superintendent_date,
                loss_date = ir.loss_date,
                field_manager_date = ir.field_manager_date,
                she_superintendent_date = ir.she_superintendent_date,
                type_of_report = ir.type_of_report,
                actual_loss_severity = ir.actual_loss_severity,
                potential_loss_severity = ir.potential_loss_severity,
                probability = ir.probability,
                investigation = ir.investigation,
                loss_control_approve = ir.loss_control_approve,
                superintendent_approve = ir.superintendent_approve,
                field_manager_approve = ir.field_manager_approve,
                she_superintendent_approve = ir.she_superintendent_approve,
            };

            if (ir.prepared_by != null)
            {
                pim.prepared_by = has.Find(p => p.id == Int32.Parse(ir.prepared_by)).alpha_name;
                pim.prepared_by_jabatan = has.Find(p => p.id == Int32.Parse(ir.prepared_by)).position;
            }

            if (ir.ack_supervisor != null)
            {
                pim.ack_supervisor = has.Find(p => p.id == Int32.Parse(ir.ack_supervisor)).alpha_name;
                pim.ack_supervisor_jabatan = has.Find(p => p.id == Int32.Parse(ir.ack_supervisor)).position;
            }

            if (ir.superintendent != null && ir.superintendent != "null" && (ir.superintendent_approve == null || ir.superintendent_approve.Substring(0,1) == "a"))
                pim.superintendent = has.Find(p => p.id == Int32.Parse(ir.superintendent)).alpha_name;
            else
                pim.superintendent = has.Find(p => p.id == Int32.Parse(ir.superintendent_delegate)).alpha_name;
            if (ir.loss_control != null && ir.loss_control != "null" && (ir.loss_control_approve == null || ir.loss_control_approve.Substring(0, 1) == "a"))
                pim.loss_control = has.Find(p => p.id == Int32.Parse(ir.loss_control)).alpha_name;
            else
                pim.loss_control = has.Find(p => p.id == Int32.Parse(ir.loss_control_delegate)).alpha_name;
            if (ir.field_manager != null && ir.field_manager != "null" && (ir.field_manager_approve == null || ir.field_manager_approve.Substring(0, 1) == "a"))
                pim.field_manager = has.Find(p => p.id == Int32.Parse(ir.field_manager)).alpha_name;
            else
                pim.field_manager = has.Find(p => p.id == Int32.Parse(ir.field_manager_delegate)).alpha_name;
            if (ir.she_superintendent != null && ir.investigation_req != "null" && (ir.she_superintendent_approve == null || ir.she_superintendent_approve.Substring(0, 1) == "a"))
                pim.she_superintendent = has.Find(p => p.id == Int32.Parse(ir.she_superintendent)).alpha_name;
            else
                pim.she_superintendent = has.Find(p => p.id == Int32.Parse(ir.she_superintendent_delegate)).alpha_name;
            if (ir.investigation_req != null && ir.investigation_req != "null") pim.investigation_req = has.Find(p => p.id == Int32.Parse(ir.investigation_req)).alpha_name;
            if (ir.lead_name != null && ir.lead_name != "null")
                pim.lead_name = has.Find(p => p.id == Int32.Parse(ir.lead_name)).alpha_name;

            pim.attach = new List<string>();
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/incident_report/" + id + "/atch"), "*.*", SearchOption.TopDirectoryOnly)
                            where file.Substring(file.LastIndexOf(".") + 1).ToLower() == "png" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpeg"
                             || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpg" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "bmp" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "pdf"
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    pim.attach.Add(f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    Debug.WriteLine(f.File);
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                Debug.WriteLine(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                Debug.WriteLine(PathEx.Message);
            }
            return pim;
        }

        [HttpPost]
        public JsonResult getCode(int id)
        {
            incident_report i = db.incident_report.Find(id);

            return Json(new { id = id, analysis_title = i.title, cost = i.cost_estimate });
        }

        public ActionResult ExportExcelTotal()
        {
            List<incident_report_r> result = new List<incident_report_r>();
            List<incident_report> list_ir = db.incident_report.Where(p => p.incident_type == "Injury / Illness").ToList();
            double total_cost = 0;
            foreach(incident_report ir in list_ir) {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Injury / Illness", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Environmental Loss").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Environmental Loss", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Properti Damage").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Properti Damage", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Process Loss / Disturb").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Process Loss / Disturb", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "External Relation").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "External Relation", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Theft / Crimes").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Theft / Crimes", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Vehicular").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Vehicular", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type == "Near Miss").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Near Miss", cases = list_ir.Count, total_cost = total_cost });
            list_ir = db.incident_report.Where(p => p.incident_type != "Injury / Illness" && p.incident_type != "Environmental Loss" && p.incident_type != "Properti Damage"
                && p.incident_type != "Process Loss / Disturb" && p.incident_type != "External Relation" && p.incident_type != "Theft / Crimes" && p.incident_type != "Vehicular" && p.incident_type != "Near Miss").ToList();
            total_cost = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                }
            }
            result.Add(new incident_report_r { type = "Other", cases = list_ir.Count, total_cost = total_cost });
            int total_case = result.Sum(p => p.cases);
            total_cost = result.Sum(p => p.total_cost);
            result.Add(new incident_report_r { type = "Total", cases = total_case, total_cost = total_cost });
            GridView gv = new GridView();
            gv.Caption = "Incident Report";
            gv.DataSource = result;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Type of Incident";
            gv.HeaderRow.Cells[1].Text = "No. of Cases";
            gv.HeaderRow.Cells[2].Text = "Gen. Loss (USD)";
            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "incident report.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }
    }

    class incident_report_r
    {
        public string type {get;set;}
        public int cases { get; set; }
        public double total_cost { get; set; }
    }
}
