namespace StarEnergi.Report
{
    partial class ReportSheObservationDate
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem1 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem2 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem3 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem4 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem5 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem6 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem7 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem8 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem9 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem10 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem11 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem12 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem13 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem14 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem15 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem16 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem17 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem18 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem19 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem20 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem21 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem22 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem23 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem24 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem25 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem26 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem27 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem28 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem29 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.Charting.ChartAxisItem chartAxisItem30 = new Telerik.Reporting.Charting.ChartAxisItem();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.sqlDataSource2 = new Telerik.Reporting.SqlDataSource();
            this.sqlDataSource3 = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.panel1 = new Telerik.Reporting.Panel();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.observationSummaryDate1 = new StarEnergi.Report.ObservationSummaryDate();
            this.chart1 = new Telerik.Reporting.Chart();
            this.chart2 = new Telerik.Reporting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryDate1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "starenergygeo";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.SelectCommand = "SELECT DISTINCT YEAR(date_time) AS year\r\nFROM            she_observation";
            // 
            // sqlDataSource2
            // 
            this.sqlDataSource2.ConnectionString = "starenergygeo";
            this.sqlDataSource2.Name = "sqlDataSource2";
            this.sqlDataSource2.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@year", System.Data.DbType.String, "=Parameters.year.Value")});
            this.sqlDataSource2.SelectCommand = "SELECT DISTINCT MONTH(date_time) AS month\r\nFROM            she_observation\r\nWHERE" +
    " YEAR(date_time) = @year";
            // 
            // sqlDataSource3
            // 
            this.sqlDataSource3.ConnectionString = "starenergygeo";
            this.sqlDataSource3.Name = "sqlDataSource3";
            this.sqlDataSource3.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@year", System.Data.DbType.String, "=Parameters.year.Value"),
            new Telerik.Reporting.SqlDataSourceParameter("@month", System.Data.DbType.String, "=Parameters.month.Value")});
            this.sqlDataSource3.SelectCommand = "SELECT        DAY(date_time) AS day\r\nFROM            she_observation\r\nWHERE YEAR(" +
    "date_time) = @year AND MONTH(date_time) = @month\r\nORDER BY day";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(10.699999809265137D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pictureBox1,
            this.panel1,
            this.subReport1,
            this.chart1,
            this.chart2});
            this.detail.Name = "detail";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(1.2300000190734863D));
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(13.699999809265137D), Telerik.Reporting.Drawing.Unit.Inch(1.2300000190734863D));
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(13.699959754943848D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(34D);
            this.textBox1.Value = "STAR ENERGY GEOTHERMAL (WAYANG WINDU) LIMITED";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.subReport1.Name = "subReport1";
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("year", "=Parameters.year.Value"));
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("month", "=Parameters.month.Value"));
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("day", "=Parameters.day.Value"));
            this.subReport1.ReportSource = this.observationSummaryDate1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(4.9499998092651367D));
            // 
            // observationSummaryDate1
            // 
            this.observationSummaryDate1.Name = "ObservationSummary";
            // 
            // chart1
            // 
            this.chart1.BitmapResolution = 96F;
            this.chart1.ChartTitle.TextBlock.Appearance.Position.AlignedPosition = Telerik.Reporting.Charting.Styles.AlignedPositions.Top;
            this.chart1.ChartTitle.TextBlock.Appearance.Position.Auto = false;
            this.chart1.ChartTitle.TextBlock.Appearance.Position.X = 6F;
            this.chart1.ChartTitle.TextBlock.Appearance.Position.Y = 4F;
            this.chart1.ChartTitle.TextBlock.Text = "GRAFIK DATA";
            this.chart1.ImageFormat = System.Drawing.Imaging.ImageFormat.Emf;
            this.chart1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(6.5D));
            this.chart1.Name = "chart1";
            this.chart1.PlotArea.Appearance.Border.Color = System.Drawing.Color.White;
            this.chart1.PlotArea.Appearance.Dimensions.AutoSize = false;
            this.chart1.PlotArea.Appearance.Dimensions.Height = new Telerik.Reporting.Charting.Styles.Unit(200D, Telerik.Reporting.Charting.Styles.UnitType.Pixel);
            this.chart1.PlotArea.Appearance.Dimensions.Width = new Telerik.Reporting.Charting.Styles.Unit(720D, Telerik.Reporting.Charting.Styles.UnitType.Pixel);
            this.chart1.PlotArea.Appearance.FillStyle.FillType = Telerik.Reporting.Charting.Styles.FillType.Solid;
            this.chart1.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.Color.White;
            this.chart1.PlotArea.EmptySeriesMessage.Appearance.Visible = true;
            this.chart1.PlotArea.EmptySeriesMessage.Visible = true;
            this.chart1.PlotArea.XAxis.Appearance.CustomFormat = "String";
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Dimensions.AutoSize = false;
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Dimensions.Height = new Telerik.Reporting.Charting.Styles.Unit(15D, Telerik.Reporting.Charting.Styles.UnitType.Pixel);
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Dimensions.Width = new Telerik.Reporting.Charting.Styles.Unit(250D, Telerik.Reporting.Charting.Styles.UnitType.Pixel);
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Position.AlignedPosition = Telerik.Reporting.Charting.Styles.AlignedPositions.Top;
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Position.Auto = false;
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Position.X = 0F;
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.Position.Y = 0F;
            this.chart1.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = 90F;
            this.chart1.PlotArea.XAxis.Appearance.ValueFormat = Telerik.Reporting.Charting.Styles.ChartValueFormat.General;
            this.chart1.PlotArea.XAxis.AutoScale = false;
            chartAxisItem2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            chartAxisItem3.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            chartAxisItem4.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            chartAxisItem5.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            chartAxisItem6.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            chartAxisItem7.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            chartAxisItem8.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            chartAxisItem9.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            chartAxisItem10.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            chartAxisItem11.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            chartAxisItem12.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            chartAxisItem13.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            chartAxisItem14.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            chartAxisItem15.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            chartAxisItem16.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            chartAxisItem17.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            chartAxisItem18.Value = new decimal(new int[] {
            17,
            0,
            0,
            0});
            chartAxisItem19.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            chartAxisItem20.Value = new decimal(new int[] {
            19,
            0,
            0,
            0});
            chartAxisItem21.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            chartAxisItem22.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            chartAxisItem23.Value = new decimal(new int[] {
            22,
            0,
            0,
            0});
            chartAxisItem24.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            chartAxisItem25.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            chartAxisItem26.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            chartAxisItem27.Value = new decimal(new int[] {
            26,
            0,
            0,
            0});
            chartAxisItem28.Value = new decimal(new int[] {
            27,
            0,
            0,
            0});
            chartAxisItem29.Value = new decimal(new int[] {
            28,
            0,
            0,
            0});
            this.chart1.PlotArea.XAxis.Items.AddRange(new Telerik.Reporting.Charting.ChartAxisItem[] {
            chartAxisItem1,
            chartAxisItem2,
            chartAxisItem3,
            chartAxisItem4,
            chartAxisItem5,
            chartAxisItem6,
            chartAxisItem7,
            chartAxisItem8,
            chartAxisItem9,
            chartAxisItem10,
            chartAxisItem11,
            chartAxisItem12,
            chartAxisItem13,
            chartAxisItem14,
            chartAxisItem15,
            chartAxisItem16,
            chartAxisItem17,
            chartAxisItem18,
            chartAxisItem19,
            chartAxisItem20,
            chartAxisItem21,
            chartAxisItem22,
            chartAxisItem23,
            chartAxisItem24,
            chartAxisItem25,
            chartAxisItem26,
            chartAxisItem27,
            chartAxisItem28,
            chartAxisItem29});
            this.chart1.PlotArea.XAxis.MaxValue = 28D;
            this.chart1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(4.1000003814697266D));
            this.chart1.NeedDataSource += new System.EventHandler(this.chart1_NeedDataSource);
            // 
            // chart2
            // 
            this.chart2.BitmapResolution = 96F;
            this.chart2.ChartTitle.TextBlock.Appearance.Position.AlignedPosition = Telerik.Reporting.Charting.Styles.AlignedPositions.Top;
            this.chart2.ChartTitle.TextBlock.Appearance.Position.Auto = false;
            this.chart2.ChartTitle.TextBlock.Appearance.Position.X = 6F;
            this.chart2.ChartTitle.TextBlock.Appearance.Position.Y = 4F;
            this.chart2.ChartTitle.TextBlock.Text = "GRAFIK DATA";
            this.chart2.ImageFormat = System.Drawing.Imaging.ImageFormat.Emf;
            this.chart2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(10.000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(6.5D));
            this.chart2.Name = "chart2";
            this.chart2.PlotArea.Appearance.Border.Color = System.Drawing.Color.White;
            this.chart2.PlotArea.Appearance.FillStyle.FillType = Telerik.Reporting.Charting.Styles.FillType.Solid;
            this.chart2.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.Color.White;
            this.chart2.PlotArea.EmptySeriesMessage.Appearance.Visible = true;
            this.chart2.PlotArea.EmptySeriesMessage.Visible = true;
            this.chart2.PlotArea.XAxis.AutoScale = false;
            chartAxisItem30.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.chart2.PlotArea.XAxis.Items.AddRange(new Telerik.Reporting.Charting.ChartAxisItem[] {
            chartAxisItem30});
            this.chart2.PlotArea.XAxis.MaxValue = 1D;
            this.chart2.PlotArea.XAxis.MinValue = 1D;
            this.chart2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3999977111816406D), Telerik.Reporting.Drawing.Unit.Inch(4.0999999046325684D));
            this.chart2.NeedDataSource += new System.EventHandler(this.chart2_NeedDataSource);
            // 
            // ReportSheObservationDate
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "ReportSheObservation";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;
            reportParameter1.AllowBlank = false;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.sqlDataSource1;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.year";
            reportParameter1.AvailableValues.ValueMember = "= Fields.year";
            reportParameter1.Name = "year";
            reportParameter1.Text = "year";
            reportParameter1.Visible = true;
            reportParameter2.AllowBlank = false;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.sqlDataSource2;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.month";
            reportParameter2.AvailableValues.ValueMember = "= Fields.month";
            reportParameter2.Name = "month";
            reportParameter2.Text = "month";
            reportParameter2.Visible = true;
            reportParameter3.AllowBlank = false;
            reportParameter3.AutoRefresh = true;
            reportParameter3.AvailableValues.DataSource = this.sqlDataSource3;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.day";
            reportParameter3.AvailableValues.ValueMember = "= Fields.day";
            reportParameter3.Name = "day";
            reportParameter3.Text = "day";
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.5D);
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryDate1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.SubReport subReport1;
        private Telerik.Reporting.Chart chart1;
        private Telerik.Reporting.Chart chart2;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
        private Telerik.Reporting.SqlDataSource sqlDataSource2;
        private Telerik.Reporting.SqlDataSource sqlDataSource3;
        private ObservationSummaryDate observationSummaryDate1;
    }
}