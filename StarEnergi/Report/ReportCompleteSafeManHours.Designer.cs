namespace StarEnergi.Report
{
    partial class ReportCompleteSafeManHours
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            this.Contractors = new Telerik.Reporting.SqlDataSource();
            this.NoContracts = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.reportSafeManHours1 = new StarEnergi.Report.ReportSafeManHours();
            ((System.ComponentModel.ISupportInitialize)(this.reportSafeManHours1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Contractors
            // 
            this.Contractors.ConnectionString = "starenergygeo";
            this.Contractors.Name = "Contractors";
            this.Contractors.SelectCommand = "select * from monthly_she_contractor";
            // 
            // NoContracts
            // 
            this.NoContracts.ConnectionString = "starenergygeo";
            this.NoContracts.Name = "NoContracts";
            this.NoContracts.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@contractor_id", System.Data.DbType.String, "=Parameters.contractor.Value")});
            this.NoContracts.SelectCommand = "select distinct no_contract from monthly_project_she_report where contractor_id =" +
    " @contractor_id";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(10.6899995803833D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.detail.Name = "detail";
            this.detail.PageBreak = Telerik.Reporting.PageBreak.After;
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.subReport1.Name = "subReport1";
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("contractor", "=Parameters.contractor.Value"));
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("no_contract", "=Parameters.no_contract.Value"));
            this.subReport1.ReportSource = this.reportSafeManHours1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2599215507507324D), Telerik.Reporting.Drawing.Unit.Inch(10.689921379089356D));
            // 
            // reportSafeManHours1
            // 
            this.reportSafeManHours1.Name = "ReportSafeManHours";
            // 
            // ReportCompleteSafeManHours
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "ReportCompleteSafeManHours";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.Contractors;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.id";
            reportParameter1.Name = "contractor";
            reportParameter1.Text = "Contractor";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.NoContracts;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.no_contract";
            reportParameter2.AvailableValues.ValueMember = "= Fields.no_contract";
            reportParameter2.MultiValue = true;
            reportParameter2.Name = "no_contract";
            reportParameter2.Text = "No. Contract(s)";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.2600002288818359D);
            ((System.ComponentModel.ISupportInitialize)(this.reportSafeManHours1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.SubReport subReport1;
        private ReportSafeManHours reportSafeManHours1;
        private Telerik.Reporting.SqlDataSource Contractors;
        private Telerik.Reporting.SqlDataSource NoContracts;
    }
}