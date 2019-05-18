using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models
{

    public class UserProfile
    {
        public string nickname { get; set; }

        public string avatarIpfsCID { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public decimal? contentProviderRating { get; set; }

        public decimal? contentConsumerRating { get; set; }

        public uint contentProviderVotes { get; set; }

        public uint contentConsumerVotes { get; set; }

        public string statusText { get; set; }

        public bool disabled { get; set; }
    }
}
