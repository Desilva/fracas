using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class ProcessOwnerList
    {
        public Dictionary<string, string> process_owner { get; set; }

        public ProcessOwnerList()
        {
            process_owner = new Dictionary<string, string>()
            {
                { "BPL", "BPL"},  
                { "GEL", "GEL"},    
                { "GDI", "GDI"},  
                { "POP", "POP"},
                { "MTW", "MTW"},
                { "SPE", "SPE"},
                { "OHE", "OHE"},
                { "EPE", "EPE"},
                { "EAI", "EAI"},
                { "OSU", "OSU"},
                { "LCO", "LCO"},
                { "SSU", "SSU"},
                { "MER", "MER"},
                { "SEC", "SEC"},
                { "SCM", "SCM"},
                { "MIS", "MIS"},
                { "FPR", "FPR"},
                { "PAC", "PAC"},
                { "FHR", "FHR"},
            };
        }
    }
}