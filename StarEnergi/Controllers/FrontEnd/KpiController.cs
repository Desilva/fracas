using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StarEnergi.Models;
using System.Collections.Specialized;
using System.Diagnostics;
using StarEnergi.Utilities;

namespace StarEnergi.Controllers.FrontEnd
{
    public class KpiController : Controller
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public int CalculateKpi(KPIType type)
        {
            //kamus
            int result = 0;
            KpiDashboard dashboard = new KpiDashboard();
            string employeeId;

            //algoritma
            if (Session["id"] != null)
            {
                employeeId = Session["id"].ToString();
                switch (type)
                {
                    case KPIType.SHE_OBSERVATION:
                        result = dashboard.CalculateSheObservation(employeeId);
                        break;
                    case KPIType.IR_APPROVED:
                        result = dashboard.CalculateIRApproved(employeeId);
                        break;
                    case KPIType.IR_OUTSTANDING_APPROVAL:
                        result = dashboard.CalculateIROutstandingApproval(employeeId);
                        break;
                    case KPIType.IIR_APPROVED:
                        result = dashboard.CalculateIIRApproved(employeeId);
                        break;
                    case KPIType.IIR_OUTSTANDING_APPROVED:
                        result = dashboard.CalculateIIROutstandingApproved(employeeId);
                        break;
                    case KPIType.RCA:
                        result = dashboard.CalculateRCA(employeeId);
                        break;
                    case KPIType.PIR:
                        result = dashboard.CalculatePIR(employeeId);
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}
