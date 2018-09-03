namespace BankingApp.Logic
{
    //response objects for account creation and logging in
    public struct AccountCreationResponse
    {
        public bool RequestSuccess;
        public string Message;
    }

    public struct LoginResponse
    {
        public bool RequestSuccess;
        public string Message;
        public int AccountId;
    }
}
