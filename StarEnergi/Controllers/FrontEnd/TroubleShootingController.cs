using ReportManagement;
using StarEnergi.Controllers.Utilities;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class TroubleShootingController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /TroubleShooting/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/Troubleshooting" });
            }
            else
            {
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!li.Exists(p => p.role == (int)Config.role.FRACAS))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/Troubleshooting" });
                }
                ViewBag.Nama = "Trouble Shooting";
                return View();
            }
            
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addTroubleShooting(int? id, int? id_ir)
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
            trouble_shooting ts = db.trouble_shooting.OrderBy(p => p.no).ToList().LastOrDefault();

            if (ts == null || ts.no == null || ts.no.Length != 21)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ViewBag.ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(ts.no.Substring(17));
                refs++;
                int reference_year = Int32.Parse(ts.no.Substring(12, 4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ViewBag.ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else
                {
                    refs = 1;
                    ViewBag.ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }
            ViewData["users"] = bind;
            ViewData["user_role"] = li;
            if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.trouble_shooting.Find(id);
                ViewBag.superintendent_del = string.IsNullOrWhiteSpace(ts.superintendent_approval_signature) == null ? (string.IsNullOrWhiteSpace(ts.superintendent_approval_name) ? null : db.employees.Find(Int32.Parse(ts.superintendent_approval_name == null ? "0" : ts.superintendent_approval_name)).employee_delegate) : null;
                ViewBag.supervisor_del = string.IsNullOrWhiteSpace(ts.supervisor_approval_signature) == null ? (string.IsNullOrWhiteSpace(ts.supervisor_approval_name) ? null : db.employees.Find(Int32.Parse(ts.supervisor_approval_name == null ? "0" : ts.supervisor_approval_name)).employee_delegate) : null;
            }
            else
            {
                int cur_user_id = Int32.Parse(HttpContext.Session["id"].ToString());
                int? cur_dept_id = db.employees.Find(cur_user_id).dept_id;
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
                if (superintendent_id == null)
                {
                    superintendent_id = supervisor_id;
                    superintendent_id_del = supervisor_id_del;
                }
                ViewBag.superintendent_id = superintendent_id;
                ViewBag.supervisor_id = supervisor_id;
                ViewBag.superintendent_id_del = superintendent_id_del;
                ViewBag.supervisor_id_del = supervisor_id_del;
                int last_id = db.trouble_shooting.ToList().Count == 0 ? 0 : db.trouble_shooting.Max(p => p.id);
                last_id++;
                string subPath = "~/Attachment/trouble_shooting/" + last_id + "/signatures"; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            }
            ViewBag.id_ir = id_ir;
            return PartialView();
        }

        //
        // Ajax select binding illness report
        [GridAction]
        public ActionResult _SelectAjaxTroubleShooting()
        {
            return bindingTroubleShooting();
        }

        //select data incident report
        private ViewResult bindingTroubleShooting()
        {
            List<trouble_shooting> f = new List<trouble_shooting>();
            f = db.trouble_shooting.OrderByDescending(p => p.no).ToList();

            return View(new GridModel<trouble_shooting>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(trouble_shooting troubleShooting)
        {
            int id_before = (db.trouble_shooting.ToList().Count == 0 ? 0 : db.trouble_shooting.Max(p => p.id)) + 1;

            trouble_shooting ts = db.trouble_shooting.OrderBy(p => p.no).ToList().LastOrDefault();
            string ts_no = "";
            if (ts == null || ts.no == null || ts.no.Length != 21)
            {
                int refs = 1;
                int year = DateTime.Today.Year;
                ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
            }
            else
            {
                int refs = Int32.Parse(ts.no.Substring(17));
                refs++;
                int reference_year = Int32.Parse(ts.no.Substring(12, 4));
                int year = DateTime.Today.Year;
                if (year == reference_year)
                    ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                else
                {
                    refs = 1;
                    ts_no = "W-O-EAI-TSR-" + year + "-" + refs.ToString().PadLeft(4, '0');
                }
            }
            string sign = db.employees.Find(Int32.Parse(troubleShooting.inspector_name)).signature;
            troubleShooting.no = ts_no;
            troubleShooting.inspector_signature = sign;
            db.trouble_shooting.Add(troubleShooting);
            db.SaveChanges();
            int id = troubleShooting.id;

            //send email
            SendEmailToAll(troubleShooting);

            if (troubleShooting.id_ir != null)
            {
                //incident_report ir = db.incident_report.Find(troubleShooting.id_ir);
                //ir.id_tsr = troubleShooting.id;
                //db.Entry(ir).State = EntityState.Modified;
                //db.SaveChanges();
            }

            if (id != id_before)
            {
                ts = db.trouble_shooting.Find(id);

                string subPathSign = "~/Attachment/trouble_shooting/" + id + "/signatures"; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPathSign));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPathSign));
                try
                {
                    string old_path = Server.MapPath("~/Attachment/trouble_shooting/" + id_before + "/signatures");
                    string new_path = Server.MapPath("~/Attachment/trouble_shooting/" + id + "/signatures");
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/trouble_shooting/" + id_before + "/signatures"), "*.*", SearchOption.TopDirectoryOnly)
                                select new
                                {
                                    File = file
                                };

                    foreach (var f in files)
                    {
                        ts.inspector_signature = "/Attachment/trouble_shooting/" + id + "/signatures/" + f.File.Substring(f.File.LastIndexOf("\\") + 1);
                        System.IO.File.Move(old_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1), new_path + "/" + f.File.Substring(f.File.LastIndexOf("\\") + 1));
                    }

                    db.Entry(ts).State = EntityState.Modified;
                    db.SaveChanges();
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

            return Json(new { ts_no = ts_no });
        }

        [HttpPost]
        public JsonResult Edit(trouble_shooting troubleShooting)
        {
            trouble_shooting ts = db.trouble_shooting.Find(troubleShooting.id);

            ts.no = troubleShooting.no;
            ts.equipment_no = troubleShooting.equipment_no;
            ts.equipment_description = troubleShooting.equipment_description;
            ts.date_of_trouble = troubleShooting.date_of_trouble;
            ts.time_of_trouble = troubleShooting.time_of_trouble;
            ts.start_date_repair = troubleShooting.start_date_repair;
            ts.end_date_repair = troubleShooting.end_date_repair;
            ts.time_repaired = troubleShooting.time_repaired;
            ts.wo_number = troubleShooting.wo_number;
            ts.description_trouble = troubleShooting.description_trouble;
            ts.as_found_condition = troubleShooting.as_found_condition;
            ts.analysis = troubleShooting.analysis;
            ts.action = troubleShooting.action;
            ts.recommendation = troubleShooting.recommendation;

            db.Entry(ts).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        #region approval reject
        [HttpPost]
        public ActionResult approveSuperintendent(int id, int employee_id, DateTime date)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                trouble_shooting ts = db.trouble_shooting.Find(id);
                if (ts.superintendent_approval_name == employee_id.ToString())
                {
                    ts.superintendent_approval_signature = "a" + sign;
                }
                else
                {
                    ts.superintendent_approval_signature = "d" + sign;
                }
                ts.superintendent_approval_date = date;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, path = sign });
            }
            else
            {
                return Json(new { success = false });
            }

        }

        [HttpPost]
        public ActionResult approveSupervisor(int id, int employee_id, DateTime date)
        {
            string sign = db.employees.Find(employee_id).signature;
            if (sign != null)
            {
                trouble_shooting ts = db.trouble_shooting.Find(id);
                if (ts.supervisor_approval_name == employee_id.ToString())
                {
                    ts.supervisor_approval_signature = "a" + sign;
                }
                else
                {
                    ts.supervisor_approval_signature = "d" + sign;
                }
                ts.supervisor_approval_date = date;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
                if (ts.superintendent_approval_name != null && ts.superintendent_delegate == null)
                    SendEmailApprove(ts, Int32.Parse(ts.superintendent_approval_name));
                else if (ts.superintendent_delegate != null)
                    SendEmailApprove(ts, Int32.Parse(ts.superintendent_delegate));

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
            trouble_shooting ts = db.trouble_shooting.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            SendEmailToAll(ts, 2, comment);
            return Json(new { success = true });

        }

        [HttpPost]
        public ActionResult rejectSupervisor(int id, string comment)
        {
            trouble_shooting ts = db.trouble_shooting.Find(id);
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            SendEmailToAll(ts, 2, comment);
            return Json(new { success = true });
        }

        public void SendEmailApprove(trouble_shooting ts, int employeeId)
        {
            var sendEmail = new SendEmailController();
            employee e = db.employees.Find(employeeId);
            if (e.email != null)
            {
                List<string> s = new List<string>();
                s.Add(e.email);
                sendEmail.Send(s, "Bapak/Ibu,<br />Mohon review dan approval untuk Troubleshooting Report dengan nomor referensi " + ts.no + ".Terima Kasih.<br /><br /><i>Dear Sir/Madam,<br />Please review and approval for Troubleshooting Report with reference number " + ts.no + ".Thank you.</i><br /><br />Salam,<br /><i>Regards,</i><br /> Sytem Fracas Application", "Approving Troubleshooting Report " + ts.no);
            }
        }

        //reject type = 2
        public void SendEmailToAll(trouble_shooting ts, int type = 1, string comment = "")
        {
            employee e;
            List<String> s = new List<string>();
            var sendEmail = new SendEmailController();
            if (ts.superintendent_approval_name != null && ts.superintendent_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(ts.superintendent_approval_name));
                if (e.email != null)
                    s.Add(e.email);

            }
            else if (ts.superintendent_delegate != null)
            {
                e = db.employees.Find(Int32.Parse(ts.superintendent_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (ts.supervisor_approval_name != null && ts.supervisor_delegate == null)
            {
                e = db.employees.Find(Int32.Parse(ts.supervisor_approval_name));
                if (e.email != null)
                    s.Add(e.email);
            }
            else if (ts.supervisor_delegate != null)
            {
                e = db.employees.Find(Int32.Parse(ts.supervisor_delegate));
                if (e.email != null)
                    s.Add(e.email);
            }

            if (ts.inspector_name != null)
            {
                e = db.employees.Find(Int32.Parse(ts.inspector_name));
                if (e.email != null)
                    s.Add(e.email);
            }
            if (s.Count > 0)
            {
                if (type == 2)
                    sendEmail.Send(s, "Bapak/Ibu,<br />Dokumen berikut dengan no referensi " + ts.no + " kami kembalikan untuk diperbaiki sesuai dengan alasan di bawah.Terima Kasih.<br />Alasan :<br />" + comment + "<br /><br /><i>Dear Sir/Madam,<br />Document with reference number " + ts.no + " need to be reviewed in accordance with the following reasons .Thank you.<br />Reasons :<br />" + comment + "</i><br /><br />Salam,<br /><i>Regards,</i><br />" + db.employees.Find(Int32.Parse(HttpContext.Session["id"].ToString())).alpha_name, "Rejected Troubleshooting Report " + ts.no);
            }
        }

        #endregion

        public ActionResult printTroubleShooting(int id)
        {
            return this.ViewPdf("", "TroubleShootingPrint", createTSModel(id));
        }

        private trouble_shooting createTSModel(int id)
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
            trouble_shooting ts = db.trouble_shooting.Find(id);
            if (ts.supervisor_approval_name != null) {
                ts.supervisor_approval_name = has.Find(p => p.id == Int32.Parse(ts.supervisor_approval_name)).alpha_name;
            }
            if (ts.superintendent_approval_name != null)
            {
                ts.superintendent_approval_name = has.Find(p => p.id == Int32.Parse(ts.superintendent_approval_name)).alpha_name;
            }
            if (ts.inspector_name != null)
            {
                ts.inspector_name = has.Find(p => p.id == Int32.Parse(ts.inspector_name)).alpha_name;
            }
            
            return ts;
        }

        public ActionResult ViewTroubleshooting(int id) {
            return PartialView(db.trouble_shooting.Find(id));
        }
    }
}
