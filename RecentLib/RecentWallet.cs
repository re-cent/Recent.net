using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Signer;
using RecentLib.Models;
using System;
using System.Linq;
using static RecentLib.Constants.RecentProject;

namespace RecentLib
{
    public class RecentWallet
    {
        public WalletData createWallet()
        {

            var ecKey = EthECKey.GenerateKey();

            return new WalletData { address = ecKey.GetPublicAddress(), PK = ecKey.GetPrivateKey(), crypto = CryptoName, LastUpdated = DateTime.UtcNow };

        }


        public WalletData importWalletFromPK(string PK)
        {

            var address = EthECKey.GetPublicAddress(PK);
            return new WalletData { address = address, PK = PK, crypto = CryptoName, LastUpdated = DateTime.UtcNow };

        }

        public string createSeedPhrase()
        {
            Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
            return string.Join(" ", wallet.Words);
        }

        public WalletData createWalletFromSeedPhrase(string seedPhrase)
        {
            var wordList = seedPhrase.Split(" ".ToCharArray());
            Wallet wallet = new Wallet(seedPhrase, null);
            var account = wallet.GetAccount(0);
            return new WalletData { address = account.Address, PK = account.PrivateKey, crypto = CryptoName, LastUpdated = DateTime.UtcNow };
        }

    }
}
