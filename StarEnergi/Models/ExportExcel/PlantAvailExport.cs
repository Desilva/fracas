using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models.ExportExcel
{
    public class PlantAvailExport
    {
        public string tag_num { get; set; }
        public string nama { get; set; }
        public string equipment_group { get; set; }
        public double avail_inherent { get; set; }
        public double avail_operational { get; set; }
        
    }
}