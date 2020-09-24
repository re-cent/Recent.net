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
    /// <summary>
    /// Partial class that contains methods for interaction with VSC(Validators Smart Contract)
    /// </summary>
    public partial class RecentCore
    {

        /// <summary>
        /// Return the number of Blocks per Validators Epoch
        /// </summary>
        /// <returns></returns>
        public async Task<uint> epochBlocks()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("epochBlocks");
            return await function.CallAsync<uint>();

        }
        /// <summary>
        /// Return the currenct Validators Epoch
        /// </summary>
        /// <returns></returns>
        public async Task<uint> getCurrentValidatorsEpoch()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getCurrentEpoch");
            var currentEpoch = await function.CallAsync<BigInteger>();
            return (uint)currentEpoch;
        }

        /// <summary>
        /// Return the max allowed Validators number
        /// </summary>
        /// <returns></returns>
        public async Task<uint> validatorsNumber()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("maximumValidatorsNumber");
            return await function.CallAsync<uint>();

        }


        /// <summary>
        /// The price per Mb for free service
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> freeServicePricePerMb()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("pricePerMb");
            return weiToRecent(await function.CallAsync<BigInteger>());

        }

        /// <summary>
        /// THe Block reward for an Epoch
        /// </summary>
        /// <param name="epoch">Epoch</param>
        /// <returns></returns>
        public async Task<decimal> epochBlockReward(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("calculateReward");
            return weiToRecent(await function.CallAsync<BigInteger>(epoch));

        }

        /// <summary>
        /// Get required staking funds for a Validator
        /// </summary>
        /// <param name="epoch">Target Epoch</param>
        /// <returns></returns>
        public async Task<decimal> getRequiredStakingFunds(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getRequiredStakingFunds");
            return weiToRecent(await function.CallAsync<BigInteger>(epoch));
        }


        /// <summary>
        /// Get the required Witness balance percent
        /// </summary>
        /// <returns></returns>
        public async Task<uint> witnessRequiredBalancePercent()
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("witnessRequiredBalancePercent");

            return await function.CallAsync<uint>();
        }

        /// <summary>
        /// List of Validators for an Epoch
        /// </summary>
        /// <param name="epoch">Epoch</param>
        /// <returns></returns>
        public async Task<List<string>> getValidatorsByEpoch(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorsByEpoch");
            return (await function.CallAsync<List<string>>(epoch));
        }

        /// <summary>
        /// List of Candidates for an Epoch
        /// </summary>
        /// <param name="epoch">Epoch</param>
        /// <returns></returns>
        public async Task<List<string>> getCandidatesByEpoch(uint epoch)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getCandidates");
            return (await function.CallAsync<List<string>>(epoch));
        }

        /// <summary>
        /// Get the List of service providers for a Candidate
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<List<string>> getCandidateFreeServiceProviders(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorFreeServiceProviders");
            return (await function.CallAsync<List<string>>(epoch, candidate));
        }

        /// <summary>
        /// Get the list of Witnesses for a Candidate
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<List<string>> getCandidateWitnesses(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("getValidatorWitnesses");
            return (await function.CallAsync<List<string>>(epoch, candidate));
        }



        /// <summary>
        /// Free service in Mbs provided by Service Providers for a Validator and Epoch
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<decimal> getCandidateFreeMbs(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorFreeMbs");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        /// <summary>
        /// The staking funds for a Candidate
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<decimal> getCandidateStakingFunds(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorStakingFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        /// <summary>
        /// The total staking funds of a Candidate
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate<</param>
        /// <returns></returns>
        public async Task<decimal> getCandidateTotalStakingFunds(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorTotalStakingFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        /// <summary>
        /// Reward for Witnesses locked from a Validator for an  Epoch
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<decimal> getCandidateFundsForWitnesses(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("validatorWitnessesFunds");
            return weiToRecent((await function.CallAsync<BigInteger>(epoch, candidate)));
        }

        /// <summary>
        /// The status of a Candidate
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <param name="candidate">The Candidate</param>
        /// <returns></returns>
        public async Task<AddressStatusData> getCandidateElectedStatus(uint epoch, string candidate)
        {
            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction("status");
            return await function.CallDeserializingToObjectAsync<AddressStatusData>(epoch, candidate);
        }

        /// <summary>
        /// Get Candidates details for an Epoch
        /// </summary>
        /// <param name="epoch">The target Epoch</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sign a prroof that a free service provider provided free content to a Witness. Should be signed by wallet owner as Witness
        /// </summary>
        /// <param name="offchainTransaction"></param>
        /// <returns></returns>
        public SignedOffchainFreeServiceTransaction signFreeServiceProviderMbs(SignedOffchainFreeServiceTransaction offchainTransaction)
        {
            var signer = new MessageSigner();
            var encoder = new ABIEncode();

            ABIValue[] ABIValues = new ABIValue[]{
            new ABIValue("address", offchainTransaction.freeServiceProvider),
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

        /// <summary>
        /// Request as Candidate for the upcoming Epoch
        /// </summary>
        /// <param name="stakingFunds">The amount required to be allocated for staking</param>
        /// <param name="witnessesFunds">The amount provided to Witnesses</param>
        /// <param name="calcNetFeeOnly"></param>
        /// <param name="waitReceipt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OutgoingTransaction> validatorAsCandidate(decimal stakingFunds, decimal witnessesFunds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executeValidatorsMethod("validatorAsCandidate", new object[] { recentToWei(stakingFunds), recentToWei(witnessesFunds) }, calcNetFeeOnly, waitReceipt, cancellationToken, stakingFunds + witnessesFunds);
        }

        /// <summary>
        /// Vote a Candidate as Witness
        /// </summary>
        /// <param name="validator">The Candidate address</param>
        /// <param name="funds">The staking funds. Total Wallet balance * witnessRequiredBalancePercent % should be lower than funds</param>
        /// <param name="calcNetFeeOnly"></param>
        /// <param name="waitReceipt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OutgoingTransaction> voteValidatorAsWitness(string validator, decimal funds, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            return await executeValidatorsMethod("voteValidatorAsWitness", new object[] { validator }, calcNetFeeOnly, waitReceipt, cancellationToken, funds);
        }

        /// <summary>
        /// Vote a Candidate as Service provider
        /// </summary>
        /// <param name="validator">The Candidate address</param>
        /// <param name="freeContentInMb">The free contant to be provided to Witnesses</param>
        /// <param name="calcNetFeeOnly"></param>
        /// <param name="waitReceipt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OutgoingTransaction> voteValidatorAsServiceProvider(string validator, uint freeContentInMb, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            decimal requiredFunds = await freeServicePricePerMb() * freeContentInMb;
            return await executeValidatorsMethod("voteValidatorAsServiceProvider", new object[] { validator, freeContentInMb }, calcNetFeeOnly, waitReceipt, cancellationToken, requiredFunds);
        }

        /// <summary>
        /// Generic method that invokes VSC methods
        /// </summary>
        /// <param name="method">The Smart contract method</param>
        /// <param name="input">THe input arguments</param>
        /// <param name="calcNetFeeOnly">Calculate network fees and return. Don't place Tx Onchain</param>
        /// <param name="waitReceipt">Wait for the Tx to be mined</param>
        /// <param name="cancellationToken"></param>
        /// <param name="value">When Tx is payable the amount to be transfered to Smart Contract</param>
        /// <returns>The Tx</returns>
        protected async Task<OutgoingTransaction> executeValidatorsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value = null)
        {

            var contract = _web3.Eth.GetContract(ValidatorsABI, ValidatorsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
