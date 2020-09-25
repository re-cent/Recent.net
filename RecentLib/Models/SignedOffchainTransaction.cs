using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models
{
    /// <summary>
    /// A P2P Offchain Transaction signed by both a Peer and Relayer and settled by the Relayer
    /// </summary>
    public class SignedOffchainTransaction
    {
        /// <summary>
        /// Hash of Peer signature
        /// </summary>
        public byte[] h { get; set; }
        /// <summary>
        ///  Recovery id of Peer signature
        /// </summary>
        public uint v { get; set; }
        /// <summary>
        /// Output of the ECDSA Peer signature
        /// </summary>
        public byte[] r { get; set; }
        /// <summary>
        /// Output of the ECDSA Peer signature
        /// </summary>
        public byte[] s { get; set; }
        /// <summary>
        /// The P2P Tx unique identifier
        /// </summary>
        public string nonce { get; set; }
        /// <summary>
        /// The Relayer fee
        /// </summary>
        public uint fee { get; set; }
        /// <summary>
        /// The service provider address. The beneficiary of settlement
        /// </summary>
        public string beneficiary { get; set; }
        /// <summary>
        /// The amount of settlement in wei
        /// </summary>
        public BigInteger amount { get; set; }
        /// <summary>
        /// The peer address
        /// </summary>
        public string signer { get; set; }
        /// <summary>
        /// Hash of Relayer signature
        /// </summary>
        public byte[] rh { get; set; }
        /// <summary>
        /// Output of the ECDSA Relayer signature
        /// </summary>
        public uint rv { get; set; }
        /// <summary>
        /// Output of the ECDSA Relayer signature
        /// </summary>
        public byte[] rr { get; set; }
        /// <summary>
        /// Output of the ECDSA Relayer signature
        /// </summary>
        public byte[] rs { get; set; }
        /// <summary>
        /// The Releayer Owner address
        /// </summary>
        public string relayerId { get; set; }
        /// <summary>
        /// Block number that Relayer should proceed with the Tx Onchain 
        /// </summary>
        public uint txUntilBlock { get; set; }
    }
}
