<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register assembly="Telerik.ReportViewer.WebForms, Version=6.0.12.215, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.ReportViewer.WebForms" tagprefix="telerik" %>

<script runat="server">
    public override void VerifyRenderingInServerForm(Control control)
    {
        // to avoid the server form (<form runat="server">) requirement
    }
 
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
             
        // bind the report viewer
        Telerik.Reporting.Report rpt = new StarEnergi.Report.SheObservationSeverity();
        ReportViewer1.Report = rpt;
    }
</script>
<form id="form1" runat="server" style="width:100%">
    <telerik:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="1115px">
    </telerik:ReportViewer>
</form>
