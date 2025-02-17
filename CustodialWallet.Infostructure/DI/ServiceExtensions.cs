using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Infostructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustodialWallet.Infostructure.DI
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DapperContext>();

            services.AddScoped<IInitRepository, InitRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
