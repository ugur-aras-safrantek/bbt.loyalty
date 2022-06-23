using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.Report;


namespace Bbt.Campaign.Services.Services.Remote
{
    public interface IRemoteService
    {
        public Task<List<string>> GetChannelCodeList();

        public Task<string> GetCustomerReportData(CustomerReportRequest request);
        public Task<string> GetEarningByCustomerAndCampaingData(string customerId, int campaignId, string lang);
        public Task<GoalResultByCustomerAndCampaing> GetGoalResultByCustomerAndCampaingData(string customerId, int campaignId, string lang);
    }
}
