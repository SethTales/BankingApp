using BankingApp.Data;
using System;

namespace BankingApp.Logic
{
    public class DepositService : TransactionBase
    {
        private readonly TransactionQueries _transactionQueries;

        public DepositService(TransactionQueries transactionQueries)
        {
            _transactionQueries = transactionQueries;
        }

        public override string SubmitTransaction(decimal amount)
        {
            if (amount < 0.01m)
            {
                return "Deposit must be at least 1 penny. Cannot deposit.";
            }

            _transactionQueries.WriteTransaction(FormatTransactionAmount(amount));
            return $"Successfully deposited ${amount}";

        }
    }

}