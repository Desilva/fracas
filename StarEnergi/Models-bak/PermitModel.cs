using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class PermitDeparmentModel
    {
        public int id { get; set; }
        public string department { get; set; }
    }

    public class PermitHolderNoModel
    {
        public int id { get; set; }
        [UIHint("EmployeePTW")]
        public string id_employee { get; set; }
        public string ptw_holder_no { get; set; }
        [DataType(DataType.Date)]
        public Nullable<DateTime> activated_date_until { get; set; }
    }

    public class PermitSectionModel
    {
        public int id { get; set; }
        public string section { get; set; }
    }

    public class PermitFOModel
    {
        public int id { get; set; }
        [UIHint("EmployeePTW")]
        public string id_employee { get; set; }
        public string fo_code { get; set; }
    }

    public class PermitAssessorModel
    {
        public int id { get; set; }
        [UIHint("EmployeePTW")]
        public string id_employee { get; set; }
        public string assessor_code { get; set; }
    }
}