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
    public class HseAuditorController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /HseAuditor/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseAuditor" });
            }
            else
            {
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!li.Exists(p => p.role == (int)Config.role.AUDITOR))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseAuditor" });
                }
                ViewBag.Nama = "SHE Auditor";
                return View();
            }
            
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addAudit(int? id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       join user_per_roles in db.user_per_role on ue.username equals user_per_roles.username
                       orderby employees.dept_id
                       where user_per_roles.role == 4 || user_per_roles.role == 11
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
                           employee = employees.employee2
                       }).Distinct().ToList();
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
                ee.employee = null;
            }
            ViewBag.employee = bind;
            List<pir_clause> list_clause = db.pir_clause.ToList();
            ViewBag.list_clause = list_clause;
            if (id != null)
            {
                ViewBag.datas = db.audits.Find(id);
                ViewBag.mod = id;
                ViewBag.isSubmit = db.audits.Find(id).is_submit;
            }
            return PartialView();
        }

        //
        // Ajax select binding Hse Auditor report
        [GridAction]
        public ActionResult _SelectAjaxHseAuditor()
        {
            return bindingHseAuditor();
        }

        //select data incident she observation report
        private ViewResult bindingHseAuditor()
        {
            List<audit> f = new List<audit>();
            f = db.audits.ToList();

            return View(new GridModel<audit>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(audit audit)
        {
            db.audits.Add(audit);
            db.SaveChanges();
            int id_audit = db.audits.ToList().LastOrDefault().id;
            List<audit_log> li = db.audit_log.Where(p => p.id_audit == null).ToList();
            foreach (audit_log l in li)
            {
                l.id_audit = audit.id;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { id = audit.id });
        }

        [HttpPost]
        public JsonResult Edit(audit audit)
        {
            audit audits = db.audits.Find(audit.id);

            audits.in_ex = audit.in_ex;
            audits.audit_no = audit.audit_no;
            audits.is_submit = audit.is_submit;

            db.Entry(audits).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        #region audit_log
        //
        // Ajax select binding power station activity
        [GridAction]
        public ActionResult _SelectAjaxAuditLog(int? id)
        {
            return bindingAuditLog(id);
        }

        //select data power station activity
        private ViewResult bindingAuditLog(int? id)
        {
            //id = id == null ? 0 : id;
            var r = (from log in db.audit_log
                    join pirs in db.pirs on log.id_pir equals pirs.id into pir_audit
                    from pir_au in pir_audit.DefaultIfEmpty()
                    select new AuditLogEntity
                    {
                        id = log.id,
                        id_audit = log.id_audit,
                        grade = log.grade,
                        status = log.status,
                        id_pir = log.id_pir,
                        pir_number = pir_au.no,
                        finding = log.finding,
                        process = log.process,
                        date = log.date,
                        requester = log.requester,
                        reference = log.reference
                    });
            List<AuditLogEntity> audit_log_entity = r.Where(p => p.id_audit == id).ToList();
            Debug.WriteLine("aaa " + id);
            if (id == null)
            {
                audit_log_entity = r.Where(p => p.id_audit.Equals(null)).ToList();
            }
            else
            {
                audit_log_entity = r.Where(p => p.id_audit == id).ToList();
            }
            
            foreach (AuditLogEntity au in audit_log_entity)
            {
                var ids = au.id;
                var c = (from clause in db.audit_log_clause
                         join pir_cl in db.pir_clause on clause.id_pir_clause equals pir_cl.id
                         where clause.id_audit_log == ids
                         select new AuditLogClauseEntity
                         {
                             id = clause.id,
                             id_audit_log = clause.id_audit_log,
                             id_pir_clause = clause.id_pir_clause,
                             clause_no = pir_cl.clause_no
                         }).ToList();
                au.list_clauses = c;
                if (au.requester != null) au.requester = db.employees.Find(Int32.Parse(au.requester != null ? au.requester : "0")).alpha_name;
            }
            return View(new GridModel<AuditLogEntity>
            {
                Data = audit_log_entity
            });
        }

        public JsonResult addLog()
        {
            return Json(true);
        }

        public JsonResult raisePIR(int id)
        {
            return Json(new { id = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult raisePIRs(int requester, int id)
        {
            Debug.WriteLine(requester);
            audit_log al = db.audit_log.Find(id);
            al.requester = requester.ToString();
            audit audit = db.audits.Find(al.id_audit);
            db.Entry(al).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { in_ex = audit.in_ex, reference = audit.audit_no, finding = System.Web.HttpUtility.HtmlEncode(al.finding) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [GridAction]
        public JsonResult addLogs(audit_log log, int[] arrClauses)
        {
            //if (addPIR == 1)
            //{
            //    //generate pir_number first
            //    ir_recommend.pir_number = "12";
            //}
            db.audit_log.Add(log);
            db.SaveChanges();
            if (arrClauses != null)
            {
                foreach (int i in arrClauses)
                {
                    audit_log_clause alc = new audit_log_clause { id = 0, id_audit_log = log.id, id_pir_clause = i };
                    db.audit_log_clause.Add(alc);
                    db.SaveChanges();
                }
            }
            return Json(true);
        }

        public JsonResult GetLog(int id)
        {
            var r = (from log in db.audit_log
                     join pirs in db.pirs on log.id_pir equals pirs.id into pir_audit
                     from pir_au in pir_audit.DefaultIfEmpty()
                     where log.id == id
                     select new AuditLogEntity
                     {
                         id = log.id,
                         id_audit = log.id_audit,
                         grade = log.grade,
                         status = log.status,
                         id_pir = log.id_pir,
                         pir_number = pir_au.no,
                         finding = log.finding,
                         process = log.process,
                         date = log.date,
                         reference = log.reference
                     });
            AuditLogEntity audit_log_entity = r.ToList().FirstOrDefault();
            var c = (from clause in db.audit_log_clause
                        where clause.id_audit_log == id
                        select new AuditLogClauseEntity
                        {
                            id = clause.id,
                            id_audit_log = clause.id_audit_log,
                            id_pir_clause = clause.id_pir_clause
                        }).ToList();
            audit_log_entity.list_clauses = c;
            return Json(audit_log_entity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult editLog(audit_log log, int[] arrClauses)
        {
            audit_log logs = db.audit_log.Find(log.id);

            logs.grade = log.grade;
            logs.status = log.status;
            logs.finding = log.finding;
            logs.process = log.process;
            logs.date = log.date;
            logs.reference = log.reference;

            db.Entry(logs).State = EntityState.Modified;
            db.SaveChanges();

            var c = (from clause in db.audit_log_clause
                     where clause.id_audit_log == log.id
                     select new AuditLogClauseEntity
                     {
                         id = clause.id,
                         id_audit_log = clause.id_audit_log,
                         id_pir_clause = clause.id_pir_clause
                     }).ToList();

            foreach (var a in c)
            {
                db.audit_log_clause.Remove(db.audit_log_clause.Find(a.id));
                db.SaveChanges();
            }
            if (arrClauses != null)
            {
                foreach (int i in arrClauses)
                {
                    audit_log_clause alc = new audit_log_clause { id = 0, id_audit_log = log.id, id_pir_clause = i };
                    db.audit_log_clause.Add(alc);
                    db.SaveChanges();
                }
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteAllLog()
        {
            List<audit_log> li = db.audit_log.Where(p => p.id_audit == null).ToList();

            foreach (audit_log log in li)
            {
                db.audit_log.Remove(log);
                db.SaveChanges();
            }
            return Json(true);
        }
        #endregion
    }
}
