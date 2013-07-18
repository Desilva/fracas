using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class AuditLogEntity
    {
        public int id { get; set; }
        public Nullable<int> id_audit { get; set; }
        public Nullable<byte> grade { get; set; }
        public Nullable<byte> status { get; set; }
        public string finding { get; set; }
        public string requester { get; set; }
        public Nullable<int> id_pir { get; set; }
        public string pir_number { get; set; }
        public string process { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date { get; set; }
        public string reference { get; set; }
        public List<AuditLogClauseEntity> list_clauses { get; set; }
    }

    public class AuditLogClauseEntity
    {
        public int id { get; set; }
        public Nullable<int> id_audit_log { get; set; }
        public Nullable<int> id_pir_clause { get; set; }
        public string clause_no { get; set; }
    }

    public class PirClauseEntity
    {
        public int id { get; set; }
        public string clause_no { get; set; }
        public string clause { get; set; }
    }
}