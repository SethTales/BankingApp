using System;
using Microsoft.EntityFrameworkCore;
using BankingApp.Logic;
using BankingApp.Data;
using BankingApp.Models;

namespace BankingApp.Service
{
    class BankingAppMain
    {
        static void Main(string[] args)
        {
            //instantiate dependencies of AccountCreatorService and LoginService
            var encryptionUtility = new EncryptionUtility();
            var accountOptions = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase("BankingAppDatabase")
                .Options;    
            var accountDbContext = new AccountDbContext(accountOptions);
            var accountQueries = new AccountQueries(accountDbContext);
            var accountId = 1;

            while(true)
            {
                Console.WriteLine("Welcome to Taylor Bank. Please choose from the following menu options:");
                Console.WriteLine("a) Create new account");
                Console.WriteLine("b) Login");
                Console.WriteLine("c) Quit");

                var menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "a":
                        var accountCreator = new AccountCreator(encryptionUtility, accountQueries);
                        accountCreator.NewAccountId = accountId;
                        var accountCreated = false;
                        var creationAttempts = 0; //used as a flag to limit failed attempts so users don't get stuck in an endless loop
                        while (!accountCreated)
                        {
                            var newUsername = GetUserInput("Username:");
                            var newPassword = GetUserInput("Password:");
                            var reEnteredPassword = GetUserInput("Re-enter password:");
                            var accountCreatedResponse =
                                accountCreator.ValidateCredentials(newUsername, newPassword, reEnteredPassword);
                            accountCreated = accountCreatedResponse.RequestSuccess;
                            Console.WriteLine(accountCreatedResponse.Message);
                            creationAttempts++;

                            if (creationAttempts != 5) continue;
                            Console.WriteLine("Exiting after 5 failed account creation attempts.\n");
                            break;
                        }
                        accountId++;
                        break;
                    case "b":
                        var loggedIn = false;
                        var loginAttempts = 0;
                        var loggedInAccountId = -1;
                        var loginService = new LoginService(encryptionUtility, accountQueries);
                        while(!loggedIn)
                        {
                            var username = GetUserInput("Username:");
                            var password = GetUserInput("Password:");
                            var accountAuthenticatedResponse = loginService.AuthenticateCredentials(username, password);

                            loggedIn = accountAuthenticatedResponse.RequestSuccess;
                            loggedInAccountId = accountAuthenticatedResponse.AccountId;
                            Console.WriteLine(accountAuthenticatedResponse.Message);
                            loginAttempts++;

                            if (loginAttempts != 5) continue;
                            Console.WriteLine("Exiting after 5 failed login attempts.\n");
                            break;
                        }
                        //loggedInAccountId always equals -1 unless the account credentials have been successfully validated
                        if (loggedInAccountId != -1)
                        {
                            LoggedInMenuOptions(loggedInAccountId, ref loggedIn);
                        }
                        break;
                    case "c":
                        Quit();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please choose again.\n");
                        break;
                }
            } 
        }

        public static void LoggedInMenuOptions(int accountId, ref bool loggedIn)
        {
            //instanticate dependencies for depositService, withdrawalService and transactionService
            var loggedinAccountId = accountId;
            var transactionOptions = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseInMemoryDatabase("BankingAppDatabase")
                .Options;
            var transactionDbContext = new TransactionDbContext(transactionOptions);
            var transactionQueries = new TransactionQueries(transactionDbContext, loggedinAccountId);

            var accountHistoryService = new AccountHistoryService(transactionQueries);
            
            while (loggedIn)
            {
                Console.WriteLine("Please choose from the following menu options:\n");
                Console.WriteLine("a) Make a deposit");
                Console.WriteLine("b) Make a withdrawal");
                Console.WriteLine("c) Check your balance");
                Console.WriteLine("d) See your transaction history");
                Console.WriteLine("e) Log out");
                Console.WriteLine("f) Quit");

                var menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "a":
                        var depositService = new DepositService(transactionQueries);
                        var depositAmount = ValidateTransactionEntry("Deposit");
                        var depositMessage = depositService.SubmitTransaction(depositAmount);
                        Console.WriteLine(depositMessage);
                        break;
                    case "b":
                        var withdrawalService = new WithdrawalService(transactionQueries);
                        var withdrawalAmount = ValidateTransactionEntry("Withdrawal");
                        var withdrawalMessage = withdrawalService.SubmitTransaction(withdrawalAmount);
                        Console.WriteLine(withdrawalMessage);
                        break;
                    case "c":
                        Console.WriteLine($"Your account balance is: ${accountHistoryService.GetAccountBalance()}");
                        break;
                    case "d":
                        var transactionHistory = accountHistoryService.GetransactionHistory();

                        Console.WriteLine("Transaction History:\n");
                        foreach (var transaction in transactionHistory)
                        {
                            Console.WriteLine($"${transaction.Amount}");
                            Console.WriteLine(transaction.Timestamp);
                            Console.WriteLine("--------------------------------------------------");
                        }
                        break;
                    case "e":
                        loggedIn = false;
                        break;
                    case "f":
                        Quit();
                        break;
                    default:
                        Console.WriteLine("Invalid selection, please choose again.\n");
                        break;
                }
            }
        }
        public static void Quit()
        {
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static decimal ValidateTransactionEntry(string transactionType)
        {
            decimal amount;
            Console.WriteLine($"{transactionType} amount:");
            if (!decimal.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("Not a valid dollar amount");
                ValidateTransactionEntry(transactionType);
            }

            return amount;
        }

        public static string GetUserInput(string outputMessage)
        {
            Console.WriteLine(outputMessage);
            var inputVariable = Console.ReadLine();
            return inputVariable;
        }
    }
}
