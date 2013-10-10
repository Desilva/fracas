using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class WinnerEntity
    {
        public int id;
        public double reward;
        public string reward_string;
        public string category;
        public string winner;
    }

    public class RewardEntity
    {
        public int id;
        public double reward;
        public string reward_string;
    }

    public class WinnerReport
    {
        public DateTime? from;
        public DateTime? to;
        public int count_q;
        public int count;
        public List<WinnerEntity> winners;
    }
}