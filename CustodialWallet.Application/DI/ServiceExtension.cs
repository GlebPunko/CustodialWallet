using CustodialWallet.Application.Helper;
using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Service;
using CustodialWallet.Application.Validator.User;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace CustodialWallet.Application.DI
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<DepositValidator>();

            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IResponseHelper, ResponseHelper>();

            return services;
        }
    }
}
