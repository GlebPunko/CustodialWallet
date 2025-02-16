using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Infostructure.Repository;
using FluentValidation;

namespace CustodialWallet.Application.Validator.User
{
    public class DepositValidator : AbstractValidator<(Guid, DepositRequest)>
    {
        public DepositValidator(IUserRepository userRepository)
        {
            RuleFor(request => request.Item1).NotNull().NotEmpty().Must(guid => Guid.Empty != guid)
                .WithMessage("User Id must be not null or not default.");

            RuleFor(request => request.Item2.CurrencyId).Must(currencyId => Guid.Empty != currencyId)
                .WithMessage("Currency Id must be not null or not default.");

            RuleFor(request => request.Item2.Amount).NotNull().NotEmpty().Must(amount => amount > 0)
                .WithMessage("Amount must be more than zero.");

            RuleFor(userModel => userModel.Item1).Must(userId => userRepository.CheckIfUserExistsAsync(userId).Result)
               .WithMessage("User doesn`t exist.");

            RuleFor(userModel => userModel.Item2.CurrencyId).Must(currencyId => userRepository.CheckIfCurrencyExistsAsync(currencyId).Result)
               .WithMessage("Currency doesn`t exist.");
        }
    }
}
