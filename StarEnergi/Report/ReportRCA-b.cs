namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Web;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Summary description for ReportRCA_b.
    /// </summary>
    public partial class ReportRCA_b : Telerik.Reporting.Report
    {
        public ReportRCA_b()
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
            Image im = null;
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