using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class AddressStatusData
    {
        [Parameter("bool", "isIn")]
        public bool isIn { get; set; }

        [Parameter("uint", "index")]
        public uint index { get; set; }


    }
}
