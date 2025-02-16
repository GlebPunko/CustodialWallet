using CustodialWallet.Application.Interface;
using CustodialWallet.Domain.Dto.Request;
using CustodialWallet.Domain.Dto.Response.User;
using CustodialWallet.Domain.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace CustodialWallet.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("{userId}/balances")]
        public async Task<ActionResult<UserBalanceResponse>> GetUserBalanceAsync([FromRoute] Guid userId)
        {
            var response = await _userService.GetUserBalanceAsync(userId);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserWithBalancesResponse>> CreateUserAsync(UserModel userModel)
        {
            var response = await _userService.CreateUserAsync(userModel);

            return Ok(response);
        }

        [HttpPost("{userId}/deposit")]
        public async Task<ActionResult<UserBalanceByCurrencyResponse>> DepositAsync([FromRoute] Guid userId, [FromBody] DepositRequest depositRequest)
        {
            var response = await _userService.DepositAsync(userId, depositRequest);

            return Ok(response);
        }

        [HttpPost("{userId}/withdraw")]
        public async Task<ActionResult<UserBalanceByCurrencyResponse>> WithdrawAsync([FromRoute] Guid userId, WithdrawRequest withdrawRequest)
        {
            var response = await _userService.WithdrawAsync(userId, withdrawRequest);

            return Ok(response);
        }
    }
}
