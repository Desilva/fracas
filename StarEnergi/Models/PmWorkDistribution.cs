using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;

namespace StarEnergi.Models
{
    public class PmWorkDistribution
    {
        public string CurrentYear;
        public string LastYear;
        public string Month;

        public PmWorkDistribution (string currentYear, string lastYear, string month)
        {
            CurrentYear = currentYear;
            LastYear = lastYear;
            Month = month;
        }
    }

    public class PmWorkDistributionPoint
    {
        public static readonly PmWorkDistribution [] Points = new PmWorkDistribution[]
        {
            new PmWorkDistribution("70.50","68.00","Jan"),
            new PmWorkDistribution("67.90","72.00","Feb"),
            new PmWorkDistribution("86.00","80.00","Mar"),
            new PmWorkDistribution("74.00","75.00","April"),
            new PmWorkDistribution("78.00","70.00","Mei"),
        };
    }
}
