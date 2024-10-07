namespace BankWebService.Models
{
    //Updated to be a helper class for transaction controller
    public class TransactionRequest
    {
        public int? SenderAccount { get; set; }      // The account from which money will be withdrawn (nullable)
        public int? RecipientAccount { get; set; }   // The account to which money will be deposited (nullable)
        public double Amount { get; set; }           // The amount to be transferred

    }
}
