namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Web;

    /// <summary>
    /// Summary description for EachCertificate.
    /// </summary>
    public partial class EachCertificate : Telerik.Reporting.Report
    {
        public EachCertificate()
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
            return Image.FromFile(path);
        }
    }
}