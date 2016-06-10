using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class RCATemplateEntity
    {
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<byte> type { get; set; }
        public string type_name { get; set; }
    }
}