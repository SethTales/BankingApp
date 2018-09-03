using System;
using System.Collections.Generic;
using BankingApp.Data;
using BankingApp.Models;

namespace BankingApp.Logic
{
    public class AccountHistoryService 
    {
        private readonly TransactionQueries _transactionQueries;

        public AccountHistoryService(TransactionQueries transactionQueries)
        {
            _transactionQueries = transactionQueries;
        }

        public decimal GetAccountBalance()
        {
            return _transactionQueries.GetAccountBalance();
        }

        public List<Transaction> GetransactionHistory()
        {
            return _transactionQueries.GetTransactionHistory();
            
        }


    }

    

}