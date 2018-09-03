using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BankingApp.Logic;
using BankingApp.Data;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;


namespace BankingAppTests
{

    public class TransactionTests
    {
        private readonly TransactionDbContext _testTransactionDbContext;
        private readonly TransactionQueries _testTransactionQueries;
        private readonly DepositService _testDepositService;
        private readonly WithdrawalService _testWithdrawalService;
        private readonly AccountHistoryService _testAccountHistoryService;
        public int LoggedInAccountId { get; set; }

        public TransactionTests()
        {
            LoggedInAccountId = 1;
            var testTransactionOptions = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseInMemoryDatabase("TestTransactionsDatabase")
                .Options;
            _testTransactionDbContext = new TransactionDbContext(testTransactionOptions);
            _testTransactionQueries = new TransactionQueries(_testTransactionDbContext, LoggedInAccountId);
            _testDepositService = new DepositService(_testTransactionQueries);
            _testWithdrawalService = new WithdrawalService(_testTransactionQueries);
            _testAccountHistoryService = new AccountHistoryService(_testTransactionQueries);
        }

        [Theory]
        [InlineData(100, 1, 100.00)]
        [InlineData(123.345, 1, 123.35)]
        [InlineData(999.99999, 1, 1000)]
        [InlineData(123.344, 1, 123.34)]

        public void Test_IfDepositValid_WriteToDatabase(decimal amount, int expectedAccountId, decimal expectedAmount)
        {
            _testDepositService.SubmitTransaction(amount);
            var testTransactions = _testAccountHistoryService.GetransactionHistory();

            Assert.Equal(expectedAccountId, testTransactions.Last().AccountId);
            Assert.Equal(expectedAmount, testTransactions.Last().Amount);

        }

        [Theory]
        [InlineData(0, "Deposit must be at least 1 penny. Cannot deposit.")]
        [InlineData(0.001, "Deposit must be at least 1 penny. Cannot deposit.")]
        [InlineData(0.009, "Deposit must be at least 1 penny. Cannot deposit.")]
        [InlineData(-5, "Deposit must be at least 1 penny. Cannot deposit.")]
        public void Test_IfDepositNotValid_DoNotWriteToDatabase(decimal amount, string expectedMessage)
        {
            var testBadDepositMessage = _testDepositService.SubmitTransaction(amount);
            
            Assert.Equal(expectedMessage, testBadDepositMessage);
        }

        [Theory]
        [InlineData(150, 350.00)]
        [InlineData(133.33, 366.67)]
        [InlineData(499.99, 0.01)]
        [InlineData(500, 0)]
        public void Test_IfWithdrawalValid_WriteToDatabase(decimal amount, decimal expectedAccountBalance)
        {
            //clear the database to get a clean account balance
            _testTransactionDbContext.Database.EnsureDeleted();
            _testDepositService.SubmitTransaction(500);

            _testWithdrawalService.SubmitTransaction(amount);
            var testAccountBalance = _testAccountHistoryService.GetAccountBalance();

            Assert.Equal(expectedAccountBalance, testAccountBalance);
        }

        [Theory]
        [InlineData(500.01, "Withdrawal amount is greater than account balance. Cannot withdraw.")]
        [InlineData(600, "Withdrawal amount is greater than account balance. Cannot withdraw.")]
        [InlineData(0.009, "Withdrawal must be at least 1 penny. Cannot deposit.")]
        [InlineData(-0.01, "Withdrawal must be at least 1 penny. Cannot deposit.")]
        [InlineData(-100, "Withdrawal must be at least 1 penny. Cannot deposit.")]
        public void Test_IfWithdrawalNotValid_DoNotWriteToDatabase(decimal amount, string expectedMessage)
        {
            //clear the database to get a clean account balance
            _testTransactionDbContext.Database.EnsureDeleted();
            _testDepositService.SubmitTransaction(500);

            var testBadWithdrawalMessage = _testWithdrawalService.SubmitTransaction(amount);

            Assert.Equal(expectedMessage, testBadWithdrawalMessage);
        }


    }
}