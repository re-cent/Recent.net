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
        public static string ProfileContract = "0x2A263c3264f5fba02202E4F7023a97Fb53fd0B19";
        public static string PaymentChannelsContract = "0x062b63831141499938fa200AB4d864e4a194018A";

        public static string UserProfileABI = ABIs.ProfileABI;
        public static string PaymentChannelsABI = ABIs.PaymentChannelsABI;

        public static decimal GasPrice = 1;
        

        public static string IpfsClientEndpoint = "https://ipfs.infura.io:5001";


        //public static string ipfsEndpoint = "https://cloudflare-ipfs.com/ipfs/";
        public static string IpfsGatewayEndpoint = "https://ipfs.infura.io/ipfs/";
        //public static string ipfsEndpoint = "https://ipfs.io/ipfs/";
    }
}
