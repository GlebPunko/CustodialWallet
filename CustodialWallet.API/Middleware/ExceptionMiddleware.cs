﻿using CustodialWallet.Application.CustomException;
using CustodialWallet.Domain.Dto.Response;
using CustodialWallet.Infostructure.Interface;
using System.Net;
using System.Text.Json;

namespace CustodialWallet.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogRepository logRepository)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogRepository _logRepository = logRepository;

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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case UserNotFoundException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error = ex.Message;
                    break;

                case DepositIssueException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error = ex.Message;
                    break;

                case WithdrawIssueException ex:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Error = ex.Message;
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Error = "Something wrong. Try again later.";

                    await _logRepository.LogErrorAsync(exception.Message, exception.Source, exception.StackTrace);

                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
