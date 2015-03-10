using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace StarEnergi.Controllers.Admin
{
    [Authorize]
    public class UserController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /User/

        public ActionResult Index()
        {
            ViewBag.roles = db.user_role.ToList();
            
            var has = (from employees in db.employees
                       join dept in db.employee_dept on employees.dept_id equals dept.id
                       join users in db.users on employees.id equals users.employee_id into user_employee
                       from ue in user_employee.DefaultIfEmpty()
                       orderby employees.dept_id
                       where employees.employee_dept != null && employees.employee_boss != null
                       select new EmployeeEntity
                       {
                           id = employees.id,
                           alpha_name = employees.alpha_name,
                           employee_no = employees.employee_no,
                           position = employees.position,
                           work_location = employees.work_location,
                           dob = employees.dob,
                           dept_name = dept.dept_name,
                           username = ue.username,
                           employee = employees.employee2
                       }).ToList();
            List<EmployeeEntity> bind = has;
            foreach (EmployeeEntity ee in bind) {
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
            ViewBag.employee = bind;
            return PartialView();
        }

        public JsonResult listUser()
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
                           username = ue.username,
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
                ee.employee = null;
            }
            ViewBag.employee = bind;
            return Json(new { employee = bind });
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
        public ActionResult _DeleteAjaxEditing(string id)
        {
            delete(id);
            return binding();
        }

        //select data user
        private ViewResult binding()
        {
            List<UserEntity> bind = new List<UserEntity>();
            var has = (from users in db.users
                       join employees in db.employees on users.employee_id equals employees.id
                       select new UserEntity
                       {
                           username = users.username,
                           fullname = users.fullname,
                           password = users.password,
                           jabatan = users.jabatan,
                           create_date = users.create_date,
                           rm_role = users.rm_role,
                           alpha_name = employees.alpha_name,
                           position = employees.position,
                           employee_id = users.employee_id
                       }).ToList();
            bind = has;
            foreach (UserEntity ue in bind)
            {
                ue.roles = db.user_per_role.Where(p => p.username == ue.username).ToList();
            }
            return View(new GridModel<UserEntity>
            {
                Data = bind
            });
        }

        //insert data user
        [HttpPost]
        public void create(user user, int[] role)
        {
            if (role != null)
            {
                user.password = EncodePassword(user.password);
                db.users.Add(user);
                db.SaveChanges();
                foreach (int i in role)
                {
                    user_per_role upr = new user_per_role { id = 0, username = user.username, role = i };
                    db.user_per_role.Add(upr);
                    db.SaveChanges();
                }
            }
        }

        //update data user
        [HttpPost]
        public void update(user user, int[] role)
        {
            user u = db.users.Find(user.username);
            u.fullname = user.fullname;
            u.jabatan = user.jabatan;
            if (db.users.Where(s => s.employee_id == user.employee_id).ToList().Count == 0)
            {
                u.employee_id = user.employee_id;
            }
            if(user.password != null){
                
                u.password = EncodePassword(user.password);
            }
            List<user_per_role> li = db.user_per_role.Where(p => p.username == u.username).ToList();
            foreach (user_per_role up in li)
            {
                db.user_per_role.Remove(up);
                db.SaveChanges();
            }

            foreach (int i in role)
            {
                if (!db.user_per_role.ToList().Exists(p => p.username == u.username && p.role == i))
                {
                    user_per_role upr = new user_per_role { id = 0, username = u.username, role = i };
                    db.user_per_role.Add(upr);
                    db.SaveChanges();
                }
            }
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data user
        private void delete(string username)
        {
            user user = db.users.Find(username);
            db.users.Remove(user);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult addUser()
        {
            return Json(true);
        }

        public JsonResult GetDetail(string id)
        {
            user u = db.users.Find(id);
            UserEntity ret = new UserEntity
            {
                username = u.username,
                fullname = u.fullname,
                password = u.password,
                jabatan = u.jabatan,
                create_date = u.create_date,
                rm_role = u.rm_role,
                roles = db.user_per_role.Where(p => p.username == u.username).ToList(),
                alpha_name = db.employees.Find(u.employee_id).alpha_name,
                employee_id = u.employee_id
            };
            return Json(ret,JsonRequestBehavior.AllowGet);
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
            return BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
        }
    }
}