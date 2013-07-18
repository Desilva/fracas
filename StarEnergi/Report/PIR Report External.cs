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
    public partial class PIR_Report_External : Telerik.Reporting.Report
    {
        public PIR_Report_External()
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
                select '4.2 Policy'
                UNION ALL
                select '4.3 Target & Programs'
                UNION ALL
                select '4.4 Operational Control'
                UNION ALL
                select '4.5 Monitoring'
                UNION ALL
                select '4.6 Management Review'
            )
            SELECT procedure_reference_list.proc_ref_list as procedure_reference, COUNT(pir.date_rise) as yeartodate
            from procedure_reference_list
            left join (select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3 ) as pir on procedure_reference_list.proc_ref_list = pir.procedure_reference group by proc_ref_list"
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
            SELECT 'Closed' AS [Status], COUNT(CASE WHEN pir.status = 'VERIFIED' THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3 UNION ALL SELECT 'Raised' AS [Status],COUNT(CASE WHEN pir.status != 'VERIFIED' THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3 UNION ALL SELECT 'Open' AS [Status], COUNT(CASE WHEN DateDiff(day,date_rise,GETDATE()) <=0 THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3 UNION ALL SELECT 'Overdue' AS [Status], COUNT(CASE WHEN DateDiff(day,date_rise,GETDATE()) > 0 THEN 1 END) as yeartodate FROM pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3"
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
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
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
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
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
            left join (Select * from pir where YEAR(date_rise) = '" + reportParamaters + "' and [from] = 3) as p on p.process_owner = proc_owner_list.ownlist group by ownlist"
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