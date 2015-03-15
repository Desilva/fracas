using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class DutyManagerPresentationStub
    {
        
        public int id { get; set; }

        [Display(Name = "Duty Manager")]
        public string user_id { get; set; }

        [Display(Name = "Start Date")]
        public System.DateTime start_date { get; set; }

        [Display(Name = "End Date")]
        public System.DateTime end_date { get; set; }

        public DutyManagerPresentationStub() { }

        public DutyManagerPresentationStub(duty_manager dbItem)
        {
            id = dbItem.id;
            user_id = dbItem.user.username;
            start_date = dbItem.start_date;
            end_date = dbItem.end_date;
        }

        public List<DutyManagerPresentationStub> MapList(List<duty_manager> dbItems)
        {
            List<DutyManagerPresentationStub> result = new List<DutyManagerPresentationStub>();
            foreach (duty_manager row in dbItems)
            {
                result.Add(new DutyManagerPresentationStub(row));
            }
            return result;
        }
    }
}