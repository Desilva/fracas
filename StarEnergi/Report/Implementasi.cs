namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Web;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using Telerik.Reporting.Expressions;

    /// <summary>
    /// Summary description for Implementasi.
    /// </summary>
    public partial class Implementasi : Telerik.Reporting.Report
    {
        public Implementasi()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public static Image ResolveUrl(string relativeUrl)
        {

            string path = HttpContext.Current.Server.MapPath(relativeUrl);
            Debug.WriteLine(path);
            return Image.FromFile(path);
        }

        public static int DateComparer(DateTime From, DateTime To)
        {
            return From.CompareTo(To);
        }
    }
}