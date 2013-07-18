namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
using System.Data.SqlClient;
using Telerik.Reporting.Charting;
using System.Data;

    /// <summary>
    /// Summary description for Reporttest.
    /// </summary>
    public partial class Reporttest : Telerik.Reporting.Report
    {
        public Reporttest()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        private void chart1_NeedDataSource(object sender, EventArgs e)
        {
            string sql =
                @"SELECT * from observationChart12('2013')";
            string connectionString =
              "Data Source=SEPTU-PC\\SQLEXPRESS;Initial Catalog=star_energi_geo;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            ChartSeries series = new ChartSeries();
            series.DataYColumn = "ua";
            series.DataLabelsColumn = "name";
            chart1.Series.Add(series);
            (sender as Telerik.Reporting.Processing.Chart).DataSource = dataSet.Tables[0].DefaultView;
        } 
    }
}