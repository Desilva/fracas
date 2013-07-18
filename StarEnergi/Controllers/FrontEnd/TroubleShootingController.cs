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
            ViewBag.Nama = "Trouble Shooting";
            return View();
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
            }
            else
            {
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
            f = db.trouble_shooting.ToList();

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
            troubleShooting.no = ts_no;
            db.trouble_shooting.Add(troubleShooting);
            db.SaveChanges();
            int id = db.trouble_shooting.Max(p => p.id);

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
            ts.inspector_name = troubleShooting.inspector_name;
            ts.approval_name = troubleShooting.approval_name;
            ts.inspector_date = troubleShooting.inspector_date;
            ts.approval_date = troubleShooting.approval_date;

            db.Entry(ts).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult approveInspector(HttpPostedFileBase inspector_signature, int? id)
        {
            var currpath = "";
            if (id == null)
            {
                id = db.trouble_shooting.Max(p => p.id) + 1;
            }
            if (inspector_signature != null && inspector_signature.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/trouble_shooting/" + id + "/signatures"),
                    inspector_signature.FileName
                );
                inspector_signature.SaveAs(currpath);

                trouble_shooting ir = db.trouble_shooting.Find(id);
                if (ir != null)
                {
                    currpath = "/Attachment/trouble_shooting/" + id + "/signatures/" + inspector_signature.FileName;
                    ir.inspector_signature = currpath;
                    db.Entry(ir).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new { success = true, path = currpath });
        }

        [HttpPost]
        public ActionResult approvalSignature(HttpPostedFileBase approval_signature, int id)
        {
            var currpath = "";
            if (approval_signature != null && approval_signature.ContentLength > 0)
            {
                currpath = Path.Combine(
                    Server.MapPath("~/Attachment/trouble_shooting/" + id + "/signatures"),
                    approval_signature.FileName
                );
                approval_signature.SaveAs(currpath);

                trouble_shooting ts = db.trouble_shooting.Find(id);
                currpath = "/Attachment/trouble_shooting/" + id + "/signatures/" + approval_signature.FileName;
                ts.approval_signature = currpath;
                db.Entry(ts).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true, path = currpath });
        }

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
            if (ts.approval_name != null) {
                ts.approval_name = has.Find(p => p.id == Int32.Parse(ts.approval_name)).alpha_name;
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
