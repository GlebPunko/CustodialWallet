namespace CustodialWallet.Application.CustomException
{
    public class DepositIssueException(string error) : Exception(error)
    {
    }
}
