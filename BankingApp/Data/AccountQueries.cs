using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using BankingApp.Models;

namespace BankingApp.Data
{
    public class AccountQueries
    {
        private readonly AccountDbContext _accountDbContext;
        public AccountQueries(AccountDbContext accountDbContext)
        {
            _accountDbContext = accountDbContext;
        }

        public void WriteNewAccountCredentialsToDatabase(Account newAccount)
        {
            _accountDbContext.Accounts.Add(newAccount);
            _accountDbContext.SaveChanges();
        }

        public int CheckIfUserExists(string username)
        {
            var queryUserNames = from account in _accountDbContext.Accounts
                where account.Username == username
                select account.AccountId;
            if (!queryUserNames.Any())
            {
                return -1;
            }
            return Convert.ToInt32(queryUserNames.FirstOrDefault());
        }

        public string GetSaltFromDatabase(int accountId)
        {
            var querySalt = from account in _accountDbContext.Accounts
                where account.AccountId == accountId
                select account.Salt;
            return querySalt.FirstOrDefault();
        }

        public string GetPasswordFromDatabase(int accountId)
        {
            var queryPassword = from account in _accountDbContext.Accounts
                where account.AccountId == accountId
                select account.Password;
            return queryPassword.FirstOrDefault();
        }
    }
}