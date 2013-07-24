using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi
{
    public static class Config
    {

        public static string conectionString = "Data Source=(local);Initial Catalog=star_energi_geo;Integrated Security=True";
        public static string EquipmentDataSheetFolder = "C:\\upload";
        public static string Title = "WW-FRACAS Application";
        public static string Area = "Wayang Windu";
        public static string currency = "$";
        public static string defaultDasboard = "ma";
        public static string defaultIdDasboard = "2";
        public enum TreeType
        {
            PLANT,
            AREA,
            UNIT,
            SYSTEM,
            EQUIPMENT_GROUP,
            EQUIPMENTS,
            PART,
            COMPONENT,
            SUBCOMPONENT
        };

        public enum role
        {
            ADMIN,
            FRACAS,
            RCA,
            RCAVIEW,
            FULLPIR,
            PIRINITIATOR,
            PIRPROCESS,
            AUDITOR,
            IIR,
            INITIATORIR,
            ADMINSHEOBSERVATION,
            ADMINMASTERSHE,
            MEDIC,
            DAILYLOG,
            DAILYLOGWEEKLYTARGET,
            DAILYLOGLEADER,
            DAILYLOGSUPERVISOR
        };

        public enum MenuFrontEnd
        {
            DASHBOARD,
            EQRELCHAR,
            PLANTAVAILBILITY,
            READINESSNAVIGATOR,
            ECA,
            FSCA,
            ASSETREGISTER,
            FRACAS
        };

        public static string[] Month = { 
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "Mei",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Okt",
            "Nov",
            "Des"
        };


        public static MenuFrontEnd menu;

        public static int getDaysOfTheMonth(int month)
        {
            int days = 30;
            if (month == 1)
            {
                days = 31;
            }
            else if (month == 2)
            {
                days = 28;
            }
            else if (month == 3)
            {
                days = 31;
            }
            else if (month == 4)
            {
                days = 30;
            }
            else if (month == 5)
            {
                days = 31;
            }
            else if (month == 6)
            {
                days = 30;
            }
            else if (month == 7)
            {
                days = 31;
            }
            else if (month == 8)
            {
                days = 31;
            }
            else if (month == 9)
            {
                days = 30;
            }
            else if (month == 10)
            {
                days = 31;
            }
            else if (month == 11)
            {
                days = 30;
            }
            else if (month == 12)
            {
                days = 31;
            }
            return days;
        }

        public static string convertMonth(int month)
        {
            string sMonth = "";
            if (month == 1)
            {
                sMonth = "Jan";
            }
            else if (month == 2)
            {
                sMonth = "Feb";
            }
            else if (month == 3)
            {
                sMonth = "Mar";
            }
            else if (month == 4)
            {
                sMonth = "Apr";
            }
            else if (month == 5)
            {
                sMonth = "Mei";
            }
            else if (month == 6)
            {
                sMonth = "Jun";
            }
            else if (month == 7)
            {
                sMonth = "Jul";
            }
            else if (month == 8)
            {
                sMonth = "Aug";
            }
            else if (month == 9)
            {
                sMonth = "Sep";
            }
            else if (month == 10)
            {
                sMonth = "Okt";
            }
            else if (month == 11)
            {
                sMonth = "Nov";
            }
            else if (month == 12)
            {
                sMonth = "Dec";
            }
            return sMonth;
        }

        public static double GetGamma(string n)
        {
            double gamma = 1.00000;
            switch (n)
            {
                case "1.00":
                    gamma = 1.00000;
                    break;
                case "1.01":
                    gamma = 0.99433;
                    break;
                case "1.02":
                    gamma = 0.98884;
                    break;
                case "1.03":
                    gamma = 0.98355;
                    break;
                case "1.04":
                    gamma = 0.97844;
                    break;
                case "1.05":
                    gamma = 0.97350;
                    break;
                case "1.06":
                    gamma = 0.96874;
                    break;
                case "1.07":
                    gamma = 0.96415;
                    break;
                case "1.08":
                    gamma = 0.95973;
                    break;
                case "1.09":
                    gamma = 0.95546;
                    break;
                case "1.10":
                    gamma = 0.95135;
                    break;
                case "1.11":
                    gamma = 0.94739;
                    break;
                case "1.12":
                    gamma = 0.94359;
                    break;
                case "1.13":
                    gamma = 0.93993;
                    break;
                case "1.14":
                    gamma = 0.93642;
                    break;
                case "1.15":
                    gamma = 0.93304;
                    break;
                case "1.16":
                    gamma = 0.92980;
                    break;
                case "1.17":
                    gamma = 0.92670;
                    break;
                case "1.18":
                    gamma = 0.92373;
                    break;
                case "1.19":
                    gamma = 0.92088;
                    break;
                case "1.20":
                    gamma = 0.91817;
                    break;
                case "1.21":
                    gamma = 0.91558;
                    break;
                case "1.22":
                    gamma = 0.91311;
                    break;
                case "1.23":
                    gamma = 0.91075;
                    break;
                case "1.24":
                    gamma = 0.90852;
                    break;
                case "1.25":
                    gamma = 0.90640;
                    break;
                case "1.26":
                    gamma = 0.90440;
                    break;
                case "1.27":
                    gamma = 0.90250;
                    break;
                case "1.28":
                    gamma = 0.90072;
                    break;
                case "1.29":
                    gamma = 0.89904;
                    break;
                case "1.30":
                    gamma = 0.89747;
                    break;
                case "1.31":
                    gamma = 0.89600;
                    break;
                case "1.32":
                    gamma = 0.89464;
                    break;
                case "1.33":
                    gamma = 0.89338;
                    break;
                case "1.34":
                    gamma = 0.89222;
                    break;
                case "1.35":
                    gamma = 0.89115;
                    break;
                case "1.36":
                    gamma = 0.89018;
                    break;
                case "1.37":
                    gamma = 0.88931;
                    break;
                case "1.38":
                    gamma = 0.88854;
                    break;
                case "1.39":
                    gamma = 0.88785;
                    break;
                case "1.40":
                    gamma = 0.88726;
                    break;
                case "1.41":
                    gamma = 0.88676;
                    break;
                case "1.42":
                    gamma = 0.88636;
                    break;
                case "1.43":
                    gamma = 0.88604;
                    break;
                case "1.44":
                    gamma = 0.88580;
                    break;
                case "1.45":
                    gamma = 0.88565;
                    break;
                case "1.46":
                    gamma = 0.88560;
                    break;
                case "1.47":
                    gamma = 0.88563;
                    break;
                case "1.48":
                    gamma = 0.88575;
                    break;
                case "1.49":
                    gamma = 0.88595;
                    break;
                case "1.50":
                    gamma = 0.88623;
                    break;
                case "1.51":
                    gamma = 0.88659;
                    break;
                case "1.52":
                    gamma = 0.88704;
                    break;
                case "1.53":
                    gamma = 0.88757;
                    break;
                case "1.54":
                    gamma = 0.88818;
                    break;
                case "1.55":
                    gamma = 0.88887;
                    break;
                case "1.56":
                    gamma = 0.88964;
                    break;
                case "1.57":
                    gamma = 0.89049;
                    break;
                case "1.58":
                    gamma = 0.89142;
                    break;
                case "1.59":
                    gamma = 0.89243;
                    break;
                case "1.60":
                    gamma = 0.89352;
                    break;
                case "1.61":
                    gamma = 0.89468;
                    break;
                case "1.62":
                    gamma = 0.89592;
                    break;
                case "1.63":
                    gamma = 0.89724;
                    break;
                case "1.64":
                    gamma = 0.89864;
                    break;
                case "1.65":
                    gamma = 0.90012;
                    break;
                case "1.66":
                    gamma = 0.90167;
                    break;
                case "1.67":
                    gamma = 0.90330;
                    break;
                case "1.68":
                    gamma = 0.90500;
                    break;
                case "1.69":
                    gamma = 0.90678;
                    break;
                case "1.70":
                    gamma = 0.90864;
                    break;
                case "1.71":
                    gamma = 0.91057;
                    break;
                case "1.72":
                    gamma = 0.91258;
                    break;
                case "1.73":
                    gamma = 0.91466;
                    break;
                case "1.74":
                    gamma = 0.91683;
                    break;
                case "1.75":
                    gamma = 0.91906;
                    break;
                case "1.76":
                    gamma = 0.92137;
                    break;
                case "1.77":
                    gamma = 0.92376;
                    break;
                case "1.78":
                    gamma = 0.92623;
                    break;
                case "1.79":
                    gamma = 0.92877;
                    break;
                case "1.80":
                    gamma = 0.93138;
                    break;
                case "1.81":
                    gamma = 0.93408;
                    break;
                case "1.82":
                    gamma = 0.93685;
                    break;
                case "1.83":
                    gamma = 0.93969;
                    break;
                case "1.84":
                    gamma = 0.94261;
                    break;
                case "1.85":
                    gamma = 0.94561;
                    break;
                case "1.86":
                    gamma = 0.94869;
                    break;
                case "1.87":
                    gamma = 0.95184;
                    break;
                case "1.88":
                    gamma = 0.95507;
                    break;
                case "1.89":
                    gamma = 0.95838;
                    break;
                case "1.90":
                    gamma = 0.96177;
                    break;
                case "1.91":
                    gamma = 0.96523;
                    break;
                case "1.92":
                    gamma = 0.96878;
                    break;
                case "1.93":
                    gamma = 0.97240;
                    break;
                case "1.94":
                    gamma = 0.97610;
                    break;
                case "1.95":
                    gamma = 0.97988;
                    break;
                case "1.96":
                    gamma = 0.98374;
                    break;
                case "1.97":
                    gamma = 0.98768;
                    break;
                case "1.98":
                    gamma = 0.99171;
                    break;
                case "1.99":
                    gamma = 0.99581;
                    break;
                case "2.00":
                    gamma = 1.00000;
                    break;
                default:
                    gamma = 1.00000;
                    break;
            }
            return gamma;
        }

        //maximum value availibility
        public const double GREEN = 100;
        public const double YELLOW = 90;
        public const double RED = 60;

        //color BEC read nav
        public const double BEC = 50;

        public static string fileExcelSBSTemplate = "SBS_template.xls";
        public static string fileExcelDataMaster = "Data_Master_Template.xls";
        public static string fileExcelReadNav = "Readiness_Navigator.xls";
        public static string fileMA = "MA_template.xls";
        public static string filePAF = "PAF_POF_template.xls";
        public static string fileFracas = "Fracas_template.xls";
        public static string fileDailyLogDay = "DailyLogDay_template.xls";
        public static string fileDailyLogNight = "DailyLogNight_template.xls";
        public static string pirNumberFormat = "W-O-SPE-PIR-" + DateTime.Now.Year + "-";
        public static string fileIR = "IR_template.xls";
        public static string filePIR = "PIR_template.xls";
    }
}