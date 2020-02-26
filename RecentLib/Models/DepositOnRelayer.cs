using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models
{
    public class DepositOnRelayer
    {
        public uint lockUntilBlock { get; set; }

        public decimal balance { get; set; }

    }
}
