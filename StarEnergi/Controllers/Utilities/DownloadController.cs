using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Collections.Specialized;
using System.IO;

namespace StarEnergi.Controllers.Utilities
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        
        public ActionResult Download(int id)
        {
            int fid = Convert.ToInt32(id);
                
            string filename = (from o in db.equipments
                                where o.id == fid
                                select o.data_sheet_path).First();
                               
            string [] filenameArray = filename.Split('.');
            string contentType = "application/"+filenameArray[1];
            var physicalPath = Path.Combine(Config.EquipmentDataSheetFolder, filename);
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The connent type MIME type
            //3. The paraneter for the file save asked by the browser
            return File(physicalPath, contentType, filename);
            
        }


        public string[] name { get; set; }

        public int extension { get; set; }
    }
}
