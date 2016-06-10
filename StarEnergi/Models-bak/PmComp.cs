using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarEnergi.Models
{
    public class PmComp
    {
        
        public PmComp(string currentYear,string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
        public string CurrentYear;
        public string LastYear;
        public string Month;

    }

    public static class PmCompPoint
    {
        public static readonly PmComp[] Points = new PmComp[]
        {  
            new PmComp("99.00","99.00","Jan"),
            new PmComp("98.00","97.00","Feb"),
            new PmComp("92.00","90.00","Mar"),
            new PmComp("93.00","99.00","Apr"),
            new PmComp("95.00","97.00","Mei")
        };
    }
}