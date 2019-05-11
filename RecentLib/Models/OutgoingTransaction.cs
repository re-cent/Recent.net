using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models
{
    public class OutgoingTransaction
    {
        public string txId { get; set; }
        public BigInteger gasPrice { get; set; }
        public BigInteger gasLimit { get; set; }
        public decimal networkFee { get; set; }
    }
}
