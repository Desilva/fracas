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
    public partial class PIR_Report : Telerik.Reporting.Report
    {
        public PIR_Report()
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
            //Models.relmon_star_energiEntities re = new Models.relmon_star_energiEntities();
            //var query = from a in re.pirs
            //            select a;
            ////List < reportpie > dataReportPie = new List<reportpie>();
            ////foreach (var a in query)
            ////{
            ////    dataReportPie.Add(new reportpie { procedure_reference = a.Procedure_Reference, year_to_date = 10 });
            ////}
            //Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            //Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            //ChartSeries series = new ChartSeries();
            //series.Type = ChartSeriesType.Pie;
            //defChart.Series.Add(series);
            //series.Clear();
            //series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            //foreach (var row in query)
            //{
            //    ChartSeriesItem item = new ChartSeriesItem();
            //    item.YValue = (double)10;
            //    item.Name = (string)row.procedure_reference;
            //    item.Label.TextBlock.Text = "#%";
            //    series.Items.Add(item);
            //} 


            ////Telerik.Reporting.Processing.Chart chart = (Telerik.Reporting.Processing.Chart)sender;

            //SqlDataAdapter adapter1 = new SqlDataAdapter(
            //        @"SELECT TOP 1 * FROM Sales.vSalesPersonSalesByFiscalYears"
            //      , ConfigurationManager.ConnectionStrings["relmon_star_energiEntities"].ConnectionString);
            //DataSet ds = new DataSet();
            //adapter1.Fill(ds);
            //chart.DataSource = ds;
            //DataTable table = new DataTable();
            //table.Columns.Add("Year");
            //table.Columns.Add("Qty");

            //table.Rows.Add(ds.Tables[0].Columns["2002"].ColumnName, ds.Tables[0].Rows[0]["2002"]);
            //table.Rows.Add(ds.Tables[0].Columns["2003"].ColumnName, ds.Tables[0].Rows[0]["2003"]);
            //table.Rows.Add(ds.Tables[0].Columns["2004"].ColumnName, ds.Tables[0].Rows[0]["2004"]);

            ////chart.DataSource = table.DefaultView;             
            DataTable table = new DataTable();
            //string sql = @"SELECT procedure_reference, COUNT(date_rise) as [yeartodate] FROM pir group by procedure_reference";
            //string connectionString = "Data Source=LIGHTCROSS;Initial Catalog=star_energi_geo;Integrated Security=True";
            //SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter(
            @"with procedure_reference_list(proc_ref_list) as
            (
                select (clause_no + ' ' + clause) from pir_clause
            )
            SELECT procedure_reference_list.proc_ref_list as procedure_reference, COUNT(pir.date_rise) as yeartodate
            from procedure_reference_list
            left join (select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1 ) as pir on procedure_reference_list.proc_ref_list = pir.procedure_reference group by proc_ref_list"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);    
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            ChartSeries series = new ChartSeries();
            series.Type = ChartSeriesType.Pie;
            defChart.Series.Clear();
            defChart.Series.Add(series);
            series.Clear();
            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
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
                ColorTranslator.FromHtml("#FFF200"),
                ColorTranslator.FromHtml("#D7DF23"),
                ColorTranslator.FromHtml("#A1CC2B"),
                ColorTranslator.FromHtml("#6CBA34"),
                ColorTranslator.FromHtml("#36A73C"),
                ColorTranslator.FromHtml("#009444"),
                ColorTranslator.FromHtml("#098054"),
                ColorTranslator.FromHtml("#126C63"),
                ColorTranslator.FromHtml("#1C5973"),
                ColorTranslator.FromHtml("#254582"),
                ColorTranslator.FromHtml("#2E3192"),
                ColorTranslator.FromHtml("#26469F"),
                ColorTranslator.FromHtml("#1E5BAC"),
                ColorTranslator.FromHtml("#176FB9"),
                ColorTranslator.FromHtml("#0F84C6"),
                ColorTranslator.FromHtml("#0799D3"),
                ColorTranslator.FromHtml("#069FC8"),
                ColorTranslator.FromHtml("#04A4BD"),
                ColorTranslator.FromHtml("#03AAB1"),
                ColorTranslator.FromHtml("#01AFA6"),
                ColorTranslator.FromHtml("#00B59B"),
                ColorTranslator.FromHtml("#262262"),
                ColorTranslator.FromHtml("#582468"),
                ColorTranslator.FromHtml("#8A266F"),
                ColorTranslator.FromHtml("#BC2875"),
                ColorTranslator.FromHtml("#EE2A7B"),
                ColorTranslator.FromHtml("#F14785"),
                ColorTranslator.FromHtml("#F5658F"),
                ColorTranslator.FromHtml("#F8829A"),
                ColorTranslator.FromHtml("#FCA0A4"),
                ColorTranslator.FromHtml("#FFBDAE")
            };
            Palette pal = new Palette();
            foreach (var color in colors)
            {
                pal.Items.Add(new PaletteItem(color, color));
            }
            pal.Name = "MyPalette";
            this.ImprovementIndicatorChart.CustomPalettes.Add(pal);
            this.ImprovementIndicatorChart.SeriesPalette = "MyPalette";
            foreach (DataRow row in table.Rows)
            {
                ChartSeriesItem item = new ChartSeriesItem();
                item.Name = (string)row["procedure_reference"];
                item.YValue = (int)row["yeartodate"];
                item.Label.TextBlock.Text = "#%";
                series.Items.Add(item);
            } 

        }

        private void PIRYearToDateChart_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(@"
            SELECT 'Closed' AS [Status], COUNT(CASE WHEN pir.status = 'VERIFIED' THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1 UNION ALL SELECT 'Raised' AS [Status],COUNT(CASE WHEN pir.status != 'VERIFIED' THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1 UNION ALL SELECT 'Open' AS [Status], COUNT(CASE WHEN DateDiff(day,date_rise,pir.initiator_verified_date) <=0 OR (isnull(pir.initiator_verified_date,'') = '' AND isnull(pir.date_rise,'') <> '') THEN 1 END) as yeartodate FROM pir where [from] = 1 AND month(date_rise) <= month(GETDATE()) UNION ALL SELECT 'Overdue' AS [Status], COUNT(CASE WHEN isnull(pir.initiator_verified_date,'') <> '' AND DateDiff(day,date_rise,pir.initiator_verified_date) > 0 THEN 1 END) as yeartodate FROM pir where [from] = 1 AND month(date_rise) <= month(GETDATE())"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
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
                item.Name = (string)row["status"];
                item.YValue = (int)row["yeartodate"];
                item.Label.TextBlock.Text = "#%";
                series.Items.Add(item);
            } 
        }

        private void OverdueChart_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(@"
            with proc_owner_list(ownlist) as
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
            )
            select proc_owner_list.ownlist as process_owner,
	            COUNT(CASE WHEN DateDiff(day,date_rise,GETDATE()) > 0 THEN 1 END) as Overdue
            from proc_owner_list
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            ChartSeries series = new ChartSeries();
            series.Type = ChartSeriesType.Pie;
            //series.Appearance.ShowLabelConnectors = true;
            defChart.Series.Clear();
            defChart.Series.Add(series);
            series.Clear();
            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            foreach (DataRow row in table.Rows)
            {
                ChartSeriesItem item = new ChartSeriesItem();
                item.Name = (string)row["process_owner"];
                item.YValue = (int)row["Overdue"];
                if (item.YValue == 0)
                {
                    item.Label.Visible = false;
                }
                item.Label.TextBlock.Text = (string)row["process_owner"] +  ", #Y";
                series.Items.Add(item);
            } 
        }

        private void OpenChart_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(@"
            with proc_owner_list(ownlist) as
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
            )
            select proc_owner_list.ownlist as process_owner,
	            COUNT(CASE WHEN DateDiff(day,date_rise,GETDATE()) <=0 THEN 1 END) as [Open]
            from proc_owner_list
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            ChartSeries series = new ChartSeries();
            series.Type = ChartSeriesType.Pie;
            //series.Appearance.ShowLabelConnectors = true;
            defChart.Series.Clear();
            defChart.Series.Add(series);
            series.Clear();
            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            foreach (DataRow row in table.Rows)
            {
                ChartSeriesItem item = new ChartSeriesItem();
                item.Name = (string)row["process_owner"];
                item.YValue = (int)row["Open"];
                if (item.YValue == 0)
                {
                    item.Label.Visible = false;
                }
                item.Label.TextBlock.Text = (string)row["process_owner"] + ", #Y";
                series.Items.Add(item);
            } 
        }

        private void ClosedChart_NeedDataSource(object sender, EventArgs e)
        {
            Telerik.Reporting.Processing.Chart report = (Telerik.Reporting.Processing.Chart)sender;
            string reportParamaters = report.Report.Parameters["year"].Value.ToString();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(@"
            with proc_owner_list(ownlist) as
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
            )
            select proc_owner_list.ownlist as process_owner,
	            COUNT(CASE WHEN p.status = 'VERIFIED' THEN 1 END) as Closed
            from proc_owner_list
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 1) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
                 , ConfigurationManager.ConnectionStrings["starenergygeo"].ConnectionString);
            adapter.Fill(table);
            Telerik.Reporting.Processing.Chart procChart = (Telerik.Reporting.Processing.Chart)sender;
            Telerik.Reporting.Chart defChart = (Telerik.Reporting.Chart)procChart.ItemDefinition;
            ChartSeries series = new ChartSeries();
            series.Type = ChartSeriesType.Pie;
            //series.Appearance.ShowLabelConnectors = true;
            defChart.Series.Clear();
            defChart.Series.Add(series);
            series.Clear();
            series.Appearance.LegendDisplayMode = ChartSeriesLegendDisplayMode.ItemLabels;
            foreach (DataRow row in table.Rows)
            {
                ChartSeriesItem item = new ChartSeriesItem();
                item.Name = (string)row["process_owner"];
                item.YValue = (int)row["Closed"];
                if (item.YValue == 0)
                {
                    item.Label.Visible = false;
                }
                item.Label.TextBlock.Text = (string)row["process_owner"] + " #%";
                series.Items.Add(item);
            } 
        }

    }

}