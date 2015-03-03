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
using System.Collections;
using System.Data.Objects.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace StarEnergi.Controllers.FrontEnd
{
    public class IncidentController : PdfViewController
    {
        //
        // GET: /Incident/
        public relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;
        private static int count;

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
            if (!li.Exists(p => p.role == (int)Config.role.INITIATORIR))
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Incident" });
            }
            ViewData["user_role"] = li;
            return View();
        }

        public ActionResult addIncident(int? id, int? id_fracas, int? id_injury, int?id_fracas_part)
        {
            
            string username = (String)Session["username"].ToString();
            li = db.user_per_role.Where(p => p.username == username).ToList();
            if (!li.Exists(p => p.role == (int)Config.role.INITIATORIR))
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Incident" });
            }
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
                ViewBag.superintendent_del = String.IsNullOrWhiteSpace(ir.superintendent_approve) == false ? (String.IsNullOrWhiteSpace(ir.superintendent) ? null : db.employees.Find(Int32.Parse(ir.superintendent == null ? "0" : ir.superintendent)).employee_delegate) : null;
                ViewBag.she_superintendent_del = String.IsNullOrWhiteSpace(ir.she_superintendent_approve) == false ? db.employees.Find(Int32.Parse(String.IsNullOrWhiteSpace(ir.she_superintendent) ? "0" : ir.she_superintendent)).employee_delegate : null;
                ViewBag.loss_control_del = String.IsNullOrWhiteSpace(ir.loss_control_approve) == false ? db.employees.Find(Int32.Parse(String.IsNullOrWhiteSpace(ir.loss_control) ? "0" : ir.loss_control)).employee_delegate : null;
                ViewBag.field_manager_del = String.IsNullOrWhiteSpace(ir.field_manager_approve) == false ? db.employees.Find(Int32.Parse(String.IsNullOrWhiteSpace(ir.field_manager) ? "0" : ir.field_manager)).employee_delegate : null;

                bool isCanEdit = false;
                string employeeId = Session["id"].ToString();
                employee employeeDelegation = new employee();
                if (employeeId == ir.prepared_by && ir.supervisor_approve == null)
                {
                    isCanEdit = true;
                }
                
                if (employeeId == ir.ack_supervisor && ir.supervisor_approve == null)
                {
                    isCanEdit = true;
                }
                
                if (employeeId == ir.superintendent && ir.superintendent_approve == null && ir.supervisor_approve != null)
                {
                    isCanEdit = true;
                }
                
                if (employeeId == ir.loss_control && ir.loss_control_approve == null && ir.superintendent_approve != null)
                {
                    isCanEdit = true;
                }
                
                if (employeeId == ir.she_superintendent && ir.she_superintendent_approve == null && ir.loss_control_approve != null)
                {
                    isCanEdit = true;
                }
                
                if (employeeId == ir.field_manager && ir.field_manager_approve == null && ir.she_superintendent_approve != null)
                {
                    isCanEdit = true;
                }

                if (isCanEdit == false)
                {
                    employeeDelegation = db.employees.Find(Int32.Parse(ir.ack_supervisor));
                    if (employeeId == employeeDelegation.employee_delegate.ToString() && ir.supervisor_approve == null)
                    {
                        isCanEdit = true;
                    }
                    
                    if (isCanEdit == false && employeeId == (employeeDelegation = db.employees.Find(Int32.Parse(ir.superintendent))).employee_delegate.ToString() && ir.superintendent_approve == null && ir.supervisor_approve != null)
                    {
                        isCanEdit = true;
                    }
                    
                    if (isCanEdit == false && employeeId == (employeeDelegation = db.employees.Find(Int32.Parse(ir.loss_control))).employee_delegate.ToString() && ir.loss_control_approve == null && ir.superintendent_approve != null)
                    {
                        isCanEdit = true;
                    }
                    
                    if (isCanEdit == false && employeeId == (employeeDelegation = db.employees.Find(Int32.Parse(ir.she_superintendent))).employee_delegate.ToString() && ir.she_superintendent_approve == null && ir.loss_control_approve != null)
                    {
                        isCanEdit = true;
                    }
                    
                    if (isCanEdit == false && employeeId == (employeeDelegation = db.employees.Find(Int32.Parse(ir.field_manager))).employee_delegate.ToString() && ir.field_manager_approve == null && ir.she_superintendent_approve != null)
                    {
                        isCanEdit = true;
                    }
                }

                ViewBag.isCanEdit = isCanEdit;
            }
            else
            {
                int cur_user_id = Int32.Parse(HttpContext.Session["id"].ToString());
                employee curUser = db.employees.Find(cur_user_id);
                int? cur_dept_id = curUser.dept_id;
                int? cur_user_del = curUser.employee_delegate;
                employee cur_user_boss = db.employees.Find(cur_user_id).employee2;
                int? superintendent_id = null;
                int? superintendent_id_del = null;
                int? supervisor_id = null;
                int? supervisor_id_del = null;
                string supervisor_position = null;
                while (cur_user_boss != null)
                {
                    if (cur_user_boss.approval_level == 1)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            supervisor_id = cur_user_boss.id;
                            supervisor_id_del = cur_user_boss.employee_delegate;
                            supervisor_position = cur_user_boss.position;
                        }
                        else
                        {
                            supervisor_id = cur_user_boss.id;
                        }
                    }
                    else if (supervisor_id == null && cur_user_boss.approval_level == 2)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            supervisor_id = cur_user_boss.id;
                            supervisor_id_del = cur_user_boss.employee_delegate;
                            supervisor_position = cur_user_boss.position;
                        }
                        else
                        {
                            supervisor_id = cur_user_boss.id;
                        }

                        if (cur_user_boss.delagate == 1)
                        {
                            superintendent_id = cur_user_boss.id;
                            superintendent_id_del = cur_user_boss.employee_delegate;
                        }
                        else
                        {
                            superintendent_id = cur_user_boss.id;
                        }
                    }
                    else if (cur_user_boss.approval_level == 2)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            superintendent_id = cur_user_boss.id;
                            superintendent_id_del = cur_user_boss.employee_delegate;
                        }
                        else
                        {
                            superintendent_id = cur_user_boss.id;
                        }
                    }
                    else if (supervisor_id == null && superintendent_id == null && cur_user_boss.employee2 == null)
                    {
                        if (cur_user_boss.delagate == 1)
                        {
                            supervisor_id = cur_user_boss.id;
                            supervisor_id_del = cur_user_boss.employee_delegate;
                            supervisor_position = cur_user_boss.position;
                        }
                        else
                        {
                            supervisor_id = cur_user_boss.id;
                        }

                        if (cur_user_boss.delagate == 1)
                        {
                            superintendent_id = cur_user_boss.id;
                            superintendent_id_del = cur_user_boss.employee_delegate;
                        }
                        else
                        {
                            superintendent_id = cur_user_boss.id;
                        }
                    }
                    
                    cur_user_boss = cur_user_boss.employee2;
                }
                if (supervisor_id == null)
                {
                    supervisor_id = cur_user_id;
                    supervisor_id_del = cur_user_del;
                }
                if (superintendent_id == null) 
                { 
                    superintendent_id = supervisor_id; 
                    superintendent_id_del = supervisor_id_del; 
                }
                ViewBag.superintendent_id = superintendent_id;
                ViewBag.supervisor_id = supervisor_id;
                ViewBag.superintendent_id_del = superintendent_id_del;
                ViewBag.supervisor_id_del = supervisor_id_del;
                ViewBag.supervisor_position = supervisor_position;
                ViewBag.id_fracas = id_fracas;
                ViewBag.id_injury = id_injury;
                ViewBag.id_fracas_part = id_fracas_part;

                ViewBag.isCanEdit = true;
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
            ViewData["total"] = count;
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
                i.inves = i.investigation == 1 ? "Yes" : "No";
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

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _CustomBinding(GridCommand command)
        {
            var dataContext = new relmon_star_energiEntities();
            string idLogin = Session["id"].ToString();
            IEnumerable data = GetData(command, idLogin);
            string employeeId = idLogin;
            foreach (IREntity ir in data)
            {
                bool isCanEdit = false;
                employee employeeDelegation = new employee();
                if (employeeId == ir.prepared_by && ir.supervisor_approve == null)
                {
                    isCanEdit = true;
                }

                if (employeeId == ir.ack_supervisor && ir.supervisor_approve == null)
                {
                    isCanEdit = true;
                }

                if (employeeId == ir.superintendent && ir.superintendent_approve == null && ir.supervisor_approve != null)
                {
                    isCanEdit = true;
                }

                if (employeeId == ir.loss_control && ir.loss_control_approve == null && ir.superintendent_approve != null)
                {
                    isCanEdit = true;
                }

                if (employeeId == ir.she_superintendent && ir.she_superintendent_approve == null && ir.loss_control_approve != null)
                {
                    isCanEdit = true;
                }

                if (employeeId == ir.field_manager && ir.field_manager_approve == null && ir.she_superintendent_approve != null)
                {
                    isCanEdit = true;
                }

                if (isCanEdit == false)
                {
                    employeeDelegation = dataContext.employees.Find(Int32.Parse(ir.ack_supervisor));
                    if (employeeId == employeeDelegation.employee_delegate.ToString() && ir.supervisor_approve == null)
                    {
                        isCanEdit = true;
                    }

                    if (isCanEdit == false && employeeId == (employeeDelegation = dataContext.employees.Find(Int32.Parse(ir.superintendent))).employee_delegate.ToString() && ir.superintendent_approve == null && ir.supervisor_approve != null)
                    {
                        isCanEdit = true;
                    }

                    if (isCanEdit == false && employeeId == (employeeDelegation = dataContext.employees.Find(Int32.Parse(ir.loss_control))).employee_delegate.ToString() && ir.loss_control_approve == null && ir.superintendent_approve != null)
                    {
                        isCanEdit = true;
                    }

                    if (isCanEdit == false && employeeId == (employeeDelegation = dataContext.employees.Find(Int32.Parse(ir.she_superintendent))).employee_delegate.ToString() && ir.she_superintendent_approve == null && ir.loss_control_approve != null)
                    {
                        isCanEdit = true;
                    }

                    if (isCanEdit == false && employeeId == (employeeDelegation = dataContext.employees.Find(Int32.Parse(ir.field_manager))).employee_delegate.ToString() && ir.field_manager_approve == null && ir.she_superintendent_approve != null)
                    {
                        isCanEdit = true;
                    }
                }

                ir.isCanEdit = isCanEdit;   
            }
            return View(new GridModel
            {
                Data = data,
                Total = count
            });
        }

        private static IEnumerable GetData(GridCommand command, string idLogin)
        {
            var dataContext = new relmon_star_energiEntities();
            IQueryable<IREntity> data = (from p in dataContext.incident_report
                                                join rcass in dataContext.rcas
                                                on p.id_rca equals rcass.id into pr
                                                from ir_rca in pr.DefaultIfEmpty()
                                                join employeei in dataContext.employees
                                                on p.prepared_by equals SqlFunctions.StringConvert((double)employeei.id).Trim() into ps
                                                from emp in ps.DefaultIfEmpty()
                                                join tsr in dataContext.trouble_shooting
                                                on p.id_tsr equals tsr.id into pt
                                                from ir_tsr in pt.DefaultIfEmpty()
                                                select new IREntity
                                                {
                                                    id = p.id,
                                                    facility = p.facility,
                                                    incident_location = p.incident_location,
                                                    reference_number = p.reference_number,
                                                    type_of_report = p.type_of_report,
                                                    date_incident = p.date_incident,
                                                    title = p.title,
                                                    incident_type = p.incident_type,
                                                    actual_loss_severity = p.actual_loss_severity,
                                                    potential_loss_severity = p.potential_loss_severity,
                                                    probability = p.probability,
                                                    factual_information = p.factual_information,
                                                    cost_estimate = p.cost_estimate,
                                                    immediate_action = p.immediate_action,
                                                    ack_supervisor = p.ack_supervisor,
                                                    prepare_date = p.prepare_date,
                                                    ack_date = p.ack_date,
                                                    superintendent = p.superintendent,
                                                    loss_control = p.loss_control,
                                                    field_manager = p.field_manager,
                                                    she_superintendent = p.she_superintendent,
                                                    superintendent_date = p.superintendent_date,
                                                    loss_date = p.loss_date,
                                                    field_manager_date = p.field_manager_date,
                                                    she_superintendent_date = p.she_superintendent_date,
                                                    investigation = p.investigation,
                                                    investigation_req = p.investigation_req,
                                                    id_rca = p.id_rca,
                                                    superintendent_approve = p.superintendent_approve,
                                                    field_manager_approve = p.field_manager_approve,
                                                    loss_control_approve = p.loss_control_approve,
                                                    she_superintendent_approve = p.she_superintendent_approve,
                                                    lead_name = p.lead_name,
                                                    superintendent_delegate = p.superintendent_delegate,
                                                    field_manager_delegate = p.field_manager_delegate,
                                                    loss_control_delegate = p.loss_control_delegate,
                                                    she_superintendent_delegate = p.she_superintendent_delegate,
                                                    supervisor_approve = p.supervisor_approve,
                                                    supervisor_delegate = p.supervisor_delegate,
                                                    kontraktor_seg = p.kontraktor_seg,
                                                    prepared_by = p.prepared_by,
                                                    rca_number = ir_rca.rca_code != null ? ir_rca.rca_code : "",
                                                    inves = p.investigation == 1 ? "Yes" : "No",
                                                    type_report = p.type_of_report == 1 ? "On the job" : p.type_of_report == 0 ? "Off the job" : "",
                                                    actual_loss = p.actual_loss_severity == 1 ? "Major" : p.actual_loss_severity == 2 ? "Serious" : p.actual_loss_severity == 3 ? "Moderate" : p.actual_loss_severity == 4 ? "Minor" : "",
                                                    potential_loss = p.potential_loss_severity == 1 ? "Major" : p.potential_loss_severity == 2 ? "Serious" : p.potential_loss_severity == 3 ? "Moderate" : p.potential_loss_severity == 4 ? "Minor" : "",
                                                    probability_str = p.probability == 1 ? "Frequent" : p.probability == 2 ? "Occasional" : p.probability == 3 ? "Seldom" : p.probability == 4 ? "Rare" : "",
                                                    tsr_number = ir_tsr.no != null ? ir_tsr.no : "",
                                                    prepared_by_name = emp.alpha_name
                                                });
            //data = data.Where(p => p.prepared_by == idLogin || p.ack_supervisor == idLogin || p.superintendent == idLogin || p.loss_control == idLogin || p.she_superintendent == idLogin || p.field_manager == idLogin ||
            //    p.supervisor_delegate == idLogin || p.superintendent_delegate == idLogin || p.loss_control_delegate == idLogin || p.she_superintendent_delegate == idLogin || p.field_manager_delegate == idLogin);
            data = data.ApplyFiltering(command.FilterDescriptors);
            count = data.FirstOrDefault() == null ? 0 : data.Count();
            //Apply sorting
            data = data.ApplySorting(command.GroupDescriptors, command.SortDescriptors);
            //Apply paging
            data = data.ApplyPaging(command.Page, command.PageSize);
            //Apply grouping
            if (command.GroupDescriptors.Any())
            {
                return data.ApplyGrouping(command.GroupDescriptors);
            }

            List<IREntity> retVal = data.FirstOrDefault() == null ? new List<IREntity>() : data.ToList();
            return retVal;
        }
        private static int GetCount()
        {
            using (relmon_star_energiEntities dataContext = new relmon_star_energiEntities())
            {
                return dataContext.incident_report.Count();
            }
        }



        [HttpPost]
        public JsonResult Add(incident_report incidentReport, int? id_fracas, int? id_injury, int? id_fracas_part)
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
            string sign = db.employees.Find(Int32.Parse(incidentReport.prepared_by)).signature;
            incidentReport.requestor_approve = "a" + sign;
            db.incident_report.Add(incidentReport);
            db.SaveChanges();

            //SEND TO NEXT LEVEL
            if (incidentReport.ack_supervisor != null && incidentReport.ack_supervisor != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.ack_supervisor), "Please Approve " + incidentReport.reference_number);
            }
            if (incidentReport.supervisor_delegate != null && incidentReport.supervisor_delegate != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.supervisor_delegate), "Please Approve "+ incidentReport.reference_number);
            }

            // add link to fracas
            if (id_fracas != null)
            {
                equipment_event fracas = db.equipment_event.Find(id_fracas);
                fracas.id_ir = incidentReport.id;
                db.Entry(fracas).State = EntityState.Modified;
                db.SaveChanges();
            }

            // add link to fracas
            if (id_fracas_part != null)
            {
                part_unit_event fracas = db.part_unit_event.Find(id_fracas_part);
                fracas.id_ir = incidentReport.id;
                db.Entry(fracas).State = EntityState.Modified;
                db.SaveChanges();
            }

            // add link to Injury
            if (id_injury != null)
            {
                she_injury_report injury = db.she_injury_report.Find(id_injury);
                injury.id_ir = incidentReport.id;
                db.Entry(injury).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            //send email
            SendEmailToAll(incidentReport);

            int id = db.incident_report.Max(p => p.id);
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Create new Incident Report",
                date = DateTime.Now
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
            //ir.superintendent_delegate = incidentReport.superintendent_delegate;
            //ir.she_superintendent_delegate = incidentReport.she_superintendent_delegate;
            //ir.loss_control_delegate = incidentReport.loss_control_delegate;
            //ir.field_manager_delegate = incidentReport.field_manager_delegate;
            //ir.supervisor_delegate = incidentReport.supervisor_delegate;
            ir.kontraktor_seg = incidentReport.kontraktor_seg;

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

        [HttpPost]
        public ActionResult selectSupervisor(int id, int employee_id)
        {
            employee emp = db.employees.Find(employee_id);
            bool delegates = emp.delagate == 1;

            return Json(new { success = true, delegates = delegates, delegate_name = emp.employee_delegate });
        }

        #region approval

        [HttpPost]
        public ActionResult approveLossControl(int id, int employee_id, DateTime date)
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
                    ir.loss_control_delegate = employee_id.ToString();
                    ir.loss_control_approve = "d" + sign;
                }
                ir.loss_date = date;
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

                if (ir.she_superintendent != null && ir.she_superintendent_delegate == null)
                    SendEmailApprove(ir, Int32.Parse(ir.she_superintendent));
                else if (ir.she_superintendent_delegate != null)
                    SendEmailApprove(ir, Int32.Parse(ir.she_superintendent_delegate));

                //SEND TO NEXT LEVEL
                if (ir.she_superintendent != null && ir.she_superintendent != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.she_superintendent), "Please Approve "+ ir.reference_number);
                }
                if (ir.she_superintendent_delegate != null && ir.she_superintendent_delegate != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.she_superintendent_delegate), "Please Approve " + ir.reference_number);
                }


                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveSuperintendent(int id, int employee_id, DateTime date)
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
                    ir.superintendent_delegate = employee_id.ToString();
                    ir.superintendent_approve = "d" + sign;
                }
                ir.superintendent_date = date;
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

                if (ir.loss_control != null && ir.loss_control_delegate == null)
                    SendEmailApprove(ir, Int32.Parse(ir.loss_control));
                else if (ir.loss_control_delegate != null)
                    SendEmailApprove(ir, Int32.Parse(ir.loss_control_delegate));

                //SEND TO NEXT LEVEL
                if (ir.loss_control != null && ir.loss_control != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.loss_control), "Please Approve " + ir.reference_number);
                }
                if (ir.loss_control_delegate != null && ir.loss_control_delegate !="")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.loss_control_delegate), "Please Approve " + ir.reference_number);
                }
              
                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false});
            }
            
        }

        [HttpPost]
        public ActionResult approveSupervisor(int id, int employee_id, DateTime date)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                incident_report ir = db.incident_report.Find(id);
                if (ir.ack_supervisor == employee_id.ToString())
                {
                    ir.supervisor_approve = "a" + sign;
                }
                else
                {
                    ir.supervisor_delegate = employee_id.ToString();
                    ir.supervisor_approve = "d" + sign;
                }
                ir.ack_date = date;
                db.Entry(ir).State = EntityState.Modified;
                db.SaveChanges();
                incident_report_log ir_log = new incident_report_log
                {
                    id_ir = id,
                    username = HttpContext.Session["username"].ToString(),
                    status = "Approved by Supervisor",
                    date = DateTime.Now
                };
                db.incident_report_log.Add(ir_log);
                db.SaveChanges();

                if (ir.superintendent != null && ir.superintendent_delegate == null)
                    SendEmailApprove(ir, Int32.Parse(ir.superintendent));
                else if (ir.superintendent_delegate != null)
                    SendEmailApprove(ir, Int32.Parse(ir.superintendent_delegate));

                //SEND TO NEXT LEVEL
                if (ir.superintendent != null && ir.superintendent != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.superintendent), "Please Approve " + ir.reference_number);
                }
                if (ir.superintendent_delegate != null && ir.superintendent_delegate != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.superintendent_delegate), "Please Approve " + ir.reference_number);
                }

                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }

        }



        [HttpPost]
        public ActionResult approveFieldManager(int id, int employee_id, DateTime date)
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
                    ir.field_manager_delegate = employee_id.ToString();
                    ir.field_manager_approve = "d" + sign;
                }
                ir.field_manager_date = date;
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

                //SEND TO INVESTIGATION LEAD
                if (ir.lead_name != null && ir.lead_name != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.lead_name), "Please Make RCA",true);
                }

                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public ActionResult approveSheSuperintendent(int id, int employee_id, DateTime date)
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
                    ir.she_superintendent_delegate = employee_id.ToString();
                    ir.she_superintendent_approve = "d" + sign;
                }
                ir.she_superintendent_date = date;
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

                if (ir.field_manager != null && ir.field_manager_delegate == null)
                    SendEmailApprove(ir, Int32.Parse(ir.field_manager));
                else if (ir.field_manager_delegate != null)
                    SendEmailApprove(ir, Int32.Parse(ir.field_manager_delegate));
                
                //SEND TO NEXT LEVEL
                if (ir.field_manager != null && ir.field_manager != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.field_manager), "Please Approve " + ir.reference_number);
                }
                if (ir.field_manager_delegate != null && ir.field_manager_delegate != "")
                {
                    this.SendUserNotification(ir, Int32.Parse(ir.field_manager_delegate), "Please Approve " + ir.reference_number);
                }

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

            incidentReport.supervisor_approve = null;
            incidentReport.supervisor_delegate = null;
            db.Entry(incidentReport).State = EntityState.Modified;
            db.SaveChanges();
            SendEmailToAll(incidentReport, 2, comment, 2);

            if (incidentReport.ack_supervisor != null && incidentReport.ack_supervisor != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.ack_supervisor), incidentReport.reference_number + " is rejected with comment: " + comment);
            }
            if (incidentReport.supervisor_delegate != null && incidentReport.supervisor_delegate != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.supervisor_delegate), incidentReport.reference_number + " is rejected with comment: " + comment);
            }
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectSupervisor(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            
            incident_report_log ir_log = new incident_report_log
            {
                id_ir = id,
                username = HttpContext.Session["username"].ToString(),
                status = "Rejected by Supervisor",
                comment = comment,
                date = DateTime.Now
            };
            db.incident_report_log.Add(ir_log);
            db.SaveChanges();

            SendEmailToAll(incidentReport, 2, comment, 1);

            if (incidentReport.prepared_by != null && incidentReport.prepared_by != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.prepared_by), incidentReport.reference_number + " is rejected with comment: " + comment);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult rejectLossControl(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            
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

            incidentReport.superintendent_approve = null;
            incidentReport.superintendent_delegate = null;
            db.Entry(incidentReport).State = EntityState.Modified;
            db.SaveChanges();
            SendEmailToAll(incidentReport, 2, comment, 3);

            if (incidentReport.superintendent != null && incidentReport.superintendent != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.superintendent), incidentReport.reference_number + " is rejected with comment: " + comment);
            }
            if (incidentReport.superintendent_delegate != null && incidentReport.superintendent_delegate != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.superintendent_delegate), incidentReport.reference_number + " is rejected with comment: " + comment);
            }

            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectSheSuperintendent(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            
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

            incidentReport.loss_control_approve = null;
            incidentReport.loss_control_delegate = null;
            db.Entry(incidentReport).State = EntityState.Modified;
            db.SaveChanges();
            SendEmailToAll(incidentReport, 2, comment, 4);

            if (incidentReport.loss_control != null && incidentReport.loss_control != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.loss_control), incidentReport.reference_number + " is rejected with comment: " + comment);
            }
            if (incidentReport.loss_control_delegate != null && incidentReport.loss_control_delegate != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.loss_control_delegate), incidentReport.reference_number + " is rejected with comment: " + comment);
            }

            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectFieldManager(int id, string comment)
        {
            incident_report incidentReport = db.incident_report.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            
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

            incidentReport.she_superintendent_approve = null;
            incidentReport.she_superintendent_delegate = null;
            db.Entry(incidentReport).State = EntityState.Modified;
            db.SaveChanges();
            SendEmailToAll(incidentReport, 2, comment, 5);

            if (incidentReport.she_superintendent != null && incidentReport.she_superintendent != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.she_superintendent), incidentReport.reference_number + " is rejected with comment: " + comment);
            }
            if (incidentReport.she_superintendent_delegate != null && incidentReport.she_superintendent_delegate != "")
            {
                this.SendUserNotification(incidentReport, Int32.Parse(incidentReport.she_superintendent_delegate), incidentReport.reference_number + " is rejected with comment: " + comment);
            }

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
                supervisor_approve = ir.supervisor_approve,
                requestor_approve = ir.requestor_approve
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

            return Json(new { id = id, analysis_title = i.title, cost = i.cost_estimate, lead_name = i.lead_name });
        }

        public ActionResult ExportExcelData(DateTime fromD, DateTime toD)
        {
            List<incident_report_e> result = new List<incident_report_e>();
            var r = (from ir in db.incident_report
                     where ir.prepare_date >= fromD && ir.prepare_date <= toD
                     select new incident_report_e
                     {
                         prepare_date = ir.prepare_date,
                         date_incident = ir.date_incident,
                         cost_estimate = ir.cost_estimate,
                         prepared_by = ir.prepared_by,
                         title = ir.title,
                         incident_type = ir.incident_type,
                         reference_number = ir.reference_number
                     }).OrderBy(p => p.date_incident).ToList();
            result = r;
            foreach (incident_report_e ire in result)
            {
                if (ire.prepared_by != null && ire.prepared_by != "")
                    ire.prepared_by = db.employees.Find(Int32.Parse(ire.prepared_by)).alpha_name;
            }
            GridView gv = new GridView();
            gv.Caption = "Incident Report Data From " + fromD.ToShortDateString() + " To " + toD.ToShortDateString();
            gv.DataSource = result;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Incident Report No.";
            gv.HeaderRow.Cells[1].Text = "Date";
            gv.HeaderRow.Cells[2].Text = "Date of Incident";
            gv.HeaderRow.Cells[3].Text = "Description";
            gv.HeaderRow.Cells[4].Text = "Type";
            gv.HeaderRow.Cells[5].Text = "Value";
            gv.HeaderRow.Cells[6].Text = "Person By";
            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "incident report data.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        public ActionResult ExportExcelTotal(DateTime from, DateTime to)
        {
            List<incident_report_r> result = new List<incident_report_r>();
            List<incident_report> list_ir = db.incident_report.Where(p => p.incident_type == "Injury / Illness").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            double total_cost = 0;
            double cost_kon = 0;
            double cost_seg = 0;
            foreach(incident_report ir in list_ir) {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Injury / Illness", cases = list_ir.Count, total_cost = total_cost , cost_kon = cost_kon, cost_seg = cost_seg});
            list_ir = db.incident_report.Where(p => p.incident_type == "Environmental Loss").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Environmental Loss", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "Properti Damage").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Properti Damage", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "Process Loss / Disturb").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Process Loss / Disturb", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "External Relation").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "External Relation", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "Theft / Crimes").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Theft / Crimes", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "Vehicular").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Vehicular", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type == "Near Miss").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Near Miss", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            list_ir = db.incident_report.Where(p => p.incident_type != "Injury / Illness" && p.incident_type != "Environmental Loss" && p.incident_type != "Properti Damage"
                && p.incident_type != "Process Loss / Disturb" && p.incident_type != "External Relation" && p.incident_type != "Theft / Crimes" && p.incident_type != "Vehicular" && p.incident_type != "Near Miss").Where(p => p.prepare_date >= from && p.prepare_date <= to).ToList();
            total_cost = 0;
            cost_kon = 0;
            cost_seg = 0;
            foreach (incident_report ir in list_ir)
            {
                if (ir.cost_estimate != null && ir.cost_estimate != "")
                {
                    total_cost += double.Parse(ir.cost_estimate);
                    if (ir.kontraktor_seg == 1)
                    {
                        cost_kon += double.Parse(ir.cost_estimate);
                    }
                    else if (ir.kontraktor_seg == 2)
                    {
                        cost_seg += double.Parse(ir.cost_estimate);
                    }
                }
            }
            result.Add(new incident_report_r { type = "Other", cases = list_ir.Count, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            int total_case = result.Sum(p => p.cases);
            total_cost = result.Sum(p => p.total_cost);
            cost_kon = result.Sum(p => p.cost_kon);
            cost_seg = result.Sum(p => p.cost_seg);
            result.Add(new incident_report_r { type = "Total", cases = total_case, total_cost = total_cost, cost_kon = cost_kon, cost_seg = cost_seg });
            GridView gv = new GridView();
            gv.Caption = "Incident Report";
            gv.DataSource = result;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Type of Incident";
            gv.HeaderRow.Cells[1].Text = "No. of Cases";
            gv.HeaderRow.Cells[2].Text = "Company (USD)";
            gv.HeaderRow.Cells[3].Text = "Contractor (USD)";
            gv.HeaderRow.Cells[4].Text = "Total Loss (USD)";
            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "incident report.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        public void SendEmailApprove(incident_report ir, int employeeId) {
            var sendEmail = new SendEmailController();
            employee e = db.employees.Find(employeeId);
            if (e.email != null)
            {
                List<string> s = new List<string>();
                s.Add(e.email);
                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Report dengan nomor referensi " + ir.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Report with reference number " + ir.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br /> Sytem Fracas Application", "Approving Incident Report " + ir.reference_number);
            }
        }

        //reject type = 2
        public void SendEmailToAll(incident_report incidentReport, int type = 1, string comment = "", int rejectType = 0)
        {
            employee e;
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (incidentReport.superintendent != null && incidentReport.superintendent_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.superintendent));
                if (e.email != null)
                    s.Add(e.email);

            }
            else if (incidentReport.superintendent_delegate != null)
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
            else if (incidentReport.field_manager_delegate != null)
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
            else if (incidentReport.loss_control_delegate != null)
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
            else if (incidentReport.she_superintendent_delegate != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.she_superintendent_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (incidentReport.ack_supervisor != null && incidentReport.supervisor_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.ack_supervisor));
                if (e.email != null)
                    s.Add(e.email);

            }
            else if (incidentReport.supervisor_delegate != null)
            {
                e = db.employees.Find(Int32.Parse(incidentReport.supervisor_delegate));
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
            {
                if (type == 1)
                    sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Incident Report dengan nomor referensi " + incidentReport.reference_number + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Incident Report with reference number " + incidentReport.reference_number + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(incidentReport.prepared_by)).alpha_name, "Approving Incident Report " + incidentReport.reference_number);
                else
                    sendEmail.Send(s, "Bapak/Ibu,<br />Dokumen berikut dengan no referensi " + incidentReport.reference_number + " kami kembalikan untuk diperbaiki sesuai dengan alasan di bawah.Terima Kasih.<br />Alasan :<br />" + comment + "<br /><br /><i>Dear Sir/Madam,<br />Document with reference number " + incidentReport.reference_number + " need to be reviewed in accordance with the following reasons .Thank you.<br />Reasons :<br />" + comment + "</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(HttpContext.Session["id"].ToString())).alpha_name, "Rejected Incident Report " + incidentReport.reference_number);
            }
        }

        private void SendUserNotification(incident_report data, int sendUserId,string message, bool isInvestigationLead=false)
        {
            WWService.UserServiceClient client = new WWService.UserServiceClient();
            if (isInvestigationLead == true)
            {
                WWService.ResponseModel response = client.CreateNotification(
                EncodeMd5("starenergyww"),
                sendUserId,
                System.Configuration.ConfigurationManager.AppSettings["ApplicationName"],
                "SHE Incident Report",
                message,
                "/NotificationUrlResolver/FRACAS?name=SHE_INCIDENT_REPORT_NEW_INVESTIGATION&id=0");
            }
            else
            {
                WWService.ResponseModel response = client.CreateNotification(
                EncodeMd5("starenergyww"),
                sendUserId,
                System.Configuration.ConfigurationManager.AppSettings["ApplicationName"],
                "SHE Incident Report",
                message,
                "/NotificationUrlResolver/FRACAS?name=SHE_INCIDENT_REPORT&id=" + data.id);
            }
            
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
