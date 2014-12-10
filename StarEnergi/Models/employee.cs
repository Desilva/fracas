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
    public partial class employee
    {
        public employee()
        {
            this.employee1 = new HashSet<employee>();
            this.rca_implementation = new HashSet<rca_implementation>();
            this.rca_pre_task = new HashSet<rca_pre_task>();
            this.users = new HashSet<user>();
        }
    
        public int id { get; set; }
        public string employee_no { get; set; }
        public string alpha_name { get; set; }
        public string position { get; set; }
        public string work_location { get; set; }
        public Nullable<int> employee_dept { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public Nullable<int> employee_boss { get; set; }
        public Nullable<int> dept_id { get; set; }
        public string email { get; set; }
        public string signature { get; set; }
        public Nullable<byte> delagate { get; set; }
        public Nullable<int> employee_delegate { get; set; }
        public string department { get; set; }
        public Nullable<byte> approval_level { get; set; }
        public string kbp_code { get; set; }
    
        public virtual ICollection<employee> employee1 { get; set; }
        public virtual employee employee2 { get; set; }
        public virtual employee_dept employee_dept1 { get; set; }
        public virtual ICollection<rca_implementation> rca_implementation { get; set; }
        public virtual ICollection<rca_pre_task> rca_pre_task { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
    
}
