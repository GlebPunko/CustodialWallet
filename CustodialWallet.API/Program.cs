using CustodialWallet.API.Middleware;
using CustodialWallet.Application.DI;
using CustodialWallet.Application.Helper;
using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Service;
using CustodialWallet.Application.Validator.User;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Models.User;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Infostructure.Repository;
using FluentValidation;

namespace CustodialWallet.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<DapperContext>();

            builder.Services.AddScoped<IInitRepository, InitRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddApplication();

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var initRepository = scope.ServiceProvider.GetRequiredService<IInitRepository>();
                await initRepository.InitDatabaseAsync();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.Run();
        }
    }
}
