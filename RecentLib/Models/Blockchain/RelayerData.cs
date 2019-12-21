using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class RelayerData
    {

        [Parameter("string", "name")]
        public string name { get; set; }

        [Parameter("address", "owner")]
        public string owner{ get; set; }

        [Parameter("string", "domain")]
        public string domain { get; set; }

        [Parameter("uint", "maxUsers")]
        public uint maxUsers { get; set; }

        [Parameter("uint256", "maxCoins")]
        public BigInteger maxCoins { get; set; }

        [Parameter("uint", "maxTxThroughput")]
        public uint maxTxThroughput { get; set; }

        [Parameter("uint", "currentUsers")]
        public uint currentUsers { get; set; }

        [Parameter("uint256", "currentCoins")]
        public BigInteger currentCoins { get; set; }

        [Parameter("uint", "currentTxThroughput")]
        public uint currentTxThroughput { get; set; }

        [Parameter("uint", "offchainTxDelay")]
        public uint offchainTxDelay { get; set; }

        //Thousands percent
        [Parameter("uint", "fee")]
        public uint fee { get; set; }

        [Parameter("uint256", "remainingPenaltyFunds")]
        public BigInteger remainingPenaltyFunds { get; set; }

    }

    
}
