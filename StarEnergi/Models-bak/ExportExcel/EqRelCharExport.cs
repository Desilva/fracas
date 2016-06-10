using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace StarEnergi.Models.ExportExcel
{
    public class EqRelCharExport
    {
        public string tag_num { get; set; }
        public string pdf { get; set; }
        public Nullable<double> tetha { get; set; }
        public Nullable<double> beta { get; set; }
        public Nullable<double> lamda { get; set; }
        public Nullable<double> mean { get; set; }
        public Nullable<double> varian { get; set; }
        public Nullable<int> mtbf { get; set; }
    }
}