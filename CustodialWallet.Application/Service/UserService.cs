using CustodialWallet.Application.CustomException;
using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Validator.User;
using CustodialWallet.Domain.Dto.Balance;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.Response.User;
using CustodialWallet.Domain.Models.User;
using CustodialWallet.Infostructure.Interface;
using FluentValidation;

namespace CustodialWallet.Application.Service
{
    public class UserService(IUserRepository userRepository, IResponseHelper responseHelper, 
        UserGuidValidator userGuidValidator, DepositValidator depositValidator, WithdrawValidator withdrawValidator,
        UserModelValidator userModelValidator) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IResponseHelper _responseHelper = responseHelper;

        private readonly UserGuidValidator _userGuidValidator = userGuidValidator;
        private readonly DepositValidator _depositValidator = depositValidator;
        private readonly WithdrawValidator _withdrawValidator = withdrawValidator;
        private readonly UserModelValidator _userModelValidator = userModelValidator;

        public async Task<UserWithBalancesResponse> CreateUserAsync(UserModel userModel)
        {
            var validResult = await _userModelValidator.ValidateAsync(userModel);

            if (!validResult.IsValid)
                return _responseHelper.PrepareErrorResponse<UserWithBalancesResponse>(validResult.Errors);

            var guid = await _userRepository.CreateUserAsync(userModel);

            return await GetUserWithBalancesByIdAsync(guid);
        }

        public async Task<UserBalanceResponse> GetUserBalanceAsync(Guid userId)
        {
            var validResult = await _userGuidValidator.ValidateAsync(userId);

            if (!validResult.IsValid)
                return _responseHelper.PrepareErrorResponse<UserBalanceResponse>(validResult.Errors);

            var userBalances = await _userRepository.GetUserBalancesAsync(userId) ?? throw new UserNotFoundException("User not found.");

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
            var validResult = await _userGuidValidator.ValidateAsync(userId);

            if (!validResult.IsValid)
                return _responseHelper.PrepareErrorResponse<UserWithBalancesResponse>(validResult.Errors);

            var userWithBalances = await _userRepository.GetUserWithBalancesByIdAsync(userId) ?? throw new UserNotFoundException("User not found.");

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
            var validResult = await _depositValidator.ValidateAsync((userId, depositRequest));

            if (!validResult.IsValid)
                return _responseHelper.PrepareErrorResponse<UserBalanceByCurrencyResponse>(validResult.Errors);

            var isSuccess = await _userRepository.DepositAsync(userId, depositRequest);

            if (!isSuccess)
                throw new DepositIssueException("Deposit issue. Try again later.");

            var newAmount = await _userRepository.GetBalanceByUserIdAndCurrencyIdAsync(userId, depositRequest.CurrencyId);

            return new UserBalanceByCurrencyResponse { NewAmount = newAmount.Amount, UserId = userId, ShortCurrencyName = newAmount.CurrencyShortName };
        }

        public async Task<UserBalanceByCurrencyResponse> WithdrawAsync(Guid userId, WithdrawRequest withdrawRequest)
        {
            var validResult = await _withdrawValidator.ValidateAsync((userId, withdrawRequest));

            if (!validResult.IsValid)
                return _responseHelper.PrepareErrorResponse<UserBalanceByCurrencyResponse>(validResult.Errors);

            var isSuccess = await _userRepository.WithdrawAsync(userId, withdrawRequest);

            if (!isSuccess)
                throw new WithdrawIssueException("Withdraw issue. Try again later.");

            var newAmount = await _userRepository.GetBalanceByUserIdAndCurrencyIdAsync(userId, withdrawRequest.CurrencyId);

            return new UserBalanceByCurrencyResponse { NewAmount = newAmount.Amount, UserId = userId, ShortCurrencyName = newAmount.CurrencyShortName };
        }
    }
}
