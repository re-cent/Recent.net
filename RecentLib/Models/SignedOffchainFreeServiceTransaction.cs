using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RecentLib.Models
{
    /// <summary>
    /// A P2P Offchain Transaction should by signed by a Witness that received free content from a service provider 
    /// </summary>
    public class SignedOffchainFreeServiceTransaction
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
        /// The address of free service provider
        /// </summary>
        public string freeServiceProvider { get; set; }
        /// <summary>
        /// The address of the Validator
        /// </summary>
        public string validator { get; set; }
        /// <summary>
        /// Free content provided in Mbs
        /// </summary>
        public BigInteger freeMb { get; set; }
        /// <summary>
        /// The Epoch that Tx happened
        /// </summary>
        public uint epoch { get; set; }
        /// <summary>
        /// Peer address
        /// </summary>
        public string signer { get; set; }

    }
}
