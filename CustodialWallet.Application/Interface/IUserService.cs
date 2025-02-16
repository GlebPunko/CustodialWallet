using CustodialWallet.Application.Models;
using CustodialWallet.Domain.Dto.Response;

namespace CustodialWallet.Application.Interface
{
    public interface IUserService
    {
        Task<UserWithBalancesResponse> CreateUserAsync(UserModel userModel);
        Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId);
    }
}
