namespace StarEnergi.Reporting
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data.SqlClient;
    using System.Data;
    using System.Linq;
    using System.Configuration;
    using System.Collections.Generic;
    using Telerik.Reporting.Charting;

    /// <summary>
    /// Summary description for PIR_Report.
    /// </summary>
    public partial class PIRTrend : Telerik.Reporting.Report
    {
        public PIRTrend()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void ImprovementIndicatorChart_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            
            DataTable table = new DataTable();
            //string sql = @"SELECT procedure_reference, COUNT(date_rise) as [yeartodate] FROM pir group by procedure_reference";
            //string connectionString = "Data Source=LIGHTCROSS;Initial Catalog=star_energi_geo;Integrated Security=True";
            //SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter(
            @"with proc_owner_list(ownlist) as
                (
                select 'BPL'
                union all
                select 'GEL'
                union all
                select 'GDI'
                union all
                select 'POP'
                union all
                select 'MTW'
                union all
                select 'SPE'
                union all
                select 'OHE'
                union all
                select 'EPE'
                union all
                select 'EAI'
                union all
                select 'OSU'
                union all
                select 'LCO'
                union all
                select 'SSU'
                union all
                select 'MER'
                union all
                select 'SEC'
                union all
                select 'SCM'
                union all
                select 'MIS'
                union all
                select 'FPR'
                union all
                select 'PAC'
                union all
                select 'FHR'
                ),
                mnth(mnuum) as
                (
                select 2012
                union all
                select mnuum+1 from mnth where mnuum<year(getDate())
                )
                select a.mnuum as year, proc_owner_list.ownlist as process_owner, COUNT(CASE WHEN isnull(p.target_completion_init,'') <> '' AND (((year(GETDATE()) = a.mnuum) AND DateDiff(day,GETDATE(),p.target_completion_init) < 0 AND (p.status <> 'VERIFIED' OR (p.status = 'VERIFIED' AND DateDiff(day,GETDATE(),p.initiator_verified_date) > 0))) OR ((year(GETDATE()) <> a.mnuum) AND DateDiff(day,convert(datetime, ('12-31-' + Convert(varchar, a.mnuum))),p.target_completion_init) < 0 AND (p.status <> 'VERIFIED' OR (p.status = 'VERIFIED' AND DateDiff(day,convert(datetime, ('12-31-' + Convert(varchar, a.mnuum))),p.initiator_verified_date) > 0)))) THEN 1 END) as jumlah
                from proc_owner_list cross join (select mnth.mnuum from mnth) a
                left join (Select * from pir where DATEDIFF(day,GETDATE(),date_rise) <= 0) as p on p.process_owner = proc_owner_list.ownlist
                group by ownlist, a.mnuum"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);    
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            defChart.PlotArea.XAxis.IsZeroBased = false;
            defChart.PlotArea.XAxis.AutoScale = false;
            defChart.PlotArea.XAxis.MinValue = 2012;
            defChart.PlotArea.XAxis.MaxValue = 2014;
            defChart.PlotArea.XAxis.Step = 1;
            defChart.PlotArea.XAxis.LabelStep = 1;
            Dictionary<string, ChartSeries> chartSeries = new Dictionary<string, ChartSeries>();
            Dictionary<string, List<ChartSeriesItem>> chartSeriesItems = new Dictionary<string, List<ChartSeriesItem>>();
            defChart.Series.Clear();
            Color[] colors = new Color[] { 
                ColorTranslator.FromHtml("#512F1C"),
                ColorTranslator.FromHtml("#6B471E"),
                ColorTranslator.FromHtml("#865F1F"),
                ColorTranslator.FromHtml("#A07621"),
                ColorTranslator.FromHtml("#BA8E22"),
                ColorTranslator.FromHtml("#D19F2F"),
                ColorTranslator.FromHtml("#B47127"),
                ColorTranslator.FromHtml("#97441E"),
                ColorTranslator.FromHtml("#7A1616"),
                ColorTranslator.FromHtml("#9E2038"),
                ColorTranslator.FromHtml("#BE1E2D"),
                ColorTranslator.FromHtml("#CC3C29"),
                ColorTranslator.FromHtml("#DB5926"),
                ColorTranslator.FromHtml("#E97722"),
                ColorTranslator.FromHtml("#F7941E"),
                ColorTranslator.FromHtml("#F9A718"),
                ColorTranslator.FromHtml("#FABA12"),
                ColorTranslator.FromHtml("#FCCC0C"),
                ColorTranslator.FromHtml("#FDDF06"),
            };
            Palette pal = new Palette();
            foreach (var color in colors)
            {
                pal.Items.Add(new PaletteItem(color, color));
            }
            pal.Name = "MyPalette2";
            this.ImprovementIndicatorChart.CustomPalettes.Add(pal);
            this.ImprovementIndicatorChart.SeriesPalette = "MyPalette2";
            int i = 0;
            foreach (DataRow row in table.Rows)
            {
                if (!chartSeries.Keys.Contains((string)row["process_owner"]))
                {
                    ChartSeries series = new ChartSeries();
                    series.Type = ChartSeriesType.Line;
                    defChart.Series.Add(series);
                    series.Clear();
                    series.Appearance.LineSeriesAppearance.Color = colors[i]; i++;
                    series.Name = (string)row["process_owner"];
                    series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.SeriesName;
                    chartSeries.Add((string)row["process_owner"], series);
                } 

                ChartSeriesItem item = new ChartSeriesItem();
                item.XValue = (int)row["year"];
                item.YValue = (int)row["jumlah"];
                item.Label.TextBlock.Text = (string)row["process_owner"] + ", #Y";
                chartSeries[(string)row["process_owner"]].AddItem(item);
            } 

        }
    }

}