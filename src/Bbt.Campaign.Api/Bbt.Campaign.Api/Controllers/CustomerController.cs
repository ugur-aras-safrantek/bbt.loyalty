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
        /// <param name="customerCode">Code of the customer</param>
        /// <param name="campaignId">Id of the campaign</param>
        /// <param name="isJoin">Is joined to campaign</param>
        /// <returns></returns>
        [HttpPost]
        [Route("join")]
        public async Task<IActionResult> SetJoin(string customerCode, int campaignId, bool isJoin)
        {
            var createResult = await _customerService.SetJoin(customerCode, campaignId, isJoin);
            return Ok(createResult);
        }
        /// <summary>
        /// Signs the campaign as favorite for the customer
        /// </summary>
        /// <param name="customerCode">Code of the customer</param>
        /// <param name="campaignId">Id of the campaign</param>
        /// <param name="isFavorite">Is favorite or not</param>
        /// <returns></returns>
        [HttpPost]
        [Route("favorite")]
        public async Task<IActionResult> SetFavorite(string customerCode, int campaignId, bool isFavorite)
        {
            var createResult = await _customerService.SetFavorite(customerCode, campaignId, isFavorite);
            return Ok(createResult);
        }
        /// <summary>
        /// Returns the form data for customer view min page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("view-customer-min")]
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
        [Route("get-join-form")]
        public async Task<IActionResult> GetCustomerJoinFormAsync(int campaignId, string customerCode)
        {
            var result = await _customerService.GetCustomerJoinFormAsync(campaignId, customerCode, _webHostEnvironment.ContentRootPath);
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign list by selected filter options
        /// </summary>
        /// <param name="request">Page types: Campaign = 1, Join = 2, Favorite = 3, OverDue = 4</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CustomerCampaignListFilterRequest request)
        {
            var result = await _customerService.GetByFilterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer view min page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <param name="customerCode">customer tckn or code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-customer-achievements")]
        public async Task<IActionResult> GetCustomerAchievementFormAsync(int campaignId, string customerCode)
        {
            var result = await _customerService.GetCustomerAchievementFormAsync(campaignId, customerCode);
            return Ok(result);
        }
    }
}
