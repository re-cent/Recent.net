using RecentLib.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecentLib.Models
{
    public class Validator
    {
        public string address { get; set; }
        public decimal totalStakingFunds { get; set; }
        public decimal stakingFunds { get; set; }
        public decimal witnessesFunds { get; set; }
        public decimal freeMbs { get; set; }
        public List<string> witnesses { get; set; }
        public List<string> freeServiceProviders { get; set; }
        public AddressStatusData addressStatusData { get; set; }
    }
}
