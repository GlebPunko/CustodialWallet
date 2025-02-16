using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Models;
using CustodialWallet.Domain.Dto.Balance;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.Response;
using CustodialWallet.Infostructure.Interface;

namespace CustodialWallet.Application.Service
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<UserWithBalancesResponse> CreateUserAsync(UserModel userModel)
        {
            var guid = await _userRepository.CreateUserAsync(userModel);

            return await GetUserWithBalancesByIdAsync(guid);
        }

        public async Task<UserBalanceResponse> GetUserBalanceAsync(Guid userId)
        {
            var userBalances = await _userRepository.GetUserBalancesAsync(userId);

            var response = new UserBalanceResponse
            {
                UserId = userId,
                Balances = userBalances
                   .Where(r => r.BalanceId != Guid.Empty)
                   .Select(r => new BalanceShortDto
                   {
                       Amount = r.Amount,
                       CurrencyShortName = r.CurrencyShortName,
                   })
                   .ToList()
            };

            return response;
        }

        public async Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId)
        {
            var userWithBalances = await _userRepository.GetUserWithBalancesByIdAsync(userId) ?? throw new Exception("Пользователь не найден."); // todo custom ex

            var response = new UserWithBalancesResponse
            {
                UserId = userId,
                Email = userWithBalances.FirstOrDefault()?.Email,
                Balances = userWithBalances
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

            return response;
        }

        public async Task<UserBalanceByCurrencyResponse> DepositAsync(Guid userId, DepositRequest depositRequest)
        {
            await _userRepository.DepositAsync(userId, depositRequest);

            var newAmount = await _userRepository.GetBalanceByUserIdAndCurrencyIdAsync(userId, depositRequest.CurrencyId);

            return new UserBalanceByCurrencyResponse { NewAmount = newAmount, UserId = userId, ShortCurrencyName = "BTC"}; // TODO TEST CURR
        }

        public async Task<UserBalanceByCurrencyResponse> WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest)
        {
            await _userRepository.WithdrawAsync(userId, withdrawRequest);

            var newAmount = await _userRepository.GetBalanceByUserIdAndCurrencyIdAsync(userId, withdrawRequest.CurrencyId);

            return new UserBalanceByCurrencyResponse { NewAmount = newAmount, UserId = userId, ShortCurrencyName = "BTC" }; // TODO TEST CURR
        }
    }
}
