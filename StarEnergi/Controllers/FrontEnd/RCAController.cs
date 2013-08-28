using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Web.UI.WebControls;
using StarEnergi.Utilities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Drawing.Imaging;
using com.mxgraph;
using System.Web.Script.Serialization;
using System.Data;
using StarEnergi.Controllers.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class RCAController : Controller
    {
        public static List<user_per_role> li;

        //==============================================================

        #region index
        //
        // GET: /RCA/
        public ActionResult Index(int? isTree)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree="+isTree });
            }
            else
            {
                string username = (String)Session["username"].ToString();
                li = RCASessionRepository.db.user_per_role.Where(p => p.username == username).ToList();
                if (!(li.Exists(p => p.role == (int)Config.role.RCA)) && !(li.Exists(p => p.role == (int)Config.role.RCAVIEW)))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=" + isTree });
                }

                ViewBag.stat = "rca";
            }
            if (isTree != null)
            {
                HttpContext.Session["isTree"] = isTree;
                Debug.WriteLine(isTree);
            }

            if (HttpContext.Session["done"] != null && HttpContext.Session["done"].ToString() == "done")
            {
                if (HttpContext.Session["id_analysis"] != null)
                {
                    RCAEntityModel rca = new RCAEntityModel();
                    rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                    RCASessionRepository.Delete(rca);
                }
            }

            HttpContext.Session.Remove("id_analysis");
            ViewData["user_role"] = li;
            ViewBag.isTree = Int32.Parse(HttpContext.Session["isTree"].ToString());
            Debug.WriteLine(HttpContext.Session["isTree"].ToString());
            return View();
        }

        #endregion

        //==============================================================

        #region addRCA

        #region addRCA1
        //
        // GET: /RCA/addRCA

        public ActionResult addRCA(int? id, int? isTree, int? id_eq, int? i, string a, string c)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca/addRCA?isTree=" + isTree + "&id_eq=" + id_eq + "&i=" + i + "&a=" + a });
            }
            else
            {
                string username = (String)Session["username"].ToString();
                li = RCASessionRepository.db.user_per_role.Where(p => p.username == username).ToList();
                if (!(li.Exists(p => p.role == (int)Config.role.RCA)) && !(li.Exists(p => p.role == (int)Config.role.RCAVIEW)))
                {
                    return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca/addRCA?isTree=" + isTree + "&id_eq=" + id_eq + "&i=" + i + "&a=" + a });
                }

                ViewBag.stat = "rca";
            }
            int ids = 0;
            Debug.WriteLine("ids = " + id);
            if (isTree != null)
            {
                HttpContext.Session["isTree"] = isTree;
                Debug.WriteLine("isTree = " + HttpContext.Session["isTree"].ToString());
            }
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            if (id_eq != null) ViewBag.id_eq = id_eq;
            if (i != null) ViewBag.i = i;
            if (a != null) ViewBag.a = a;
            if (c != null) ViewBag.c = c;
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            if (rca != null)
            {
                ViewBag.analysis_Description = rca.description;
                ViewBag.analysis_Name = rca.name;
                ViewBag.id_type = rca.id_type.ToString();
                ViewBag.type_name = rca.type_name;
                ViewBag.benefit = rca.cost;
                ViewBag.isPublish = rca.is_publish;
            }

            List<rca_analisys_type> result = RCASessionRepository.db.rca_analisys_type.ToList();
            ViewData["user_role"] = li;
            return View(result);
        }

        //
        // POST: /RCA/addRCA

        [HttpPost]
        public ActionResult addRCA(string next, string cancel,
            string analysis_Name, string analysis_Description, string id_type,
            string benefit, string types, string isPublish, string id_eq, string i)
        {
            var button = next ?? cancel;
            if (button == "Next")
            {
                RCAEntityModel rcas = new RCAEntityModel();
                if (HttpContext.Session["id_analysis"] == null)
                {
                    rcas.is_publish = 0;
                    rcas.name = analysis_Name;
                    rcas.description = analysis_Description;
                    rcas.id_type = Int32.Parse(id_type);
                    rcas.cost = benefit;
                    if (HttpContext.Session["isTree"].ToString() != null)
                        rcas.is_tree = Byte.Parse(HttpContext.Session["isTree"].ToString());
                    else
                        return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1" });
                    rcas.has_pir = 0;

                    //RCAEntityModel rcass = RCASessionRepository.AllView().LastOrDefault();
                    //if (rcass != null)
                    //{
                    //    if (rcass.rca_code != null && rcass.rca_code.Length == 21)
                    //    {
                    //        int prev_code = Int32.Parse(rcass.rca_code.Substring(17));
                    //        int prev_year = Int32.Parse(rcass.rca_code.Substring(12,4));
                    //        prev_code++;
                    //        DateTime now = DateTime.Now;
                    //        if (prev_year != now.Year)
                    //        {
                    //            prev_code = 1;
                    //        }
                    //        rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                    //    }
                    //    else
                    //    {
                    //        int prev_code = 1;
                    //        DateTime now = DateTime.Now;
                    //        rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                    //    }
                    //}
                    //else
                    //{
                    //    int prev_code = 1;
                    //    DateTime now = DateTime.Now;
                    //    rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                    //}

                    if (id_eq != null && id_eq != "" && i != null)
                    {
                        equipment eq = RCASessionRepository.db.equipments.Find(Int32.Parse(id_eq));
                        rcas.id_equipment_class = eq.id_discipline;
                        rcas.id_type_equipment = eq.id_tag_type;
                        rcas.equipment_code = eq.tag_num;
                        rcas.fracas_ir_id = Int32.Parse(i);
                        rcas.fracas_ir = 1;
                    }
                    else if (i != null && i != "")
                    {
                        rcas.fracas_ir_id = Int32.Parse(i);
                        rcas.fracas_ir = 2;
                    }

                    HttpContext.Session["id_analysis"] = RCASessionRepository.Insert(rcas);
                    return RedirectToAction("addRCA2");
                }
                else
                {
                    
                    rcas.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                    if (isPublish == "0")
                    {   
                        rcas.name = analysis_Name;
                        rcas.description = analysis_Description;
                        rcas.id_type = Int32.Parse(id_type);
                        rcas.cost = benefit;

                        RCASessionRepository.UpdateRCA(rcas);
                    }
                    return RedirectToAction("addRCA2", new { id = rcas.id });
                }

            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                        rca.name = analysis_Name;
                        rca.description = analysis_Description;
                        rca.id_type = Int32.Parse(id_type);
                        rca.cost = benefit;

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }


                return RedirectToAction("Index");
            }

        }

        #endregion

        //==============================================================

        #region addRCA2
        //
        // POST: /RCA/getDivision

        [HttpPost]
        public JsonResult getDivision(string selectedFacility)
        {
            int selectFacility = 0;
            if (selectedFacility != null)
                selectFacility = Int32.Parse(selectedFacility);
            var query = RCASessionRepository.db.rca_facility.Where(p => p.id == selectFacility).Join(RCASessionRepository.db.rca_department
,
                               f => f.id,
                               d => d.id_facility,
                               (f, d) =>
                                   new { id_division = d.id, value = d.name });
            Debug.WriteLine("selected Facility = " + selectedFacility);
            return Json(query);
        }

        //
        // POST: /RCA/getDepartment

        [HttpPost]
        public JsonResult getDepartment(string selectedDivision)
        {
            int selectFacility = 0;
            if (selectedDivision != null)
                selectFacility = Int32.Parse(selectedDivision);
            var query = RCASessionRepository.db.rca_department.Where(p => p.id == selectFacility).Join(RCASessionRepository.db.rca_section,
                               f => f.id,
                               d => d.id_department,
                               (f, d) =>
                                   new { id_department = d.id, value = d.name });
            Debug.WriteLine("selected Division = " + selectedDivision);
            return Json(query);
        }

        //
        // POST: /RCA/getBuilding

        [HttpPost]
        public JsonResult getBuilding(string selectedDepartment)
        {
            int selectFacility = 0;
            if (selectedDepartment != null)
                selectFacility = Int32.Parse(selectedDepartment);
            var query = RCASessionRepository.db.rca_department.Where(p => p.id == selectFacility).Join(RCASessionRepository.db.rca_building,
                               f => f.id,
                               d => d.id_department,
                               (f, d) =>
                                   new { id_building = d.id, value = d.name });
            Debug.WriteLine("selected Division = " + selectedDepartment);
            return Json(query);
        }

        //
        // POST: /RCA/getFloor

        [HttpPost]
        public JsonResult getFloor(string selectedBuilding)
        {
            int selectFacility = 0;
            if (selectedBuilding != null)
                selectFacility = Int32.Parse(selectedBuilding);
            var query = RCASessionRepository.db.rca_building.Where(p => p.id == selectFacility).Join(RCASessionRepository.db.rca_floor,
                               f => f.id,
                               d => d.id_building,
                               (f, d) =>
                                   new { id_floor = d.id, value = d.name });
            Debug.WriteLine("selected Division = " + selectedBuilding);
            return Json(query);
        }

        //
        // GET: /RCA/addRCA2

        public ActionResult addRCA2(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            if (rca != null)
            {
                ViewBag.id_facility = rca.id_facility.ToString();
                if (rca.id_facility != null)
                {
                    ViewBag.facilities = rca.facility;
                    ViewData["divisions"] = RCASessionRepository.db.rca_department.Where(p => p.id_facility == rca.id_facility).ToList();
                }
                ViewBag.id_division = rca.id_division.ToString();
                if (rca.id_division != null)
                {
                    ViewBag.division = rca.division;
                    ViewData["departments"] = RCASessionRepository.db.rca_section.Where(p => p.id_department == rca.id_division).ToList();
                }
                ViewBag.id_department = rca.id_department.ToString();
                if (rca.id_department != null)
                {
                    ViewBag.department = rca.department;
                    ViewData["buildings"] = RCASessionRepository.db.rca_building.Where(p => p.id_department == rca.id_department).ToList();
                }
                ViewBag.id_building = rca.id_building.ToString();
                if (rca.id_building != null)
                {
                    ViewBag.building = rca.building;
                    ViewData["floors"] = RCASessionRepository.db.rca_floor.Where(p => p.id_building == rca.id_building).ToList();
                }
                ViewBag.id_floor = rca.id_floor.ToString();
                if (rca.id_floor != null)
                {
                    ViewBag.floor = rca.floor;
                }
                ViewBag.functional_location = rca.functional_location;
                ViewBag.isPublish = rca.is_publish;
                Debug.WriteLine("division = " + rca.id_division + ";");
            }
            ViewData["facility"] = RCASessionRepository.db.rca_facility.ToList();
            ViewData["user_role"] = li;
            return View();
        }

        //
        // POST: /RCA/addRCA2

        [HttpPost]
        public ActionResult addRCA2(string next, string cancel, string previous,
            string id_facility, string id_division, string id_department, string id_building,
            string id_floor, string id_wing, string functional_location, string types, string isPublish)
        {
            var button = next ?? cancel ?? previous;
            if (button == "Next")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    if (id_facility != null && id_facility != "")
                        rca.id_facility = Int32.Parse(id_facility);
                    if (id_division != null && id_division != "")
                        rca.id_division = Int32.Parse(id_division);
                    if (id_department != null && id_department != "")
                        rca.id_department = Int32.Parse(id_department);
                    if (id_building != null && id_building != "")
                        rca.id_building = Int32.Parse(id_building);
                    if (id_floor != null && id_floor != "")
                        rca.id_floor = Int32.Parse(id_floor);
                    rca.functional_location = functional_location;
                    
                    RCASessionRepository.UpdateRCA2(rca);
                }

                if (types == "add")
                    return RedirectToAction("addRCA3");
                else
                    return RedirectToAction("addRCA3", new { id = rca.id });
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    Debug.WriteLine("id = " + rca.id);
                    if (id_facility != null && id_facility != "")
                        rca.id_facility = Int32.Parse(id_facility);
                    if (id_division != null && id_division != "")
                        rca.id_division = Int32.Parse(id_division);
                    if (id_department != null && id_department != "")
                        rca.id_department = Int32.Parse(id_department);
                    if (id_building != null && id_building != "")
                        rca.id_building = Int32.Parse(id_building);
                    if (id_floor != null && id_floor != "")
                        rca.id_floor = Int32.Parse(id_floor);
                    rca.functional_location = functional_location;

                    RCASessionRepository.UpdateRCA2(rca);
                }
                if (types == "add")
                    return RedirectToAction("addRCA");
                else
                    return RedirectToAction("addRCA", new { id = rca.id });
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }
        }

        #endregion

        //==============================================================

        #region addRCA3

        //
        // GET: /RCA/addRCA3

        public ActionResult addRCA3(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
                ids = RCASessionRepository.db.rcas.Max(p => p.id);
            }

            
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            ViewBag.id_equipment_type = rca.id_type_equipment.ToString();
            ViewBag.equipment_type = rca.equipment_type;
            ViewBag.id_equipment_class = rca.id_equipment_class.ToString();
            ViewBag.equipment_class = rca.equipment_class;
            if (rca.equipment_code != "")
                ViewBag.equipment_code = rca.equipment_code;
            if (rca.equipment_code != null)
            {
                ViewData["equipmentCodes"] = RCASessionRepository.db.equipments.Where(p => p.id_tag_type == rca.id_type_equipment && p.id_discipline == rca.id_equipment_class).Select(p => p.tag_num).Distinct().ToList();
            }
            ViewBag.other = rca.other;
            ViewBag.manufacture = rca.manufacture;
            ViewBag.isPublish = rca.is_publish;

            ViewData["equipments"] = RCASessionRepository.db.tag_types.ToList();
            ViewData["equipmentClass"] = RCASessionRepository.db.disciplines.ToList();
            ViewData["user_role"] = li;
            return View();
        }

        //
        // POST: /RCA/getEquipmentClass

        [HttpPost]
        public JsonResult getEquipmentCode(string selectedType, string selectedClass)
        {
            int selectType = 0;
            if (selectedType != null && selectedType != "")
                selectType = Int32.Parse(selectedType);
            int selectClass = 0;
            if (selectedClass != null && selectedClass != "")
                selectClass = Int32.Parse(selectedClass);
            var query = RCASessionRepository.db.equipments.Where(p => p.id_tag_type == selectType && p.id_discipline == selectClass).Select(p => new { tag_num = p.tag_num }).Distinct();
            Debug.WriteLine("selected Class = " + selectedClass);
            return Json(query);
        }

        //
        // POST: /RCA/addRCA3

        [HttpPost]
        public ActionResult addRCA3(string next, string cancel, string previous,
            string equipment_type, string equipment_class, string equipment_code, string other,
            string manufacture, string types, string isPublish)
        {
            var button = next ?? cancel ?? previous;
            if (button == "Next")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    Debug.WriteLine("id = " + rca.id);
                    if (equipment_type != null)
                        rca.id_type_equipment = Int32.Parse(equipment_type);
                    if (equipment_class != null)
                    {
                        int id = 0;
                        if (Int32.TryParse(equipment_class, out id))
                            rca.id_equipment_class = id;
                    }
                    if (equipment_code != "Choose Equipment Type and Equipment Class")
                        rca.equipment_code = equipment_code;
                    rca.other = other;
                    rca.manufacture = manufacture;

                    RCASessionRepository.UpdateRCA3(rca);
                }
                if (types == "add")
                    return RedirectToAction("addRCA4");
                else
                    return RedirectToAction("addRCA4", new { id = rca.id });
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    Debug.WriteLine("id = " + rca.id);
                    if (equipment_type != null)
                        rca.id_type_equipment = Int32.Parse(equipment_type);
                    if (equipment_class != null)
                    {
                        int id = 0;
                        if (Int32.TryParse(equipment_class, out id))
                            rca.id_equipment_class = id;
                    }
                    if (equipment_code != "Choose Equipment Type and Equipment Class")
                        rca.equipment_code = equipment_code;
                    rca.other = other;
                    rca.manufacture = manufacture;

                    RCASessionRepository.UpdateRCA3(rca);
                }
                if (types == "add")
                    return RedirectToAction("addRCA2");
                else
                    return RedirectToAction("addRCA2", new { id = rca.id });
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }

        }

        #endregion

        //==============================================================

        #region addRCA4

        //
        // GET: /RCA/addRCA4

        public ActionResult addRCA4(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            ViewData["rcaConnector"] = RCASessionRepository.db.rca_csf_conector.Where(p => p.id_rca == id).ToList();
            ViewBag.isPublish = rca.is_publish;
            ViewData["csfs"] = RCASessionRepository.db.rca_csf.ToList();
            ViewData["user_role"] = li;
            return View();
        }

        //
        // POST: /RCA/addRCA4

        [HttpPost]
        public ActionResult addRCA4(string next, string cancel, string previous, string[] csf, string types, string isPublish)
        {
            var button = next ?? cancel ?? previous;
            if (button == "Next")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    CsfComparer ccs = new CsfComparer();
                    List<rca_csf_conector> lc = RCASessionRepository.db.rca_csf_conector.Where(p => p.id_rca == rca.id).ToList();
                    foreach (rca_csf_conector r in lc)
                    {
                        RCASessionRepository.db.rca_csf_conector.Remove(r);
                        RCASessionRepository.db.SaveChanges();
                    }
                    if (csf != null)
                    {
                        foreach (string n in csf)
                        {
                            Debug.WriteLine("n = " + n);
                            int a;
                            rca_csf_conector csfc = new rca_csf_conector();
                            if (Int32.TryParse(n, out a))
                            {
                                csfc = new rca_csf_conector()
                                {
                                    id_csf = Int32.Parse(n),
                                    id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString())
                                };
                            }
                            else
                            {
                                csfc = new rca_csf_conector()
                                {
                                    custom = n,
                                    id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString())
                                };
                            }

                            RCASessionRepository.db.rca_csf_conector.Add(csfc);
                            RCASessionRepository.db.SaveChanges();
                        }
                    }
                }
                if (types == "add")
                    return RedirectToAction("addRCA5");
                else
                    return RedirectToAction("addRCA5", new { id = rca.id });
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    CsfComparer ccs = new CsfComparer();
                    List<rca_csf_conector> lc = RCASessionRepository.db.rca_csf_conector.Where(p => p.id_rca == rca.id).ToList();
                    foreach (rca_csf_conector r in lc)
                    {
                        RCASessionRepository.db.rca_csf_conector.Remove(r);
                        RCASessionRepository.db.SaveChanges();
                    }
                    if (csf != null)
                    {
                        foreach (string n in csf)
                        {
                            Debug.WriteLine("n = " + n);
                            int a;
                            rca_csf_conector csfc = new rca_csf_conector();
                            if (Int32.TryParse(n, out a))
                            {
                                csfc = new rca_csf_conector()
                                {
                                    id_csf = Int32.Parse(n),
                                    id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString())
                                };
                            }
                            else
                            {
                                csfc = new rca_csf_conector()
                                {
                                    custom = n,
                                    id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString())
                                };
                            }

                            RCASessionRepository.db.rca_csf_conector.Add(csfc);
                            RCASessionRepository.db.SaveChanges();
                        }
                    }
                }

                if (types == "add")
                    return RedirectToAction("addRCA3");
                else
                    return RedirectToAction("addRCA3", new { id = rca.id });
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }

        }

        #endregion

        //==============================================================

        #region addRCA5

        //
        // GET: /RCA/addRCA5

        public ActionResult addRCA5(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            if (rca != null)
            {
                ViewBag.charter = rca.charter;
                ViewBag.comment = rca.comment;
                ViewBag.isPublish = rca.is_publish;
            }
            ViewData["user_role"] = li;
            return View();
        }

        //
        // GET: /RCA/Saves

        public ActionResult Saves(HttpPostedFileBase Charter)
        {
            string s = "";
            StreamReader sr = new StreamReader(Charter.InputStream);
            s = sr.ReadToEnd();
            return Json(new { cont = s }, "text/plain");
        }

        //
        // POST: /RCA/addRCA5

        [HttpPost]
        public ActionResult addRCA5(string next, string cancel, string previous, string charter, string comment, string types, string isPublish)
        {
            var button = next ?? cancel ?? previous;
            if (button == "Next")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    Debug.WriteLine("id = " + rca.id);
                    rca.charter = charter;
                    rca.comment = comment;

                    RCASessionRepository.UpdateRCA5(rca);
                }
                if (types == "add")
                    return RedirectToAction("addRCA6");
                else
                    return RedirectToAction("addRCA6", new { id = rca.id });
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                if (isPublish == "0")
                {    
                    Debug.WriteLine("id = " + rca.id);
                    rca.charter = charter;
                    rca.comment = comment;

                    RCASessionRepository.UpdateRCA5(rca);
                }
                if (types == "add")
                    return RedirectToAction("addRCA4");
                else
                    return RedirectToAction("addRCA4", new { id = rca.id });
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }

        }

        #endregion

        //==============================================================

        #region addRCA6

        //
        // GET: /RCA/addRCA6

        public ActionResult addRCA6(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            if (rca != null)
            {
                ViewData["teams"] = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == rca.id).ToList();
                ViewBag.isPublish = rca.is_publish;
                if (rca.id_team != null)
                    ViewBag.analyst = RCASessionRepository.db.rca_team_connector.Where(p => p.id == rca.id_team).FirstOrDefault().id_user;
            }
            // || p.role == (int)StarEnergi.Config.role.PIRINITIATOR
            List<user_per_role> upr_rca = RCASessionRepository.db.user_per_role.Where(p => p.role == (int)Config.role.RCA).ToList();
            List<user_per_role> upr_pir = RCASessionRepository.db.user_per_role.Where(p => p.role == (int)StarEnergi.Config.role.PIRINITIATOR).ToList();
            List<user_per_role> upr_rca_pir = upr_rca.Join(upr_pir, u => u.username, t => t.username, (u, t) => u).ToList();
            List<RCATeamEmployeeModel> rt = upr_rca_pir.Join(RCASessionRepository.db.users,
                u => u.username,
                t => t.username,
                (u, t) => new RCATeamModel { username = u.username, fullname = t.fullname, jabatan = t.jabatan, role = u.role, employee_id = t.employee_id }).Join(RCASessionRepository.db.employees,
                u => u.employee_id,
                t => t.id,
                (u, t) => new RCATeamEmployeeModel { username = u.username, alpha_name = t.alpha_name, position = t.position, role = u.role, employee_id = u.employee_id }).ToList();
            ViewData["user_role"] = li;
            ViewData["peoples"] = rt.ToList();
            return View();
        }

        //
        //Select ajax binding (listing)

        [GridAction]
        public ActionResult _SelectAjaxTeam()
        {
            return bindingTeam();
        }

        //select all Team
        private ViewResult bindingTeam()
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            string username = "";
            if (rca != null)
            {
                if (rca.id_team != null)
                    username = RCASessionRepository.db.rca_team_connector.Where(p => p.id == rca.id_team).FirstOrDefault().id_user;
                //else
                //{
                //    //username = HttpContext.Session["username"].ToString();
                //}
            }
            else
            {
                //username = HttpContext.Session["username"].ToString();
            }
            List<RCATeamEmployeeModel> result;
            if (rca.is_publish == 0)
            {
                result = RCASessionRepository.db.user_per_role.Join(RCASessionRepository.db.users,
                u => u.username,
                t => t.username,
                (u, t) => new RCATeamModel { username = u.username, fullname = t.fullname, jabatan = t.jabatan, employee_id = t.employee_id }).Distinct().Join(RCASessionRepository.db.employees,
                u => u.employee_id,
                t => t.id,
                (u, t) => new RCATeamEmployeeModel { username = u.username, alpha_name = t.alpha_name, position = t.position, employee_id = u.employee_id }).ToList();
            }
            else
            {
                result = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == ids).Join(
                    RCASessionRepository.db.user_per_role,
                    f => f.id_user,
                    d => d.username,
                    (f, d) => d).Join(
                        RCASessionRepository.db.users,
                        u => u.username,
                        r => r.username,
                        (u, r) => new RCATeamModel { username = u.username, fullname = r.fullname, jabatan = r.jabatan, employee_id = r.employee_id }).Distinct().Join(
                        RCASessionRepository.db.employees,
                        u => u.employee_id,
                        r => r.id,
                        (u, r) => new RCATeamEmployeeModel { username = u.username, alpha_name = r.alpha_name, position = r.position, employee_id = u.employee_id }).ToList();
            }
            result.RemoveAll(p => p.username == username);
            return View(new GridModel<RCATeamEmployeeModel>
            {
                Data = result
            });
        }

        [HttpPost]
        [GridAction]
        public ActionResult rend(string analyst)
        {
            List<RCATeamEmployeeModel> result = RCASessionRepository.db.user_per_role.Where(p => p.username != analyst).Join(RCASessionRepository.db.users,
                u => u.username,
                t => t.username,
                (u, t) => new RCATeamModel { username = u.username, fullname = t.fullname, jabatan = t.jabatan, role = u.role, employee_id = t.employee_id }).Join(RCASessionRepository.db.employees,
                u => u.employee_id,
                t => t.id,
                (u, t) => new RCATeamEmployeeModel { username = u.username, alpha_name = t.alpha_name, position = t.position, role = u.role, employee_id = u.employee_id }).ToList();

            return View(new GridModel<RCATeamEmployeeModel>
            {
                Data = result
            });
        }

        //
        // POST: /RCA/addRCA6

        [HttpPost]
        public ActionResult addRCA6(string next, string cancel, string previous, string[] checkedRecords, string types, string analyst, string isPublish, string[] select)
        {
            var button = next ?? cancel ?? previous;

            if (button == "Next")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                int i = 0;
                if (isPublish == "0")
                {    
                    TeamComparer tc = new TeamComparer();
                    List<rca_team_connector> lc = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == rca.id).ToList();
                    foreach (rca_team_connector r in lc)
                    {
                        RCASessionRepository.db.rca_team_connector.Remove(r);
                        RCASessionRepository.db.SaveChanges();
                    }
                    rca_team_connector csfc = new rca_team_connector()
                    {
                        id_user = analyst,
                        id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString()),
                        rca_position = null
                    };
                    RCASessionRepository.db.rca_team_connector.Add(csfc);
                    RCASessionRepository.db.SaveChanges();
                    int idd = csfc.id;

                    rca.id_team = idd;
                    RCASessionRepository.UpdateRCA6(rca);
                    if (checkedRecords != null)
                    {

                        foreach (string name in checkedRecords)
                        {
                            Debug.WriteLine("name = " + name);
                            csfc = new rca_team_connector()
                            {
                                id_user = name,
                                id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString()),
                                rca_position = Byte.Parse(select[i])
                            };

                            RCASessionRepository.db.rca_team_connector.Add(csfc);
                            RCASessionRepository.db.SaveChanges();
                            i++;
                        }
                        int id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        
                    }
                }
                if (types == "add")
                {
                    //List<string> s = new List<string>();
                    //List<string> t = new List<string>();
                    //var sendEmail = new SendEmailController();
                    //employee e = null;
                    //rca = RCASessionRepository.OneView(p => p.id == Int32.Parse(HttpContext.Session["id_analysis"].ToString()));
                    //s.Clear();
                    //i = 0;
                    //if (checkedRecords != null)
                    //{
                    //    foreach (string name in checkedRecords)
                    //    {
                    //        e = RCASessionRepository.db.employees.Find(RCASessionRepository.db.users.Find(name).employee_id);
                    //        if (Byte.Parse(select[i]) == 1)
                    //        {
                    //            if (e.email != null) s.Add(e.email);
                    //        }
                    //        else if (Byte.Parse(select[i]) == 2)
                    //        {
                    //            if (e.email != null) t.Add(e.email);
                    //        }
                            
                    //    }
                    //}
                    //if (s.Count > 0) sendEmail.Send(s, "Bapak/Ibu,<br />Anda terpilih dan mendapat tugas sebagai Leader Team Investigator untuk Root Cause Analysis dengan nomor referensi " + rca.rca_code + ".<br />"
                    //        + "Mohon anda untuk menunjuk dan memilih anggota timnya.Terima kasih."
                    //        + "<br/><br/><i>Dear Sir/Madam,<br/>We inform you that you are assigned as the Team Investigator Leader for Root Cause Analysis with reference number " + rca.rca_code + ".<br />"
                    //        + "Please nominate and assign your team member.Thank you.</i>"
                    //        + "<br/><br/>Salam,<br/><i>Regard,</i><br/>" + RCASessionRepository.db.employees.Find(Int32.Parse(Session["id"].ToString())).alpha_name, "Root Cause Analysis \"" + rca.rca_code + "\" Team Leader");

                    //if (t.Count > 0) sendEmail.Send(t, "Bapak/Ibu,<br />Anda terpilih dan mendapat tugas sebagai Anggota Team Investigator untuk Root Cause Analysis dengan nomor referensi " + rca.rca_code + "."
                    //        + "Terima kasih."
                    //        + "<br/><br/><i>Dear Sir/Madam,<br/>We inform you that you are assigned as the Team Investigator Member for Root Cause Analysis with reference number " + rca.rca_code + "."
                    //        + "Thank you.</i>"
                    //        + "<br/><br/>Salam,<br/><i>Regard,</i><br/>" + RCASessionRepository.db.employees.Find(Int32.Parse(Session["id"].ToString())).alpha_name, "Root Cause Analysis \"" + rca.rca_code + "\" Team Member");
                    return RedirectToAction("addRCA7");
                }
                else
                    return RedirectToAction("addRCA7", new { id = rca.id });
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                int i = 0;
                if (isPublish == "0")
                {    
                    TeamComparer tc = new TeamComparer();
                    List<rca_team_connector> lc = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == rca.id).ToList();
                    foreach (rca_team_connector r in lc)
                    {
                        RCASessionRepository.db.rca_team_connector.Remove(r);
                        RCASessionRepository.db.SaveChanges();
                    }
                    rca_team_connector csfc = new rca_team_connector()
                    {
                        id_user = analyst,
                        id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString()),
                        rca_position = null
                    };
                    RCASessionRepository.db.rca_team_connector.Add(csfc);
                    RCASessionRepository.db.SaveChanges();
                    int idd = csfc.id;

                    rca.id_team = idd;
                    RCASessionRepository.UpdateRCA6(rca);
                    if (checkedRecords != null)
                    {
                        foreach (string name in checkedRecords)
                        {
                            Debug.WriteLine("name = " + name);
                            csfc = new rca_team_connector()
                            {
                                id_user = name,
                                id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString()),
                                rca_position = Byte.Parse(select[i])
                            };
                            RCASessionRepository.db.rca_team_connector.Add(csfc);
                            RCASessionRepository.db.SaveChanges();
                            i++;
                        }

                        int id_rca = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                    }
                }
                if (types == "add")
                {
                    return RedirectToAction("addRCA5");
                }
                else
                    return RedirectToAction("addRCA5", new { id = rca.id });
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }

        }

        #endregion

        //==============================================================

        #region addRCA7

        //
        // GET: /RCA/addRCA7

        public ActionResult addRCA7(int? id)
        {
            int ids = 0;
            if (HttpContext.Session["id_analysis"] != null)
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

            if (id != null && id != 0)
            {
                HttpContext.Session["id_analysis"] = id.ToString();
                ids = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                ViewBag.types = "edit";
            }
            else
            {
                ViewBag.types = "add";
            }

            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == ids);
            if (rca != null)
            {
                ViewBag.start_date = rca.start_date;
                ViewBag.completion_date = rca.completion_date;
                ViewBag.isPublish = rca.is_publish;
            }
            ViewData["user_role"] = li;
            return View(rca);
        }

        //
        // POST: /RCA/addRCA7

        [HttpPost]
        public ActionResult addRCA7(string done, string cancel, string previous, string start_date, string completion_date, string types, string isPublish)
        {
            var button = done ?? cancel ?? previous;
            if (button == "Done")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                DateTime? startDate = null;
                DateTime? finishDate = null;
                if (start_date != null && start_date != "")
                    startDate = DateTime.Parse(start_date);
                if (completion_date != null && completion_date != "")
                    finishDate = DateTime.Parse(completion_date);
                Debug.WriteLine("time = " + startDate);
                Debug.WriteLine("time2 = " + finishDate);
                rca.start_date = startDate;
                rca.completion_date = finishDate;
                //RCAEntityModel rcas = new RCAEntityModel();
                //if (ModelState.IsValid)
                if (isPublish == "0")
                {
                    if (start_date != null && start_date != "")
                    {
                        if (types == "add")
                        {
                            RCAEntityModel rcass = RCASessionRepository.AllView().OrderBy(p => p.rca_code).LastOrDefault();
                            if (rcass != null)
                            {
                                if (rcass.rca_code != null && rcass.rca_code.Length == 21)
                                {
                                    int prev_code = Int32.Parse(rcass.rca_code.Substring(17));
                                    int prev_year = Int32.Parse(rcass.rca_code.Substring(12, 4));
                                    prev_code++;
                                    DateTime now = DateTime.Now;
                                    if (prev_year != now.Year)
                                    {
                                        prev_code = 1;
                                    }
                                    rca.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                                }
                                else
                                {
                                    int prev_code = 1;
                                    DateTime now = DateTime.Now;
                                    rca.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                                }
                            }
                            else
                            {
                                int prev_code = 1;
                                DateTime now = DateTime.Now;
                                rca.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                            }
                        }
                        else
                        {
                            rca.rca_code = RCASessionRepository.OneView(p => p.id == rca.id).rca_code;
                        }
                        RCASessionRepository.UpdateRCA7(rca);
                        rca o_rca = RCASessionRepository.db.rcas.Find(rca.id);

                        if (o_rca.fracas_ir_id != null && o_rca.fracas_ir == 2)
                        {
                            incident_report ir = RCASessionRepository.db.incident_report.Find(o_rca.fracas_ir_id);
                            ir.id_rca = rca.id;
                            RCASessionRepository.db.Entry(ir).State = EntityState.Modified;
                            RCASessionRepository.db.SaveChanges();
                        }

                        List<string> s = new List<string>();
                        List<string> t = new List<string>();
                        var sendEmail = new SendEmailController();
                        employee e = null;
                        s.Clear();
                        List<rca_team_connector> list_team = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == rca.id).ToList();
                        if (list_team != null)
                        {
                            foreach (rca_team_connector rtc in list_team)
                            {
                                e = RCASessionRepository.db.employees.Find(RCASessionRepository.db.users.Find(rtc.id_user).employee_id);
                                if (rtc.rca_position == 1)
                                {
                                    if (e.email != null) s.Add(e.email);
                                }
                                else if (rtc.rca_position == 2)
                                {
                                    if (e.email != null) t.Add(e.email);
                                }

                            }
                        }
                        if (s.Count > 0)
                        {
                            sendEmail.Send(s, "Bapak/Ibu,<br />Anda terpilih dan mendapat tugas sebagai Leader Team Investigator untuk Root Cause Analysis dengan nomor referensi " + rca.rca_code + ".<br />"
                                + "Mohon anda untuk menunjuk dan memilih anggota timnya.Terima kasih."
                                + "<br/><br/><i>Dear Sir/Madam,<br/>We inform you that you are assigned as the Team Investigator Leader for Root Cause Analysis with reference number " + rca.rca_code + ".<br />"
                                + "Please nominate and assign your team member.Thank you.</i>"
                                + "<br/><br/>Salam,<br/><i>Regard,</i><br/>" + RCASessionRepository.db.employees.Find(Int32.Parse(Session["id"].ToString())).alpha_name, "Root Cause Analysis \"" + rca.rca_code + "\" Team Leader");
                        }

                        if (t.Count > 0)
                        {
                            sendEmail.Send(t, "Bapak/Ibu,<br />Anda terpilih dan mendapat tugas sebagai Anggota Team Investigator untuk Root Cause Analysis dengan nomor referensi " + rca.rca_code + "."
                                + "Terima kasih."
                                + "<br/><br/><i>Dear Sir/Madam,<br/>We inform you that you are assigned as the Team Investigator Member for Root Cause Analysis with reference number " + rca.rca_code + "."
                                + "Thank you.</i>"
                                + "<br/><br/>Salam,<br/><i>Regard,</i><br/>" + RCASessionRepository.db.employees.Find(Int32.Parse(Session["id"].ToString())).alpha_name, "Root Cause Analysis \"" + rca.rca_code + "\" Team Member");
                        }

                        HttpContext.Session.Remove("id_analysis");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewData["user_role"] = li;
                        ViewBag.types = types;
                        ModelState.AddModelError("start_date", "Start date required");
                        return View(rca);
                    }
                }
                else
                {
                    HttpContext.Session.Remove("id_analysis");
                    return RedirectToAction("Index");
                }
            }
            else if (button == "Previous")
            {
                RCAEntityModel rca = new RCAEntityModel();
                rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());
                DateTime? startDate = null;
                DateTime? finishDate = null;
                if (start_date != null && start_date != "")
                    startDate = DateTime.Parse(start_date);
                if (completion_date != null && completion_date != "")
                    finishDate = DateTime.Parse(completion_date);
                Debug.WriteLine("time = " + startDate);
                Debug.WriteLine("time2 = " + finishDate);
                rca.start_date = startDate;
                rca.completion_date = finishDate;
                if (isPublish == "0")
                {
                    if (start_date != null && start_date != "")
                    {
                        RCASessionRepository.UpdateRCA7(rca);
                        rca o_rca = RCASessionRepository.db.rcas.Find(rca.id);
                        if (o_rca.fracas_ir_id != null && o_rca.fracas_ir == 2)
                        {
                            incident_report ir = RCASessionRepository.db.incident_report.Find(o_rca.fracas_ir_id);
                            ir.id_rca = rca.id;
                            RCASessionRepository.db.Entry(ir).State = EntityState.Modified;
                            RCASessionRepository.db.SaveChanges();
                        }
                        if (types == "add")
                            return RedirectToAction("addRCA6");
                        else
                            return RedirectToAction("addRCA6", new { id = rca.id });
                    }
                    else
                    {
                        ViewData["user_role"] = li;
                        ViewBag.types = types;
                        ModelState.AddModelError("start_date", "Start date required");
                        return View(rca);
                    }
                }
                else
                {
                    if (types == "add")
                        return RedirectToAction("addRCA6");
                    else
                        return RedirectToAction("addRCA6", new { id = rca.id });
                }
            }
            else
            {
                if (types == "add")
                {
                    if (HttpContext.Session["id_analysis"] != null)
                    {
                        RCAEntityModel rca = new RCAEntityModel();
                        rca.id = Int32.Parse(HttpContext.Session["id_analysis"].ToString());

                        RCASessionRepository.Delete(rca);
                        HttpContext.Session.Remove("id_analysis");
                    }
                }

                return RedirectToAction("Index");
            }

        }

        #endregion

        #endregion

        //==============================================================

        #region action

        //
        // GET: /RCA/open/5
        //
        public ActionResult open(int id, int? temp)
        {
            var model = from o in RCASessionRepository.db.rcas
                        where o.id == id
                        select new RCAEntityModel
                        {
                            id = o.id,
                            name = o.name,
                            description = o.description,
                            type_name = o.rca_analisys_type.name
                        };
            List<RCAEntityModel> result = model.ToList();

            RCAEntityModel rcass = RCASessionRepository.OneView(p => p.id == id);
            ViewBag.saveFile = rcass.analysis_file;
            ViewBag.isPublish = rcass.is_publish;
            if (HttpContext.Session["isTree"].ToString() != null)
                ViewBag.isTree = Byte.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1"});
            if (rcass.fracas_ir_id != null && rcass.fracas_ir == 1)
                ViewBag.events = RCASessionRepository.db.equipment_event.Find(rcass.fracas_ir_id).event_description == null ? "" : RCASessionRepository.db.equipment_event.Find(rcass.fracas_ir_id).event_description;
            else if (rcass.fracas_ir_id != null && rcass.fracas_ir == 2)
                ViewBag.events = RCASessionRepository.db.incident_report.Find(rcass.fracas_ir_id).title == null ? "" : RCASessionRepository.db.incident_report.Find(rcass.fracas_ir_id).title;
            else
                ViewBag.events = "";
            ViewData["user_role"] = li;
            if (temp != null)
            {
                ViewBag.template = RCASessionRepository.db.rca_template.Find(temp);
            }
            ViewBag.list_template = RCASessionRepository.db.rca_template.Where(p => p.type == 1).ToList();
            if (ViewBag.isTree.ToString() == "0")
            {
                if (rcass.analysis_file != null || temp != null)
                {
                    ViewBag.load = 1;
                    String filepath = "";
                    if (rcass.analysis_file != null)
                    {
                        filepath = Server.MapPath("~/Content/xml/" + rcass.analysis_file);
                    }
                    else
                    {
                        filepath = Server.MapPath("~/Content/xml/rca_template/" + (ViewBag.template as rca_template).name + ".xml");
                    }
                    StreamReader sw = new StreamReader(filepath);
                    List<String> why = new List<String>();
                    using (XmlReader reader = XmlReader.Create(sw))
                    {
                        while (reader.Read())
                        {
                            // Only detect start elements.
                            if (reader.IsStartElement())
                            {
                                // Get element name and switch on it.
                                switch (reader.Name)
                                {
                                    case "RCA":
                                        // Detect this element.
                                        Debug.WriteLine("Start <RCA> element.");
                                        break;
                                    case "Event":
                                        // Detect this article element.
                                        Debug.WriteLine("Start <Event> element.");
                                        // Search for the attribute name on this current node.

                                        // Next read will contain text.
                                        if (reader.Read())
                                        {
                                            Debug.WriteLine("  Text node: " + reader.Value.Trim());
                                            ViewBag.events = reader.Value.Trim();
                                        }
                                        break;
                                    case "Why":
                                        // Detect this article element.
                                        Debug.WriteLine("Start <Why> element.");
                                        // Search for the attribute name on this current node.

                                        // Next read will contain text.
                                        if (reader.Read())
                                        {
                                            if (reader.IsStartElement())
                                            {
                                                // Get element name and switch on it.
                                                switch (reader.Name)
                                                {
                                                    case "value":
                                                        Debug.WriteLine("Start <value> element.");
                                                        if (reader.Read())
                                                        {
                                                            Debug.WriteLine("  Text node: " + reader.Value.Trim());
                                                            why.Add(reader.Value.Trim());
                                                        }
                                                        break;
                                                }

                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    sw.Close();
                    ViewBag.why = why;
                    ViewBag.list_template = RCASessionRepository.db.rca_template.Where(p => p.type == 0).ToList();
                }
                else
                {
                    ViewBag.load = 0;
                    ViewBag.list_template = RCASessionRepository.db.rca_template.Where(p => p.type == 0).ToList();
                }
            }
            
            Debug.WriteLine(rcass.fracas_ir);
            ViewBag.fracasir = rcass.fracas_ir;
            ViewBag.fracasirid = rcass.fracas_ir_id;
            return View(result.ElementAt(0));
        }

        //
        // GET: /RCA/test/5
        //
        public ActionResult test(int id, int? temp)
        {
            var model = from o in RCASessionRepository.db.rcas
                        where o.id == id
                        select new RCAEntityModel
                        {
                            id = o.id,
                            name = o.name,
                            description = o.description,
                            type_name = o.rca_analisys_type.name
                        };
            List<RCAEntityModel> result = model.ToList();

            RCAEntityModel rcass = RCASessionRepository.OneView(p => p.id == id);
            ViewBag.saveFile = rcass.analysis_file != null ? rcass.analysis_file : rcass.equipment_type != null ? System.IO.File.Exists(Server.MapPath("~/Content/xml/fishbone_template/template_" + rcass.equipment_type + ".xml")) == true ? "template_" + rcass.equipment_type + ".xml" : "" : "";
            ViewBag.isPublish = rcass.is_publish;
            if (HttpContext.Session["isTree"].ToString() != null)
                ViewBag.isTree = Byte.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1"});
            ViewBag.equipmentType = rcass.equipment_type;
            if (rcass.fracas_ir_id != null && rcass.fracas_ir == 1)
                ViewBag.events = RCASessionRepository.db.equipment_event.Find(rcass.fracas_ir_id).event_description == null ? "" : RCASessionRepository.db.equipment_event.Find(rcass.fracas_ir_id).event_description;
            else if (rcass.fracas_ir_id != null && rcass.fracas_ir == 2)
                ViewBag.events = RCASessionRepository.db.incident_report.Find(rcass.fracas_ir_id).title == null ? "" : RCASessionRepository.db.incident_report.Find(rcass.fracas_ir_id).title;
            else
                ViewBag.events = "";
            ViewData["user_role"] = li;
            ViewBag.master = RCASessionRepository.db.rca_fishbone_master.ToList();
            ViewBag.fracasir = rcass.fracas_ir;
            ViewBag.fracasirid = rcass.fracas_ir_id;
            if (temp != null)
            {
                ViewBag.template = RCASessionRepository.db.rca_template.Find(temp);
            }
            ViewBag.list_template = RCASessionRepository.db.rca_template.Where(p => p.type == 2).ToList();
            return View(result.ElementAt(0));
        }

        //
        // GET: /RCA/copy/5
        //
        [GridAction]
        public ActionResult copy(int id)
        {
            RCAEntityModel rcas = RCASessionRepository.OneView(p => p.id == id);

            RCAEntityModel rcass = RCASessionRepository.AllView().OrderBy(p => p.rca_code).LastOrDefault();
            if (rcass != null)
            {
                if (rcass.rca_code != null && rcass.rca_code.Length == 21)
                {
                    int prev_code = Int32.Parse(rcass.rca_code.Substring(17));
                    int prev_year = Int32.Parse(rcass.rca_code.Substring(12, 4));
                    prev_code++;
                    DateTime now = DateTime.Now;
                    if (prev_year != now.Year)
                    {
                        prev_code = 1;
                    }
                    rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                }
                else
                {
                    int prev_code = 1;
                    DateTime now = DateTime.Now;
                    rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
                }
            }
            else
            {
                int prev_code = 1;
                DateTime now = DateTime.Now;
                rcas.rca_code = "W-O-EAI-RCA-" + now.Year + "-" + prev_code.ToString().PadLeft(4, '0');
            }

            int newId = RCASessionRepository.Insert(rcas);

            rcas.id = newId;
            List<rca_csf_conector> lcc = RCASessionRepository.db.rca_csf_conector.Where(p => p.id_rca == id).ToList();

            foreach (rca_csf_conector cc in lcc)
            {
                cc.id_rca = newId;
                RCASessionRepository.db.rca_csf_conector.Add(cc);
                RCASessionRepository.db.SaveChanges();
            }

            List<rca_team_connector> ltc = RCASessionRepository.db.rca_team_connector.Where(p => p.id_rca == id).ToList();

            foreach (rca_team_connector tc in ltc)
            {
                tc.id_rca = newId;
                RCASessionRepository.db.rca_team_connector.Add(tc);
                RCASessionRepository.db.SaveChanges();
            }

            if (rcas.analysis_file != null)
            {
                String filepath = Server.MapPath("~/Content/xml/" + rcas.analysis_file);
                StreamReader sr = new StreamReader(filepath);
                string xml = sr.ReadToEnd();
                sr.Close();
                string filename = "analysis" + newId + ".xml";
                string filepath2 = Server.MapPath("~/Content/xml/" + filename);
                StreamWriter sw = new StreamWriter(filepath2);
                sw.WriteLine(xml);
                sw.Close();
                Debug.WriteLine("save id = " + id);
                rcas.analysis_file = filename;
                RCASessionRepository.UpdateRCATree(rcas);
            }

            int isTree = 0;
            if (HttpContext.Session["isTree"].ToString() != null)
                isTree = Int32.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1" });

            return View(new GridModel(RCASessionRepository.AllView().Where(p => p.is_tree == isTree)));
        }

        //
        // GET: /RCA/publish/5
        //
        [GridAction]
        public ActionResult publish(int id)
        {
            RCAEntityModel rcas = RCASessionRepository.OneView(p => p.id == id);
            RCASessionRepository.UpdatePublish(rcas);

            int isTree = 0;
            if (HttpContext.Session["isTree"].ToString() != null)
                isTree = Int32.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1" });

            return View(new GridModel(RCASessionRepository.AllView().Where(p => p.is_tree == isTree)));
        }


        //
        //Select ajax binding (listing)
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //select all analisis
        private ViewResult binding()
        {
            List<RCAEntityModel> result = RCASessionRepository.AllView().Reverse().Where(p => p.is_tree == (int)(HttpContext.Session["isTree"])).ToList();
            return View(new GridModel<RCAEntityModel>
            {
                Data = result
            });
        }

        //
        //Delete ajax binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            return deleteBinding(id);
        }

        //Delete analisis dengan Id = id
        private ActionResult deleteBinding(int id)
        {
            RCAEntityModel analysis = RCASessionRepository.OneView(p => p.id == id);
            if (analysis != null)
            {
                Debug.WriteLine("id delete = " + analysis.id);
                RCASessionRepository.Delete(analysis);
            }

            int isTree = 0;
            if (HttpContext.Session["isTree"].ToString() != null)
                isTree = Int32.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1" });

            return View(new GridModel(RCASessionRepository.AllView().Where(p => p.is_tree == isTree).Reverse()));
        }

        public class JsonModelBinder : IModelBinder
        {
            private readonly static JavaScriptSerializer _serializer = new JavaScriptSerializer();
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var stringified = controllerContext.HttpContext.Request[bindingContext.ModelName];
                if (string.IsNullOrEmpty(stringified))
                    return null;
                return _serializer.Deserialize(stringified, bindingContext.ModelType);
            }
        }

        public class FromJsonAttribute : CustomModelBinderAttribute
        {
            public override IModelBinder GetBinder()
            {
                return new JsonModelBinder();
            }
        }

        //
        //Save ajax binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {
            RCAEntityModel product = RCASessionRepository.OneView(p => p.id == id);

            TryUpdateModel(product);
            //RCASessionRepository.Update(product,false);
            return View(new GridModel(RCASessionRepository.AllView()));
        }

        #endregion

        //==============================================================

        #region open.html

        // save in open.cshtml
        //
        // Post: RCA/save
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult save(string filename, string xml, string id, int fracasir, int? sel_val)
        {
            String filepath = Server.MapPath("~/Content/xml/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            sw.WriteLine(xml);
            sw.Close();
            Debug.WriteLine("save id = " + id);
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == Int32.Parse(id));
            rca.analysis_file = filename;
            rca.fracas_ir = (Byte)fracasir;
            rca.fracas_ir_id = sel_val;
            RCASessionRepository.UpdateRCATree(rca);
            return Json(true);
        }

        //
        // Post: RCA/saveWhy
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult saveWhy(string filename, string events, string[] item, string id, int fracasir, string fracasirid)
        {
            String filepath = Server.MapPath("~/Content/xml/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("RCA");
                writer.WriteElementString("Event", events);
                Debug.WriteLine(item.Length);
                foreach (string it in item)
                {
                    writer.WriteStartElement("Why");

                    writer.WriteElementString("value", it);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            sw.Close();
            Debug.WriteLine("save id = " + filename);
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == Int32.Parse(id));
            rca.analysis_file = filename;
            rca.fracas_ir = (Byte)fracasir;
            if (fracasirid != "")
            {
                rca.fracas_ir_id = Int32.Parse(fracasirid);
            }
            else
            {
                rca.fracas_ir_id = null;
            }
            RCASessionRepository.UpdateRCATree(rca);
            return Json(true);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public void SaveFullImage(string filename, string format, string bg, string w, string h, string xml)
        {
            xml = HttpUtility.UrlDecode(xml);
            string width = w;
            string height = h;
            xml = xml.Replace(HttpUtility.HtmlEncode("<p style=\"margin:0px;width:115px\">&nbsp;</p><h4 style=\"margin:0px;color:#1d258f;text-align:center;\">"), "\n    ");
            xml = xml.Replace(HttpUtility.HtmlEncode("&nbsp;&nbsp;&nbsp;&nbsp;</h4><p style=\"text-align:left;margin:0px;color:black;text-indent:1px;float:left;width:20px\">"), "\n");
            xml = xml.Replace(HttpUtility.HtmlEncode("</p><p style=\"text-align:right;margin:0px;color:black;\">"), "                                  ");
            xml = xml.Replace(HttpUtility.HtmlEncode("</p>"), "");
            if (xml != null && width != null && height != null && bg != null
                    && filename != null && format != null)
            {
                System.Drawing.Image image = mxUtils.CreateImage(int.Parse(width), int.Parse(height),
                    ColorTranslator.FromHtml(bg));
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.HighQuality;
                mxSaxOutputHandler handler = new mxSaxOutputHandler(new mxGdiCanvas2D(g));
                handler.Read(new XmlTextReader(new StringReader(xml)));

                MemoryStream memStream = new MemoryStream();
                image.Save(memStream, ImageFormat.Png);
                String filepath = Server.MapPath("~/Content/full_image/" + filename);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                memStream.WriteTo(fs);
                fs.Close();
            }
            else
            {
            }
        }


        // print image in open as tree diagram
        //
        // POST: /RCA/ProcessRequest

        [AcceptVerbs(HttpVerbs.Post)]
        public void ProcessRequest(string filename, string format, string bg, string w, string h, string xml)
        {
            xml = HttpUtility.UrlDecode(xml);
            string width = w;
            string height = h;
            xml = xml.Replace(HttpUtility.HtmlEncode("<p style=\"margin:0px;width:115px\">&nbsp;</p><h4 style=\"margin:0px;color:#1d258f;text-align:center;\">"), "\n    ");
            xml = xml.Replace(HttpUtility.HtmlEncode("&nbsp;&nbsp;&nbsp;&nbsp;</h4><p style=\"text-align:left;margin:0px;color:black;text-indent:1px;float:left;width:20px\">"), "\n");
            xml = xml.Replace(HttpUtility.HtmlEncode("</p><p style=\"text-align:right;margin:0px;color:black;\">"), "                                  ");
            xml = xml.Replace(HttpUtility.HtmlEncode("</p>"), "");
            if (xml != null && width != null && height != null && bg != null
                    && filename != null && format != null)
            {
                Debug.WriteLine(xml);
                System.Drawing.Image image = mxUtils.CreateImage(int.Parse(width), int.Parse(height),
                    ColorTranslator.FromHtml(bg));
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.HighQuality;
                mxSaxOutputHandler handler = new mxSaxOutputHandler(new mxGdiCanvas2D(g));
                handler.Read(new XmlTextReader(new StringReader(xml)));
                HttpContext.Response.ContentType = "image/" + format;
                HttpContext.Response.AddHeader("Content-Disposition",
                  "attachment; filename=" + filename);

                MemoryStream memStream = new MemoryStream();
                image.Save(memStream, ImageFormat.Png);
                memStream.WriteTo(HttpContext.Response.OutputStream);

                HttpContext.Response.StatusCode = 200; /* OK */
            }
            else
            {
                HttpContext.Response.StatusCode = 400; /* Bad Request */
            }
        }

        [HttpPost]
        public JsonResult getAllIr()
        {
            int years = DateTime.Today.Year - 2;
            List<incident_report> ir = RCASessionRepository.db.incident_report.Where(p => p.date_incident.Value.Year >= years).OrderByDescending(p => p.reference_number).ToList();
            return Json(new { pir = ir });
        }

        public JsonResult getAllFracas()
        {
            var ee = (from equipment in RCASessionRepository.db.equipment_event
                      where equipment.datetime_stop != null 
                      select new { id = equipment.id, event_description = equipment.event_description }).ToList();
            return Json(new { pir = ee });
        }

        public JsonResult getDescription(int id, int fracasir)
        {
            string desc = "";
            if (fracasir == 1)
            {
                desc = RCASessionRepository.db.equipment_event.Find(id).event_description;
            }
            else if (fracasir == 2)
            {
                desc = RCASessionRepository.db.incident_report.Find(id).title;
            }
            return Json(new { val = desc });
        }

        #endregion

        //==============================================================

        #region open2 (fishbone)

        // save in open2.cshtml
        //
        // Post: RCA/save2
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult save2(string filename, string xml, string id, int fracasir, int? sel_val)
        {
            String filepath = Server.MapPath("~/Content/xml/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            sw.WriteLine(xml);
            sw.Close();
            Debug.WriteLine(filename);
            RCAEntityModel rca = RCASessionRepository.OneView(p => p.id == Int32.Parse(id));
            rca.analysis_file = filename;
            rca.fracas_ir = (Byte)fracasir;
            rca.fracas_ir_id = sel_val;
            RCASessionRepository.UpdateRCATree(rca);
            return Json(true);
        }

        // save as template in open2.cshtml
        //
        // Post: RCA/saveAs
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult saveAs(string filename, string xml)
        {
            String filepath = Server.MapPath("~/Content/xml/fishbone_template/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            sw.WriteLine(xml);
            sw.Close();
            return Json(true);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ProcessRequest2(string filename, string format, string bg, string w, string h, string xml)
        {
            xml = HttpUtility.UrlDecode(xml);
            string width = w;
            string height = h;
            xml = xml.Replace(HttpUtility.HtmlEncode("<h4 style=\"margin:0px;color:#1d258f;text-align:center;\">&nbsp;&nbsp;&nbsp;&nbsp;"), "    ");
            xml = xml.Replace(HttpUtility.HtmlEncode("&nbsp;&nbsp;&nbsp;&nbsp;</h4>"), "");
            if (xml != null && width != null && height != null && bg != null
                    && filename != null && format != null)
            {
                Debug.WriteLine(xml);
                System.Drawing.Image image = mxUtils.CreateImage(int.Parse(width), int.Parse(height),
                    ColorTranslator.FromHtml(bg));
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.HighQuality;
                mxSaxOutputHandler handler = new mxSaxOutputHandler(new mxGdiCanvas2D(g));
                handler.Read(new XmlTextReader(new StringReader(xml)));
                HttpContext.Response.ContentType = "image/" + format;
                HttpContext.Response.AddHeader("Content-Disposition",
                  "attachment; filename=" + filename);

                MemoryStream memStream = new MemoryStream();
                image.Save(memStream, ImageFormat.Png);
                memStream.WriteTo(HttpContext.Response.OutputStream);

                HttpContext.Response.StatusCode = 200; /* OK */
            }
            else
            {
                HttpContext.Response.StatusCode = 400; /* Bad Request */
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public void SaveFullImage2(string filename, string format, string bg, string w, string h, string xml)
        {
            xml = HttpUtility.UrlDecode(xml);
            string width = w;
            string height = h;
            xml = xml.Replace(HttpUtility.HtmlEncode("<h4 style=\"margin:0px;color:#1d258f;text-align:center;\">&nbsp;&nbsp;&nbsp;&nbsp;"), "    ");
            xml = xml.Replace(HttpUtility.HtmlEncode("&nbsp;&nbsp;&nbsp;&nbsp;</h4>"), "");
            if (xml != null && width != null && height != null && bg != null
                    && filename != null && format != null)
            {
                System.Drawing.Image image = mxUtils.CreateImage(int.Parse(width), int.Parse(height),
                    ColorTranslator.FromHtml(bg));
                Graphics g = Graphics.FromImage(image);
                g.SmoothingMode = SmoothingMode.HighQuality;
                mxSaxOutputHandler handler = new mxSaxOutputHandler(new mxGdiCanvas2D(g));
                handler.Read(new XmlTextReader(new StringReader(xml)));

                MemoryStream memStream = new MemoryStream();
                image.Save(memStream, ImageFormat.Png);
                String filepath = Server.MapPath("~/Content/full_image/" + filename);
                FileStream fs = new FileStream(filepath, FileMode.Create);
                memStream.WriteTo(fs);
                fs.Close();
            }
            else
            {
            }
        }

        #endregion

        //==============================================================

        #region verification.html

        //
        // GET: /RCA/verification/5
        //
        [HttpPost]
        public ActionResult verification(string id, [FromJson]string[][] list)
        {
            int ids = Int32.Parse(id);
            RCAEntityModel rcas = RCASessionRepository.OneView(p => p.id == ids);
            List<rca_desc> lc = RCASessionRepository.db.rca_desc.Where(p => p.id_rca == ids).ToList();
            ViewBag.id = id;
            ViewBag.list = list;
            ViewBag.descs = lc;
            ViewBag.equipmentCode = rcas.equipment_code;
            ViewBag.saveFile = rcas.analysis_file;
            if (HttpContext.Session["isTree"].ToString() != null)
                ViewBag.isTree = Byte.Parse(HttpContext.Session["isTree"].ToString());
            else
                return RedirectToAction("LogOn", "Account", new { returnUrl = "/rca?isTree=1" });
            ViewData["user_role"] = li;
            ViewBag.isPublish = rcas.is_publish;
            ViewBag.events = "";
            return View();
        }

        //==============================================================

        //
        // Post: RCA/saveDesc
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult saveDesc(string id, string[] desc)
        {
            if (id != null)
            {
                int ids = Int32.Parse(id);
                List<rca_desc> lc = RCASessionRepository.db.rca_desc.Where(p => p.id_rca == ids).ToList();
                foreach (rca_desc r in lc)
                {
                    RCASessionRepository.db.rca_desc.Remove(r);
                    RCASessionRepository.db.SaveChanges();
                }

                foreach (string s in desc)
                {
                    rca_desc r = new rca_desc
                    {
                        id_rca = ids,
                        description = s
                    };
                    RCASessionRepository.db.rca_desc.Add(r);
                    RCASessionRepository.db.SaveChanges();
                }
            }
            return Json(true);
        }

        //
        // Post: RCA/delDesc
        //
        [HttpPost]
        public JsonResult delDesc(string id)
        {
            if (id != null)
            {
                int ids = Int32.Parse(id);
                List<rca_desc> lc = RCASessionRepository.db.rca_desc.Where(p => p.id_rca == ids).ToList();
                foreach (rca_desc r in lc)
                {
                    RCASessionRepository.db.rca_desc.Remove(r);
                    RCASessionRepository.db.SaveChanges();
                }
            }
            return Json(true);
        }

        #endregion

        //==============================================================

        #region implementation.html

        //
        // Post: RCA/implementasi
        //
        [HttpPost]
        public ActionResult implementation(string id)
        {
            int ids = -1;
            if (id != null)
            {
                ids = Int32.Parse(id);
                ViewBag.addPir = RCASessionRepository.db.rcas.Find(ids).has_pir;
            }

            var has = (from employees in RCASessionRepository.db.employees
                       join dept in RCASessionRepository.db.employee_dept on employees.dept_id equals dept.id
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name
                       }).ToList();
            ViewData["users"] = has;
            
            ViewBag.id = ids;
            ViewData["user_role"] = li;
            rca_implementation[] df = RCASessionRepository.db.rca_implementation.ToArray();
            return View();
        }

        //
        //Select implementation
        [GridAction]
        public ActionResult _SelectImplementation(int ids)
        {
            return bindingImplementation(ids);
        }

        //select all implementation
        [GridAction]
        private ViewResult bindingImplementation(int ids)
        {
            List<rca_implementation> result = RCASessionRepository.db.rca_implementation.Where(p => p.id_rca == ids).ToList();
            int? fracasir = (Nullable<Int32>)RCASessionRepository.db.rcas.Find(ids).fracas_ir;
            int? fracasirid = (Nullable<Int32>)RCASessionRepository.db.rcas.Find(ids).fracas_ir_id;

            string s = "";
            

            if (fracasir == 1)
            {
                equipment_event ee = RCASessionRepository.db.equipment_event.Find(fracasirid);
                foreach (rca_implementation ri in result)
                {
                    if (ee.long_term_act != null)
                    {
                        if (!ee.long_term_act.Contains(ri.next_action))
                        {
                            s += ri.next_action + ", ";
                        }
                    }
                    else
                    {
                        s += ri.next_action + ", ";
                    }
                    
                }

                if (s != "")
                {
                    s = s.Substring(0, s.Length - 2);
                    if (ee.long_term_act != null && ee.long_term_act != "") ee.long_term_act += ", " + s;
                    else ee.long_term_act = s;
                }
                Debug.WriteLine(ee.long_term_act);
                RCASessionRepository.db.Entry(ee).State = EntityState.Modified;
                RCASessionRepository.db.SaveChanges();
            }
            else if (fracasir == 2)
            {
                incident_report ee = RCASessionRepository.db.incident_report.Find(fracasirid);
                foreach (rca_implementation ri in result)
                {
                    if (ee.immediate_action == null) {
                        s += ri.next_action + "\n";
                    }else if (!ee.immediate_action.Contains(ri.next_action))
                    {
                        s += ri.next_action + "\n";
                    }
                }

                if (s != "") 
                {
                    s = s.Substring(0, s.Length - 1);
                    if (ee.immediate_action != "") ee.immediate_action += "\n" + s;
                    else ee.immediate_action += s;
                }
                Debug.WriteLine(ee.immediate_action);
                RCASessionRepository.db.Entry(ee).State = EntityState.Modified;
                RCASessionRepository.db.SaveChanges();
            }
            return View(new GridModel<rca_implementation>
            {
                Data = result
            });
        }

        //
        // Ajax insert binding implementation
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertImplementation(int ids)
        {
            rca_implementation implementation = new rca_implementation();
            implementation.is_complete = 0;
            implementation.id_rca = ids;
            if (TryUpdateModel(implementation))
            {
                if (implementation.next_action == null || implementation.next_action == "")
                {
                    ModelState.AddModelError("next_action", "Next action required");
                }
                else
                {
                    create(implementation);
                }
            }
            return bindingImplementation(ids);
        }

        //insert data implementation
        public void create(rca_implementation implementation)
        {
            RCASessionRepository.db.rca_implementation.Add(implementation);
            RCASessionRepository.db.SaveChanges();
        }

        //
        // Ajax update binding implementation
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _UpdateImplementation(int ids, int id)
        {
            rca_implementation editable = RCASessionRepository.db.rca_implementation.Find(id);
            if (TryUpdateModel(editable))
            {
                update(editable);
            }
            return bindingImplementation(ids);
        }

        //update data rca_implementation
        private void update(rca_implementation implementation)
        {
            RCASessionRepository.db.Entry(implementation).State = EntityState.Modified;
            RCASessionRepository.db.SaveChanges();
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteImplementation(int id, int ids)
        {
            delete(id);
            return bindingImplementation(ids);
        }

        //delete data rca_implementation
        private void delete(int id)
        {
            rca_implementation implementation = RCASessionRepository.db.rca_implementation.Find(id);
            RCASessionRepository.db.rca_implementation.Remove(implementation);
            RCASessionRepository.db.SaveChanges();

            int? fracasir = (Nullable<Int32>)RCASessionRepository.db.rcas.Find(implementation.id_rca).fracas_ir;
            int? fracasirid = (Nullable<Int32>)RCASessionRepository.db.rcas.Find(implementation.id_rca).fracas_ir_id;


            if (fracasir == 1)
            {
                equipment_event ee = RCASessionRepository.db.equipment_event.Find(fracasirid);
                Debug.WriteLine(ee.long_term_act.Remove(ee.long_term_act.IndexOf(implementation.next_action) == 0 ? 0 : ee.long_term_act.IndexOf(implementation.next_action) - 2, implementation.next_action.Length + 2));
                ee.long_term_act = ee.long_term_act.Remove(ee.long_term_act.IndexOf(implementation.next_action) == 0 ? 
                    0 : ee.long_term_act.IndexOf(implementation.next_action) - 2, implementation.next_action.Length + 2);
                Debug.WriteLine(ee.long_term_act);
                RCASessionRepository.db.Entry(ee).State = EntityState.Modified;
                RCASessionRepository.db.SaveChanges();
            }
            else if (fracasir == 2)
            {
                incident_report ee = RCASessionRepository.db.incident_report.Find(fracasirid);
                ee.immediate_action.Remove(ee.immediate_action.IndexOf(implementation.next_action) - 1, implementation.next_action.Length + 1);
                RCASessionRepository.db.Entry(ee).State = EntityState.Modified;
                RCASessionRepository.db.SaveChanges();
            }
        }

        //
        // Ajax complete
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult complete(int id, int ids)
        {
            rca_implementation implementation = RCASessionRepository.db.rca_implementation.Find(id);
            implementation.is_complete = 1;
            implementation.complete_date = DateTime.Now;
            RCASessionRepository.db.SaveChanges();

            return bindingImplementation(ids);
        }

        //
        // Ajax add_pir
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult add_pir(int id, byte add)
        {
            rca rr = RCASessionRepository.db.rcas.Find(id);
            rr.has_pir = add;
            RCASessionRepository.db.SaveChanges();

            return Json(true);
        }

        //
        // Ajax sendEmailToPrinciple
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult sendEmailToPrinciple(int id)
        {
            rca rr = RCASessionRepository.db.rcas.Find(id);
            List<string> s = new List<string>();
            var sendEmail = new SendEmailController();
            employee e = RCASessionRepository.db.employees.Find(RCASessionRepository.db.users.Find(RCASessionRepository.db.rca_team_connector.Find(rr.id_team).id_user).employee_id);
            if (e.email != null)
            {
                s.Add(e.email);
                sendEmail.Send(s, "Salam,\n\nImplementasi dari RCA dengan nama Analisis '" + rr.name + "' harus dibuat PIR.\n\nTerima Kasih.", "Add PIR for Implementation of RCA, Name '" + rr.name + "'");
            }
            return Json(true);
        }

        #endregion

        //==============================================================

        #region report

        //
        // GET: /RCA/report/5
        //
        public ActionResult report(int id)
        {
            ViewBag.ids = id;
            ViewData["imp"] = RCASessionRepository.db.rca_implementation.Where(p => p.id_rca == id).ToList();
            return View();
        }

        //
        // POST: /RCA/SavePrio/5
        //
        [HttpPost]
        public ActionResult SavePrio(string image, int id)
        {
            string dataUrl = image.Replace(' ', '+');
            string data = dataUrl.Substring(dataUrl.IndexOf(',')+1);
            Debug.WriteLine("image = " + data);

            byte[] todecode_byte = Convert.FromBase64String(data);
            
            string filename = "prio" + id + ".png";
            String filepath = Server.MapPath("~/Content/full_image/" + filename);
            FileStream fs = new FileStream(filepath, FileMode.Create);
            fs.Write(todecode_byte, 0, todecode_byte.Length);
            fs.Close();
            return Json(true);
        }

        #endregion

        //==============================================================

        #region importExcel

        //
        // GET: /RCA/importExcelIndex

        public ActionResult importExcelIndex()
        {
            var model = (from o in RCASessionRepository.db.rcas
                         join p in RCASessionRepository.db.rca_department
                         on o.id_division equals p.id into gp
                         from div in gp.DefaultIfEmpty()
                         join q in RCASessionRepository.db.tag_types
                         on o.id_type_equipment equals q.id into gq
                         from types in gq.DefaultIfEmpty()
                         join r in RCASessionRepository.db.disciplines
                         on o.id_equipment_class equals r.id into gr
                         from classes in gr.DefaultIfEmpty()
                         select new RCAEntityModelExcel
                         {
                             name = o.name,
                             description = o.description,
                             type_name = o.rca_analisys_type.name,
                             cost = o.cost,
                             facility = o.rca_facility.name,
                             division = (div == null ? String.Empty : div.name),
                             department = o.rca_section.name,
                             building = o.rca_building.name,
                             floor = o.rca_floor.name,
                             functional_location = o.functional_location,
                             equipment_type = (types == null ? String.Empty : types.title),
                             equipment_class = (classes == null ? String.Empty : classes.title),
                             other = o.other,
                             manufacture = o.manufacture,
                             charter = o.charter,
                             comment = o.comment,
                             start_date = o.start_date,
                             completion_date = o.completion_date,
                             equipment_code = o.equipment_code
                         }
                );

            List<RCAEntityModelExcel> result = model.ToList();

            GridView gv = new GridView();
            gv.Caption = "Root Cause Analysis";
            gv.DataSource = result;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "Analysis Name";
            gv.HeaderRow.Cells[1].Text = "Analysis Description";
            gv.HeaderRow.Cells[2].Text = "Analysis Type";
            gv.HeaderRow.Cells[3].Text = "Estimated Annual Cost of Event (Benefit)";
            gv.HeaderRow.Cells[4].Text = "Facility";
            gv.HeaderRow.Cells[5].Text = "Division";
            gv.HeaderRow.Cells[6].Text = "Department";
            gv.HeaderRow.Cells[7].Text = "Building";
            gv.HeaderRow.Cells[8].Text = "Floor";
            gv.HeaderRow.Cells[9].Text = "Functional Location ID";
            gv.HeaderRow.Cells[10].Text = "Equipment Type";
            gv.HeaderRow.Cells[11].Text = "Equipment Class";
            gv.HeaderRow.Cells[12].Text = "Equipment Code";
            gv.HeaderRow.Cells[13].Text = "Other";
            gv.HeaderRow.Cells[14].Text = "Manufacture";
            gv.HeaderRow.Cells[15].Text = "Charter";
            gv.HeaderRow.Cells[16].Text = "Comment";
            gv.HeaderRow.Cells[17].Text = "Analysis Start Date";
            gv.HeaderRow.Cells[18].Text = "Estimated End Date";

            if (gv != null)
            {
                return new DownloadFileActionResult(gv, "rca.xls");
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        #endregion

        //==============================================================

    }

    //==============================================================

    #region RCASessionRepository

    public static class RCASessionRepository {
        public static relmon_star_energiEntities db = new relmon_star_energiEntities();

        public static IList<RCAEntityModel> AllView()
        {
            IList<RCAEntityModel> result =
                (from o in db.rcas join 
                    p in db.rca_department
                    on o.id_division equals p.id into gp
                    from div in gp.DefaultIfEmpty()
                    join q in db.tag_types
                    on o.id_type_equipment equals q.id into gq
                    from types in gq.DefaultIfEmpty()
                    join r in db.disciplines
                    on o.id_equipment_class equals r.id into gr
                    from classes in gr.DefaultIfEmpty()
                    select new RCAEntityModel
                    {
                        id = o.id,
                        name = o.name,
                        description = o.description,
                        type_name = o.rca_analisys_type.name,
                        id_type = o.id_type,
                        cost = o.cost,
                        id_facility = o.id_facility,
                        facility = o.rca_facility.name,
                        id_division = o.id_division,
                        division = (div == null ? String.Empty : div.name),
                        id_department = o.id_department,
                        department = o.rca_section.name,
                        id_building = o.id_building,
                        building = o.rca_building.name,
                        id_floor = o.id_floor,
                        floor = o.rca_floor.name,
                        functional_location = o.functional_location,
                        id_type_equipment = o.id_type_equipment,
                        equipment_type = (types == null ? String.Empty : types.title),
                        id_equipment_class = o.id_equipment_class,
                        equipment_class = (classes == null ? String.Empty : classes.title),
                        other = o.other,
                        manufacture = o.manufacture,
                        id_csf_connector = o.id_csf_connector,
                        charter = o.charter,
                        comment = o.comment,
                        id_team = o.id_team,
                        start_date = o.start_date,
                        completion_date = o.completion_date,
                        is_publish = o.is_publish,
                        analysis_file = o.analysis_file,
                        equipment_code = o.equipment_code,
                        is_tree = o.is_tree,
                        has_pir = o.has_pir,
                        fracas_ir = o.fracas_ir,
                        fracas_ir_id = o.fracas_ir_id,
                        rca_code = o.rca_code,
                        id_iir = o.id_iir,
                        publish_date = o.publish_date
                    }
                ).ToList();

            foreach (RCAEntityModel rcas in result)
            {
                rcas.member_name = db.rca_team_connector.Where(p => p.id_rca == rcas.id && p.rca_position == 1).Select(p => p.id_user).ToList();

            }
            return result;
        }

        public static IList<rca> AllDb()
        {
            IList<rca> result = db.rcas.ToList();
            return result;
        }

        public static RCAEntityModel OneView(Func<RCAEntityModel, bool> predicate)
        {
            return AllView().Where(predicate).FirstOrDefault();
        }

        public static rca OneDb(Func<rca, bool> predicate)
        {
            return AllDb().Where(predicate).FirstOrDefault();
        }

        public static int Insert(RCAEntityModel analysis)
        {

            rca anal = new rca()
            {
                name = analysis.name,
                description = analysis.description,
                id_type = analysis.id_type,
                cost = analysis.cost,
                id_facility = analysis.id_facility,
                id_division = analysis.id_division,
                id_department = analysis.id_department,
                id_building = analysis.id_building,
                id_floor = analysis.id_floor,
                functional_location = analysis.functional_location,
                id_type_equipment = analysis.id_type_equipment,
                id_equipment_class = analysis.id_equipment_class,
                other = analysis.other,
                manufacture = analysis.manufacture,
                id_csf_connector = analysis.id_csf_connector,
                charter = analysis.charter,
                comment = analysis.comment,
                id_team = analysis.id_team,
                start_date = analysis.start_date,
                completion_date = analysis.completion_date,
                is_publish = analysis.is_publish,
                analysis_file = analysis.analysis_file,
                equipment_code = analysis.equipment_code,
                is_tree = analysis.is_tree,
                has_pir = analysis.has_pir,
                fracas_ir = analysis.fracas_ir,
                fracas_ir_id = analysis.fracas_ir_id,
                rca_code = analysis.rca_code
            };

            db.rcas.Add(anal);
            db.SaveChanges();

            analysis.id = db.rcas.Max(p => p.id);
            analysis.type_name = db.rca_analisys_type.Where(p => p.id == analysis.id_type).FirstOrDefault().name;

            AllView().Add(analysis);

            return analysis.id;
        }
        public static void UpdateRCA(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null && rca != null)
            {
                target.name = analysis.name;
                target.description = analysis.description;
                target.type_name = analysis.type_name;
                target.id_type = analysis.id_type;
                target.cost = analysis.cost;
                target.is_publish = 0;

                rca.name = analysis.name;
                rca.description = analysis.description;
                rca.id_type = analysis.id_type;
                rca.cost = analysis.cost;
                rca.is_publish = 0;
                db.SaveChanges();
            }
        }

        public static void UpdateRCA2(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null && rca != null)
            {
                target.id_facility = analysis.id_facility;
                target.id_division = analysis.id_division;
                target.id_department = analysis.id_department;
                target.id_building = analysis.id_building;
                target.id_floor = analysis.id_floor;
                target.functional_location = analysis.functional_location;
                    
                rca.id_facility = analysis.id_facility;
                rca.id_division = analysis.id_division;
                rca.id_department = analysis.id_department;
                rca.id_building = analysis.id_building;
                rca.id_floor = analysis.id_floor;
                rca.functional_location = analysis.functional_location;
                db.SaveChanges();
            }
        }

        public static void UpdateRCA3(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null && rca != null)
            {
                target.id_type_equipment = analysis.id_type_equipment;
                target.id_equipment_class = analysis.id_equipment_class;
                target.equipment_code = analysis.equipment_code;
                target.other = analysis.other;
                target.manufacture = analysis.manufacture;
                    
                rca.id_type_equipment = analysis.id_type_equipment;
                rca.id_equipment_class = analysis.id_equipment_class;
                rca.equipment_code = analysis.equipment_code;
                rca.other = analysis.other;
                rca.manufacture = analysis.manufacture;
                db.SaveChanges();
            }
        }

        public static void UpdateRCA5(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null)
            {
                target.charter = analysis.charter;
                target.comment = analysis.comment;

                rca.charter = analysis.charter;
                rca.comment = analysis.comment;
                db.SaveChanges();
            }
        }

        public static void UpdateRCA6(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null)
            {
                target.id_team = analysis.id_team;

                rca.id_team = analysis.id_team;
                db.SaveChanges();
            }
        }

        public static void UpdateRCA7(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null)
            {
                target.start_date = analysis.start_date;
                target.completion_date = analysis.completion_date;
                target.rca_code = analysis.rca_code;
                rca.start_date = analysis.start_date;
                rca.completion_date = analysis.completion_date;
                rca.rca_code = analysis.rca_code;
                db.SaveChanges();
            }
        }

        public static void UpdatePublish(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null)
            {
                target.is_publish = 1;
                target.publish_date = DateTime.Today;
                rca.is_publish = 1;
                rca.publish_date = DateTime.Today;
                db.SaveChanges();
            }
        }

        public static void UpdateRCATree(RCAEntityModel analysis)
        {
            RCAEntityModel target = OneView(p => p.id == analysis.id);
            rca rca = OneDb(p => p.id == analysis.id);
            if (target != null)
            {
                target.analysis_file = analysis.analysis_file;
                Debug.WriteLine(analysis.fracas_ir);
                target.fracas_ir = analysis.fracas_ir;
                target.fracas_ir_id = analysis.fracas_ir_id;
                rca.analysis_file = analysis.analysis_file;
                rca.fracas_ir = analysis.fracas_ir;
                rca.fracas_ir_id = analysis.fracas_ir_id;
                db.SaveChanges();
            }
        }

        public static void Delete(RCAEntityModel analysis)
        {
            rca target = OneDb(p => p.id == analysis.id);
            RCAEntityModel hasil = OneView(p => p.id == analysis.id);
            if (target != null)
            {
                //Debug.WriteLine("hasil id = " + hasil.id);
                AllView().Remove(hasil);
                db.rcas.Remove(target);
                db.SaveChanges();
            }
        }
    }

    class TeamComparer : IEqualityComparer<rca_team_connector>
    {
        // Products are equal if their names and product numbers are equal. 
        public bool Equals(rca_team_connector x, rca_team_connector y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.id_user == y.id_user && x.id_rca == y.id_rca;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(rca_team_connector product)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null. 
            int hashProductName = product.id_user == null ? 0 : product.id_user.GetHashCode();

            //Get hash code for the Code field. 
            int hashProductCode = product.id_rca.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashProductCode;
        }
    }

    class CsfComparer : IEqualityComparer<rca_csf_conector>
    {
        // Products are equal if their names and product numbers are equal. 
        public bool Equals(rca_csf_conector x, rca_csf_conector y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.id_csf == y.id_csf && x.id_rca == y.id_rca;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public int GetHashCode(rca_csf_conector product)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null. 
            int hashProductName = product.id_csf == null ? 0 : product.id_csf.GetHashCode();

            //Get hash code for the Code field. 
            int hashProductCode = product.id_rca == null ? 0 : product.id_rca.GetHashCode();

            //Calculate the hash code for the product. 
            return hashProductName ^ hashProductCode;
        }

    }

    #endregion

}

