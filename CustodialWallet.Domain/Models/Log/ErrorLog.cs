namespace CustodialWallet.Domain.Models.Log
{
    public class ErrorLog
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
