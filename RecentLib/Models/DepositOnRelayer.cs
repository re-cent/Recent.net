using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models
{
    /// <summary>
    /// Deposit to a Relayer info
    /// </summary>
    public class DepositOnRelayer
    {
        /// <summary>
        /// Wallet address remaining deposits lock until Block
        /// </summary>
        public uint lockUntilBlock { get; set; }

        /// <summary>
        /// Wallet address remaining deposits value on Relayer
        /// </summary>
        public decimal balance { get; set; }

    }
}
