using CustodialWallet.Application.Models;
using CustodialWallet.Domain.Dto.Response;

namespace CustodialWallet.Infostructure.Interface
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(UserModel userModel);
        Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId);
    }
}
