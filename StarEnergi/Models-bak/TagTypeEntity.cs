using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class TagTypeEntity
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string title { get; set; }
    }
}