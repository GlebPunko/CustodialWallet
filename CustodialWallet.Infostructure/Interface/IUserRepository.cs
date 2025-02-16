using CustodialWallet.Application.Models;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.User;

namespace CustodialWallet.Infostructure.Interface
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(UserModel userModel);
        Task<IEnumerable<UserWithBalancesDto>> GetUserBalancesAsync(Guid userId);
        Task<IEnumerable<UserWithBalancesDto>> GetUserWithBalancesByIdAsync(Guid userId);
        Task DepositAsync(Guid userId, DepositRequest depositRequest);
        Task WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest);
        Task<decimal> GetBalanceByUserIdAndCurrencyIdAsync(Guid userId, Guid currencyId);
    }
}
