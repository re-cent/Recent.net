using NBitcoin;
using Nethereum.Contracts;
using Nethereum.HdWallet;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using RecentLib.Models;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace RecentLib
{
    public partial class RecentCore
    {
	    private readonly string _nodeUrl;

		public RecentCore() 
			: this(Constants.RecentProject.NodeUrl)
		{ }

	    public RecentCore(string nodeUrl)
	    {
		    _nodeUrl = nodeUrl;
	    }

        internal WalletData _wallet { get; set; }
        internal Web3 _web3 { get; set; }



        protected async Task<OutgoingTransaction> executeBlockchainTransaction(string souceAddress, object[] input, bool calcNetFeeOnly, Function function, bool waitReceipt,CancellationTokenSource cancellationToken, HexBigInteger value = null)
        {
            var gas =await function.EstimateGasAsync(souceAddress, null, value, input);
            var gasPrice =await getGasPrice();
            var txId = "";
            if (!calcNetFeeOnly)
            {
                var txInput = new TransactionInput("", function.ContractAddress, souceAddress, gas, new HexBigInteger(gasPrice), value);

                if (waitReceipt)
                {
                    txId = (await function.SendTransactionAndWaitForReceiptAsync(txInput,cancellationToken, input)).TransactionHash;
                }
                else
                {
                    txId =await function.SendTransactionAsync(txInput, input);
                }
                
                
            }
            //var nonce=web3.Eth.TransactionManager.Account.NonceService.GetNextNonceAsync().Result;
            return new OutgoingTransaction { txId = txId, networkFee = Web3.Convert.FromWei(gas.Value * gasPrice), gasLimit = gas, gasPrice = gasPrice };
        }


        /// <summary>
        /// Create a Recent wallet
        /// </summary>
        /// <returns>Wallet data</returns>
        public WalletData createWallet()
        {

            var ecKey = EthECKey.GenerateKey();
            _wallet= new WalletData { address = ecKey.GetPublicAddress(), PK = ecKey.GetPrivateKey() };
            _web3 = new Web3(new Nethereum.Web3.Accounts.Account(_wallet.PK), _nodeUrl);
            return _wallet;

        }

        /// <summary>
        /// Get wallet
        /// </summary>
        /// <returns>Wallet data</returns>
        public WalletData getWallet()
        {
            return _wallet;
        }

        /// <summary>
        /// Encrypt wallet and return keystore
        /// </summary>
        /// <param name="PK">Password</param>
        /// <returns>Keystore</returns>
        public string encryptWallet(string password)
        {
            EthECKey ethECKey = new EthECKey(_wallet.PK);
            var service = new Nethereum.KeyStore.KeyStoreService();
            return service.EncryptAndGenerateDefaultKeyStoreAsJson(
                        password, ethECKey.GetPrivateKeyAsBytes(), ethECKey.GetPublicAddress());
        }

        /// <summary>
        /// Decrypt and setup wallet from Keystore
        /// </summary>
        /// <param name="PK">Password</param>
        /// <returns>The Wallet</returns>
        public WalletData importFromKeyStore(string keyStore, string password)
        {

            var service = new Nethereum.KeyStore.KeyStoreService();
            var key = new Nethereum.Signer.EthECKey(
                    service.DecryptKeyStoreFromJson(password, keyStore),
                    true);

            var address = EthECKey.GetPublicAddress(key.GetPrivateKey());
            _wallet = new WalletData { address = address, PK = key.GetPrivateKey() };
            _web3 = new Web3(new Nethereum.Web3.Accounts.Account(_wallet.PK), _nodeUrl);
            return _wallet;
        }

        /// <summary>
        /// Create a Recent wallet given a Private Key
        /// </summary>
        /// <param name="PK">The Private key</param>
        /// <returns>Wallet data</returns>
        public WalletData importWalletFromPK(string PK)
        {

            var address = EthECKey.GetPublicAddress(PK);
            _wallet=new WalletData { address = address, PK = PK};
            _web3 = new Web3(new Nethereum.Web3.Accounts.Account(_wallet.PK), _nodeUrl);
            return _wallet;

        }

        /// <summary>
        /// Create a seed phrase and a Recent wallet
        /// </summary>
        /// <returns>The seed phrase</returns>
        public string createAndRetrieveSeedPhrase()
        {
            Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
            var account = wallet.GetAccount(0);
            _wallet = new WalletData { address = account.Address, PK = account.PrivateKey };
            _web3 = new Web3(account, _nodeUrl);
            return string.Join(" ", wallet.Words);
        }


        /// <summary>
        /// Import a Recent wallet given a seed phrase
        /// </summary>
        /// <param name="seedPhrase"></param>
        /// <returns></returns>
        public WalletData importWalletFromSeedPhrase(string seedPhrase)
        {
            var wordList = seedPhrase.Split(" ".ToCharArray());
            Wallet wallet = new Wallet(seedPhrase, null);
            var account = wallet.GetAccount(0);
            _wallet = new WalletData { address = account.Address, PK = account.PrivateKey };
            _web3 = new Web3(account, _nodeUrl);
            return new WalletData { address = account.Address, PK = account.PrivateKey};
        }


        /// <summary>
        /// Get Recent last mined block
        /// </summary>
        /// <returns></returns>
        public async Task<ulong> getLastBlock()
        {
            return (ulong)(await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
        }

        /// <summary>
        /// Convert a decimal value to Wei
        /// </summary>
        /// <param name="value">The value in decimal</param>
        /// <returns></returns>
        public HexBigInteger recentToWei(decimal? value)
        {
            if (value.HasValue)
                return new HexBigInteger(Web3.Convert.ToWei(value.Value, Nethereum.Util.UnitConversion.EthUnit.Ether));
            else
                return null;
        }

        /// <summary>
        /// Convert a decimal value to Wei
        /// </summary>
        /// <param name="value">The value in decimal</param>
        /// <returns></returns>
        public BigInteger recentToWei(decimal value)
        {
            return Web3.Convert.ToWei(value, Nethereum.Util.UnitConversion.EthUnit.Ether);
        }

        /// <summary>
        /// Convert a BigInteger value in Wei to decimal
        /// </summary>
        /// <param name="wei">The value in BigInteger</param>
        /// <returns></returns>
        public decimal weiToRecent(BigInteger wei)
        {
            return Web3.Convert.FromWei(wei, Nethereum.Util.UnitConversion.EthUnit.Ether);
        }

        /// <summary>
        /// Get Recent Network current gas price
        /// </summary>
        /// <returns></returns>
        private async Task<BigInteger> getGasPrice()
        {
            return (await _web3.Eth.GasPrice.SendRequestAsync()).Value;
        }

        /// <summary>
        /// Get current wallet balance
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> getBalance()
        {
            return await getBalance(_wallet.address);
        }

        /// <summary>
        /// Get current wallet balance
        /// </summary>
        /// <returns></returns>
        private async Task<BigInteger> getBalanceAsBigInteger(string address)
        {
            return await _web3.Eth.GetBalance.SendRequestAsync(address);
        }

        /// <summary>
        /// Get address balance
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> getBalance(string address)
        {
            return weiToRecent(await getBalanceAsBigInteger(address));
        }



        /// <summary>
        /// Check transaction confirmations
        /// </summary>
        /// <param name="txId">The transaction hash</param>
        /// <returns>Tuple<Null => Unconfirmed else the number of confirmations, Succeeded or Failed></returns>
        public async Task<Tuple<ulong?,bool>> getTransactionConfirmations(string txId)
        {
            TransactionReceipt transactionReceipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txId);
            if (transactionReceipt != null)
            {
                return new Tuple<ulong?, bool> ((ulong)transactionReceipt.BlockNumber.Value - await getLastBlock(), transactionReceipt.Status.Value == 1);
            }
            else
            {
                return new Tuple<ulong?, bool>(null,false);
            }
        }

        /// <summary>
        /// Ensure that input address is a valid Recent address.
        /// </summary>
        /// <param name="address">Requested address</param>
        /// <returns>Formats input address, throw Exception when not a valid address</returns>
        public string ensureValidAddress(string address)
        {
            address = address.Replace("ethereum:", "");
            Nethereum.Util.AddressUtil AddrUtil = new AddressUtil();
            if (!AddrUtil.IsValidAddressLength(address))
                throw new Exception("Invalid address");

            return address.EnsureHexPrefix();
        }

        /// <summary>
        /// transfer coins from wallet to an address
        /// </summary>
        /// <param name="amount">The coins amount</param>
        /// <param name="destinationAddress">The destination address</param>
        /// <param name="gasPrice">Setup GasPrice, null get the current netowkr price</param>
        /// <param name="calcNetFeeOnly">When true calculates the network fee cost, else broadcast transaction to the network</param>
        /// <returns></returns>
        public async Task<OutgoingTransaction> transfer(decimal amount, string destinationAddress, BigInteger? gasPrice, bool calcNetFeeOnly, bool waitReceipt, CancellationTokenSource cancellationToken)
        {

            BigInteger gas = new BigInteger(21000);
            if (!gasPrice.HasValue)
            {
                gasPrice = await getGasPrice();
            }

            decimal networkFee = weiToRecent(gasPrice.Value * gas);

            var txId = "";
            if (!calcNetFeeOnly)
            {
                

                if (waitReceipt)
                {
                    txId = (await _web3.TransactionManager.SendTransactionAndWaitForReceiptAsync(new TransactionInput("", destinationAddress.EnsureHexPrefix(), _wallet.address.EnsureHexPrefix(), new HexBigInteger(gas), new HexBigInteger(gasPrice.Value), new HexBigInteger(recentToWei(amount))), cancellationToken)).TransactionHash;
                }
                else
                {
                    txId = await _web3.TransactionManager.SendTransactionAsync(new TransactionInput("", destinationAddress.EnsureHexPrefix(), _wallet.address.EnsureHexPrefix(), new HexBigInteger(gas), new HexBigInteger(gasPrice.Value), new HexBigInteger(recentToWei(amount))));
                }


            }
            return new OutgoingTransaction { txId = txId, networkFee = networkFee, gasPrice = gasPrice.Value, gasLimit = gas };
        }
    }
}
