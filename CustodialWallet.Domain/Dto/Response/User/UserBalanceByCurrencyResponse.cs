namespace CustodialWallet.Domain.Dto.Response.User
{
    public class UserBalanceByCurrencyResponse : ResponseBase
    {
        public Guid UserId { get; set; }
        public decimal NewAmount { get; set; }
        public string ShortCurrencyName { get; set; }
    }
}
