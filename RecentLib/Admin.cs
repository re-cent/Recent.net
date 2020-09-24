using RecentLib.Models;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    /// <summary>
    /// Partial class that contains methods for Relayer administration such as Request Relayer license, Update Relayer properties on RSC, Withdraw remaining locked funds, Calculate the required amount for new licnense request
    /// Interact with RSC(Relayer Smart Contract)
    /// </summary>
    public partial class RecentCore
    {


        /// <summary>
        /// Request new Relayer license(After epoch 1)
        /// </summary>
        /// <param name="targetEpoch">The target Epoch for the requested license</param>
        /// <param name="domain">The Relayer Endpoint URL(Should expose the Relayer functionality via API in this Endpoint)</param>
        /// <param name="name">The Relayer name</param>
        /// <param name="fee">The commision fee percent</param>
        /// <param name="maxUsers">The requested max number of Peers(Depositors)</param>
        /// <param name="maxCoins">The requested max Coins could be deposited by Peers(Total Deposits)</param>
        /// <param name="maxTxThroughput">The requested max allowed Offcain transactions throughput per 100000 Blocks</param>
        /// <param name="offchainTxDelay">The expected settlement delay in Blocks</param>
        /// <param name="penaltyFunds">The Coins required for lock by Relayer based on requested license</param>
        /// 
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> requestRelayerLicense(uint targetEpoch, string domain, string name, decimal fee, uint maxUsers, decimal maxCoins, uint maxTxThroughput, uint offchainTxDelay, decimal penaltyFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            //Fee should be between 0 and 100
            if (fee > 100 || fee<0)
                throw new Exception("Fee should be lower than 100");

            //Get the required amount for the license request
            var requiredAmount = await getFundRequiredForRelayer(maxUsers,  maxCoins, maxTxThroughput);

            //requiredAmount and penaltyFunds should be equal
            if (requiredAmount!= penaltyFunds)
                throw new Exception($"Required amount is {requiredAmount}");

            //Invoke RSC Method
            return await executePaymentChannelsMethod("requestRelayerLicense", new object[] { targetEpoch, domain, name, (uint)(fee * 10), maxUsers, recentToWei(maxCoins), maxTxThroughput, offchainTxDelay }, calcNetFeeOnly, waitReceipt, cancellationToken, penaltyFunds);

        }

        /// <summary>
        /// Update Relayer
        /// </summary>
        /// <param name="targetEpoch">The target Epoch</param>
        /// <param name="domain">The Relayer Endpoint URL(Should expose the Relayer functionality via API in this Endpoint)</param>
        /// <param name="name">The Relayer name</param>
        /// <param name="fee">The commision fee percent</param>
        /// <param name="offchainTxDelay">The expected settlement delay in Blocks</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> updateRelayer(uint targetEpoch, string domain, string name, decimal fee, uint offchainTxDelay, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            //Fee should be between 0 and 100
            if (fee > 100 || fee < 0)
                throw new Exception("Fee should be lower than 100");

            //Invoke RSC Method
            return await executePaymentChannelsMethod("updateRelayer", new object[] { targetEpoch, domain, name, (uint)(fee * 10), offchainTxDelay }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }

        /// <summary>
        /// Withdraw remaing funds
        /// <param name="targetEpoch">The target Epoch</param>
        /// </summary>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> relayerWithdrawPenaltyFunds(uint targetEpoch, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            //Invoke RSC Method
            return await executePaymentChannelsMethod("relayerWithdrawPenaltyFunds", new object[] { targetEpoch }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }



        /// <summary>
        /// Calculate the amount required for a new license
        /// </summary>
        /// <param name="maxUsers">The requested max number of Peers(Depositors)</param>
        /// <param name="maxCoins">The requested max Coins could be deposited by Peers(Total Deposits)</param>
        /// <param name="maxTxThroughput">The requested max allowed Offcain transactions throughput per 100000 Blocks</param>
        /// <returns>The coins required for license</returns>
        public async Task<decimal> getFundRequiredForRelayer(uint maxUsers, decimal maxCoins, uint maxTxThroughput)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("getFundRequiredForRelayer");
            var fundRequired = await function.CallAsync<BigInteger>(maxUsers, recentToWei(maxCoins), maxTxThroughput);


            return weiToRecent(fundRequired);

        }

    }
}
