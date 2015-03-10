using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarEnergi.Utilities
{
    public enum KPIType
    {
        [Description("SHE Observation")]
        SHE_OBSERVATION,
        [Description("Incident Report Approved")]
        IR_APPROVED,
        [Description("Incident Report Outstanding Approval")]
        IR_OUTSTANDING_APPROVAL,
        [Description("RCA")]
        RCA,
        [Description("Incident Investigation Report Outstanding Approval")]
        IIR_OUTSTANDING_APPROVED,
        [Description("Incident Investigation Report Approved")]
        IIR_APPROVED,
        [Description("PIR")]
        PIR
    }

    public enum Day
    {
        SUNDAY = 0,
        MONDAY = 1,
        TUESDAY = 2,
        WEDNESDAY = 3,
        THURSDAY = 4,
        FRIDAY = 5,
        SATURDAY = 6,
    }
}
