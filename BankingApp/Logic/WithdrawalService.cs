using System;
using BankingApp.Data;
using BankingApp.Models;


namespace BankingApp.Logic
{
    public class WithdrawalService : TransactionBase
    {
        private readonly TransactionQueries _transactionQueries;

        public WithdrawalService(TransactionQueries transactionQueries)
        {
            _transactionQueries = transactionQueries;
        }

        public override string SubmitTransaction(decimal amount)
        {
            if (amount < 0.01m)
            {
                return "Withdrawal must be at least 1 penny. Cannot withdraw.";
            }

            if (CheckIfAmountWillOverDraw(amount))
            {
                return $"Withdrawal amount is greater than account balance. Cannot withdraw.";
            }

            _transactionQueries.WriteTransaction(FormatTransactionAmount(-amount));
            return $"Successfully withdrew ${amount}";
        }

        private bool CheckIfAmountWillOverDraw(decimal wAmount)
        {
            if (wAmount > _transactionQueries.GetAccountBalance())
            {
                return true;
            }

            return false;
        }
    }

}