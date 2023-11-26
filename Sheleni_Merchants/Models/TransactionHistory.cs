using System;
using System.Collections.Generic;
using System.Text;

namespace Sheleni_Merchants.Models
{
    public class TransactionHistory
    {
        public int MerchantId { get;set; }
        public decimal Amount { get;set; }
        public DateTime TransactionDate { get;set; } = DateTime.Now;
        public TimeSpan TransactionTime { get;set; }
        public string Description { get;set; }
        public string TransactionType { get;set; }
        public string TransactionColour { get;set; }
        public string TransactionIcon { get;set; }
        public int CustomerId { get;set; }
        public int Income { get;set; }
    }
}
