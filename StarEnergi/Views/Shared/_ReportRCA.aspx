<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register assembly="Telerik.ReportViewer.WebForms, Version=6.0.12.215, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.ReportViewer.WebForms" tagprefix="telerik" %>
<%@ Register assembly="Telerik.Reporting, Version=6.0.12.215, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.Reporting" tagprefix="telerik" %>
<script runat="server">
    public override void VerifyRenderingInServerForm(Control control)
    {
        // to avoid the server form (<form runat="server">) requirement
    }
 
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        //InstanceReportSource reportSource = new InstanceReportSource();
        //reportSource.ReportDocument = new StarEnergi.Report.ReportRCA_all();
        //ReportViewer1.S = reportSource;
        // bind the report viewer
        Telerik.Reporting.ReportBook rpt = new ReportBook();
        rpt.Reports.Add(new StarEnergi.Report.ReportRCA());
        rpt.Reports.Add(new StarEnergi.Report.ReportRCA_b());
        rpt.Reports.Add(new StarEnergi.Report.ReportRCA_c());
        rpt.Reports.ElementAt(0).ReportParameters["id"].Value = ViewBag.ids;
        rpt.Reports.ElementAt(1).ReportParameters["id"].Value = ViewBag.ids;
        rpt.Reports.ElementAt(2).ReportParameters["id"].Value = ViewBag.ids;
        ReportViewer1.Report = rpt;
    }
</script>
<form id="form1" runat="server" style="width:100%">
    <telerik:ReportViewer ID="ReportViewer1" runat="server" Width="1000px" Height="750px">
    </telerik:ReportViewer>
</form>
<script type="text/javascript">
    $(window).load(function () {
        //$("#ReportViewer1ReportFrame").load(function () {
            var int = self.setInterval(function () { clock() }, 200);
            
            var clock = function () {
                var doc = document.getElementById("ReportViewer1ReportFrame").contentWindow.document;
                //alert(doc.body.getElementsByClassName("panel3")[0]);
                if (doc.body.getElementsByClassName("panel3")[0] != null) {
                    doc.body.getElementsByClassName("panel3")[0].style.backgroundSize = "100%";
                    doc.body.getElementsByClassName("panel11")[0].style.backgroundSize = "100%";
                }
                if (doc.body.getElementsByClassName("panel7")[0] != null) {
                    doc.body.getElementsByClassName("panel7")[0].style.backgroundSize = "100%";
                    doc.body.getElementsByClassName("panel23")[0].style.backgroundSize = "100%";
                }
                if (doc.body.getElementsByClassName("panel8")[0] != null) {
                    doc.body.getElementsByClassName("panel8")[0].style.backgroundSize = "100%";
                    doc.body.getElementsByClassName("panel24")[0].style.backgroundSize = "100%";
                }
                if (doc.body.getElementsByClassName("panel9")[0] != null) {
                    doc.body.getElementsByClassName("panel9")[0].style.backgroundSize = "100%";
                    doc.body.getElementsByClassName("panel17")[0].style.backgroundSize = "100%";
                }
            }
            

            
        //});
        
    });
</script>