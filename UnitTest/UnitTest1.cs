using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecentLib;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        const string NodeUrl = "http://ec2-52-59-205-93.eu-central-1.compute.amazonaws.com:8545";

        [TestMethod]
        public void TestMethod1()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            //Parallel.For(0, 100, i =>
            //{
            //    var l = lib.addRelayer($"https://2.google.com.{i}", "2.Google.{i}", true, 0.1m, false, null).Result;

            //});
            //for (int i = 0; i < 10; i++)
            //{
            //    var l = lib.addRelayer($"https://www.google.com.{i}", "Google.{i}", true, 0.1m, false, null).Result;
            //}

            //var r11 = lib.addRelayer("https://www.google.com1", "Google1", true, 0.1m, false, null).Result;
            //var r12 = lib.addRelayer("https://www.google.com2", "Google1", true, 0.1m, false, null).Result;

            //var r2 = lib.getRelayer("https://www.google.com1").Result;

            var r3 = lib.getRelayers().Result;

            var cid = lib.uploadBinary(File.ReadAllBytes(@"C:\Users\jzari_000\Pictures\img001.jpg")).Result;
            var ipfsUrl = lib.getIpfsCIDUrl(cid);
            File.WriteAllBytes(@"C:\Users\jzari_000\Pictures\" + cid + ".jpg", lib.downloadBinary(cid).Result);

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
        public void AddRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = lib.addRelayer("https://www.abc.com/", "Test", true, 12.1m,false, true, null).Result;


        }

        [TestMethod]
        public void updateRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = lib.updateRelayer("https://www.abc.com/", "Test 1", true, 12.8m, false, true, null).Result;
            var relayer = lib.getRelayer("https://www.abc.com/", true).Result;
            Assert.AreEqual(relayer.fee, 12.8m);

        }

        [TestMethod]
        public void depositToRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = lib.depositToRelayer("https://www.abc.com/",0.001m,1, false, true, null).Result;

        }


        private async Task<bool> stressDepostisToRelayerParallel()
        {

            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            var lib2 = new RecentCore("http://127.0.0.1:8545");
            var wallet2 = lib2.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");

            try
            {
                const int TaskCount = 5000;
                var tasks = new Task[TaskCount];
                for (int i = 0; i < TaskCount; i++)
                {
                    tasks[i] = lib.depositToRelayer("https://www.abc.com/", 0.000000000000001m, 1, false, true, null);
                }

                const int TaskCount2 = 5000;
                var tasks2 = new Task[TaskCount2];
                for (int i = 0; i < TaskCount2; i++)
                {
                    tasks2[i] = lib2.depositToRelayer("https://www.abc.com/", 0.000000000000001m, 1, false, true, null);
                }

                await Task.WhenAll(tasks);
                await Task.WhenAll(tasks2);

                // handle or rethrow the exceptions
            }

            catch (Exception Ex)
            {
                Assert.Fail("Exception!");
            }
            return true;
        }


        [TestMethod]
        public void stressDepostisToRelayer()
        {

            var alala= stressDepostisToRelayerParallel().Result;



        }

        [TestMethod]
        public void voteRelayer()
        {
            var lib = new RecentCore(NodeUrl);
            var wallet = lib.importWalletFromSeedPhrase("combine close before lawsuit asthma glimpse yard debate mixture stool adjust ride");
            var tx = lib.voteRelayer("https://www.abc.com/",2.8d, false, true, null).Result;

        }


    }
}
