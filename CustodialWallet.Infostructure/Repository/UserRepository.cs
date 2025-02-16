using Dapper;
using CustodialWallet.Application.Models;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Domain.Dto.User;
using CustodialWallet.Domain.Dto.Request;

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

        public async Task DepositAsync(Guid userId, DepositRequest depositRequest)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var balance = await connection.QueryFirstOrDefaultAsync<decimal>(
                            @"SELECT Amount FROM Balances 
                                WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                            new { UserId = userId, CurrencyId = depositRequest.CurrencyId },
                            transaction
                        );

                        await connection.ExecuteAsync(
                            @"UPDATE Balances 
                            SET Amount = Amount + CAST(@Amount as MONEY)
                            WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                            new { UserId = userId, CurrencyId = depositRequest.CurrencyId, Amount = depositRequest.Amount },
                            transaction
                        );

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();

                        throw; 
                    }
                }
            }
        }

        public async Task WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var balance = await connection.QueryFirstOrDefaultAsync<decimal>(
                            @"SELECT Amount FROM Balances 
                            WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                            new { UserId = userId, CurrencyId = withdrawRequest.CurrencyId},
                            transaction
                        );

                        if (balance < withdrawRequest.Amount)
                        {
                            throw new Exception("Недостаточно средств для снятия.");
                        }

                        await connection.ExecuteAsync(
                            @"UPDATE Balances 
                            SET Amount = Amount - @Amount 
                            WHERE UserId = @UserId AND CurrencyId = @CurrencyId;",
                            new { 
                                UserId = userId, 
                                CurrencyId = withdrawRequest.CurrencyId, 
                                Amount = withdrawRequest.Amount },
                            transaction
                        );

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

        public async Task<decimal> GetBalanceByUserIdAndCurrencyIdAsync(Guid userId, Guid currencyId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                var sql = @"
                SELECT Amount 
                FROM Balances 
                WHERE UserId = @UserId AND CurrencyId = @CurrencyId;";

                var amount = await connection.QueryFirstOrDefaultAsync<decimal>(sql, new
                {
                    UserId = userId,
                    CurrencyId = currencyId
                });

                return amount;
            }
        }
    }
}
