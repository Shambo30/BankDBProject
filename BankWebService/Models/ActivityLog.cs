namespace BankWebService.Models
{
    public class ActivityLog
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string Timestamp { get; set; }
    }
}

