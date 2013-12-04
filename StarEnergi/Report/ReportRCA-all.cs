using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace StarEnergi.Report
{
    [Description("A collection of Product-related reports")]
    public class ReportRCA_all : Telerik.Reporting.ReportBook
    {
        public ReportRCA_all()
        {
            this.Reports.Add(new ReportRCA());
            this.Reports.Add(new ReportRCA_b());
        }
    }
}