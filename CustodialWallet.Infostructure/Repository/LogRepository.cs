using CustodialWallet.Domain.Models.Log;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using Dapper;

namespace CustodialWallet.Infostructure.Repository
{
    public class LogRepository(DapperContext dapperContext) : ILogRepository
    {
        private readonly DapperContext _dapperContext = dapperContext;

        public async Task LogErrorAsync(string message, string source, string stackTrace)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                INSERT INTO ErrorLogs (Message, Source, StackTrace)
                VALUES (@Message, @Source, @StackTrace)";

            await connection.ExecuteAsync(sql, new
            {
                Message = message,
                Source = source,
                StackTrace = stackTrace,
            });
        }
    }
}
