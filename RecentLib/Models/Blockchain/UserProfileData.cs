using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class UserProfileData
    {
        [Parameter("bytes32", "nickname")]
        public string nickname { get; set; }

        [Parameter("string", "avatarIpfsCID")]
        public string avatarIpfsCID { get; set; }

        [Parameter("bytes32", "firstname")]
        public string firstname { get; set; }

        [Parameter("bytes32", "lastname")]
        public string lastname { get; set; }

        [Parameter("uint", "contentProviderRatingTotalPoints")]
        public uint contentProviderRatingTotalPoints { get; set; }

        [Parameter("uint", "contentConsumerRatingTotalPoints")]
        public uint contentConsumerRatingTotalPoints { get; set; }

        [Parameter("uint", "contentProviderVotes")]
        public uint contentProviderVotes { get; set; }

        [Parameter("uint", "contentConsumerVotes")]
        public uint contentConsumerVotes { get; set; }

        [Parameter("string", "statusText")]
        public string statusText { get; set; }

        [Parameter("bool", "disabled")]
        public bool disabled { get; set; }
    }
}
