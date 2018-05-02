namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;
    using Telerik.Reporting.Charting;

    /// <summary>
    /// Summary description for SheObservationSeverity.
    /// </summary>
    public partial class SheObservationSeverity : Telerik.Reporting.Report
    {
        public SheObservationSeverity()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            Color[] barColors = new Color[12]{
               Color.Purple,
               Color.SteelBlue,
               Color.Aqua,
               Color.Yellow,
               Color.Navy,
               Color.Green,
               Color.Blue,
               Color.Red,
               Color.Honeydew,
               Color.Magenta,
               Color.SpringGreen,
               Color.Gold
           };
            //int i = 0;
            //if (this.ReportParameters[2].Value != null)
            //{
            //    foreach (ChartSeriesItem item in chart1.Series[0].Items)
            //    {
            //        item.Appearance.FillStyle.MainColor = barColors[i++];
            //    }
            //}

            Palette seriesPalette = new Palette("seriesPalette", barColors, true);
            chart1.CustomPalettes.Add(seriesPalette);
            chart1.SeriesPalette = "seriesPalette";
        }

        private void SheObservation_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            //string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            DataTable table = new DataTable();
            string from_date_str = this.ReportParameters[0].Value.ToString();
            string from_year = from_date_str.Substring(from_date_str.LastIndexOf('/') + 1, 4);
            string from_day = from_date_str.Substring(from_date_str.IndexOf('/') + 1, from_date_str.LastIndexOf('/') - (from_date_str.IndexOf('/') + 1));
            string from_month = from_date_str.Substring(0, from_date_str.IndexOf('/'));
            string to_date_str = this.ReportParameters[1].Value.ToString();
            string to_year = to_date_str.Substring(to_date_str.LastIndexOf('/') + 1, 4);
            string to_day = to_date_str.Substring(to_date_str.IndexOf('/') + 1, to_date_str.LastIndexOf('/') - (to_date_str.IndexOf('/') + 1));
            string to_month = to_date_str.Substring(0, to_date_str.IndexOf('/'));
            string sql = "";
            if (this.ReportParameters[2].Value == null)
            {
                sql = @"select sum(major_ua)+sum(major_uc)+sum(major_sa)+sum(major_sc) as major, 
                    sum(mod_ua)+sum(mod_uc)+sum(mod_sa)+sum(mod_sc) as mod, 
                    sum(minor_ua)+sum(minor_uc)+sum(minor_sa)+sum(minor_sc) as minor 
                    from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + "')";
            }
            else if ((Int64)this.ReportParameters[2].Value == 1)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                    sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                    sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                    from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                    WHERE num = 1 or num = 2 or num = 3 or num = 4";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                    sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                    sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                    from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                    WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
                
            }
            else if ((Int64)this.ReportParameters[2].Value == 2)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = 5 or num = 6 or num = 7 or num = 8 or num = 9";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
            }
            else if ((Int64)this.ReportParameters[2].Value == 3)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        where num = 10 or num = 11 or num = 12 or num = 13 or num = 14 or num = 15";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
            }
            else if ((Int64)this.ReportParameters[2].Value == 4)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        where num = 16 or num = 17 or num = 18 or num = 19 or num = 20 or num = 21";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
            }
            else if ((Int64)this.ReportParameters[2].Value == 5)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        where num = 22 or num = 23 or num = 24 or num = 25 or num = 26";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
            }
            else if ((Int64)this.ReportParameters[2].Value == 6)
            {
                if (this.ReportParameters[3].Value == null)
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        where num = 27 or num = 28 or num = 29";
                }
                else
                {
                    sql = @"select sum(major_ua) as major_ua, sum(major_uc) as major_uc, sum(major_sa) as major_sa, sum(major_sc) as major_sc, 
                        sum(mod_ua) as mod_ua, sum(mod_uc) as mod_uc, sum(mod_sa) as mod_sa, sum(mod_sc) as mod_sc, 
                        sum(minor_ua) as minor_ua, sum(minor_uc) as minor_uc, sum(minor_sa) as minor_sa, sum(minor_sc) as minor_sc 
                        from observationChartSeverity('" + from_year + "','" + from_month.PadLeft(2, '0') + "','" + from_day.PadLeft(2, '0') + "','" + to_year + "','" + to_month.PadLeft(2, '0') + "','" + to_day.PadLeft(2, '0') + @"') 
                        WHERE num = " + (Int64)this.ReportParameters[3].Value;
                }
            }
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(sql);
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
            cmd.Connection = connection;
            cmd.CommandTimeout = 1800;

            adapter.SelectCommand = cmd;            
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            ChartSeries series = new ChartSeries();
            series.Type = ChartSeriesType.Pie;
            defChart.Series.Clear();
            defChart.Series.Add(series);
            series.Clear();
            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;

            foreach (DataRow row in table.Rows)
            {
                ChartSeriesItem item = new ChartSeriesItem();
                if (this.ReportParameters[2].Value == null)
                {
                    int total = (int)row["major"] + (int)row["mod"] + (int)row["minor"];
                    item.Name = "Serious";
                    item.YValue = (int)row["major"] * 100 / total;
                    item.Label.TextBlock.Text = "#%";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Moderate";
                    item.YValue = (int)row["mod"] * 100 / total;
                    item.Label.TextBlock.Text = "#%";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Minor";
                    item.YValue = (int)row["minor"] * 100 / total;
                    item.Label.TextBlock.Text = "#%";
                    series.Items.Add(item);
                }
                else
                {
                    item.Name = "Serious - UA";
                    item.YValue = (int)row["major_ua"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Serious - UC";
                    item.YValue = (int)row["major_uc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Serious - SA";
                    item.YValue = (int)row["major_sa"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Serious - SC";
                    item.YValue = (int)row["major_sc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Moderate - UA";
                    item.YValue = (int)row["mod_ua"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Moderate - UC";
                    item.YValue = (int)row["mod_uc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Moderate - SA";
                    item.YValue = (int)row["mod_sa"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Moderate - SC";
                    item.YValue = (int)row["mod_sc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Minor - UA";
                    item.YValue = (int)row["minor_ua"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Minor - UC";
                    item.YValue = (int)row["minor_uc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Minor - SA";
                    item.YValue = (int)row["minor_sa"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                    item = new ChartSeriesItem();
                    item.Name = "Minor - SC";
                    item.YValue = (int)row["minor_sc"];
                    item.Label.TextBlock.Text = "#Y";
                    series.Items.Add(item);
                }
            }
        }
    }
}