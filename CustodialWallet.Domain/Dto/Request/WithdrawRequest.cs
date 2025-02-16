namespace CustodialWallet.Domain.Dto.Request
{
    public class WithdrawRequest
    {
        public Guid CurrencyId { get; set; } 
        public decimal Amount { get; set; } 
    }
}
