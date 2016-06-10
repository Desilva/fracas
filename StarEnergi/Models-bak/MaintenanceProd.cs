using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class MaintenanceProd
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;
        
        public MaintenanceProd (string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }
}
