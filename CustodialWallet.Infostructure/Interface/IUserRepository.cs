﻿using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.User;
using CustodialWallet.Domain.Models.User;

namespace CustodialWallet.Infostructure.Interface
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(UserModel userModel);
        Task<IEnumerable<UserWithBalancesDto>> GetUserBalancesAsync(Guid userId);
        Task<IEnumerable<UserWithBalancesDto>> GetUserWithBalancesByIdAsync(Guid userId);
        Task DepositAsync(Guid userId, DepositRequest depositRequest);
        Task WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest);
        Task<UserBalanceInfoModel> GetBalanceByUserIdAndCurrencyIdAsync(Guid userId, Guid currencyId);
        Task<bool> CheckIfEmailExistsAsync(string email);
        Task<bool> CheckIfUserExistsAsync(Guid userId);
        Task<bool> CheckIfCurrencyExistsAsync(Guid currencyId);
    }
}
