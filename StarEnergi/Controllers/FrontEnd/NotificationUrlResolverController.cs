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
    public class NotificationUrlResolverController : Controller
    {
        //
        // GET: /NotificationUrlResolver/

        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult FRACAS(string name, int id)
        {
            if (Session["username"] == null)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                 List<user_per_role> li;
                string username = (String)Session["username"].ToString();
                li = db.user_per_role.Where(p => p.username == username).ToList();
                //Response.Write(name);
                //Response.Write(id);
                //Response.End();
                ViewBag.Id = id;
                if (name == "SHE_INCIDENT_REPORT")
                {
                    if (!li.Exists(p => p.role == (int)Config.role.INITIATORIR))
                    {
                        return RedirectToAction("LogOn", "Account", new { returnUrl = "/Incident" });
                    }

                    return View("FRACASSHEIncidentReport");
                }
                else if (name == "SHE_INCIDENT_REPORT_NEW_INVESTIGATION")
                {
                    return RedirectToAction("Index", "RCA", null);
                }
                else if (name == "FRACAS_TROUBLESHOOTING_REPORT")
                {
                    return View("FRACASTroubleshootingReport");
                }
                else if (name == "FRACAS_PIR")
                {
                    if (!(li.Exists(p => p.role == (int)Config.role.PIRINITIATOR)) && !(li.Exists(p => p.role == (int)Config.role.PIRPROCESS)) && !(li.Exists(p => p.role == (int)Config.role.FULLPIR)) && !(li.Exists(p => p.role == (int)Config.role.AUDITOR)))
                    {
                        return RedirectToAction("LogOn", "Account", new { returnUrl = "/pir" });
                    }
                    else
                    {
                        var check = (from a in db.pirs
                                     where a.id == id
                                     select a).FirstOrDefault();

                        if (check == null)
                        {
                            Response.Write("404 NOT FOUND");
                            Response.End();
                        }

                        if (check.status == "FROM INITIATOR")
                        {
                            return View("FRACASPIRProcess");
                        }
                        else
                        {
                            return View("FRACASPIRInitiator");
                        }
                    }
                }
                else if (name == "INCIDENT_INVESTIGATION_REPORT")
                {
                    return RedirectToAction("addInvestigation", "Investigation", new {@id=id });
                }
                else
                {
                    Response.Write("404 NOT FOUND");
                    Response.End();
                    return View();
                }
            }
           
            
        }

    }
}
