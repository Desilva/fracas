using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class SysAvailiability
    {
        
        public SysAvailiability(string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
        public string CurrentYear;
        public string LastYear;
        public string Month;
    }

    public class SysAvailiabilityPoint
    {
        public static readonly SysAvailiability[] Points = new SysAvailiability[]
        {
            new SysAvailiability("99.00","89.00","Jan"),
            new SysAvailiability("98.00","97.00","Feb"),
            new SysAvailiability("86.00","90.00","Mar"),
            new SysAvailiability("85.00","99.00","Apr"),
            new SysAvailiability("95.00","87.00","Mei")
        };
    }
}
