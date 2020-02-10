using RecentLib.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    public partial class RecentCore
    {

        public async Task<uint> blocksPerEpoch(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("epochBlocks");
            return await function.CallAsync<uint>();

        }

        public async Task<uint> validatorsNumber(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("maximumValidatorsNumber");
            return await function.CallAsync<uint>();

        }

        public async Task<decimal> epochBlockReward(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("calculateReward");
            return weiToRecent(await function.CallAsync<BigInteger>(epoch));

        }

        public async Task<decimal> requiredStakingFunds(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("calculateReward");
            var blocks = await blocksPerEpoch(epoch);
            var validators = await validatorsNumber(epoch);
            var currentBlock = await getLastBlock();
            var funds = await function.CallAsync<BigInteger>(currentBlock-1, currentBlock-2);
            var fundsInRecent =  weiToRecent(funds);

            return blocks * fundsInRecent / validators;
        }


        public async Task<OutgoingTransaction> validatorAsCandidate(decimal stakingFunds, decimal witnessesFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executeValidatorsMethod("validatorAsCandidate", new object[] { recentToWei(stakingFunds), recentToWei(witnessesFunds) }, calcNetFeeOnly, waitReceipt, cancellationToken, stakingFunds + witnessesFunds);
        }

        protected async Task<OutgoingTransaction> executeValidatorsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value = null)
        {

            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
