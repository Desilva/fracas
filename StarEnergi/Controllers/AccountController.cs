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
                    string err = ValidateUserAndRole(0,returnUrl,model.UserName);
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
                    if ((li.Exists(p => p.role == (int)Config.role.ADMIN)) || (li.Exists(p => p.role == (int)Config.role.ADMINDATA))
                         || (li.Exists(p => p.role == (int)Config.role.SHE)) || (li.Exists(p => p.role == (int)Config.role.INITIATORIR)))
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

            employee es = db.employees.Find(employee.id);
            es.delagate = employee.delagate;
            es.employee_delegate = employee.employee_delegate;
            db.Entry(es).State = EntityState.Modified;
            IEnumerable<DbEntityValidationResult> error = db.GetValidationErrors();
            if (error.Count() == 0)
            {
                db.SaveChanges();
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
