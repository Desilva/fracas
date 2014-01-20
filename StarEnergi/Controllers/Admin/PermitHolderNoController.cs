using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Controllers.Admin
{
    public class PermitHolderNoController : Controller
    {
        //
        // GET: /PermitHolderNo/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public ActionResult Index()
        {
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
                           dept_name = employees.department != null ? employees.department : dept.dept_name,
                           dept_id = employees.dept_id,
                           username = (ue.username == null ? String.Empty : ue.username)
                       }).ToList();
            ViewData["employees"] = has;
            return PartialView();
        }

    }
}
