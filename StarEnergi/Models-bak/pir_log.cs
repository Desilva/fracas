//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace StarEnergi.Models
{
    public partial class pir_log
    {
        public int id { get; set; }
        public Nullable<int> id_pir { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string username { get; set; }
        public string process { get; set; }
        public string description { get; set; }
    
        public virtual pir pir { get; set; }
    }
    
}