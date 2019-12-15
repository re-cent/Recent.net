using System;
using static RecentLib.Utils;

namespace RecentLib.Models
{



    public class Relayer
    {
        public string name { get; set; }


        public string owner { get; set; }

        public string domain { get; set; }


        public decimal fee { get; set; }

        public uint maxUsers { get; set; }

        public uint maxCoins { get; set; }

        public uint maxTxThroughput { get; set; }

        public uint offchainTxDelay { get; set; }

        public uint epoch { get; set; }

        public decimal? userBalance { get; set; }

        public uint? lockUntilBlock { get; set; }




    }
}
