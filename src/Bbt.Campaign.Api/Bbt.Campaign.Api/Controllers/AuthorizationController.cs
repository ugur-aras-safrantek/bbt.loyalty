using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Services.Services.Approval;
using Bbt.Campaign.Services.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class AuthorizationController :  BaseController<AuthorizationController>
    {
        private readonly IAuthorizationservice _authorizationService;

        public AuthorizationController(IAuthorizationservice authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Login of the user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var createResult = await _authorizationService.LoginAsync(request);
            return Ok(createResult);
        }

        /// <summary>
        /// Updates roles of the user on development environment
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="userRoles">Roles of the user</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-user-roles-development")]
        public async Task<IActionResult> UpdateUserRolesDevelopmentAsync(string userId, string userRoles)
        {
            var createResult = await _authorizationService.UpdateUserRolesDevelopmentAsync(userId, userRoles);
            return Ok(createResult);
        }

    }
}
