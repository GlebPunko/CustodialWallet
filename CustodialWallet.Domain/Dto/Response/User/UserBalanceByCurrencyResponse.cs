using System.Text.Json.Serialization;

namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserBalanceByCurrencyResponse : ResponseBase
    {
        [JsonPropertyOrder(2)]
        public Guid UserId { get; set; }
        [JsonPropertyOrder(3)]
        public decimal NewAmount { get; set; }
        [JsonPropertyOrder(4)]
        public string ShortCurrencyName { get; set; }
    }
}
