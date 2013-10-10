namespace StarEnergi.Report
{
    partial class Ceritificates
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.eachCertificate1 = new StarEnergi.Report.EachCertificate();
            ((System.ComponentModel.ISupportInitialize)(this.eachCertificate1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "starenergygeo";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.SelectCommand = "select id from she_observation_undian";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(8.25D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.detail.Name = "detail";
            this.detail.PageBreak = Telerik.Reporting.PageBreak.After;
            // 
            // subReport1
            // 
            this.subReport1.KeepTogether = false;
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.subReport1.Name = "subReport1";
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("id", "=Parameters.id.Value"));
            this.subReport1.ReportSource = this.eachCertificate1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.670001029968262D), Telerik.Reporting.Drawing.Unit.Inch(8.25D));
            // 
            // eachCertificate1
            // 
            this.eachCertificate1.Name = "EachCertificate";
            // 
            // Ceritificates
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "Ceritificates";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AvailableValues.DataSource = this.sqlDataSource1;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.id";
            reportParameter1.AvailableValues.ValueMember = "= Fields.id";
            reportParameter1.Name = "id";
            reportParameter1.Text = "id";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.670000076293945D);
            ((System.ComponentModel.ISupportInitialize)(this.eachCertificate1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.SubReport subReport1;
        private EachCertificate eachCertificate1;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
    }
}