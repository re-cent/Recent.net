using RecentLib.Models;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    public partial class RecentCore
    {
        /// <summary>
        /// Register new Relayer(Only allowed on epoch 1)
        /// </summary>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> addRelayer(string domain, string name, decimal fee, uint maxUsers, decimal maxCoins, uint maxTxThroughput, uint offchainTxDelay, decimal penaltyFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (fee > 100)
                throw new Exception("Fee should be lower than 100");
            var requiredAmount = await getFundRequiredForRelayer(maxUsers, maxCoins, maxTxThroughput);
            if (requiredAmount != penaltyFunds)
                throw new Exception($"Required amount is {requiredAmount}");
            return await executePaymentChannelsMethod("addRelayer", new object[] { domain, name, (uint)(fee * 10), maxUsers, recentToWei(maxCoins), maxTxThroughput, offchainTxDelay }, calcNetFeeOnly, waitReceipt, cancellationToken, penaltyFunds);

        }

        /// <summary>
        /// Request new Relayer license(After epoch 1)
        /// </summary>
        /// <param name="domain">The Relayer domain name or Ip</param>
        /// <param name="name">The Relayer name</param>
        /// <param name="isActive">Active or not</param>
        /// <param name="fee">The commision fee percent</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> requestRelayerLicense(string domain, string name, decimal fee, uint maxUsers, decimal maxCoins, uint maxTxThroughput, uint offchainTxDelay, decimal penaltyFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (fee > 100)
                throw new Exception("Fee should be lower than 100");
            var requiredAmount = await getFundRequiredForRelayer(maxUsers,  maxCoins, maxTxThroughput);
            if (requiredAmount!= penaltyFunds)
                throw new Exception($"Required amount is {requiredAmount}");
            return await executePaymentChannelsMethod("requestRelayerLicense", new object[] { domain, name, (uint)(fee * 10), maxUsers, recentToWei(maxCoins), maxTxThroughput, offchainTxDelay }, calcNetFeeOnly, waitReceipt, cancellationToken, penaltyFunds);

        }

        /// <summary>
        /// Update Relayer
        /// </summary>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> updateRelayer(string domain, string name, decimal fee, uint offchainTxDelay, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (fee > 100)
                throw new Exception("Fee should be lower than 100");
            return await executePaymentChannelsMethod("updateRelayer", new object[] { domain, name, (uint)(fee * 10), offchainTxDelay }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }

        /// <summary>
        /// Update Relayer
        /// </summary>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> relayerWithdrawPenaltyFunds(uint epoch, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executePaymentChannelsMethod("relayerWithdrawPenaltyFunds", new object[] { epoch }, calcNetFeeOnly, waitReceipt, cancellationToken);

        }

        public async Task<decimal> getRemainingFunds(uint epoch, string owner)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayerDepositPerEpoch");
            var remainingAmount = await function.CallAsync<BigInteger>(epoch, owner);


            return weiToRecent(remainingAmount);

        }


        public async Task<decimal> getFundRequiredForRelayer(uint maxUsers, decimal maxCoins, uint maxTxThroughput)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("getFundRequiredForRelayer");
            var fundRequired = await function.CallAsync<BigInteger>(maxUsers, recentToWei(maxCoins), maxTxThroughput);


            return weiToRecent(fundRequired);

        }

    }
}
