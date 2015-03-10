using StarEnergi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Utilities
{
    public class KpiDashboard
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        /// <summary>
        /// calculate KPI for SHE
        /// </summary>
        /// <returns></returns>
        public int CalculateSheObservation(string userIdStr)
        {
            //kamus
            int result = 0;
            string comparation;

            //algoritma
            comparation = "#" + userIdStr;
            result = db.she_observation.Where(m => m.observer.EndsWith(comparation) && (m.date_time == null ? true : m.date_time.Value.Year == DateTime.Now.Year)).Count();

            return result;
        }

        /// <summary>
        /// KPI for incident report approved
        /// role: user, supervisor, superintendent, facility manager
        /// </summary>
        /// <returns></returns>
        public int CalculateIRApproved(string userIdStr)
        {
            //kamus
            int result = 0;
            string comparation;
            employee employee;
            int userId;

            //algoritma
            userId = int.Parse(userIdStr);
            employee = db.employees.Where(m => m.id == userId).FirstOrDefault();

            if (employee != null)
            {
                comparation = "-" + DateTime.Now.Year + "-";

                //user biasa
                if ((employee.approval_level == null ? true : employee.approval_level.Value == 0) && !employee.position.Contains("Operation Manager"))
                {
                    result = db.incident_report.Where(m => m.prepared_by == userIdStr && m.reference_number.Contains(comparation)).Count();
                }
                //supervisor
                else if (employee.approval_level == null ? false : employee.approval_level == 1)
                {
                    result = db.incident_report.Where(m => ((m.ack_supervisor == userIdStr && m.supervisor_approve != null) || (m.loss_control == userIdStr && m.loss_control_approve != null)) && m.reference_number.Contains(comparation)).Count();
                }
                //superintendent & she superintendent
                else if (employee.approval_level == null ? false : employee.approval_level == 2)
                {
                    result = db.incident_report.Where(m => ((m.superintendent == userIdStr && m.superintendent_approve != null) || (m.she_superintendent == userIdStr && m.she_superintendent_approve != null)) && m.reference_number.Contains(comparation)).Count();
                }
                //operation manager
                else if (employee.position.Contains("Operation Manager"))
                {
                    result = db.incident_report.Where(m => m.field_manager == userIdStr && m.field_manager_approve != null && m.reference_number.Contains(comparation)).Count();
                }
            }

            return result;
        }

        public int CalculateIROutstandingApproval(string userIdStr)
        {
            //kamus
            int result = 0;
            string comparation;
            employee employee;
            int userId;

            //algoritma
            userId = int.Parse(userIdStr);
            employee = db.employees.Where(m => m.id == userId).FirstOrDefault();

            if (employee != null)
            {
                comparation = "-" + DateTime.Now.Year + "-";

                //supervisor
                if (employee.approval_level == null ? false : employee.approval_level == 1)
                {
                    result = db.incident_report.Where(m => ((m.ack_supervisor == userIdStr && m.supervisor_approve == null) || (m.loss_control == userIdStr && m.loss_control_approve == null)) && m.reference_number.Contains(comparation)).Count();
                }
                //superintendent & she superintendent
                else if (employee.approval_level == null ? false : employee.approval_level == 2)
                {
                    result = db.incident_report.Where(m => ((m.superintendent == userIdStr && m.superintendent_approve == null) || (m.she_superintendent == userIdStr && m.she_superintendent_approve == null)) && m.reference_number.Contains(comparation)).Count();
                }
                //operation manager
                else if (employee.position.Contains("Operation Manager"))
                {
                    result = db.incident_report.Where(m => m.field_manager == userIdStr && m.field_manager_approve == null && m.reference_number.Contains(comparation)).Count();
                }
            }

            return result;
        }

        public int CalculateIIRApproved(string userIdStr)
        {
            //kamus
            int result = 0;
            string comparation;
            employee employee;
            int userId;

            //algoritma
            userId = int.Parse(userIdStr);
            employee = db.employees.Where(m => m.id == userId).FirstOrDefault();

            if (employee != null)
            {
                comparation = "-" + DateTime.Now.Year + "-";

                //supervisor
                if (employee.approval_level == null ? false : employee.approval_level == 1)
                {
                    //result = db.investigation_report.Where(m => m.investigator.Split(';').ToList().Contains(userIdStr) && m.investigator_approve != null && m.reference_number.Contains(comparation)).Count();
                    result = db.investigation_report.Where(m => m.investigator_approve != null && m.reference_number.Contains(comparation)).AsEnumerable()
                        .Where(row => row.investigator.Split(';').ToList()
                            .Where(l => l.Contains(userIdStr)).Any()).Count();
                }
                //superintendent & she superintendent
                else if (employee.approval_level == null ? false : employee.approval_level == 2)
                {
                    //user as she superintendent
                    result = db.investigation_report.Where(m => m.safety_officer == userIdStr && m.safety_officer_approve != null && m.reference_number.Contains(comparation)).Count();
                    //user as superintendent & not she superintendent
                    result += db.investigation_report.Where(m => m.safety_officer != userIdStr && m.investigator_approve != null && m.reference_number.Contains(comparation)).AsEnumerable()
                           .Where(row => row.investigator.Split(';').ToList()
                               .Where(l => l.Contains(userIdStr)).Any()).Count();
                }
                //operation / field manager
                else if (employee.position.Contains("Operation Manager"))
                {
                    result = db.investigation_report.Where(m => m.field_manager == userIdStr && m.field_manager_approve != null && m.reference_number.Contains(comparation)).Count();
                }
            }

            return result;
        }

        public int CalculateIIROutstandingApproved(string userIdStr)
        {
            //kamus
            int result = 0;
            string comparation;
            employee employee;
            int userId;

            //algoritma
            userId = int.Parse(userIdStr);
            employee = db.employees.Where(m => m.id == userId).FirstOrDefault();

            if (employee != null)
            {
                comparation = "-" + DateTime.Now.Year + "-";

                //supervisor
                if (employee.approval_level == null ? false : employee.approval_level == 1)
                {
                    //result = db.investigation_report.Where(m => m.investigator.Split(';').ToList().Contains(userIdStr) && m.investigator_approve != null && m.reference_number.Contains(comparation)).Count();
                    result = db.investigation_report.Where(m => m.investigator_approve == null && m.reference_number.Contains(comparation)).AsEnumerable()
                        .Where(row => row.investigator.Split(';').ToList()
                            .Where(l => l.Contains(userIdStr)).Any()).Count();
                }
                //superintendent & she superintendent
                else if (employee.approval_level == null ? false : employee.approval_level == 2)
                {
                    //user as she superintendent
                    result = db.investigation_report.Where(m => m.safety_officer == userIdStr && m.safety_officer_approve == null && m.reference_number.Contains(comparation)).Count();
                    //user as superintendent & not she superintendent
                    result += db.investigation_report.Where(m => m.safety_officer != userIdStr && m.investigator_approve == null && m.reference_number.Contains(comparation)).AsEnumerable()
                           .Where(row => row.investigator.Split(';').ToList()
                               .Where(l => l.Contains(userIdStr)).Any()).Count();
                }
                //operation manager
                else if (employee.position.Contains("Operation Manager"))
                {
                    result = db.investigation_report.Where(m => m.field_manager == userIdStr && m.field_manager_approve == null && m.reference_number.Contains(comparation)).Count();
                }
            }

            return result;
        }

        public int CalculateRCA(string userIdStr)
        {
            //kamus
            int result = 0;
            user user;
            int userId;

            //algoritma
            userId = int.Parse(userIdStr);
            user = db.employees.Where(m => m.id == userId).Select(m => m.users.FirstOrDefault()).FirstOrDefault();

            if (user != null)
            {
                result = db.rca_team_connector.Where(m => m.id_user == user.username && (m.rca.start_date == null ? false : m.rca.start_date.Value.Year == DateTime.Now.Year)).Select(m => m.id_rca).Distinct().Count();
            }

            return result;
        }

        public int CalculatePIR(string userIdStr)
        {
            //kamus
            int result = 0;
            int userId;
            string comparation;

            //algoritma
            userId = int.Parse(userIdStr);
            comparation = "-" + DateTime.Now.Year + "-";

            result = db.pirs.Where(m => (m.process_user == null ? false : m.process_user.Value == userId) && (m.no == null ? false : m.no.Contains(comparation))).Count();

            return result;
        }

        public bool Allow(string userIdStr, KPIType type)
        {
            //kamus
            bool result = true;
            int userId = int.Parse(userIdStr);
            employee employee = db.employees.Where(m => m.id == userId).FirstOrDefault();

            //algoritma
            if (employee.approval_level == 1) //supervisor
            {
            }
            else if (employee.approval_level == 2) //superintendent
            {
            }
            else if (employee.position.Contains("Operation Manager")) //operation / field manager
            {
                if (!(type == KPIType.SHE_OBSERVATION || type == KPIType.IR_APPROVED || type == KPIType.IR_OUTSTANDING_APPROVAL || type == KPIType.IIR_APPROVED || type == KPIType.IIR_OUTSTANDING_APPROVED))
                    result = false;
            }
            else //user biasa
            {
                if (!(type == KPIType.SHE_OBSERVATION || type == KPIType.IR_APPROVED))
                    result = false;
            }

            return result;
        }
    }
}