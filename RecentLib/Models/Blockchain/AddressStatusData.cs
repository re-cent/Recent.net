using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models.Blockchain
{
    [FunctionOutput]
    public class AddressStatusData
    {
        /// <summary>
        /// WHen true Validator is active
        /// </summary>
        [Parameter("bool", "isIn")]
        public bool isIn { get; set; }
        /// <summary>
        /// The index on active Validators array
        /// </summary>
        [Parameter("uint", "index")]
        public uint index { get; set; }


    }
}
