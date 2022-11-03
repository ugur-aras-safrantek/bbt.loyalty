using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Public.Models.MessagingTemplate;
using Bbt.Campaign.Public.Models.Report;


namespace Bbt.Campaign.Services.Services.Remote
{
    public interface IRemoteService
    {
        //public Task<List<string>> GetChannelCodeList();

        public Task<GoalResultByCustomerIdAndMonthCount> GetGoalResultByCustomerIdAndMonthCountData(string customerCode, int campaignId);
        public Task<CustomerReportServiceDto> GetCustomerReportData(CustomerReportRequest request);
        public Task<List<EarningByCustomerAndCampaing>> GetEarningByCustomerAndCampaingData(string customerId, int campaignId, string lang);
        public Task<GoalResultByCustomerAndCampaing> GetGoalResultByCustomerAndCampaingData(string customerId, int campaignId, string lang, string term = null);
        public Task<TargetReportServiceDto> GetTargetReportData(TargetReportRequest request);
        public Task<Document> GetDocument(int id, string customerCode);
        public Task<UserModelService> GetUserRoles(string code, string state);
        public Task SendSmsMessageTemplate(string customerId, int campaignId, int messageTypeId, TemplateInfo templateData);
        public Task SendNotificationMessageTemplate(string customerId, int campaignId, int messageTypeId, TemplateInfo templateData);
        public Task<HttpResponseMessage> CustomerAchievementsAdd(string customerId, int campaignId, string term);
        public Task<HttpResponseMessage> CustomerAchievementsDelete(string customerId, int campaignId, string term);
        public Task<HttpResponseMessage> LeaveProgramAchievementDelete(string customerId, int campaignId, string term);
        public Task<HttpResponseMessage> SendDmsDocuments(string customerId, List<int> documentIds);
        public Task<bool> GetAccounts(string customerId);
        public Task<bool> CleanCache();

    }
}
