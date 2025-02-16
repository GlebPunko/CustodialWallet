namespace CustodialWallet.Domain.Dto.Response
{
    public class UserBalanceByCurrencyResponse
    {
        public Guid UserId {  get; set; }
        public decimal NewAmount { get; set; }
        public string ShortCurrencyName { get; set; }
    }
}
