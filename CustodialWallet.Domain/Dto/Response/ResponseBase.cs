namespace CustodialWallet.Domain.Dto.Response
{
    public abstract class ResponseBase
    {
        public bool Success { get; set; } = false;
        public List<string> Messages { get; set; } = [];
    }
}
