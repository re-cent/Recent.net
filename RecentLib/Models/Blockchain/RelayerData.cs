using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class RelayerData
    {
        [Parameter("bytes32", "name")]
        public string name { get; set; }

        [Parameter("address", "owner")]
        public string owner { get; set; }

        [Parameter("string", "domain")]
        public string domain { get; set; }

        [Parameter("bool", "isActive")]
        public bool isActive { get; set; }

        [Parameter("uint", "fee")]
        public uint fee { get; set; }

        [Parameter("uint", "totalPoints")]
        public uint totalPoints { get; set; }

        [Parameter("uint", "totalVotes")]
        public uint totalVotes { get; set; }

    }
}
