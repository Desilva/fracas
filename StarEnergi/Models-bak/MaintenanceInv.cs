using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class MaintenanceInv
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;

        public MaintenanceInv(string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }
}
