﻿using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.Response.User;
using CustodialWallet.Domain.Models.User;

namespace CustodialWallet.Application.Interface
{
    public interface IUserService
    {
        Task<UserWithBalancesResponse> CreateUserAsync(UserModel userModel);
        Task<UserBalanceResponse> GetUserBalanceAsync(Guid userId);
        Task<UserWithBalancesResponse> GetUserWithBalancesByIdAsync(Guid userId);
        Task<UserBalanceByCurrencyResponse> DepositAsync(Guid userId, DepositRequest depositRequest);
        Task<UserBalanceByCurrencyResponse> WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest);
    }
}
