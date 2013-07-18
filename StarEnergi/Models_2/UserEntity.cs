using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class UserEntity
    {
        public string username { get; set; }
        public string fullname { get; set; }
        public string password { get; set; }
        public string jabatan { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<int> rm_role { get; set; }
        public List<user_per_role> roles { get; set; }
        public string alpha_name { get; set; }
        public string position { get; set; }
        public Nullable<int> employee_id { get; set; }
    }
}