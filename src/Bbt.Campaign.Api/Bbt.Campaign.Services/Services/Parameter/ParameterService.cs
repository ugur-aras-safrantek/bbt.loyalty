using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Branch;
using Bbt.Campaign.Public.Models.CampaignRule;
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
        //public async Task<BaseResponse<List<string>>> GetCampaignChannelListAsync()
        //{
        //    var result = new List<string>()
        //    {"BATCH","BAYI","DIGER","INTERNET","PTT","REMOTE","SMS","TABLET","WEB","WEBBAYI","WEBMEVDUAT" };

        //    return await BaseResponse<List<string>>.SuccessAsync(result);
        //}
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

        public async Task<BaseResponse<List<ParameterDto>>> GetBranchListAsync()
        {
            List<ParameterDto> result = null;
            bool isExists = false;
            bool isUpToDate = false;

            var cacheBranchDate = await _redisDatabaseProvider.GetAsync(CacheKeys.BranchSelectDate);
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.BranchList);

            if (cacheBranchDate != null)
            {
                if (cacheBranchDate.ToString() != "null")
                {
                    isExists = true;

                    var valueDateResult = JsonConvert.DeserializeObject<List<ParameterDto>>(cacheBranchDate);

                    isUpToDate = valueDateResult[0].Name == todayStr;
                }
            }

            // cache te varsa ve bugun güncellenmiş ise direk getir
            if (cache != null && cache.ToString() != "null" && isExists && isUpToDate)
            {
                result = JsonConvert.DeserializeObject<List<ParameterDto>>(cache);
            }
            else
            {
                List<ParameterDto> valueDateList = new List<ParameterDto>();
                valueDateList.Add(new ParameterDto() { Id = 1, Name = todayStr, Code = todayStr, });
                var valueDate = JsonConvert.SerializeObject(valueDateList);
                await _redisDatabaseProvider.SetAsync(CacheKeys.BranchSelectDate, valueDate);

                if (StaticValues.IsDevelopment)
                {
                    var serviceResult = new List<BranchDto>(){
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

                    var value = JsonConvert.SerializeObject(serviceResult);

                    await _redisDatabaseProvider.SetAsync(CacheKeys.BranchList, value);

                    result = serviceResult.Select(x => new ParameterDto
                    {
                        Code = x.Code,
                        Name = x.Name
                    }).ToList();
                }
                else
                {
                    List<Branch> branchList = new List<Branch>();
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await httpClient.GetAsync(StaticValues.BranchServiceUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content != null)
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                await _redisDatabaseProvider.SetAsync(CacheKeys.BranchList, apiResponse);
                                branchList = JsonConvert.DeserializeObject<List<Branch>>(apiResponse);
                                result = branchList.Select(x => new ParameterDto
                                {
                                    Code = x.Code,
                                    Name = x.Name
                                }).ToList();
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
                }
            }

            return await BaseResponse<List<ParameterDto>>.SuccessAsync(result);
        }

        public async Task<BaseResponse<List<string>>> GetCampaignChannelListAsync()
        {
            List<string> result = null;
            bool isExists = false;
            bool isUpToDate = false;

            var cacheChannelCodeDate = await _redisDatabaseProvider.GetAsync(CacheKeys.ChannelCodeSelectDate);
            var cache = await _redisDatabaseProvider.GetAsync(CacheKeys.CampaignChannelList);

            if (cacheChannelCodeDate != null)
            {
                if (cacheChannelCodeDate.ToString() != "null")
                {
                    isExists = true;

                    var valueDateResult = JsonConvert.DeserializeObject<List<string>>(cacheChannelCodeDate);

                    isUpToDate = valueDateResult[0] == todayStr;
                }
            }

            // cache te varsa ve bugun güncellenmiş ise direk getir
            if (cache != null && cache.ToString() != "null" && isExists && isUpToDate)
            {
                result = JsonConvert.DeserializeObject<List<string>>(cache);
            }
            else
            {
                List<string> valueDateList = new List<string>();
                valueDateList.Add(todayStr);
                var valueDate = JsonConvert.SerializeObject(valueDateList);
                await _redisDatabaseProvider.SetAsync(CacheKeys.ChannelCodeSelectDate, valueDate);

                if (StaticValues.IsDevelopment)
                {
                    result = new List<string>()
                    {"BATCH","BAYI","DIGER","INTERNET","PTT","REMOTE","SMS","TABLET","WEB","WEBBAYI","WEBMEVDUAT" };

                    var value = JsonConvert.SerializeObject(result);

                    await _redisDatabaseProvider.SetAsync(CacheKeys.CampaignChannelList, value);
                }
                else
                {
                    result = new List<string>();
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await httpClient.GetAsync(StaticValues.ChannelCodeServiceUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content != null)
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                await _redisDatabaseProvider.SetAsync(CacheKeys.CampaignChannelList, apiResponse);
                                result = JsonConvert.DeserializeObject<List<string>>(apiResponse);
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
            }
            return await BaseResponse<List<string>>.SuccessAsync(result);
        }

        public Task<BaseResponse<List<ParameterDto>>> GetParticipationTypeListAsync()
        {
            return GetListAsync<ParticipationTypeEntity>(CacheKeys.ParticipationType);
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
