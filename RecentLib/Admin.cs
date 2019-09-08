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
        /// Add Relayer
        /// </summary>
        /// <param name="address">Relayer</param>
        /// <returns></returns>
        public async Task<OutgoingTransaction> addRelayer(string domain, string name, bool isActive, decimal fee, bool calcNetFeeOnly, CancellationTokenSource cancellationToken)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("addRelayer");
            
            return await executeBlockchainTransaction(_wallet.address, new object[] { domain, name, isActive, (int)(fee * 1000) }, calcNetFeeOnly, function, true, cancellationToken);
            ////var encoder = new Nethereum.ABI.Encoders.BytesTypeEncoder();
            ////var relayerId = Nethereum.Util.Sha3Keccack.Current.CalculateHash(encoder.EncodePacked(domain));
            //var stringEncoder = new Nethereum.ABI.Encoders.StringTypeEncoder();
            //var relayerId = Nethereum.Util.Sha3Keccack.Current.CalculateHash(stringEncoder.EncodePacked(domain));
            //return await getRelayerData(relayerId);



        }

        


    }
}
