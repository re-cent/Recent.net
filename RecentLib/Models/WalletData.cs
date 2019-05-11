using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RecentLib.Models
{
    /// <summary>
    /// Wallet Model
    /// </summary>
    public class WalletData
    {
        [Key]
        public string address { get; set; }

        [JsonIgnore]
        public string PK { get; set; }

        [NotMapped]
        public string crypto { get; set; }

        [JsonIgnore]
        public DateTime LastUpdated { get; set; }

    }
}
