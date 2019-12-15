using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class DepositOnRelayerData
    {
        [Parameter("uint", "lockUntilBlock")]
        public uint lockUntilBlock { get; set; }

        [Parameter("uint256", "balance")]
        public BigInteger balance { get; set; }


    }
}
