using ReportManagement;
using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.FrontEnd
{
    public class EquipmentDailyReportController : PdfViewController
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public static List<user_per_role> li;
        //
        // GET: /EquipmentDailyReport/

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/EquipmentDailyReport" });
            }
            else
            {
                var has = (from users in db.users
                           select new UserEntity { username = users.username, fullname = users.fullname, jabatan = users.jabatan }).ToList();
                ViewData["users"] = has;
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!li.Exists(p => p.role == (int)Config.role.DAILYLOG))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/EquipmentDailyReport" });
                }
                ViewData["user_role"] = li;
                return View();
            }
            
        }

        public ActionResult addEquipmentDailyReport(int? id)
        {
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       where employees.department == "Production"
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
            ViewBag.employee = has;
            if (id != null)
            {
                ViewBag.mod = id;
                equipment_daily_report es = db.equipment_daily_report.Find(id);

                var equip = (from e in db.equipments
                         select new EquipmentTableReportEntity
                         {
                             id_equipment = e.id,
                             tag_number = e.tag_num,
                             description = e.nama
                         }
                    );

                var x = (from e in db.equipment_daily_report_table
                         where e.id_equipment_daily_report == id
                         select new EquipmentTableReportEntity
                         {
                             id_equipment = e.id_equipment
                         }
                    );
                List<EquipmentTableReportEntity> etre = equip.ToList();
                List<EquipmentTableReportEntity> xxx = x.ToList();
                if (xxx.Count != etre.Count)
                {
                    foreach (EquipmentTableReportEntity e in etre)
                    {
                        if (xxx.Find(p => p.id_equipment == e.id_equipment) == null)
                        {
                            equipment_daily_report_table edrt = new equipment_daily_report_table
                            {
                                id_equipment = e.id_equipment,
                                id_equipment_daily_report = id
                            };

                            db.equipment_daily_report_table.Add(edrt);
                            db.SaveChanges();
                        }

                    }
                }

                var r = (from e in db.equipment_daily_report_table
                         join f in db.equipments on e.id_equipment equals f.id
                         where e.id_equipment_daily_report == id
                         select new EquipmentTableReportEntity
                         {
                             id = e.id,
                             id_equipment_daily_report = e.id_equipment_daily_report,
                             id_equipment = e.id_equipment,
                             tag_number = f.tag_num,
                             description = f.nama,
                             barcode = e.barcode,
                             min_limit = e.min_limit,
                             max_limit = e.max_limit,
                             tag_value = e.tag_value,
                             unit = e.unit,
                             time = e.time,
                             name_operator = e.name_operator,
                             keterangan = e.keterangan
                         }
                    );
                EquipmentReportEntity eq = new EquipmentReportEntity
                {
                    id = es.id,
                    date = es.date,
                    operator_name = r.ToList().First().name_operator,
                    table = r.ToList()
                };

                ViewBag.datas = eq;
            }
            else
            {
                var r = (from e in db.equipments
                         select new EquipmentTableReportEntity
                        {
                            id_equipment = e.id,
                            tag_number = e.tag_num,
                            description = e.nama
                        }
                    );

                var x = (from e in db.equipment_daily_report_table
                         where e.id_equipment_daily_report == null
                         select new EquipmentTableReportEntity
                         {
                             id_equipment = e.id_equipment
                         }
                    );
                List<EquipmentTableReportEntity> etre = r.ToList();
                List<EquipmentTableReportEntity> xxx = x.ToList();
                if (xxx.Count != etre.Count)
                {
                    foreach (EquipmentTableReportEntity e in etre)
                    {
                        if (xxx.Find(p => p.id_equipment == e.id_equipment) == null)
                        {
                            equipment_daily_report_table es = new equipment_daily_report_table
                            {
                                id_equipment = e.id_equipment
                            };

                            db.equipment_daily_report_table.Add(es);
                            db.SaveChanges();
                        }
                        
                    }
                }

                ViewData["etre"] = etre;
            }
            return PartialView();
        }

        public ActionResult detailEquipmentDailyReport(int? id)
        {
            ViewBag.mod = id;
            equipment_daily_report es = db.equipment_daily_report.Find(id);

            var equip = (from e in db.equipments
                            select new EquipmentTableReportEntity
                            {
                                id_equipment = e.id,
                                tag_number = e.tag_num,
                                description = e.nama
                            }
                );

            var x = (from e in db.equipment_daily_report_table
                        where e.id_equipment_daily_report == id
                        select new EquipmentTableReportEntity
                        {
                            id_equipment = e.id_equipment
                        }
                );
            List<EquipmentTableReportEntity> etre = equip.ToList();
            List<EquipmentTableReportEntity> xxx = x.ToList();
            if (xxx.Count != etre.Count)
            {
                foreach (EquipmentTableReportEntity e in etre)
                {
                    if (xxx.Find(p => p.id_equipment == e.id_equipment) == null)
                    {
                        equipment_daily_report_table edrt = new equipment_daily_report_table
                        {
                            id_equipment = e.id_equipment,
                            id_equipment_daily_report = id
                        };

                        db.equipment_daily_report_table.Add(edrt);
                        db.SaveChanges();
                    }

                }
            }

            var r = (from e in db.equipment_daily_report_table
                        join f in db.equipments on e.id_equipment equals f.id
                        where e.id_equipment_daily_report == id
                        select new EquipmentTableReportEntity
                        {
                            id = e.id,
                            id_equipment_daily_report = e.id_equipment_daily_report,
                            id_equipment = e.id_equipment,
                            tag_number = f.tag_num,
                            description = f.nama,
                            barcode = e.barcode,
                            min_limit = e.min_limit,
                            max_limit = e.max_limit,
                            tag_value = e.tag_value,
                            unit = e.unit,
                            time = e.time,
                            name_operator = e.name_operator,
                            keterangan = e.keterangan
                        }
                );
            EquipmentReportEntity eq = new EquipmentReportEntity
            {
                id = es.id,
                date = es.date,
                operator_name = r.ToList().First().name_operator,
                table = r.ToList()
            };

            ViewBag.datas = eq;
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
        public ActionResult _SelectAjaxEquipmentDailyReport()
        {
            return bindingEquipmentDailyReport();
        }

        //select data incident report
        private ViewResult bindingEquipmentDailyReport()
        {
            List<EquipmentReportEntity> f = new List<EquipmentReportEntity>();
            var d = (from c in db.equipment_daily_report
                     select new EquipmentReportEntity()
                     {
                         id = c.id,
                         date = c.date,
                     }
                );
            f = d.ToList();

            return View(new GridModel<EquipmentReportEntity>
            {
                Data = f.OrderByDescending(p => p.date)
            });
        }

        [HttpPost]
        public JsonResult DeleteAllTable()
        {
            List<equipment_daily_report_table> li = db.equipment_daily_report_table.Where(p => p.id_equipment_daily_report == null).ToList();

            foreach (equipment_daily_report_table ir in li)
            {
                db.equipment_daily_report_table.Remove(ir);
                db.SaveChanges();
            }
            return Json(true);
        }

        //
        // Ajax delete binding daily log
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEquipmentDailyReport(int id)
        {
            deleteEquipmentDailyReport(id);
            return bindingEquipmentDailyReport();
        }

        //delete data daily log
        private void deleteEquipmentDailyReport(int id)
        {
            equipment_daily_report edr = db.equipment_daily_report.Find(id);
            db.equipment_daily_report.Remove(edr);
            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult Add(EquipmentReportEntity ere)
        {
            equipment_daily_report edr = new equipment_daily_report()
            {
                date = ere.date
            };
            db.equipment_daily_report.Add(edr);
            db.SaveChanges();

            int id = db.equipment_daily_report.Max(p => p.id);

            List<equipment_daily_report_table> li = db.equipment_daily_report_table.Where(p => p.id_equipment_daily_report == null).ToList();
            foreach (equipment_daily_report_table l in li)
            {
                EquipmentTableReportEntity etre = ere.table.Find(p => p.id_equipment == l.id_equipment);
                l.id_equipment_daily_report = id;
                l.barcode = etre.barcode;
                l.min_limit = etre.min_limit;
                l.max_limit = etre.max_limit;
                l.tag_value = etre.tag_value;
                l.unit = etre.unit;
                l.time = etre.time;
                l.name_operator = ere.operator_name;
                l.keterangan = etre.keterangan;
                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public JsonResult Edit(EquipmentReportEntity ere)
        {
            int id = ere.id;

            equipment_daily_report ed = db.equipment_daily_report.Find(id);
            ed.date = ere.date;
            db.Entry(ed).State = EntityState.Modified;
            db.SaveChanges();

            List<equipment_daily_report_table> li = db.equipment_daily_report_table.Where(p => p.id_equipment_daily_report == id).ToList();
            foreach (EquipmentTableReportEntity l in ere.table)
            {

                equipment_daily_report_table etre = li.Find(p => p.id_equipment == l.id_equipment);
                if (etre != null)
                {
                    etre.barcode = l.barcode;
                    etre.min_limit = l.min_limit;
                    etre.max_limit = l.max_limit;
                    etre.tag_value = l.tag_value;
                    etre.unit = l.unit;
                    etre.time = l.time;
                    etre.name_operator = ere.operator_name;
                    etre.keterangan = l.keterangan;
                    db.Entry(etre).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
            }

            return Json(true);
        }

        public ActionResult printEDR(int id)
        {
            equipment_daily_report edr = db.equipment_daily_report.Find(id);

            var r = (from e in db.equipment_daily_report_table
                    join f in db.equipments on e.id_equipment equals f.id
                    where e.id_equipment_daily_report == id
                    select new EquipmentTableReportEntity
                    {
                        id = e.id,
                        id_equipment_daily_report = e.id_equipment_daily_report,
                        id_equipment = e.id_equipment,
                        tag_number = f.tag_num,
                        description = f.nama,
                        barcode = e.barcode,
                        min_limit = e.min_limit,
                        max_limit = e.max_limit,
                        tag_value = e.tag_value,
                        unit = e.unit,
                        time = e.time,
                        name_operator = e.name_operator,
                        keterangan = e.keterangan
                    }
            );

            EquipmentReportEntity ere = new EquipmentReportEntity()
            {
                id = edr.id,
                date = edr.date,
                table = r.ToList()
            };
            return this.ViewPdf("", "EquipmentDailyReportPrint", ere);
        }
    }
}
