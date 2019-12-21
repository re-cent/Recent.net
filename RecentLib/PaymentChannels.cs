using Nethereum.ABI;
using Nethereum.Signer;
using Nethereum.Util;
using RecentLib.Models;
using RecentLib.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{

    public partial class RecentCore
    {


        public async Task<Relayer> getRelayer(string owner, bool includeBalance = false, string balanceAddress = "")
        {
            return await getRelayer(await getCurrentEpoch(), await getEpochRelayerIndex(await getCurrentEpoch(), owner), includeBalance, balanceAddress);
        }


        public async Task<Relayer> getRelayer(uint epoch, string owner, bool includeBalance = false, string balanceAddress = "")
        {

            return await getRelayer(epoch, await getEpochRelayerIndex(epoch, owner), includeBalance, balanceAddress);
        }



        /// <summary>
        /// Returns Relayer
        /// </summary>
        /// <returns>Relayer</returns>
        public async Task<Relayer> getRelayer(uint epoch, uint index, bool includeBalance = false, string balanceAddress = "")
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayers");
            var result = await function.CallDeserializingToObjectAsync<RelayerData>(epoch, index);
            uint? lockUntilBlock = null;
            decimal? balance = null;
            if (includeBalance)
            {
                var userBalanceFunction = contract.GetFunction("userDepositOnRelayer");
                var userBalance = await getUserDepositOnRelayer(string.IsNullOrEmpty(balanceAddress) ? _wallet.address : balanceAddress, result.owner);
                lockUntilBlock = userBalance.lockUntilBlock;
                balance = userBalance.balance;
            }
            return new Relayer
            {
                domain = result.domain,
                fee = result.fee / 10m,
                name = result.name,
                owner = result.owner,
                lockUntilBlock = lockUntilBlock,
                userBalance = balance,
                currentCoins = weiToRecent(result.currentCoins),
                currentTxThroughput = result.currentTxThroughput,
                currentUsers = result.currentUsers,
                remainingPenaltyFunds = weiToRecent(result.remainingPenaltyFunds),
                maxCoins = weiToRecent(result.maxCoins),
                maxTxThroughput = result.maxTxThroughput,
                maxUsers = result.maxUsers,
                offchainTxDelay = result.offchainTxDelay

            };
        }

        public async Task<DepositOnRelayer> getUserDepositOnRelayer(string userAddress, string relayer)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("userDepositOnRelayer");
            var result = await function.CallDeserializingToObjectAsync<DepositOnRelayerData>(userAddress, relayer);
            return new DepositOnRelayer { balance = weiToRecent(result.balance), lockUntilBlock = result.lockUntilBlock };
        }

        public async Task<uint> getCurrentEpoch()
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("getCurrentEpoch");
            var currentEpoch = await function.CallAsync<BigInteger>();
            return (uint)currentEpoch;
        }

        public async Task<uint> getCurrentValidatorsElectionEnd()
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("getCurrentValidatorsElectionEnd");
            var currentValidatorsElectionEnd = await function.CallAsync<BigInteger>();
            return (uint)currentValidatorsElectionEnd;
        }





        public async Task<uint> getEpochRelayerIndex(uint epoch, string owner)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("epochRelayerIndex");
            return await function.CallAsync<uint>(epoch, owner);

        }

        /// <summary>
        /// Get registered Relayers
        /// </summary>
        /// <returns>The list of Relayers</returns>
        public async Task<List<Relayer>> getRelayers(uint? epoch)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("relayersCounter");
            if (!epoch.HasValue)
            {
                epoch = await getCurrentEpoch();
            }
            uint totalRelayersCount = (uint)await function.CallAsync<BigInteger>(epoch);

            var ret = new List<Relayer>();
            Parallel.For(0, totalRelayersCount, i =>
            {
                ret.Add(getRelayer(epoch.Value, (uint)i).Result);

            });
            return ret;
        }





        /// <summary>
        /// Deposit to Relayer
        /// </summary>
        /// <param name="id">The Relayer Id</param>
        /// <param name="amount">The amount</param>
        /// <param name="lockTimeInDays">Time lock perdio in days</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> depositToRelayer(string owner, decimal amount, uint lockUntilBlock, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            if (lockUntilBlock <= await getLastBlock())
                throw new Exception("lockUntilBlock should be greater than current block");
            return await executePaymentChannelsMethod("depositToRelayer", new object[] { owner, lockUntilBlock }, calcNetFeeOnly, waitReceipt, cancellationToken, amount);
        }

        /// <summary>
        /// Withdraw funds from Relayer
        /// </summary>
        /// <param name="domain">The Relayer domain or Ip</param>
        /// <param name="amount">The amount</param>
        /// <returns>The tx</returns>
        public async Task<OutgoingTransaction> withdrawFundsFromRelayer(string relayer, decimal amount, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            return await executePaymentChannelsMethod("withdrawFunds", new object[] { relayer, recentToWei(amount) }, calcNetFeeOnly, waitReceipt, cancellationToken, null);
        }




        public async Task<SignedOffchainTransaction> relayerSignOffchainPayment(SignedOffchainTransaction offchainTransaction)
        {

            var validSignature = await checkOffchainSignature(offchainTransaction);
            if (!validSignature)
            {
                throw new Exception("Invalid signature");
            }

            var currentBlock = await getLastBlock();
            var relayer = await getRelayer(_wallet.address);
            offchainTransaction.txUntilBlock = currentBlock + relayer.offchainTxDelay;

            var signer = new MessageSigner();
            var encoder = new ABIEncode();

            ABIValue[] ABIValues = new ABIValue[]{
            new ABIValue("bytes32", offchainTransaction.h),
            new ABIValue("uint", offchainTransaction.txUntilBlock)
            };

            var payloadEncoded = encoder.GetABIEncodedPacked(ABIValues);
            var proof = Sha3Keccack.Current.CalculateHash(payloadEncoded);

            offchainTransaction.rh = proof;

            var signedTx = signer.Sign(offchainTransaction.rh, _wallet.PK);


            var signature = MessageSigner.ExtractEcdsaSignature(signedTx);
            offchainTransaction.rv = signature.V.FirstOrDefault();
            offchainTransaction.rr = signature.R;
            offchainTransaction.rs = signature.S;
            offchainTransaction.relayerId = _wallet.address;
            return offchainTransaction;
        }

        public async Task<SignedOffchainTransaction> signOffchainPayment(SignedOffchainTransaction offchainTransaction)
        {
            var signer = new MessageSigner();
            var encoder = new ABIEncode();

            ABIValue[] ABIValues = new ABIValue[]{
            new ABIValue("address", offchainTransaction.beneficiary),
            new ABIValue("bytes32", offchainTransaction.nonce),
            new ABIValue("uint256", offchainTransaction.amount),
            new ABIValue("uint", offchainTransaction.fee)
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


        public async Task<bool> checkOffchainSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("checkOffchainSignature");
            string signer = await function.CallAsync<string>(signedOffchainTransaction.h, signedOffchainTransaction.v, signedOffchainTransaction.r, signedOffchainTransaction.s, signedOffchainTransaction.nonce, signedOffchainTransaction.fee, signedOffchainTransaction.beneficiary, signedOffchainTransaction.amount);
            var addressEqualityComparer = new AddressEqualityComparer();
            return addressEqualityComparer.Equals(signer, signedOffchainTransaction.signer);
            //return AddressUtil.Current.ConvertToChecksumAddress(signer) == AddressUtil.Current.ConvertToChecksumAddress(signedOffchainTransaction.signer);
        }


        public async Task<bool> checkOffchainRelayerSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("checkOffchainRelayerSignature");
            string signer = await function.CallAsync<string>(signedOffchainTransaction.h, signedOffchainTransaction.rh, signedOffchainTransaction.rv, signedOffchainTransaction.rr, signedOffchainTransaction.rs, signedOffchainTransaction.txUntilBlock);
            var addressEqualityComparer = new AddressEqualityComparer();
            return addressEqualityComparer.Equals(signer, signedOffchainTransaction.relayerId);
        }

        public async Task<byte[]> getFinalizeOffchainRelayerSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction("getFinalizeOffchainRelayerSignature");
            return await function.CallAsync<byte[]>(signedOffchainTransaction.relayerId, signedOffchainTransaction.nonce, signedOffchainTransaction.fee, signedOffchainTransaction.beneficiary, signedOffchainTransaction.amount);
        }

        protected async Task<OutgoingTransaction> executePaymentChannelsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value = null)
        {

            var contract = _web3.Eth.GetContract(PaymentChannelsABI, PaymentChannelsContract);
            var function = contract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
