using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Infostructure.Interface;
using FluentValidation;

namespace CustodialWallet.Application.Validator.User
{
    public class WithdrawValidator : AbstractValidator<(Guid, WithdrawRequest)>
    {
        public WithdrawValidator(IUserRepository userRepository)
        {
            RuleFor(request => request.Item1).NotNull().NotEmpty()
                .WithMessage("User Id must be not null or not default.");

            RuleFor(request => request.Item2.CurrencyId).NotNull().NotEmpty()
                .WithMessage("Currency Id must be not null or not default.");

            RuleFor(request => request.Item2.Amount).Must(amount => amount > 0)
                .WithMessage("Amount must be more than zero.");

            RuleFor(request => request).Must(request => userRepository.GetBalanceByUserIdAndCurrencyIdAsync(request.Item1, request.Item2.CurrencyId)?.Result?.Amount >= request.Item2.Amount)
                .WithMessage("Balance is less that amount that you want withdrow.");

            RuleFor(userModel => userModel.Item1).Must(userId => userRepository.CheckIfUserExistsAsync(userId).Result)
               .WithMessage("User doesn`t exist.");

            RuleFor(userModel => userModel.Item2.CurrencyId).Must(currencyId => userRepository.CheckIfCurrencyExistsAsync(currencyId).Result)
               .WithMessage("Currency doesn`t exist.");
        }
    }
}
