using System;
using Xunit;
using BankingApp.Logic;
using BankingApp.Data;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;


namespace BankingAppTests
{

    public class AccountCreatorServiceTests
    {
        private readonly EncryptionUtility _testEncryptionUtility;
        private readonly AccountQueries _testAccountQueries;
        private readonly AccountCreator _testAccountCreator;
        private readonly int _testAccountId;

        public AccountCreatorServiceTests()
        {
            _testEncryptionUtility = new EncryptionUtility();
            var testAccountOptions = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase("TestAccountCreatorDatabase")
                .Options;
            var testAccountDbContext = new AccountDbContext(testAccountOptions);
            _testAccountQueries = new AccountQueries(testAccountDbContext);
            _testAccountCreator = new AccountCreator(_testEncryptionUtility, _testAccountQueries);
        }

        [Theory]
        [InlineData("", "password", "password", false, "Username cannot be blank.\n")]
        [InlineData("user1", "", "password", false, "Password cannot be blank.\n")]
        [InlineData("user2", "pass", "word", false, "Passwords do not match.\n")]
        [InlineData("user", "password", "password", true, "Account created successfully. Please log in to continue.\n")]
        [InlineData("user", "password", "password", false,
            "User name already exists, please choose a different user name.\n")]

        public void AccountCreationTest(string username, string password1, string password2,
            bool expectedSuccess, string expectedMessage)
        {
            _testAccountCreator.NewAccountId = 1;
            var response = _testAccountCreator.ValidateCredentials(username, password1, password2);
            Assert.Equal(expectedMessage, response.Message);
            Assert.Equal(expectedSuccess, response.RequestSuccess);
        }

        [Theory]
        //the expectedHash is the result of computing the sha256 hash on 'hashedPassword' and 'salt'
        [InlineData("password",
            "94C8E2C37423A1189892F2E2F0D1EAE3", "90413C973AC9FC8D9BF468607BE13C494F4C085C636766EE26F1625E84ACC827")]
        public void CreateValidatedCredentialsTest(string plainTextPassword, string salt,
            string expectedHash)
        {
            var hashedSaltAndPassword = _testEncryptionUtility.EncryptStringAndSalt(plainTextPassword, salt);
            Assert.Equal(expectedHash, hashedSaltAndPassword);
        }

        [Theory]
        [InlineData("same", "different", false)]
        [InlineData("same", "same", true)]
        public void TestCheckIfPasswordsMatch(string password1, string password2, bool expected)
        {
            var result = _testAccountCreator.CheckIfPasswordsMatch(password1, password2);
            Assert.Equal(result, expected);
        }

        [Theory]
        [InlineData("newUser", "password", "94C8E2C37423A1189892F2E2F0D1EAE3", 2)]
        public void TestWritingNewAccountToDatabase(string newUser, string password, string salt, int accountId)
        {
            var newAccount = new Account
            {
                Username = newUser,
                AccountId = accountId,
                Password = password,
                Salt = salt
            };

            _testAccountQueries.WriteNewAccountCredentialsToDatabase(newAccount);
            var expectedAccountId = _testAccountQueries.CheckIfUserExists(newUser);

            Assert.Equal(accountId, expectedAccountId);
        }
}
}
