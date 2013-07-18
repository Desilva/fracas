using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models.ExportExcel
{
    public class EquipmentReadNavExport
    {
        public string equipment_tag_number { get; set; }
        public Nullable<double> operation { get; set; }
        public Nullable<double> monitoring { get; set; }
        public Nullable<double> maintenance { get; set; }
        public Nullable<double> spares { get; set; }
        public Nullable<double> safe_operation { get; set; }       
    }
}