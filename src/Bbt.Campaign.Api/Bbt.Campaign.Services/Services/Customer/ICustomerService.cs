using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Customer
{
    public interface ICustomerService
    {
        public Task<BaseResponse<CustomerCampaignDto>> SetJoin(SetJoinRequest request);
        public Task<BaseResponse<CustomerCampaignDto>> SetFavorite(SetFavoriteRequest request);
        public Task<BaseResponse<CustomerCampaignDto>> DeleteAsync(int id);
        public Task<BaseResponse<CustomerCampaignDto>> GetCustomerCampaignAsync(int id);
        public Task<BaseResponse<CustomerCampaignListFilterResponse>> GetByFilterAsync(CustomerCampaignListFilterRequest request);
        public Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode, string language);
        public Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewFormAsync(int campaignId, string contentRootPath);
        public Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerJoinFormAsync(int campaignId, string customerCode, string contentRootPath);
        public Task<BaseResponse<CustomerJoinSuccessFormDto>> GetCustomerJoinSuccessFormAsync(int campaignId, string customerCode);
    }
}
