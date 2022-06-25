using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Public.Models.Parameter;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.Remote
{
    public class RemoteService : IRemoteService, IScopedService
    {
        private readonly IParameterService _parameterService;

        public RemoteService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public async Task<GoalResultByCustomerIdAndMonthCount> GetGoalResultByCustomerIdAndMonthCountData(string customerCode) 
        {
            var goalResultByCustomerIdAndMonthCount = new GoalResultByCustomerIdAndMonthCount();
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessToken();
                string serviceUrl = await _parameterService.GetServiceConstantValue("GoalResultByCustomerIdAndMonthCount");
                serviceUrl = serviceUrl.Replace("{customerId}", customerCode).Replace("{monthCount}", "2");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content != null)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(apiResponse))
                            goalResultByCustomerIdAndMonthCount = JsonConvert.DeserializeObject<GoalResultByCustomerIdAndMonthCount>(apiResponse);
                    }
                }
                else
                {
                    throw new Exception("Müşteri kazanımları servisinden veri çekilemedi.");
                }
            }
            return goalResultByCustomerIdAndMonthCount;
        }
        public async Task<GoalResultByCustomerAndCampaing> GetGoalResultByCustomerAndCampaingData(string customerId, int campaignId, string lang) 
        {
            GoalResultByCustomerAndCampaing goalResultByCustomerAndCampaing = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessToken();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("GoalResultByCustomerAndCampaing");
                string serviceUrl = string.Concat(baseAddress, apiAddress);

                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{lang}", lang);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(apiResponse))
                        goalResultByCustomerAndCampaing = JsonConvert.DeserializeObject<GoalResultByCustomerAndCampaing>(apiResponse);
                }
                else
                    throw new Exception("Müşteri hedef servisinden veri çekilemedi.");
            }
            return goalResultByCustomerAndCampaing;
        }
        public async Task<List<EarningByCustomerAndCampaing>> GetEarningByCustomerAndCampaingData(string customerId, int campaignId, string lang) 
        {
            var earningByCustomerAndCampaingList = new List<EarningByCustomerAndCampaing>();
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessToken();
                string serviceUrl = await _parameterService.GetServiceConstantValue("EarningByCustomerAndCampaing");
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{lang}", lang);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    earningByCustomerAndCampaingList = JsonConvert.DeserializeObject<List<EarningByCustomerAndCampaing>>(apiResponse);
                }
                else 
                    throw new Exception("Müşteri kazanım servisinden hata alındı.");
            }
            return earningByCustomerAndCampaingList;
        }
        public async Task<CustomerReportServiceDto> GetCustomerReportData(CustomerReportRequest request) 
        {
            CustomerReportServiceDto customerReportServiceDto = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessToken();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("CampaignReport");
                string serviceUrl = string.Concat(baseAddress, apiAddress);

                int pageNumber = (request.PageNumber - 1) ?? 0;
                serviceUrl += "?PageNumber=" + pageNumber;
                int pageSize = (request.PageSize) ?? 25;
                serviceUrl += "&PageSize=" + pageSize;
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    if (request.SortBy.EndsWith("Str"))
                        request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

                    string sortBy = request.SortBy;
                    switch (request.SortBy)
                    {
                        case "Code":
                            sortBy = "CampaignCode";
                            break;
                        case "Name":
                            sortBy = "CampaignName";
                            break;
                        case "IsActive":
                            sortBy = "IsActive";
                            break;
                        case "IsBundle":
                            sortBy = "IsBundle";
                            break;
                        case "JoinDate":
                            sortBy = "CustomerJoinDate";
                            break;
                        case "CustomerCode":
                            sortBy = "CustomerNumber";
                            break;
                        case "CustomerIdentifier":
                            sortBy = "CustomerId";
                            break;
                        case "EarningReachDate":
                            sortBy = "EarningReachDate";
                            break;
                        case "AchievementAmount":
                            sortBy = "EarningAmount";
                            break;
                        case "AchievementRate":
                            sortBy = "EarningRate";
                            break;
                        case "CustomerTypeName":
                            sortBy = "CustomerType";
                            break;
                        case "BranchCode":
                            sortBy = "BranchCode";
                            break;
                        case "BusinessLineName":
                            sortBy = "BusinessLine";
                            break;
                        case "AchievementTypeName":
                            sortBy = "EarningType";
                            break;
                        case "AchievementDate":
                            sortBy = "EarningUsedDate";
                            break;
                    }

                    serviceUrl += "&SortBy=" + sortBy;

                    bool isDescending = request.SortDir?.ToLower() == "desc";

                    serviceUrl += "&SortType=" + (isDescending ? (int)SortTypeEnum.Descending : (int)SortTypeEnum.Ascending);
                }
                else
                {
                    serviceUrl += "&SortBy=CustomerJoinDate";
                    serviceUrl += "&SortType=" + (int)SortTypeEnum.Descending;
                }


                if (!string.IsNullOrEmpty(request.CustomerCode))
                    serviceUrl += "&CustomerNumber=" + request.CustomerCode;
                if (!string.IsNullOrEmpty(request.CustomerIdentifier))
                    serviceUrl += "&CustomerId=" + request.CustomerIdentifier;
                if (request.CustomerTypeId.HasValue)
                {
                    int customerTypeId = request.CustomerTypeId ?? 0;
                    serviceUrl += "&CustomerType=" + Helpers.GetEnumDescription<CustomerTypeEnum>((customerTypeId));
                }

                if (!string.IsNullOrEmpty(request.BranchCode))
                    serviceUrl += "&BranchCode=" + request.BranchCode;
                if (request.AchievementTypeId.HasValue)
                {
                    int achievementTypeId = request.AchievementTypeId ?? 0;
                    serviceUrl += "&EarningType=" + Helpers.GetEnumDescription<AchievementTypeEnum>((achievementTypeId));
                }

                if (request.BusinessLineId.HasValue)
                {
                    int businessLineId = request.BusinessLineId ?? 0;
                    string businessLine = Helpers.GetEnumDescription<BusinessLineEnum>((businessLineId));
                    string businessLineShort = businessLine.Substring(businessLine.Length - 2, 1);
                    serviceUrl += "&BusinessLine=" + businessLineShort;
                }
                if (request.IsActive.HasValue)
                    serviceUrl += "&IsActive=" + request.IsActive;

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    customerReportServiceDto = JsonConvert.DeserializeObject<CustomerReportServiceDto>(apiResponse);
                }
                else
                {
                    throw new Exception("Rapor servisinden veri çekilemedi.");
                }
            }
            return customerReportServiceDto;
        }
        public async Task<UserModelService> GetUserRoles(string code, string state)
        {
            UserModelService userModel;
            string accessToken = string.Empty;

            if (state == "LoyaltyGondor")
            {
                using (var client = new HttpClient())
                {
                    string baseAddress = await _parameterService.GetServiceConstantValue("AccessTokenBaseAddress");
                    string apiAddress = await _parameterService.GetServiceConstantValue("AccessTokenApiAddress");
                    client.BaseAddress = new Uri(baseAddress);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("client_id", await _parameterService.GetServiceConstantValue("client_id")),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("client_secret", await _parameterService.GetServiceConstantValue("client_secret")),
                        new KeyValuePair<string, string>("redirect_uri", await _parameterService.GetServiceConstantValue("redirect_uri")),
                    });

                    var result = await client.PostAsync(apiAddress, content);
                    var responseContent = result.Content.ReadAsStringAsync().Result;
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                    if (token.Access_token == null)
                        throw new Exception(responseContent);
                    accessToken = token.Access_token;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(await _parameterService.GetServiceConstantValue("BaseAddress"));
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("access_token", accessToken),
                    });
                    var result = await client.PostAsync(await _parameterService.GetServiceConstantValue("ResourceApiAddress"), content);

                    userModel = JsonConvert.DeserializeObject<UserModelService>(result.Content.ReadAsStringAsync().Result);

                    if (userModel == null)
                        throw new Exception("Kullanıcı rolleri bulunamadı.");

                    if (userModel.Credentials == null || !userModel.Credentials.Any())
                        throw new Exception("Kullanıcı rolleri bulunamadı.");

                    if (userModel.Tckn == null)
                        throw new Exception("Tckn bilgisi bulunamadı.");
                }
            }
            else { throw new Exception("Invalid state."); }
            return userModel;
        }
        public async Task<Document> GetDocument(int id) 
        {
            var document = new Document();
            string accessToken = await GetAccessToken();

            using (var client = new HttpClient())
            {
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("Document");
                apiAddress = apiAddress.Replace("{key}", id.ToString());
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    document = JsonConvert.DeserializeObject<Document>(apiResponse);
                }
                else if (response.StatusCode.ToString() == "455")
                {
                    throw new Exception("Document template not found.");
                }
                else
                {
                    throw new Exception("Sözleşme servisine bağlanılamadı.");
                }
            }
            return document;
        }
        private async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                string accessToken = string.Empty;
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("AccessToken");
                client.BaseAddress = new Uri(baseAddress);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", await _parameterService.GetServiceConstantValue("client_id")),
                    new KeyValuePair<string, string>("grant_type", await _parameterService.GetServiceConstantValue("grant_type")),
                    new KeyValuePair<string, string>("client_secret", await _parameterService.GetServiceConstantValue("client_secret"))
                });
                var result = await client.PostAsync(apiAddress, content);
                var responseContent = result.Content.ReadAsStringAsync().Result;
                AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                accessToken = token.Access_token;
                return accessToken;
            }
        }
        public enum SortTypeEnum
        {
            [Description("ASC")]
            Ascending = 0,
            [Description("DESC")]
            Descending = 1,
        }
    }
}
