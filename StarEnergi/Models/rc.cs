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
    public partial class rc
    {
        public rc()
        {
            this.systems = new HashSet<system>();
        }
    
        public int id { get; set; }
        public Nullable<double> rc_value { get; set; }
        public string rc_description { get; set; }
        public Nullable<double> rc_score { get; set; }
    
        public virtual ICollection<system> systems { get; set; }
    }
    
}