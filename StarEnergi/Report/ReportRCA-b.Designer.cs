namespace StarEnergi.Report
{
    partial class ReportRCA_b
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportRCA_b));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.panel22 = new Telerik.Reporting.Panel();
            this.panel23 = new Telerik.Reporting.Panel();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.pictureBox5 = new Telerik.Reporting.PictureBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox44 = new Telerik.Reporting.TextBox();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.textBox48 = new Telerik.Reporting.TextBox();
            this.textBox49 = new Telerik.Reporting.TextBox();
            this.panel15 = new Telerik.Reporting.Panel();
            this.panel7 = new Telerik.Reporting.Panel();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.pictureBox2 = new Telerik.Reporting.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "starenergygeo";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.SelectCommand = resources.GetString("sqlDataSource1.SelectCommand");
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(10.6899995803833D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel22,
            this.panel15});
            this.detail.Name = "detail";
            // 
            // panel22
            // 
            this.panel22.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel23,
            this.pictureBox5,
            this.textBox43,
            this.textBox44,
            this.textBox45,
            this.textBox46,
            this.textBox47,
            this.textBox48,
            this.textBox49});
            this.panel22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11041673272848129D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.panel22.Name = "panel22";
            this.panel22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.329999923706055D), Telerik.Reporting.Drawing.Unit.Inch(0.88999998569488525D));
            this.panel22.Style.BorderColor.Default = System.Drawing.Color.Orange;
            this.panel22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.panel22.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(2D);
            // 
            // panel23
            // 
            this.panel23.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox41,
            this.textBox42});
            this.panel23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.024266242980957D), Telerik.Reporting.Drawing.Unit.Inch(0.25343278050422668D));
            this.panel23.Name = "panel23";
            this.panel23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.565318107604981D), Telerik.Reporting.Drawing.Unit.Inch(0.618056058883667D));
            this.panel23.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("panel23.Style.BackgroundImage.ImageData")));
            this.panel23.Style.BackgroundImage.MimeType = "image/png";
            this.panel23.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            this.panel23.Style.Visible = true;
            // 
            // textBox41
            // 
            this.textBox41.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.47573375701904297D), Telerik.Reporting.Drawing.Unit.Inch(0.076388880610466D));
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6034715175628662D), Telerik.Reporting.Drawing.Unit.Inch(0.15412735939025879D));
            this.textBox41.Style.Font.Bold = true;
            this.textBox41.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox41.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox41.Value = "Title :";
            // 
            // textBox42
            // 
            this.textBox42.KeepTogether = false;
            this.textBox42.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.57999998331069946D), Telerik.Reporting.Drawing.Unit.Inch(0.25999999046325684D));
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.900001525878906D), Telerik.Reporting.Drawing.Unit.Inch(0.31531718373298645D));
            this.textBox42.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox42.Style.Font.Bold = true;
            this.textBox42.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox42.Value = "=Fields.name";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.0069444449618458748D));
            this.pictureBox5.MimeType = "";
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.009374737739563D), Telerik.Reporting.Drawing.Unit.Inch(0.859375D));
            this.pictureBox5.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox5.Style.BorderColor.Default = System.Drawing.Color.Orange;
            this.pictureBox5.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.pictureBox5.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.Solid;
            this.pictureBox5.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.pictureBox5.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.pictureBox5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.pictureBox5.Style.Color = System.Drawing.Color.Orange;
            this.pictureBox5.Value = "=ResolveUrl(\"~/Content/image/logo.png\")";
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.0218354463577271D), Telerik.Reporting.Drawing.Unit.Inch(0.053124964237213135D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(12.5677490234375D), Telerik.Reporting.Drawing.Unit.Inch(0.168016254901886D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox43.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox43.Value = "RCA Visualization";
            // 
            // textBox44
            // 
            this.textBox44.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(14.311420440673828D), Telerik.Reporting.Drawing.Unit.Inch(0.66909664869308472D));
            this.textBox44.Name = "textBox44";
            this.textBox44.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.94791650772094727D), Telerik.Reporting.Drawing.Unit.Inch(0.13055568933486939D));
            this.textBox44.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox44.Value = resources.GetString("textBox44.Value");
            // 
            // textBox45
            // 
            this.textBox45.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(13.689585685729981D), Telerik.Reporting.Drawing.Unit.Inch(0.6135406494140625D));
            this.textBox45.Name = "textBox45";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59575396776199341D), Telerik.Reporting.Drawing.Unit.Inch(0.227777898311615D));
            this.textBox45.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox45.Value = "Printed Date:";
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(13.689583778381348D), Telerik.Reporting.Drawing.Unit.Inch(0.1458333283662796D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59575396776199341D), Telerik.Reporting.Drawing.Unit.Inch(0.12361123412847519D));
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox46.Value = "Start Date:";
            // 
            // textBox47
            // 
            this.textBox47.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(14.320832252502441D), Telerik.Reporting.Drawing.Unit.Inch(0.1458333283662796D));
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93749982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.13055568933486939D));
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox47.Value = resources.GetString("textBox47.Value");
            // 
            // textBox48
            // 
            this.textBox48.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(14.320832252502441D), Telerik.Reporting.Drawing.Unit.Inch(0.3958333432674408D));
            this.textBox48.Name = "textBox48";
            this.textBox48.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93749982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.13055568933486939D));
            this.textBox48.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox48.Value = resources.GetString("textBox48.Value");
            // 
            // textBox49
            // 
            this.textBox49.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(13.689583778381348D), Telerik.Reporting.Drawing.Unit.Inch(0.3229166567325592D));
            this.textBox49.Name = "textBox49";
            this.textBox49.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59575396776199341D), Telerik.Reporting.Drawing.Unit.Inch(0.227777898311615D));
            this.textBox49.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(7D);
            this.textBox49.Value = "Completion Date:";
            // 
            // panel15
            // 
            this.panel15.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel7});
            this.panel15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(1.1104166507720947D));
            this.panel15.Name = "panel15";
            this.panel15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.340000152587891D), Telerik.Reporting.Drawing.Unit.Inch(9.4799995422363281D));
            this.panel15.Style.BorderColor.Default = System.Drawing.Color.Orange;
            this.panel15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.panel15.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(2D);
            // 
            // panel7
            // 
            this.panel7.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.pictureBox2});
            this.panel7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11041641235351563D), Telerik.Reporting.Drawing.Unit.Inch(0.1145833358168602D));
            this.panel7.Name = "panel7";
            this.panel7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.130000114440918D), Telerik.Reporting.Drawing.Unit.Inch(9.2700004577636719D));
            this.panel7.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("panel7.Style.BackgroundImage.ImageData")));
            this.panel7.Style.BackgroundImage.MimeType = "image/png";
            this.panel7.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            this.panel7.Style.BorderColor.Default = System.Drawing.Color.Transparent;
            this.panel7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.panel7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1899995803833D), Telerik.Reporting.Drawing.Unit.Inch(0.15000000596046448D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.29999995231628418D));
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(20D);
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox11.Value = "Strukturisasi Masalah";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1041666641831398D), Telerik.Reporting.Drawing.Unit.Inch(0.67500013113021851D));
            this.pictureBox2.MimeType = "";
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(14.930000305175781D), Telerik.Reporting.Drawing.Unit.Inch(8.5000009536743164D));
            this.pictureBox2.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox2.Value = "= IIf(Fields.analysis_file Is Null,ResolveUrl(\"~/Content/image/blank.png\"), Resol" +
    "veUrl(\"~/Content/full_image/images\"+Fields.id+\".png\"))";
            // 
            // ReportRCA_b
            // 
            this.DataSource = this.sqlDataSource1;
            this.Filters.AddRange(new Telerik.Reporting.Data.Filter[] {
            new Telerik.Reporting.Data.Filter("=Fields.id", Telerik.Reporting.Data.FilterOperator.Equal, "=Parameters.id.Value")});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "ReportRCA_b";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.sqlDataSource1;
            reportParameter1.AvailableValues.ValueMember = "= Fields.id";
            reportParameter1.Name = "id";
            reportParameter1.Text = "id";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.Integer;
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.535393714904785D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Panel panel22;
        private Telerik.Reporting.Panel panel23;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.PictureBox pictureBox5;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox44;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.TextBox textBox48;
        private Telerik.Reporting.TextBox textBox49;
        private Telerik.Reporting.Panel panel15;
        private Telerik.Reporting.Panel panel7;
        private Telerik.Reporting.TextBox textBox11;
        public Telerik.Reporting.PictureBox pictureBox2;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
    }
}