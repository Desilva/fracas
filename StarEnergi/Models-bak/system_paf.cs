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
    public partial class system_paf
    {
        public system_paf()
        {
            tanggal = DateTime.Now;
            calculated_paf = 100;
            measured_paf = 100;
        }

        public int id { get; set; }
        public int id_system { get; set; }
        public System.DateTime tanggal { get; set; }
        public double measured_paf { get; set; }
        public double calculated_paf { get; set; }

        public virtual system system { get; set; }
    }
    
}