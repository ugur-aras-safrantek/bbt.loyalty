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
        private readonly IConfiguration _configuration;

        public AuthorizationController(IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _authorizationService = authorizationService;
            _configuration = configuration;
        }

        

        /// <summary>
        /// Login of the user
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(string code, string state)
        {
            var createResult = await _authorizationService.LoginAsync(code, state);
            return Ok(createResult);
        }

        /// <summary>
        /// Checks if a user has authorization for the spesific module
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("check-authorization")]
        //public async Task<IActionResult> CheckAuthorizationAsync(CheckAuthorizationRequest request)
        //{
        //    if (User.Claims.Count() > 0) 
        //    {
        //        var x = User.Claims.Where(x => x.Value == "cc");

        //    }



        //   var createResult = await _authorizationService.CheckAuthorizationAsync(request);

        //    return Ok(createResult);
        //}

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
