using BankingApp.Data;
using BankingApp.Models;

    namespace BankingApp.Logic
    {
        public class AccountCreator 
        {
            private readonly EncryptionUtility _encryptionUtility;
            private readonly AccountQueries _accountQueries;
            public int NewAccountId { get; set; }
            
            public struct ValidatedCredentials
            {
                public string Username;
                public string Password;
                public string Salt;

            }

        public AccountCreator(EncryptionUtility encryptionUtility, AccountQueries accountQueries) 
            {
                _encryptionUtility = encryptionUtility;
                _accountQueries = accountQueries;
            }

            public AccountCreationResponse ValidateCredentials(string username, string password, string reEnteredPassword)
            {
                //uses a request-response model to supply meaningul output to the user in the event their credentials are/are not valid
                var response = new AccountCreationResponse();
                if (username == string.Empty)
                {
                    response.RequestSuccess = false;
                    response.Message = "Username cannot be blank.\n";
                    return response;
                }

                if (_accountQueries.CheckIfUserExists(username) != -1)
                {
                    response.RequestSuccess = false;
                    response.Message = "User name already exists, please choose a different user name.\n";
                    return response;
                }

                if (password == string.Empty)
                {
                    response.RequestSuccess = false;
                    response.Message = "Password cannot be blank.\n";
                    return response;
                }

                if (!CheckIfPasswordsMatch(password, reEnteredPassword))
                {
                    response.RequestSuccess = false;
                    response.Message = "Passwords do not match.\n";
                    return response;
                }

                var finalCredentials = SetFinalAccountCredentials(username, password);
                CreateAccount(finalCredentials);

                response.RequestSuccess = true;
                response.Message = "Account created successfully. Please log in to continue.\n";
                return response;
            }

            public ValidatedCredentials SetFinalAccountCredentials(string username, string password)
            {
                var validatedCredentials = new ValidatedCredentials
                {
                    Username = username,
                    Salt = _encryptionUtility.GenerateSalt()
                };
                validatedCredentials.Password =
                        _encryptionUtility.EncryptStringAndSalt(password, validatedCredentials.Salt);
                return validatedCredentials;
            }

            public void CreateAccount(ValidatedCredentials credentials)
            {
                var account = new Account { Username = credentials.Username, Password = credentials.Password, Salt = credentials.Salt, AccountId = NewAccountId };
                _accountQueries.WriteNewAccountCredentialsToDatabase(account);

            }
            public bool CheckIfPasswordsMatch(string userPassword, string reEnteredPassword)
            {
                if (userPassword == reEnteredPassword)
                {
                    return true;
                }

                return false;
            }                 
        }
    }