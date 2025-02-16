using CustodialWallet.Application.Interface;
using CustodialWallet.Domain.Dto.Response;
using FluentValidation.Results;

namespace CustodialWallet.Application.Helper
{
    public class ResponseHelper : IResponseHelper
    {
        public T PrepareErrorResponse<T>(List<ValidationFailure> errors) where T : ResponseBase, new() => new()
        {
            Messages = errors.Select(e => e.ErrorMessage).ToList(),
        };
    }
}
