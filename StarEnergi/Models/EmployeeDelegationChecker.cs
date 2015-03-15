using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class EmployeeDelegationChecker
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();
        private List<weekend_duty> weekendDuties;
        private employee employee;

        public EmployeeDelegationChecker()
        {
            weekendDuties = db.weekend_duty.Where(p => p.start_date.CompareTo(DateTime.Today) <= 0 && p.end_date.CompareTo(DateTime.Today) >= 0).ToList();
        }

        public EmployeeDelegationChecker(employee employee) : this()
        {
            this.employee = employee;
        }

        public employee Employee
        {
            get
            {
                return employee;
            }
            set
            {
                employee = value;
            }
        }

        public bool isDelegateTo(employee employeeCheck)
        {
            if (employee.employee_delegate == employeeCheck.id)
            {
                return true;
            }
            else
            {
                bool isDelegate = false;
                List<weekend_duty> weekendDutyCheck = weekendDuties.Where(p => p.delegate_id == employeeCheck.id && p.department == employee.department.ToUpper()).ToList();
                int length = weekendDutyCheck.Count;
                employee.department = employee.department == null ? "" : employee.department;
                for (int i = 0; i < length && isDelegate != true; i++)
                {
                    weekend_duty weekendDuty = weekendDutyCheck.ElementAt(i);
                    if (weekendDuty.delegate_id == employeeCheck.id && employee.department.ToUpper() == weekendDuty.department && employee.department.ToUpper() == employeeCheck.department.ToUpper())
                    {
                        isDelegate = true;
                    }
                }
                return isDelegate;
            }
        }

        public void setDelegate(employee employee, employee employeeCheck)
        {
            if (employee.department == null)
            {
                employee.department = "";
            }
            List<weekend_duty> weekendDutyCheck = weekendDuties.Where(p => p.delegate_id == employeeCheck.id && p.department == employee.department.ToUpper()).ToList();
            int length = weekendDutyCheck.Count;
            employee.department = employee.department == null ? "" : employee.department;
            bool isFound = false;
            for (int i = 0; i < length && isFound != true; i++)
            {
                weekend_duty weekendDuty = weekendDutyCheck.ElementAt(i);
                if (weekendDuty.delegate_id == employeeCheck.id && employee.department.ToUpper() == weekendDuty.department && employee.department.ToUpper() == employeeCheck.department.ToUpper())
                {
                    employee.employee_delegate = weekendDuty.delegate_id;
                    isFound = true;
                }
            }
        }

        public void setDelegate(EmployeeEntity employee, employee employeeCheck)
        {
            if (employee.department == null)
            {
                employee.department = "";
            }
            List<weekend_duty> weekendDutyCheck = weekendDuties.Where(p => p.delegate_id == employeeCheck.id && p.department == employee.department.ToUpper()).ToList();
            int length = weekendDutyCheck.Count;
            employee.department = employee.department == null ? "" : employee.department;
            bool isFound = false;
            for (int i = 0; i < length && isFound != true; i++)
            {
                weekend_duty weekendDuty = weekendDutyCheck.ElementAt(i);
                if (weekendDuty.delegate_id == employeeCheck.id && employee.department == null || employee.department.ToUpper() == weekendDuty.department && employee.department.ToUpper() == employeeCheck.department.ToUpper())
                {
                    employee.employee_delegate = weekendDuty.delegate_id;
                    isFound = true;
                }
            }
        }
    }
}