using CustodialWallet.Domain.Dto.Balance;
using System.Text.Json.Serialization;

namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserWithBalancesResponse : ResponseBase
    {
        [JsonPropertyOrder(2)]
        public Guid UserId { get; set; }
        [JsonPropertyOrder(3)]
        public string Email { get; set; }
        [JsonPropertyOrder(4)]
        public List<BalanceDto> Balances { get; set; } = [];
    }
}
