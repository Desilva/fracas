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
    public class SecurityController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        //
        // GET: /User/

        public ActionResult Index()
        {
            var data = from a in db.master_security
                       select new SecurityEntity()
                       {
                           id = a.id,
                           id_employee = a.id_employee,
                           employee_name = a.employee.alpha_name,
                       };
            ViewData["employee"] = db.employees.OrderBy(x=>x.alpha_name);

            return PartialView(data);
        }

        //
        // Ajax select binding
        [GridAction]
        public ActionResult _SelectAjaxEditing()
        {
            return binding();
        }

        //
        // Ajax insert binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _InsertAjaxEditing()
        {
            SecurityEntity data = new SecurityEntity();
            
            if (TryUpdateModel(data))
            {
                var check = (from a in db.master_security
                             where a.id_employee == data.id_employee
                             select a).FirstOrDefault();
                if (check != null)
                {
                    ModelState.AddModelError("id_employee", "User already registered as security");
                }
                else
                {
                    create(data);
                }
            }
            
            return binding();
        }

        //
        // Ajax update binding
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveAjaxEditing(int id)
        {

            //daily_log_wells editable = db.daily_log_wells.Find(id);
            //if (TryUpdateModel(editable))
            //{
            //    update(editable);
            //}
            //return binding();

            var editable = new SecurityEntity
            {
                id = id,
            };

            if (TryUpdateModel(editable, null, null, new[] { "Employee" }))
            {
                var check = (from a in db.master_security
                             where a.id_employee == editable.id_employee && a.id == editable.id
                             select a).FirstOrDefault();
                if (check != null)
                {
                    update(editable);
                    
                }
                else
                {
                    var check2 = (from a in db.master_security
                                 where a.id_employee == editable.id_employee
                                 select a).FirstOrDefault();
                    if (check2 != null)
                    {
                        ModelState.AddModelError("id_employee", "User already registered as security");
                    }
                    else
                    {
                        update(editable);
                    }
                }
                
            }
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

        //select data failure mode
        private ViewResult binding()
        {
            var model = from o in db.master_security
                        select new SecurityEntity
                        {
                            id = o.id,
                            id_employee = o.id_employee,
                            employee_name = o.employee.alpha_name,
                        };
            return View(new GridModel<SecurityEntity>
            {
                Data = model,
            });
        }

        //insert data failure causes
        public void create(SecurityEntity securityEntity)
        {
            master_security data = new master_security();
            data.id_employee = securityEntity.id_employee;
            db.master_security.Add(data);
            db.SaveChanges();
        }

        //update data failure cause
        private void update(SecurityEntity dt)
        {
            master_security data = db.master_security.Find(dt.id);
            data.id_employee = dt.id_employee;
            //data.description = dt.description;
            //data.id_tag_type = dt.id_tag_type;
            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();
        }

        //delete data failure mode
        private void delete(int id)
        {
            master_security data = db.master_security.Find(id);
            db.master_security.Remove(data);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}