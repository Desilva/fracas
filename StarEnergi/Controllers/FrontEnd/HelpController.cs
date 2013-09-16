using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HelpContent(string type)
        {
            ViewBag.type = type;
            return PartialView();
        }

        public ActionResult GetContent(string type)
        {
            string file = "";
            if (type == "Fracas_Event_Log")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Historical_Data")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Incident_Report")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Incident_Investigation_Report")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Injury_Report")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if(type = "Audit")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Observation_Form")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Daily_Log")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Equipt._Report")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "Asset_Register")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "RCA")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            else if (type = "PIR")
                file = Server.MapPath(Url.Content("~/Content/Help/road-map.pdf"));
            PdfReader reader = new PdfReader(file);
            MemoryStream pdfStream = new MemoryStream();

            PdfStamper pdfStamper = new PdfStamper(reader, pdfStream);

            reader.Close();
            pdfStamper.Close();
            pdfStream.Flush();
            pdfStream.Close();

            byte[] pdfArray = pdfStream.ToArray();
            return new BinaryContentResult(pdfArray, "application/pdf");
        }
    }
}
