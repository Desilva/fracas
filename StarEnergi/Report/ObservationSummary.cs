namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data.SqlClient;
    using System.Data;
    using Telerik.Reporting.Charting;
using System.Threading;

    /// <summary>
    /// Summary description for ObservationSummary.
    /// </summary>
    public partial class ObservationSummary : Telerik.Reporting.Report
    {
        public ObservationSummary()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread t = new Thread(Run, 419430400); // 4M of stack size
            t.Start();
            t.Join();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        void Run()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
            }
        }
    }
}