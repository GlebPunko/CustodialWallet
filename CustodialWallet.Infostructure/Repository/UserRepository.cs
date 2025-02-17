using Dapper;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Domain.Dto.User;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Models.User;

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

        public async Task<IEnumerable<UserWithBalancesDto>> GetUserBalancesAsync(Guid userId)
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

            return result;
        }

        public async Task<IEnumerable<UserWithBalancesDto>> GetUserWithBalancesByIdAsync(Guid userId)
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

            return result;
        }

        public async Task<bool> DepositAsync(Guid userId, DepositRequest depositRequest)
        {
            using var connection = _dapperContext.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Balances 
                    SET Amount = Amount + CAST(@Amount as MONEY)
                    WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                    new
                    {
                        UserId = userId,
                        CurrencyId = depositRequest.CurrencyId,
                        Amount = depositRequest.Amount
                    },
                    transaction
                );

                if(rowsAffected == 0)
                {
                    transaction.Rollback();

                    return false;
                }

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();

                throw;
            }
        }

        public async Task<bool> WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest)
        {
            using var connection = _dapperContext.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Balances 
                    SET Amount = Amount - @Amount 
                    WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                    new
                    {
                        UserId = userId,
                        CurrencyId = withdrawRequest.CurrencyId,
                        Amount = withdrawRequest.Amount
                    },
                    transaction
                );

                if(rowsAffected == 0)
                {
                    transaction.Rollback();

                    return false;
                }

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();

                throw;
            }
        }

        public async Task<UserBalanceInfoDto> GetBalanceByUserIdAndCurrencyIdAsync(Guid userId, Guid currencyId)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                    SELECT b.Amount, c.ShortName AS CurrencyShortName
                    FROM Balances b
                    JOIN Currencies c ON b.CurrencyId = c.Id
                    WHERE b.UserId = @UserId AND b.CurrencyId = @CurrencyId;";

            var balanceInfo = await connection.QueryFirstOrDefaultAsync<UserBalanceInfoDto>(sql, new
            {
                UserId = userId,
                CurrencyId = currencyId
            });

            return balanceInfo;
        }

        public async Task<bool> CheckIfEmailExistsAsync(string email)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                    SELECT EXISTS (
                    SELECT 1 
                    FROM Users 
                    WHERE Email = @Email
                );";

            var emailExists = await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Email = email });

            return emailExists;
        }

        public async Task<bool> CheckIfUserExistsAsync(Guid userId)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                    SELECT EXISTS (
                    SELECT 1 
                    FROM Users 
                    WHERE Id = @UserId
                );";

            var userlExists = await connection.QueryFirstOrDefaultAsync<bool>(sql, new { UserId = userId });

            return userlExists;
        }

        public async Task<bool> CheckIfCurrencyExistsAsync(Guid currencyId)
        {
            using var connection = _dapperContext.CreateConnection();

            var sql = @"
                    SELECT EXISTS (
                    SELECT 1 
                    FROM Currencies 
                    WHERE Id = @CurrencyId
                );";

            var userlExists = await connection.QueryFirstOrDefaultAsync<bool>(sql, new { CurrencyId = currencyId });

            return userlExists;
        }
    }
}
