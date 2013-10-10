namespace StarEnergi.Report
{
    partial class EachCertificate
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EachCertificate));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.panel1 = new Telerik.Reporting.Panel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(8.2399997711181641D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.detail.Name = "detail";
            this.detail.PageBreak = Telerik.Reporting.PageBreak.None;
            this.detail.Style.BackgroundColor = System.Drawing.Color.Orange;
            this.detail.Style.BackgroundImage.ImageData = null;
            this.detail.Style.BackgroundImage.MimeType = "";
            this.detail.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.2999608516693115D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.669921875D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox1.Style.Font.Name = "BlackChancery";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(25D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "= Fields.observer";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(3.9999606609344482D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.0000007152557373D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox2.Style.Font.Name = "AI kelso";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox2.Value = "= Fields.from + \" - \" + Fields.to";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(4.399960994720459D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.669921875D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox3.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox3.Style.Font.Name = "AI kelso";
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.Value = "= Iif(Fields.category = 0,\"Category Quality SHE Observation Report\",\"Category SHE" +
    " Observation Report\")";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5D), Telerik.Reporting.Drawing.Unit.Inch(4.899960994720459D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D));
            this.textBox4.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox4.Style.Font.Name = "AI kelso";
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(45D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "= Fields.reward";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0029551188927143812D), Telerik.Reporting.Drawing.Unit.Inch(6.6000003814697266D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.667003631591797D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox5.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox5.Style.Font.Name = "AI kelso";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Value = "= \'Pangalengan, \' + IsNull(Fields.drawing_date,\'\')";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(7.7000002861022949D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.669919013977051D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox6.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Name = "AI kelso";
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Value = "= Fields.alpha_name";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.5999608039855957D), Telerik.Reporting.Drawing.Unit.Inch(6.800079345703125D));
            this.pictureBox1.MimeType = "";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.89984196424484253D));
            this.pictureBox1.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pictureBox1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(41)))), ((int)(((byte)(42)))));
            this.pictureBox1.Value = "= ResolveUrl(\"~\" + Fields.signature)";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "starenergygeo";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@id", System.Data.DbType.String, "=Parameters.id.Value")});
            this.sqlDataSource1.SelectCommand = resources.GetString("sqlDataSource1.SelectCommand");
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox5,
            this.pictureBox1,
            this.textBox6,
            this.textBox4,
            this.textBox3,
            this.textBox2,
            this.textBox1});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(11.672874450683594D), Telerik.Reporting.Drawing.Unit.Inch(8.2399206161499023D));
            this.panel1.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("panel1.Style.BackgroundImage.ImageData")));
            this.panel1.Style.BackgroundImage.MimeType = "image/jpeg";
            // 
            // EachCertificate
            // 
            this.DataSource = this.sqlDataSource1;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail});
            this.Name = "EachCertificate";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Left = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Right = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.Margins.Top = Telerik.Reporting.Drawing.Unit.Inch(0.0099999997764825821D);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AutoRefresh = true;
            reportParameter1.Name = "id";
            reportParameter1.Text = "id";
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.Style.BackgroundImage.ImageData = null;
            this.Style.BackgroundImage.MimeType = "";
            this.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.672913551330566D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.SqlDataSource sqlDataSource1;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.Panel panel1;
    }
}