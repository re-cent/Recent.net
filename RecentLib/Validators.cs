using Nethereum.ABI;
using Nethereum.Signer;
using Nethereum.Util;
using RecentLib.Models;
using RecentLib.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    public partial class RecentCore
    {

        public async Task<uint> epochBlocks(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("epochBlocks");
            return await function.CallAsync<uint>();

        }

        public async Task<uint> getCurrentValidatorsEpoch()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getCurrentEpoch");
            var currentEpoch = await function.CallAsync<BigInteger>();
            return (uint)currentEpoch;
        }

        public async Task<uint> validatorsNumber()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("maximumValidatorsNumber");
            return await function.CallAsync<uint>();

        }

        public async Task<decimal> freeServicePricePerMb()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("pricePerMb");
            return weiToRecent(await function.CallAsync<BigInteger>());

        }

        public async Task<decimal> epochBlockReward(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("calculateReward");
            return weiToRecent(await function.CallAsync<BigInteger>(epoch));

        }

        public async Task<decimal> getRequiredStakingFunds(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getRequiredStakingFunds");
            return weiToRecent(await function.CallAsync<BigInteger>(epoch));
        }



        public async Task<uint> witnessRequiredBalancePercent()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("witnessRequiredBalancePercent");

            return await function.CallAsync<uint>();
        }

        public async Task<List<string>> getValidatorsByEpoch(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorsByEpoch");
            return (await function.CallAsync<List<string>>(epoch));
        }

        public async Task<List<string>> getCandidatesByEpoch(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getCandidates");
            return (await function.CallAsync<List<string>>(epoch));
        }

        public async Task<List<string>> getCandidateFreeServiceProviders(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorFreeServiceProviders");
            return (await function.CallAsync<List<string>>(epoch, candidate));
        }

        public async Task<List<string>> getCandidateWitnesses(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorWitnesses");
            return (await function.CallAsync<List<string>>(epoch, candidate));
        }




        public async Task<decimal> getCandidateFreeMbs(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorFreeMbs");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        public async Task<decimal> getCandidateStakingFunds(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorStakingFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        public async Task<decimal> getCandidateTotalStakingFunds(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorTotalStakingFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        public async Task<decimal> getCandidateFundsForWitnesses(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorWitnessesFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        public async Task<AddressStatusData> getCandidateElectedStatus(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("status");
            return await function.CallDeserializingToObjectAsync<AddressStatusData>(epoch, candidate);
        }

        public async Task<List<Validator>> getCandidatesDetailsByEpoch(uint epoch)
        {
            var candidates = await getCandidatesByEpoch(epoch);
            var candidatesFound = new List<Validator>();
            foreach (var candidate in candidates)
            {
                var validator = new Validator
                {
                    address = candidate,
                    freeMbs = await getCandidateFreeMbs(epoch, candidate),
                    freeServiceProviders = await getCandidateFreeServiceProviders(epoch, candidate),
                    stakingFunds = await getCandidateStakingFunds(epoch, candidate),
                    totalStakingFunds = await getCandidateTotalStakingFunds(epoch, candidate),
                    witnesses = await getCandidateWitnesses(epoch, candidate),
                    witnessesFunds = await getCandidateFundsForWitnesses(epoch, candidate),
                    addressStatusData = await getCandidateElectedStatus(epoch, candidate)
                };
                candidatesFound.Add(validator);
            }

            return candidatesFound;
        }

        public SignedOffchainFreeServiceTransaction signFreeServiceProviderMbs(SignedOffchainFreeServiceTransaction offchainTransaction)
        {
            var signer = new MessageSigner();
            var encoder = new ABIEncode();

            ABIValue[] ABIValues = new ABIValue[]{
            new ABIValue("address", offchainTransaction.beneficiary),
            new ABIValue("address", offchainTransaction.validator),
            new ABIValue("uint", offchainTransaction.epoch),
            new ABIValue("uint256", offchainTransaction.freeMb)
            };

            var payloadEncoded = encoder.GetABIEncodedPacked(ABIValues);
            var proof = Sha3Keccack.Current.CalculateHash(payloadEncoded);

            offchainTransaction.h = proof;

            var signedTx = signer.Sign(offchainTransaction.h, _wallet.PK);


            var signature = MessageSigner.ExtractEcdsaSignature(signedTx);
            offchainTransaction.v = signature.V.FirstOrDefault();
            offchainTransaction.r = signature.R;
            offchainTransaction.s = signature.S;
            offchainTransaction.signer = _wallet.address;
            return offchainTransaction;
        }


        public async Task<OutgoingTransaction> validatorAsCandidate(decimal stakingFunds, decimal witnessesFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executeValidatorsMethod("validatorAsCandidate", new object[] { recentToWei(stakingFunds), recentToWei(witnessesFunds) }, calcNetFeeOnly, waitReceipt, cancellationToken, stakingFunds + witnessesFunds);
        }


        public async Task<OutgoingTransaction> voteValidatorAsWitness(string validator, decimal funds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executeValidatorsMethod("voteValidatorAsWitness", new object[] { validator }, calcNetFeeOnly, waitReceipt, cancellationToken, funds);
        }

        public async Task<OutgoingTransaction> voteValidatorAsServiceProvider(string validator, uint freeContentInMb, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            decimal requiredFunds = await freeServicePricePerMb() * freeContentInMb;
            return await executeValidatorsMethod("voteValidatorAsServiceProvider", new object[] { validator, freeContentInMb }, calcNetFeeOnly, waitReceipt, cancellationToken, requiredFunds);
        }

        protected async Task<OutgoingTransaction> executeValidatorsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value = null)
        {

            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
