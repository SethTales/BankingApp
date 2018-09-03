using BankingApp.Models;

namespace BankingApp.Logic
{
    public abstract class TransactionBase
    {
        public decimal FormatTransactionAmount(decimal transactionAmount)
        {
            //formats transactionAmount to 2 decimal places
            decimal result;
            var tAmount = transactionAmount.ToString("F");
            decimal.TryParse(tAmount, out result);
            return result;
        }
        public abstract string SubmitTransaction(decimal quantity);
    }
    
}