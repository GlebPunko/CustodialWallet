using CustodialWallet.Domain.Dto.Balance;

namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserBalanceResponse : ResponseBase
    {
        public Guid UserId { get; set; }
        public List<BalanceShortDto> Balances { get; set; }
    }
}
