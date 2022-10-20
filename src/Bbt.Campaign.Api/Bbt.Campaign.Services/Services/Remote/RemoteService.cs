using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Public.Models.MessagingTemplate;
using Bbt.Campaign.Public.Models.Parameter;
using Bbt.Campaign.Public.Models.RemoteServicModel;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.CacheKey;
using Bbt.Campaign.Shared.ServiceDependencies;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;

namespace Bbt.Campaign.Services.Services.Remote
{
    public class RemoteService : IRemoteService, IScopedService
    {
        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;

        public RemoteService(IUnitOfWork unitOfWork, IParameterService parameterService, IRedisDatabaseProvider redisDatabaseProvider)
        {
            _unitOfWork = unitOfWork;
            _parameterService = parameterService;
            _redisDatabaseProvider = redisDatabaseProvider;
        }

        public async Task<GoalResultByCustomerIdAndMonthCount> GetGoalResultByCustomerIdAndMonthCountData(string customerCode, int campaignId)
        {
            var goalResultByCustomerIdAndMonthCount = new GoalResultByCustomerIdAndMonthCount();
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("GoalResultByCustomerIdAndMonthCount");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString()).Replace("{customerId}", customerCode).Replace("{monthCount}", "2");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(serviceUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await httpClient.GetAsync(serviceUrl);
                }

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(apiResponse))
                        goalResultByCustomerIdAndMonthCount = JsonConvert.DeserializeObject<GoalResultByCustomerIdAndMonthCount>(apiResponse);
                }
                else
                {
                    throw new Exception("GoalResultByCustomerIdAndMonthCount servisinden veri çekilemedi.");
                }
            }
            return goalResultByCustomerIdAndMonthCount;
        }
        public async Task<GoalResultByCustomerAndCampaing> GetGoalResultByCustomerAndCampaingData(string customerId, int campaignId, string lang)
        {
            GoalResultByCustomerAndCampaing goalResultByCustomerAndCampaing = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("GoalResultByCustomerAndCampaing");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{lang}", lang);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.GetAsync(serviceUrl);
                }

                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
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
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("EarningByCustomerAndCampaing");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{lang}", lang);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.GetAsync(serviceUrl);
                }

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
                string accessToken = await GetAccessTokenFromCache();
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
                    serviceUrl += "&CustomerId=" + request.CustomerCode;

                if (!string.IsNullOrEmpty(request.CustomerIdentifier))
                    serviceUrl += "&CustomerNumber=" + request.CustomerIdentifier;
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

                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.GetAsync(serviceUrl);
                }

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

        public async Task<TargetReportServiceDto> GetTargetReportData(TargetReportRequest request)
        {
            TargetReportServiceDto targetReportServiceDto = null;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("TargetReport");
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
                        case "TargetName":
                            sortBy = "TargetName";
                            break;
                        case "CampaignName":
                            sortBy = "CampaignName";
                            break;
                        case "IsJoin":
                            sortBy = "IsJoin";
                            break;
                        case "CustomerCode":
                            sortBy = "CustomerNumber";
                            break;
                        case "IdentitySubTypeId":
                            sortBy = "SubSegment";
                            break;
                        case "TargetAmount":
                            sortBy = "TargetAmount";
                            break;
                        case "TargetAmountCurrency":
                            sortBy = "TargetAmountCurrency";
                            break;
                        case "RemainAmount":
                            sortBy = "RemainingAmount";
                            break;
                        case "RemainAmountCurrency":
                            sortBy = "RemainingAmountCurrency";
                            break;
                        case "IsTargetSuccess":
                            sortBy = "IsCompleted";
                            break;
                        case "TargetSuccessStartDate":
                            sortBy = "CompletedAt";
                            break;
                    }

                    serviceUrl += "&SortBy=" + sortBy;

                    bool isDescending = request.SortDir?.ToLower() == "desc";

                    serviceUrl += "&SortType=" + (isDescending ? (int)SortTypeEnum.Descending : (int)SortTypeEnum.Ascending);

                }
                else
                {
                    serviceUrl += "&SortBy=CompletedAt";
                    serviceUrl += "&SortType=" + (int)SortTypeEnum.Descending;
                }

                if (request.CampaignId.HasValue)
                {
                    int campaignId = request.CampaignId ?? 0;
                    var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
                    serviceUrl += "&CampaignCode=" + campaignEntity.Code;
                }

                if (request.TargetId.HasValue)
                {
                    int targetId = request.TargetId ?? 0;
                    var targetEntity = await _unitOfWork.GetRepository<TargetEntity>().GetByIdAsync(targetId);
                    serviceUrl += "&TargetCode=" + targetEntity.Code;
                }

                if (!string.IsNullOrEmpty(request.CustomerCode))
                    serviceUrl += "&CustomerNumber=" + request.CustomerCode;
                if (request.IdentitySubTypeId.HasValue)
                {
                    int identitySubTypeId = request.IdentitySubTypeId ?? 0;
                    serviceUrl += "&SubSegment=" + identitySubTypeId;
                }
                if (request.IsJoin.HasValue)
                    serviceUrl += "&IsJoined=" + request.IsJoin;

                if (!string.IsNullOrEmpty(request.TargetSuccessStartDate))
                {
                    string[] startDateArray = request.TargetSuccessStartDate.Split('-');
                    if (startDateArray.Length == 3)
                    {
                        serviceUrl += "&StartDate=" + startDateArray[2] + "-" + startDateArray[1] + "-" + startDateArray[0];
                    }
                }

                if (!string.IsNullOrEmpty(request.TargetSuccessEndDate))
                {
                    string[] endDateArray = request.TargetSuccessEndDate.Split('-');
                    if (endDateArray.Length == 3)
                    {
                        serviceUrl += "&EndDate=" + endDateArray[2] + "-" + endDateArray[1] + "-" + endDateArray[0];
                    }
                }

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);

                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.GetAsync(serviceUrl);
                }

                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    targetReportServiceDto = JsonConvert.DeserializeObject<TargetReportServiceDto>(apiResponse);
                }
                else
                {
                    throw new Exception("Hedef raporu servisinden veri çekilemedi.");
                }
            }
            return targetReportServiceDto;
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
                        throw new Exception("Kullanıcı modeli bulunamadı.");

                    if (userModel.Credentials == null || !userModel.Credentials.Any())
                        throw new Exception("Kullanıcı rolleri bulunamadı.");

                    if (userModel.CitizenshipNumber == null)
                        throw new Exception("Tckn bilgisi bulunamadı.");
                }
            }
            else { throw new Exception("Invalid state."); }
            return userModel;
        }
        public async Task<Document> GetDocument(int id)
        {
            var document = new Document();
            string accessToken = await GetAccessTokenFromCache();

            using (var httpClient = new HttpClient())
            {
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("Document");
                apiAddress = apiAddress.Replace("{key}", id.ToString());
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var restResponse = await httpClient.GetAsync(serviceUrl);

                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.GetAsync(serviceUrl);
                }

                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    document = JsonConvert.DeserializeObject<Document>(apiResponse);
                }
                else if (restResponse.StatusCode.ToString() == "455")
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

        public async Task SendSmsMessageTemplate(string customerId, int campaignId,int messageTypeId, TemplateInfo templateData)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("SendSmsMessageTemplate");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{messageTypeId}", messageTypeId.ToString());
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var template = JsonConvert.SerializeObject(templateData);
                var requestContent = new StringContent(template, Encoding.UTF8, "application/json");

                var restResponse = await httpClient.PostAsync(serviceUrl, requestContent);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, requestContent);
                }
            }
        }

        public async Task SendNotificationMessageTemplate(string customerId, int campaignId, int messageTypeId, TemplateInfo templateData)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("SendNotificationMessageTemplate");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{messageTypeId}", messageTypeId.ToString());
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var template = JsonConvert.SerializeObject(templateData);
                var requestContent = new StringContent(template, Encoding.UTF8, "application/json");

                var restResponse = await httpClient.PostAsync(serviceUrl, requestContent);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, requestContent);
                }
            }
        }

        public async Task<HttpResponseMessage> CustomerAchievementsAdd(string customerId, int campaignId, string term)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("CustomerAchievementAdd");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{term}", term.ToString());
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.PostAsync(serviceUrl,null);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                return restResponse;
            }
        }

        public async Task<HttpResponseMessage> CustomerAchievementsDelete(string customerId, int campaignId, string term)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("CustomerAchievementDelete");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{term}", term.ToString());
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.PostAsync(serviceUrl, null);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                return restResponse;
            }
        }

        public async Task<HttpResponseMessage> LeaveProgramAchievementDelete(string customerId, int campaignId, string term)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("LeaveProgramAchievementDelete");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                serviceUrl = serviceUrl.Replace("{term}", term.ToString());
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.PostAsync(serviceUrl, null);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                return restResponse;
            }
        }

        public async Task<HttpResponseMessage> SendDmsDocuments(string customerId, List<int> documentIds)
        {
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("SendDmsDocument");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var data = JsonConvert.SerializeObject(documentIds);
                var requestContent = new StringContent(data, Encoding.UTF8, "application/json");
                var restResponse = await httpClient.PostAsync(serviceUrl, requestContent);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                return restResponse;
            }
        }

        public async Task<bool> GetAccounts(string customerId)
        {
            bool response = false;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("GetAccounts");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                serviceUrl = serviceUrl.Replace("{customerId}", customerId);
                serviceUrl = serviceUrl + "?status=active%2Cpassive";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                if (restResponse.IsSuccessStatusCode)
                {
                    var apiResponse = await restResponse.Content.ReadAsStringAsync();
                    var accountsList = JsonConvert.DeserializeObject<GetAccounts.Response>(apiResponse);
                    var result = accountsList?.Checking.Where(x => x.Product.SubProduct?.ProductCode == "VDLGLDR").FirstOrDefault();
                    if (result != null)
                        response = true;
                }
                return response;
            }
        }
        public async Task<bool> CleanCache()
        {
            bool response = false;
            using (var httpClient = new HttpClient())
            {
                string accessToken = await GetAccessTokenFromCache();
                string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                string apiAddress = await _parameterService.GetServiceConstantValue("CleanCache");
                string serviceUrl = string.Concat(baseAddress, apiAddress);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var restResponse = await httpClient.GetAsync(serviceUrl);
                if (restResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    accessToken = await GetAccessTokenFromService();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    restResponse = await httpClient.PostAsync(serviceUrl, null);
                }
                if (restResponse.IsSuccessStatusCode)
                {
                        response = true;
                }
                return response;
            }
        }
        private async Task<string> GetAccessTokenFromCache()
        {
            string result = string.Empty;
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.AccessToken);
            if (!string.IsNullOrEmpty(cache))
            {
                var accessTokenList = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
                if (accessTokenList != null && accessTokenList.Count == 1)
                    result = accessTokenList[0].Name;
            }
            else
            {
                result = await GetAccessTokenFromService();
            }
            return result;
        }

        private async Task<string> GetAccessTokenFromService()
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

                //set cache
                var accessTokenList = new List<ParameterDto>();
                accessTokenList.Add(new ParameterDto() { Id = 1, Code = CacheKeys.AccessToken, Name = accessToken });
                var cacheValue = JsonConvert.SerializeObject(accessTokenList);
                await _redisDatabaseProvider.SetAsync(CacheKeys.AccessToken, cacheValue);

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
