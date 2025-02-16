namespace CustodialWallet.Domain.Entity.Balance
{
    public class BalanceEntity
    {
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }
    }
}
