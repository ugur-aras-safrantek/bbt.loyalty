using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewMinFormAsync(int campaignId) 
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

            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignVisibleTargetDto(campaignId);

            response.CampaignTarget = campaignTargetDto;


            //achievement
            var campaignAchievement = await _campaignAchievementService.GetCampaignAchievementDto(campaignId);

            response.CampaignAchievement = campaignAchievement;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);

        }
    }
}
