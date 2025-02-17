using CustodialWallet.Domain.Models.User;
using CustodialWallet.Infostructure.Interface;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CustodialWallet.Application.Validator.User
{
    public class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator(IUserRepository userRepository)
        {
            RuleFor(userModel => userModel).NotNull().NotEmpty()
                .WithMessage("User body can`t be empty.");

            RuleFor(userModel => userModel.Email).NotNull().NotEmpty().Must(email =>
            {
                var regex = new Regex("^((?!\\.)[\\w\\-_.]*[^.])(@\\w+)(\\.\\w+(\\.\\w+)?[^.\\W])$");

                return regex.IsMatch(email);
            })
                .WithMessage("Email is invalid!");

            RuleFor(userModel => userModel.Email).Must(email => !userRepository.CheckIfEmailExistsAsync(email).Result)
                .WithMessage("User with this email exists exist.");
        }
    }
}
