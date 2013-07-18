using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StarEnergi.Models;

namespace StarEnergi.Models
{
    public class EquipmentEntity
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        public equipment equipment { get; set; }
        public IEnumerable<EquipmentPartEntity> parts { get; set; }
        public IEnumerable<part> partsNotInEquipment { get; set; }

        public EquipmentEntity(){}

        public EquipmentEntity(equipment e) {
            var ep = from x in db.equipment_part
                     where x.id_equipment == e.id
                     select new EquipmentPartEntity
                     {
                         id = x.id,
                         id_equipment = x.id_equipment,
                         id_parts = x.id_parts,
                         mtbf = x.mtbf,
                         mttr = x.mttr,
                         status = x.status,
                         lead_time = x.lead_time,
                         installed_date = x.installed_date,
                         obsolete_date = x.obsolete_date,
                         tag_num = x.part.tag_number,
                         nama = x.part.nama,
                         vendor = x.part.vendor,
                         warranty = x.part.warranty
                     };
            equipment = e;
            parts = ep.ToList();
        }

        //set parts from id equipment
        public EquipmentEntity(int id) {
            equipment = db.equipments.Find(id);

            var ep = from x in db.equipment_part
                     where x.id_equipment == id
                     select new EquipmentPartEntity
                     {
                         id = x.id,
                         id_equipment = x.id_equipment,
                         id_parts = x.id_parts,
                         mtbf = x.mtbf,
                         mttr = x.mttr,
                         status = x.status,
                         lead_time = x.lead_time,
                         installed_date = x.installed_date,
                         obsolete_date = x.obsolete_date,
                         tag_num = x.part.tag_number,
                         nama = x.part.nama,
                         vendor = x.part.vendor,
                         warranty = x.part.warranty
                     };
            parts = ep.ToList();
            
        }

        public void SetPartNotInEquipment(){
            List<int> idParts = new List<int>();
            foreach(EquipmentPartEntity x in parts){
                idParts.Add(x.id_parts);  
            }
            partsNotInEquipment = db.parts.Where(x => !idParts.Contains(x.id));
        }
        
    }
}