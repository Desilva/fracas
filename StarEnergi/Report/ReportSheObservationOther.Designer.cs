namespace StarEnergi.Report
{
    partial class ReportSheObservationOther
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
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            this.sqlDataSource3 = new Telerik.Reporting.SqlDataSource();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.sqlDataSource2 = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.panel1 = new Telerik.Reporting.Panel();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther1 = new StarEnergi.Report.ObservationSummaryOther();
            this.chart1 = new Telerik.Reporting.Chart();
            this.chart2 = new Telerik.Reporting.Chart();
            this.subReport2 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther11 = new StarEnergi.Report.ObservationSummaryOther1();
            this.subReport3 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther21 = new StarEnergi.Report.ObservationSummaryOther2();
            this.subReport4 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther31 = new StarEnergi.Report.ObservationSummaryOther3();
            this.subReport5 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther41 = new StarEnergi.Report.ObservationSummaryOther4();
            this.subReport6 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther51 = new StarEnergi.Report.ObservationSummaryOther5();
            this.subReport7 = new Telerik.Reporting.SubReport();
            this.observationSummaryOther61 = new StarEnergi.Report.ObservationSummaryOther6();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther41)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther51)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther61)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource3
            // 
            this.sqlDataSource3.ConnectionString = "starenergygeo";
            this.sqlDataSource3.Name = "sqlDataSource3";
            this.sqlDataSource3.SelectCommand = "SELECT DISTINCT department\r\nFROM            she_observation";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "starenergygeo";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.SelectCommand = "select distinct observer, department\r\nfrom she_observation";
            // 
            // sqlDataSource2
            // 
            this.sqlDataSource2.ConnectionString = "starenergygeo";
            this.sqlDataSource2.Name = "sqlDataSource2";
            this.sqlDataSource2.SelectCommand = "select id,title from she_obs_category";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(10.699999809265137D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pictureBox1,
            this.panel1,
            this.subReport1,
            this.chart1,
            this.chart2,
            this.subReport2,
            this.subReport3,
            this.subReport4,
            this.subReport5,
            this.subReport6,
            this.subReport7});
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
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport1.ReportSource = this.observationSummaryOther1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(4.9499998092651367D));
            // 
            // observationSummaryOther1
            // 
            this.observationSummaryOther1.Name = "ObservationSummary";
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
            this.chart2.PlotArea.XAxis.MaxValue = 1D;
            this.chart2.PlotArea.XAxis.MinValue = 1D;
            this.chart2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3999977111816406D), Telerik.Reporting.Drawing.Unit.Inch(4.0999999046325684D));
            this.chart2.NeedDataSource += new System.EventHandler(this.chart2_NeedDataSource);
            // 
            // subReport2
            // 
            this.subReport2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999748170375824D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.subReport2.Name = "subReport2";
            this.subReport2.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport2.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport2.ReportSource = this.observationSummaryOther11;
            this.subReport2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299999237060547D), Telerik.Reporting.Drawing.Unit.Inch(4.9499993324279785D));
            this.subReport2.Style.Visible = false;
            // 
            // observationSummaryOther11
            // 
            this.observationSummaryOther11.Name = "ObservationSummary1";
            // 
            // subReport3
            // 
            this.subReport3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000038146972656D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.subReport3.Name = "subReport3";
            this.subReport3.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport3.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport3.ReportSource = this.observationSummaryOther21;
            this.subReport3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299998283386231D), Telerik.Reporting.Drawing.Unit.Inch(4.9499993324279785D));
            // 
            // observationSummaryOther21
            // 
            this.observationSummaryOther21.Name = "ObservationSummary2";
            // 
            // subReport4
            // 
            this.subReport4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000038146972656D), Telerik.Reporting.Drawing.Unit.Inch(1.4000002145767212D));
            this.subReport4.Name = "subReport4";
            this.subReport4.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport4.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport4.ReportSource = this.observationSummaryOther31;
            this.subReport4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299998283386231D), Telerik.Reporting.Drawing.Unit.Inch(4.9499993324279785D));
            // 
            // observationSummaryOther31
            // 
            this.observationSummaryOther31.Name = "ObservationSummary3";
            // 
            // subReport5
            // 
            this.subReport5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000038146972656D), Telerik.Reporting.Drawing.Unit.Inch(1.4000002145767212D));
            this.subReport5.Name = "subReport5";
            this.subReport5.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport5.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport5.ReportSource = this.observationSummaryOther41;
            this.subReport5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299999237060547D), Telerik.Reporting.Drawing.Unit.Inch(4.9499998092651367D));
            // 
            // observationSummaryOther41
            // 
            this.observationSummaryOther41.Name = "ObservationSummary4";
            // 
            // subReport6
            // 
            this.subReport6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000038146972656D), Telerik.Reporting.Drawing.Unit.Inch(1.4000002145767212D));
            this.subReport6.Name = "subReport6";
            this.subReport6.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport6.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport6.ReportSource = this.observationSummaryOther51;
            this.subReport6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299999237060547D), Telerik.Reporting.Drawing.Unit.Inch(4.9499998092651367D));
            // 
            // observationSummaryOther51
            // 
            this.observationSummaryOther51.Name = "ObservationSummary5";
            // 
            // subReport7
            // 
            this.subReport7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000038146972656D), Telerik.Reporting.Drawing.Unit.Inch(1.4000002145767212D));
            this.subReport7.Name = "subReport7";
            this.subReport7.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport7.Parameters.Add(new Telerik.Reporting.Parameter("observer", "=Parameters.observer.Value"));
            this.subReport7.ReportSource = this.observationSummaryOther61;
            this.subReport7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.299998283386231D), Telerik.Reporting.Drawing.Unit.Inch(4.9499993324279785D));
            // 
            // observationSummaryOther61
            // 
            this.observationSummaryOther61.Name = "ObservationSummary6";
            // 
            // ReportSheObservationOther
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "ReportSheObservation";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;
            reportParameter1.AllowBlank = false;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.sqlDataSource3;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.department";
            reportParameter1.AvailableValues.ValueMember = "= Fields.department";
            reportParameter1.Name = "department";
            reportParameter1.Text = "department";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.sqlDataSource1;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.observer";
            reportParameter2.AvailableValues.Filters.AddRange(new Telerik.Reporting.Data.Filter[] {
            new Telerik.Reporting.Data.Filter("=Fields.department", Telerik.Reporting.Data.FilterOperator.Equal, "=Parameters.department.Value")});
            reportParameter2.AvailableValues.ValueMember = "= Fields.observer";
            reportParameter2.Name = "observer";
            reportParameter2.Text = "observer";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AutoRefresh = true;
            reportParameter3.AvailableValues.DataSource = this.sqlDataSource2;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.title";
            reportParameter3.AvailableValues.ValueMember = "= Fields.id";
            reportParameter3.Name = "category";
            reportParameter3.Text = "category";
            reportParameter3.Type = Telerik.Reporting.ReportParameterType.Integer;
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.535433769226074D);
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther41)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther51)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryOther61)).EndInit();
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
        private Telerik.Reporting.SqlDataSource sqlDataSource3;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
        private ObservationSummaryOther observationSummaryOther1;
        private Telerik.Reporting.SubReport subReport2;
        private ObservationSummaryOther1 observationSummaryOther11;
        private Telerik.Reporting.SubReport subReport3;
        private ObservationSummaryOther2 observationSummaryOther21;
        private Telerik.Reporting.SubReport subReport4;
        private ObservationSummaryOther3 observationSummaryOther31;
        private Telerik.Reporting.SubReport subReport5;
        private ObservationSummaryOther4 observationSummaryOther41;
        private Telerik.Reporting.SubReport subReport6;
        private ObservationSummaryOther5 observationSummaryOther51;
        private Telerik.Reporting.SubReport subReport7;
        private ObservationSummaryOther6 observationSummaryOther61;
        private Telerik.Reporting.SqlDataSource sqlDataSource2;
    }
}