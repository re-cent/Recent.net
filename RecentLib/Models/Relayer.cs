using System;
using static RecentLib.Utils;

namespace RecentLib.Models
{

    /// <summary>
    /// Relayer Payment Channel
    /// </summary>
    public class Relayer
    {
        /// <summary>
        /// Relayer name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Relayer Owner address. It is unique identifier for a Relayer
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// Relayer Offchain Endpoint URL that expose the Relayer API
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// Max allowed number of Peers(Depositors) based on requested Relayer license
        /// </summary>
        public uint maxUsers { get; set; }
        /// <summary>
        /// Max allowed number of Coins(Deposits) based on requested Relayer license
        /// </summary>
        public decimal maxCoins { get; set; }
        /// <summary>
        /// Max allowed Offcain transactions throughput per 100000 Blocks based on requested Relayer license
        /// </summary>
        public uint maxTxThroughput { get; set; }
        /// <summary>
        /// Current number of Peers(Depositors)
        /// </summary>
        public uint currentUsers { get; set; }
        /// <summary>
        /// Current deposited Coins
        /// </summary>
        public decimal currentCoins { get; set; }
        /// <summary>
        /// Current used Offcain transactions throughput per 100000 Blocks
        /// </summary>
        public uint currentTxThroughput { get; set; }
        /// <summary>
        /// Excepted Delay of an Offchain transaction settlement in number of Blocks
        /// </summary>
        public uint offchainTxDelay { get; set; }
        /// <summary>
        /// Relayer fee(Thousands percent)
        /// </summary>
        public decimal fee { get; set; }
        /// <summary>
        /// Remaining Releayer penalty funds
        /// </summary>
        public decimal remainingPenaltyFunds { get; set; }
        /// <summary>
        /// Wallet address remaining deposits value on Relayer
        /// </summary>
        public decimal? userBalance { get; set; }
        /// <summary>
        /// Wallet address remaining deposits lock until Block
        /// </summary>
        public uint? lockUntilBlock { get; set; }
    }
}
