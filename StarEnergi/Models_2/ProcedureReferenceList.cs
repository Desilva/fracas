using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class ProcedureReferenceList
    {
        public Dictionary<string, string> procedure_reference { get; set; }

        public ProcedureReferenceList()  //constructor
        {
            procedure_reference = new Dictionary<string, string>()
            {
                { "4.2 Policy", "4.2 Policy"},  
                { "4.3 Target & Programs", "4.3 Target & Programs"},    
                { "4.4 Operational Control", "4.4 Operational Control"},  
                { "4.5 Monitoring", "4.5 Monitoring"},
                { "4.6 Management Review", "4.6 Management Review"},
            };

        }
    }
}