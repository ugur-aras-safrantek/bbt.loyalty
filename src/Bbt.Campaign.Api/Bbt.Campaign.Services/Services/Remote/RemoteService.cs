using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
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
        public async Task<List<string>> GetChannelCodeList() 
        {
            List<string> channelCodeList = new List<string>();
            using (var httpClient = new HttpClient())
            {
                string baseAddress = await GetServiceConstantValue("BaseAddress");
                string apiAddress = await GetServiceConstantValue("Document");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content != null)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        channelCodeList = JsonConvert.DeserializeObject<List<string>>((JObject.Parse(apiResponse)["data"]).ToString());
                        if (channelCodeList != null && channelCodeList.Any()) 
                        { 
                            return channelCodeList;
                        }
                        else 
                        { 
                            throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                        }
                    }
                    else 
                    { 
                        throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                    }
                }
                else 
                { 
                    throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); 
                }
            }
        }
        private async Task<string> GetAccessToken() 
        {
            string retVal = string.Empty;
            var serviceConstantList = (await _parameterService.GetServiceConstantListAsync())?.Data;
            string baseAddress = serviceConstantList?.Where(x => x.Code == "BaseAddress").FirstOrDefault().Name;
            string tokenAddress = serviceConstantList?.Where(x => x.Code == "Token").FirstOrDefault().Name;

            string client_id_value = serviceConstantList?.Where(x => x.Code == "client_id").FirstOrDefault().Name;
            string grant_type_value = serviceConstantList?.Where(x => x.Code == "grant_type").FirstOrDefault().Name;
            string client_secret_value = serviceConstantList?.Where(x => x.Code == "client_secret").FirstOrDefault().Name;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress); 
                var content = new FormUrlEncodedContent(new[]
                {

                        new KeyValuePair<string, string>("client_id", client_id_value),
                        new KeyValuePair<string, string>("grant_type", grant_type_value),
                        new KeyValuePair<string, string>("client_secret", client_secret_value)
                });
                var result = await client.PostAsync(tokenAddress, content);
                var responseContent = result.Content.ReadAsStringAsync().Result;
                //AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                //accessToken = token.access_token;
                return "";
            }

        }
        private async Task<string> GetDocument(int id) 
        {
            string accessToken = await GetAccessToken();
            string baseAddress = await GetServiceConstantValue("BaseAddress");
            string apiAddress = await GetServiceConstantValue("Document");
            apiAddress = apiAddress.Replace("{key}", id.ToString());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress); 

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var result = await client.GetAsync(apiAddress);
                var responseContent = result.Content.ReadAsStringAsync().Result;
            }
            return string.Empty;
        }
        private async Task<string> GetServiceConstantValue(string code) 
        {
            string retVal = string.Empty;
            var serviceConstantList = (await _parameterService.GetServiceConstantListAsync())?.Data;
            var serviceConstant = serviceConstantList?.Where(x => x.Code == code).FirstOrDefault();
            if (serviceConstant != null)
                retVal = serviceConstant.Name;
            return retVal;
        }

        public async Task<GoalResultByCustomerAndCampaing> GetGoalResultByCustomerAndCampaingData(string customerId, int campaignId, string lang) 
        {
            GoalResultByCustomerAndCampaing goalResultByCustomerAndCampaing = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await _parameterService.GetAccessToken();
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
                    if (restResponse.Content != null) 
                    {
                        var apiResponse = await restResponse.Content.ReadAsStringAsync();
                        if(!string.IsNullOrEmpty(apiResponse))
                            goalResultByCustomerAndCampaing = JsonConvert.DeserializeObject<GoalResultByCustomerAndCampaing>(apiResponse);
                    }
                        
                    else
                        throw new Exception("Müşteri hedef servisinden veri çekilemedi.");
                }
                else
                    throw new Exception("Müşteri hedef servisinden veri çekilemedi.");
            }
            return goalResultByCustomerAndCampaing;
        }
        public async Task<string> GetEarningByCustomerAndCampaingData(string customerId, int campaignId, string lang) 
        {
            string apiResponse = string.Empty;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await _parameterService.GetAccessToken();
                string serviceUrl = await _parameterService.GetServiceConstantValue("EarningByCustomerAndCampaing");
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{lang}", lang);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.IsSuccessStatusCode)
                {
                    if (restResponse.Content != null)
                        apiResponse = await restResponse.Content.ReadAsStringAsync();
                    else 
                        throw new Exception("Müşteri kazanım servisinden hata alındı.");
                }
                else 
                    throw new Exception("Müşteri kazanım servisinden hata alındı.");
            }
            return apiResponse;
        }

        public async Task<CustomerReportServiceDto> GetCustomerReportData(CustomerReportRequest request) 
        {
            CustomerReportServiceDto customerReportServiceDto = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await _parameterService.GetAccessToken();
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
                    if (restResponse.Content != null)
                    {
                        var apiResponse = await restResponse.Content.ReadAsStringAsync();
                        customerReportServiceDto = JsonConvert.DeserializeObject<CustomerReportServiceDto>(apiResponse);
                    }
                }
                else
                {
                    throw new Exception("Rapor servisinden veri çekilemedi.");
                }
            }
            return customerReportServiceDto;
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
