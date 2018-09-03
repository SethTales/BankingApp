using System;

namespace BankingApp.Models
{
    public class Transaction
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid TransactionId { get; set; }
    }
}