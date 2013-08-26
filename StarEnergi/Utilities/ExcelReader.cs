using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using StarEnergi.Models;
using StarEnergi.Utilities.Statistical_Engine;
using System.Diagnostics;
using System.Globalization;

namespace StarEnergi.Utilities
{
    public class ExcelReader
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        #region region SBS
        public List<string> LoadSBS(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i,j,k = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach(Excel.Worksheet sheet in book.Sheets){
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    if(k == 0){ //insert unit
                        errTemp = saveUnit(temp);
                        if (errTemp != "") {
                            err.Add(errTemp);   
                        };
                    }else if(k == 1){//insert system
                        errTemp = saveSystem(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                    else if (k == 2){//insert equipment group
                        errTemp = saveEquipmentGroup(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }else if(k == 3){//insert equipment
                        errTemp = saveEquipment(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                    else if (k == 4){//insert part
                        errTemp = savePart(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                    else if (k == 5)//insert component
                    {
                        errTemp = saveComponent(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                    else if (k == 6)//insert subcomponent
                    {
                        errTemp = saveSubComponent(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                
                }
                k++;
            }  
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveUnit(List<object> data) {
            string err = "";
            string temp = data[1].ToString();
            List<unit> exist = db.units.Where(x => x.nama == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                foc foc = db.focs.Where(x => x.nama == temp).SingleOrDefault();
                if (foc != null)
                {
                    unit unit = new unit();
                    unit.id_foc = foc.id;
                    unit.nama = data[1].ToString();
                    db.units.Add(unit);
                    db.SaveChanges();
                }
                else {
                    err = "Area " + data[0] + " tidak terdaftar di dalam database";
                }
                
            }
            else {
                err = "Unit " + data[1] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string saveSystem(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            List<system> exist = db.systems.Where(x => x.kode == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                unit unit = db.units.Where(x => x.nama == temp).SingleOrDefault();
                if (unit != null)
                {
                    system system = new system();
                    system.id_unit = unit.id;
                    system.nama = data[2].ToString();
                    system.kode = data[1].ToString();
                    system.funct_description = data[3].ToString();
                    system.failure_scenario = data[4].ToString();
                    system.primary_impact = data[5].ToString();
                    system.secondary_impact = data[6].ToString();
                    system.cons_econ = data[7].ToString();
                    system.cons_hs = data[8].ToString();
                    system.cons_env = data[9].ToString();
                    system.cons_total = data[10].ToString();
                    system.likelihood = data[11].ToString();
                    system.crit_code = data[12].ToString();
                    system.ram_crit = data[13].ToString();
                    system.exist_crit = data[14].ToString();
                    system.h = data[15].ToString();
                    system.e = data[16].ToString();
                    system.econ = data[17].ToString();
                    system.prob = data[18].ToString();
                    
                    db.systems.Add(system);
                    db.SaveChanges();
                }
                else
                {
                    err = "Unit " + data[0] + " tidak terdaftar di dalam database";
                }

            }
            else
            {
                err = "System " + data[2] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string saveEquipmentGroup(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            List<equipment_groups> exist = db.equipment_groups.Where(x => x.nama == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                system system = db.systems.Where(x => x.kode == temp).SingleOrDefault();
                if (system != null)
                {
                    equipment_groups equipment_group = new equipment_groups();
                    equipment_group.id_system = system.id;
                    equipment_group.nama = data[1].ToString();
                    equipment_group.description = data[2].ToString() == "" ? data[1].ToString() : data[2].ToString();
                    db.equipment_groups.Add(equipment_group);
                    db.SaveChanges();
                }
                else
                {
                    err = "Functional Code " + data[0] + " tidak terdaftar di dalam database";
                }

            }
            else
            {
                err = "Equipment Group " + data[1] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string saveEquipment(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            List<equipment> exist = db.equipments.Where(x => x.tag_num == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                equipment_groups equipment_group = db.equipment_groups.Where(x => x.nama == temp).SingleOrDefault();
                if (equipment_group != null)
                {

                    //check discipline and tag_type
                    temp = data[8].ToString(); //tag_types
                    tag_types t = db.tag_types.Where(x => x.title == temp).SingleOrDefault();

                    temp = data[9].ToString(); //discipline
                    discipline d = db.disciplines.Where(x => x.title == temp).SingleOrDefault();

                    if (t == null)
                    {
                        t = new tag_types();
                        t.title = data[8].ToString();
                        db.tag_types.Add(t);
                    }

                    if (d == null)
                    {
                        d = new discipline();
                        d.title = data[9].ToString();
                        d.id_tag_type = t.id;
                        db.disciplines.Add(d);
                    }
                    
                    db.SaveChanges();

                    //equipment
                    equipment equipment = new equipment();
                    equipment.id_equipment_group = equipment_group.id;
                    equipment.tag_num = data[1].ToString();
                    equipment.nama = data[2].ToString();
                    equipment.econ = (int)(data[3].ToString() == "" ? 0 : float.Parse(data[3].ToString()));
                    equipment.installed_date = data[4].ToString() == "" ? DateTime.Now : DateTime.Parse(data[4].ToString());        
                    equipment.vendor = data[5].ToString();
                    equipment.warranty = data[6].ToString() == "" ? 0 : int.Parse(data[6].ToString());
                    equipment.obsolete_date = equipment.installed_date.Value.Add(new TimeSpan((int)equipment.warranty, 0, 0));
                    equipment.ram_crit = data[7].ToString();
                    equipment.sertifikasi = data[10].ToString() == "" ? DateTime.Now : DateTime.Parse(data[10].ToString());        
                    equipment.id_discipline = d.id;
                    equipment.id_tag_type = t.id;
                    equipment.status_read_nav = 0;
                    db.equipments.Add(equipment);
                    db.SaveChanges();

                    //insert equipment detail
                    equipment_readiness_nav eqReadNav = new equipment_readiness_nav()
                    {
                        id_equipment = equipment.id
                    };
                    equipment_paf eqPaf = new equipment_paf()
                    {
                        id_equipment = equipment.id
                    };
                    equipment_event eqEvent = new equipment_event()
                    {
                        id_equipment = equipment.id,
                        datetime_ops = equipment.installed_date.Value
                    };

                    db.equipment_readiness_nav.Add(eqReadNav);
                    db.equipment_paf.Add(eqPaf);
                    db.equipment_event.Add(eqEvent);

                    db.SaveChanges();
                }
                else
                {
                    err = "Equipment Group " + data[0] + " tidak terdaftar di dalam database";
                }

            }
            else
            {
                err = "Equipment " + data[1] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string savePart(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            part part = db.parts.Where(x => x.tag_number == temp).SingleOrDefault();
            if(part == null){
                part = new part();
                part.tag_number = data[1].ToString();
                part.nama = data[2].ToString();
                part.vendor = data[3].ToString();
                part.warranty = data[4].ToString() == "" ? 0 : int.Parse(data[4].ToString());
                db.parts.Add(part);
                db.SaveChanges();
            }

            temp = data[0].ToString();
            equipment equipment = db.equipments.Where(x => x.tag_num == temp).SingleOrDefault();


            if (equipment != null)
            {
                List<equipment_part> exist = db.equipment_part.Where(x => x.id_equipment == equipment.id).Where(x => x.id_parts == part.id).ToList();
                if (exist.Count == 0)
                {

                    equipment_part ep = new equipment_part();
                    ep.id_parts = part.id;
                    ep.id_equipment = equipment.id;
                    ep.status = 1;
                    ep.installed_date = data[5].ToString() == "" ? DateTime.Now : DateTime.Parse(data[5].ToString());
                    ep.obsolete_date = ep.installed_date.Value.Add(new TimeSpan((int)part.warranty, 0, 0));
                    db.equipment_part.Add(ep);
                    db.SaveChanges();

                    part_unit_event pEvent = new part_unit_event()
                    {
                        id_equipment_part = ep.id,
                        datetime_ops = ep.installed_date.Value
                    };
                    db.part_unit_event.Add(pEvent);
                    db.SaveChanges();
                }
                else
                {
                    err = "Part " + data[1] + " sudah terdaftar di dalam database";
                }

            }
            else
            {
                err = "Equipment " + data[0] + " tidak terdaftar di dalam database";
            }
            return err;
        }

        private string saveComponent(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            List<component> exist = db.components.Where(x => x.tag_number == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                equipment_part ep = db.equipment_part.Where(x => x.part.tag_number == temp).SingleOrDefault();
                if (ep != null)
                {
                    component co = new component();
                    co.id_equipment_part = ep.id;
                    co.tag_number = data[1].ToString();
                    co.description = data[2].ToString();
                    db.components.Add(co);
                    db.SaveChanges();
                }
                else
                {
                    err = "Sub Equipment " + data[0] + " tidak terdaftar di dalam database";
                }

            }
            else
            {
                err = "Component " + data[1] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string saveSubComponent(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            List<sub_component> exist = db.sub_component.Where(x => x.tag_number == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[0].ToString();
                component c = db.components.Where(x => x.tag_number == temp).SingleOrDefault();
                if (c != null)
                {
                    sub_component sc = new sub_component();
                    sc.id_component = c.id;
                    sc.tag_number = data[1].ToString();
                    sc.description = data[2].ToString();
                    db.sub_component.Add(sc);
                    db.SaveChanges();
                }
                else
                {
                    err = "Component " + data[0] + " tidak terdaftar di dalam database";
                }

            }
            else
            {
                err = "Sub Component " + data[1] + " sudah terdapat di dalam database";
            }
            return err;
        }

        #endregion

        #region dataMaster
        public List<string> LoadDataMaster(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j, k = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    if (k == 0)
                    { //insert failure mode
                        errTemp = saveFailureMode(temp);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }
                    else 
                    {//insert other
                        errTemp = saveOther(temp, k);
                        if (errTemp != "")
                        {
                            err.Add(errTemp);
                        };
                    }

                }
                k++;
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveFailureMode(List<object> data)
        {
            string err = "";
            string temp = data[0].ToString();
            List<failure_modes> exist = db.failure_modes.Where(x => x.title == temp).ToList();
            if (exist.Count == 0)
            {
                temp = data[2].ToString();
                tag_types tagType = db.tag_types.Where(x => x.title == temp).SingleOrDefault();

                if (tagType == null) {
                    tagType = new tag_types();
                    tagType.title = temp;
                    db.tag_types.Add(tagType);
                    db.SaveChanges();
                }
           
                failure_modes failureMode = new failure_modes();
                failureMode.title = data[0].ToString();
                failureMode.description = data[1].ToString();
                failureMode.id_tag_type = tagType.id;
                db.failure_modes.Add(failureMode);
                db.SaveChanges();
            }
            else
            {
                err = "Failure Mode " + data[0] + " sudah terdapat di dalam database";
            }
            return err;
        }

        private string saveOther(List<object> data, int type)
        {
            string err = "";
            string temp = data[0].ToString();
            if (type == 1) {//cause
                List<failure_causes> exist = db.failure_causes.Where(x => x.title == temp).ToList();
                if (exist.Count == 0)
                {
                    failure_causes x = new failure_causes{
                        title = data[0].ToString(),
                        description = data[1].ToString()                    
                    };
                    db.failure_causes.Add(x);
                }
                else {
                    err = "Failure Cause " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 2) { //effect
                List<failure_effects> exist = db.failure_effects.Where(x => x.title == temp).ToList();
                if (exist.Count == 0)
                {
                    failure_effects x = new failure_effects
                    {
                        title = data[0].ToString(),
                        description = data[1].ToString()
                    };
                    db.failure_effects.Add(x);
                }
                else
                {
                    err = "Failure Effect " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 3) //secondary
            {
                List<secondary_effects> exist = db.secondary_effects.Where(x => x.title == temp).ToList();
                if (exist.Count == 0)
                {
                    secondary_effects x = new secondary_effects
                    {
                        title = data[0].ToString(),
                        description = data[1].ToString()
                    };
                    db.secondary_effects.Add(x);
                }
                else
                {
                    err = "Secondary Effect " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 4)//immidiate
            {
                List<immediate_actions> exist = db.immediate_actions.Where(x => x.action == temp).ToList();
                if (exist.Count == 0)
                {
                    immediate_actions x = new immediate_actions
                    {
                        action = data[0].ToString(),
                        description = data[1].ToString()
                    };
                    db.immediate_actions.Add(x);
                }
                else
                {
                    err = "Immediate Action " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 5)//long term
            {
                List<long_term_actions> exist = db.long_term_actions.Where(x => x.action == temp).ToList();
                if (exist.Count == 0)
                {
                    long_term_actions x = new long_term_actions
                    {
                        action = data[0].ToString(),
                        description = data[1].ToString()
                    };
                    db.long_term_actions.Add(x);
                }
                else
                {
                    err = "Long term action " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 6)//event
            {
                List<event_descriptions> exist = db.event_descriptions.Where(x => x.title == temp).ToList();
                if (exist.Count == 0)
                {
                    event_descriptions x = new event_descriptions
                    {
                        title = data[0].ToString(),
                        description = data[1].ToString()
                    };
                    db.event_descriptions.Add(x);
                }
                else
                {
                    err = "Event description " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 7)//critical success factor
            {
                List<rca_csf> exist = db.rca_csf.Where(x => x.name == temp).ToList();
                if (exist.Count == 0)
                {
                    rca_csf x = new rca_csf
                    {
                        name = data[0].ToString()
                    };
                    db.rca_csf.Add(x);
                }
                else
                {
                    err = "Critical Success Factor " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 8)//facility
            {
                List<rca_facility> exist = db.rca_facility.Where(x => x.id == Int32.Parse(temp)).ToList();
                if (exist.Count == 0)
                {
                    rca_facility x = new rca_facility
                    {
                        id = Int32.Parse(data[0].ToString()),
                        name = data[1].ToString()
                    };
                    db.rca_facility.Add(x);
                }
                else
                {
                    err = "Facility dengan id " + temp + " sudah terdapat di dalam database";
                }
            }
            else if (type == 9)//division
            {
                List<rca_department> exist = db.rca_department.Where(x => x.id == Int32.Parse(data[1].ToString())).ToList();
                if (exist.Count == 0)
                {
                    rca_department x = new rca_department
                    {
                        id = Int32.Parse(data[1].ToString()),
                        id_facility = Int32.Parse(data[0].ToString()),
                        name = data[2].ToString()
                    };
                    db.rca_department.Add(x);
                }
                else
                {
                    err = "Department dengan id " + data[1].ToString() + " sudah terdapat di dalam database";
                }
            }
            else if (type == 10)//department
            {
                List<rca_section> exist = db.rca_section.Where(x => x.id == Int32.Parse(data[1].ToString())).ToList();
                if (exist.Count == 0)
                {
                    rca_section x = new rca_section
                    {
                        id = Int32.Parse(data[1].ToString()),
                        id_department = Int32.Parse(data[0].ToString()),
                        name = data[2].ToString()
                    };
                    db.rca_section.Add(x);
                }
                else
                {
                    err = "Section dengan id " + data[1].ToString() + " sudah terdapat di dalam database";
                }
            }
            else if (type == 11)//analysis type
            {
                List<rca_analisys_type> exist = db.rca_analisys_type.Where(x => x.name == temp).ToList();
                if (exist.Count == 0)
                {
                    rca_analisys_type x = new rca_analisys_type
                    {
                        name = data[0].ToString()
                    };
                    db.rca_analisys_type.Add(x);
                }
                else
                {
                    err = "Analysis type " + temp + " sudah terdapat di dalam database";
                }
            }
            db.SaveChanges();
            return err;
        }
        #endregion

        #region readinessNavigator
        public List<string> LoadReadinessNavigator(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    errTemp = saveReadNav(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };
                   
                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveReadNav(List<object> data)
        {
            string err = "";
            string temp = data[0].ToString();
            equipment exist = db.equipments.Where(x => x.tag_num == temp).SingleOrDefault();
            if (exist != null)
            {
                equipment_paf last = db.equipment_paf.Where(x => x.id_equipment == exist.id).OrderByDescending(x => x.tanggal).FirstOrDefault();
                double operation = (double.Parse(data[1].ToString())/100 * double.Parse(data[2].ToString())/100) * 100;
                double performance = (double.Parse(data[3].ToString())/100 * double.Parse(data[4].ToString())/100 * last.avail_measured/100) * 100;
                double maintenace = (double.Parse(data[7].ToString())/100 * double.Parse(data[8].ToString())/100) * 100;
               
                double spares = (double.Parse(data[9].ToString())/100 * double.Parse(data[10].ToString())/100) * 100;
                double safe_operation = (double.Parse(data[11].ToString())/100 * double.Parse(data[12].ToString())/100) * 100;
                double monitoring = (double.Parse(data[5].ToString())/100 * double.Parse(data[6].ToString())/100 * performance/100) * 100;

                double score = (operation + maintenace + spares + safe_operation + monitoring) / 5;

                equipment_readiness_nav update = db.equipment_readiness_nav.Where(x => x.id_equipment == exist.id).SingleOrDefault();

                update.boc = double.Parse(data[1].ToString());
                update.bec = double.Parse(data[2].ToString());
                update.capacity = double.Parse(data[3].ToString());
                update.quality = double.Parse(data[4].ToString());
                update.cm_program = double.Parse(data[5].ToString());
                update.sertifikasi = double.Parse(data[6].ToString());
                update.pm_program = double.Parse(data[7].ToString());
                update.overhaul = double.Parse(data[8].ToString());
                update.lifecycle_spare = double.Parse(data[9].ToString());
                update.main_act_spare = double.Parse(data[10].ToString());
                update.safeguard = double.Parse(data[11].ToString());
                update.accesories = double.Parse(data[12].ToString());

                update.operation = operation;
                update.performance = performance;
                update.maintenance = maintenace;
                update.spares = spares;
                update.safe_operation = safe_operation;
                update.monitoring = monitoring;
                update.score = score;

                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                err = "Equipment " + data[0] + " tidak terdaftar di dalam database";
            }
            return err;
        }

        #endregion

        #region MA
        public List<string> LoadMA(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    errTemp = saveMA(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };

                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveMA(List<object> data)
        {
            string err = "";
            string temp = data[1].ToString();
            foc foc = db.focs.Where(x => x.nama == temp).SingleOrDefault();
            if (foc != null)
            {
                string[] input = new string[2];
                input[0] = data[4].ToString();
                input[1] = data[5].ToString();
                int[] result = ConvertData(input);

                DateTime dtDateTime = DateTime.FromOADate(double.Parse(data[0].ToString()));

                ma ma = new ma()
                {
                    last_update = dtDateTime,
                    id_foc = foc.id,
                    ma1 = double.Parse(data[2].ToString()),
                    masd = double.Parse(data[3].ToString()),
                    category = (byte)result[0],
                    type = (byte)result[1]
                };

                db.mas.Add(ma);
                db.SaveChanges();
            }
            else
            {
                err = "Area " + data[1] + " tidak terdaftar di dalam database";
            }
            return err;
        }

        //Convert Category and Type in MA excel template
        private int[] ConvertData(string[] data){
            int[] result = new int[2];

            result[0] = 0;
            result[1] = 0;
            if(data[0] == "Tahunan"){
                result[0] = 1;
            }

            if (data[1] == "Secondary")
            {
                result[1] = 1;
            }

            return result;
        }

        #endregion

        #region Plant Availibility
        public List<string> LoadPlantAvailibility(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    errTemp = savePAF(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };

                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string savePAF(List<object> data)
        {
            string err = "";
            string temp = data[0].ToString();
            foc foc = db.focs.Where(x => x.nama == temp).SingleOrDefault();
            if (foc != null)
            {
                foc_op_avail paf = new foc_op_avail()
                {
                    id_foc = foc.id,
                    tahun = int.Parse(data[1].ToString()),
                    bulan = int.Parse(data[2].ToString()),
                    pof_bulanan = double.Parse(data[3].ToString()),
                    op_avail = double.Parse(data[4].ToString()),
                    paf_bulanan = double.Parse(data[5].ToString()),
                    paf = double.Parse(data[6].ToString()),
                    tipe = data[7].ToString(),
                };

                db.foc_op_avail.Add(paf);
                db.SaveChanges();
            }
            else
            {
                err = "Area " + data[1] + " tidak terdaftar di dalam database";
            }
            return err;
        }

        #endregion

        #region Fracas
        public List<string> LoadFracas(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp;
                    errTemp = saveFracas(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };

                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveFracas(List<object> data)
        {
            string err = "";
            string temp = data[0].ToString();
            equipment equipment = db.equipments.Where(x => x.tag_num == temp).SingleOrDefault();
            if (equipment != null)
            {
                if (data[16].Equals("equipment")) //insert equipment
                {
                    equipment_event e = new equipment_event()
                    {
                        id_equipment = equipment.id,
                        datetime_stop = DateTime.Parse(data[1].ToString()),
                        datetime_ops = DateTime.Parse(data[2].ToString()),
                        downtime = (DateTime.Parse(data[2].ToString()).Subtract(DateTime.Parse(data[1].ToString()))).TotalHours,
                        durasi = double.Parse(data[3].ToString()),
                        failure_mode = data[4].ToString(),
                        failure_cause = data[5].ToString(),
                        failure_effect = data[6].ToString(),
                        secondary_effect = data[7].ToString(),
                        failure_severity = data[8].ToString(),
                        failure_clss = data[9].ToString(),
                        immediate_act = data[10].ToString(),
                        long_term_act = data[11].ToString(),
                        repair_cost = double.Parse(data[12].ToString()),
                        financial_cost = double.Parse(data[13].ToString()),
                        engineer = data[14].ToString(),
                        event_description = data[15].ToString(),
                        status = 0
                    };

                    db.equipment_event.Add(e);
                    equipment eq = db.equipments.Find(equipment.id);
                    if (data[2].ToString() != "")
                    {
                        eq.status = 1;
                    }
                    else
                    {
                        eq.status = 0;
                    }
                    db.Entry(eq).State = EntityState.Modified;
                    db.SaveChanges();
                    CalculateEventData ca = new CalculateEventData(e.id_equipment);
                }
                else { //insert part
                    equipment_part part = db.equipment_part.Where(x => x.part.tag_number == temp).SingleOrDefault();
                    if (part != null)
                    {//part ga kosong
                        part_unit_event p = new part_unit_event()
                        {
                            id_equipment_part = part.id,
                            datetime_stop = DateTime.Parse(data[1].ToString()),
                            datetime_ops = DateTime.Parse(data[2].ToString()),
                            downtime = (DateTime.Parse(data[2].ToString()).Subtract(DateTime.Parse(data[1].ToString()))).TotalHours,
                            durasi = double.Parse(data[3].ToString()),
                            failure_mode = data[4].ToString(),
                            failure_cause = data[5].ToString(),
                            failure_effect = data[6].ToString(),
                            secondary_effect = data[7].ToString(),
                            failure_severity = data[8].ToString(),
                            failure_clss = data[9].ToString(),
                            immediate_act = data[10].ToString(),
                            long_term_act = data[11].ToString(),
                            repair_cost = double.Parse(data[12].ToString()),
                            financial_cost = double.Parse(data[13].ToString()),
                            engineer = data[14].ToString(),
                            event_description = data[15].ToString(),
                            status = 0
                        };
                        db.part_unit_event.Add(p);

                        equipment e = db.equipments.Find(part.id_equipment);
                        equipment_part eq = db.equipment_part.Find(part.id);

                        if (data[2].ToString() != "")
                        {
                            eq.status = 1;
                            //check another part if broken
                            var brokenPart = from o in db.equipment_part
                                             where o.id_equipment == e.id && o.status == 0
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
                        CalculateEventData ca = new CalculateEventData(e.id);
                    }
                    else {
                        err = "Tag Number Part" + data[0] + " tidak terdaftar di dalam database";
                    }
                }
            }
            else
            {
                err = "Tag Number Equipment" + data[0] + " tidak terdaftar di dalam database";
            }
            return err;
        }

        #endregion

        #region IR
        public List<string> LoadIR(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;
            bool add = true;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                        {
                            if (j == 1)
                                add = false;
                            temp.Add("");
                        }
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp = "";
                    if (add) errTemp = saveIR(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };
                    add = true;
                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveIR(List<object> data)
        {
            string err = "";
            byte tor = (byte)(data[4].ToString() == "On the job" ? 1 : 0);
            byte actual = (byte)(data[7].ToString() == "Major" ? 1 : data[7].ToString() == "Serious" ? 2 : data[7].ToString() == "Moderate" ? 3 : 4);
            byte potential = (byte)(data[8].ToString() == "Major" ? 1 : data[8].ToString() == "Serious" ? 2 : data[8].ToString() == "Moderate" ? 3 : 4);
            byte prob = (byte)(data[9].ToString() == "Frequent" ? 1 : data[9].ToString() == "Occasional" ? 2 : data[9].ToString() == "Seldom" ? 3 : 4);
            CultureInfo provider = CultureInfo.InvariantCulture;
            incident_report e = new incident_report()
            {
                facility = data[0].ToString(),
                incident_location = data[1].ToString(),
                reference_number = data[2].ToString(),
                incident_type = data[3].ToString(),
                type_of_report = tor,
                title = data[5].ToString(),
                date_incident = DateTime.Parse(data[6].ToString()),
                actual_loss_severity = actual,
                potential_loss_severity = potential,
                probability = prob,
                factual_information = data[10].ToString(),
                cost_estimate = data[11].ToString(),
                immediate_action = data[12].ToString(),
                prepared_by = data[13].ToString(),
                prepare_date = data[14].ToString() != "" ? DateTime.Parse(data[14].ToString()) : DateTime.Parse(data[6].ToString()),
                ack_supervisor = data[15].ToString(),
                ack_date = data[16].ToString() != "" ? DateTime.Parse(data[16].ToString()) : DateTime.Parse(data[6].ToString()),
                superintendent = data[17].ToString(),
                superintendent_date = data[18].ToString() != "" ? DateTime.Parse(data[18].ToString()) : DateTime.Parse(data[6].ToString()),
                loss_control = data[19].ToString(),
                loss_date = data[20].ToString() != "" ? DateTime.Parse(data[20].ToString()) : DateTime.Parse(data[6].ToString()),
                field_manager = data[21].ToString(),
                field_manager_date = data[22].ToString() != "" ? DateTime.Parse(data[22].ToString()) : DateTime.Parse(data[6].ToString()),
                she_superintendent = data[23].ToString(),
                she_superintendent_date = data[24].ToString() != "" ? DateTime.Parse(data[24].ToString()) : DateTime.Parse(data[6].ToString())
            };

            db.incident_report.Add(e);
            db.SaveChanges();
            return err;
        }

        #endregion

        #region Equipment Daily Report

        public List<string> LoadEquipmentDailyReport(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;
            bool add = true;
            bool isFirst = true;
            int id_report = 0;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            var r = (from e in db.equipments
                     select new EquipmentTableReportEntity
                     {
                         id_equipment = e.id,
                         tag_number = e.tag_num.Replace("\n",""),
                         description = e.nama
                     }
                    );

            List<EquipmentTableReportEntity> etre = r.ToList();

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 4; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                        {
                            if (j == 1)
                                add = false;
                            temp.Add("");
                        }
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }

                    string errTemp = "";
                    if (isFirst)
                    {
                        string s = temp.ElementAt(8).ToString();
                        s = s.Remove(0,1);
                        //string[] dateArray = s.Split('/');
                        //s = dateArray[1] + "-" + dateArray[2] + "-" + dateArray[0];
                        DateTime date = Convert.ToDateTime(s);
                        id_report = saveEquipmentReport(date);
                        isFirst = false;
                    }

                    EquipmentTableReportEntity e = etre.Find(p => p.tag_number == temp.ElementAt(1).ToString());
                    
                    int? id_equipment = e == null ? null : e.id_equipment;

                    
                    
                    if (add) errTemp = saveEquipmentTableReport(id_report,id_equipment,temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    };
                    add = true;
                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveEquipmentTableReport(int id_report,int? id_equipment, List<object> data)
        {
            string err = "";
            if (id_equipment != null)
            {
                equipment_daily_report_table eq = new equipment_daily_report_table()
                {
                    id_equipment_daily_report = id_report,
                    id_equipment = id_equipment,
                    barcode = data[2].ToString().PadLeft(8, '0'),
                    min_limit = data[4].ToString(),
                    max_limit = data[5].ToString(),
                    tag_value = data[6].ToString(),
                    unit = data[7].ToString(),
                    time = data[9].ToString().Remove(0,1),
                    name_operator = data[12].ToString(),
                    keterangan = data[13].ToString()
                };

                db.equipment_daily_report_table.Add(eq);
                db.SaveChanges();
            }
            else
            {
                err = "Equipment dengan tag number " + data[1].ToString() + " tidak ditemukan.";
            }

            return err;
        }

        private int saveEquipmentReport(DateTime? date)
        {
            equipment_daily_report eq = new equipment_daily_report()
            {
                date = date
            };

            db.equipment_daily_report.Add(eq);
            db.SaveChanges();
            int retVal = db.equipment_daily_report.Max(p =>p.id);

            return retVal;
        }

        #endregion

        #region Daily Log

        public List<string> LoadDailyLog(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<List<object>> temp;
            List<object> temp_row;
            List<string> err;
            int i, j = 0;
            bool add = true;

            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                temp = new List<List<object>>();
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 8; i <= ShtRange.Rows.Count; i++)
                {
                    temp_row = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                        {
                            temp_row.Add(null);
                        }
                        else
                            temp_row.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                    }
                    temp.Add(temp_row);
                }

                string errTemp = "";
                if (add) errTemp = saveDailyLog(temp);
                if (errTemp != "")
                {
                    err.Add(errTemp);
                };
                add = true;
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        private string saveDailyLog(List<List<object>> data)
        {
            string err = "";
            Nullable<DateTime> dat = null;
            Nullable<TimeSpan> ts = null;
            if (data[0][0] != null)
            {
                dat = DateTime.FromOADate(Double.Parse(data[0][0].ToString()));
            }
            if (data[4][0] != null)
            {
                ts = DateTime.FromOADate(Double.Parse(data[4][0].ToString())).TimeOfDay;
            }

            byte shift = 0;
            if (data.Count > 28)
            {
                shift = 1;
            }
            else
            {
                shift = 2;
            }
            List<daily_log> list_dl = db.daily_log.Where(p => p.date == dat && p.shift == shift).ToList();
            if (list_dl.Count > 0)
            {
                daily_log dl = list_dl.First();
                if (dl.is_approve == 1)
                {
                }
                else
                {
                    dl.date = dat;
                    dl.grup = data[0][3] == null ? null : data[0][3].ToString();
                    dl.production_foreman = data[0][4] == null ? null : data[0][4].ToString();
                    dl.production_operator_1 = data[0][6] == null ? null : data[0][6].ToString();
                    dl.production_operator_2 = data[1][6] == null ? null : data[1][6].ToString();
                    dl.production_operator_3 = data[0][8] == null ? null : data[0][8].ToString();
                    dl.production_operator_4 = data[1][8] == null ? null : data[1][8].ToString();
                    dl.production_operator_5 = data[0][10] == null ? null : data[0][10].ToString();
                    dl.production_operator_6 = data[1][10] == null ? null : data[1][10].ToString();
                    dl.production_operator_7 = data[0][12] == null ? null : data[0][12].ToString();
                    dl.production_operator_8 = data[1][12] == null ? null : data[1][12].ToString();
                    dl.time_check = ts;
                    dl.wma_2_is_text = (data[6][3] != null && data[6][4] != null) ? (byte)0 : (byte)1;
                    dl.wma_2_fcv = data[6][2] == null ? null : data[6][2].ToString();
                    dl.wma_2_flow = data[6][3] == null ? null : data[6][3].ToString();
                    dl.wma_2_whp = data[6][4] == null ? null : data[6][4].ToString();
                    dl.wma_4_is_text = (data[7][3] != null && data[7][4] != null) ? (byte)0 : (byte)1;
                    dl.wma_4_fcv = data[7][2] == null ? null : data[7][2].ToString();
                    dl.wma_4_flow = data[7][3] == null ? null : data[7][3].ToString();
                    dl.wma_4_whp = data[7][4] == null ? null : data[7][4].ToString();
                    dl.wma_6_is_text = (data[8][3] != null && data[8][4] != null) ? (byte)0 : (byte)1;
                    dl.wma_6_fcv = data[8][2] == null ? null : data[8][2].ToString();
                    dl.wma_6_flow = data[8][3] == null ? null : data[8][3].ToString();
                    dl.wma_6_whp = data[8][4] == null ? null : data[8][4].ToString();
                    dl.mbd_1_is_text = (data[9][3] != null && data[9][4] != null) ? (byte)0 : (byte)1;
                    dl.mbd_1_fcv = data[9][2] == null ? null : data[9][2].ToString();
                    dl.mbd_1_flow = data[9][3] == null ? null : data[9][3].ToString();
                    dl.mbd_1_whp = data[9][4] == null ? null : data[9][4].ToString();
                    dl.mbd_2_is_text = (data[10][3] != null && data[10][4] != null) ? (byte)0 : (byte)1;
                    dl.mbd_2_fcv = data[10][2] == null ? null : data[10][2].ToString();
                    dl.mbd_2_flow = data[10][3] == null ? null : data[10][3].ToString();
                    dl.mbd_2_whp = data[10][4] == null ? null : data[10][4].ToString();
                    dl.mbd_3_is_text = (data[11][3] != null && data[11][4] != null) ? (byte)0 : (byte)1;
                    dl.mbd_3_fcv = data[11][2] == null ? null : data[11][2].ToString();
                    dl.mbd_3_flow = data[11][3] == null ? null : data[11][3].ToString();
                    dl.mbd_3_whp = data[11][4] == null ? null : data[11][4].ToString();
                    dl.mbd_4_is_text = (data[12][3] != null && data[12][4] != null) ? (byte)0 : (byte)1;
                    dl.mbd_4_fcv = data[12][2] == null ? null : data[12][2].ToString();
                    dl.mbd_4_flow = data[12][3] == null ? null : data[12][3].ToString();
                    dl.mbd_4_whp = data[12][4] == null ? null : data[12][4].ToString();
                    dl.mbd_5_is_text = (data[13][3] != null && data[13][4] != null) ? (byte)0 : (byte)1;
                    dl.mbd_5_fcv = data[13][2] == null ? null : data[13][2].ToString();
                    dl.mbd_5_flow = data[13][3] == null ? null : data[13][3].ToString();
                    dl.mbd_5_whp = data[13][4] == null ? null : data[13][4].ToString();
                    dl.wwq_1_is_text = (data[14][3] != null && data[14][4] != null) ? (byte)0 : (byte)1;
                    dl.wwq_1_fcv = data[14][2] == null ? null : data[14][2].ToString();
                    dl.wwq_1_flow = data[14][3] == null ? null : data[14][3].ToString();
                    dl.wwq_1_whp = data[14][4] == null ? null : data[14][4].ToString();
                    dl.wwq_2_is_text = (data[15][3] != null && data[15][4] != null) ? (byte)0 : (byte)1;
                    dl.wwq_2_fcv = data[15][2] == null ? null : data[15][2].ToString();
                    dl.wwq_2_flow = data[15][3] == null ? null : data[15][3].ToString();
                    dl.wwq_2_whp = data[15][4] == null ? null : data[15][4].ToString();
                    dl.wwq_3_is_text = (data[16][3] != null && data[16][4] != null) ? (byte)0 : (byte)1;
                    dl.wwq_3_fcv = data[16][2] == null ? null : data[16][2].ToString();
                    dl.wwq_3_flow = data[16][3] == null ? null : data[16][3].ToString();
                    dl.wwq_3_whp = data[16][4] == null ? null : data[16][4].ToString();
                    dl.wwq_4_is_text = (data[17][3] != null && data[17][4] != null) ? (byte)0 : (byte)1;
                    dl.wwq_4_fcv = data[17][2] == null ? null : data[17][2].ToString();
                    dl.wwq_4_flow = data[17][3] == null ? null : data[17][3].ToString();
                    dl.wwq_4_whp = data[17][4] == null ? null : data[17][4].ToString();
                    dl.wwq_5_is_text = (data[18][3] != null && data[18][4] != null) ? (byte)0 : (byte)1;
                    dl.wwq_5_fcv = data[18][2] == null ? null : data[18][2].ToString();
                    dl.wwq_5_flow = data[18][3] == null ? null : data[18][3].ToString();
                    dl.wwq_5_whp = data[18][4] == null ? null : data[18][4].ToString();
                    dl.mbe_3_is_text = (data[19][3] != null && data[19][4] != null) ? (byte)0 : (byte)1;
                    dl.mbe_3_fcv = data[19][2] == null ? null : data[19][2].ToString();
                    dl.mbe_3_flow = data[19][3] == null ? null : data[19][3].ToString();
                    dl.mbe_3_whp = data[19][4] == null ? null : data[19][4].ToString();
                    dl.mbe_4_is_text = (data[20][3] != null && data[20][4] != null) ? (byte)0 : (byte)1;
                    dl.mbe_4_fcv = data[20][2] == null ? null : data[20][2].ToString();
                    dl.mbe_4_flow = data[20][3] == null ? null : data[20][3].ToString();
                    dl.mbe_4_whp = data[20][4] == null ? null : data[20][4].ToString();
                    dl.mba_1_is_text = (data[6][8] != null && data[6][9] != null) ? (byte)0 : (byte)1;
                    dl.mba_1_fcv = data[6][6] == null ? null : data[6][6].ToString();
                    dl.mba_1_flow = data[6][7] == null ? null : data[6][7].ToString();
                    dl.mba_1_whp = data[6][8] == null ? null : data[6][8].ToString();
                    dl.mba_2_is_text = (data[7][8] != null && data[7][9] != null) ? (byte)0 : (byte)1;
                    dl.mba_2_fcv = data[7][6] == null ? null : data[7][6].ToString();
                    dl.mba_2_flow = data[7][7] == null ? null : data[7][7].ToString();
                    dl.mba_2_whp = data[7][8] == null ? null : data[7][8].ToString();
                    dl.mba_3_is_text = (data[8][8] != null && data[8][9] != null) ? (byte)0 : (byte)1;
                    dl.mba_3_fcv = data[8][6] == null ? null : data[8][6].ToString();
                    dl.mba_3_flow = data[8][7] == null ? null : data[8][7].ToString();
                    dl.mba_3_whp = data[8][8] == null ? null : data[8][8].ToString();
                    dl.mba_4_is_text = (data[9][8] != null && data[9][9] != null) ? (byte)0 : (byte)1;
                    dl.mba_4_fcv = data[9][6] == null ? null : data[9][6].ToString();
                    dl.mba_4_flow = data[9][7] == null ? null : data[9][7].ToString();
                    dl.mba_4_whp = data[9][8] == null ? null : data[9][8].ToString();
                    dl.mba_5_is_text = (data[10][8] != null && data[10][9] != null) ? (byte)0 : (byte)1;
                    dl.mba_5_fcv = data[10][6] == null ? null : data[10][6].ToString();
                    dl.mba_5_flow = data[10][7] == null ? null : data[10][7].ToString();
                    dl.mba_5_whp = data[10][8] == null ? null : data[10][8].ToString();
                    dl.mbb_1_is_text = (data[11][8] != null && data[11][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_1_fcv = data[11][6] == null ? null : data[11][6].ToString();
                    dl.mbb_1_flow = data[11][7] == null ? null : data[11][7].ToString();
                    dl.mbb_1_whp = data[11][8] == null ? null : data[11][8].ToString();
                    dl.mbb_2_is_text = (data[12][8] != null && data[12][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_2_fcv = data[12][6] == null ? null : data[12][6].ToString();
                    dl.mbb_2_flow = data[12][7] == null ? null : data[12][7].ToString();
                    dl.mbb_2_whp = data[12][8] == null ? null : data[12][8].ToString();
                    dl.mbb_3_is_text = (data[13][8] != null && data[13][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_3_fcv = data[13][6] == null ? null : data[13][6].ToString();
                    dl.mbb_3_flow = data[13][7] == null ? null : data[13][7].ToString();
                    dl.mbb_3_whp = data[13][8] == null ? null : data[13][8].ToString();
                    dl.mbb_4_is_text = (data[14][8] != null && data[14][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_4_fcv = data[14][6] == null ? null : data[14][6].ToString();
                    dl.mbb_4_flow = data[14][7] == null ? null : data[14][7].ToString();
                    dl.mbb_4_whp = data[14][8] == null ? null : data[14][8].ToString();
                    dl.mbb_5_is_text = (data[15][8] != null && data[15][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_5_fcv = data[15][6] == null ? null : data[15][6].ToString();
                    dl.mbb_5_flow = data[15][7] == null ? null : data[15][7].ToString();
                    dl.mbb_5_whp = data[15][8] == null ? null : data[15][8].ToString();
                    dl.mbb_6_is_text = (data[16][8] != null && data[16][9] != null) ? (byte)0 : (byte)1;
                    dl.mbb_6_fcv = data[16][6] == null ? null : data[16][6].ToString();
                    dl.mbb_6_flow = data[16][7] == null ? null : data[16][7].ToString();
                    dl.mbb_6_whp = data[16][8] == null ? null : data[16][8].ToString();
                    dl.wwf_1_is_text = (data[17][8] != null && data[17][9] != null) ? (byte)0 : (byte)1;
                    dl.wwf_1_fcv = data[17][6] == null ? null : data[17][6].ToString();
                    dl.wwf_1_flow = data[17][7] == null ? null : data[17][7].ToString();
                    dl.wwf_1_whp = data[17][8] == null ? null : data[17][8].ToString();
                    dl.wwf_3_is_text = (data[18][8] != null && data[18][9] != null) ? (byte)0 : (byte)1;
                    dl.wwf_3_fcv = data[18][6] == null ? null : data[18][6].ToString();
                    dl.wwf_3_flow = data[18][7] == null ? null : data[18][7].ToString();
                    dl.wwf_3_whp = data[18][8] == null ? null : data[18][8].ToString();
                    dl.www_1_is_text = (data[19][8] != null && data[19][9] != null) ? (byte)0 : (byte)1;
                    dl.www_1_fcv = data[19][6] == null ? null : data[19][6].ToString();
                    dl.www_1_flow = data[19][7] == null ? null : data[19][7].ToString();
                    dl.www_1_whp = data[19][8] == null ? null : data[19][8].ToString();
                    dl.wwp_1_is_text = (data[20][8] != null && data[20][9] != null) ? (byte)0 : (byte)1;
                    dl.wwp_1_fcv = data[20][6] == null ? null : data[20][6].ToString();
                    dl.wwp_1_flow = data[20][7] == null ? null : data[20][7].ToString();
                    dl.wwp_1_whp = data[20][8] == null ? null : data[20][8].ToString();
                    dl.generator_output_1 = data[6][11] == null ? null : data[6][11].ToString();
                    dl.gross_1 = data[7][11] == null ? null : data[7][11].ToString();
                    dl.generator_output_counter_1 = data[8][11] == null ? null : data[8][11].ToString();
                    dl.power_factor_1 = data[9][11] == null ? null : data[9][11].ToString();
                    dl.tap_charger_1 = data[10][11] == null ? null : data[10][11].ToString();
                    dl.pln_grid_voltage_1 = data[11][11] == null ? null : data[11][11].ToString();
                    dl.valve_limiter_1 = data[12][11] == null ? null : data[12][11].ToString();
                    dl.governor_output_1 = data[13][11] == null ? null : data[13][11].ToString();
                    dl.wcp_counter_1 = data[14][11] == null ? null : data[14][11].ToString();
                    dl.condenser_pressure_1 = data[15][11] == null ? null : data[15][11].ToString();
                    dl.main_cw_flow_1 = data[16][11] == null ? null : data[16][11].ToString();
                    dl.ppc_g_co_1 = data[17][11] == null ? null : data[17][11].ToString();
                    dl.interface_pressure_1 = data[18][11] == null ? null : data[18][11].ToString();
                    dl.vent_bias_1 = data[19][11] == null ? null : data[19][11].ToString();
                    dl.main_cw_pressure_1 = data[20][11] == null ? null : data[20][11].ToString();
                    dl.ct_basin_ph_1 = data[21][11] == null ? null : data[21][11].ToString();
                    dl.condenser_cw_inlet_a_1 = data[22][11] == null ? null : data[22][11].ToString();
                    dl.condenser_cw_inlet_b_1 = data[23][11] == null ? null : data[23][11].ToString();
                    dl.gen_trans_winding_temp_1 = data[24][11] == null ? null : data[24][11].ToString();
                    dl.unit_trans_winding_temp_1 = data[25][11] == null ? null : data[25][11].ToString();
                    dl.wheel_case_pressure_1 = data[26][11] == null ? null : data[26][11].ToString();
                    dl.generator_output_2 = data[6][12] == null ? null : data[6][12].ToString();
                    dl.gross_2 = data[7][12] == null ? null : data[7][12].ToString();
                    dl.generator_output_counter_2 = data[8][12] == null ? null : data[8][12].ToString();
                    dl.power_factor_2 = data[9][12] == null ? null : data[9][12].ToString();
                    dl.tap_charger_2 = data[10][12] == null ? null : data[10][12].ToString();
                    dl.pln_grid_voltage_2 = data[11][12] == null ? null : data[11][12].ToString();
                    dl.valve_limiter_2 = data[12][12] == null ? null : data[12][12].ToString();
                    dl.governor_output_2 = data[13][12] == null ? null : data[13][12].ToString();
                    dl.wcp_counter_2 = data[14][12] == null ? null : data[14][12].ToString();
                    dl.condenser_pressure_2 = data[15][12] == null ? null : data[15][12].ToString();
                    dl.main_cw_flow_2 = data[16][12] == null ? null : data[16][12].ToString();
                    dl.ppc_g_co_2 = data[17][12] == null ? null : data[17][12].ToString();
                    dl.interface_pressure_2 = data[18][12] == null ? null : data[18][12].ToString();
                    dl.vent_bias_2 = data[19][12] == null ? null : data[19][12].ToString();
                    dl.main_cw_pressure_2 = data[20][12] == null ? null : data[20][12].ToString();
                    dl.ct_basin_ph_2 = data[21][12] == null ? null : data[21][12].ToString();
                    dl.condenser_cw_inlet_a_2 = data[22][12] == null ? null : data[22][12].ToString();
                    dl.condenser_cw_inlet_b_2 = data[23][12] == null ? null : data[23][12].ToString();
                    dl.gen_trans_winding_temp_2 = data[24][12] == null ? null : data[24][12].ToString();
                    dl.unit_trans_winding_temp_2 = data[25][12] == null ? null : data[25][12].ToString();
                    dl.wheel_case_pressure_2 = data[26][12] == null ? null : data[26][12].ToString();
                    dl.ncg_1 = data[23][1] == null ? null : data[23][1].ToString();
                    dl.ncg_2 = data[23][2] == null ? null : data[23][2].ToString();
                    dl.turbine_1 = data[23][3] == null ? null : data[23][3].ToString();
                    dl.turbine_2 = data[23][4] == null ? null : data[23][4].ToString();
                    dl.ct_temp_1 = data[23][5] == null ? null : data[23][5].ToString();
                    dl.ct_temp_2 = data[23][6] == null ? null : data[23][6].ToString();
                    dl.exhaust_1 = data[23][7] == null ? null : data[23][7].ToString();
                    dl.exhaust_2 = data[23][8] == null ? null : data[23][8].ToString();
                    dl.upper_tp_level = data[26][1] == null ? null : data[26][1].ToString();
                    dl.lower_tp_level = data[26][2] == null ? null : data[26][2].ToString();
                    dl.mv_333 = data[26][3] == null ? null : data[26][3].ToString();
                    dl.mv_334 = data[26][4] == null ? null : data[26][4].ToString();
                    dl.brine_level = data[26][5] == null ? null : data[26][5].ToString();
                    dl.condensate_level = data[26][6] == null ? null : data[26][6].ToString();
                    dl.naoh_level = data[26][7] == null ? null : data[26][7].ToString();
                    dl.wwd_pond_level = data[26][8] == null ? null : data[26][8].ToString();
                    if (shift == 1)
                    {
                        dl.uti_active_1 = data[29][4] == null ? null : data[29][4].ToString();
                        dl.uti_reactive_1 = data[30][4] == null ? null : data[30][4].ToString();
                        dl.sc_main_1 = data[31][4] == null ? null : data[31][4].ToString();
                        dl.sc_auxiliary_1 = data[32][4] == null ? null : data[32][4].ToString();
                        dl.ge_active_1 = data[34][4] == null ? null : data[34][4].ToString();
                        dl.ge_reactive_1 = data[35][4] == null ? null : data[35][4].ToString();
                        dl.metering_segwwl_1 = data[36][4] == null ? null : data[36][4].ToString();
                        dl.metering_pln_1 = data[37][4] == null ? null : data[37][4].ToString();
                        dl.condensate_ps_1 = data[38][4] == null ? null : data[38][4].ToString();
                        dl.segwwl_availability_1 = data[29][11] == null ? null : data[29][11].ToString();
                        dl.pln_dispatch_1 = data[30][11] == null ? null : data[30][11].ToString();
                        dl.pln_meter_1 = data[31][11] == null ? null : data[31][11].ToString();
                        dl.segwwl_export_1 = data[32][11] == null ? null : data[32][11].ToString();
                        dl.actual_export_1 = data[33][11] == null ? null : data[33][11].ToString();
                        dl.production_excess_1 = data[34][11] == null ? null : data[34][11].ToString();
                        dl.rpf_1 = data[36][11] == null ? null : data[36][11].ToString();
                        dl.pgf_1 = data[37][11] == null ? null : data[37][11].ToString();
                        dl.pln_1 = data[38][11] == null ? null : data[38][11].ToString();
                        dl.uti_active_2 = data[29][6] == null ? null : data[29][6].ToString();
                        dl.uti_reactive_2 = data[30][6] == null ? null : data[30][6].ToString();
                        dl.sc_main_2 = data[31][6] == null ? null : data[31][6].ToString();
                        dl.sc_auxiliary_2 = data[32][6] == null ? null : data[32][6].ToString();
                        dl.ge_active_2 = data[34][6] == null ? null : data[34][6].ToString();
                        dl.ge_reactive_2 = data[35][6] == null ? null : data[35][6].ToString();
                        dl.metering_segwwl_2 = data[36][6] == null ? null : data[36][6].ToString();
                        dl.metering_pln_2 = data[37][6] == null ? null : data[37][6].ToString();
                        dl.condensate_ps_2 = data[38][6] == null ? null : data[38][6].ToString();
                        dl.segwwl_availability_2 = data[29][12] == null ? null : data[29][12].ToString();
                        dl.pln_dispatch_2 = data[30][12] == null ? null : data[30][12].ToString();
                        dl.pln_meter_2 = data[31][12] == null ? null : data[31][12].ToString();
                        dl.segwwl_export_2 = data[32][12] == null ? null : data[32][12].ToString();
                        dl.actual_export_2 = data[33][12] == null ? null : data[33][12].ToString();
                        dl.production_excess_2 = data[34][12] == null ? null : data[34][12].ToString();
                        dl.rpf_2 = data[36][12] == null ? null : data[36][12].ToString();
                        dl.pgf_2 = data[37][12] == null ? null : data[37][12].ToString();
                        dl.pln_2 = data[38][12] == null ? null : data[38][12].ToString();
                        dl.condensate_total = data[39][4] == null ? null : data[39][4].ToString();
                        dl.brine_total = data[40][4] == null ? null : data[40][4].ToString();
                        dl.note = data[40][9] == null ? null : data[40][9].ToString();
                    }
                    dl.shift = shift;
                    db.Entry(dl).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                if (shift == 1)
                {
                    DateTime prev = dat.Value.AddDays(-1);
                    daily_log dlw = db.daily_log.Where(p => p.date == prev && p.shift == 2).ToList().FirstOrDefault();
                    if (dlw != null)
                    {
                        if (dlw.is_approve != 1)
                        {
                            err += "Daily log night shift must be approved first.";
                            return err;
                        }
                    }
                }
                else
                {
                    daily_log dlw = db.daily_log.Where(p => p.date == dat.Value && p.shift == 1).ToList().FirstOrDefault();
                    if (dlw != null)
                    {
                        if (dlw.is_approve != 1)
                        {
                            err += "Daily log day shift must be approved first.";
                            return err;
                        }
                    }
                }
                daily_log dl = new daily_log()
                {
                    date = dat,
                    grup = data[0][3] == null ? null : data[0][3].ToString(),
                    production_foreman = data[0][4] == null ? null : data[0][4].ToString(),
                    production_operator_1 = data[0][6] == null ? null : data[0][6].ToString(),
                    production_operator_2 = data[1][6] == null ? null : data[1][6].ToString(),
                    production_operator_3 = data[0][8] == null ? null : data[0][8].ToString(),
                    production_operator_4 = data[1][8] == null ? null : data[1][8].ToString(),
                    production_operator_5 = data[0][10] == null ? null : data[0][10].ToString(),
                    production_operator_6 = data[1][10] == null ? null : data[1][10].ToString(),
                    production_operator_7 = data[0][12] == null ? null : data[0][12].ToString(),
                    production_operator_8 = data[1][12] == null ? null : data[1][12].ToString(),
                    time_check = ts,
                    wma_2_is_text = (data[6][3] != null && data[6][4] != null) ? (byte)0 : (byte)1,
                    wma_2_fcv = data[6][2] == null ? null : data[6][2].ToString(),
                    wma_2_flow = data[6][3] == null ? null : data[6][3].ToString(),
                    wma_2_whp = data[6][4] == null ? null : data[6][4].ToString(),
                    wma_4_is_text = (data[7][3] != null && data[7][4] != null) ? (byte)0 : (byte)1,
                    wma_4_fcv = data[7][2] == null ? null : data[7][2].ToString(),
                    wma_4_flow = data[7][3] == null ? null : data[7][3].ToString(),
                    wma_4_whp = data[7][4] == null ? null : data[7][4].ToString(),
                    wma_6_is_text = (data[8][3] != null && data[8][4] != null) ? (byte)0 : (byte)1,
                    wma_6_fcv = data[8][2] == null ? null : data[8][2].ToString(),
                    wma_6_flow = data[8][3] == null ? null : data[8][3].ToString(),
                    wma_6_whp = data[8][4] == null ? null : data[8][4].ToString(),
                    mbd_1_is_text = (data[9][3] != null && data[9][4] != null) ? (byte)0 : (byte)1,
                    mbd_1_fcv = data[9][2] == null ? null : data[9][2].ToString(),
                    mbd_1_flow = data[9][3] == null ? null : data[9][3].ToString(),
                    mbd_1_whp = data[9][4] == null ? null : data[9][4].ToString(),
                    mbd_2_is_text = (data[10][3] != null && data[10][4] != null) ? (byte)0 : (byte)1,
                    mbd_2_fcv = data[10][2] == null ? null : data[10][2].ToString(),
                    mbd_2_flow = data[10][3] == null ? null : data[10][3].ToString(),
                    mbd_2_whp = data[10][4] == null ? null : data[10][4].ToString(),
                    mbd_3_is_text = (data[11][3] != null && data[11][4] != null) ? (byte)0 : (byte)1,
                    mbd_3_fcv = data[11][2] == null ? null : data[11][2].ToString(),
                    mbd_3_flow = data[11][3] == null ? null : data[11][3].ToString(),
                    mbd_3_whp = data[11][4] == null ? null : data[11][4].ToString(),
                    mbd_4_is_text = (data[12][3] != null && data[12][4] != null) ? (byte)0 : (byte)1,
                    mbd_4_fcv = data[12][2] == null ? null : data[12][2].ToString(),
                    mbd_4_flow = data[12][3] == null ? null : data[12][3].ToString(),
                    mbd_4_whp = data[12][4] == null ? null : data[12][4].ToString(),
                    mbd_5_is_text = (data[13][3] != null && data[13][4] != null) ? (byte)0 : (byte)1,
                    mbd_5_fcv = data[13][2] == null ? null : data[13][2].ToString(),
                    mbd_5_flow = data[13][3] == null ? null : data[13][3].ToString(),
                    mbd_5_whp = data[13][4] == null ? null : data[13][4].ToString(),
                    wwq_1_is_text = (data[14][3] != null && data[14][4] != null) ? (byte)0 : (byte)1,
                    wwq_1_fcv = data[14][2] == null ? null : data[14][2].ToString(),
                    wwq_1_flow = data[14][3] == null ? null : data[14][3].ToString(),
                    wwq_1_whp = data[14][4] == null ? null : data[14][4].ToString(),
                    wwq_2_is_text = (data[15][3] != null && data[15][4] != null) ? (byte)0 : (byte)1,
                    wwq_2_fcv = data[15][2] == null ? null : data[15][2].ToString(),
                    wwq_2_flow = data[15][3] == null ? null : data[15][3].ToString(),
                    wwq_2_whp = data[15][4] == null ? null : data[15][4].ToString(),
                    wwq_3_is_text = (data[16][3] != null && data[16][4] != null) ? (byte)0 : (byte)1,
                    wwq_3_fcv = data[16][2] == null ? null : data[16][2].ToString(),
                    wwq_3_flow = data[16][3] == null ? null : data[16][3].ToString(),
                    wwq_3_whp = data[16][4] == null ? null : data[16][4].ToString(),
                    wwq_4_is_text = (data[17][3] != null && data[17][4] != null) ? (byte)0 : (byte)1,
                    wwq_4_fcv = data[17][2] == null ? null : data[17][2].ToString(),
                    wwq_4_flow = data[17][3] == null ? null : data[17][3].ToString(),
                    wwq_4_whp = data[17][4] == null ? null : data[17][4].ToString(),
                    wwq_5_is_text = (data[18][3] != null && data[18][4] != null) ? (byte)0 : (byte)1,
                    wwq_5_fcv = data[18][2] == null ? null : data[18][2].ToString(),
                    wwq_5_flow = data[18][3] == null ? null : data[18][3].ToString(),
                    wwq_5_whp = data[18][4] == null ? null : data[18][4].ToString(),
                    mbe_3_is_text = (data[19][3] != null && data[19][4] != null) ? (byte)0 : (byte)1,
                    mbe_3_fcv = data[19][2] == null ? null : data[19][2].ToString(),
                    mbe_3_flow = data[19][3] == null ? null : data[19][3].ToString(),
                    mbe_3_whp = data[19][4] == null ? null : data[19][4].ToString(),
                    mbe_4_is_text = (data[20][3] != null && data[20][4] != null) ? (byte)0 : (byte)1,
                    mbe_4_fcv = data[20][2] == null ? null : data[20][2].ToString(),
                    mbe_4_flow = data[20][3] == null ? null : data[20][3].ToString(),
                    mbe_4_whp = data[20][4] == null ? null : data[20][4].ToString(),
                    mba_1_is_text = (data[6][8] != null && data[6][9] != null) ? (byte)0 : (byte)1,
                    mba_1_fcv = data[6][6] == null ? null : data[6][6].ToString(),
                    mba_1_flow = data[6][7] == null ? null : data[6][7].ToString(),
                    mba_1_whp = data[6][8] == null ? null : data[6][8].ToString(),
                    mba_2_is_text = (data[7][8] != null && data[7][9] != null) ? (byte)0 : (byte)1,
                    mba_2_fcv = data[7][6] == null ? null : data[7][6].ToString(),
                    mba_2_flow = data[7][7] == null ? null : data[7][7].ToString(),
                    mba_2_whp = data[7][8] == null ? null : data[7][8].ToString(),
                    mba_3_is_text = (data[8][8] != null && data[8][9] != null) ? (byte)0 : (byte)1,
                    mba_3_fcv = data[8][6] == null ? null : data[8][6].ToString(),
                    mba_3_flow = data[8][7] == null ? null : data[8][7].ToString(),
                    mba_3_whp = data[8][8] == null ? null : data[8][8].ToString(),
                    mba_4_is_text = (data[9][8] != null && data[9][9] != null) ? (byte)0 : (byte)1,
                    mba_4_fcv = data[9][6] == null ? null : data[9][6].ToString(),
                    mba_4_flow = data[9][7] == null ? null : data[9][7].ToString(),
                    mba_4_whp = data[9][8] == null ? null : data[9][8].ToString(),
                    mba_5_is_text = (data[10][8] != null && data[10][9] != null) ? (byte)0 : (byte)1,
                    mba_5_fcv = data[10][6] == null ? null : data[10][6].ToString(),
                    mba_5_flow = data[10][7] == null ? null : data[10][7].ToString(),
                    mba_5_whp = data[10][8] == null ? null : data[10][8].ToString(),
                    mbb_1_is_text = (data[11][8] != null && data[11][9] != null) ? (byte)0 : (byte)1,
                    mbb_1_fcv = data[11][6] == null ? null : data[11][6].ToString(),
                    mbb_1_flow = data[11][7] == null ? null : data[11][7].ToString(),
                    mbb_1_whp = data[11][8] == null ? null : data[11][8].ToString(),
                    mbb_2_is_text = (data[12][8] != null && data[12][9] != null) ? (byte)0 : (byte)1,
                    mbb_2_fcv = data[12][6] == null ? null : data[12][6].ToString(),
                    mbb_2_flow = data[12][7] == null ? null : data[12][7].ToString(),
                    mbb_2_whp = data[12][8] == null ? null : data[12][8].ToString(),
                    mbb_3_is_text = (data[13][8] != null && data[13][9] != null) ? (byte)0 : (byte)1,
                    mbb_3_fcv = data[13][6] == null ? null : data[13][6].ToString(),
                    mbb_3_flow = data[13][7] == null ? null : data[13][7].ToString(),
                    mbb_3_whp = data[13][8] == null ? null : data[13][8].ToString(),
                    mbb_4_is_text = (data[14][8] != null && data[14][9] != null) ? (byte)0 : (byte)1,
                    mbb_4_fcv = data[14][6] == null ? null : data[14][6].ToString(),
                    mbb_4_flow = data[14][7] == null ? null : data[14][7].ToString(),
                    mbb_4_whp = data[14][8] == null ? null : data[14][8].ToString(),
                    mbb_5_is_text = (data[15][8] != null && data[15][9] != null) ? (byte)0 : (byte)1,
                    mbb_5_fcv = data[15][6] == null ? null : data[15][6].ToString(),
                    mbb_5_flow = data[15][7] == null ? null : data[15][7].ToString(),
                    mbb_5_whp = data[15][8] == null ? null : data[15][8].ToString(),
                    mbb_6_is_text = (data[16][8] != null && data[16][9] != null) ? (byte)0 : (byte)1,
                    mbb_6_fcv = data[16][6] == null ? null : data[16][6].ToString(),
                    mbb_6_flow = data[16][7] == null ? null : data[16][7].ToString(),
                    mbb_6_whp = data[16][8] == null ? null : data[16][8].ToString(),
                    wwf_1_is_text = (data[17][8] != null && data[17][9] != null) ? (byte)0 : (byte)1,
                    wwf_1_fcv = data[17][6] == null ? null : data[17][6].ToString(),
                    wwf_1_flow = data[17][7] == null ? null : data[17][7].ToString(),
                    wwf_1_whp = data[17][8] == null ? null : data[17][8].ToString(),
                    wwf_3_is_text = (data[18][8] != null && data[18][9] != null) ? (byte)0 : (byte)1,
                    wwf_3_fcv = data[18][6] == null ? null : data[18][6].ToString(),
                    wwf_3_flow = data[18][7] == null ? null : data[18][7].ToString(),
                    wwf_3_whp = data[18][8] == null ? null : data[18][8].ToString(),
                    www_1_is_text = (data[19][8] != null && data[19][9] != null) ? (byte)0 : (byte)1,
                    www_1_fcv = data[19][6] == null ? null : data[19][6].ToString(),
                    www_1_flow = data[19][7] == null ? null : data[19][7].ToString(),
                    www_1_whp = data[19][8] == null ? null : data[19][8].ToString(),
                    wwp_1_is_text = (data[20][8] != null && data[20][9] != null) ? (byte)0 : (byte)1,
                    wwp_1_fcv = data[20][6] == null ? null : data[20][6].ToString(),
                    wwp_1_flow = data[20][7] == null ? null : data[20][7].ToString(),
                    wwp_1_whp = data[20][8] == null ? null : data[20][8].ToString(),
                    generator_output_1 = data[6][11] == null ? null : data[6][11].ToString(),
                    gross_1 = data[7][11] == null ? null : data[7][11].ToString(),
                    generator_output_counter_1 = data[8][11] == null ? null : data[8][11].ToString(),
                    power_factor_1 = data[9][11] == null ? null : data[9][11].ToString(),
                    tap_charger_1 = data[10][11] == null ? null : data[10][11].ToString(),
                    pln_grid_voltage_1 = data[11][11] == null ? null : data[11][11].ToString(),
                    valve_limiter_1 = data[12][11] == null ? null : data[12][11].ToString(),
                    governor_output_1 = data[13][11] == null ? null : data[13][11].ToString(),
                    wcp_counter_1 = data[14][11] == null ? null : data[14][11].ToString(),
                    condenser_pressure_1 = data[15][11] == null ? null : data[15][11].ToString(),
                    main_cw_flow_1 = data[16][11] == null ? null : data[16][11].ToString(),
                    ppc_g_co_1 = data[17][11] == null ? null : data[17][11].ToString(),
                    interface_pressure_1 = data[18][11] == null ? null : data[18][11].ToString(),
                    vent_bias_1 = data[19][11] == null ? null : data[19][11].ToString(),
                    main_cw_pressure_1 = data[20][11] == null ? null : data[20][11].ToString(),
                    ct_basin_ph_1 = data[21][11] == null ? null : data[21][11].ToString(),
                    condenser_cw_inlet_a_1 = data[22][11] == null ? null : data[22][11].ToString(),
                    condenser_cw_inlet_b_1 = data[23][11] == null ? null : data[23][11].ToString(),
                    gen_trans_winding_temp_1 = data[24][11] == null ? null : data[24][11].ToString(),
                    unit_trans_winding_temp_1 = data[25][11] == null ? null : data[25][11].ToString(),
                    wheel_case_pressure_1 = data[26][11] == null ? null : data[26][11].ToString(),
                    generator_output_2 = data[6][12] == null ? null : data[6][12].ToString(),
                    gross_2 = data[7][12] == null ? null : data[7][12].ToString(),
                    generator_output_counter_2 = data[8][12] == null ? null : data[8][12].ToString(),
                    power_factor_2 = data[9][12] == null ? null : data[9][12].ToString(),
                    tap_charger_2 = data[10][12] == null ? null : data[10][12].ToString(),
                    pln_grid_voltage_2 = data[11][12] == null ? null : data[11][12].ToString(),
                    valve_limiter_2 = data[12][12] == null ? null : data[12][12].ToString(),
                    governor_output_2 = data[13][12] == null ? null : data[13][12].ToString(),
                    wcp_counter_2 = data[14][12] == null ? null : data[14][12].ToString(),
                    condenser_pressure_2 = data[15][12] == null ? null : data[15][12].ToString(),
                    main_cw_flow_2 = data[16][12] == null ? null : data[16][12].ToString(),
                    ppc_g_co_2 = data[17][12] == null ? null : data[17][12].ToString(),
                    interface_pressure_2 = data[18][12] == null ? null : data[18][12].ToString(),
                    vent_bias_2 = data[19][12] == null ? null : data[19][12].ToString(),
                    main_cw_pressure_2 = data[20][12] == null ? null : data[20][12].ToString(),
                    ct_basin_ph_2 = data[21][12] == null ? null : data[21][12].ToString(),
                    condenser_cw_inlet_a_2 = data[22][12] == null ? null : data[22][12].ToString(),
                    condenser_cw_inlet_b_2 = data[23][12] == null ? null : data[23][12].ToString(),
                    gen_trans_winding_temp_2 = data[24][12] == null ? null : data[24][12].ToString(),
                    unit_trans_winding_temp_2 = data[25][12] == null ? null : data[25][12].ToString(),
                    wheel_case_pressure_2 = data[26][12] == null ? null : data[26][12].ToString(),
                    ncg_1 = data[23][1] == null ? null : data[23][1].ToString(),
                    ncg_2 = data[23][2] == null ? null : data[23][2].ToString(),
                    turbine_1 = data[23][3] == null ? null : data[23][3].ToString(),
                    turbine_2 = data[23][4] == null ? null : data[23][4].ToString(),
                    ct_temp_1 = data[23][5] == null ? null : data[23][5].ToString(),
                    ct_temp_2 = data[23][6] == null ? null : data[23][6].ToString(),
                    exhaust_1 = data[23][7] == null ? null : data[23][7].ToString(),
                    exhaust_2 = data[23][8] == null ? null : data[23][8].ToString(),
                    upper_tp_level = data[26][1] == null ? null : data[26][1].ToString(),
                    lower_tp_level = data[26][2] == null ? null : data[26][2].ToString(),
                    mv_333 = data[26][3] == null ? null : data[26][3].ToString(),
                    mv_334 = data[26][4] == null ? null : data[26][4].ToString(),
                    brine_level = data[26][5] == null ? null : data[26][5].ToString(),
                    condensate_level = data[26][6] == null ? null : data[26][6].ToString(),
                    naoh_level = data[26][7] == null ? null : data[26][7].ToString(),
                    wwd_pond_level = data[26][8] == null ? null : data[26][8].ToString(),
                    shift = shift
                    // To-do : add all data
                };

            if (shift == 1)
                {
                    dl.uti_active_1 = data[29][4] == null ? null : data[29][4].ToString();
                    dl.uti_reactive_1 = data[30][4] == null ? null : data[30][4].ToString();
                    dl.sc_main_1 = data[31][4] == null ? null : data[31][4].ToString();
                    dl.sc_auxiliary_1 = data[32][4] == null ? null : data[32][4].ToString();
                    dl.ge_active_1 = data[34][4] == null ? null : data[34][4].ToString();
                    dl.ge_reactive_1 = data[35][4] == null ? null : data[35][4].ToString();
                    dl.metering_segwwl_1 = data[36][4] == null ? null : data[36][4].ToString();
                    dl.metering_pln_1 = data[37][4] == null ? null : data[37][4].ToString();
                    dl.condensate_ps_1 = data[38][4] == null ? null : data[38][4].ToString();
                    dl.segwwl_availability_1 = data[29][11] == null ? null : data[29][11].ToString();
                    dl.pln_dispatch_1 = data[30][11] == null ? null : data[30][11].ToString();
                    dl.pln_meter_1 = data[31][11] == null ? null : data[31][11].ToString();
                    dl.segwwl_export_1 = data[32][11] == null ? null : data[32][11].ToString();
                    dl.actual_export_1 = data[33][11] == null ? null : data[33][11].ToString();
                    dl.production_excess_1 = data[34][11] == null ? null : data[34][11].ToString();
                    dl.rpf_1 = data[36][11] == null ? null : data[36][11].ToString();
                    dl.pgf_1 = data[37][11] == null ? null : data[37][11].ToString();
                    dl.pln_1 = data[38][11] == null ? null : data[38][11].ToString();
                    dl.uti_active_2 = data[29][6] == null ? null : data[29][6].ToString();
                    dl.uti_reactive_2 = data[30][6] == null ? null : data[30][6].ToString();
                    dl.sc_main_2 = data[31][6] == null ? null : data[31][6].ToString();
                    dl.sc_auxiliary_2 = data[32][6] == null ? null : data[32][6].ToString();
                    dl.ge_active_2 = data[34][6] == null ? null : data[34][6].ToString();
                    dl.ge_reactive_2 = data[35][6] == null ? null : data[35][6].ToString();
                    dl.metering_segwwl_2 = data[36][6] == null ? null : data[36][6].ToString();
                    dl.metering_pln_2 = data[37][6] == null ? null : data[37][6].ToString();
                    dl.condensate_ps_2 = data[38][6] == null ? null : data[38][6].ToString();
                    dl.segwwl_availability_2 = data[29][12] == null ? null : data[29][12].ToString();
                    dl.pln_dispatch_2 = data[30][12] == null ? null : data[30][12].ToString();
                    dl.pln_meter_2 = data[31][12] == null ? null : data[31][12].ToString();
                    dl.segwwl_export_2 = data[32][12] == null ? null : data[32][12].ToString();
                    dl.actual_export_2 = data[33][12] == null ? null : data[33][12].ToString();
                    dl.production_excess_2 = data[34][12] == null ? null : data[34][12].ToString();
                    dl.rpf_2 = data[36][12] == null ? null : data[36][12].ToString();
                    dl.pgf_2 = data[37][12] == null ? null : data[37][12].ToString();
                    dl.pln_2 = data[38][12] == null ? null : data[38][12].ToString();
                    dl.condensate_total = data[39][4] == null ? null : data[39][4].ToString();
                    dl.brine_total = data[40][4] == null ? null : data[40][4].ToString();
                    dl.note = data[40][9] == null ? null : data[40][9].ToString();
                }

                db.daily_log.Add(dl);
                db.SaveChanges();

                int id = db.daily_log.Max(p => p.id);
                string subPath = "~/Attachment/daily_log/" + id; // your code goes here
                bool IsExists = System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(subPath));
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(subPath));
                if (data[36][11] == null && data[36][11] == null && data[36][12] == null && data[37][12] == null)
                {
                    daily_log shift1 = db.daily_log.Where(p => p.date == dat).FirstOrDefault();
                    shift1.id_shift2 = id;
                    db.Entry(shift1).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return err;
        }

        #endregion

        #region PIR

        public List<string> LoadPIR(string filename)
        {
            Excel.Application app;
            Excel.Workbook book;
            Excel.Range ShtRange;
            List<object> temp;
            List<string> err;
            int i, j = 0;
            int[] datecolumn = new int[5]{3,5,17,18,19}; //columns with date as its value
            DateTime? dateconvert = new DateTime?();
            app = new Excel.Application();
            book = app.Workbooks.Open(Filename: filename);

            err = new List<string>();
            foreach (Excel.Worksheet sheet in book.Sheets)
            {
                ShtRange = sheet.UsedRange;
                string a = sheet.Name;
                for (i = 2; i <= ShtRange.Rows.Count; i++)
                {
                    temp = new List<object>();
                    for (j = 1; j <= ShtRange.Columns.Count; j++)
                    {
                        var az = ShtRange.Cells[i, j] as Excel.Range;
                        if ((ShtRange.Cells[i, j] as Excel.Range).Value2 == null)
                            temp.Add("");
                        else if (datecolumn.Contains(j))
                        {
                            dateconvert = DateTime.FromOADate(Convert.ToDouble((ShtRange.Cells[i, j] as Excel.Range).Value2));
                            temp.Add(dateconvert);
                        }
                        else
                            temp.Add((ShtRange.Cells[i, j] as Excel.Range).Value2.ToString());
                        //3,5,17,18,19
                    }

                    string errTemp;
                    errTemp = savePIR(temp);
                    if (errTemp != "")
                    {
                        err.Add(errTemp);
                    }

                }
            }
            book.Close(true, Missing.Value, Missing.Value);
            app.Quit();

            return err;
        }

        public string savePIR(List<object> data)
        {
            string err = "";
            int? user_id = null;
            string kbp = data[14].ToString();
            employee kbpe = db.employees.Where(p => p.alpha_name == kbp).FirstOrDefault();
            user_id = kbpe != null ? kbpe.id : (Int32?)null;
            pir pir = new pir()
                {
                    no = null,
                    improvement_request = data[13].ToString(),
                    date_rise = String.IsNullOrEmpty(data[4].ToString()) ? (DateTime?)null : DateTime.Parse(data[4].ToString()),
                    initiate_by = data[7].ToString(),
                    title = null,
                    reference = data[8].ToString(),
                    procedure_reference = data[10].ToString(),
                    initiator_sign_date = String.IsNullOrEmpty(data[2].ToString()) ? (DateTime?)null : DateTime.Parse(data[2].ToString()),
                    kpbo_sign_date_init = null,
                    target_completion_init = null,
                    desc_prob = null,
                    investigator = null,
                    investigator_date = null,
                    improvement_plant = data[5].ToString(),
                    start_implement_date = String.IsNullOrEmpty(data[16].ToString()) ? (DateTime?)null : DateTime.Parse(data[16].ToString()),
                    process_owner = null,
                    target_completion_process = String.IsNullOrEmpty(data[17].ToString()) ? (DateTime?)null : DateTime.Parse(data[17].ToString()),
                    action_by = null,
                    require_dokument = null,
                    hira_require = null,
                    kpbo_sign_date_process = null,
                    review_date = null,
                    result_of_action = null,
                    kpbo_sign_date_process_result = null,
                    sign_date_verified = null,
                    verified_desc = null,
                    initiator_verified_date = String.IsNullOrEmpty(data[18].ToString()) ? (DateTime?)null : DateTime.Parse(data[18].ToString()),
                    review_mgmt_verified_date = null,
                    description = null,
                    status = data[1].ToString(),
                    process_user = user_id,
                    from = 1,
                };
            db.pirs.Add(pir);
            db.SaveChanges();

            string subPath = "D:\\Informatics\\Proyek\\Star Energy\\fracas\\StarEnergi\\Attachment\\pir\\" + pir.id; // your code goes here
            bool IsExists = System.IO.Directory.Exists(subPath);
            if (!IsExists)
                System.IO.Directory.CreateDirectory(subPath);
            return err;
        }
        #endregion
		
        public string generateError(List<string> err) {
            string html = "";
            if(err.Count > 0){
                html += "<b>LOG</b>";
                html += "<br/>";
                html += "<ul>";
                foreach (string x in err) {
                    html += "<li>";
                    html += x;
                    html += "</li>";
                }
                html += "</ul>";
            }
            return html;
        }
    }

}