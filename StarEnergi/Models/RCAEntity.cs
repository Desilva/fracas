using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StarEnergi.Models
{
    public class RCAEntityModel
    {
        public int id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string name { get; set; }

        public string description { get; set; }
        public int? id_type { get; set; }
        public string type_name { get; set; }
        public string cost { get; set; }
        public int? id_facility { get; set; }
        public string facility { get; set; }
        public int? id_division { get; set; }
        public string division { get; set; }
        public int? id_department { get; set; }
        public string department { get; set; }
        public int? id_building { get; set; }
        public string building { get; set; }
        public int? id_floor { get; set; }
        public string floor { get; set; }
        public string functional_location { get; set; }
        public int? id_type_equipment { get; set; }
        public string equipment_type { get; set; }
        public int? id_equipment_class { get; set; }
        public string equipment_class { get; set; }
        public string other { get; set; }
        public string manufacture { get; set; }
        public int? id_csf_connector { get; set; }
        public string charter { get; set; }
        public string comment { get; set; }
        public int? id_team { get; set; }
        public int identity { get; set; }
        public int isView { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime? start_date { get; set; }

        public DateTime? completion_date { get; set; }
        public byte? is_publish { get; set; }
        public string analysis_file { get; set; }
        public string equipment_code { get; set; }
        public byte? is_tree { get; set; }
        public string pir_number { get; set; }
        public Nullable<byte> has_pir { get; set; }
        public Nullable<byte> fracas_ir { get; set; }
        public int? fracas_ir_id { get; set; }
        public string rca_code { get; set; }
        public int? id_iir { get; set; }
        public DateTime? publish_date { get; set; }
        public bool is_implement { get; set; }
        public Nullable<System.DateTime> actual_start_date { get; set; }
        public Nullable<System.DateTime> due_date { get; set; }

        public List<string> member_name { get; set; }
    }

    public class RCAEntityModelExcel
    {
        public string rca_code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type_name { get; set; }
        public string cost { get; set; }
        public string facility { get; set; }
        public string division { get; set; }
        public string department { get; set; }
        public string functional_location { get; set; }
        public string equipment_type { get; set; }
        public string equipment_class { get; set; }
        public string equipment_code { get; set; }
        public string other { get; set; }
        public string manufacture { get; set; }
        public string charter { get; set; }
        public string comment { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? completion_date { get; set; }
    }

    public class RCAAnalysisTypeEntityModel
    {
        public int id { get; set; }
        public int name { get; set; }
    }

    public class RCATeamModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
        public string jabatan { get; set; }
        public int? role { get; set; }
        public int? employee_id { get; set; }
        public byte? rca_position { get; set; }
    }

    public class RCATeamEmployeeModel
    {
        public string username { get; set; }
        public string alpha_name { get; set; }
        public string position { get; set; }
        public int? role { get; set; }
        public int? employee_id { get; set; }
        public byte? rca_position { get; set; }
    }
}
