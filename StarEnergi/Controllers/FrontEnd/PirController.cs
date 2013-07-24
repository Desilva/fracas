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
                    List<pir> list_pir = db.pirs.Where(p => p.date_rise.Value.Year == DateTime.Today.Year).ToList();
                    int totalPir = list_pir.Count;
                    int totalOverdue = list_pir.Where(a => a.target_completion_init < DateTime.Today).ToList().Count;
                    double pir_target = db.she_KPI_target.Find(2).target != null ? db.she_KPI_target.Find(2).target.Value : 0;

                    Debug.WriteLine(((double)totalOverdue / (double)totalPir) * (double)100);
                    if (((double)totalOverdue / (double)totalPir) * (double)100 <= pir_target)
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
                    ViewBag.nama = "Performance Improvement Request Inititator";
                    return View("ListInitiator");
                }
                else
                {
                    ViewBag.nama = "Performance Improvement Request Process";
                    return View("ListProcess");
                }
            }
        }


        //load 
        //identity 0 rca, 1 iir
        public ActionResult pageInitiator(int identity=-1, int id = -1, int idRca = -1, string requester = null, string reference = null)
        {
            var processUserList = new List<employee>();
            var query = (from a in db.employees
                         select new {a.id, a.alpha_name, a.position }).ToList();
            foreach (var a in query)
            {
                if (a.position.ToLower().Contains("superintendent"))
                    processUserList.Add(new employee { alpha_name = a.alpha_name, id = a.id });
            }

            ViewBag.idRca = idRca;
            ViewBag.identity = identity;
            ViewBag.reference = reference;
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
                ViewBag.requester = requester != null ? has.Where(p => p.id == Int32.Parse(requester)).ToList().FirstOrDefault().username : null;

                //add file initiator
                string pirNumber = db.pirs.Count() == 0 ? "" : db.pirs.Where(x=>x.no != null).OrderByDescending(x => x.id).First().no;
                pirNumber = MyTools.generatePirNumber(pirNumber);
                pir pir = new pir();
                pir.no = pirNumber;
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
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
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
        private ViewResult binding()
        {
            string username = Session["username"].ToString();
            List<user_per_role> li = db.user_per_role.Where(p => p.username == username).ToList();
            var model = db.pirs.Where(a => a.initiate_by == username).Where(a => a.status != "FROM INITIATOR");
            if (li.Exists(p => p.role == (int)Config.role.FULLPIR))
            {
                model = db.pirs.Where(a => a.status != "FROM INITIATOR");
            }

            List<PIREntity> ret = new List<PIREntity>();
            foreach (pir x in model.ToList()) {
                ret.Add(new PIREntity { 
                    id = x.id,
                    no = x.no,
                    title = x.title,
                    date_rise = x.date_rise,
                    target_completion_init = x.target_completion_init,
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
            return binding();
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
            
            var model = from p in db.iir_recommendations
                        where p.PIC == id && (p.has_pir == 1)
                        select p;

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
            int totalPir = list_pir.Count;
            int totalOverdue = list_pir.Where(a => a.target_completion_init < DateTime.Today).ToList().Count;
            bool success = ((double)totalOverdue / (double)totalPir) * (double)100 <= target;
            return Json(new { success = true, totalPir = totalPir, totalOverdue = totalOverdue, successFail = success });

        }

        #endregion

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
