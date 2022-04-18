using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;
using System.Collections;

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
            await CheckValidationAsync(campaignId);

            CustomerCampaignDto response = new CustomerCampaignDto();

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == customerCode && x.CampaignId == campaignId && x.IsDeleted != true)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.IsJoin = isJoin;
                entity.StartDate = isJoin ? DateTime.Parse(DateTime.Now.ToShortDateString()) : null;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = customerCode;
                entity.CampaignId = campaignId;
                entity.IsFavorite = false;
                entity.IsJoin = isJoin;
                entity.StartDate = isJoin ? DateTime.Parse(DateTime.Now.ToShortDateString()) : null;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }
            

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        public async Task<BaseResponse<CustomerCampaignDto>> SetFavorite(string customerCode, int campaignId, bool isFavorite)
        {
            await CheckValidationAsync(campaignId);

            CustomerCampaignDto response = new CustomerCampaignDto();

            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity> ()
               .GetAll(x => x.CustomerCode == customerCode && x.CampaignId == campaignId && x.IsDeleted != true)
               .FirstOrDefaultAsync();
            if(entity != null) 
            { 
                entity.IsFavorite = isFavorite;
                
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().UpdateAsync(entity);
            }
            else 
            {
                entity = new CustomerCampaignEntity();
                entity.CustomerCode = customerCode;
                entity.CampaignId = campaignId; 
                entity.IsFavorite = isFavorite;
                entity.IsJoin = false;

                entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(entity);
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerCampaignAsync(entity.Id);
        }
        private async Task CheckValidationAsync(int campaignId) 
        {
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
            CustomerCampaignListFilterResponse response = new CustomerCampaignListFilterResponse();
            
            List<CustomerCampaignListDto> customerCampaignList = await this.GetFilteredCampaignList(request);
            var totalItems = customerCampaignList.Count();
            if (totalItems == 0)
                return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");
            
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            customerCampaignList = customerCampaignList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            response.CustomerCampaignList = customerCampaignList;
            response.Paging = Core.Helper.Helpers.Paging(totalItems, pageNumber, pageSize);
            
            if (customerCampaignList.Any()) Dispose(customerCampaignList);
            
            return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response);
        }
        private async Task<List<CustomerCampaignListDto>> GetFilteredCampaignList(CustomerCampaignListFilterRequest request) 
        {
            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());

            List<CustomerCampaignListDto> returnList = new List<CustomerCampaignListDto>();

            var campaignList = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive)
                .ToListAsync();
            if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                campaignList = campaignList.Where(t => t.EndDate < today).ToList();

            var customerCampaignList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.CustomerCode == request.CustomerCode)
                .ToListAsync();

            foreach (var campaign in campaignList)
            {
                CustomerCampaignListDto customerCampaignListDto = new CustomerCampaignListDto();
                
                customerCampaignListDto.CampaignId = campaign.Id;                
                CampaignMinDto campaignMinDto = new CampaignMinDto();
                campaignMinDto.Id = campaign.Id;
                campaignMinDto.TitleTr = campaign.TitleTr;
                campaignMinDto.TitleEn = campaign.TitleEn;
                customerCampaignListDto.Campaign = campaignMinDto;

                customerCampaignListDto.CustomerCode = request.CustomerCode;
                customerCampaignListDto.IsJoin = false;
                customerCampaignListDto.IsFavorite = false;

                if (customerCampaignList.Any()) 
                {
                    var customerCampaign = customerCampaignList.Where(x => x.CampaignId == campaign.Id).FirstOrDefault();
                    if (customerCampaign != null)
                    {
                        customerCampaignListDto.Id = customerCampaign.Id;
                        customerCampaignListDto.IsJoin = customerCampaign.IsJoin;
                        customerCampaignListDto.IsFavorite = customerCampaign.IsFavorite;
                    }
                }

                if(campaign.EndDate > today) 
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
                    if(customerCampaignListDto.IsJoin)
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
            
            return returnList;
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewMinFormAsync(int campaignId, string contentRootPath)
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

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);

            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignVisibleTargetDto(campaignId);

            response.CampaignTarget = campaignTargetDto;


            //achievement
            var campaignAchievement = await _campaignAchievementService.GetCampaignAchievementDto(campaignId);

            response.CampaignAchievement = campaignAchievement;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }
        public static void Dispose(IEnumerable collection)
        {
            foreach (var obj in collection.OfType<IDisposable>())
            {
                obj.Dispose();
            }
        }
    }
}
