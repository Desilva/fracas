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
        [UIHint("BomDropDownList")]
        public string functional_location { get; set; }
        [UIHint("NoKeyMap")]
        public string no_keymap { get; set; }
        [UIHint("Remark")]
        public string remark { get; set; }
    }
}