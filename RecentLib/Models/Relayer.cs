using System;
using System.Collections.Generic;
using System.Text;

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

            public double rating { get { return totalVotes > 0 ? totalPoints / totalVotes : 0; } }

    }
}
