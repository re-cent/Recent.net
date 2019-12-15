using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class RelayerData
    {
        [Parameter("string", "name")]
        public string name { get; set; }

        [Parameter("address", "owner")]
        public string owner { get; set; }

        [Parameter("string", "domain")]
        public string domain { get; set; }

        [Parameter("uint", "maxUsers")]
        public uint maxUsers { get; set; }

        [Parameter("uint", "maxCoins")]
        public uint maxCoins { get; set; }

        [Parameter("uint", "maxTxThroughput")]
        public uint maxTxThroughput { get; set; }

        [Parameter("uint", "offchainTxDelay")]
        public uint offchainTxDelay { get; set; }

        [Parameter("uint", "epoch")]
        public uint epoch { get; set; }

        [Parameter("uint", "fee")]
        public uint fee { get; set; }

    }
}
