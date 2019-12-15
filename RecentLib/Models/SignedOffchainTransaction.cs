using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models
{
    public class SignedOffchainTransaction
    {
        public byte[] h { get; set; }
        public uint v { get; set; }
        public byte[] r { get; set; }
        public byte[] s { get; set; }
        
        public string nonce { get; set; }
        public uint fee { get; set; }
        
        public string beneficiary { get; set; }
        public BigInteger amount { get; set; }
        public string signer { get; set; }

        public byte[] rh { get; set; }
        public uint rv { get; set; }
        public byte[] rr { get; set; }
        public byte[] rs { get; set; }
        public string relayerId { get; set; }
        public uint txUntilBlock { get; set; }
    }
}
