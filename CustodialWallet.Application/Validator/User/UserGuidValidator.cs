using FluentValidation;

namespace CustodialWallet.Application.Validator.User
{
    public class UserGuidValidator : AbstractValidator<Guid>
    {
        public UserGuidValidator()
        {
            RuleFor(guid => guid).NotNull().NotEmpty().Must(guid => guid != Guid.Empty)
                .WithMessage("User Id must be not null or not default.");
        }
    }
}
