namespace BankWebService.Models
{
    //Updated to be a helper class for transaction controller
    public class TransactionRequest
    {
        public int AccountNumber { get; set; }
        public double Amount { get; set; }

    }
}
