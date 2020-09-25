using RecentLib.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models
{
    /// <summary>
    /// Validator
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// The address entitled to participate in the consensus for an Epoch when is active for this Epoch
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// Total staking funds for an Epoch including Witnesses and Service Providers staking funds
        /// </summary>
        public decimal totalStakingFunds { get; set; }
        /// <summary>
        /// Staking funds for an Epoch
        /// </summary>
        public decimal stakingFunds { get; set; }
        /// <summary>
        /// Reward for Witnesses locked from a Validator for an  Epoch
        /// </summary>
        public decimal witnessesFunds { get; set; }
        /// <summary>
        /// Free service provided by Service Providers for an Epoch
        /// </summary>
        public decimal freeMbs { get; set; }
        /// <summary>
        /// List of Witness addresses
        /// </summary>
        public List<string> witnesses { get; set; }
        /// <summary>
        /// List of Service provider addresses
        /// </summary>
        public List<string> freeServiceProviders { get; set; }

        /// <summary>
        /// State for an Epoch
        /// </summary>
        public AddressStatusData addressStatusData { get; set; }
    }
}
