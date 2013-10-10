<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=6.0.12.215, Culture=neutral, PublicKeyToken=a9d7983dfcc261be"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<script runat="server">      
    public override void VerifyRenderingInServerForm(Control control)
    {
        // to avoid the server form (<form runat="server">) requirement
    }

    protected override void OnLoad(EventArgs e)
    {
        // bind the report viewer
        base.OnLoad(e);
        Telerik.Reporting.Report rpt = new StarEnergi.Report.EachCertificate();
        rpt.ReportParameters["id"].Value = ViewBag.id;
        Certificates.Report = rpt;
    }

</script>
<html>
    <head id="Head1" runat="server">
    </head>
    <body>
        <form clientidmode="Static" id="frep" runat="server"><div>
            <telerik:ReportViewer ID="Certificates" runat="server" 
                Width="1000px"
                Height="718px" ViewMode="PrintPreview" 
                ZoomMode="FullPage"/>
                </div>
        </form>
    </body>
</html>
<script type="text/javascript">
    $(document).ready(function () {
        //$("#ReportViewer1ReportFrame").load(function () {
        var int = self.setInterval(function () { clock() }, 200);

        var clock = function () {
            var doc = document.getElementById("CertificatesReportFrame").contentWindow.document;
            //alert(doc.body.getElementsByClassName("panel3")[0]);
            if (doc.body.getElementsByClassName("panel1")[0] != null) {
                doc.body.getElementsByClassName("panel1")[0].style.backgroundSize = "100%";
            }
        }



        //});

    });
</script>