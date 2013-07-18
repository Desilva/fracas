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

namespace StarEnergi.Controllers.Admin
{
    public class IncidentsController : PdfViewController
    {
        //
        // GET: /Incident/
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;


        public ActionResult Index()
        {
            var has = (from users in db.users
                      select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
            ViewData["users"] = has;
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            ViewData["user_role"] = li;
            return PartialView();
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
                           employee = employees.employee2
                       }).ToList();
            List<EmployeeEntity> bind = has;
            foreach (EmployeeEntity ee in bind)
            {
                int level = 0;

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

            incident_report inc = db.incident_report.ToList().LastOrDefault();

            if (inc == null || inc.reference_number == null || inc.reference_number.Length != 12)
            {
                int refs = 1;
                ViewBag.ir_ref = "IRxxxx-" + refs.ToString().PadLeft(5, '0');
            }
            else
            {
                int refs = Int32.Parse(inc.reference_number.Substring(7));
                refs++;
                ViewBag.ir_ref = "IRxxxx-" + refs.ToString().PadLeft(5, '0');
            }
            ViewData["users"] = bind;
            ViewData["user_role"] = li;
            if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.incident_report.Find(id);
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
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewData["users"] = has;
            ViewData["user_role"] = li;
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

            return View(new GridModel<incident_report>
            {
                Data = f.OrderBy(p => p.id)
            });
        }

        [HttpPost]
        public JsonResult Add(incident_report incidentReport)
        {
            db.incident_report.Add(incidentReport);
            db.SaveChanges();
            employee e;
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.superintendent != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.superintendent));
                if (e.email != null)
                    s.Add(e.email);
                
            }
            if (incidentReport.field_manager != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.field_manager));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (incidentReport.loss_control != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.loss_control));
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
                sendEmail.Send(s, "Salam,\n\nIncident Report dengan Reference Number " + incidentReport.reference_number + " perlu di-approve.\n\nTerima Kasih.", "Approving Incident Report " + incidentReport.reference_number);

            int last_id = db.incident_report.Max(p => p.id);
            string subPath = "~/Attachment/incident_report/" + last_id + "/signatures"; // your code goes here
            bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
            if (!IsExists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

            subPath = "~/Attachment/incident_report/" + last_id + "/atch"; // your code goes here
            IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
            if (!IsExists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

            return Json(true);
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
            ir.doms = incidentReport.doms;
            ir.doms_date = incidentReport.doms_date;
            ir.investigation_req = incidentReport.investigation_req;
            ir.investigation = incidentReport.investigation;
            ir.lead_name = incidentReport.lead_name;

            db.Entry(ir).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult Attach(int type_approve)
        {
            

            return Json(true);
        }

        [HttpPost]
        public ActionResult approveLossControl(HttpPostedFileBase fileData,int id)
        {
            var currpath = "";
            if (fileData != null && fileData.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/incident_report/" + id + "/signatures"),
                    fileData.FileName
                );
                fileData.SaveAs(currpath);

                incident_report ir = db.incident_report.Find(id);
                currpath = "/Attachment/incident_report/" + id + "/signatures/" + fileData.FileName;
                ir.loss_control_approve = currpath;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true , path = currpath});
        }

        [HttpPost]
        public ActionResult approveSuperintendent(HttpPostedFileBase fileData, int id)
        {
            var currpath = "";
            if (fileData != null && fileData.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/incident_report/" + id + "/signatures"),
                    fileData.FileName
                );
                fileData.SaveAs(currpath);

                incident_report ir = db.incident_report.Find(id);
                currpath = "/Attachment/incident_report/" + id + "/signatures/" + fileData.FileName;
                ir.superintendent_approve = currpath;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true, path = currpath });
        }

        [HttpPost]
        public ActionResult approveFieldManager(HttpPostedFileBase fileData, int id)
        {
            var currpath = "";
            if (fileData != null && fileData.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/incident_report/" + id + "/signatures"),
                    fileData.FileName
                );
                fileData.SaveAs(currpath);

                incident_report ir = db.incident_report.Find(id);
                currpath = "/Attachment/incident_report/" + id + "/signatures/" + fileData.FileName;
                ir.field_manager_approve = currpath;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true, path = currpath });
        }

        [HttpPost]
        public ActionResult attachment(HttpPostedFileBase fileData, int id)
        {
            var currpath = "";
            string st = "";
            if (fileData != null && fileData.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/incident_report/" + id + "/atch"),
                    fileData.FileName
                );
                fileData.SaveAs(currpath);
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
        public ActionResult Atch(int id)
        {
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
            Debug.WriteLine("testing unit");
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
                doms_date = ir.doms_date,
                type_of_report = ir.type_of_report,
                actual_loss_severity = ir.actual_loss_severity,
                potential_loss_severity = ir.potential_loss_severity,
                probability = ir.probability,
                investigation = ir.investigation
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

            if (ir.superintendent != null && ir.superintendent != "null")
                pim.superintendent = has.Find(p => p.id == Int32.Parse(ir.superintendent)).alpha_name;
            if (ir.loss_control != null && ir.loss_control != "null")
                pim.loss_control = has.Find(p => p.id == Int32.Parse(ir.loss_control)).alpha_name;
            if (ir.field_manager != null && ir.field_manager != "null")
                pim.field_manager = has.Find(p => p.id == Int32.Parse(ir.field_manager)).alpha_name;
            if (ir.doms != null && ir.investigation_req != "null")
                pim.doms = has.Find(p => p.id == Int32.Parse(ir.doms)).alpha_name;
            if (ir.investigation_req != null && ir.investigation_req != "null") pim.investigation_req = has.Find(p => p.id == Int32.Parse(ir.investigation_req)).alpha_name;

            return pim;
        }

        [HttpPost]
        public JsonResult getCode(int id)
        {
            return Json(new { id = id });
        }
    }
}
