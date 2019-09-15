using RecentLib.Models;
using RecentLib.Models.Blockchain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;
using static RecentLib.Models.Blockchain.SolidityPrimitives;

namespace RecentLib
{

    public partial class RecentCore
    {


        /// <summary>
        /// Returns RelayerId for a Domain
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public byte[] getRelayerIdFromDomain(string domain)
        {
            return Nethereum.Util.Sha3Keccack.Current.CalculateHash(new Nethereum.ABI.Encoders.StringTypeEncoder().EncodePacked(domain));
        }


        /// <summary>
        /// Returns Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain</param>
        /// <returns>Relayer</returns>
        public async Task<Relayer> getRelayer(string domain, bool includeBalance = false, string balanceAddress = "")
        {
            return await getRelayer(getRelayerIdFromDomain(domain), includeBalance, balanceAddress);  
        }

        /// <summary>
        /// Returns Relayer
        /// </summary>
        /// <param name="relayerId">The relayerId</param>
        /// <returns>Relayer</returns>
        public async Task<Relayer> getRelayer(byte[] relayerId, bool includeBalance = false, string balanceAddress="")
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayers");
            var result = await function.CallDeserializingToObjectAsync<RelayerData>(relayerId);
            uint? lockedUntil=null;
            decimal? balance = null;
            if (includeBalance)
            {
                var userBalanceFunction = contract.GetFunction("userDepositOnRelayer");
                var userBalance = await userBalanceFunction.CallDeserializingToObjectAsync<DepositOnRelayerData>(string.IsNullOrEmpty(balanceAddress) ? _wallet.address : balanceAddress, relayerId);
                lockedUntil = userBalance.lockUntil;
                balance = weiToRecent(userBalance.balance);
            }
            return new Relayer
            {
                domain = result.domain,
                fee = result.fee / 10m,
                isActive = result.isActive,
                name = result.name,
                owner = result.owner,
                totalPoints = result.totalPoints,
                totalVotes = result.totalVotes,
                lockedUntil = lockedUntil,
                userBalance = balance

            };
        }


        /// <summary>
        /// Get registered Relayers
        /// </summary>
        /// <returns>The list of Relayers</returns>
        public async Task<List<Relayer>> getRelayers()
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayersCounter");
            var totalRelayersCount = await function.CallAsync<BigInteger>();

            var relayerIdsFunction = contract.GetFunction("relayerIds");
            var ret = new List<Relayer>();
            Parallel.For(0, (int)totalRelayersCount, i =>
            {
                ret.Add(getRelayer(relayerIdsFunction.CallAsync<byte[]>(i + 1).Result).Result);

            });
            return ret;
        }

        /// <summary>
        /// Vote Relayer
        /// </summary>
        /// <param name="id">The Relayer Id</param>
        /// <param name="rating">The rating (1 to 5)</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> voteRelayer(byte[] id, double rating, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (rating > 5 || rating < 1)
                throw new Exception("Rating should be between 1 and 5");
            return await executePaymentChannelsMethod("voteRelayer", new object[] { id, (uint)(rating * 100d) }, calcNetFeeOnly, waitReceipt, cancellationToken);
        }

        /// <summary>
        /// Vote Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain name or Ip</param>
        /// <param name="rating">The rating (1 to 5)</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> voteRelayer(string domain, double rating, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            return await voteRelayer(getRelayerIdFromDomain(domain), rating, calcNetFeeOnly, waitReceipt, cancellationToken);
        }


        /// <summary>
        /// Deposit to Relayer
        /// </summary>
        /// <param name="id">The Relayer Id</param>
        /// <param name="amount">The amount</param>
        /// <param name="lockTimeInDays">Time lock perdio in days</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> depositToRelayer(byte[] id, decimal amount, uint lockTimeInDays, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (lockTimeInDays==0)
                throw new Exception("lockTimeInDays should be greater than zero");
            return await executePaymentChannelsMethod("depositToRelayer", new object[] { id, lockTimeInDays }, calcNetFeeOnly, waitReceipt, cancellationToken, amount);
        }

        /// <summary>
        /// Withdraw funds from Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain or Ip</param>
        /// <param name="amount">The amount</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> withdrawFunds(string domain, decimal amount, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            return await withdrawFunds(getRelayerIdFromDomain(domain), amount , calcNetFeeOnly, waitReceipt, cancellationToken);
        }

        /// <summary>
        /// Withdraw funds from Relayer
        /// </summary>
        /// <param name="id">The Relayer Id</param>
        /// <param name="amount">The amount</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> withdrawFunds(byte[] id, decimal amount, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            return await executePaymentChannelsMethod("withdrawFunds", new object[] { id, recentToWei(amount) }, calcNetFeeOnly, waitReceipt, cancellationToken);
        }

        /// <summary>
        /// Deposit Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain name or Ip</param>
        /// <param name="amount">The amount</param>
        /// <param name="lockTimeInDays">Time lock perdio in days</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> depositToRelayer(string domain, decimal amount, uint lockTimeInDays, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await depositToRelayer(getRelayerIdFromDomain(domain), amount, lockTimeInDays, calcNetFeeOnly, waitReceipt, cancellationToken);
        }

        protected async Task<OutgoingTransaction> executePaymentChannelsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value=null)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
