using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Customer;
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
        /// Adds new customer to a campaign
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("join/{customerId}")]
        public async Task<IActionResult> SetJoin(SetJoinRequest request)
        {
            var createResult = await _customerService.SetJoin(request);
            return Ok(createResult);
        }
        /// <summary>
        /// Signs the campaign as favorite for the customer
        /// </summary>
        /// <param name="request">Code of the customer</param>
        /// <returns></returns>
        [HttpPost]
        [Route("favorite/{customerId}")]
        public async Task<IActionResult> SetFavorite(SetFavoriteRequest request)
        {
            var createResult = await _customerService.SetFavorite(request);
            return Ok(createResult);
        }
        /// <summary>
        /// Returns the form data for customer view min page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("view-customer-min/{customerId}")]
        public async Task<IActionResult> GetCustomerViewFormAsync(int campaignId)
        {
            var result = await _customerService.GetCustomerViewFormAsync(campaignId, _webHostEnvironment.ContentRootPath);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer join page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <param name="customerCode">Code of the customer</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-join-form/{customerId}")]
        public async Task<IActionResult> GetCustomerJoinFormAsync(int campaignId, string customerCode)
        {
            var result = await _customerService.GetCustomerJoinFormAsync(campaignId, customerCode, _webHostEnvironment.ContentRootPath);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer join success page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <param name="customerCode">Code of the customer</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-join-success-form/{customerId}")]
        public async Task<IActionResult> GetCustomerJoinSuccessFormAsync(int campaignId, string customerCode)
        {
            var result = await _customerService.GetCustomerJoinSuccessFormAsync(campaignId, customerCode);
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign list by selected filter options
        /// </summary>
        /// <param name="request">Page types: Campaign = 1, Join = 2, Favorite = 3, OverDue = 4</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter/{customerId}")]
        public async Task<IActionResult> GetByFilter(CustomerCampaignListFilterRequest request)
        {
            var result = await _customerService.GetByFilterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer detail page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <param name="customerCode">customer tckn or code</param>
        /// <param name="language">language (send tr for turkish)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-customer-achievements/{customerId}")]
        public async Task<IActionResult> GetCustomerAchievementFormAsync2(int campaignId, string customerCode, string? language)
        {
            
            var result = await _customerService.GetCustomerAchievementFormAsync(campaignId, customerCode, language);
            return Ok(result);
        }
    }
}
