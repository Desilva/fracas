namespace StarEnergi.Report
{
    partial class ReportSheObservationDepartment
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
            this.sqlDataSource3 = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.panel1 = new Telerik.Reporting.Panel();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.observationSummaryDepartment1 = new StarEnergi.Report.ObservationSummaryDepartment();
            this.chart1 = new Telerik.Reporting.Chart();
            this.chart2 = new Telerik.Reporting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryDepartment1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource3
            // 
            this.sqlDataSource3.ConnectionString = "starenergygeo";
            this.sqlDataSource3.Name = "sqlDataSource3";
            this.sqlDataSource3.SelectCommand = "SELECT DISTINCT department\r\nFROM            she_observation";
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
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.800000190734863D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(36D);
            this.textBox1.Value = "STAR ENERGY WAYANG WINDU (WW) LIMITED";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.subReport1.Name = "subReport1";
            this.subReport1.Parameters.Add(new Telerik.Reporting.Parameter("department", "=Parameters.department.Value"));
            this.subReport1.ReportSource = this.observationSummaryDepartment1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(4.9499998092651367D));
            // 
            // observationSummaryDepartment1
            // 
            this.observationSummaryDepartment1.Name = "ObservationSummary";
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
            this.chart1.PlotArea.XAxis.AddRange(0, 28, 1);
            this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Berpotensi kecelakaan";
            this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Berpotensi kebakaran/meledak";
            this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "Pencegahan kecelakaan";
            this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "Pengawas berada dilokasi";
            this.chart1.PlotArea.XAxis.Items[4].TextBlock.Text = "Fit untuk kerja";
            this.chart1.PlotArea.XAxis.Items[5].TextBlock.Text = "Masalah Psykologi";
            this.chart1.PlotArea.XAxis.Items[6].TextBlock.Text = "Ergonomi/posisi tubuh";
            this.chart1.PlotArea.XAxis.Items[7].TextBlock.Text = "Bahan berbahaya ke tubuh";
            this.chart1.PlotArea.XAxis.Items[8].TextBlock.Text = "Kebersihan & kesehatan";
            this.chart1.PlotArea.XAxis.Items[9].TextBlock.Text = "Kerapihan & kebersihan";
            this.chart1.PlotArea.XAxis.Items[10].TextBlock.Text = "Pemberian tanda";
            this.chart1.PlotArea.XAxis.Items[11].TextBlock.Text = "Berpotensi pencemaran";
            this.chart1.PlotArea.XAxis.Items[12].TextBlock.Text = "Boros enerji";
            this.chart1.PlotArea.XAxis.Items[13].TextBlock.Text = "Pencegahan pencemaran";
            this.chart1.PlotArea.XAxis.Items[14].TextBlock.Text = "Ketersediaan alat penyerap";
            this.chart1.PlotArea.XAxis.Items[15].TextBlock.Text = "Identifikasi bahaya & penanggulangan";
            this.chart1.PlotArea.XAxis.Items[16].TextBlock.Text = "Dokumen ijin kerja";
            this.chart1.PlotArea.XAxis.Items[17].TextBlock.Text = "SOP- Petunjuk kerja";
            this.chart1.PlotArea.XAxis.Items[18].TextBlock.Text = "MSDS – Material Safety Data Sheets";
            this.chart1.PlotArea.XAxis.Items[19].TextBlock.Text = "Siap tanggap darurat";
            this.chart1.PlotArea.XAxis.Items[20].TextBlock.Text = "sertifikat operator/peralatan";
            this.chart1.PlotArea.XAxis.Items[21].TextBlock.Text = "PPE – Alat Pelindung Diri";
            this.chart1.PlotArea.XAxis.Items[22].TextBlock.Text = "Alat portabel";
            this.chart1.PlotArea.XAxis.Items[23].TextBlock.Text = "Mekanis";
            this.chart1.PlotArea.XAxis.Items[24].TextBlock.Text = "Listrik";
            this.chart1.PlotArea.XAxis.Items[25].TextBlock.Text = "kendaraan/alat berat";
            this.chart1.PlotArea.XAxis.Items[26].TextBlock.Text = "Fasilitas substandard";
            this.chart1.PlotArea.XAxis.Items[27].TextBlock.Text = "Paparan H2S";
            this.chart1.PlotArea.XAxis.Items[28].TextBlock.Text = "bahaya tempat kerja terhadap kesehatan";
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
            this.chart2.PlotArea.XAxis.AddRange(1, 1, 1);
            this.chart2.PlotArea.XAxis.Items[0].TextBlock.Text = "Kondisi";
            this.chart2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.3999977111816406D), Telerik.Reporting.Drawing.Unit.Inch(4.0999999046325684D));
            this.chart2.NeedDataSource += new System.EventHandler(this.chart2_NeedDataSource);
            // 
            // ReportSheObservationDepartment
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
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.sqlDataSource3;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.department";
            reportParameter1.AvailableValues.ValueMember = "= Fields.department";
            reportParameter1.Name = "department";
            reportParameter1.Text = "department";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.5D);
            ((System.ComponentModel.ISupportInitialize)(this.observationSummaryDepartment1)).EndInit();
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
        private ObservationSummaryDepartment observationSummaryDepartment1;
    }
}