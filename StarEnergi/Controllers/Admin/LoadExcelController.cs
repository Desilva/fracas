using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using StarEnergi.Utilities;
using System.Collections.Specialized;

namespace StarEnergi.Controllers.Admin
{
    public class LoadExcelController : Controller
    {
        //
        // GET: /LoadExcel/

        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult DataMaster()
        {
            return PartialView();
        }

        public ActionResult BOM()
        {
            return PartialView();
        }

        [HttpPost]
        public string Index(HttpPostedFileBase userfile)
        {
            // extract only the fielname
            var fileName = Path.GetFileName(userfile.FileName);
            string err = "";
            // store the file inside ~/App_Data/uploads folder
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);           
            userfile.SaveAs(path);
            if (fileName == Config.fileExcelSBSTemplate)
                err = SaveExcel(path);
            else if (fileName == Config.fileExcelDataMaster)
                err = SaveExcelDataMaster(path);
            else if (fileName == Config.fileExcelReadNav)
                err = SaveExcelReadNav(path);
            else if (fileName == Config.fileMA)
                err = SaveMA(path);
            else if (fileName == Config.filePAF)
                err = SavePAF(path);
            else if (fileName == Config.fileFracas)
                err = SaveFracas(path);
            else if (fileName == Config.fileIR)
                err = SaveIR(path);
            else if (fileName.Contains("Daily"))
                err = SaveEquipmentDailyReport(path);
            else if (fileName == Config.filePIR)
                err = SavePIR(path);
            else if (fileName == Config.fileExcelBOM)
                err = SaveBOM(path);
            else if (fileName == Config.fileExcelSafeManHours)
                err = SaveSafeMan(path);
            return err;
        }

        [HttpPost]
        public string SafeManHours(HttpPostedFileBase userfile)
        {
            // extract only the fielname
            var fileName = Path.GetFileName(userfile.FileName);
            string err = "";
            // store the file inside ~/App_Data/uploads folder
            var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
            userfile.SaveAs(path);
            err = SaveSafeMan(path);
            return err;
        }

        
        private string SaveExcel(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadSBS(path);
            return excel.generateError(err);
        }

        
        private string SaveExcelDataMaster(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadDataMaster(path);
            return excel.generateError(err);
        }

        private string SaveBOM(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadBOM(path);
            return excel.generateError(err);
        }

        
        private string SaveExcelReadNav(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadReadinessNavigator(path);
            return excel.generateError(err);
        }

        
        private string SaveMA(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadMA(path);
            return excel.generateError(err);
        }

        
        private string SavePAF(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadPlantAvailibility(path);
            return excel.generateError(err);
        }

        
        private string SaveFracas(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadFracas(path);
            return excel.generateError(err);
        }

        private string SaveIR(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadIR(path);
            return excel.generateError(err);
        }

        private string SaveEquipmentDailyReport(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadEquipmentDailyReport(path);
            return excel.generateError(err);
        }

        private string SavePIR(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadPIR(path);
            return excel.generateError(err);
        }

        private string SaveSafeMan(string path)
        {
            ExcelReader excel = new ExcelReader();
            List<string> err = excel.LoadSafeManHours(path);
            return excel.generateError(err);
        }

        public ActionResult Template()
        {
            // extract only the fielname
            NameValueCollection file = Request.Params;

            var fileName = file[0];
            // store the file inside ~/App_Data/uploads folder
            var path = Path.Combine(Server.MapPath("~/Content/template"), fileName);
            string[] filenameArray = fileName.Split('.');
            string contentType = "application/" + filenameArray[1];

            return File(path, contentType, fileName);
        }
    }
    
}
