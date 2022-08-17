﻿using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Branch;
using Bbt.Campaign.Public.Models.Parameter;
using Bbt.Campaign.Shared.CacheKey;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.Parameter
{
    public class ParameterService : IParameterService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;


        private static string todayStr = DateTime.Now.ToString("dd-MM-yyyy");

        public ParameterService(IUnitOfWork unitOfWork, IMapper mapper, IRedisDatabaseProvider redisDatabaseProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisDatabaseProvider = redisDatabaseProvider;
        }

        public async Task<BaseResponse<List<ParameterDto>>> GetListAsync<T>(string cacheKey) where T : class
        {
            List<ParameterDto> result = null;
            var cache = await _redisDatabaseProvider.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cache))
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
            else
            {
                result = _unitOfWork.GetRepository<T>().GetAll()
                                                    .Select(x => _mapper.Map<ParameterDto>(x))
                                                    .ToList();
                var value = JsonConvert.SerializeObject(result);
                await _redisDatabaseProvider.SetAsync(cacheKey, value);
            }
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetAchievementFrequencyListAsync()
        {
            return GetListAsync<AchievementFrequencyEntity>(CacheKeys.AchievementFrequencyList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetActionOptionListAsync()
        {
            return GetListAsync<ActionOptionEntity>(CacheKeys.ActionOptionList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetBusinessLineListAsync()
        {
            return GetListAsync<BusinessLineEntity>(CacheKeys.BusinessLineList);
        }  
        public Task<BaseResponse<List<ParameterDto>>> GetCampaignStartTermListAsync()
        {
            return GetListAsync<CampaignStartTermEntity>(CacheKeys.CampaignStartTermList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetCurrencyListAsync()
        {
            return GetListAsync<CurrencyEntity>(CacheKeys.CurrencyList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetCustomerTypeListAsync()
        {
            return GetListAsync<CustomerTypeEntity>(CacheKeys.CustomerTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetJoinTypeListAsync()
        {
            return GetListAsync<JoinTypeEntity>(CacheKeys.JoinTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetLanguageListAsync()
        {
            return GetListAsync<LanguageEntity>(CacheKeys.LanguageList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetSectorListAsync()
        {
            return GetListAsync<SectorEntity>(CacheKeys.SectorList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetTargetDefinitionListAsync()
        {
            return GetListAsync<TargetDefinitionEntity>(CacheKeys.TargetDefinitionList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetTargetOperationListAsync()
        {
            return GetListAsync<TargetOperationEntity>(CacheKeys.TargetOperationList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetViewOptionListAsync()
        {
            return GetListAsync<ViewOptionEntity>(CacheKeys.ViewOptionList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetProgramTypeListAsync()
        {
            return GetListAsync<ProgramTypeEntity>(CacheKeys.ProgramTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetTargetSourceListAsync()
        {
            return GetListAsync<TargetSourceEntity>(CacheKeys.TargetSourceList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetTargetViewTypeListAsync()
        {
            return GetListAsync<TargetViewTypeEntity>(CacheKeys.TargetViewTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetTriggerTimeListAsync()
        {
            return GetListAsync<TriggerTimeEntity>(CacheKeys.TriggerTimeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetVerificationTimeListAsync()
        {
            return GetListAsync<VerificationTimeEntity>(CacheKeys.VerificationTime);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetAchievementTypeListAsync()
        {
            return GetListAsync<AchievementTypeEntity>(CacheKeys.AchievementType);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetParticipationTypeListAsync()
        {
            return GetListAsync<ParticipationTypeEntity>(CacheKeys.ParticipationType);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetRoleTypeListAsync()
        {
            return GetListAsync<RoleTypeEntity>(CacheKeys.RoleTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetModuleTypeListAsync()
        {
            return GetListAsync<ModuleTypeEntity>(CacheKeys.ModuleTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetAuthorizationTypeListAsync()
        {
            return GetListAsync<AuthorizationTypeEntity>(CacheKeys.AuthorizationTypeList);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetAllUsersRoleListAsync()
        {
            List<ParameterDto> result = new List<ParameterDto>();
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.AllUsersRoleList);
            if (!string.IsNullOrEmpty(cache))
            {
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
            }
            else
            {
                var userRoleList = _unitOfWork.GetRepository<UserRoleEntity>().GetAll().ToList();
                foreach (var userRole in userRoleList)
                    result.Add(new ParameterDto() { Id = userRole.Id, Code = userRole.UserId, Name = userRole.RoleTypeId.ToString() });

                var cacheValue = JsonConvert.SerializeObject(result);
                await _redisDatabaseProvider.SetAsync(CacheKeys.AllUsersRoleList, cacheValue);
            }
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetAllUsersRoleListInProgressAsync(string cacheKey)
        {
            List<ParameterDto> result = null;
            var cache = await _redisDatabaseProvider.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cache))
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetSingleUserRoleListAsync(string userId)
        {
            List<ParameterDto> result = new List<ParameterDto>();
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.AllUsersRoleList);
            if (!string.IsNullOrEmpty(cache)) 
            { 
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
                if(result != null)
                    result = result.Where(x => x.Code == userId).ToList();
            }  
            else
            {
                var userRoleList = _unitOfWork.GetRepository<UserRoleEntity>().GetAll().Where(x => x.UserId == userId).ToList();
                foreach (var userRole in userRoleList)
                    result.Add(new ParameterDto() { Id = userRole.Id, Code = userRole.UserId, Name = userRole.RoleTypeId.ToString()});
            }
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }

        public async Task<string> GetUserLastProcessDate(string userId) 
        {
            string retVal = string.Empty;
            List<ParameterDto> result = null;
            string cacheKey = string.Concat(userId, "_", CacheKeys.LastProcessDate);
            var cache = await _redisDatabaseProvider.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cache))
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
            if (result != null && result.Any())
                retVal = result[0].Name;
            return retVal;
        }

        public async Task<string> SetUserLastProcessDate(string userId) 
        {
            string lastProcessDate = Helpers.ConvertWithTimeForBackEnd(DateTime.Now);
            List<ParameterDto> result = new List<ParameterDto>();
            ParameterDto parameterDto = new ParameterDto() { Id= 1, Code = userId, Name = lastProcessDate };
            result.Add(parameterDto);
            string cacheKey = string.Concat(userId, "_", CacheKeys.LastProcessDate);
            var value = JsonConvert.SerializeObject(result);
            await _redisDatabaseProvider.SetAsync(cacheKey, value);

            return await GetUserLastProcessDate(userId);
        }


        //public async Task<BaseResponse<List<UserRoleDto>>> GetUserRoleListAsync(string userId)
        //{
        //    List<UserRoleDto> result = new List<UserRoleDto>();
        //    string cacheKey = string.Format(CacheKeys.UserRoleList, userId);
        //    var cache = await _redisDatabaseProvider.GetAsync(cacheKey);
        //    if (string.IsNullOrEmpty(cache))
        //    {
        //        return await SetUserRoleListAsync(userId, result);
        //    }
        //    else
        //    {
        //        result = JsonConvert.DeserializeObject<List<UserRoleDto>>(cache);
        //    }
        //    return await BaseResponse<List<UserRoleDto>>.SuccessAsync(result);
        //}
        //public async Task<BaseResponse<List<UserRoleDto>>> SetUserRoleListAsync(string userId, List<UserRoleDto> _userRoleList)
        //{
        //    string cacheKey = string.Format(CacheKeys.UserRoleList, userId);
        //    await _redisDatabaseProvider.RemoveAsync(cacheKey);
        //    List<UserRoleDto> userRoleList = _userRoleList;
        //    if(!userRoleList.Any()) 
        //    {
        //        userRoleList = _unitOfWork.GetRepository<UserRoleEntity>().GetAll()
        //            .Where(x => x.UserId == userId).Select(x => _mapper.Map<UserRoleDto>(x)).ToList();
        //    }
        //    if (userRoleList.Any()) 
        //    {
        //        var cacheValue = JsonConvert.SerializeObject(userRoleList);
        //        await _redisDatabaseProvider.SetAsync(cacheKey, cacheValue);
        //    }
        //    return await BaseResponse<List<UserRoleDto>>.SuccessAsync(userRoleList);
        //}
        public async Task<BaseResponse<List<RoleAuthorizationDto>>> GetRoleAuthorizationListAsync()
        {
            List<RoleAuthorizationDto> result = null;
            string cacheKey = CacheKeys.RoleAuthorizationList;
            var cache = await _redisDatabaseProvider.GetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cache))
                result = JsonConvert.DeserializeObject<List<RoleAuthorizationDto>>(cache);
            else
            {
                result = _unitOfWork.GetRepository<RoleAuthorizationEntity>().GetAll()
                                                    .Select(x => _mapper.Map<RoleAuthorizationDto>(x))
                                                    .ToList();
                var value = JsonConvert.SerializeObject(result);
                await _redisDatabaseProvider.SetAsync(cacheKey, value);
            }
            return await BaseResponse<List<RoleAuthorizationDto>>.SuccessAsync(result);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetBranchListAsync()
        {
            List<ParameterDto> result = new List<ParameterDto>();
            var serviceResult = new List<BranchDto>();

            var cacheBranchDateData = await _redisDatabaseProvider.GetAsync(CacheKeys.BranchSelectDate);
            var cacheBranchData = await _redisDatabaseProvider.GetAsync(CacheKeys.BranchList);

            if (cacheBranchDateData != null && cacheBranchData != null)
            {
                var cacheBranchDateList = JsonConvert.DeserializeObject<List<ParameterDto>>(cacheBranchDateData);
                if(cacheBranchDateList != null && cacheBranchDateList.Any() && cacheBranchDateList[0].Name == todayStr) 
                { 
                    var cacheBranchList = JsonConvert.DeserializeObject<List<ParameterDto>>(cacheBranchData);
                    if (cacheBranchList != null && cacheBranchList.Any())
                    {
                        return await GetListAsync<ParameterDto>(CacheKeys.BranchList);
                    }
                }
            }

            await _redisDatabaseProvider.RemoveByPattern(CacheKeys.BranchList);
            await _redisDatabaseProvider.RemoveByPattern(CacheKeys.BranchSelectDate);

            //get branch list
            if (StaticValues.IsDevelopment)
            {
                serviceResult = new List<BranchDto>(){
                new BranchDto {
                    Address = "REŞATBEY MAH. ATATÜRK CAD. KORAL APT. NO:36 SEYHAN",
                    CityCode= "1",
                    CityName = "ADANA",
                    Code = "9297",
                    FaxNo = "3223521700",
                    Name = "ADANA",
                    TelNo = "3223524444",
                    TownCode = "0116",
                    TownName = "SEYHAN"},
                new BranchDto{Address= "Çankaya Cad. No:8 Daire:7-8 P.K. 06680",
                    CityCode= "6",
                    CityName= "ANKARA",
                    Code= "9092",
                    FaxNo= "312 4182262",
                    Name= "ANKARA",
                    TelNo= "312 4187979",
                    TownCode= "0602",
                    TownName= "ÇANKAYA"},
                new BranchDto{
                    Address= "Mustafa Kemal Mahallesi, Dumlupınar Bulvarı, B Blok No:274/7-201",
                    CityCode= "6",
                    CityName= "ANKARA",
                    Code= "9560",
                    FaxNo= "312 2856070",
                    Name= "ANKARA PLAZA",
                    TelNo= "312 2861900",
                    TownCode= "0602",
                    TownName= "ÇANKAYA"}};
            }
            else
            {
                using (var httpClient = new HttpClient())
                {
                    string baseAddress = await GetServiceConstantValue("BaseAddress");
                    string apiAddress = await GetServiceConstantValue("BranchInfo");
                    string serviceUrl = string.Concat(baseAddress, apiAddress);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var restResponse = await httpClient.GetAsync(serviceUrl);
                    if (restResponse.IsSuccessStatusCode)
                    {
                        string apiResponse = await restResponse.Content.ReadAsStringAsync();
                        serviceResult = JsonConvert.DeserializeObject<List<BranchDto>>(apiResponse);
                        if (serviceResult != null && serviceResult.Any()) { }
                        else
                        {
                            throw new Exception("Şube listesi servisinden veri çekilemedi.");
                        }
                    }
                    else { throw new Exception("Şube listesi servisinden veri çekilemedi."); }
                }
            }

            List<ParameterDto> todayDateList = new List<ParameterDto>();
            todayDateList.Add(new ParameterDto() { Id = 1, Name = todayStr, Code = todayStr, });
            var todayDateSerialized = JsonConvert.SerializeObject(todayDateList);

            result = serviceResult.Select(x => new ParameterDto() { Id = 0, Name= x.Name, Code= x.Code}).ToList();
            var branchListSerialized = JsonConvert.SerializeObject(result);
            await _redisDatabaseProvider.SetAsync(CacheKeys.BranchList, branchListSerialized);
            await _redisDatabaseProvider.SetAsync(CacheKeys.BranchSelectDate, todayDateSerialized);
            return await GetListAsync<ParameterDto>(CacheKeys.BranchList);
        }
        public async Task<BaseResponse<List<string>>> GetCampaignChannelListAsync()
        {
            List<string> channelCodeList = new List<string>();

            var channelCodeDateData = await _redisDatabaseProvider.GetAsync(CacheKeys.ChannelCodeSelectDate);
            var channelCodeData = await _redisDatabaseProvider.GetAsync(CacheKeys.CampaignChannelList);

            // cache te varsa ve bugun güncellenmiş ise direk getir
            if (channelCodeDateData != null && channelCodeData != null)
            {
                var channelCodeDateList = JsonConvert.DeserializeObject<List<ParameterDto>>(channelCodeDateData);
                if(channelCodeDateList != null && channelCodeDateList.Any() && channelCodeDateList[0].Code == todayStr) 
                { 
                    channelCodeList = JsonConvert.DeserializeObject<List<string>>(channelCodeData);
                    if (channelCodeList != null && channelCodeList.Any())
                        return await BaseResponse<List<string>>.SuccessAsync(channelCodeList);
                }
            }

            await _redisDatabaseProvider.RemoveByPattern(CacheKeys.CampaignChannelList);
            await _redisDatabaseProvider.RemoveByPattern(CacheKeys.ChannelCodeSelectDate);

            //get data
            if (StaticValues.IsDevelopment)
            {
                channelCodeList = new List<string>()
                    {"BATCH","BAYI","DIGER","INTERNET","PTT","REMOTE","SMS","TABLET","WEB","WEBBAYI","WEBMEVDUAT" };
            }
            else 
            {
                using (var httpClient = new HttpClient())
                {
                    string baseAddress = await GetServiceConstantValue("BaseAddress");
                    string apiAddress = await GetServiceConstantValue("ChannelCode");
                    string serviceUrl = string.Concat(baseAddress, apiAddress);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await httpClient.GetAsync(serviceUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        channelCodeList = JsonConvert.DeserializeObject<List<string>>(apiResponse);
                        if (channelCodeList != null && channelCodeList.Any()) { }
                        else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                    }
                    else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                }
            }

            List<ParameterDto> todayDateList = new List<ParameterDto>();
            todayDateList.Add(new ParameterDto() { Id = 1, Code = todayStr, Name = todayStr,  });
            var todayDateSerialized = JsonConvert.SerializeObject(todayDateList);

            string channelCodeListSerialized = JsonConvert.SerializeObject(channelCodeList);
            await _redisDatabaseProvider.SetAsync(CacheKeys.CampaignChannelList, channelCodeListSerialized);
            await _redisDatabaseProvider.SetAsync(CacheKeys.ChannelCodeSelectDate, todayDateSerialized);
            return await BaseResponse<List<string>>.SuccessAsync(channelCodeList);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetBranchSelectDateListAsync()
        {
            List<ParameterDto> result = new List<ParameterDto>();
            var cacheBranchDateData = await _redisDatabaseProvider.GetAsync(CacheKeys.BranchSelectDate);
            if (!string.IsNullOrEmpty(cacheBranchDateData))
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cacheBranchDateData);
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
        public async Task<BaseResponse<List<ParameterDto>>> GetChannelCodeSelectDateListAsync()
        {
            List<ParameterDto> result = new List<ParameterDto>();
            var channelCodeDateData = await _redisDatabaseProvider.GetAsync(CacheKeys.ChannelCodeSelectDate);
            if (!string.IsNullOrEmpty(channelCodeDateData))
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(channelCodeDateData);
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetServiceConstantListAsync()
        {
            return GetListAsync<ConstantsEntity>(CacheKeys.ServiceConstantList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetStatusListAsync()
        {
            return GetListAsync<StatusEntity>(CacheKeys.StatusList);
        }
        public async Task<string> GetServiceData(string serviceUrl) 
        {
            string retVal = string.Empty;
            using (var httpClient = new HttpClient()) 
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode) 
                {
                    if (response.Content != null)
                    {
                        retVal = await response.Content.ReadAsStringAsync();
                        //var list = JsonConvert.DeserializeObject<string>(retVal);


                        //List<string> tmp = JsonConvert.DeserializeObject<List<string>>((JObject.Parse(retVal)["data"]).ToString());
                    }
                    else
                    {
                        throw new Exception("Şube listesi servisinden veri çekilemedi.");
                    }
                }
                else 
                {
                    throw new Exception("Şube listesi servisinden veri çekilemedi.");
                }
            }

            return retVal;
        }
        public async Task<string> GetServiceConstantValue(string code)
        {
            string retVal = string.Empty;
            var serviceConstantList = (await GetServiceConstantListAsync())?.Data;
            var serviceConstant = serviceConstantList?.Where(x => x.Code == code).FirstOrDefault();
            if (serviceConstant != null)
                retVal = serviceConstant.Name;
            return retVal;
        }
        public async Task<string> GetUserRoles(string code, string state)
        {
            string accessToken = string.Empty;
            if (state == "LoyaltyGondor") 
            {
                using (var client = new HttpClient())
                {
                    string baseAddress = await GetServiceConstantValue("AccessTokenBaseAddress");
                    string apiAddress = await GetServiceConstantValue("AccessTokenApiAddress");
                    client.BaseAddress = new Uri(baseAddress);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("client_id", await GetServiceConstantValue("client_id")),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("client_secret", await GetServiceConstantValue("client_secret")),
                        new KeyValuePair<string, string>("redirect_uri", await GetServiceConstantValue("redirect_uri")),
                    });

                    var result = await client.PostAsync(apiAddress, content);
                    var responseContent = result.Content.ReadAsStringAsync().Result;
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(result.Content.ReadAsStringAsync().Result);
                    if (token == null)
                        throw new Exception("Token servisinden veri çekilirken hata alındı.");
                    accessToken = token.Access_token;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://gondor-apigateway.burgan.com.tr"); 
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("access_token", accessToken),
                    });
                    var result = await client.PostAsync(await GetServiceConstantValue("ResourceApiAddress"), content);
                    var responseContent = result.Content.ReadAsStringAsync().Result;
                }
            }
            else { throw new Exception("Invalid state."); }
            return accessToken;
        }
        public Task<BaseResponse<List<ParameterDto>>> GetIdentitySubTypeListAsync()
        {
            return GetListAsync<IdentitySubTypeEntity>(CacheKeys.IdentitySubTypeList);
        }
    }
}
