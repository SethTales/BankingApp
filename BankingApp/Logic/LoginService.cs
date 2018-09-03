using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BankingApp.Data;

namespace BankingApp.Logic
{
    public class LoginService 
    {
        private readonly EncryptionUtility _encryptionUtility;
        private readonly AccountQueries _accountQueries;


        public LoginService(EncryptionUtility encryptionUtility, AccountQueries accountQueries) 
        {
            _encryptionUtility = encryptionUtility;
            _accountQueries = accountQueries;
        }

        public LoginResponse AuthenticateCredentials(string username, string password)
        {
            var response = new LoginResponse();
            //authentication consists of:
            //1) checking if the user exists
            //2) if they exist, checking if entered password matches stored password
            //an accountId of -1 and RequestSuccess = false indicates a failure
            response.AccountId = _accountQueries.CheckIfUserExists(username);

            if (response.AccountId == -1)
            {
                response.Message = "Incorrect username or password.\n";
                response.RequestSuccess = false;
                return response;
            }

            if (!CheckIfPasswordsMatch(password, response.AccountId))
            {
                response.Message = "Incorrect username or password.\n";
                response.RequestSuccess = false;
                response.AccountId = -1;
                return response;
            }

            response.Message = "Thank you for logging in. Please continue.\n";
            response.RequestSuccess = true;
            return response;
        }

        public bool CheckIfPasswordsMatch(string plainTextPassword, int accountId)
        {
            //compares stored password (hash of password + randomly generated salt) with hash of entered password and the stored salt
            var storedPassword = _accountQueries.GetPasswordFromDatabase(accountId);
            var hashedEnteredPassword = _encryptionUtility.EncryptStringAndSalt(plainTextPassword, _accountQueries.GetSaltFromDatabase(accountId));

            if (storedPassword != hashedEnteredPassword)
            {
                return false;
            }

            return true;
       }               

    }
}