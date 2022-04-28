using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Branch;
using Bbt.Campaign.Shared.CacheKey;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.Parameter
{
    public class ParameterService : IParameterService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;

        private static string todayStr = DateTime.Now.ToString("dd-MM-yyyy");

        public ParameterService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IRedisDatabaseProvider redisDatabaseProvider)
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
        public Task<BaseResponse<List<ParameterDto>>> GetProcessTypeListAsync()
        {
            return GetListAsync<ProcessTypeEntity>(CacheKeys.ProcessTypeList);
        }
        public Task<BaseResponse<List<ParameterDto>>> GetAllUsersRoleListAsync()
        {
            return GetListAsync<UserRoleEntity>(CacheKeys.AllUsersRoleList);
        }

        public async Task<BaseResponse<List<ParameterDto>>> GetSingleUserRoleListAsync(string userId)
        {
            List<ParameterDto> result = null;
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.AllUsersRoleList);
            if (!string.IsNullOrEmpty(cache)) 
            { 
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
                if(result != null)
                    result = result.Where(x=>x.Code == userId).ToList();
            }  
            else
            {
                result = _unitOfWork.GetRepository<UserRoleEntity>()
                    .GetAll()
                    .Where(x=>x.UserId == userId && !x.IsDeleted)
                    .Select(x => _mapper.Map<ParameterDto>(x))
                    .ToList();
            }
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }
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
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await httpClient.GetAsync(StaticValues.BranchServiceUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content != null)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            serviceResult = JsonConvert.DeserializeObject<List<BranchDto>>((JObject.Parse(apiResponse)["data"]).ToString());
                            if(serviceResult != null && serviceResult.Any()) { } 
                            else{ throw new Exception("Şube listesi servisinden veri çekilemedi."); }
                            
                        }
                        else { throw new Exception("Şube listesi servisinden veri çekilemedi."); }
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
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await httpClient.GetAsync(StaticValues.ChannelCodeServiceUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content != null)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            channelCodeList = JsonConvert.DeserializeObject<List<string>>((JObject.Parse(apiResponse)["data"]).ToString());
                            if (channelCodeList != null && channelCodeList.Any()){}
                            else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                        }
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
    }
}
