using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class ScheduleNonConformance
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;
        
        public ScheduleNonConformance (string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }

    public class ScheduleNonConformancePoint
    {
        public static readonly ScheduleNonConformance [] Points = new ScheduleNonConformance[]
        {
            new ScheduleNonConformance("3.00","3.50","Jan"),
            new ScheduleNonConformance("4.00","5.00","Feb"),
            new ScheduleNonConformance("5.00","6.00","Mar"),
            new ScheduleNonConformance("6.00","7.00","April"),
            new ScheduleNonConformance("4.00","3.00","Mei"),
        };
    }
}
