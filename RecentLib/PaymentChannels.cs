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
    /// <summary>
    /// Partial class that contains methods for interaction with RSC(Relayer Smart Contract)
    /// </summary>
    public partial class RecentCore
    {

        /// <summary>
        /// Get Relayer info for current Epoch
        /// </summary>
        /// <param name="owner">The Relayer Owner address(This is the Id of a Relayer)</param>
        /// <param name="includeBalance">When true retuns the locked amount for Wallet Owner or the balanceAddress parameter</param>
        /// <param name="balanceAddress">When includeBalance and not empty string returns the locked amount of this address on Requested Relayer</param>
        /// <returns>The Relayer Info class</returns>
        public async Task<Relayer> getRelayer(string owner, bool includeBalance = false, string balanceAddress = "")
        {
            return await getRelayer(await getCurrentRelayersEpoch(), await getEpochRelayerIndex(await getCurrentRelayersEpoch(), owner), includeBalance, balanceAddress);
        }

        /// <summary>
        /// Get Relayer info for a requested Epoch
        /// </summary>
        /// <param name="epoch">The requested Epoch</param>
        /// <param name="owner">The Relayer Owner address(This is the Id of a Relayer)</param>
        /// <param name="includeBalance">When true retuns the locked amount for Wallet Owner or the balanceAddress parameter</param>
        /// <param name="balanceAddress">When includeBalance and not empty string returns the locked amount of this address on Requested Relayer</param>
        /// <returns>The Relayer Info class</returns>
        public async Task<Relayer> getRelayer(uint epoch, string owner, bool includeBalance = false, string balanceAddress = "")
        {

            return await getRelayer(epoch, await getEpochRelayerIndex(epoch, owner), includeBalance, balanceAddress);
        }



        /// Get Relayer info for a requested Epoch
        /// </summary>
        /// <param name="epoch">The requested Epoch</param>
        /// <param name="index">The index of the requested Relayer in Relayers list</param>
        /// <param name="includeBalance">When true retuns the locked amount for Wallet Owner or the balanceAddress parameter</param>
        /// <param name="balanceAddress">When includeBalance and not empty string returns the locked amount of this address on Requested Relayer</param>
        /// <returns>The Relayer Info class</returns>
        public async Task<Relayer> getRelayer(uint epoch, uint index, bool includeBalance = false, string balanceAddress = "")
        {

            var function = _paymentChannelsContract.GetFunction("relayers");
            var result = await function.CallDeserializingToObjectAsync<RelayerData>(epoch, index);
            uint? lockUntilBlock = null;
            decimal? balance = null;
            if (includeBalance)
            {
                var userBalanceFunction = _paymentChannelsContract.GetFunction("userDepositOnRelayer");
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

        /// <summary>
        /// Return the locked amount and lock until block 
        /// </summary>
        /// <param name="userAddress">The Peer wallet address</param>
        /// <param name="relayer">The Relayer owner address(Relayer Id)</param>
        /// <returns>DepositOnRelayer class</returns>
        public async Task<DepositOnRelayer> getUserDepositOnRelayer(string userAddress, string relayer)
        {
            var function = _paymentChannelsContract.GetFunction("userDepositOnRelayer");
            var result = await function.CallDeserializingToObjectAsync<DepositOnRelayerData>(userAddress, relayer);
            return new DepositOnRelayer { balance = weiToRecent(result.balance), lockUntilBlock = result.lockUntilBlock };
        }

        /// <summary>
        /// Return the total number of Relayers for current Epoch
        /// </summary>
        /// <returns>Number of Relayers</returns>
        public async Task<uint> getCurrentRelayersEpoch()
        {
            var function = _paymentChannelsContract.GetFunction("getCurrentEpoch");
            var currentEpoch = await function.CallAsync<BigInteger>();
            return (uint)currentEpoch;
        }

        /// <summary>
        /// Return the block number that current election period ends
        /// </summary>
        /// <returns>Block number</returns>
        public async Task<uint> getCurrentRelayersElectionEnd()
        {
            var function = _paymentChannelsContract.GetFunction("getCurrentRelayersElectionEnd");
            return (uint)await function.CallAsync<BigInteger>();
        }




        /// <summary>
        /// Return the index of Relayer in Epoch Relayers list
        /// </summary>
        /// <param name="epoch">The requested Epoch</param>
        /// <param name="owner">The Relayer owner address(Relayer Id)</param>
        /// <returns></returns>
        public async Task<uint> getEpochRelayerIndex(uint epoch, string owner)
        {
            var function = _paymentChannelsContract.GetFunction("epochRelayerIndex");
            return await function.CallAsync<uint>(epoch, owner);

        }

        /// <summary>
        /// Return the already settled amount between 2 Peers for a nonce
        /// </summary>
        /// <param name="signer">User1</param>
        /// <param name="beneficiary">User2 (beneficiary)</param>
        /// <param name="nonce">The unique P2P transaction id</param>
        /// <returns></returns>
        public async Task<decimal> userToBeneficiaryFinalizedAmountForNonce(string signer, string beneficiary, string relayer, string nonce)
        {
            var function = _paymentChannelsContract.GetFunction("userToBeneficiaryFinalizedAmountForNonce");
            var currentAmount = await function.CallAsync<BigInteger>(signer, beneficiary, relayer, nonce);
            return weiToRecent(currentAmount);

        }

        /// <summary>
        /// Get the list registered Relayers for an Epoch
        /// </summary>
        /// <param name="epoch">The requested Epoch. null for current Epoch</param>
        /// <param name="includeBalance">When true retuns the locked amount for Wallet Owner or the balanceAddress parameter</param>
        /// <param name="balanceAddress">When includeBalance and not empty string returns the locked amount of this address on Requested Relayer</param>
        /// <returns>The list of Relayers</returns>
        public async Task<List<Relayer>> getRelayers(uint? epoch, bool includeBalance = false, string balanceAddress = "")
        {
            var function = _paymentChannelsContract.GetFunction("relayersCounter");
            if (!epoch.HasValue)
            {
                epoch = await getCurrentRelayersEpoch();
            }
            uint totalRelayersCount = (uint)await function.CallAsync<BigInteger>(epoch);

            var ret = new List<Relayer>();
            for (int i = 1; i <= totalRelayersCount; i++)
            {
                ret.Add(await getRelayer(epoch.Value, (uint)i, includeBalance, balanceAddress));
            }
            //Parallel.For(1, totalRelayersCount + 1, async i =>
            //{
            //    ret.Add(await getRelayer(epoch.Value, (uint)i, includeBalance, balanceAddress));
            //});
            return ret;
        }





        /// <summary>
        /// Deposit to Relayer
        /// </summary>
        /// <param name="owner">The Relayer Owner address(This is the Id of a Relayer)</param>
        /// <param name="amount">The amount</param>
        /// <param name="lockUntilBlock">Time lock through Block</param>
        /// <returns>The Tx</returns>
        public async Task<OutgoingTransaction> depositToRelayer(string owner, decimal amount, uint lockUntilBlock, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            //lockUntilBlock should be greater than last mined Block
            if (lockUntilBlock <= await getLastBlock())
                throw new Exception("lockUntilBlock should be greater than current block");
            //Invoke RSC
            return await executePaymentChannelsMethod("depositToRelayer", new object[] { owner, lockUntilBlock }, calcNetFeeOnly, waitReceipt, cancellationToken, amount);
        }

        /// <summary>
        /// Withdraw funds from Relayer
        /// </summary>
        /// <param name="relayer">The Relayer Owner address(This is the Id of a Relayer)</param>
        /// <param name="amount">The amount</param>
        /// <returns>The Tx</returns>
        public async Task<OutgoingTransaction> withdrawFundsFromRelayer(string relayer, decimal amount, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            //Invoke RSC
            return await executePaymentChannelsMethod("withdrawFunds", new object[] { relayer, recentToWei(amount) }, calcNetFeeOnly, waitReceipt, cancellationToken, null);
        }



        /// <summary>
        /// Sign an Offchain Payemnt as Relayer using Wallet private key
        /// </summary>
        /// <param name="offchainTransaction">The already signed by Peer Offchain Payment payload</param>
        /// <returns>The transaction signed by Wallet owner as Relayer</returns>
        public async Task<SignedOffchainTransaction> relayerSignOffchainPayment(SignedOffchainTransaction offchainTransaction)
        {
            //Check that already signed payload is valid
            var validSignature = await checkOffchainSignature(offchainTransaction);
            if (!validSignature)
            {
                throw new Exception("Invalid signature");
            }



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

        /// <summary>
        /// Sign an Offchain payment as a Peer
        /// </summary>
        /// <param name="offchainTransaction">The transaction to be signed</param>
        /// <returns>The Transaction signed by wallet owner as Peer </returns>
        public SignedOffchainTransaction signOffchainPayment(SignedOffchainTransaction offchainTransaction)
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

        /// <summary>
        /// Settle an Offchain payment by invoking the RSC. SHould be called by the Relayer tha uas received the transaction for Settlement
        /// </summary>
        /// <param name="signedOffchainTransaction">The Signed by both Peer and Relayer payload</param>
        /// <param name="calcNetFeeOnly"></param>
        /// <param name="waitReceipt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The Tx</returns>
        public async Task<OutgoingTransaction> finalizeOffchainRelayerTransaction(SignedOffchainTransaction signedOffchainTransaction, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {
            var txInput = new object[]
            {
                signedOffchainTransaction.h,
                signedOffchainTransaction.v,
                signedOffchainTransaction.r,
                signedOffchainTransaction.s,
                signedOffchainTransaction.rh,
                signedOffchainTransaction.rv,
                signedOffchainTransaction.rr,
                signedOffchainTransaction.rs,
                signedOffchainTransaction.nonce,
                signedOffchainTransaction.fee,
                signedOffchainTransaction.txUntilBlock,
                signedOffchainTransaction.beneficiary,
                signedOffchainTransaction.amount
        };
            return await executePaymentChannelsMethod("finalizeOffchainRelayerTransaction", txInput, calcNetFeeOnly, waitReceipt, cancellationToken);

        }

        /// <summary>
        /// Check the validity of an Offchain Payment payload signed by a Peer
        /// </summary>
        /// <param name="signedOffchainTransaction"></param>
        /// <returns></returns>
        public async Task<bool> checkOffchainSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var function = _paymentChannelsContract.GetFunction("checkOffchainSignature");
            string signer = await function.CallAsync<string>(signedOffchainTransaction.h, signedOffchainTransaction.v, signedOffchainTransaction.r, signedOffchainTransaction.s, signedOffchainTransaction.nonce, signedOffchainTransaction.fee, signedOffchainTransaction.beneficiary, signedOffchainTransaction.amount);
            var addressEqualityComparer = new AddressEqualityComparer();
            return addressEqualityComparer.Equals(signer, signedOffchainTransaction.signer);
            //return AddressUtil.Current.ConvertToChecksumAddress(signer) == AddressUtil.Current.ConvertToChecksumAddress(signedOffchainTransaction.signer);
        }


        /// <summary>
        /// Check the validity of an Offchain Payment payload signed by a Relayer
        /// </summary>
        /// <param name="signedOffchainTransaction"></param>
        /// <returns></returns>
        public async Task<bool> checkOffchainRelayerSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var function = _paymentChannelsContract.GetFunction("checkOffchainRelayerSignature");
            string signer = await function.CallAsync<string>(signedOffchainTransaction.h, signedOffchainTransaction.rh, signedOffchainTransaction.rv, signedOffchainTransaction.rr, signedOffchainTransaction.rs, signedOffchainTransaction.txUntilBlock);
            var addressEqualityComparer = new AddressEqualityComparer();
            return addressEqualityComparer.Equals(signer, signedOffchainTransaction.relayerId);
        }

        /// <summary>
        /// Convert a Offchain payment payload to byte[]
        /// </summary>
        /// <param name="signedOffchainTransaction"></param>
        /// <returns></returns>
        public async Task<byte[]> getFinalizeOffchainRelayerSignature(SignedOffchainTransaction signedOffchainTransaction)
        {
            var function = _paymentChannelsContract.GetFunction("getFinalizeOffchainRelayerSignature");
            return await function.CallAsync<byte[]>(signedOffchainTransaction.relayerId, signedOffchainTransaction.nonce, signedOffchainTransaction.fee, signedOffchainTransaction.beneficiary, signedOffchainTransaction.amount);
        }

        /// <summary>
        /// Generic method that invokes RSC methods
        /// </summary>
        /// <param name="method">The Smart contract method</param>
        /// <param name="input">THe input arguments</param>
        /// <param name="calcNetFeeOnly">Calculate network fees and return. Don't place Tx Onchain</param>
        /// <param name="waitReceipt">Wait for the Tx to be mined</param>
        /// <param name="cancellationToken"></param>
        /// <param name="value">When Tx is payable the amount to be transfered to Smart Contract</param>
        /// <returns>The Tx</returns>
        protected async Task<OutgoingTransaction> executePaymentChannelsMethod(string method, object[] input, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken, decimal? value = null)
        {
            var function = _paymentChannelsContract.GetFunction(method);

            return await executeBlockchainTransaction(_wallet.address, input, calcNetFeeOnly, function, waitReceipt, cancellationToken, recentToWei(value));
        }

    }
}
