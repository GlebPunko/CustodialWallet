using CustodialWallet.Domain.Dto.Response;
using FluentValidation.Results;

namespace CustodialWallet.Application.Interface
{
    public interface IResponseHelper
    {
        T PrepareErrorResponse<T>(List<ValidationFailure> errors) where T : ResponseBase, new();
    }
}
