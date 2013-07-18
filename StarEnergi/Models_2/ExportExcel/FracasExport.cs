using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models.ExportExcel
{
    public class FracasExport
    {
        public DateTime? dateTimeStop { get; set; }
        public string eventDesc { get; set; }
        public string cause { get; set; }
        public string relatedItem { get; set; }
    }
}