using System;
using System.Collections.Generic;
using BankingApp.Models;
using System.Linq;
using Remotion.Linq.Utilities;

namespace BankingApp.Data
{
    public class TransactionQueries
    {
        private readonly TransactionDbContext _transactionDbContext;
        private readonly int _accountId;

        public TransactionQueries(TransactionDbContext transactionDbContext, int accountId)
        {
            _transactionDbContext = transactionDbContext;
            _accountId = accountId;
        }

        public Transaction WriteTransaction(decimal quantity)
        {
            Transaction transaction = new Transaction
            {
                AccountId = _accountId,
                Amount = quantity,
                Timestamp = DateTime.Now,
                TransactionId = Guid.NewGuid()
            };
            _transactionDbContext.Add(transaction);
            _transactionDbContext.SaveChanges();
            return transaction;
        }

        public List<Transaction> GetTransactionHistory()
        {
            var transactionList = new List<Transaction>();
            
            var query =
                from transaction in _transactionDbContext.Transactions
                where transaction.AccountId == _accountId
                select transaction;

            transactionList.AddRange(query);
            return transactionList;
        }

        public decimal GetAccountBalance()
        {
            var transactionList = new List<Transaction>();
            decimal accountBalance = 0;
           
            var query =
                from transaction in _transactionDbContext.Transactions
                where transaction.AccountId == _accountId
                select transaction;

            transactionList.AddRange(query);

            for (var i = 0; i < transactionList.Count; i++)
            {
                accountBalance += transactionList[i].Amount;
            }

            return accountBalance;
        }
    }

}