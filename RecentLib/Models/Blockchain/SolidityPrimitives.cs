using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    public class SolidityPrimitives
    {
        [FunctionOutput]
        public class Uint256
        {
            [Parameter("uint256")]
            public BigInteger value { get; set; }
        }

        [FunctionOutput]
        public class Bytes32
        {
            [Parameter("bytes32")]
            public string value { get; set; }
        }
    }
}
