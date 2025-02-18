namespace CustodialWallet.Domain.Dto.Balance
{
    public class BalanceDto
    {
        public Guid BalanceId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyShortName { get; set; }
        public string CurrencyFullName { get; set; }
    }
}
