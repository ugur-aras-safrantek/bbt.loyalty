using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Services.Services.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CustomerController : BaseController<CustomerController>
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Returns the form data for customer view page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("view-campaign-min")]
        //public async Task<IActionResult> GetCampaignViewFormMinAsync(int campaignId)
        //{
        //    var result = await _customerService.GetCampaignViewFormMinAsync(campaignId);
        //    return Ok(result);
        //}
    }
}
