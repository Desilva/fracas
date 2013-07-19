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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public partial class daily_log_operation_warning
    {
        [ReadOnly(true)]
        public int id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public Nullable<System.DateTime> start_date { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public Nullable<System.DateTime> end_date { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string memo_direction { get; set; }

        [Required]
        public string action_required_by { get; set; }
        public string remark { get; set; }
        public Nullable<int> initiator { get; set; }

        [ReadOnly(true)]
        public string initiator_name { get; set; }
    }
    
}
