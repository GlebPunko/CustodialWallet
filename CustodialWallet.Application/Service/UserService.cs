using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Models;
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

        public async Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId)
        {
            var userWithBalances = await _userRepository.GetUserWithBalancesByIdAsync(userId) ?? throw new Exception("Пользователь не найден."); // todo custom ex

            return userWithBalances;
        }
    }
}
