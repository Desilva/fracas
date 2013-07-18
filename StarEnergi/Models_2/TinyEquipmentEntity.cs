using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class TinyEquipmentEntity
    {
        public TinyEquipmentEntity() {
            EH = 0;
            H = 0;
            MH = 0;
            M = 0;
            LN = 0;
        }

        public int id { get; set; }
        public Nullable<byte> id_category { get; set; }
        public string tag_num { get; set; }
        public string nama { get; set; }
        public string pdf { get; set; }
        public Nullable<double> tetha { get; set; }
        public Nullable<double> beta { get; set; }
        public Nullable<double> mean { get; set; }
        public Nullable<double> varian { get; set; }
        public Nullable<double> lamda { get; set; }
        public Nullable<int> id_discipline { get; set; }
        public Nullable<int> mtbf { get; set; }
        public Nullable<int> mttr { get; set; }
        public Nullable<int> mdt { get; set; }
        public Nullable<byte> status { get; set; }
        public string method { get; set; }
        public Nullable<int> econ { get; set; }
        public string ram_crit { get; set; }
        public double running_hours { get; set; }
        public Nullable<System.DateTime> installed_date { get; set; }
        public Nullable<System.DateTime> obsolete_date { get; set; }
        public Nullable<int> warranty { get; set; }
        public string vendor { get; set; }
        public string data_sheet_path { get; set; }
        public Nullable<System.DateTime> sertifikasi { get; set; }
        public double avail_inherent { get; set; }
        public double avail_operational { get; set; }
        public double remaining_hours { get; set; }
        public string equipment_group { get; set; }
        
        public int EH { get; set; }
        public int H { get; set; }
        public int MH { get; set; }
        public int M { get; set; }
        public int LN { get; set; }

        public void setCriticality() {
            if (ram_crit == "EH")
            {
                EH = 1;
            }
            else if (ram_crit == "H"){
                H = 1;
            }
            else if (ram_crit == "MH")
            {
                MH = 1;
            }
            else if (ram_crit == "M")
            {
                M = 1;
            }
            else if (ram_crit == "LN")
            {
                LN = 1;
            }
        }
    }
}