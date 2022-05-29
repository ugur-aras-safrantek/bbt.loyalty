using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Authorization;
using Bbt.Campaign.Services.Services.Approval;
using Bbt.Campaign.Services.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class AuthorizationController :  BaseController<AuthorizationController>
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationController(IAuthorizationService authorizationService)
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
        /// Checks if a user has authorization for the spesific module
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("check-authorization")]
        public async Task<IActionResult> CheckAuthorizationAsync(CheckAuthorizationRequest request)
        {
            var createResult = await _authorizationService.CheckAuthorizationAsync(request);
            return Ok(createResult);
        }

        /// <summary>
        /// Updates roles of the user 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="userRoles">Roles of the user</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-user-roles")]
        public async Task<IActionResult> UpdateUserRolesAsync(string userId, string userRoles)
        {
            var createResult = await _authorizationService.UpdateUserRolesAsync(userId, userRoles);
            return Ok(createResult);
        }

    }
}
