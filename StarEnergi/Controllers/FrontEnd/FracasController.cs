using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using Telerik.Web.Mvc;
using System.Data;
using System.Data.Entity.Validation;
using StarEnergi.Utilities.Statistical_Engine;
using System.Collections.Specialized;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    [Authorize]
    public class FracasController : Controller
    {

        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private ErrorHandling er = new ErrorHandling();
        //
        // GET: /Fracas/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridEquipment(int id)
        {
            return PartialView();
        }

        public ActionResult GridPart(int id)
        {
            return PartialView();
        }

        #region grid index
        //
        // Ajax select binding equipment
        [GridAction]
        public ActionResult _SelectAjaxEditingEquipment(int id)
        {
            return bindingEquipment(id);
        }

        //select data equipment fracas
        private ViewResult bindingEquipment(int id)
        {
            List<equipment_event> temp = new List<equipment_event>();
            if(id == 0){
                var model = from o in db.equipment_event
                            where o.datetime_stop != null 
                            select o;
                temp = model.ToList();
            }
            else if (id == 1)
            {
                var model = from o in db.equipment_event
                            where o.datetime_stop != null && o.status == 1
                            select o;
                temp = model.ToList();
            }
            else if (id == 2)
            {
                var model = from o in db.equipment_event
                            where o.datetime_stop != null && o.status == 0
                            select o;
                temp = model.ToList();
            }
            else if (id == 3)
            {
                var model = from o in db.equipment_event
                            where o.datetime_stop != null && o.datetime_ops == null
                            select o;
                temp = model.ToList();
            }
            List<FracasEventModel> f = new List<FracasEventModel>();
            List<incident_report> list_ir = db.incident_report.ToList();
            List<trouble_shooting> list_tsr = db.trouble_shooting.ToList();
            foreach (var item in temp)
            {
                FracasEventModel fTemp = new FracasEventModel();
                fTemp.id = item.id;
                fTemp.dateTimeStop = item.datetime_stop;
                fTemp.dateTimeStart = item.datetime_ops;
                fTemp.durasi = item.durasi;
                fTemp.downtime = item.downtime;
                fTemp.tagNumber = item.equipment.tag_num;
                fTemp.failureMode = item.failure_mode;
                fTemp.status = item.status;
                fTemp.eventDesc = item.event_description;
                fTemp.id_ir = item.id_ir;
                incident_report ir = list_ir.Find(p => p.id == item.id_ir);
                trouble_shooting tsr = list_tsr.Find(p => p.id_ir == item.id);
                fTemp.id_tsr = tsr != null ? tsr.id : 0;
                fTemp.tsr_number = tsr != null ? tsr.no : "";
                fTemp.ir_number = ir != null ? ir.reference_number : "";
                f.Add(fTemp);
            }

            return View(new GridModel<FracasEventModel>
            {
                Data = f.OrderByDescending(o => o.dateTimeStop)
            });
        }

        //
        // Ajax delete binding equipment fracas
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditingEquipment(int id)
        {
            delete(id);
            return bindingEquipment(0);
        }

        //delete data fracas equipment
        private void delete(int id)
        {
            equipment_event equipment_event = db.equipment_event.Find(id);
            int idEquipment = equipment_event.id_equipment;
            db.equipment_event.Remove(equipment_event);
            db.SaveChanges();
            List<equipment_event> temp = db.equipment_event.Where(o => o.id_equipment == idEquipment).Where(o => o.datetime_ops == null).ToList();

            equipment eq = db.equipments.Find(idEquipment);
            if (temp.Count == 0)
            {
                eq.status = 1;
            }
            else
            {
                eq.status = 0;
            }
            db.Entry(eq).State = EntityState.Modified;
            db.SaveChanges();           
            CalculateEventData ca = new CalculateEventData(idEquipment);
        }

        //
        // Ajax select binding part
        [GridAction]
        public ActionResult _SelectAjaxEditingPart(int id)
        {
            return bindingPart(id);
        }

        //select data part fracas
        private ViewResult bindingPart(int id)
        {
            List<part_unit_event> temp = new List<part_unit_event>();
            if (id == 0)
            {
                var model = from o in db.part_unit_event
                            where o.datetime_stop != null
                            select o;
                temp = model.ToList();
            }
            else if (id == 1)
            {
                var model = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 1
                            select o;
                temp = model.ToList();
            }
            else if (id == 2)
            {
                var model = from o in db.part_unit_event
                            where o.datetime_stop != null && o.status == 0
                            select o;
                temp = model.ToList();
            }
            else if (id == 3)
            {
                var model = from o in db.part_unit_event
                            where o.datetime_stop != null && o.datetime_ops == null
                            select o;
                temp = model.ToList();
            }
            List<FracasEventModel> f = new List<FracasEventModel>();
            List<incident_report> list_ir = db.incident_report.ToList();
            List<trouble_shooting> list_tsr = db.trouble_shooting.ToList();
            foreach (var item in temp)
            {
                FracasEventModel fTemp = new FracasEventModel();
                fTemp.id = item.id;
                fTemp.dateTimeStop = item.datetime_stop;
                fTemp.dateTimeStart = item.datetime_ops;
                fTemp.durasi = item.durasi;
                fTemp.downtime = item.downtime;
                fTemp.tagNumber = item.equipment_part.part.tag_number;
                fTemp.failureMode = item.failure_mode;
                fTemp.eventDesc = item.event_description;
                fTemp.id_ir = item.id_ir;
                incident_report ir = list_ir.Find(p => p.id == item.id_ir);
                trouble_shooting tsr = list_tsr.Find(p => p.id_ir == item.id);
                fTemp.id_tsr = tsr != null ? tsr.id : 0;
                fTemp.tsr_number = tsr != null ? tsr.no : "";
                fTemp.ir_number = ir != null ? ir.reference_number : "";
                fTemp.status = item.status;
                f.Add(fTemp);
            }

            return View(new GridModel<FracasEventModel>
            {
                Data = f.OrderByDescending(o => o.dateTimeStop)
            });
        }
        #endregion

        //
        // Ajax delete binding part fracas
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _DeleteAjaxEditingPart(int id)
        {
            deleteFracasPart(id);
            return bindingPart(0);
        }

        //delete data fracas part
        private void deleteFracasPart(int id)
        {
            part_unit_event part_event = db.part_unit_event.Find(id);
            int idEquipmentPart = part_event.id_equipment_part;
            int idEquipment = part_event.equipment_part.id_equipment;
            db.part_unit_event.Remove(part_event);
            db.SaveChanges();

            //update status part
            List<part_unit_event> temp = db.part_unit_event.Where(o => o.id_equipment_part == idEquipmentPart).Where(o => o.datetime_ops == null).ToList();         
            equipment_part eqp = db.equipment_part.Find(idEquipmentPart);
            if (temp.Count == 0)
            {
                eqp.status = 1;
            }
            else
            {
                eqp.status = 0;
            }
            db.Entry(eqp).State = EntityState.Modified;

            // update status equipment
            // check data in equipment event 
            // if in equipment event running check data another broken part
            List<equipment_event> tempEquipmentEvent = db.equipment_event.Where(o => o.id_equipment == idEquipment).Where(o => o.datetime_ops == null).ToList();

            equipment eq = db.equipments.Find(idEquipment);
            if (tempEquipmentEvent.Count == 0)
            {
                var brokenPart = from o in db.equipment_part
                                 where o.id_equipment == idEquipment && o.status == 0
                                 select o;
                if (brokenPart.ToList().Count > 0)
                {
                    eq.status = 0;
                }
                else
                {
                    eq.status = 1;
                }
            }
            else
            {
                eq.status = 0;
            }
            db.Entry(eq).State = EntityState.Modified;

            db.SaveChanges();
            CalculateEventData ca = new CalculateEventData(idEquipment);
        }

        public ActionResult AddEvent()
        {
            ViewBag.id_area = new SelectList(db.focs, "id", "nama");
            ViewBag.id_immidiate = new SelectList(db.immediate_actions, "id", "description");
            ViewBag.id_long_term = new SelectList(db.long_term_actions, "id", "description");
            ViewBag.id_event_desc = new SelectList(db.event_descriptions, "id", "description");
            ViewBag.id_failure_cause = new SelectList(db.failure_causes, "id", "description");
            ViewBag.id_failure_effect = new SelectList(db.failure_effects, "id", "description");
            ViewBag.id_secondary_effect = new SelectList(db.secondary_effects, "id", "description");

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
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewBag.list_employee = has;
            return PartialView();
        }

        public ActionResult EditEvent(int id)
        {
            equipment_event e = db.equipment_event.Find(id);
            var failureMode = from o in db.failure_modes
                              join eq in db.equipments on o.id_tag_type equals eq.id_tag_type
                              where eq.id == e.id_equipment
                              select new FailureModeEntity
                              {
                                  id = o.id,
                                  description = o.description
                              };

            ViewBag.id_immidiate = new SelectList(db.immediate_actions, "id", "description");
            ViewBag.id_long_term = new SelectList(db.long_term_actions, "id", "description");
            ViewBag.id_event_desc = new SelectList(db.event_descriptions, "id", "description");
            ViewBag.id_failure_cause = new SelectList(db.failure_causes, "id", "description");
            ViewBag.id_failure_mode = new SelectList(failureMode.ToList(), "id", "description");
            ViewBag.id_failure_effect = new SelectList(db.failure_effects, "id", "description");
            ViewBag.id_secondary_effect = new SelectList(db.secondary_effects, "id", "description");          

            
            var model = from o in db.equipment_event
                        where o.id_equipment == e.id_equipment && o.id != id
                        select o;
            DateTime? temp = model.Where(x => x.datetime_ops <= e.datetime_stop).ToList().Max(x => x.datetime_ops).Value;

            ViewBag.last_operation = temp.ToString();

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
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewBag.list_employee = has;

            return PartialView(e);
        }

        public ActionResult ViewEvent(int id)
        {
            equipment_event e = db.equipment_event.Find(id);
            var failureMode = from o in db.failure_modes
                              join eq in db.equipments on o.id_tag_type equals eq.id_tag_type
                              where eq.id == e.id_equipment
                              select new FailureModeEntity
                              {
                                  id = o.id,
                                  description = o.description
                              };         

            
            var model = from o in db.equipment_event
                        where o.id_equipment == e.id_equipment && o.id != id
                        select o;
            DateTime? temp = model.Where(x => x.datetime_ops <= e.datetime_stop).ToList().Max(x => x.datetime_ops).Value;

            ViewBag.last_operation = temp.ToString();

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
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewBag.list_employee = has;

            return PartialView(e);
        }

        public ActionResult EditEventP(int id)
        {
            part_unit_event e = db.part_unit_event.Find(id);
            var failureMode = from o in db.failure_modes
                              join eq in db.equipments on o.id_tag_type equals eq.id_tag_type
                              where eq.id == e.equipment_part.id_equipment
                              select new FailureModeEntity
                              {
                                  id = o.id,
                                  description = o.description
                              };

            ViewBag.id_immidiate = new SelectList(db.immediate_actions, "id", "description");
            ViewBag.id_long_term = new SelectList(db.long_term_actions, "id", "description");
            ViewBag.id_event_desc = new SelectList(db.event_descriptions, "id", "description");
            ViewBag.id_failure_cause = new SelectList(db.failure_causes, "id", "description");
            ViewBag.id_failure_mode = new SelectList(failureMode.ToList(), "id", "description");
            ViewBag.id_failure_effect = new SelectList(db.failure_effects, "id", "description");
            ViewBag.id_secondary_effect = new SelectList(db.secondary_effects, "id", "description");


            var model = from p in db.part_unit_event
                        join o in db.equipment_part on p.id_equipment_part equals o.id
                        where o.id == e.id_equipment_part 
                        select p;
            DateTime? temp = model.Where(x => x.datetime_ops <= e.datetime_stop).ToList().Max(x => x.datetime_ops).Value;

            ViewBag.last_operation = temp.ToString();

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
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewBag.list_employee = has;

            return PartialView(e);
        }

        public ActionResult ViewEventP(int id)
        {
            part_unit_event e = db.part_unit_event.Find(id);
            var failureMode = from o in db.failure_modes
                              join eq in db.equipments on o.id_tag_type equals eq.id_tag_type
                              where eq.id == e.equipment_part.id_equipment
                              select new FailureModeEntity
                              {
                                  id = o.id,
                                  description = o.description
                              };

            var model = from p in db.part_unit_event
                        join o in db.equipment_part on p.id_equipment_part equals o.id
                        where o.id == e.id_equipment_part
                        select p;
            DateTime? temp = model.Where(x => x.datetime_ops <= e.datetime_stop).ToList().Max(x => x.datetime_ops).Value;

            ViewBag.last_operation = temp.ToString();

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
                           dept_name = dept.dept_name,
                           username = (ue.username == null ? String.Empty : ue.username),
                           delagate = employees.delagate,
                           employee_delegate = employees.employee_delegate
                       }).ToList();
            ViewBag.list_employee = has;

            return PartialView(e);
        }

        [HttpPost]
        public JsonResult Save(FracasEventModel fracas) {
            //event Equipment
            if (fracas.idEquipmentPart == null)
            {
                equipment_event e = new equipment_event()
                {
                    id_equipment = fracas.idEquipment,
                    datetime_stop = fracas.dateTimeStop,
                    datetime_ops = fracas.dateTimeStart,
                    downtime = fracas.downtime,
                    durasi = fracas.durasi,
                    failure_mode = fracas.failureMode,
                    failure_cause = fracas.cause,
                    failure_effect = fracas.failureEffect,
                    secondary_effect = fracas.secondaryEffect,
                    failure_severity = fracas.failureSeverity,
                    failure_clss = fracas.failureClss,
                    immediate_act = fracas.immediateAction,
                    long_term_act = fracas.longTermAction,
                    financial_cost = fracas.financialCost,
                    engineer = fracas.engineer,
                    repair_cost = fracas.repairCost,
                    event_description = fracas.eventDesc,
                    status = 0
                };
                db.equipment_event.Add(e);
                equipment eq = db.equipments.Find(fracas.idEquipment);
                if (fracas.dateTimeStart != null)
                {
                    eq.status = 1;
                }
                else
                {
                    eq.status = 0;
                }
                db.Entry(eq).State = EntityState.Modified;
                db.SaveChanges();
                CalculateEventData ca = new CalculateEventData(fracas.idEquipment);
            }
            else
            { //event part
                part_unit_event p = new part_unit_event()
                {
                    id_equipment_part = (int)fracas.idEquipmentPart,
                    datetime_stop = fracas.dateTimeStop,
                    datetime_ops = fracas.dateTimeStart,
                    downtime = fracas.downtime,
                    durasi = fracas.durasi,
                    failure_mode = fracas.failureMode,
                    failure_cause = fracas.cause,
                    failure_effect = fracas.failureEffect,
                    secondary_effect = fracas.secondaryEffect,
                    failure_severity = fracas.failureSeverity,
                    failure_clss = fracas.failureClss,
                    immediate_act = fracas.immediateAction,
                    long_term_act = fracas.longTermAction,
                    financial_cost = fracas.financialCost,
                    engineer = fracas.engineer,
                    repair_cost = fracas.repairCost,
                    event_description = fracas.eventDesc,
                    status = 0
                };
                db.part_unit_event.Add(p);

                equipment e = db.equipments.Find(fracas.idEquipment);
                equipment_part eq = db.equipment_part.Find(fracas.idEquipmentPart);

                if (fracas.dateTimeStart != null)
                {
                    eq.status = 1;
                    //check another part if broken
                    var brokenPart = from o in db.equipment_part
                                     where o.id_equipment == fracas.idEquipment && o.status == 0
                                     select o;
                    if (brokenPart.ToList().Count > 0)
                    {
                        e.status = 0;
                    }
                    else {
                        e.status = 1;
                    }
                }
                else
                {
                    eq.status = 0;
                    e.status = 0;
                }
                db.Entry(eq).State = EntityState.Modified;
                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();
                CalculateEventData ca = new CalculateEventData(fracas.idEquipment);
            }
            return Json(true);
        }

        public JsonResult SaveEdit(FracasEventModel fracas)
        {
            /*
             * cek tanggal fracas
             * if(empty){
             *      save fracas
             * }else{
             *      error tanggal ga memenuhi syarat
             * }
             * 
             */
            bool valid = false;
            equipment_event e = db.equipment_event.Find(fracas.id);
            var validate = from f in db.equipment_event
                           where f.id_equipment == fracas.idEquipment
                                && ((fracas.dateTimeStart > f.datetime_stop && fracas.dateTimeStart < f.datetime_ops) ||
                                    (fracas.dateTimeStop > f.datetime_stop && fracas.dateTimeStop < f.datetime_ops))
                                && f.id != fracas.id
                           select f;

            var upper = from f in db.equipment_event
                        where f.datetime_stop >= e.datetime_ops && f.id != e.id
                        select f.datetime_stop; 
            List<DateTime?> uppers = upper.ToList();
            if (uppers.Count > 0)
            {
                DateTime temp = uppers.Min().Value;
                if((fracas.dateTimeStop < temp ) && (fracas.dateTimeStart < temp)){
                    if (validate.ToList().Count == 0)
                    {
                        valid = true;
                    }
                }
            }
            else {
                if (validate.ToList().Count == 0)
                {
                    valid = true;
                }
            }

            if (valid)
            {
                //update fracas               
                e.id_equipment = fracas.idEquipment;
                e.datetime_stop = fracas.dateTimeStop;
                e.datetime_ops = fracas.dateTimeStart;
                e.downtime = fracas.downtime;
                e.durasi = fracas.durasi;
                e.failure_mode = fracas.failureMode;
                e.failure_cause = fracas.cause;
                e.failure_effect = fracas.failureEffect;
                e.secondary_effect = fracas.secondaryEffect;
                e.failure_severity = fracas.failureSeverity;
                e.failure_clss = fracas.failureClss;
                e.immediate_act = fracas.immediateAction;
                e.long_term_act = fracas.longTermAction;
                e.financial_cost = fracas.financialCost;
                e.engineer = fracas.engineer;
                e.repair_cost = fracas.repairCost;
                e.event_description = fracas.eventDesc;
                e.status = 0;

                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();

                //update equipment
                equipment eq = db.equipments.Find(fracas.idEquipment);
                if (fracas.dateTimeStart != null)
                {
                    eq.status = 1;
                }
                else
                {
                    eq.status = 0;
                }
                db.Entry(eq).State = EntityState.Modified;
                db.SaveChanges();
                CalculateEventData ca = new CalculateEventData(fracas.idEquipment);
                return Json(true);
            }
            else {
                return Json(er.Fail());
            }
        }

        public JsonResult SaveEditP(FracasEventModel fracas)
        {
            /*
             * cek tanggal fracas
             * if(empty){
             *      save fracas
             * }else{
             *      error tanggal ga memenuhi syarat
             * }
             * 
             */
            bool valid = false;
            part_unit_event p = db.part_unit_event.Find(fracas.id);
            var validate = from f in db.part_unit_event
                           where f.id_equipment_part == fracas.idEquipmentPart
                                && ((fracas.dateTimeStart > f.datetime_stop && fracas.dateTimeStart < f.datetime_ops) ||
                                    (fracas.dateTimeStop > f.datetime_stop && fracas.dateTimeStop < f.datetime_ops))
                                && f.id != fracas.id
                           select f;

            var upper = from f in db.part_unit_event
                        where f.datetime_stop >= p.datetime_ops && f.id != p.id
                        select f.datetime_stop;
            List<DateTime?> uppers = upper.ToList();
            if (uppers.Count > 0)
            {
                DateTime temp = uppers.Min().Value;
                if ((fracas.dateTimeStop < temp) && (fracas.dateTimeStart < temp))
                {
                    if (validate.ToList().Count == 0)
                    {
                        valid = true;
                    }
                }
            }
            else
            {
                if (validate.ToList().Count == 0)
                {
                    valid = true;
                }
            }

            if (valid)
            {
                p.id_equipment_part = (int)fracas.idEquipmentPart;
                p.datetime_stop = fracas.dateTimeStop;
                p.datetime_ops = fracas.dateTimeStart;
                p.downtime = fracas.downtime;
                p.durasi = fracas.durasi;
                p.failure_mode = fracas.failureMode;
                p.failure_cause = fracas.cause;
                p.failure_effect = fracas.failureEffect;
                p.secondary_effect = fracas.secondaryEffect;
                p.failure_severity = fracas.failureSeverity;
                p.failure_clss = fracas.failureClss;
                p.immediate_act = fracas.immediateAction;
                p.long_term_act = fracas.longTermAction;
                p.financial_cost = fracas.financialCost;
                p.engineer = fracas.engineer;
                p.repair_cost = fracas.repairCost;
                p.event_description = fracas.eventDesc;
                p.status = 0;

                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();

                equipment e = db.equipments.Find(p.equipment_part.id_equipment);
                equipment_part eq = db.equipment_part.Find(fracas.idEquipmentPart);

                if (fracas.dateTimeStart != null)
                {
                    eq.status = 1;
                    //check another part if broken
                    var brokenPart = from o in db.equipment_part
                                     where o.id_equipment == p.equipment_part.id_equipment && o.status == 0
                                     select o;
                    if (brokenPart.ToList().Count > 0)
                    {
                        e.status = 0;
                    }
                    else
                    {
                        e.status = 1;
                    }
                }
                else
                {
                    eq.status = 0;
                    e.status = 0;
                }
                db.Entry(eq).State = EntityState.Modified;
                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();
                CalculateEventData ca = new CalculateEventData(p.equipment_part.id_equipment);
                return Json(true);
            }
            else
            {
                return Json(er.Fail());
            }
        }

        #region dropdown
        public JsonResult GetUnit(int id_area)
        {
            var model = from o in db.units
                        where o.id_foc == id_area
                        select new UnitEntity
                        {
                            id = o.id,
                            nama = o.nama
                        };
            return Json(model.ToList());
        }

        public JsonResult GetSystem(int id_unit)
        {
            var model = from o in db.systems
                        where o.id_unit == id_unit
                        orderby o.nama
                        select new SystemEntity
                        {
                            id = o.id,
                            nama = o.nama
                        };
            return Json(model.ToList());
        }

        public JsonResult GetEquipment(int id_system)
        {
            var equipmentEvent = from o in db.equipment_event
                                 where o.datetime_ops == null
                                 select o.id_equipment;

            List<int> temp = equipmentEvent.ToList();

            var model = from o in db.equipments
                        where o.equipment_groups.id_system == id_system && !temp.Contains(o.id)
                        orderby o.tag_num
                        select new TinyEquipmentEntity
                        {
                            id = o.id,
                            tag_num = o.tag_num,
                            nama = o.nama
                        };
            return Json(model.ToList());
        }


        public JsonResult GetDataEquipment(int id_equipment)
        {
            object[] result = new object[6];

            var partEvent = from o in db.part_unit_event
                            where o.datetime_ops == null
                            select o.id_equipment_part;

            List<int> tempPartEvent = partEvent.ToList();

            var part = from o in db.equipment_part
                       where o.id_equipment == id_equipment && !tempPartEvent.Contains(o.id)
                       select new PartEntity
                       {
                           id = o.id,
                           tag_number = o.part.tag_number,
                           nama = o.part.nama
                       };
            var model = from o in db.equipment_event
                        where o.id_equipment == id_equipment
                        select o.datetime_ops;
            DateTime? temp = model.ToList().Max();

            var failureMode = from o in db.failure_modes
                              join e in db.equipments on o.id_tag_type equals e.id_tag_type
                              where e.id == id_equipment
                              select new FailureModeEntity
                              {
                                  id = o.id,
                                  description = o.description
                              };

            var detailEquipment = from o in db.equipments
                                  where o.id == id_equipment
                                  select new { o.nama, o.pdf, o.mtbf, o.mttr,o.mpi,o.acr,o.id_afp,o.id_ocr };

            var detailEquipmentList = detailEquipment.ToList();

            result[0] = part.ToList();
            result[1] = temp.ToString();
            result[2] = failureMode.ToList();
            result[3] = detailEquipmentList;// nama, pdf, mtbf,mttr,mpi,acr
            result[4] = "";//OCR
            result[5] = "";//AFP
            if(detailEquipmentList.Count>0)
            {
                var eq = detailEquipmentList.First();
                if (eq.id_ocr.HasValue)
                {
                    var a = db.ocrs.Where(x => x.id == eq.id_ocr).FirstOrDefault();
                    if(a !=null)
                    {
                        result[4] = a.ocr_value;
                    }
                }
                if (eq.id_afp.HasValue)
                {
                    var a = db.afps.Where(x => x.id == eq.id_afp).FirstOrDefault();
                    if (a != null)
                    {
                        result[5] = a.afp_value;
                    }
                }

            }

            return Json(result);
        }

        public JsonResult GetLastOpPart(int id_part, int id_equipment)
        {
            var model = from p in db.part_unit_event
                        join o in db.equipment_part on p.id_equipment_part equals o.id
                        where o.id == id_part 
                        select p.datetime_ops;
            DateTime? temp = model.ToList().Max();
            return Json(temp.ToString());
        }
        #endregion

        public ActionResult CompleteEq(int id) {
            equipment_event e = db.equipment_event.Find(id);
            e.status = 1;
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        public ActionResult CompleteP(int id)
        {
            part_unit_event e = db.part_unit_event.Find(id);
            e.status = 1;
            db.Entry(e).State = EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public JsonResult getCode(int id)
        {
            int id_equip = db.equipment_event.Find(id).id_equipment;
            return Json(new { id_equip = id_equip, id = id });
        }

        
        
    }
}
