namespace CustodialWallet.Domain.Dto.Request
{
    public class DepositRequest
    {
        public Guid CurrencyId { get; set; } 
        public decimal Amount { get; set; } 
    }
}
