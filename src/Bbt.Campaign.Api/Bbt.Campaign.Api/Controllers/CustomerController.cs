using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Services.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CustomerController : BaseController<CustomerController>
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CustomerController(ICustomerService customerService, IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Returns the form data for customer view min page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("view-customer-min")]
        public async Task<IActionResult> GetCustomerViewMinFormAsync(int campaignId)
        {
            var result = await _customerService.GetCustomerViewMinFormAsync(campaignId, _webHostEnvironment.ContentRootPath);
            return Ok(result);
        }
    }
}
