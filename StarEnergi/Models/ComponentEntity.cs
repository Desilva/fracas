using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class ComponentEntity
    {
        public int id_component { get; set; }
        public Nullable<int> id_equipment_part { get; set; }
        public string tag_number { get; set; }
        public string description { get; set; }
    }
}