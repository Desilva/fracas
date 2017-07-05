using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class OCREntity
    {
        public int id { get; set; }        
        public double? ocr_value { get; set; }
        public string ocr_description { get; set; }
        public double? ocr_score { get; set; }
        public string nama { get; set; }
    }
}