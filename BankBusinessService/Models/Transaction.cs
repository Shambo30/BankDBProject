namespace BankBusinessService.Models
{
    public class Transaction
    {
        public int transaction_id { get; set; }
        public int account_number { get; set; }
        public double amount { get; set; }
        public DateTime transaction_date { get; set; } //Timestamps transactions into transaction table

    }
}
