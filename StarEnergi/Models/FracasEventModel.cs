using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class FracasEventModel
    {
        public int id { get;set; }
        public int idEquipment { get; set; }
        public int? idEquipmentPart { get; set; }
        public string tagNumber { get; set; }
        public DateTime? dateTimeStop { get; set; }
        public DateTime? dateTimeStart { get; set; }
        public string unitName { get; set; }
        public string areaName { get; set; }
        public double? durasi { get; set; }
        public double? downtime { get; set; }

        public string failureMode { get; set; }
        public string eventDesc { get; set; }
        public string cause { get; set; }
        public string immediateAction { get; set; }
        public string longTermAction { get; set; }
        public string relatedItem { get; set; }

        public string failureEffect { get; set; }
        public string secondaryEffect { get; set; }
        public string failureSeverity { get; set; }
        public string failureClss { get; set; }
        public string engineer { get; set; }
        public Nullable<double> financialCost { get; set; }
        public Nullable<double> repairCost { get; set; }
        public string type { get; set; }
        public byte? status { get; set; }

        public string name { get; set; }

        public int part { get; set; }//0 equipment 1 part
    }
}