using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class FailureModeEntity
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string title { get; set; }
        public string description { get; set; }

        [Required]
        public Nullable<int> id_tag_type { get; set; }
        public string tag_types_name { get; set; }
    }
}