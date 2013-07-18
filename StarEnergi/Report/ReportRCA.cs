namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Web;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using Telerik.Reporting.Expressions;

    /// <summary>
    /// Summary description for ReportRCA.
    /// </summary>
    public partial class ReportRCA : Telerik.Reporting.Report
    {
        public ReportRCA()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            // create connection, command, adapter
        }

        public static Image ResolveUrl(string relativeUrl)
        {
            string path = HttpContext.Current.Server.MapPath(relativeUrl);
            Debug.WriteLine(relativeUrl);
            Image im = null;
            Image im2 = null;
            try
            {
                var bytes = File.ReadAllBytes(path);
                var ms = new MemoryStream(bytes);
                im = Image.FromStream(ms);
                ms.Close();
            }
            catch (System.IO.FileNotFoundException e)
            {
                path = HttpContext.Current.Server.MapPath("~/Content/image/blank.png");
                var bytes = File.ReadAllBytes(path);
                var ms = new MemoryStream(bytes);
                im = Image.FromStream(ms);
                ms.Close();
            }
            return im;
        }
    }
}