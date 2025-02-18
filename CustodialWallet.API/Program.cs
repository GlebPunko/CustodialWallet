using CustodialWallet.API.Middleware;
using CustodialWallet.Application.DI;
using CustodialWallet.Infostructure.DI;
using CustodialWallet.Infostructure.Interface;
using CustodialWallet.Infostructure.Repository;

namespace CustodialWallet.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplication();
            builder.Services.AddData(builder.Configuration);

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSwagger", policy =>
                {
                    policy.WithOrigins("http://localhost:8080", "https://localhost:8081") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors("AllowSwagger");

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
