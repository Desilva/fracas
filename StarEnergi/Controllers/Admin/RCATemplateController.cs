using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Telerik.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class RCATemplateController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /RCATemplate/

        public ActionResult Index()
        {
            return PartialView();
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //
        // Ajax delete binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            delete(id);
            return binding();
        }

        //select data user
        private ViewResult binding()
        {
            List<RCATemplateEntity> bind = new List<RCATemplateEntity>();
            var has = (from users in db.rca_template
                       select new RCATemplateEntity
                       {
                           id = users.id,
                           name = users.name,
                           type = users.type
                       }).ToList();
            bind = has;
            foreach (RCATemplateEntity ue in bind)
            {
                ue.type_name = ue.type == 0 ? "5 Why's" : (ue.type == 1 ? "Tree Diagram" : (ue.type == 2 ? "Fishbone Diagram" : ""));
            }
            return View(new GridModel<RCATemplateEntity>
            {
                Data = bind
            });
        }

        //delete data user
        private void delete(int id)
        {
            rca_template template = db.rca_template.Find(id);
            db.rca_template.Remove(template);
            db.SaveChanges();
        }

        public ActionResult Detail3(int? id)
        {
            if (id != null)
            {
                rca_template template = db.rca_template.Find(id);
                ViewBag.template = template;
            }
            return PartialView();
        }

        public ActionResult DetailFish(int? id)
        {
            if (id != null)
            {
                rca_template template = db.rca_template.Find(id);
                ViewBag.template = template;
            }
            ViewBag.master = db.rca_fishbone_master.ToList();
            return PartialView();
        }

        public ActionResult Detail5(int? id)
        {
            ViewBag.events = "";
            if (id != null)
            {
                rca_template template = db.rca_template.Find(id);
                ViewBag.template = template;
                ViewBag.load = 1;
                String filepath = Server.MapPath("~/Content/xml/rca_template/" + template.name + ".xml");
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
            }
            return PartialView();
        }


        #region save

        //
        // Post: RCA/save
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult save3(string filename, string xml, int id)
        {
            string name = filename.Substring(0,filename.LastIndexOf('.'));
            if (db.rca_template.Where(p => p.name == name && p.id != id).Count() > 0)
            {
                return Json(null);
            }
            String filepath = Server.MapPath("~/Content/xml/rca_template/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            rca_template template = null;
            if (id == 0)
            {
                template = new rca_template
                {
                    name = name,
                    type = 1
                };
                db.rca_template.Add(template);
            }
            else
            {
                template = db.rca_template.Find(id);
                template.name = name;
                db.Entry(template).State = EntityState.Modified;
            }
            db.SaveChanges();
            sw.WriteLine(xml);
            sw.Close();
            return Json(true);
        }

        //
        // Post: RCA/save
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult saveFish(string filename, string xml, int id)
        {
            string name = filename.Substring(0, filename.LastIndexOf('.'));
            if (db.rca_template.Where(p => p.name == name && p.id != id).Count() > 0)
            {
                return Json(null);
            }
            String filepath = Server.MapPath("~/Content/xml/rca_template/" + filename);
            StreamWriter sw = new StreamWriter(filepath);
            rca_template template = null;
            if (id == 0)
            {
                template = new rca_template
                {
                    name = name,
                    type = 2
                };
                db.rca_template.Add(template);
            }
            else
            {
                template = db.rca_template.Find(id);
                template.name = name;
                db.Entry(template).State = EntityState.Modified;
            }
            db.SaveChanges();
            sw.WriteLine(xml);
            sw.Close();
            return Json(true);
        }

        //
        // Post: RCA/saveWhy
        //
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult save5(string filename, string events, string[] item, int id)
        {
            string name = filename.Substring(0, filename.LastIndexOf('.'));
            if (db.rca_template.Where(p => p.name == name && p.id != id).Count() > 0)
            {
                return Json(null);
            }
            String filepath = Server.MapPath("~/Content/xml/rca_template/" + filename);
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
            rca_template template = null;
            if (id == 0)
            {
                template = new rca_template
                {
                    name = name,
                    type = 0
                };
                db.rca_template.Add(template);
            }
            else
            {
                template = db.rca_template.Find(id);
                template.name = name;
                db.Entry(template).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Json(true);
        }

        #endregion

    }
}
