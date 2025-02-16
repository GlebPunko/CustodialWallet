using CustodialWallet.Domain.Dto.Response;
using System.Net;
using System.Text.Json;

namespace CustodialWallet.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception) // todo will prepare custom exceptions
            {
                case ArgumentNullException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error = "Custom ex";
                    break;

                case KeyNotFoundException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error = "Custom ex";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Error = "Custom ex";
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
