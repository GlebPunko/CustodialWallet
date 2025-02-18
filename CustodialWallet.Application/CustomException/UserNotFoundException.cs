namespace CustodialWallet.Application.CustomException
{
    public class UserNotFoundException(string error) : Exception(error)
    {
    }
}
