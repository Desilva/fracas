using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc.UI;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;


namespace StarEnergi.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/admin" });
            }
            else {
                string username = (String)Session["username"].ToString();
                List<user_per_role> li = db.user_per_role.Where(p => p.username == username).ToList();
                if (!(li.Exists(p => p.role == (int)Config.role.ADMIN)))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/admin" });
                }
                ViewData["user_role"] = li;
            }
            ViewBag.Message = "Welcome to Admin Reliability Monitoring!";
            ViewBag.stat = "admin";
            List<plant> plant = db.plants.ToList();
            plant = plant.OrderBy(a => a.nama).ToList();
            foreach (plant p in plant) {
                p.focs = p.focs.OrderBy(a => a.nama).ToList();
            }
            
            return View(plant);
        }

        [HttpPost]
        public ActionResult _AjaxLoadingTreeViewEmployee(TreeViewItem node)
        {
            string[] value = node.Value.Split(';');
            int? parentId = (int?)Int32.Parse(value[1]);
            if (value[0] == Config.TreeType.PLANT.ToString())
            {
                IEnumerable nodes = from item in db.employee_dept
                                    where item.plant_id == parentId || (parentId == null && item.plant_id == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.dept_name,
                                        Value = "DEPARTMENT" + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.employees.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.employees.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == "DEPARTMENT")
            {
                IEnumerable nodes = from item in db.employees
                                    where item.employee_dept == parentId || (parentId == null && item.employee_dept == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.alpha_name,
                                        Value = "EMPLOYEEBOSS" + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.employee1.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.employee1.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == "EMPLOYEEBOSS")
            {
                IEnumerable nodes = from item in db.employees
                                    where item.employee_boss == parentId || (parentId == null && item.employee_boss == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.alpha_name,
                                        Value = "EMPLOYEE" + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.employee1.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.employee1.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == "EMPLOYEE")
            {
                IEnumerable nodes = from item in db.employees
                                    where item.employee_boss == parentId || (parentId == null && item.employee_boss == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.alpha_name,
                                        Value = "EMPLOYEE" + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.employee1.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.employee1.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }

            return new JsonResult { Data = null };
        }

        [HttpPost]
        public ActionResult _AjaxLoadingTreeView(TreeViewItem node)
        {
            string[] value = node.Value.Split(';');
            int? parentId = (int?)Int32.Parse(value[1]);
            if (value[0] == Config.TreeType.PLANT.ToString())
            {
                string area = Config.TreeType.AREA.ToString();
                IEnumerable nodes = from item in db.focs
                                    where item.plant_id == parentId || (parentId == null && item.plant_id == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.nama,
                                        Value = area + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.units.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.units.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.AREA.ToString())
            {
                string unit = Config.TreeType.UNIT.ToString();
                IEnumerable nodes = from item in db.units
                                    where item.id_foc == parentId || (parentId == null && item.id_foc == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.nama,
                                        Value = unit + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.systems.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.systems.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.UNIT.ToString())
            {
                string system = Config.TreeType.SYSTEM.ToString();
                IEnumerable nodes = from item in db.systems
                                    where item.id_unit == parentId || (parentId == null && item.id_unit == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.nama,
                                        Value = system + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.equipment_groups.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.equipment_groups.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.SYSTEM.ToString())
            {
                string equipment_group = Config.TreeType.EQUIPMENT_GROUP.ToString();
                IEnumerable nodes = from item in db.equipment_groups
                                    where item.id_system == parentId || (parentId == null && item.id_system == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.nama,
                                        Value = equipment_group + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.equipments.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.equipments.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.EQUIPMENT_GROUP.ToString())
            {
                string equipment = Config.TreeType.EQUIPMENTS.ToString();
                IEnumerable nodes = from item in db.equipments
                                    where item.id_equipment_group == parentId || (parentId == null && item.id_equipment_group == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.nama,
                                        Value = equipment + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.equipment_part.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.equipment_part.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.EQUIPMENTS.ToString())
            {
                string part = Config.TreeType.PART.ToString();
                IEnumerable nodes = from item in db.equipment_part
                                    where item.id_equipment == parentId || (parentId == null && item.id_equipment == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.part.nama,
                                        Value = part + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.components.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.components.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.PART.ToString())
            {
                string component = Config.TreeType.COMPONENT.ToString();
                IEnumerable nodes = from item in db.components
                                    where item.id_equipment_part == parentId || (parentId == null && item.id_equipment_part == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.description,
                                        Value = component + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        LoadOnDemand = item.sub_component.Count > 0,
                                        Enabled = true,
                                        ImageUrl = item.sub_component.Count > 0 ? ("/Content/image/folder.png") : ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }
            else if (value[0] == Config.TreeType.COMPONENT.ToString())
            {
                string subcomponent = Config.TreeType.SUBCOMPONENT.ToString();
                IEnumerable nodes = from item in db.sub_component
                                    where item.id_component == parentId || (parentId == null && item.id_component == null)
                                    select new TreeViewItemModel
                                    {
                                        Text = item.description,
                                        Value = subcomponent + ";" + SqlFunctions.StringConvert((double)item.id).Trim(),
                                        Enabled = true,
                                        ImageUrl = ("/Content/image/file.png")
                                    };
                return new JsonResult { Data = nodes };
            }

            return new JsonResult { Data = null };
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
