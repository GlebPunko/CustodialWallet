namespace CustodialWallet.Application.CustomException
{
    public class WithdrawIssueException(string error) : Exception(error)
    {
    }
}
