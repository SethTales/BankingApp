
namespace BankingApp.Models
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public int AccountId { get; set; }
    }
}