using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class DisciplineEntity
    {
        public int id { get; set; }
        public string title { get; set; }
        public int id_tag_type { get; set; }
    }
}