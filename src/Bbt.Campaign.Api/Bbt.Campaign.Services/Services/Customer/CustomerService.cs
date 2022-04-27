using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Services.ListData;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;

namespace Bbt.Campaign.Services.Services.Customer
{
    public class CustomerService : ICustomerService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRuleService _campaignRuleService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly ICampaignAchievementService _campaignAchievementService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService,
            ICampaignService campaignService, ICampaignRuleService campaignRuleService, ICampaignTargetService campaignTargetService,
            ICampaignAchievementService campaignAchievementService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignRuleService = campaignRuleService;
            _campaignTargetService = campaignTargetService;
            _campaignAchievementService = campaignAchievementService;
        }

        public async Task<BaseResponse<CustomerCampaignDto>> SetJoin(string customerCode, int campaignId, bool isJoin) 
        {
            await CheckValidationAsync(customerCode, campaignId);

            var entity = new CustomerCampaignEntity();
            entity.CustomerCode = customerCode;
            entity.CampaignId = campaignId;
            entity.IsJoin= isJoin;
            entity.StartDate = DateTime.Parse(DateTime.Now.ToShortDateString());          
            entity.CreatedBy = customerCode;

            entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            if (!StaticValues.IsDevelopment) 
            {
                string errorMessage = string.Empty;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string contentValue = "";
                    var httpContent = new StringContent(JsonConvert.SerializeObject(contentValue), Encoding.UTF8, "application/json");
                    var httpResponse = await httpClient.PutAsync(StaticValues.ChannelCodeServiceUrl, httpContent);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        if (httpResponse.Content != null)
                        {
                            string apiResponse = await httpResponse.Content.ReadAsStringAsync();

                            entity.RefId = "";
                            entity.ApiResponse = apiResponse;
                        }
                        else 
                        {
                            errorMessage = "Kampanya müşteri kaydı başarısız.";
                            return await BaseResponse<CustomerCampaignDto>.FailAsync(errorMessage);
                        }
                    }
                    else 
                    {
                        errorMessage = "Kampanya müşteri kaydı başarısız.";
                        return await BaseResponse<CustomerCampaignDto>.FailAsync(errorMessage);
                    }
                }
            }

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        public async Task<BaseResponse<CustomerCampaignDto>> SetFavorite(string customerCode, int campaignId, bool isFavorite)
        {
            await CheckValidationAsync(customerCode, campaignId);

            var entity = new CustomerCampaignFavoriteEntity();
            entity.CustomerCode = customerCode;
            entity.CampaignId = campaignId;
            entity.IsFavorite = isFavorite;
            entity.StartDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            entity.CreatedBy = customerCode;

            entity = await _unitOfWork.GetRepository<CustomerCampaignFavoriteEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            if (!StaticValues.IsDevelopment)
            {
                string errorMessage = string.Empty;

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string contentValue = "";
                    var httpContent = new StringContent(JsonConvert.SerializeObject(contentValue), Encoding.UTF8, "application/json");
                    var httpResponse = await httpClient.PutAsync(StaticValues.ChannelCodeServiceUrl, httpContent);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        if (httpResponse.Content != null)
                        {
                            string apiResponse = await httpResponse.Content.ReadAsStringAsync();

                            entity.RefId = "";
                            entity.ApiResponse = apiResponse;
                        }
                        else
                        {
                            errorMessage = "Kampanya müşteri kaydı başarısız.";
                            return await BaseResponse<CustomerCampaignDto>.FailAsync(errorMessage);
                        }
                    }
                    else
                    {
                        errorMessage = "Kampanya müşteri kaydı başarısız.";
                        return await BaseResponse<CustomerCampaignDto>.FailAsync(errorMessage);
                    }
                }
            }

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        private async Task CheckValidationAsync(string customerCode, int campaignId) 
        {
            if(string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            if(campaignId <= 0)
                throw new Exception("Kampanya giriniz.");

            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
            var campaignEntity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted && x.IsActive &&x.EndDate >= today)
                    .FirstOrDefault();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }
        }
        public async Task<BaseResponse<CustomerCampaignDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCustomerCampaignAsync(entity.Id);
            }
            return await BaseResponse<CustomerCampaignDto>.FailAsync("Kayıt bulunamadı.");
        }
        public async Task<BaseResponse<CustomerCampaignDto>> GetCustomerCampaignAsync(int id)
        {
            var customerCampaignEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (customerCampaignEntity != null)
            {
                CustomerCampaignDto customerCampaignDto = _mapper.Map<CustomerCampaignDto>(customerCampaignEntity);
                return await BaseResponse<CustomerCampaignDto>.SuccessAsync(customerCampaignDto);
            }
            return null;
        }
        public async Task<BaseResponse<CustomerCampaignListFilterResponse>> GetByFilterAsync(CustomerCampaignListFilterRequest request)
        {
            if(request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join || request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite) 
            {
                if (string.IsNullOrEmpty(request.CustomerCode))
                    throw new Exception("Müşteri kodu giriniz.");
            }

            CustomerCampaignListFilterResponse response = new CustomerCampaignListFilterResponse();
            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());

            IQueryable<CampaignDetailListEntity> campaignQuery = await GetCampaignQueryAsync(request);
            if (campaignQuery.Count() == 0)
                return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignQuery.Count();
            campaignQuery = campaignQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);
           
            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
            }).ToList();

            List<int> joinList = new List<int>();
            List<int> favoriteList = new List<int>();

            if (StaticValues.IsDevelopment) 
            {
                if (!string.IsNullOrEmpty(request.CustomerCode)) 
                {
                    var customerJoinList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => !x.IsDeleted && x.CustomerCode == request.CustomerCode)
                        .ToListAsync();
                    if (customerJoinList.Any())
                    {
                        var distinctJoinIdList = customerJoinList.Select(x => x.CampaignId).Distinct().ToList();
                        foreach (int campaignId in distinctJoinIdList)
                        {
                            var item = customerJoinList
                                .Where(x => x.CampaignId == campaignId)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();
                            if (item != null)
                            {
                                if (item.IsJoin)
                                    joinList.Add(item.CampaignId);
                            }
                        }
                    }

                    var customerFavoriteList = await _unitOfWork.GetRepository<CustomerCampaignFavoriteEntity>()
                    .GetAll(x => !x.IsDeleted && x.CustomerCode == request.CustomerCode)
                    .ToListAsync();
                    if (customerFavoriteList.Any())
                    {
                        var distinctFavoriteIdList = customerFavoriteList.Select(x => x.CampaignId).Distinct().ToList();
                        foreach (int campaignId in distinctFavoriteIdList)
                        {
                            var item = customerFavoriteList
                                .Where(x => x.CampaignId == campaignId)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();
                            if (item != null)
                            {
                                if (item.IsFavorite)
                                    favoriteList.Add(item.CampaignId);
                            }
                        }

                    }
                }
            }
            else 
            { 
                
            }
                
            List<CustomerCampaignMinListDto> returnList = new List<CustomerCampaignMinListDto>();
            foreach (var campaign in campaignList)
            {
                CustomerCampaignMinListDto customerCampaignListDto = new CustomerCampaignMinListDto();

                customerCampaignListDto.CampaignId = campaign.Id;
                customerCampaignListDto.Campaign = campaign;
                customerCampaignListDto.CustomerCode = request.CustomerCode;
                customerCampaignListDto.IsJoin = false;
                customerCampaignListDto.IsFavorite = false;

                int customerCampaignId = 0;
                if (joinList.Any()) 
                {
                    customerCampaignId = joinList.Where(x => x == campaign.Id).FirstOrDefault();
                    if (customerCampaignId > 0)
                    {
                        customerCampaignListDto.Id = customerCampaignId;
                        customerCampaignListDto.IsJoin = true;
                    }
                }

                if (favoriteList.Any()) 
                {
                    customerCampaignId = favoriteList.Where(x => x == campaign.Id).FirstOrDefault();
                    if (customerCampaignId > 0)
                    {
                        customerCampaignListDto.Id = customerCampaignId;
                        customerCampaignListDto.IsFavorite = true;
                    }
                }

                if (campaign.EndDate > today)
                {
                    TimeSpan ts = campaign.EndDate - today;
                    customerCampaignListDto.DueDay = ts.Days + 1;
                }

                if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                {
                    if (campaign.EndDate >= today)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join)
                {
                    if (customerCampaignListDto.IsJoin)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite)
                {
                    if (customerCampaignListDto.IsFavorite)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                {
                    if (DateTime.UtcNow > campaign.EndDate)
                        returnList.Add(customerCampaignListDto);
                }
            }

            response.CustomerCampaignList = returnList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            
            return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response);
        }
        private async Task<IQueryable<CampaignDetailListEntity>> GetCampaignQueryAsync(CustomerCampaignListFilterRequest request) 
        {
            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive);

            if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                campaignQuery = campaignQuery.Where(t => t.EndDate >= today);
            else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                campaignQuery = campaignQuery.Where(t => t.EndDate < today);

            //sort

            if (string.IsNullOrEmpty(request.SortBy))
            {
                campaignQuery = campaignQuery.OrderByDescending(x => x.Id);
                return campaignQuery;
            }

            if (request.SortBy.EndsWith("Str"))
                request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

            bool isDescending = request.SortDir?.ToLower() == "desc";

            if (request.SortBy.Equals("Id") || request.SortBy.Equals("Code")) 
            { 
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Id) : campaignQuery = campaignQuery.OrderBy(x => x.Id);
            }
            else if (request.SortBy.Equals("Order"))
            {
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Order) : campaignQuery = campaignQuery.OrderBy(x => x.Order);
            }

            return campaignQuery;
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewFormAsync(int campaignId, string contentRootPath)
        {
            return await GetCustomerFormAsync(campaignId, null, (int)CustomerFormTypesEnum.View, contentRootPath);
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerJoinFormAsync(int campaignId, string customerCode, string contentRootPath)
        {
            return await GetCustomerFormAsync(campaignId, customerCode, (int)CustomerFormTypesEnum.Join, contentRootPath);
        }
        private async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerFormAsync(int campaignId, string? customerCode, int customerFormType, string contentRootPath)
        {
            CustomerViewFormMinDto response = new CustomerViewFormMinDto();

            //campaign
            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            response.IsInvisibleCampaign = false;

            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }

            response.IsJoin = false;
            if(customerFormType== (int)CustomerFormTypesEnum.Join) 
            {
                if(string.IsNullOrEmpty(customerCode))
                    throw new Exception("Müşteri kodu giriniz.");

                if (StaticValues.IsDevelopment)
                {
                    var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                            .GetAll(x => !x.IsDeleted && x.CustomerCode == customerCode)
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefaultAsync();
                    if (customerJoin != null)
                    {
                        response.IsJoin = customerJoin.IsJoin;
                    }
                }
                else
                {

                }
            }

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);

            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId, true);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode)
        {
            CustomerAchievementFormDto response = new CustomerAchievementFormDto();

            //campaign
            response.CampaignId = campaignId;
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            response.IsInvisibleCampaign = false;
            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }
            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);
            response.Campaign = campaignDto;

            //servisten gelecek bilgiler

            var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);

            decimal? totalAchievement = 0;
            decimal? previousMonthAchievement = 0;

            List<TargetParameterDto> targetSourceList = new List<TargetParameterDto>();

            if (StaticValues.IsDevelopment)
            {
                totalAchievement = 190;
                previousMonthAchievement = 120;

                var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoTestCustomer(campaignId);
                response.CampaignTarget = campaignTargetDto;
            }
            else
            {

            }

            response.TotalAchievement = totalAchievement;
            response.TotalAchievementStr = Helpers.ConvertNullablePriceString(totalAchievement);

            response.PreviousMonthAchievement = previousMonthAchievement;
            response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerAchievementFormDto>.SuccessAsync(response);        }
    }
}
