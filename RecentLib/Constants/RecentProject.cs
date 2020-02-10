using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Constants
{
    public static class RecentProject
    {
        public static string BlockhainName = "Recent";
        public static string NodeUrl = "http://127.0.0.1:8545";
        public static string ProfileContract = "0xA91e33aD760407bB9582287D99Ef13B6a528aCa3";
        public static string PaymentChannelsContract = "0x9d5f44d379B0f0bD9b37988690D6DaFE2572659E";
        public static string ValidatorsContract = "0x2de434a49716ac2B82f7a1726E714E0804a98D5a";
        public static string BlockRewardContract="0x3FDc40A61642DB0C7B68d87Aec41F337c8C9c544";

        public static string UserProfileABI = ABIs.ProfileABI;
        public static string PaymentChannelsABI = ABIs.PaymentChannelsABI;
        public static string ValidatorsABI = ABIs.ValidatorsABI;

        public static decimal GasPrice = 1;

        //public static string prefix = "\x19Re-CentT Signed Message:\n32";
        

        public static string IpfsClientEndpoint = "https://ipfs.infura.io:5001";


        //public static string ipfsEndpoint = "https://cloudflare-ipfs.com/ipfs/";
        public static string IpfsGatewayEndpoint = "https://ipfs.infura.io/ipfs/";
        //public static string ipfsEndpoint = "https://ipfs.io/ipfs/";
    }
}
