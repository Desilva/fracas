using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class EmployeeEntity
    {
        public int id { get; set; }
        public string employee_no { get; set; }
        public string alpha_name { get; set; }
        public string position { get; set; }
        public string work_location { get; set; }
        public Nullable<System.DateTime> dob { get; set; }
        public string dept_name { get; set; }
        public string username { get; set; }
        public int level { get; set; }
        public int? dept_id { get; set; }
        public employee employee { get; set; }
        public List<user_per_role> role { get; set; }
        public byte? delagate { get; set; }
        public int? employee_delegate { get; set; }
        public byte? approval_level { get; set; }
    }
}