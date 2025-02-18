namespace CustodialWallet.Domain.Dto.User
{
    public class UserWithBalancesDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Guid BalanceId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyShortName { get; set; }
        public string CurrencyFullName { get; set; }
    }
}
