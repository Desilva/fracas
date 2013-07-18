using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class MaintenanceCost
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;

        public MaintenanceCost(string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }

    }

    public class MaintenanceCostPoint
    {
        public static readonly MaintenanceCost[] Points = new MaintenanceCost[]
        {
            new MaintenanceCost("12.000","9.500","Jan"),
            new MaintenanceCost("13.000","11.000","Feb"),
            new MaintenanceCost("12.500","10.500","Mar"),
            new MaintenanceCost("13.500","12.000","April"),
            new MaintenanceCost("14.000","12.500","Mei"),
        };
    }
}
