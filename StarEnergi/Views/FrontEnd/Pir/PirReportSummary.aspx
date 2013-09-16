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
        PirReport.Report = new StarEnergi.Reporting.PIR_Report_Summary();
    }

</script>
<html>
    <head id="Head1" runat="server">
    </head>
    <body>
        <form clientidmode="Static" id="frep" runat="server"><div>
            <telerik:ReportViewer ID="PirReport" runat="server" 
                Width="1000px"
                Height="718px" ViewMode="PrintPreview" 
                ZoomMode="FullPage"/>
                </div>
        </form>
    </body>
</html>