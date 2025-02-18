using System.Text.Json.Serialization;

namespace CustodialWallet.Domain.Dto.Response
{
    public abstract class ResponseBase
    {
        [JsonPropertyOrder(0)]
        public bool Success { get; set; } = true;
        [JsonPropertyOrder(1)]
        public List<string> Messages { get; set; } = ["Ok"];
    }
}
