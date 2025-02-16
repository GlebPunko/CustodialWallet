using Dapper;
using CustodialWallet.Application.Models;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Domain.Dto.Balance;
using CustodialWallet.Domain.Dto.User;
using CustodialWallet.Domain.Dto.Response;

namespace CustodialWallet.Infostructure.Repository
{
    public class UserRepository(DapperContext dapperContext) : IUserRepository
    {
        private readonly DapperContext _dapperContext = dapperContext;

        public async Task<Guid> CreateUserAsync(UserModel userModel)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                INSERT INTO Users (Email)
                VALUES (@Email)
                RETURNING Id;";

            var userId = await connection.QuerySingleAsync<Guid>(sql, new { Email = userModel.Email });

            return userId;
        }

        public async Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                SELECT 
                    Users.Id AS UserId, 
                    Users.Email, 
                    Balances.Id AS BalanceId, 
                    Balances.Amount, 
                    Currencies.ShortName AS CurrencyShortName, 
                    Currencies.FullName AS CurrencyFullName
                FROM Users
                LEFT JOIN Balances ON Users.Id = Balances.UserId
                LEFT JOIN Currencies ON Balances.CurrencyId = Currencies.Id
                WHERE Users.Id = @UserId;";

            var result = await connection.QueryAsync<UserWithBalancesDto>(sql, new { UserId = userId });

            var userWithBalances = new UserWithBalancesResponse
            {
                UserId = userId,
                Email = result.FirstOrDefault()?.Email,
                Balances = result
                    .Where(r => r.BalanceId != Guid.Empty)
                    .Select(r => new BalanceDto
                    {
                        BalanceId = r.BalanceId,
                        Amount = r.Amount,
                        CurrencyShortName = r.CurrencyShortName,
                        CurrencyFullName = r.CurrencyFullName
                    })
                    .ToList()
            };

            return userWithBalances;
        }
    }
}
