using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class BomEquipmentEntity
    {
        public int id { get; set; }        
        public int id_bom { get; set; }
        public string functional_location { get; set; }
        public string no_keymap { get; set; }
    }
}