using ReportManagement;
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
    public class HseInjuryReportController : PdfViewController
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;

        //
        // GET: /HseInjuryReport/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseInjuryReport" });
            }
            else
            {
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!li.Exists(p => p.role == (int)Config.role.MEDIC))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/HseInjuryReport" });
                }
                ViewBag.Nama = "SHE Injury Report";
                return View();
            }
            
        }

        public ActionResult report()
        {
            return PartialView();
        }

        public ActionResult addInjuryReport(int? id)
        {
            if (id != null)
            {
                ViewBag.mod = id;
                ViewBag.datas = db.she_injury_report.Find(id);
            }
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
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewData["users"] = has;



            int last_id = db.she_injury_report.ToList().Count == 0 ? 0 : db.she_injury_report.Max(p => p.id);
            last_id++;
            string subPath = "~/Attachment/injury_report/" + last_id; // your code goes here
            bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
            if (!IsExists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            return PartialView();
        }

        //
        // Ajax select binding illness report
        [GridAction]
        public ActionResult _SelectAjaxInjuryReport()
        {
            return bindingInjuryReport();
        }

        //select data incident report
        private ViewResult bindingInjuryReport()
        {
            List<she_injury_report> f = new List<she_injury_report>();
            f = db.she_injury_report.ToList();
            foreach (she_injury_report injury in f)
            {
                incident_report ir = db.incident_report.Find(injury.id_ir);
                if (ir != null)
                {
                    injury.ir_number = ir.reference_number;
                }
            }
            return View(new GridModel<she_injury_report>
            {
                Data = f
            });
        }

        [HttpPost]
        public JsonResult Add(she_injury_report injuryReport, string time_start_bef_acc, string time_survey, string time_trauma)
        {
            int id_before = (db.she_injury_report.ToList().Count == 0 ? 0 : db.she_injury_report.Max(p => p.id)) + 1;

            injuryReport.time_start_bef_acc = DateTime.Parse(time_start_bef_acc).TimeOfDay;
            injuryReport.time_survey = DateTime.Parse(time_survey).TimeOfDay;
            injuryReport.time_trauma = DateTime.Parse(time_trauma).TimeOfDay;
            db.she_injury_report.Add(injuryReport);
            db.SaveChanges();
            int id = injuryReport.id;
            if (id != id_before)
            {
                string subPathSign = "~/Attachment/injury_report/" + id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPathSign));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPathSign));

                try
                {
                    string old_path = Server.MapPath("~/Attachment/injury_report/" + id_before);
                    string new_path = Server.MapPath("~/Attachment/injury_report/" + id);
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/injury_report/" + id_before), "*.*", SearchOption.TopDirectoryOnly)
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
            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(she_injury_report injuryReport, string time_start_bef_acc, string time_survey, string time_trauma)
        {
            injuryReport.time_start_bef_acc = DateTime.Parse(time_start_bef_acc).TimeOfDay;
            injuryReport.time_survey = DateTime.Parse(time_survey).TimeOfDay;
            injuryReport.time_trauma = DateTime.Parse(time_trauma).TimeOfDay;
            db.Entry(injuryReport).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        public ActionResult DetailInjuryReport(int id)
        {
            she_injury_report details = new she_injury_report();
            details = db.she_injury_report.Find(id);

            ViewBag.nama = "Detail Injury Report ";
            return PartialView(details);
        }

        public ActionResult printInjuryReport(int id)
        {
            she_injury_report dl = db.she_injury_report.Find(id);
            int user_id = 0;
            if (dl.signed != null)
                user_id = Int32.Parse(dl.signed);
            employee emp = db.employees.Find(user_id);
            dl.signed = emp != null ? emp.alpha_name : "";
            return this.ViewPdf("", "injuryReportPrint", dl);
        }

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
            if (id == null) id = (db.she_injury_report.ToList().Count == 0 ? 0 : db.she_injury_report.Max(p => p.id)) + 1;
            if (attachment != null)
            {
                foreach (var file in attachment)
                {
                    currpath = Path.Combine(
                    Server.MapPath("~/Attachment/injury_report/" + id),
                    file.FileName);
                    file.SaveAs(currpath);
                }
                currpath = "/Attachment/injury_report/" + id + "/";
                try
                {
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/injury_report/" + id), "*.*", SearchOption.TopDirectoryOnly)
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
        public ActionResult Atch(int? id)
        {
            if (id == null) id = (db.incident_report.ToList().Count == 0 ? 0 : db.incident_report.Max(p => p.id)) + 1;
            var currpath = "/Attachment/injury_report/" + id + "/";
            string st = "";
            try
            {
                var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/injury_report/" + id), "*.*", SearchOption.TopDirectoryOnly)
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
                var currpath = "/Attachment/injury_report/" + id + "/";
                try
                {
                    string new_path = Server.MapPath("~/Attachment/injury_report/" + id);
                    var files = from file in Directory.EnumerateFiles(Server.MapPath("~/Attachment/injury_report/" + id), "*.*", SearchOption.TopDirectoryOnly)
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

    }
}
