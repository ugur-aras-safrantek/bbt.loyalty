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
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using RestSharp;
using Bbt.Campaign.Public.Dtos.CampaignTarget;

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

        public async Task<BaseResponse<CustomerCampaignDto>> SetJoin(SetJoinRequest request) 
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                if(entity.IsJoin)
                    throw new Exception("Müşteri bu kampayaya daha önceki bir tarihte katılmış.");

                entity.IsJoin = request.IsJoin;
                entity.StartDate = request.IsJoin ? DateTime.Now : null;
                entity.LastModifiedBy = request.CustomerCode;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = request.CustomerCode;
                entity.CampaignId = request.CampaignId;
                entity.IsFavorite = false;
                entity.IsJoin = request.IsJoin;
                entity.StartDate = request.IsJoin ? Helpers.ConvertDateTimeToShortDate(DateTime.Now) : null;
                entity.CreatedBy = request.CustomerCode;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        public async Task<BaseResponse<CustomerCampaignDto>> SetFavorite(SetFavoriteRequest request)
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && x.IsDeleted != true)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.IsFavorite = request.IsFavorite;
                entity.LastModifiedBy = request.CustomerCode;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = request.CustomerCode;
                entity.CampaignId = request.CampaignId;
                entity.IsFavorite = request.IsFavorite; 
                entity.IsJoin = false;
                entity.CreatedBy = request.CustomerCode;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        private async Task CheckValidationAsync(string customerCode, int campaignId) 
        {
            if(string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            if(campaignId <= 0)
                throw new Exception("Kampanya giriniz.");

            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignEntity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted && x.IsActive && x.EndDate >= today)
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
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);

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
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
            }).ToList();

            var customerCampaignList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.CustomerCode == (request.CustomerCode ?? string.Empty))
                .ToListAsync();
                
            List<CustomerCampaignMinListDto> returnList = new List<CustomerCampaignMinListDto>();
            foreach (var campaign in campaignList)
            {
                CustomerCampaignMinListDto customerCampaignListDto = new CustomerCampaignMinListDto();

                customerCampaignListDto.CampaignId = campaign.Id;
                customerCampaignListDto.Campaign = campaign;
                customerCampaignListDto.CustomerCode = request.CustomerCode;
                customerCampaignListDto.IsJoin = false;
                customerCampaignListDto.IsFavorite = false;

                var customerCampaign = customerCampaignList.Where(x => x.CampaignId == campaign.Id).FirstOrDefault();
                if (customerCampaign != null)
                {
                    customerCampaignListDto.Id = customerCampaign.Id;
                    customerCampaignListDto.IsJoin = customerCampaign.IsJoin;
                    customerCampaignListDto.IsFavorite = customerCampaign.IsFavorite;
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
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive 
                        && (x.ViewOptionId != (int)ViewOptionsEnum.InvisibleCampaign || x.ViewOptionId == null)
                        && x.StatusId == (int)StatusEnum.Approved);

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

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
            {
                response.IsContract = false;
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
            }

            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId, true);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerJoinFormAsync(int campaignId, string customerCode, string contentRootPath)
        {
            if (string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

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
            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (customerJoin != null)
            {
                response.IsJoin = customerJoin.IsJoin;
            }

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0) 
            {
                response.IsContract = false;
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
            }
                
            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(campaignId, 0, 0);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            //var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            //response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }

        //private async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerFormAsync(int campaignId, string? customerCode, int customerFormType, string contentRootPath)
        //{
        //    CustomerViewFormMinDto response = new CustomerViewFormMinDto();

        //    //campaign
        //    response.CampaignId = campaignId;

        //    var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
        //        .GetAll(x => x.Id == campaignId && !x.IsDeleted)
        //        .FirstOrDefaultAsync();
        //    if (campaignEntity == null)
        //    {
        //        if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
        //    }

        //    response.IsInvisibleCampaign = false;

        //    if (campaignEntity != null)
        //    {
        //        int viewOptionId = campaignEntity.ViewOptionId ?? 0;
        //        response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
        //    }

        //    response.IsJoin = false;
        //    if(customerFormType== (int)CustomerFormTypesEnum.Join) 
        //    {
        //        if(string.IsNullOrEmpty(customerCode))
        //            throw new Exception("Müşteri kodu giriniz.");

        //        if (StaticValues.IsDevelopment)
        //        {
        //            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
        //                    .GetAll(x => !x.IsDeleted && x.CustomerCode == customerCode)
        //                    .OrderByDescending(x => x.Id)
        //                    .FirstOrDefaultAsync();
        //            if (customerJoin != null)
        //            {
        //                response.IsJoin = customerJoin.IsJoin;
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }

        //    var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

        //    //var campaignDto = campaignDtoAll

        //    response.Campaign = campaignDto;

        //    if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
        //        response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);

        //    //target

        //    var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId, true);

        //    response.CampaignTarget = campaignTargetDto;

        //    //achievement
        //    var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

        //    response.CampaignAchievementList = campaignAchievementList;

        //    return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        //}
        //public async Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode)
        //{
        //    CustomerAchievementFormDto response = new CustomerAchievementFormDto();

        //    //campaign
        //    //response.CampaignId = campaignId;
        //    //var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
        //    //    .GetAll(x => x.Id == campaignId && !x.IsDeleted)
        //    //    .FirstOrDefaultAsync();
        //    //if (campaignEntity == null)
        //    //{
        //    //    throw new Exception("Kampanya bulunamadı.");
        //    //}

        //    //response.IsInvisibleCampaign = false;
        //    //if (campaignEntity != null)
        //    //{
        //    //    int viewOptionId = campaignEntity.ViewOptionId ?? 0;
        //    //    response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
        //    //}
        //    //var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);
        //    //response.Campaign = campaignDto;

        //    ////servisten gelecek bilgiler

        //    //var campaignTargetQuery = _unitOfWork.GetRepository<CampaignTargetListEntity>()
        //    //    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);

        //    decimal? totalAchievement = 0;
        //    decimal? previousMonthAchievement = 0;
        //    decimal usedAmount = 0;
        //    int usedNumberOfTransaction = 0;

        //    //List<TargetParameterDto> targetSourceList = new List<TargetParameterDto>();

        //    response.IsAchieved = true;
        //    if (StaticValues.IsDevelopment)
        //    {
        //        totalAchievement = 190;
        //        previousMonthAchievement = 120;
        //        usedAmount = 1000;
        //        usedNumberOfTransaction = 2;
        //        response.UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount);
        //        response.UsedAmountCurrencyCode = "TRY";
        //        response.TotalAchievementStr = Helpers.ConvertNullablePriceString(totalAchievement);
        //        response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);
        //        response.TotalAchievementCurrencyCode = "TRY";
        //        response.PreviousMonthAchievementCurrencyCode = "TRY";


        //        var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(campaignId, usedAmount, usedNumberOfTransaction);
        //        response.CampaignTarget = campaignTargetDto;
        //        foreach(var targetGroup in campaignTargetDto.TargetGroupList) 
        //        { 
        //            foreach(var target in targetGroup.TargetList) 
        //            { 
        //                if(target.Percent < 100) 
        //                {
        //                    response.IsAchieved = false;
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        bool isTest = false;
        //        string isTestStr = await _parameterService.GetServiceConstantValue("IsTest");
        //        if (!string.IsNullOrEmpty(isTestStr))
        //        {
        //            isTest = Convert.ToBoolean(isTestStr);
        //        }
        //        string serviceUrl = string.Empty;
                
        //        #region GoalResultByCustomerIdAndMonthCount

        //        using (var httpClient1 = new HttpClient())
        //        {
        //            serviceUrl = string.Empty;
        //            serviceUrl = await _parameterService.GetServiceConstantValue("GoalResultByCustomerIdAndMonthCount");
        //            serviceUrl = serviceUrl.Replace("{customerId}", customerCode).Replace("{monthCount}", "2");
        //            httpClient1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var restResponse1 = await httpClient1.GetAsync(serviceUrl);
        //            if (restResponse1.IsSuccessStatusCode)
        //            {
        //                if (restResponse1.Content != null)
        //                {
        //                    var apiResponse1 = await restResponse1.Content.ReadAsStringAsync();
        //                    if (!string.IsNullOrEmpty(apiResponse1)) 
        //                    {
        //                        var goalResultByCustomerIdAndMonthCount = JsonConvert.DeserializeObject<GoalResultByCustomerIdAndMonthCount>(apiResponse1);
        //                        if (goalResultByCustomerIdAndMonthCount != null)
        //                        {
        //                            if (goalResultByCustomerIdAndMonthCount.Total != null)
        //                            {
        //                                response.TotalAchievementStr = Helpers.ConvertNullablePriceString(goalResultByCustomerIdAndMonthCount.Total.Amount);
        //                                response.TotalAchievementCurrencyCode = goalResultByCustomerIdAndMonthCount.Total.Currency;
        //                            }
        //                            if (goalResultByCustomerIdAndMonthCount.Months != null && goalResultByCustomerIdAndMonthCount.Months.Any())
        //                            {
        //                                int month = DateTime.Now.Month;
        //                                int year = DateTime.Now.Year;

        //                                if (month == 1)
        //                                {
        //                                    month = 12;
        //                                    year = year - 1;
        //                                }
        //                                else
        //                                {
        //                                    month = month - 1;
        //                                }

        //                                var monthAchievent = goalResultByCustomerIdAndMonthCount.Months
        //                                    .Where(x => x.Year == year && x.Month == month).FirstOrDefault();
        //                                if (monthAchievent != null)
        //                                {
        //                                    response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(monthAchievent.Amount);
        //                                    response.PreviousMonthAchievementCurrencyCode = monthAchievent.Currency;
        //                                }
        //                            }
        //                        }
        //                    } 
        //                }
        //            }
        //            else 
        //            { 
        //                throw new Exception("Müşteri kazanımları servisinden veri çekilemedi."); 
        //            }
        //        }

        //        #endregion

        //        # region GoalResultByCustomerAndCampaing

        //        using (var httpClient2 = new HttpClient()) 
        //        {
        //            if (isTest) 
        //            {
        //                customerCode = "01234567890";
        //                campaignId = 1;
        //            }

        //            serviceUrl = string.Empty;
        //            serviceUrl = await _parameterService.GetServiceConstantValue("GoalResultByCustomerAndCampaing");
        //            serviceUrl = serviceUrl.Replace("{customerId}", customerCode).Replace("{campaignId}", campaignId.ToString());
        //            httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            var restResponse2 = await httpClient2.GetAsync(serviceUrl);
        //            if (restResponse2.IsSuccessStatusCode) 
        //            {
        //                if (restResponse2.Content != null) 
        //                {
        //                    var apiResponse2 = await restResponse2.Content.ReadAsStringAsync();
        //                    if (!string.IsNullOrEmpty(apiResponse2)) 
        //                    {
        //                        var xx = JsonConvert.DeserializeObject<GoalResultByCustomerIdAndMonthCount>(apiResponse2);
        //                    }
        //                }


        //            }
        //            else
        //            {
        //                throw new Exception("Müşteri hedef servisinden veri çekilemedi.");
        //            }
        //        }

        //        #endregion
        //    }

        //    //achievement
        //    var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

        //    response.CampaignAchievementList = campaignAchievementList;

        //    return await BaseResponse<CustomerAchievementFormDto>.SuccessAsync(response);        
        //}

        public async Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode, string? language)
        {
            CustomerAchievementFormDto response = new CustomerAchievementFormDto();

            if (language == null)
                language = "tr";

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

            decimal? totalAchievement = 0;
            decimal? previousMonthAchievement = 0;
            //decimal usedAmount = 0;
            //int usedNumberOfTransaction = 0;


            response.IsAchieved = false;
            string serviceUrl = string.Empty;

            if (StaticValues.IsDevelopment) 
            {
                totalAchievement = 190;
                previousMonthAchievement = 120;
                response.TotalAchievementStr = Helpers.ConvertNullablePriceString(totalAchievement);
                response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);
                response.TotalAchievementCurrencyCode = "TRY";
                response.PreviousMonthAchievementCurrencyCode = "TRY";
            }
            else 
            {
                using (var httpClient = new HttpClient())
                {
                    serviceUrl = string.Empty;
                    serviceUrl = await _parameterService.GetServiceConstantValue("GoalResultByCustomerIdAndMonthCount");
                    serviceUrl = serviceUrl.Replace("{customerId}", customerCode).Replace("{monthCount}", "2");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var restResponse1 = await httpClient.GetAsync(serviceUrl);
                    if (restResponse1.IsSuccessStatusCode)
                    {
                        if (restResponse1.Content != null)
                        {
                            var apiResponse1 = await restResponse1.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(apiResponse1))
                            {
                                var goalResultByCustomerIdAndMonthCount = JsonConvert.DeserializeObject<GoalResultByCustomerIdAndMonthCount>(apiResponse1);
                                if (goalResultByCustomerIdAndMonthCount != null)
                                {
                                    if (goalResultByCustomerIdAndMonthCount.Total != null)
                                    {
                                        response.TotalAchievementStr = Helpers.ConvertNullablePriceString(goalResultByCustomerIdAndMonthCount.Total.Amount);
                                        response.TotalAchievementCurrencyCode = goalResultByCustomerIdAndMonthCount.Total.Currency;
                                    }
                                    if (goalResultByCustomerIdAndMonthCount.Months != null && goalResultByCustomerIdAndMonthCount.Months.Any())
                                    {
                                        int month = DateTime.Now.Month;
                                        int year = DateTime.Now.Year;

                                        if (month == 1)
                                        {
                                            month = 12;
                                            year = year - 1;
                                        }
                                        else
                                        {
                                            month = month - 1;
                                        }

                                        var monthAchievent = goalResultByCustomerIdAndMonthCount.Months
                                            .Where(x => x.Year == year && x.Month == month).FirstOrDefault();
                                        if (monthAchievent != null)
                                        {
                                            response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(monthAchievent.Amount);
                                            response.PreviousMonthAchievementCurrencyCode = monthAchievent.Currency;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Müşteri kazanımları servisinden veri çekilemedi.");
                    }
                }
            }

            response.CampaignTarget =  await _campaignTargetService.GetCampaignTargetDtoCustomer2(campaignId, customerCode, language);
            //response.CampaignTarget.ProgressBarlist = campaignTargetDto2.ProgressBarlist;
            //response.CampaignTarget.Informationlist = campaignTargetDto2.Informationlist;

            response.IsAchieved = response.CampaignTarget.IsAchieved;

            //if (StaticValues.IsDevelopment)
            //{
            //    usedAmount = 1000;
            //    usedNumberOfTransaction = 2;
            //    response.UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount);
            //    response.UsedAmountCurrencyCode = "TRY";
            //}
            //else
            //{
            //    if (response.CampaignTarget.ProgressBarlist.Any()) 
            //    {
            //        response.UsedAmountStr = response.CampaignTarget.ProgressBarlist[0].UsedAmountStr;
            //        response.UsedAmountCurrencyCode = response.CampaignTarget.ProgressBarlist[0].UsedAmountCurrencyCode;
            //    }
            //}

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCustomerAchievementsAsync(campaignId, customerCode, language);
            foreach (var campaignAchievement in campaignAchievementList)
                campaignAchievement.IsAchieved = response.IsAchieved;
            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerAchievementFormDto>.SuccessAsync(response);
        }


        public async Task<BaseResponse<CustomerJoinSuccessFormDto>> GetCustomerJoinSuccessFormAsync(int campaignId, string customerCode) 
        {
            CustomerJoinSuccessFormDto response = new CustomerJoinSuccessFormDto();

            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (customerJoin == null)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            if (!customerJoin.IsJoin)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted);
            campaignQuery = campaignQuery.Take(1);

            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
            }).ToList();

            if(!campaignList.Any())
                throw new Exception("Kampanya bulunamadı.");

            response.Campaign = campaignList[0];

            return await BaseResponse<CustomerJoinSuccessFormDto>.SuccessAsync(response);
        }
    }
}
