using CustodialWallet.Infostructure.Interface;
using FluentValidation;

namespace CustodialWallet.Application.Validator.User
{
    public class UserGuidValidator : AbstractValidator<Guid>
    {
        public UserGuidValidator(IUserRepository userRepository)
        {
            RuleFor(guid => guid).NotNull().NotEmpty()
                .WithMessage("User Id must be not null or not default.");

            RuleFor(guid => guid).Must(guid => userRepository.CheckIfUserExistsAsync(guid).Result)
               .WithMessage("User doesn`t exist.");
        }
    }
}
