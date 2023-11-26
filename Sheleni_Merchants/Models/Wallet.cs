using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public int OwnerId { get; set; }
        public decimal Balance { get; set; }
    }
}
