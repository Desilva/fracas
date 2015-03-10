namespace StarEnergi.Reporting
{
    partial class PIRTrend
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Charting.Styles.Corners corners1 = new Telerik.Reporting.Charting.Styles.Corners();
            Telerik.Reporting.Charting.Styles.ChartMargins chartMargins1 = new Telerik.Reporting.Charting.Styles.ChartMargins();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PIRTrend));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector3 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector4 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.DataReportParameters = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.panel4 = new Telerik.Reporting.Panel();
            this.panel5 = new Telerik.Reporting.Panel();
            this.ImprovementIndicatorChart = new Telerik.Reporting.Chart();
            this.DataTableKBP = new Telerik.Reporting.SqlDataSource();
            this.DataTable1 = new Telerik.Reporting.SqlDataSource();
            this.DataTable2 = new Telerik.Reporting.SqlDataSource();
            this.DataTableRekap = new Telerik.Reporting.SqlDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // DataReportParameters
            // 
            this.DataReportParameters.ConnectionString = "starenergygeo";
            this.DataReportParameters.Name = "DataReportParameters";
            this.DataReportParameters.SelectCommand = "select distinct YEAR(date_rise)as Tahun from pir";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(13.600001335144043D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel4});
            this.detail.Name = "detail";
            // 
            // panel4
            // 
            this.panel4.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel5});
            this.panel4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.panel4.Name = "panel4";
            this.panel4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(7D));
            // 
            // panel5
            // 
            this.panel5.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.ImprovementIndicatorChart});
            this.panel5.KeepTogether = true;
            this.panel5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel5.Name = "panel5";
            this.panel5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(7D));
            // 
            // ImprovementIndicatorChart
            // 
            this.ImprovementIndicatorChart.Appearance.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            corners1.BottomLeft = Telerik.Reporting.Charting.Styles.CornerType.Round;
            corners1.BottomRight = Telerik.Reporting.Charting.Styles.CornerType.Round;
            corners1.RoundSize = 6;
            corners1.TopLeft = Telerik.Reporting.Charting.Styles.CornerType.Round;
            corners1.TopRight = Telerik.Reporting.Charting.Styles.CornerType.Round;
            this.ImprovementIndicatorChart.Appearance.Corners = corners1;
            this.ImprovementIndicatorChart.Appearance.FillStyle.FillSettings.BackgroundImage = "{chart}";
            this.ImprovementIndicatorChart.Appearance.FillStyle.FillSettings.ImageDrawMode = Telerik.Reporting.Charting.Styles.ImageDrawMode.Flip;
            this.ImprovementIndicatorChart.Appearance.FillStyle.FillSettings.ImageFlip = Telerik.Reporting.Charting.Styles.ImageTileModes.FlipX;
            this.ImprovementIndicatorChart.Appearance.FillStyle.FillType = Telerik.Reporting.Charting.Styles.FillType.Image;
            this.ImprovementIndicatorChart.AutoLayout = true;
            this.ImprovementIndicatorChart.BitmapResolution = 96F;
            this.ImprovementIndicatorChart.ChartTitle.Appearance.FillStyle.MainColor = System.Drawing.Color.Empty;
            this.ImprovementIndicatorChart.ChartTitle.Appearance.Position.AlignedPosition = Telerik.Reporting.Charting.Styles.AlignedPositions.Top;
            this.ImprovementIndicatorChart.ChartTitle.TextBlock.Appearance.TextProperties.Font = new System.Drawing.Font("Tahoma", 13F);
            this.ImprovementIndicatorChart.ChartTitle.TextBlock.Text = "PIR Trend Data";
            this.ImprovementIndicatorChart.DataSource = null;
            this.ImprovementIndicatorChart.DefaultType = Telerik.Reporting.Charting.ChartSeriesType.Line;
            this.ImprovementIndicatorChart.ImageFormat = System.Drawing.Imaging.ImageFormat.Emf;
            this.ImprovementIndicatorChart.Legend.Appearance.Border.Color = System.Drawing.Color.Transparent;
            chartMargins1.Right = new Telerik.Reporting.Charting.Styles.Unit(3D, Telerik.Reporting.Charting.Styles.UnitType.Percentage);
            chartMargins1.Top = new Telerik.Reporting.Charting.Styles.Unit(15.399999618530273D, Telerik.Reporting.Charting.Styles.UnitType.Percentage);
            this.ImprovementIndicatorChart.Legend.Appearance.Dimensions.Margins = chartMargins1;
            this.ImprovementIndicatorChart.Legend.Appearance.FillStyle.MainColor = System.Drawing.Color.Empty;
            this.ImprovementIndicatorChart.Legend.Appearance.ItemMarkerAppearance.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.Legend.Appearance.ItemMarkerAppearance.Figure = "Square";
            this.ImprovementIndicatorChart.Legend.Appearance.ItemTextAppearance.MaxLength = 20;
            this.ImprovementIndicatorChart.Legend.Appearance.Position.AlignedPosition = Telerik.Reporting.Charting.Styles.AlignedPositions.TopRight;
            this.ImprovementIndicatorChart.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.ImprovementIndicatorChart.Name = "ImprovementIndicatorChart";
            this.ImprovementIndicatorChart.PlotArea.Appearance.Border.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.Appearance.FillStyle.FillType = Telerik.Reporting.Charting.Styles.FillType.Solid;
            this.ImprovementIndicatorChart.PlotArea.Appearance.FillStyle.MainColor = System.Drawing.Color.White;
            this.ImprovementIndicatorChart.PlotArea.EmptySeriesMessage.Appearance.Visible = true;
            this.ImprovementIndicatorChart.PlotArea.EmptySeriesMessage.Visible = true;
            this.ImprovementIndicatorChart.PlotArea.XAxis.Appearance.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.XAxis.Appearance.MajorGridLines.Color = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(222)))), ((int)(((byte)(227)))));
            this.ImprovementIndicatorChart.PlotArea.XAxis.Appearance.MajorGridLines.PenStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            this.ImprovementIndicatorChart.PlotArea.XAxis.Appearance.MajorTick.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.XAxis.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ImprovementIndicatorChart.PlotArea.XAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ImprovementIndicatorChart.PlotArea.XAxis.LayoutMode = Telerik.Reporting.Charting.Styles.ChartAxisLayoutMode.Normal;
            this.ImprovementIndicatorChart.PlotArea.XAxis.MinValue = 1D;
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.MajorGridLines.Color = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(222)))), ((int)(((byte)(227)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.MajorTick.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.MinorGridLines.Color = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(239)))), ((int)(((byte)(241)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.MinorTick.Color = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.MinorTick.Width = 0F;
            this.ImprovementIndicatorChart.PlotArea.YAxis.Appearance.TextAppearance.TextProperties.Color = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ImprovementIndicatorChart.PlotArea.YAxis.AxisLabel.TextBlock.Appearance.TextProperties.Color = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ImprovementIndicatorChart.SeriesPalette = "Mac";
            this.ImprovementIndicatorChart.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.300000190734863D), Telerik.Reporting.Drawing.Unit.Inch(7D));
            this.ImprovementIndicatorChart.Skin = "Mac";
            this.ImprovementIndicatorChart.NeedDataSource += new System.EventHandler(this.ImprovementIndicatorChart_NeedDataSource);
            // 
            // DataTableKBP
            // 
            this.DataTableKBP.ConnectionString = "starenergygeo";
            this.DataTableKBP.Name = "DataTableKBP";
            this.DataTableKBP.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@YEAR", System.Data.DbType.String, "=Parameters.Year.Value")});
            this.DataTableKBP.SelectCommand = resources.GetString("DataTableKBP.SelectCommand");
            // 
            // DataTable1
            // 
            this.DataTable1.ConnectionString = "starenergygeo";
            this.DataTable1.Name = "DataReport";
            this.DataTable1.SelectCommand = resources.GetString("DataTable1.SelectCommand");
            // 
            // DataTable2
            // 
            this.DataTable2.ConnectionString = "starenergygeo";
            this.DataTable2.Name = "DataReport2";
            this.DataTable2.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@YEAR", System.Data.DbType.String, "=Parameters.Year.Value")});
            this.DataTable2.SelectCommand = resources.GetString("DataTable2.SelectCommand");
            // 
            // DataTableRekap
            // 
            this.DataTableRekap.ConnectionString = "starenergygeo";
            this.DataTableRekap.Name = "DataTableRekap";
            this.DataTableRekap.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@YEAR", System.Data.DbType.String, "=Parameters.Year.Value")});
            this.DataTableRekap.SelectCommand = resources.GetString("DataTableRekap.SelectCommand");
            // 
            // PIRTrend
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "PIR_Report";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.DataReportParameters;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Tahun";
            reportParameter1.AvailableValues.Sortings.AddRange(new Telerik.Reporting.Data.Sorting[] {
            new Telerik.Reporting.Data.Sorting("=Fields.Tahun", Telerik.Reporting.Data.SortDirection.Desc)});
            reportParameter1.AvailableValues.ValueMember = "= Fields.Tahun";
            reportParameter1.Name = "Year";
            reportParameter1.Text = "Year";
            reportParameter1.Value = "= Fields.Tahun";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Normal.TableNormal")});
            styleRule1.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableHeader")});
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            descendantSelector3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableGroup")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector3});
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            descendantSelector4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.GrandTotal")});
            styleRule5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector4});
            styleRule5.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4,
            styleRule5});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.692914009094238D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.SqlDataSource DataTable1;
        private Telerik.Reporting.SqlDataSource DataTable2;
        private Telerik.Reporting.Chart ImprovementIndicatorChart;
        private Telerik.Reporting.SqlDataSource DataTableRekap;
        private Telerik.Reporting.SqlDataSource DataTableKBP;
        private Telerik.Reporting.SqlDataSource DataReportParameters;
        private Telerik.Reporting.Panel panel5;
        private Telerik.Reporting.Panel panel4;
    }
}