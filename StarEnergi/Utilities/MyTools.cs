using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace StarEnergi.Utilities
{
    public static class MyTools
    {
        public static ActionResult importExcel(object model ,string caption, string[] title) {

            GridView gv = new GridView();
            gv.Caption = caption;
            gv.DataSource = model;
            gv.DataBind();
            int i = 0;
            foreach (string s in title) {
                gv.HeaderRow.Cells[i].Text = s;
                i++;
            }

            if (gv != null)
            {
                return new DownloadFileActionResult(gv, (caption+".xls"));
            }
            else
            {
                return new JavaScriptResult();
            }
        }

        public static string GetOperationManager()
        {
            string name = "";
            relmon_star_energiEntities db = new relmon_star_energiEntities();
            var employee = db.employees.Where(x => x.employee_dept == 21 && x.employee_boss == null && (x.signature != null || x.signature != "")).FirstOrDefault();
            if(employee != null)
            {
                name = employee.alpha_name;
            }
            return name;
        }

        public static string generatePirNumber(string number) {
            string ret = "";

            if (number == "") {
                return Config.pirNumberFormat + "0001";
            }

            string[] parse = number.Split('-');

            int x = int.Parse(parse.Last());
            int year = int.Parse(parse.ElementAt(4));

            if (year != DateTime.Today.Year)
            {
                x = 0;
            }
            x++;
            ret = Config.pirNumberFormat;
            ret += x.ToString().PadLeft(4, '0');


            //switch ((x++).ToString().Length){
            //    case 1:
            //        ret += "0000" + (x++);
            //        break;
            //    case 2:
            //        ret += "000" + (x++);
            //        break;
            //    case 3:
            //        ret += "00" + (x++);
            //        break;
            //    case 4:
            //        ret += "0" + (x++);
            //        break;
            //    case 5:
            //        ret += (x++).ToString();
            //        break;
            //}

            return ret;
        }
    }
}