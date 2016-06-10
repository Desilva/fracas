using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class History
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;

        public History(string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }

    public class HistoryPoint
    {
        public static readonly History[] Points = new History[]
        {
            new History("12.40","13.40","Jan"),
            new History("14.20","12.50","Feb"),
            new History("11.50","12.63","Mar"),
            new History("15.30","14.00","April"),
            new History("20.50","18.40","Mei"),
        };
    }
}
