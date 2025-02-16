using CustodialWallet.Domain.Dto.Balance;

namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserWithBalancesResponse : ResponseBase
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public List<BalanceDto> Balances { get; set; } = [];
    }
}
