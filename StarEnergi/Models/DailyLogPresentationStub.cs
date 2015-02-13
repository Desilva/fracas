using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class DailyLogPresentationStub
    {
        public int id { get; set; }
        public Nullable<System.DateTime> date { get; set; }

        public DailyLogPresentationStub() { }

        public DailyLogPresentationStub(daily_log dbItem)
        {
            id = dbItem.id;
            date = dbItem.date;
        }

        public List<DailyLogPresentationStub> MapList(List<daily_log> dbItems)
        {
            List<DailyLogPresentationStub> result = new List<DailyLogPresentationStub>();
            foreach (daily_log row in dbItems)
            {
                result.Add(new DailyLogPresentationStub(row));
            }
            return result;
        }
    }
}