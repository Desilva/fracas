using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;

namespace StarEnergi.Models
{
    public class Overtime
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;
        public Overtime (string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }

    public class OvertimePoint
    {
        public static readonly Overtime [] Points = new Overtime[]
        {
            new Overtime("3.50","4.20","Jan"),
            new Overtime("5.00","5.30","Feb"),
            new Overtime("6.00","5.60","Mar"),
            new Overtime("6.00","7.20","April"),
            new Overtime("4.00","8.50","Mei"),
        };
    }
}
