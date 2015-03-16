using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Collections.Specialized;
using StarEnergi.Utilities;
using System.Data;
using ReportManagement;
using System.IO;
using System.Diagnostics;
using System.Web.UI;
using StarEnergi.Controllers.Utilities;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace StarEnergi.Controllers.FrontEnd
{
    [Authorize]
    public class PirController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private List<user_per_role> li;

        #region page load
        //
        // GET: /Pir/
        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/pir" });
            }
            else
            {
                string username = Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!(li.Exists(p => p.role == (int)Config.role.PIRINITIATOR)) && !(li.Exists(p => p.role == (int)Config.role.PIRPROCESS)) && !(li.Exists(p => p.role == (int)Config.role.FULLPIR)) && !(li.Exists(p => p.role == (int)Config.role.AUDITOR)))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/pir" });
                }

                ViewBag.stat = "pir";

                if (li.Exists(p => p.role == (int)Config.role.PIRINITIATOR || p.role == (int)Config.role.FULLPIR || p.role == (int)Config.role.AUDITOR))
                {
                    List<pir> list_pir = db.pirs.ToList();
                    int totalPir = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year).ToList().Count;
                    int totalOverdue = list_pir.Where(a => a.target_completion_init != null && a.target_completion_init.Value.CompareTo(DateTime.Today) < 0).ToList().Count;
                    int totalCompleteOnTime = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status == "VERIFIED" && a.sign_date_verified.Value.CompareTo(a.target_completion_init.Value) <= 0).ToList().Count;
                    double pir_target = db.she_KPI_target.Find(2).target != null ? db.she_KPI_target.Find(2).target.Value : 0;
                    int totalClose = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status == "VERIFIED").ToList().Count;
                    int totalOpen = list_pir.Where(a => a.target_completion_init != null && a.target_completion_init.Value.CompareTo(DateTime.Today) >= 0).ToList().Count;
                    int totalRaised = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status != "VERIFIED").ToList().Count;
                    if (((double)totalCompleteOnTime / (double)totalPir) * (double)100 > pir_target || totalPir == 0)
                    {
                        ViewBag.success = true;
                    }
                    else
                    {
                        ViewBag.success = false;

                    }
                    ViewBag.totalPir = totalPir;
                    ViewBag.totalOverdue = totalOverdue;
                    ViewBag.pir_target = pir_target;
                    ViewBag.totalCompleteOnTime = totalCompleteOnTime;
                    ViewBag.totalClose = totalClose;
                    ViewBag.totalOpen = totalOpen;
                    ViewBag.totalRaised = totalRaised;
                    ViewBag.user_role = li;
                    
                    ViewBag.nama = "Performance Improvement Request Inititator";
                    return View("ListInitiator");
                }
                else
                {
                    ViewBag.header = true;
                    ViewBag.nama = "Performance Improvement Request Process";
                    return View("ListProcess");
                }
            }
        }

        public ActionResult ListProcess()
        {
            ViewBag.nama = "Performance Improvement Request Process";
            return PartialView();
        }


        //load 
        //identity 0 rca, 1 iir
        public ActionResult pageInitiator(int identity=-1, int id = -1, int idRca = -1, string requester = null, string reference = null, string finding = null)
        {
            var processUserList = new List<employee>();
            var query = (from a in db.employees
                         orderby a.alpha_name
                         select new {a.id, a.alpha_name, a.position, a.kbp_code }).ToList();
            foreach (var a in query)
            {
                //if (a.position.ToLower().Contains("superintendent"))
                processUserList.Add(new employee { alpha_name = a.alpha_name + (a.kbp_code != null ? (" - " + a.kbp_code) : ""), id = a.id });
            }

            ViewBag.idRca = idRca;
            ViewBag.identity = identity;
            ViewBag.reference = reference;
            ViewBag.finding = finding;
            if (id == -1)
            {
                var has = (from employees in db.employees
                           join dept in db.employee_dept on employees.dept_id equals dept.id
                           join users in db.users on employees.id equals users.employee_id into user_employee
                           from ue in user_employee.DefaultIfEmpty()
                           orderby employees.dept_id
                           select new EmployeeEntity
                           {
                               id = employees.id,
                               alpha_name = employees.alpha_name + (employees.kbp_code != null ? (" - " + employees.kbp_code) : ""),
                               employee_no = employees.employee_no,
                               position = employees.position,
                               work_location = employees.work_location,
                               dob = employees.dob,
                               dept_name = dept.dept_name,
                               username = ue.username,
                               employee = employees.employee2,
                               approval_level = employees.approval_level,
                               kbp_code = employees.kbp_code
                           }).ToList();
                ViewBag.requester = requester != null ? has.Where(p => p.id == Int32.Parse(requester)).ToList().FirstOrDefault().username : null;

                //add file initiator
                string pirNumber = db.pirs.Where(x => x.no != null).ToList().Count() == 0 ? "" : db.pirs.Where(x => x.no != null).OrderByDescending(x => x.id).First().no;
                pirNumber = MyTools.generatePirNumber(pirNumber);
                pir pir = new pir();
                pir.no = pirNumber;
                pir.improvement_request = finding;
                pir.from = (Byte)identity;
                if (identity == 1)
                {
                    int? iir_id = db.iir_recommendations.Find(idRca).id_iir;
                    pir.reference = db.investigation_report.Find(iir_id).reference_number;
                }
                var r = (from clause in db.pir_clause
                         select new PirClauseEntity
                         {
                             id = clause.id,
                             clause_no = clause.clause_no,
                             clause = clause.clause
                         }).ToList();
                Dictionary<string, string> procedure_reference = new Dictionary<string, string>();
                foreach (PirClauseEntity clause in r)
                {
                    procedure_reference.Add(clause.clause_no + " " + clause.clause, clause.clause_no + " " + clause.clause);
                }
                ViewBag.procedure_reference = new SelectList(procedure_reference, "Key", "Value");
                ViewBag.process_user = new SelectList(processUserList, "id", "alpha_name");
                int last_id = db.pirs.ToList().Count == 0 ? 0 : db.pirs.Max(p => p.id);
                last_id++;
                string subPath = "~/Attachment/pir/" + last_id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

                return PartialView(pir);
            }
            else
            {
                pir pir;
                ProcedureReferenceList prl = new ProcedureReferenceList();
                pir = db.pirs.Find(id);
                var r = (from clause in db.pir_clause
                         select new PirClauseEntity
                         {
                             id = clause.id,
                             clause_no = clause.clause_no,
                             clause = clause.clause
                         }).ToList();
                Dictionary<string, string> procedure_reference = new Dictionary<string, string>();
                foreach (PirClauseEntity clause in r)
                {
                    procedure_reference.Add(clause.clause_no + " " + clause.clause, clause.clause_no + " " + clause.clause);
                }
                ViewBag.procedure_reference = new SelectList(procedure_reference, "Key", "Value",pir.procedure_reference);
                ViewBag.process_user = new SelectList(processUserList, "id", "alpha_name", pir.process_user);
                //edit file initiator
                return PartialView(pir);
            }

        }

        //load 
        public ActionResult pageProcess(int id = -1)
        {
            var processOwnerList = new List<process_owner>();
            var query = (from a in db.process_owner
                         select a).ToList();
            foreach (var a in query)
            {
               processOwnerList.Add(new process_owner { Id = a.Id, Name = a.Name });
            }

            pir pir = db.pirs.Find(id);
            //edit file process
            if (pir.process_owner != null)
            {
                ViewBag.process_owner = new SelectList(processOwnerList, "Name", "Name", pir.process_owner);
            }
            else
            {
                ViewBag.process_owner = new SelectList(processOwnerList, "Name", "Name");
            }
            return PartialView(pir);

        }
        #endregion

        #region binding
        //
        //Select ajax binding
        [GridAction]
        public ActionResult _SelectAjaxEditing(int? id)
        {
            int a = id == null ? 0 : id.Value;
            return binding(a);
        }

        //
        //Select ajax binding my task
        [GridAction]
        public ActionResult _SelectAjaxEditingTask()
        {
            return bindingTask();
        }

        //select rca task
        private ViewResult bindingTask()
        {
            string username = Session["username"].ToString();
            List<int> id_principals = db.rca_team_connector.Where(a => a.id_user == username).Select(b => b.id).ToList();
            var model = from p in db.rcas
                        where id_principals.Contains((int)p.id_team) && (p.has_pir == 1)
                        select p;

            List<RCAEntityModel> ret = new List<RCAEntityModel>();
            foreach(rca x in model.ToList()){
                ret.Add(new RCAEntityModel
                {
                    id = x.id,
                    name = x.name,
                    description = x.description,
                    pir_number = x.pir != null ? x.pir.no : "",
                    identity = 0
                });
            }

            return View(new GridModel<RCAEntityModel>
            {
                Data = ret
            });
        }

        //select pir initiator
        private ViewResult binding(int a)
        {
            string username = Session["username"].ToString();
            List<user_per_role> li = db.user_per_role.Where(p => p.username == username).ToList();
            var model = db.pirs.Where(p => p.initiate_by == username).Where(p => p.status != "FROM INITIATOR").OrderByDescending(p => p.id);
            if (li.Exists(p => p.role == (int)Config.role.FULLPIR))
            {
                if (a == 0)
                {
                    model = db.pirs.OrderByDescending(p => p.id);
                }
                else if (a == 1)
                {
                    model = db.pirs.Where(p => p.status == "VERIFIED").OrderByDescending(p => p.id);
                }
                else if (a == 2)
                {
                    model = db.pirs.Where(p => p.date_rise.Value.Year == DateTime.Today.Year).OrderByDescending(p => p.id);
                }
                else if (a == 3)
                {
                    model = db.pirs.Where(p => p.status != "VERIFIED").OrderByDescending(p => p.id);
                }
                else if (a == 4)
                {
                    model = db.pirs.Where(p => p.status != "VERIFIED" && p.target_completion_init != null && DateTime.Today.CompareTo(p.target_completion_init.Value) > 0).OrderByDescending(p => p.id);
                }
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
                           username = ue.username,
                           employee = employees.employee2,
                           approval_level = employees.approval_level
                       }).ToList();
            List<PIREntity> ret = new List<PIREntity>();
            foreach (pir x in model.ToList()) {
                EmployeeEntity e = has.Find(p => p.id == x.process_user);
                ret.Add(new PIREntity { 
                    id = x.id,
                    no = x.no,
                    title = x.title,
                    date_rise = x.date_rise,
                    target_completion_init = x.target_completion_init,
                    status = x.status,
                    reference = x.reference,
                    source = x.from == 1 ? "IIR" : x.from == 2 ? "Internal Audit" : x.from == 3 ? "External Audit" : "",
                    kbpo = e != null ? e.alpha_name : "",
                });
            }

            return View(new GridModel<PIREntity>
            {
                Data = ret
            });
        }
        
        //
        //Select ajax binding
        [GridAction]
        public ActionResult _SelectAjaxEditingProcess()
        {
            return bindingProcess();
        }

        //select equipment
        private ViewResult bindingProcess()
        {
            int id = int.Parse(Session["id"].ToString());
            var model = db.pirs.Where(a => a.status == "FROM INITIATOR").Where(a=>a.process_user == id);

            List<PIREntity> ret = new List<PIREntity>();
            foreach (pir x in model.ToList())
            {
                ret.Add(new PIREntity
                {
                    id = x.id,
                    no = x.no,
                    title = x.title,
                    date_rise = x.date_rise,
                    target_completion_init = x.target_completion_init,
                    initiate_by = x.initiate_by,
                    status = x.status,
                    reference = x.reference
                });
            }

            return View(new GridModel<PIREntity>
            {
                Data = ret
            });
        }

        //
        //Select ajax binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            delete(id);
            return binding(0);
        }

        //delete data failure mode
        private void delete(int id)
        {
            pir pir = db.pirs.Find(id);
            db.pirs.Remove(pir);
            db.SaveChanges();
        }

        #endregion

        #region crud

        public ActionResult createFromRca(int idRca)
        {
            string pirNumber = db.pirs.OrderByDescending(x => x.id).First().no;
            pirNumber = MyTools.generatePirNumber(pirNumber);
            pir pir = new pir();
            pir.no = pirNumber;
            return View("pageInitiator", pir);
        }

        public JsonResult AddInitiator(int identity = -1, int id = -1)
        {
            int[] data = new int[2];
            data[0] = identity; data[1] = id;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInitiator() {
            NameValueCollection nvc = Request.Form;          
            return Json(update(nvc));
        }

        private int update(NameValueCollection nvc){
            pir pir;
            int id = int.Parse(nvc["id"].ToString());
            int idRca = int.Parse(nvc["idRca"].ToString());
            sbyte? identity = SByte.Parse(nvc["identity"].ToString());

            if (id == 0)
            {
                int id_before = (db.pirs.ToList().Count == 0 ? 0 : db.pirs.Max(p => p.id)) + 1;
                pir = new pir();
                pir.status = "INITIATOR";
                pir.no = nvc["no"].ToString();
                pir.initiator_sign_date = DateTime.Now.Date;
                pir.kpbo_sign_date_init = DateTime.Now.Date;
                pir.improvement_request = nvc["improvement_request"].ToString();
                if (nvc["date_rise"].ToString() != "")
                    pir.date_rise = DateTime.Parse(nvc["date_rise"].ToString());
                pir.initiate_by = nvc["initiate_by"].ToString();
                pir.title = nvc["title"].ToString();
                pir.reference = nvc["reference"].ToString();
                pir.procedure_reference = nvc["procedure_reference"].ToString();
                if (nvc["target_completion_init"].ToString() != "")
                    pir.target_completion_init = DateTime.Parse(nvc["target_completion_init"].ToString());
                pir.initiator_sign_date = DateTime.Now.Date;
                pir.description = nvc["description"].ToString();
                pir.pir_category = nvc["pir_category"].ToString();
                pir.process_user = int.Parse(nvc["process_user"].ToString());

                employee e = db.employees.Find(int.Parse(nvc["process_user"].ToString()));
                pir.process_owner = e.kbp_code;
                pir.from = (byte)identity;
                db.pirs.Add(pir);
                db.SaveChanges();

                List<string> s = new List<string>();
                var sendEmail = new SendEmailController();
                s.Clear();

                s.Add(db.employees.Find(pir.process_user).email);
                s.Add(db.employees.Find(db.users.Find(pir.initiate_by).employee_id).email);

                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review untuk Performance Improvement Request dengan nomor referensi " + pir.no + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review for Performance Improvement Request with reference number " + pir.no + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(db.users.Find(pir.initiate_by).employee_id).alpha_name, "Reviewing Performance Improvement Request " + pir.no);

                int ids = db.pirs.Max(p => p.id);

                if (ids != id_before)
                {
                    string subPathSign = "~/Attachment/pir/" + ids; // your code goes here
                    bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPathSign));
                    if (!IsExists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(subPathSign));
                    try
                    {
                        string old_path = Server.MapPath("~/Attachment/pir/" + id_before);
                        string new_path = Server.MapPath("~/Attachment/pir/" + ids);
                        var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/pir/" + id_before), "*.*", SearchOption.TopDirectoryOnly)
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

                if (identity == 0)
                {
                    if (idRca != -1)
                    {
                        //update pir number rca
                        rca rca = db.rcas.Find(idRca);
                        rca.id_pir = pir.id;

                        db.Entry(rca).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (identity == 1) {
                    if (idRca != -1)
                    {
                        //update pir number iir
                        iir_recommendations iirRec = db.iir_recommendations.Find(idRca);
                        iirRec.pir_number = pir.id;

                        db.Entry(iirRec).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (identity == 2 || identity == 3)
                {
                    if (idRca != -1)
                    {
                        //update pir number audit_log
                        audit_log auditLog = db.audit_log.Find(idRca);
                        auditLog.id_pir = pir.id;

                        db.Entry(auditLog).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //insert data to log
                insertLog(pir);

                this.SetWorkflowNode(pir.id, "SaveInitiator");

                return pir.id;
            }
            else {
                pir = db.pirs.Find(id);
                if (nvc["type"].ToString() == "FROM PROCESS")
                {
                    //top
                    pir.process_user = int.Parse(nvc["process_user"].ToString());
                    pir.pir_category = nvc["pir_category"].ToString();
                    pir.improvement_request = nvc["improvement_request"].ToString();
                    if (nvc["date_rise"].ToString() != "")
                        pir.date_rise = DateTime.Parse(nvc["date_rise"].ToString());
                    pir.title = nvc["title"].ToString();
                    pir.reference = nvc["reference"].ToString();
                    pir.procedure_reference = nvc["procedure_reference"].ToString();
                    if (nvc["target_completion_init"].ToString() != "")
                        pir.target_completion_init = DateTime.Parse(nvc["target_completion_init"].ToString());

                    //down
                    pir.verified_desc = nvc["verified_desc"].ToString();
                    pir.description = nvc["description"].ToString();
                    pir.sign_date_verified = DateTime.Now.Date;
                }
                else {
                    pir.process_user = int.Parse(nvc["process_user"].ToString());
                    pir.pir_category = nvc["pir_category"].ToString();
                    pir.improvement_request = nvc["improvement_request"].ToString();
                    if (nvc["date_rise"].ToString() != "")
                        pir.date_rise = DateTime.Parse(nvc["date_rise"].ToString());
                    pir.title = nvc["title"].ToString();
                    pir.reference = nvc["reference"].ToString();
                    pir.procedure_reference = nvc["procedure_reference"].ToString();
                    if (nvc["target_completion_init"].ToString() != "")
                        pir.target_completion_init = DateTime.Parse(nvc["target_completion_init"].ToString());
                    pir.description = nvc["description"].ToString();
                }
                db.Entry(pir).State = EntityState.Modified;
                db.SaveChanges();

                return pir.id;
            }
        }

        public JsonResult UpdateProcess()
        {
            NameValueCollection nvc = Request.Form;
            return Json(updateP(nvc));
        }

        private int updateP(NameValueCollection nvc)
        {
            pir pir;
            int id = int.Parse(nvc["id"].ToString());

            pir = db.pirs.Find(id);
            //process
            pir.desc_prob = nvc["desc_prob"].ToString();
            if (nvc["investigator_date"].ToString() != "")
                pir.investigator_date = DateTime.Parse(nvc["investigator_date"].ToString());
            pir.improvement_plant = nvc["improvement_plant"].ToString();
            pir.process_owner = nvc["process_owner"].ToString();
            pir.action_by = nvc["action_by"].ToString();
            pir.investigator = db.employees.Find(Int32.Parse(Session["id"].ToString())).alpha_name;
            if (nvc["start_implement_date"].ToString() != "")
                pir.start_implement_date = DateTime.Parse(nvc["start_implement_date"].ToString());
            if (nvc["target_completion_process"].ToString() != "")
                pir.target_completion_process = DateTime.Parse(nvc["target_completion_process"].ToString());
            pir.require_dokument = nvc["require_dokument"].ToString();
            pir.hira_require = nvc["hira_require"].ToString();

            if (nvc["kpbo_sign_date_process"].ToString() != "")
                pir.kpbo_sign_date_process = DateTime.Parse(nvc["kpbo_sign_date_process"].ToString());
            if (nvc["review_date"].ToString() != "")
                pir.review_date = DateTime.Parse(nvc["review_date"].ToString());

            pir.result_of_action = nvc["result_of_action"].ToString();
            if (nvc["kpbo_sign_date_process_result"].ToString() != "")
                pir.kpbo_sign_date_process_result = DateTime.Parse(nvc["kpbo_sign_date_process_result"].ToString());

            pir.description = nvc["description"].ToString();

            db.Entry(pir).State = EntityState.Modified;
            db.SaveChanges();
            return pir.id;
        }
        
        //delete pir number in rca or iir
        [GridAction]
        public ActionResult DeletePirNumber(int identity = -1, int id = -1)
        {
            int? idPir;
            if (identity == 0)//delete pir in rca
            {
                rca rca = db.rcas.Find(id);
                idPir = rca.id_pir;

                rca.id_pir = null;

                db.Entry(rca).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                iir_recommendations iir = db.iir_recommendations.Find(id);
                idPir = iir.pir_number;

                iir.pir_number = null;

                db.Entry(iir).State = EntityState.Modified;
                db.SaveChanges();
            }

            pir pir = db.pirs.Find(idPir);
            if(pir != null){
                db.pirs.Remove(pir);
                db.SaveChanges();
            }

            ViewBag.nama = "Performance Improvement Request Inititator";
            return bindingTaskPir();
        }
        #endregion


        #region action
        public JsonResult sendToProcess() {
            NameValueCollection nvc = Request.Form;
            update(nvc);

            pir pir = db.pirs.Find(int.Parse(nvc["id"].ToString()));
            pir.status = "FROM INITIATOR";

            db.Entry(pir).State = EntityState.Modified;
            db.SaveChanges();

            //insert data to log
            insertLog(pir);

            if (pir.process_user != null)
            {
                this.SendUserNotification(pir, pir.process_user.Value, "Please Process "+pir.no);
            }

            this.SetWorkflowNode(pir.id, "SendInitiator");

            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public JsonResult verified()
        {
            NameValueCollection nvc = Request.Form;
            update(nvc);

            pir pir = db.pirs.Find(int.Parse(nvc["id"].ToString()));
            pir.status = "VERIFIED";

            db.Entry(pir).State = EntityState.Modified;
            db.SaveChanges();


            //insert data to log
            insertLog(pir);

            this.SetWorkflowNode(pir.id, "VerificationInitiator");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult sendToInitiator()
        {
            NameValueCollection nvc = Request.Form;
            updateP(nvc);

            pir pir = db.pirs.Find(int.Parse(nvc["id"].ToString()));
            pir.status = "FROM PROCESS";

            db.Entry(pir).State = EntityState.Modified;
            db.SaveChanges();

            //insert data to log
            insertLog(pir);

            if (pir.initiate_by != null && pir.initiate_by != "")
            {
                var user = (from a in db.users
                              where a.username == pir.initiate_by
                              select a).FirstOrDefault();
                if (user != null)
                {
                    this.SendUserNotification(pir, user.employee_id.Value, "Please Verify "+pir.no);
                }
            }

            this.SetWorkflowNode(pir.id, "ApproveProcessOwner");


            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //insert log to database
        private void insertLog(pir pir) {

            string status = "";

            switch (pir.status){
                case "INITIATOR" :
                    status = "CREATED";
                    break;
                case "FROM INITIATOR":
                    status = "SEND TO PROCESS";
                    break;
                case "FROM PROCESS":
                    status = "SEND TO INITIATOR";
                    break;
                default:
                    status = "VERIFIED";
                    break;

            }

            pir_log log = new pir_log()
            {
                date = DateTime.Now,
                description = pir.description,
                id_pir = pir.id,
                process = status,
                username = Session["username"].ToString()
            };
            db.pir_log.Add(log);
            db.SaveChanges();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Log(int id) {
            List<pir_log> logs = db.pir_log.Where(x => x.id_pir == id).ToList();
            return View(logs);
        }
        #endregion


        #region binding pir
        //
        //Select ajax binding my task
        [GridAction]
        public ActionResult _SelectAjaxEditingTaskPir()
        {
            return bindingTaskPir();
        }

        //select rca task
        private ViewResult bindingTaskPir()
        {
            string id = Session["id"].ToString();
            IQueryable<iir_recommendations> model;
            if (id == "277")
            {
                model = from p in db.iir_recommendations
                            where (p.has_pir == 1)
                            select p;
            }
            else
            {
                model = from p in db.iir_recommendations
                            where p.PIC == ""
                            select p;
            }

            List<RCAEntityModel> ret = new List<RCAEntityModel>();
            foreach (iir_recommendations x in model.ToList())
            {
                string temp = "";
                string reference = "";
                if(x.pir_number != null){
                    pir pir = db.pirs.Find(x.pir_number);
                    temp = pir.no;
                    reference = pir.reference;
                }
                ret.Add(new RCAEntityModel
                {
                    id = x.id,
                    description = x.description,
                    pir_number = temp,
                    identity = 1,
                    rca_code = reference
                });
            }

            return View(new GridModel<RCAEntityModel>
            {
                Data = ret
            });
        }
        #endregion

        #region print

        public ActionResult printPIR(int id)
        {
            pir pir = db.pirs.Find(id);
            pir.attach = new List<string>();
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/pir/" + id), "*.*", SearchOption.TopDirectoryOnly)
                            where file.Substring(file.LastIndexOf(".") + 1).ToLower() == "png" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpeg"
                             || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "jpg" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "bmp" || file.Substring(file.LastIndexOf(".") + 1).ToLower() == "pdf"
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    pir.attach.Add(f.File.Substring(f.File.LastIndexOf("\\") + 1));
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
            return this.ViewPdf("", "PIRPrint", pir);
        }

        #endregion

        #region report
        public ActionResult Report(int id_page) {

            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/pir/Report?id_page=1" });
            }
            else
            {
                ViewBag.is_page = id_page;
                Report r = new Report();

                ViewData["report"] = r;

                return View();
            }
        }

        public ActionResult PrintReport(int id)
        {
            Report r = new Report();

            ViewData["report"] = r;
            return this.ViewPdf("", "ReportPdf",ViewData);
        }
        #endregion

        #region attachment

        [HttpPost]
        public ActionResult attachment(IEnumerable<HttpPostedFileBase> attachment, int id)
        {
            var currpath = "";
            string st = "";
            if (id == 0) id = (db.pirs.ToList().Count == 0 ? 0 : db.pirs.Max(p => p.id)) + 1;
            if (attachment != null)
            {
                foreach (var file in attachment)
                {
                    currpath = Path.Combine(
                    Server.MapPath("~/Attachment/pir/" + id),
                    file.FileName);
                    file.SaveAs(currpath);
                }
                currpath = "/Attachment/pir/" + id + "/";
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/pir/" + id), "*.*", SearchOption.TopDirectoryOnly)
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
            return Json(new { success = true, path = currpath, files = st });
        }

        [HttpPost]
        public ActionResult Atch(int id)
        {
            if (id == 0) id = (db.pirs.ToList().Count == 0 ? 0 : db.pirs.Max(p => p.id)) + 1;
            var currpath = "/Attachment/pir/" + id + "/";
            string st = "";
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/pir/" + id), "*.*", SearchOption.TopDirectoryOnly)
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

        public static string readPdf(string path, string has, string filename)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            Debug.WriteLine(path + " " + has);
            p.StartInfo.Arguments = " -layout -htmlmeta" + " \"" + path + "\" \"" + has + "\"";
            p.StartInfo.FileName = filename;

            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.Start();
            p.WaitForExit();
            System.Threading.Thread.Sleep(3000);
            return ReadFile(has);

        }

        public static string ReadFile(string s)
        {
            StreamReader sr = new StreamReader(s);
            string strReturn = sr.ReadToEnd();
            return strReturn;

        }

        #endregion

        #region kpi

        [HttpPost]
        public ActionResult pirTarget()
        {
            return Json(new { success = true, target = db.she_KPI_target.Find(2).target });

        }

        [HttpPost]
        public ActionResult setPirTarget(int target)
        {
            she_KPI_target kp = db.she_KPI_target.Find(2);
            kp.target = target;
            db.Entry(kp).State = EntityState.Modified;
            db.SaveChanges();

            List<pir> list_pir = db.pirs.ToList();
            int totalPir = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year).ToList().Count;
            int totalOverdue = list_pir.Where(a => a.target_completion_init != null && a.target_completion_init.Value.CompareTo(DateTime.Today) < 0).ToList().Count;
            int totalCompleteOnTime = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status == "VERIFIED" && a.sign_date_verified.Value.CompareTo(a.target_completion_init.Value) <= 0).ToList().Count;
            double pir_target = db.she_KPI_target.Find(2).target != null ? db.she_KPI_target.Find(2).target.Value : 0;
            int totalClose = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status == "VERIFIED").ToList().Count;
            int totalOpen = list_pir.Where(a => a.target_completion_init != null && a.target_completion_init.Value.CompareTo(DateTime.Today) >= 0).ToList().Count;
            int totalRaised = list_pir.Where(a => a.date_rise.Value.Year == DateTime.Today.Year && a.status != "VERIFIED").ToList().Count;
            bool success = false;
            if (((double)totalCompleteOnTime / (double)totalPir) * (double)100 > pir_target || totalPir == 0)
            {
                success = true;
            }
            else
            {
                success = false;

            }
            return Json(new { success = true, totalPir = totalPir, totalOverdue = totalOverdue, successFail = success });

        }

        #endregion

        public ActionResult ExportExcelData()
        {
            var model = db.pirs.OrderByDescending(p => p.id);
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
                           username = ue.username,
                           employee = employees.employee2,
                           approval_level = employees.approval_level
                       }).ToList();
            List<PIREntityExport> ret = new List<PIREntityExport>();
            foreach (pir x in model.ToList())
            {
                EmployeeEntity e = has.Find(p => p.id == x.process_user);
                ret.Add(new PIREntityExport
                {
                    no = x.no,
                    title = x.title,
                    improvement_request = x.improvement_request,
                    date_rise = x.date_rise,
                    initiate_by = x.initiate_by,
                    reference = x.reference,
                    procedure_reference = x.procedure_reference,
                    target_completion_init = x.target_completion_init,
                    desc_prob = x.desc_prob,
                    investigator = x.investigator,
                    improvement_plant = x.improvement_plant,
                    start_implement_date = x.start_implement_date,
                    process_owner = x.process_owner,
                    target_completion_process = x.target_completion_process,
                    action_by = x.action_by,
                    require_dokument = x.require_dokument,
                    hira_require = x.hira_require,
                    result_of_action = x.result_of_action,
                    sign_date_verified = x.sign_date_verified,
                    verified_desc = x.verified_desc,
                    status = x.status,
                    source = x.from == 1 ? "IIR" : x.from == 2 ? "Internal Audit" : x.from == 3 ? "External Audit" : "",
                    kbpo = e != null ? e.alpha_name : "",
                    PIRStatus = x.status == "VERIFIED" ? "Closed" : (x.target_completion_init != null && DateTime.Today.CompareTo(x.target_completion_init.Value) > 0 ? "Overdue" : (x.target_completion_init != null && DateTime.Today.CompareTo(x.target_completion_init.Value) <= 0 ? "Open" : "")),
                });
            }
            GridView gv = new GridView();
            gv.Caption = "Performance Improvement Request Data";
            gv.DataSource = ret;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "PIR No.";
            gv.HeaderRow.Cells[1].Text = "Title";
            gv.HeaderRow.Cells[2].Text = "Improvement Request";
            gv.HeaderRow.Cells[3].Text = "Date Raised";
            gv.HeaderRow.Cells[4].Text = "Initiated By";
            gv.HeaderRow.Cells[5].Text = "Reference";
            gv.HeaderRow.Cells[6].Text = "Procedure Reference";
            gv.HeaderRow.Cells[7].Text = "Proposed Completion Date";
            gv.HeaderRow.Cells[8].Text = "Problem Description";
            gv.HeaderRow.Cells[9].Text = "Investigator";
            gv.HeaderRow.Cells[10].Text = "Improvement Plan";
            gv.HeaderRow.Cells[11].Text = "Start Implementation Date";
            gv.HeaderRow.Cells[12].Text = "Process Owner Code";
            gv.HeaderRow.Cells[13].Text = "Target Completion Date";
            gv.HeaderRow.Cells[14].Text = "Action By";
            gv.HeaderRow.Cells[15].Text = "Required Document?";
            gv.HeaderRow.Cells[16].Text = "Hira Required?";
            gv.HeaderRow.Cells[17].Text = "Result of Action";
            gv.HeaderRow.Cells[18].Text = "Verified Description";
            gv.HeaderRow.Cells[19].Text = "Actual Completion Date";
            gv.HeaderRow.Cells[20].Text = "Status";
            gv.HeaderRow.Cells[21].Text = "Process Owner";
            gv.HeaderRow.Cells[22].Text = "Source";
            gv.HeaderRow.Cells[23].Text = "PIR Status";
            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "PIR Data (From WW-FRACAS) " + DateTime.Today.ToString("ddmmyyyy") + ".xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        

        public JsonResult showTrending(int from, int to) {
            SqlConnection sqlConnection1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = @"with proc_owner_list(ownlist) as
                (
                select 'BPL'
                union all
                select 'GEL'
                union all
                select 'GDI'
                union all
                select 'POP'
                union all
                select 'MTW'
                union all
                select 'SPE'
                union all
                select 'OHE'
                union all
                select 'EPE'
                union all
                select 'EAI'
                union all
                select 'OSU'
                union all
                select 'LCO'
                union all
                select 'SSU'
                union all
                select 'MER'
                union all
                select 'SEC'
                union all
                select 'SCM'
                union all
                select 'MIS'
                union all
                select 'FPR'
                union all
                select 'PAC'
                union all
                select 'FHR'
                ),
                mnth(mnuum) as
                (
                select 2012
                union all
                select mnuum+1 from mnth where mnuum<year(getDate())
                )
                select a.mnuum as year, proc_owner_list.ownlist as process_owner, COUNT(CASE WHEN isnull(p.target_completion_init,'') <> '' AND (((year(GETDATE()) = a.mnuum) AND DateDiff(day,GETDATE(),p.target_completion_init) < 0 AND (p.status <> 'VERIFIED' OR (p.status = 'VERIFIED' AND DateDiff(day,GETDATE(),p.initiator_verified_date) > 0))) OR ((year(GETDATE()) <> a.mnuum) AND DateDiff(day,convert(datetime, ('12-31-' + Convert(varchar, a.mnuum))),p.target_completion_init) < 0 AND (p.status <> 'VERIFIED' OR (p.status = 'VERIFIED' AND DateDiff(day,convert(datetime, ('12-31-' + Convert(varchar, a.mnuum))),p.initiator_verified_date) > 0)))) THEN 1 END) as jumlah
                from proc_owner_list cross join (select mnth.mnuum from mnth) a
                left join (Select * from pir where DATEDIFF(day,GETDATE(),date_rise) <= 0) as p on p.process_owner = proc_owner_list.ownlist
                group by ownlist, a.mnuum";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();
            List<TrendData> listTrendData = new List<TrendData>();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                TrendData td = new TrendData
                {
                    year = reader["year"].ToString(),
                    kbp = reader["process_owner"].ToString(),
                    count = (int)reader["jumlah"],
                };
                listTrendData.Add(td);
            }
            reader.Close();

            return Json(listTrendData, JsonRequestBehavior.AllowGet);
        }

        private void SendUserNotification(pir data, int sendUserId, string message)
        {
            WWService.UserServiceClient client = new WWService.UserServiceClient();
            WWService.ResponseModel response = client.CreateNotification(
            EncodeMd5("starenergyww"),
            sendUserId,
            System.Configuration.ConfigurationManager.AppSettings["ApplicationName"],
            "PIR",
            message,
            "/NotificationUrlResolver/FRACAS?name=FRACAS_PIR&id="+data.id);

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

        #region workflow_node
        private void SetWorkflowNode(int idReport, string source)
        {

            workflow_node nodeInitiatorCreate;
            workflow_node nodeProcessOwner;
            workflow_node nodeVerificationInitiator;

            var checkExisting = (from a in db.workflow_node
                                 where a.id_report == idReport
                                 && a.report_type == "FR-PIR"
                                 select a).FirstOrDefault();

            if (checkExisting == null)
            {
                nodeInitiatorCreate = new workflow_node();
                nodeInitiatorCreate.id_report = idReport;
                nodeInitiatorCreate.report_type = "FR-PIR";
                nodeInitiatorCreate.node_name = "InitiatorC";
                nodeProcessOwner = new workflow_node();
                nodeProcessOwner.id_report = idReport;
                nodeProcessOwner.report_type = "FR-PIR";
                nodeProcessOwner.node_name = "POwner";
                nodeVerificationInitiator = new workflow_node();
                nodeVerificationInitiator.id_report = idReport;
                nodeVerificationInitiator.node_name = "InitiatorV";
                nodeVerificationInitiator.report_type = "FR-PIR";
            }
            else
            {
                nodeInitiatorCreate = (from a in db.workflow_node
                                 where a.id_report == idReport
                                 && a.node_name == "InitiatorC" && a.report_type == "FR-PIR"
                                 select a).FirstOrDefault();
                if (nodeInitiatorCreate == null)
                {
                    nodeInitiatorCreate = new workflow_node();
                    nodeInitiatorCreate.id_report = idReport;
                    nodeInitiatorCreate.node_name = "InitiatorC";
                    nodeInitiatorCreate.report_type = "FR-PIR";
                }

                nodeProcessOwner = (from a in db.workflow_node
                                  where a.id_report == idReport
                                  && a.node_name == "POwner" && a.report_type == "FR-PIR"
                                  select a).FirstOrDefault();
                if (nodeProcessOwner == null)
                {
                    nodeProcessOwner = new workflow_node();
                    nodeProcessOwner.id_report = idReport;
                    nodeProcessOwner.node_name = "POwner";
                    nodeProcessOwner.report_type = "FR-PIR";
                }

                nodeVerificationInitiator = (from a in db.workflow_node
                                      where a.id_report == idReport
                                      && a.node_name == "InitiatorV" && a.report_type == "FR-PIR"
                                      select a).FirstOrDefault();
                if (nodeVerificationInitiator == null)
                {
                    nodeVerificationInitiator = new workflow_node();
                    nodeVerificationInitiator.id_report = idReport;
                    nodeVerificationInitiator.node_name = "InitiatorV";
                    nodeVerificationInitiator.report_type = "FR-PIR";
                }

            }

            //0 Not Yet
            //1 Current
            //2 Approved
            switch (source)
            {
                case "SaveInitiator":
                    nodeInitiatorCreate.status = 1;
                    nodeProcessOwner.status = 0;
                    nodeVerificationInitiator.status = 0;
                    break;
                case "SendInitiator":
                    nodeInitiatorCreate.status = 2;
                    nodeProcessOwner.status = 1;
                    nodeVerificationInitiator.status = 0;
                    break;
                case "ApproveProcessOwner":
                    nodeInitiatorCreate.status = 2;
                    nodeProcessOwner.status = 2;
                    nodeVerificationInitiator.status = 1;
                    break;
                case "VerificationInitiator":
                    nodeInitiatorCreate.status = 2;
                    nodeProcessOwner.status = 2;
                    nodeVerificationInitiator.status = 2;
                    break;
                default: Response.Write("Internal server error. Please contact administrator"); break;
            }

            if (checkExisting == null)
            {
                db.workflow_node.Add(nodeInitiatorCreate);
                db.workflow_node.Add(nodeProcessOwner);
                db.workflow_node.Add(nodeVerificationInitiator);
                db.SaveChanges();
            }
            else
            {
                db.workflow_node.Attach(nodeInitiatorCreate);
                db.Entry(nodeInitiatorCreate).State = EntityState.Modified;
                db.SaveChanges();

                db.workflow_node.Attach(nodeProcessOwner);
                db.Entry(nodeProcessOwner).State = EntityState.Modified;
                db.SaveChanges();

                db.workflow_node.Attach(nodeVerificationInitiator);
                db.Entry(nodeVerificationInitiator).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        public ActionResult GetWorkflowContent(int id)
        {
            var data = (from a in db.workflow_node
                        where a.report_type == "FR-PIR" && a.id_report == id
                        select a).ToList();
            int dataInitiatorCreate = 0;
            int dataProcessOwner = 0;
            int dataInitiatorVerification = 0;


            if (data.Count > 0)
            {
                foreach (workflow_node a in data)
                {
                    if (a.node_name == "InitiatorC")
                    {
                        dataInitiatorCreate = a.status;
                    }
                    else if (a.node_name == "POwner")
                    {
                        dataProcessOwner = a.status;
                    }
                    else if (a.node_name == "InitiatorV")
                    {
                        dataInitiatorVerification = a.status;
                    }
                }
            }

            ViewBag.InitiatorC = dataInitiatorCreate;
            ViewBag.ProcessOwner = dataProcessOwner;
            ViewBag.InitiatorV = dataInitiatorVerification;

            return PartialView("WorkflowContent");
        }

        public string MigrateWorkflowData()
        {
            string sql = "Delete from workflow_node where report_type='FR-PIR'";
            db.Database.ExecuteSqlCommand(sql);

            List<pir> data = (from a in db.pirs
                                           select a).ToList();

            foreach (pir a in data)
            {
                if (a.status == "INITIATOR")
                {
                    workflow_node initiatorC = new workflow_node();
                    initiatorC.id_report = a.id;
                    initiatorC.report_type = "FR-PIR";
                    initiatorC.node_name = "InitiatorC";
                    initiatorC.status = 1;
                    db.workflow_node.Add(initiatorC);

                    workflow_node processOwner = new workflow_node();
                    processOwner.id_report = a.id;
                    processOwner.report_type = "FR-PIR";
                    processOwner.node_name = "Owner";
                    processOwner.status = 0;
                    db.workflow_node.Add(processOwner);

                    workflow_node workflow = new workflow_node();
                    workflow.id_report = a.id;
                    workflow.report_type = "FR-PIR";
                    workflow.node_name = "InitiatorV";
                    workflow.status = 0;
                    db.workflow_node.Add(workflow);

                }
                else if (a.status == "FROM INITIATOR")
                {
                    workflow_node initiatorC = new workflow_node();
                    initiatorC.id_report = a.id;
                    initiatorC.report_type = "FR-PIR";
                    initiatorC.node_name = "InitiatorC";
                    initiatorC.status = 2;
                    db.workflow_node.Add(initiatorC);

                    workflow_node processOwner = new workflow_node();
                    processOwner.id_report = a.id;
                    processOwner.report_type = "FR-PIR";
                    processOwner.node_name = "Owner";
                    processOwner.status = 1;
                    db.workflow_node.Add(processOwner);

                    workflow_node workflow = new workflow_node();
                    workflow.id_report = a.id;
                    workflow.report_type = "FR-PIR";
                    workflow.node_name = "InitiatorV";
                    workflow.status = 0;
                    db.workflow_node.Add(workflow);
                }
                else if (a.status == "FROM PROCESS")
                {
                    workflow_node initiatorC = new workflow_node();
                    initiatorC.id_report = a.id;
                    initiatorC.report_type = "FR-PIR";
                    initiatorC.node_name = "InitiatorC";
                    initiatorC.status = 2;
                    db.workflow_node.Add(initiatorC);

                    workflow_node processOwner = new workflow_node();
                    processOwner.id_report = a.id;
                    processOwner.report_type = "FR-PIR";
                    processOwner.node_name = "Owner";
                    processOwner.status = 2;
                    db.workflow_node.Add(processOwner);

                    workflow_node workflow = new workflow_node();
                    workflow.id_report = a.id;
                    workflow.report_type = "FR-PIR";
                    workflow.node_name = "InitiatorV";
                    workflow.status = 1;
                    db.workflow_node.Add(workflow);
                }
                else if (a.status == "VERIFIED")
                {
                    workflow_node initiatorC = new workflow_node();
                    initiatorC.id_report = a.id;
                    initiatorC.report_type = "FR-PIR";
                    initiatorC.node_name = "InitiatorC";
                    initiatorC.status = 2;
                    db.workflow_node.Add(initiatorC);

                    workflow_node processOwner = new workflow_node();
                    processOwner.id_report = a.id;
                    processOwner.report_type = "FR-PIR";
                    processOwner.node_name = "Owner";
                    processOwner.status = 2;
                    db.workflow_node.Add(processOwner);

                    workflow_node workflow = new workflow_node();
                    workflow.id_report = a.id;
                    workflow.report_type = "FR-PIR";
                    workflow.node_name = "InitiatorV";
                    workflow.status = 2;
                    db.workflow_node.Add(workflow);
                }
                db.SaveChanges();
            }


            return "success";
        }

        #endregion


    }

    public class TrendData
    {
        public string year { get; set; }
        public string kbp { get; set; }
        public int count { get; set; }
    }


    public class Report
    {
        public string[] Month;
        public int[] Closed;
        public int[] Raised;
        public int[] Open;
        public int[] Overdue;

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public Report()
        {
            int now = DateTime.Now.Month;
            Month = new string[now];
            Closed = new int[now];
            Raised = new int[now];
            Open = new int[now];
            Overdue = new int[now];
            for (int i = 1; i <= now;i++ )
            {
                Month[i - 1] = Config.convertMonth(i); 
            }

            List<pir> pirs = db.pirs.ToList();

            for (int i = 1; i <= now;i++ ){
                List<pir> temp = pirs.Where(a => a.date_rise.Value.Month == i).ToList();

                Closed[i - 1] = temp.Where(a => a.status == "VERIFIED").ToList().Count;
                Raised[i - 1] = temp.ToList().Count;
                Open[i - 1] = temp.Where(a => a.status != "VERIFIED").ToList().Count;
                Overdue[i - 1] = temp.Where(a => a.target_completion_init < DateTime.Now).ToList().Count;
            }
        }
    }


}
