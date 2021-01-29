using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecentLib;
using RecentLib.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        const string Node2Url = "http://ec2-3-124-182-37.eu-central-1.compute.amazonaws.com:8545";
        const string NodeUrl = "http://ec2-3-124-182-37.eu-central-1.compute.amazonaws.com:8545";


        [TestMethod]
        public void getRelayers()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            var r3 = lib.getRelayers(null).Result;

            //var cid = lib.uploadBinary(File.ReadAllBytes(@"C:\Users\jzari_000\Pictures\img001.jpg")).Result;
            //var ipfsUrl = lib.getIpfsCIDUrl(cid);
            //File.WriteAllBytes(@"C:\Users\jzari_000\Pictures\" + cid + ".jpg", lib.downloadBinary(cid).Result);

        }


        [TestMethod]
        public void Balance()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var coins = lib.getBalance().Result;
            var coinsAsWei = lib.recentToWei(coins);


        }

        [TestMethod]
        public void TestBlockchain()
        {
            var lib = new RecentCore(Node2Url);
            var wallet = lib.importWalletFromPK("E5ADE4B50BA041A9C77DBA91401BEA949393F2C24433B0338702E7AE06443089");
            var balance = lib.getBalance().Result;
            var txid = lib.transfer(0.01m, "0x3d176d013550b48974c1d2f0b18c6df1ff71391e", false, true, null).Result;


            wallet = lib.importWalletFromPK("B68811986F995A45C66CF30D7C9A015268A1BB2E4697D6DBB23D7B96FC3607B0");
            balance = lib.getBalance().Result;
            txid = lib.transfer(0.01m, "0x3d176d013550b48974c1d2f0b18c6df1ff71391e", false, true, null).Result;
        }

        [TestMethod]
        public void RegisterRelayer()
        {
            var relayerLib = new RecentCore(NodeUrl);
            var relayerWallet = relayerLib.importWalletFromPK("E5ADE4B50BA041A9C77DBA91401BEA949393F2C24433B0338702E7AE06443089");

            var currentEpoch = relayerLib.getCurrentRelayersEpoch().Result;
            uint maxUsers = 10;
            decimal maxCoins = 1000m;
            uint maxTxThroughput = 1;

            var requiredAmount = relayerLib.getFundRequiredForRelayer(maxUsers, maxCoins, maxTxThroughput).Result;
            var tx = relayerLib.requestRelayerLicense(currentEpoch, "https://127.0.0.1:5001/", $"Beter test Epoch {currentEpoch}", 12.1m, maxUsers, maxCoins, maxTxThroughput, 1000, requiredAmount, false, true, null).Result;

        }


        [TestMethod]
        public void CreateSeedPhrase()
        {
            var lib = new RecentCore(NodeUrl);
            var phrase = lib.createAndRetrieveSeedPhrase();

        }

        [TestMethod]
        public void Flow()
        {
            var relayerLib = new RecentCore(NodeUrl);
            var relayerWallet = relayerLib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            var relayers = relayerLib.getRelayers(null, true, relayerWallet.address).Result;

            var currentEpoch = relayerLib.getCurrentRelayersEpoch().Result;
            var relayer = relayerLib.getRelayer(currentEpoch, relayerWallet.address).Result;
            var requiredAmount = relayerLib.getFundRequiredForRelayer(10, 10, 1).Result;
            if (relayer.maxUsers == 0)
            {

                var tx = relayerLib.requestRelayerLicense(currentEpoch, "https://localhost:5001/", $"Test Epoch {currentEpoch}", 12.1m, 10, 10, 1, 1000, requiredAmount, false, true, null).Result;
                relayer = relayerLib.getRelayer(currentEpoch, relayerWallet.address).Result;
            }
            else
            {
                currentEpoch += 1;
                var nextEpochRelayer = relayerLib.getRelayer(currentEpoch, relayerWallet.address).Result;
                if (nextEpochRelayer.maxUsers == 0)
                {
                    var tx = relayerLib.requestRelayerLicense(currentEpoch, "https://localhost:5001/", $"Test Epoch {currentEpoch}", 12.1m, 10, 10, 1, 1000, requiredAmount, false, true, null).Result;
                    nextEpochRelayer = relayerLib.getRelayer(currentEpoch, relayerWallet.address).Result;
                }

            }




            var userLib = new RecentCore(NodeUrl);
            var userWallet = userLib.importWalletFromPK("E5ADE4B50BA041A9C77DBA91401BEA949393F2C24433B0338702E7AE06443089");
            var userBalance = userLib.getBalance().Result;
            if (userBalance == 0)
            {
                var tx = userLib.transfer(0.01m, userWallet.address, false, true, null).Result;
                Assert.AreEqual(userLib.getBalance().Result, 0.01m);
            }
            var currentBlock = userLib.getLastBlock().Result;

            var userBalanceOnRelayer = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
            if (userBalanceOnRelayer.lockUntilBlock < currentBlock && userBalanceOnRelayer.balance > 0m)
            {
                var tx = userLib.withdrawFundsFromRelayer(relayer.owner, userBalanceOnRelayer.balance, false, true, null).Result;
                userBalanceOnRelayer = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
                Assert.AreEqual(userBalanceOnRelayer.balance, 0m);
            }
            if (userBalanceOnRelayer.balance == 0m)
            {
                var tx = userLib.depositToRelayer(relayer.owner, 0.01m, currentBlock + 10, false, true, null).Result;
                userBalanceOnRelayer = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
                relayer = userLib.getRelayer(relayer.owner, true, userWallet.address).Result;
                Assert.AreEqual(relayer.currentUsers, (uint)1);
                Assert.AreEqual(userBalanceOnRelayer.balance, 0.01m);
            }


            var serviceProviderLib = new RecentCore(NodeUrl);
            var serviceProviderWallet = serviceProviderLib.importWalletFromPK("B68811986F995A45C66CF30D7C9A015268A1BB2E4697D6DBB23D7B96FC3607B0");

            var beneficiaryAddress = "0xd316413c82bc4a23c2b52d43504f91c15f906208";

            var nonce = Guid.NewGuid().ToString("N");
            var offchainPaymentAmount = 0m;
            for (int i = 0; i < 1; i++)
            {
                var delta = 0.001m;
                offchainPaymentAmount += 0.001m;
                var offchainTx = new SignedOffchainTransaction
                {
                    amount = userLib.recentToWei(offchainPaymentAmount),
                    beneficiary = beneficiaryAddress,
                    fee = (uint)(relayer.fee * 10m),
                    nonce = nonce,
                    relayerId = relayer.owner
                };

                var signedTx = userLib.signOffchainPayment(offchainTx);
                var signerTest = userLib.checkOffchainSignature(signedTx).Result;



                currentBlock = relayerLib.getLastBlock().Result;
                offchainTx.txUntilBlock = currentBlock + relayer.offchainTxDelay;

                var signedFromRelayerTx = relayerLib.relayerSignOffchainPayment(signedTx).Result;
                var relayerTest = relayerLib.checkOffchainRelayerSignature(signedTx).Result;

                var beneficiaryBalanceBefore = userLib.getBalance(beneficiaryAddress).Result;
                var userBalanceOnRelayerBefore = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
                var relayerBalanceBefore = userLib.getBalance(relayer.owner).Result;
                var signerBeneficiaryRelationForNonceBefore = userLib.userToBeneficiaryFinalizedAmountForNonce(userWallet.address, beneficiaryAddress, relayer.owner, nonce).Result;



                var txOutput = serviceProviderLib.finalizeOffchainRelayerTransaction(signedFromRelayerTx, false, true, null).Result;


                var signerBeneficiaryRelationForNonceAfter = userLib.userToBeneficiaryFinalizedAmountForNonce(userWallet.address, beneficiaryAddress, relayer.owner, nonce).Result;
                var beneficiaryBalanceAfter = userLib.getBalance(beneficiaryAddress).Result;
                var userBalanceOnRelayerAfter = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
                var relayerBalanceAfter = userLib.getBalance(relayer.owner).Result;


                var fee = delta * relayer.fee / 100m;
                Assert.AreEqual(signerBeneficiaryRelationForNonceBefore + delta, signerBeneficiaryRelationForNonceAfter);
                Assert.AreEqual(relayerBalanceBefore + fee, relayerBalanceAfter);
                Assert.AreEqual(beneficiaryBalanceBefore + delta - fee, beneficiaryBalanceAfter);
                Assert.AreEqual(userBalanceOnRelayerBefore.balance - delta, userBalanceOnRelayerAfter.balance);

            }



            offchainPaymentAmount = 0.001m;
            nonce = Guid.NewGuid().ToString("N");


            var offchainTxPenaltyFunded = new SignedOffchainTransaction { amount = userLib.recentToWei(offchainPaymentAmount), beneficiary = beneficiaryAddress, fee = (uint)(relayer.fee * 10m), nonce = nonce, relayerId = relayer.owner };

            var signedTxPenaltyFunded = userLib.signOffchainPayment(offchainTxPenaltyFunded);
            var signerTestPenaltyFunded = userLib.checkOffchainSignature(signedTxPenaltyFunded);



            currentBlock = relayerLib.getLastBlock().Result;
            offchainTxPenaltyFunded.txUntilBlock = currentBlock + 1;

            var signedFromRelayerTxPenaltyFunded = relayerLib.relayerSignOffchainPayment(signedTxPenaltyFunded).Result;
            var relayerTestPenaltyFunded = relayerLib.checkOffchainRelayerSignature(signedTxPenaltyFunded).Result;

            var beneficiaryBalanceBeforePenaltyFunded = userLib.getBalance(beneficiaryAddress).Result;
            var userBalanceOnRelayerBeforePenaltyFunded = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
            var relayerBalanceBeforePenaltyFunded = userLib.getBalance(relayer.owner).Result;
            var signerBeneficiaryRelationForNonceBeforePenaltyFunded = userLib.userToBeneficiaryFinalizedAmountForNonce(userWallet.address, beneficiaryAddress, relayer.owner, nonce).Result;

            var relayerPenaltyFundsBefore = relayerLib.getRelayer(relayer.owner).Result.remainingPenaltyFunds;

            var txOutputPenaltyFunded = serviceProviderLib.finalizeOffchainRelayerTransaction(signedFromRelayerTxPenaltyFunded, false, true, null).Result;


            var signerBeneficiaryRelationForNonceAfterPenaltyFunded = userLib.userToBeneficiaryFinalizedAmountForNonce(userWallet.address, beneficiaryAddress, relayer.owner, nonce).Result;
            var beneficiaryBalanceAfterPenaltyFunded = userLib.getBalance(beneficiaryAddress).Result;
            var userBalanceOnRelayerAfterPenaltyFunded = userLib.getUserDepositOnRelayer(userWallet.address, relayer.owner).Result;
            var relayerBalanceAfterPenaltyFunded = userLib.getBalance(relayer.owner).Result;
            var relayerPenaltyFundsAfter = relayerLib.getRelayer(relayer.owner).Result.remainingPenaltyFunds;

            var feePenaltyFunded = 0m;
            Assert.AreEqual(relayerPenaltyFundsBefore - offchainPaymentAmount, relayerPenaltyFundsAfter);
            Assert.AreEqual(signerBeneficiaryRelationForNonceBeforePenaltyFunded + offchainPaymentAmount, signerBeneficiaryRelationForNonceAfterPenaltyFunded);
            Assert.AreEqual(relayerBalanceBeforePenaltyFunded + feePenaltyFunded, relayerBalanceAfterPenaltyFunded);
            Assert.AreEqual(beneficiaryBalanceBeforePenaltyFunded + offchainPaymentAmount - feePenaltyFunded, beneficiaryBalanceAfterPenaltyFunded);
            Assert.AreEqual(userBalanceOnRelayerBeforePenaltyFunded.balance, userBalanceOnRelayerAfterPenaltyFunded.balance);


        }


        [TestMethod]
        public void validatorAsCandidate()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var balance = lib.getBalance().Result;
            decimal witnessesFunds = 100m;
            var currentBalance = lib.getBalance().Result;
            var epoch = lib.getCurrentValidatorsEpoch().Result + 1;
            var requiredStakingFunds = lib.getRequiredStakingFunds(epoch).Result;
            var candidates = lib.getCandidatesDetailsByEpoch(epoch).Result;
            var totalRequiedFunds = requiredStakingFunds + witnessesFunds;
            if (candidates.Select(u => u.address.ToLower()).ToList().IndexOf(wallet.address.ToLower()) == -1)
            {
                if (currentBalance < totalRequiedFunds + 1m)
                {
                    var validator2Lib = new RecentCore(NodeUrl);
                    var validator2Wallet = validator2Lib.importWalletFromPK("E5ADE4B50BA041A9C77DBA91401BEA949393F2C24433B0338702E7AE06443089");
                    var validato2Balance = validator2Lib.getBalance().Result;

                    var txTransfer = validator2Lib.transfer(totalRequiedFunds - currentBalance + 1m, wallet.address, false, true, null).Result;


                }

                var tx = lib.validatorAsCandidate(requiredStakingFunds, witnessesFunds, false, true, null).Result;

            }



        }

        [TestMethod]
        public void voteValidatorAsWitness()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var epoch = lib.getCurrentValidatorsEpoch().Result + 1;

            var candidates = lib.getCandidatesDetailsByEpoch(epoch).Result;
            var candidate = candidates.FirstOrDefault();
            var balance = lib.getBalance().Result;
            var requiredBalance = lib.witnessRequiredBalancePercent().Result;
            if (candidate != null)
            {
                var tx = lib.voteValidatorAsWitness(candidate.address, balance * requiredBalance / 100m, false, true, null).Result;
            }

        }

        [TestMethod]
        public void voteValidatorAsServiceProvider()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var epoch = lib.getCurrentValidatorsEpoch().Result + 1;

            var candidates = lib.getCandidatesDetailsByEpoch(epoch).Result;
            var candidate = candidates.FirstOrDefault();
            uint freeMBs = 1;
            var balance = lib.getBalance().Result;
            if (candidate != null)
            {
                var tx = lib.voteValidatorAsServiceProvider(candidate.address, freeMBs, false, true, null).Result;
            }

        }


        [TestMethod]
        public void updateRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var relayerWallet = lib.importWalletFromPK("E5ADE4B50BA041A9C77DBA91401BEA949393F2C24433B0338702E7AE06443089");
            var currentEpoch = lib.getCurrentRelayersEpoch().Result;
            var relayer = lib.getRelayer(currentEpoch, relayerWallet.address).Result;
            var tx = lib.updateRelayer(currentEpoch, "http://192.168.1.15", relayer.name, relayer.fee, relayer.offchainTxDelay, false, false, null).Result;
            relayer = lib.getRelayer(currentEpoch, relayerWallet.address).Result;
            Assert.AreEqual(relayer.domain, "http://192.168.1.15");

        }

        [TestMethod]
        public void depositToRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = lib.depositToRelayer(wallet.address, 0.000000000000000001m, 100000, false, true, null).Result;

        }

        [TestMethod]
        public void testSignature()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = new SignedOffchainTransaction { amount = lib.recentToWei(1.2m), beneficiary = wallet.address, fee = (uint)(12.1m * 10m), nonce = Guid.NewGuid().ToString("N"), relayerId = wallet.address };
            var signedTx = lib.signOffchainPayment(tx);
            var signerTest = lib.checkOffchainSignature(signedTx).Result;
            var signedFromRelayerTx = lib.relayerSignOffchainPayment(signedTx).Result;
            var relayerTest = lib.checkOffchainRelayerSignature(signedTx).Result;
        }

        private async Task<bool> stressDepostisToRelayerParallel()
        {

            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            var lib2 = new RecentCore(Node2Url);
            var wallet2 = lib2.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            try
            {
                const int TaskCount = 1000;
                var tasks = new Task[TaskCount];
                for (int i = 0; i < TaskCount; i++)
                {
                    tasks[i] = lib.depositToRelayer("https://www.abc.com/", 0.000000000000001m, 1, false, false, null);
                }

                const int TaskCount2 = 1000;
                var tasks2 = new Task[TaskCount2];
                for (int i = 0; i < TaskCount2; i++)
                {
                    tasks2[i] = lib2.depositToRelayer("https://www.abc.com/", 0.000000000000001m, 1, false, false, null);
                }

                await Task.WhenAll(tasks);
                await Task.WhenAll(tasks2);

                // handle or rethrow the exceptions
            }

            catch (Exception)
            {
                Assert.Fail("Exception!");
            }
            return true;
        }


        [TestMethod]
        public void stressDepositsToRelayer()
        {

            var alala = stressDepostisToRelayerParallel().Result;



        }

        [TestMethod]
        [DataRow("0xb2e21d21637f13f1d1d8b15ede8b7b5743908a72cf88e5040de22eb2325e786f")]
        public void transactionConfirmations(string txId)
        {

            var lib = new RecentCore(NodeUrl);


            var ret = lib.getTransactionConfirmations(txId).Result;


        }




    }
}
