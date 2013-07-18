namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data.SqlClient;
    using System.Data;
    using Telerik.Reporting.Charting;
    using System.Threading;

    /// <summary>
    /// Summary description for ObservationSummary.
    /// </summary>
    public partial class ObservationSummaryOther : Telerik.Reporting.Report
    {
        public ObservationSummaryOther()
        {
            //
            // Required for telerik Reporting designer support
            //
            Thread t = new Thread(Run, 4194304); // 4M of stack size
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