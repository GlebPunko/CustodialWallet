using CustodialWallet.Domain.Dto.Balance;

namespace CustodialWallet.Domain.Dto.Response
{
    public class UserBalanceResponse
    {
        public Guid UserId { get; set; }
        public List<BalanceShortDto> Balances { get; set; }
    }
}
