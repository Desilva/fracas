using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System.Data;
using StarEnergi.Utilities;
using System.Collections.Specialized;

namespace StarEnergi.Controllers
{
    public class AccountController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling e = new ErrorHandling();
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.Password))
                {
                    string err = ""; // ValidateUserAndRole(0, returnUrl, model.UserName);
                    if (err == "")
                    {
                        user a = db.users.Find(model.UserName);
                        employee e = db.employees.Find(a.employee_id);
                        FormsAuthentication.SetAuthCookie(e.alpha_name, model.RememberMe);
                        List<user_per_role> li = db.user_per_role.Where(p => p.username == model.UserName).ToList();
                        HttpContext.Session.Add("roles", li);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            if (li.Exists(r => r.role == (int)Config.role.ADMIN))
                            {
                                return RedirectToAction("Index", "admin");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Dashboard");
                            }
                            
                        }
                    }
                    else {
                        ModelState.AddModelError("", err);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            Session["role"] = null;
            Session["username"] = null;
            Session["roles"] = null;
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Dashboard");
        }

        private bool ValidateUser(string username, string password)
        {
            password = EncodePassword(password);
            user a = db.users.Find(username);
            if (a != null) {
                if(a.password == password){
                    Session["username"] = a.username;
                    Session["id"] = a.employee_id;
                    return true;
                }
                return false;
            }
            return false;
        }

        private string ValidateUserAndRole(int role, string returnUrl, string username)
        {
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                List<user_per_role> li = db.user_per_role.Where(p => p.username == username).ToList();
                if(returnUrl == "/pir"){
                    if ((li.Exists(p => p.role == (int)Config.role.PIRINITIATOR)) || (li.Exists(p => p.role == (int)Config.role.PIRPROCESS)))
                    {
                        HttpContext.Session.Add("roles", li);
                        return "";
                    }
                    else {
                        return "The user name or password provided for PIR is incorrect.";
                    }
                }

                if (returnUrl == "/rca")
                {
                    if ((li.Exists(p => p.role == (int)Config.role.RCA)) || (li.Exists(p => p.role == (int)Config.role.RCAVIEW)))
                    {
                        HttpContext.Session.Add("roles", li);
                        return "";
                    }
                    else
                    {
                        return "The user name or password provided for RCA is incorrect.";
                    }
                }

                if (returnUrl == "/admin")
                {
                    if ((li.Exists(p => p.role == (int)Config.role.ADMIN)) || (li.Exists(p => p.role == (int)Config.role.FRACAS))
                         || (li.Exists(p => p.role == (int)Config.role.IIR)) || (li.Exists(p => p.role == (int)Config.role.INITIATORIR)))
                    {
                        HttpContext.Session.Add("roles", li);
                        return "";
                    }
                    else
                    {
                        return "The user name or password provided for Admin is incorrect.";
                    }
                }
            }
            return "";
        }

        private string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes).Replace("-","").ToLower();
        }

        public ActionResult ChangeProfile(int id) {

            employee employee = db.employees.Find(id);
            ViewBag.employee_id = employee.employee_boss;
            ViewBag.dept_id = employee.dept_id;
            return View(employee);
        }

        public ActionResult Delegate(int id)
        {

            employee employee = db.employees.Find(id);

            Dictionary<int, string> delegate_name = new Dictionary<int, string>();
            if (employee.employee2 != null)
            {
                delegate_name.Add(employee.employee2.id, employee.employee2.alpha_name);
                foreach (employee e in employee.employee2.employee1)
                {
                    if (e.id != employee.id)
                        delegate_name.Add(e.id, e.alpha_name);
                }
            }
            foreach (employee e in employee.employee1)
            {
                delegate_name.Add(e.id, e.alpha_name);
            }
            ViewBag.delegate_name = new SelectList(delegate_name, "Key", "Value",employee.employee_delegate);
            return View(employee);
        }

        //
        // POST: /EmployeeDept/Edit/5

        [HttpPost]
        public ActionResult Edit(employee employee)
        {
            NameValueCollection nvc = Request.Form;
            string password = nvc["password"].ToString();


            db.Entry(employee).State = EntityState.Modified;
            IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
            if (error.Count() == 0)
            {
                db.SaveChanges();

                if(password != ""){
                    password = EncodePassword(password);
                    user u = db.users.Find(Session["username"].ToString());

                    u.password = password;

                    db.Entry(u).State = EntityState.Modified;
                    db.SaveChanges();

                }

                string result = "EMPLOYEE" + ";" + employee.id;
                return Json(e.Succes(result));
            }
            else
            {
                //return Json(error.First().ValidationErrors.ToArray());
                return Json(e.Fail(error));
            }
        }

        //
        // POST: /EmployeeDept/Edit/5

        [HttpPost]
        public ActionResult Delegate(employee employee)
        {
            NameValueCollection nvc = Request.Form;
            string prevDelegate = "";
            employee es = db.employees.Find(employee.id);
            prevDelegate = es.employee_delegate.ToString();
            es.delagate = employee.delagate;
            es.employee_delegate = employee.employee_delegate;
            db.Entry(es).State = EntityState.Modified;
            IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
            if (error.Count() == 0)
            {
                db.SaveChanges();

                // IR Delegate
                // get all data from IR where Field Manager isn't approved yet, 
                // then check whether this employee is supervisor, superintendent, safety supervisor, she superintendent, or field manager in each report
                List<incident_report> listIr = db.incident_report.Where(p => p.field_manager_approve == null).ToList();

                if (employee.delagate == 1)
                {
                    foreach (incident_report Ir in listIr)
                    {
                        // as initiator - to be confirmed because delegation hasn't been made yet

                        // as supervisor
                        if (Ir.ack_supervisor == es.id.ToString() && (Ir.supervisor_approve == null || Ir.supervisor_approve.Substring(0,1) == "a"))
                        {
                            Ir.supervisor_delegate = es.employee_delegate.ToString();
                        }

                        // as superintendent
                        if (Ir.superintendent == es.id.ToString() && (Ir.superintendent_approve == null || Ir.superintendent_approve.Substring(0, 1) == "a"))
                        {
                            Ir.superintendent_delegate = es.employee_delegate.ToString();
                        }

                        // as safety supervisor
                        if (Ir.loss_control == es.id.ToString() && (Ir.loss_control_approve == null || Ir.loss_control_approve.Substring(0, 1) == "a"))
                        {
                            Ir.loss_control_delegate = es.employee_delegate.ToString();
                        }

                        // as SHE superintendent
                        if (Ir.she_superintendent == es.id.ToString() && (Ir.she_superintendent_approve == null || Ir.she_superintendent_approve.Substring(0, 1) == "a"))
                        {
                            Ir.she_superintendent_delegate = es.employee_delegate.ToString();
                        }

                        // as Field Manager
                        if (Ir.field_manager == es.id.ToString() && (Ir.field_manager_approve == null || Ir.field_manager_approve.Substring(0, 1) == "a"))
                        {
                            Ir.field_manager_delegate = es.employee_delegate.ToString();
                        }
                        db.Entry(Ir).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (employee.delagate == 0)
                {
                    foreach (incident_report Ir in listIr)
                    {
                        // as initiator - to be confirmed because delegation hasn't been made yet

                        // as supervisor
                        if (Ir.ack_supervisor == es.id.ToString() && Ir.supervisor_delegate == prevDelegate && (Ir.supervisor_approve == null || Ir.supervisor_approve.Substring(0,1) == "a"))
                        {
                            Ir.supervisor_delegate = null;
                        }

                        // as superintendent
                        if (Ir.superintendent == es.id.ToString() && Ir.superintendent_delegate == prevDelegate && (Ir.superintendent_approve == null || Ir.superintendent_approve.Substring(0, 1) == "a"))
                        {
                            Ir.superintendent_delegate = null;
                        }

                        // as safety supervisor
                        if (Ir.loss_control == es.id.ToString() && Ir.loss_control_delegate == prevDelegate && (Ir.loss_control_approve == null || Ir.loss_control_approve.Substring(0, 1) == "a"))
                        {
                            Ir.loss_control_delegate = null;
                        }

                        // as SHE superintendent
                        if (Ir.she_superintendent == es.id.ToString() && Ir.she_superintendent_delegate == prevDelegate && (Ir.she_superintendent_approve == null || Ir.she_superintendent_approve.Substring(0, 1) == "a"))
                        {
                            Ir.she_superintendent_delegate = null;
                        }

                        // as Field Manager
                        if (Ir.field_manager == es.id.ToString() && Ir.field_manager_delegate == prevDelegate && (Ir.field_manager_approve == null || Ir.field_manager_approve.Substring(0, 1) == "a"))
                        {
                            Ir.field_manager_delegate = null;
                        }
                        db.Entry(Ir).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                

                // IIR Delegate
                // get all data from IIR where Field Manager isn't approved yet, 
                // then check whether this employee is investigator(s), safety supervisor, she superintendent, or field manager in each report
                List<investigation_report> listIir = db.investigation_report.Where(p => p.field_manager_approve == null).ToList();

                if (employee.delagate == 1)
                {
                    foreach (investigation_report Iir in listIir)
                    {
                        // as Investigator - to be continued because delegation hasn't been made yet

                        // as Safety Supervisor
                        if (Iir.loss_control == es.id.ToString() && (Iir.loss_control_approve == null || Iir.loss_control_approve.Substring(0, 1) == "a"))
                        {
                            Iir.loss_control_delegate = es.employee_delegate.ToString();
                        }

                        // as SHE Superintendent
                        if (Iir.safety_officer == es.id.ToString() && (Iir.safety_officer_approve == null || Iir.safety_officer_approve.Substring(0, 1) == "a"))
                        {
                            Iir.safety_officer_delegate = es.employee_delegate.ToString();
                        }

                        // as Field Manager
                        if (Iir.field_manager == es.id.ToString() && (Iir.field_manager_approve == null || Iir.field_manager_approve.Substring(0, 1) == "a"))
                        {
                            Iir.field_manager_delegate = es.employee_delegate.ToString();
                        }
                        db.Entry(Iir).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (employee.delagate == 0)
                {
                    foreach (investigation_report Iir in listIir)
                    {
                        // as Investigator - to be continued because delegation hasn't been made yet

                        // as Safety Supervisor
                        if (Iir.loss_control == es.id.ToString() && Iir.loss_control_delegate == prevDelegate && (Iir.loss_control_approve == null || Iir.loss_control_approve.Substring(0, 1) == "a"))
                        {
                            Iir.loss_control_delegate = null;
                        }

                        // as SHE Superintendent
                        if (Iir.safety_officer == es.id.ToString() && Iir.safety_officer_delegate == prevDelegate && (Iir.safety_officer_approve == null || Iir.safety_officer_approve.Substring(0, 1) == "a"))
                        {
                            Iir.safety_officer_delegate = null;
                        }

                        // as Field Manager
                        if (Iir.field_manager == es.id.ToString() && Iir.field_manager_delegate == prevDelegate && (Iir.field_manager_approve == null || Iir.field_manager_approve.Substring(0, 1) == "a"))
                        {
                            Iir.field_manager_delegate = null;
                        }
                        db.Entry(Iir).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                

                // TSR Delegate
                // get all data from IIR where Initiator Superintendent isn't approved yet, 
                // then check whether this employee is initiator, initiator supervisor, or initiator superintendent in each report
                List<trouble_shooting> listTsr = db.trouble_shooting.Where(p => p.superintendent_approval_signature == null).ToList();
                if (employee.delagate == 1)
                {
                    foreach (trouble_shooting Tsr in listTsr)
                    {
                        // as Initiator - to be confirmed

                        // as Initiator Supervisor
                        if (Tsr.supervisor_approval_name == es.id.ToString() && (Tsr.supervisor_approval_signature == null || Tsr.supervisor_approval_signature.Substring(0, 1) == "a"))
                        {
                            Tsr.supervisor_delegate = es.employee_delegate.ToString();
                        }

                        // as Initiator Superintendent
                        if (Tsr.superintendent_approval_name == es.id.ToString() && (Tsr.superintendent_approval_signature == null || Tsr.superintendent_approval_signature.Substring(0, 1) == "a"))
                        {
                            Tsr.superintendent_delegate = es.employee_delegate.ToString();
                        }

                        db.Entry(Tsr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (employee.delagate == 0)
                {
                    foreach (trouble_shooting Tsr in listTsr)
                    {
                        // as Initiator - to be confirmed

                        // as Initiator Supervisor
                        if (Tsr.supervisor_approval_name == es.id.ToString() && Tsr.supervisor_delegate == prevDelegate && (Tsr.supervisor_approval_signature == null || Tsr.supervisor_approval_signature.Substring(0, 1) == "a"))
                        {
                            Tsr.supervisor_delegate = null;
                        }

                        // as Initiator Superintendent
                        if (Tsr.superintendent_approval_name == es.id.ToString() && Tsr.superintendent_delegate == prevDelegate && (Tsr.superintendent_approval_signature == null || Tsr.superintendent_approval_signature.Substring(0, 1) == "a"))
                        {
                            Tsr.superintendent_delegate = null;
                        }

                        db.Entry(Tsr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                // RCA Delegate
                // get all data from RCA where Field Manager isn't approved yet,
                // then check whether this employee is Principle Analyst, PA Superintendent, or Field Manager in each report
                // to be continued - online approval system hasn't been made

                // PIR Delegate
                // get all data from PIR where PIR isn't verified yet,
                // to be continued - must be checked first with current system

                

                return Json(e.Succes("Success"));
            }
            else
            {
                //return Json(error.First().ValidationErrors.ToArray());
                return Json(e.Fail(error));
            }
        }

    }
}
