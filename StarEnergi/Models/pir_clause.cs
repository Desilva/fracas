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
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public partial class pir_clause
    {
        public pir_clause()
        {
            this.audit_log_clause = new HashSet<audit_log_clause>();
        }
    
        public int id { get; set; }

        [Required]
        public string clause_no { get; set; }

        [Required]
        public string clause { get; set; }
    
        public virtual ICollection<audit_log_clause> audit_log_clause { get; set; }
    }
    
}
