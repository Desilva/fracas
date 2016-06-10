using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models.ExportExcel
{
    public class AssetRegisterExport
    {
        public string tag_number { get; set; }
        public string unit_name { get; set; }
        public string system_name { get; set; }  
        public string area_name { get; set; }
        public string discipline { get; set; }
        public double? mtbf { get; set; }
        public double? mttr { get; set; }
    }
}