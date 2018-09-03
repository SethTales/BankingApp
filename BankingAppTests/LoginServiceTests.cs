using System;
using Xunit;
using BankingApp.Logic;
using BankingApp.Data;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;


namespace BankingAppTests
{

    public class LoginServiceTests
    {
        private readonly EncryptionUtility _testEncryptionUtility;
        private readonly AccountQueries _testAccountQueries;
        private readonly AccountCreator _testAccountCreator;
        private readonly LoginService _testLoginService;
        int testAccountId;

        public LoginServiceTests()
        {
            _testEncryptionUtility = new EncryptionUtility();
            var testAccountOptions = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase("TestLoginDatabase")
                .Options;
            var testAccountDbContext = new AccountDbContext(testAccountOptions);
            _testAccountQueries = new AccountQueries(testAccountDbContext);
            _testAccountCreator = new AccountCreator(_testEncryptionUtility, _testAccountQueries);
            _testLoginService = new LoginService(_testEncryptionUtility, _testAccountQueries);
        }

        [Theory]
        [InlineData("badUser", "anything", "Incorrect username or password.\n", false, -1)]
        [InlineData("badUser", "", "Incorrect username or password.\n", false, -1)]
        public void TestUserDoesNotExistFailedAuthentication(string badUsername, string password,
            string expectedMessage,
            bool expectedSuccess, int expectedAccountId)
        {
            var testLoginResponse = _testLoginService.AuthenticateCredentials(badUsername, password);

            Assert.Equal(expectedMessage,testLoginResponse.Message);
            Assert.Equal(expectedSuccess, testLoginResponse.RequestSuccess);
            Assert.Equal(expectedAccountId, testLoginResponse.AccountId);
        }

        [Theory]
        [InlineData("testUser", "badPassword", "Incorrect username or password.\n", false, -1)]
        public void TestWrongPasswordFailedAuthentication(string username, string badPassword,
            string expectedMessage, bool expectedSuccess, int expectedAccountId)
        {
            _testAccountCreator.ValidateCredentials("testUser", "testPassword", "testPassword");
            var testLoginResponse = _testLoginService.AuthenticateCredentials(username, badPassword);

            Assert.Equal(expectedMessage, testLoginResponse.Message);
            Assert.Equal(expectedSuccess, testLoginResponse.RequestSuccess);
            Assert.Equal(expectedAccountId, testLoginResponse.AccountId);
        }

        [Theory]
        [InlineData("testUser", "testPassword", "Thank you for logging in. Please continue.\n", true, 1)]
        public void TestSuccessfulLogin(string username, string password,
            string expectedMessage, bool expectedSuccess, int expectedAccountId)
        {
            _testAccountCreator.NewAccountId = 1;
            _testAccountCreator.ValidateCredentials("testUser", "testPassword", "testPassword");
            var testLoginResponse = _testLoginService.AuthenticateCredentials(username, password);

            Assert.Equal(expectedMessage, testLoginResponse.Message);
            Assert.Equal(expectedSuccess, testLoginResponse.RequestSuccess);
            Assert.Equal(expectedAccountId, testLoginResponse.AccountId);
        }
    }

    
}