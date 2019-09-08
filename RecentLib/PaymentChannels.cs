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
        public async Task<Relayer> getRelayer(string domain)
        {
            return await getRelayer(getRelayerIdFromDomain(domain));  
        }

        /// <summary>
        /// Returns Relayer
        /// </summary>
        /// <param name="relayerId">The relayerId</param>
        /// <returns>Relayer</returns>
        public async Task<Relayer> getRelayer(byte[] relayerId)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayers");
            var result = await function.CallDeserializingToObjectAsync<RelayerData>(relayerId);
            return new Relayer
            {
                domain = result.domain,
                fee = result.fee / 1000,
                isActive = result.isActive,
                name = result.name,
                owner = result.owner,
                totalPoints = result.totalPoints,
                totalVotes = result.totalVotes

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


        protected async Task<OutgoingTransaction> executePaymentChannelsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken);
        }

    }
}
