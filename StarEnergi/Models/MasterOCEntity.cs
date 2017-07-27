﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class MasterOCEntity
    {
        public int id { get; set; }
        [UIHint("Double")]
        public Nullable<double> oc_value { get; set; }
        public string oc_description { get; set; }
        [UIHint("Double")]
        public Nullable<double> oc_score { get; set; }
    }
}