namespace CustodialWallet.Infostructure.Interface
{
    public interface ILogRepository
    {
        Task LogErrorAsync(string message, string source, string stackTrace);
    }
}
