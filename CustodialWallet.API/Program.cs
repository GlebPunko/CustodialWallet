using CustodialWallet.Application.Interface;
using CustodialWallet.Application.Service;
using CustodialWallet.Infostructure.DbContext;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Infostructure.Repository;

namespace CustodialWallet.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<DapperContext>();

            builder.Services.AddSingleton<IInitRepository, InitRepository>();

            builder.Services.AddScoped<IUserService, UserService>();

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

            app.Run();
        }
    }
}
