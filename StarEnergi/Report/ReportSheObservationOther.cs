namespace StarEnergi.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Data.SqlClient;
    using System.Data;
    using Telerik.Reporting.Charting;
    using System.Configuration;

    /// <summary>
    /// Summary description for ReportSheObservation.
    /// </summary>
    public partial class ReportSheObservationOther : Telerik.Reporting.Report
    {
        public ReportSheObservationOther()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            //if (this.ReportParameters == null)
            //{
            //    this.subReport2.Visible = false;
            //}
            //else if ((int)this.ReportParameters[2].Value == 1)
            //{
            //    this.subReport1.Visible = false;
            //}
        }

        private void chart1_NeedDataSource(object sender, EventArgs e)
        {
            this.chart1.Series.RemoveSeries();
            this.chart1.PlotArea.XAxis.Appearance.ValueFormat = Telerik.Reporting.Charting.Styles.ChartValueFormat.General;
            this.chart1.PlotArea.XAxis.AutoScale = false;
            string sql = "";
            if (this.ReportParameters[2].Value == null)
            {
                this.subReport1.Visible = true;
                this.subReport2.Visible = false;
                this.subReport3.Visible = false;
                this.subReport4.Visible = false;
                this.subReport5.Visible = false;
                this.subReport6.Visible = false;
                this.subReport7.Visible = false;
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
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ")";
            }
            else if ((Int64)this.ReportParameters[2].Value == 1)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = true;
                this.subReport3.Visible = false;
                this.subReport4.Visible = false;
                this.subReport5.Visible = false;
                this.subReport6.Visible = false;
                this.subReport7.Visible = false;
                this.chart1.PlotArea.XAxis.AddRange(0, 3, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Berpotensi kecelakaan";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Berpotensi kebakaran/meledak";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "Pencegahan kecelakaan";
                this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "Pengawas berada dilokasi";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") WHERE num = 1 or num = 2 or num = 3 or num = 4";
            }
            else if ((Int64)this.ReportParameters[2].Value == 2)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = false;
                this.subReport3.Visible = true;
                this.subReport4.Visible = false;
                this.subReport5.Visible = false;
                this.subReport6.Visible = false;
                this.subReport7.Visible = false;
                this.chart1.PlotArea.XAxis.AddRange(0, 4, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Fit untuk kerja";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Masalah Psykologi";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "Ergonomi/posisi tubuh";
                this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "Bahan berbahaya ke tubuh";
                this.chart1.PlotArea.XAxis.Items[4].TextBlock.Text = "Kebersihan & kesehatan";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") WHERE num = 5 or num = 6 or num = 7 or num = 8 or num = 9";
            }
            else if ((Int64)this.ReportParameters[2].Value == 3)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = false;
                this.subReport3.Visible = false;
                this.subReport4.Visible = true;
                this.subReport5.Visible = false;
                this.subReport6.Visible = false;
                this.subReport7.Visible = false;
                this.chart1.PlotArea.XAxis.AddRange(0, 5, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Kerapihan & kebersihan";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Pemberian tanda";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "Berpotensi pencemaran";
                this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "Boros enerji";
                this.chart1.PlotArea.XAxis.Items[4].TextBlock.Text = "Pencegahan pencemaran";
                this.chart1.PlotArea.XAxis.Items[5].TextBlock.Text = "Ketersediaan alat penyerap";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 10 or num = 11 or num = 12 or num = 13 or num = 14 or num = 15";
            }
            else if ((Int64)this.ReportParameters[2].Value == 4)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = false;
                this.subReport3.Visible = false;
                this.subReport4.Visible = false;
                this.subReport5.Visible = true;
                this.subReport6.Visible = false;
                this.subReport7.Visible = false;
                this.chart1.PlotArea.XAxis.AddRange(0, 5, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Identifikasi bahaya & penanggulangan";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Dokumen ijin kerja";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "SOP- Petunjuk kerja";
                this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "MSDS – Material Safety Data Sheets";
                this.chart1.PlotArea.XAxis.Items[4].TextBlock.Text = "Siap tanggap darurat";
                this.chart1.PlotArea.XAxis.Items[5].TextBlock.Text = "sertifikat operator/peralatan";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 16 or num = 17 or num = 18 or num = 19 or num = 20 or num = 21";
            }
            else if ((Int64)this.ReportParameters[2].Value == 5)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = false;
                this.subReport3.Visible = false;
                this.subReport4.Visible = false;
                this.subReport5.Visible = false;
                this.subReport6.Visible = true;
                this.subReport7.Visible = false;
                this.chart1.PlotArea.XAxis.AddRange(0, 4, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "PPE – Alat Pelindung Diri";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Alat portabel";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "Mekanis";
                this.chart1.PlotArea.XAxis.Items[3].TextBlock.Text = "Listrik";
                this.chart1.PlotArea.XAxis.Items[4].TextBlock.Text = "kendaraan/alat berat";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 22 or num = 23 or num = 24 or num = 25 or num = 26";
            }
            else if ((Int64)this.ReportParameters[2].Value == 6)
            {
                this.subReport1.Visible = false;
                this.subReport2.Visible = false;
                this.subReport3.Visible = false;
                this.subReport4.Visible = false;
                this.subReport5.Visible = false;
                this.subReport6.Visible = false;
                this.subReport7.Visible = true;
                this.chart1.PlotArea.XAxis.AddRange(0, 2, 1);
                this.chart1.PlotArea.XAxis.Items[0].TextBlock.Text = "Fasilitas substandard";
                this.chart1.PlotArea.XAxis.Items[1].TextBlock.Text = "Paparan H2S";
                this.chart1.PlotArea.XAxis.Items[2].TextBlock.Text = "bahaya tempat kerja terhadap kesehatan";
                sql =
                    @"SELECT * from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 27 or num = 28 or num = 29";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["starenergygeo"].ToString();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            ChartSeries series = new ChartSeries();
            series.DataYColumn = "ua";
            series.Name = "UA";
            series.DefaultLabelValue = "";
            series.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            chart1.Series.Add(series);
            ChartSeries series2 = new ChartSeries();
            series2.DataYColumn = "uc";
            series2.Name = "UC";
            series2.DefaultLabelValue = "";
            series2.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            chart1.Series.Add(series2);
            ChartSeries series3 = new ChartSeries();
            series3.DataYColumn = "sa";
            series3.Name = "SA";
            series3.DefaultLabelValue = "";
            series3.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            chart1.Series.Add(series3);
            ChartSeries series4 = new ChartSeries();
            series4.DataYColumn = "sc";
            series4.Name = "SC";
            series4.DefaultLabelValue = "";
            series4.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            chart1.Series.Add(series4);
            chart1.ChartTitle.TextBlock.Text = "GRAFIK DATA " + this.ReportParameters[0].Value;
            (sender as Telerik.Reporting.Processing.Chart).DataSource = dataSet.Tables[0].DefaultView;
        }

        private void chart2_NeedDataSource(object sender, EventArgs e)
        {
            this.chart2.Series.RemoveSeries();
            this.chart2.PlotArea.XAxis.AutoScale = false;
            this.chart2.PlotArea.XAxis.AddRange(1, 1, 1);
            this.chart2.PlotArea.XAxis.Items[0].TextBlock.Text = "Kondisi";
            string sql = "";
            if (this.ReportParameters[2].Value == null)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ")";
            }
            else if ((Int64)this.ReportParameters[2].Value == 1)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") WHERE num = 1 or num = 2 or num = 3 or num = 4";
            }
            else if ((Int64)this.ReportParameters[2].Value == 2)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") WHERE num = 5 or num = 6 or num = 7 or num = 8 or num = 9";
            }
            else if ((Int64)this.ReportParameters[2].Value == 3)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 10 or num = 11 or num = 12 or num = 13 or num = 14 or num = 15";
            }
            else if ((Int64)this.ReportParameters[2].Value == 4)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 16 or num = 17 or num = 18 or num = 19 or num = 20 or num = 21";
            }
            else if ((Int64)this.ReportParameters[2].Value == 5)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 22 or num = 23 or num = 24 or num = 25 or num = 26";
            }
            else if ((Int64)this.ReportParameters[2].Value == 6)
            {
                sql =
                    @"SELECT sum(ua) as ua, sum(uc) as uc, sum(sa) as sa, sum(sc) as sc from observationChart12DepartmentObserver(" + (this.ReportParameters[0].Value == null ? "default" : "'" + ((string)this.ReportParameters[0].Value) + "'") + "," + (this.ReportParameters[1].Value == null ? "default" : "'" + ((string)this.ReportParameters[1].Value) + "'") + ") where num = 27 or num = 28 or num = 29";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["starenergygeo"].ToString();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            ChartSeries series = new ChartSeries();
            series.DataYColumn = "ua";
            series.Name = "UA";
            series.DefaultLabelValue = "UA,#Y";
            series.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            chart2.Series.Add(series);
            ChartSeries series2 = new ChartSeries();
            series2.DataYColumn = "uc";
            series2.Name = "UC";
            series2.DefaultLabelValue = "UC,#Y";
            series2.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            chart2.Series.Add(series2);
            ChartSeries series3 = new ChartSeries();
            series3.DataYColumn = "sa";
            series3.Name = "SA";
            series3.DefaultLabelValue = "SA,#Y";
            series3.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            chart2.Series.Add(series3);
            ChartSeries series4 = new ChartSeries();
            series4.DataYColumn = "sc";
            series4.Name = "SC";
            series4.DefaultLabelValue = "SC,#Y";
            series4.Appearance.FillStyle.MainColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            chart2.Series.Add(series4);
            chart2.ChartTitle.TextBlock.Text = "GRAFIK DATA " + this.ReportParameters[0].Value;
            (sender as Telerik.Reporting.Processing.Chart).DataSource = dataSet.Tables[0].DefaultView;
        }
    }
}