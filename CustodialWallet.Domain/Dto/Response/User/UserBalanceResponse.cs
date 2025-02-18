using CustodialWallet.Domain.Dto.Balance;
using System.Text.Json.Serialization;

namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserBalanceResponse : ResponseBase
    {
        [JsonPropertyOrder(2)]
        public Guid UserId { get; set; }
        [JsonPropertyOrder(3)]
        public List<BalanceShortDto> Balances { get; set; }
    }
}
