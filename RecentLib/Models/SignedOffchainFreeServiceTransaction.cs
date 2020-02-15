using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models
{
    public class SignedOffchainFreeServiceTransaction
    {
        public byte[] h { get; set; }
        public uint v { get; set; }
        public byte[] r { get; set; }
        public byte[] s { get; set; }


        public string beneficiary { get; set; }
        public string validator { get; set; }
        public BigInteger freeMb { get; set; }
        public uint epoch { get; set; }

        public string signer { get; set; }

    }
}
