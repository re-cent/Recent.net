using System;
using static RecentLib.Utils;

namespace RecentLib.Models
{



    public class Relayer
    {
        public string name { get; set; }


        public string owner { get; set; }

        public string domain { get; set; }

        public bool isActive { get; set; }

        public decimal fee { get; set; }

        public uint totalPoints { get; set; }

        public uint totalVotes { get; set; }

        public double rating { get { return totalVotes > 0 ? totalPoints / (100d * totalVotes) : 0; } }

        public decimal? userBalance { get; set; }


        public uint? lockedUntil { get; set; }


        public DateTime? lockedUntilDateTime
        {
            get
            {
                return lockedUntil.HasValue ? convertFromEpoch(lockedUntil.Value) : (DateTime?)null;
            }
        }


    }
}
