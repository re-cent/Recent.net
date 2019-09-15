using Nethereum.Web3;
using RecentLib.Models;
using RecentLib.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    public partial class RecentCore
    {
        /// <summary>
        /// Register new Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain name or Ip</param>
        /// <param name="name">The Relayer name</param>
        /// <param name="isActive">Active or not</param>
        /// <param name="fee">The commision fee percent</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> addRelayer(string domain, string name, bool isActive, decimal fee, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (fee > 100)
                throw new Exception("Fee should be lower than 100");
            return await executePaymentChannelsMethod("addRelayer", new object[] { domain, name, isActive, (uint)(fee * 10) }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }

        /// <summary>
        /// Update Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain name or Ip</param>
        /// <param name="name">The Relayer name</param>
        /// <param name="isActive">Active or not</param>
        /// <param name="fee">The commision fee percent</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> updateRelayer(string domain, string name, bool isActive, decimal fee, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (fee > 100)
                throw new Exception("Fee should be lower than 100");
            return await executePaymentChannelsMethod("updateRelayer", new object[] { getRelayerIdFromDomain(domain), name, (uint)(fee * 10), isActive }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }


    }
}
