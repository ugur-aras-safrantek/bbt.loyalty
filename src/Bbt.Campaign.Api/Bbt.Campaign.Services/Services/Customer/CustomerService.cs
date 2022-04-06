using AutoMapper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
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

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService,
            ICampaignService campaignService, ICampaignRuleService campaignRuleService, ICampaignTargetService campaignTargetService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignRuleService = campaignRuleService;
            _campaignTargetService = campaignTargetService;
        }


        //public async Task<BaseResponse<CampaignViewFormDto>> GetCampaignViewFormAsync(int campaignId)
        //{
        //    CampaignViewFormDto response = new CampaignViewFormDto();

        //    response.CampaignId = campaignId;

        //    var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
        //        .GetAll(x => x.Id == campaignId && !x.IsDeleted)
        //        .FirstOrDefaultAsync();
        //    if (campaignEntity == null)
        //    {
        //        if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
        //    }

        //    #region parameters

        //    response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
        //    response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
        //    response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
        //    response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
        //    response.CampaignStartTermList = (await _parameterService.GetCampaignStartTermListAsync())?.Data;
        //    response.BusinessLineList = (await _parameterService.GetBusinessLineListAsync())?.Data;
        //    response.BranchList = (await _parameterService.GetBranchListAsync())?.Data;
        //    response.CustomerTypeList = (await _parameterService.GetCustomerTypeListAsync())?.Data;
        //    response.JoinTypeList = (await _parameterService.GetJoinTypeListAsync())?.Data;
        //    response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
        //    response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
        //    response.ActionOptions = (await _parameterService.GetActionOptionListAsync())?.Data;
        //    response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

        //    #endregion


        //    var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

        //    response.Campaign = campaignDto;

        //    var campaignRule = await _campaignRuleService.GetCampaignRuleDto(campaignId);

        //    response.CampaignRule = campaignRule;

        //    var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId);

        //    response.CampaignTargetList = campaignTargetDto;

        //    //campaign
        //    //var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
        //    //mappedCampaign.StartDate = campaignEntity.StartDate.ToShortDateString().Replace('.', '-');
        //    //mappedCampaign.EndDate = campaignEntity.EndDate.ToShortDateString().Replace('.', '-');
        //    //mappedCampaign.Code = mappedCampaign.Id.ToString();
        //    //response.Campaign = mappedCampaign;

        //    //var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
        //    //    .GetAll(x => x.CampaignId == id && x.IsDeleted != true)
        //    //    .Include(x => x.Branches.Where(t => t.IsDeleted != true))
        //    //    .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
        //    //    .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
        //    //    .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
        //    //    .FirstOrDefaultAsync();
        //    //if (campaignRuleEntity != null)
        //    //{
        //    //    string identityNumber = string.Empty;
        //    //    bool isSingleIdentity = campaignRuleEntity.RuleIdentities.Count() == 1; ;

        //    //    CampaignRuleDto campaignRuleDto = new CampaignRuleDto()
        //    //    {
        //    //        Id = campaignRuleEntity.Id,
        //    //        CampaignId = campaignRuleEntity.CampaignId,
        //    //        JoinTypeId = campaignRuleEntity.JoinTypeId,
        //    //        CampaignStartTermId = campaignRuleEntity.CampaignStartTermId,
        //    //        IdentityNumber = isSingleIdentity ? campaignRuleEntity.RuleIdentities.ElementAt(0).Identities : "",
        //    //        RuleBusinessLines = campaignRuleEntity.BusinessLines.Where(c => !c.IsDeleted).Select(c => c.BusinessLineId).ToList(),
        //    //        RuleCustomerTypes = campaignRuleEntity.CustomerTypes.Where(c => !c.IsDeleted).Select(c => c.CustomerTypeId).ToList(),
        //    //        RuleBranches = campaignRuleEntity.Branches.Where(c => !c.IsDeleted).Select(c => c.BranchCode).ToList(),
        //    //    };
        //    //    response.CampaignRule = campaignRuleDto;
        //    //}

        //    //targets
        //    //List<CampaignTargetDto> campaignTargetDtos =
        //    //    _unitOfWork.GetRepository<CampaignTargetEntity>()
        //    //    .GetAll(x => x.CampaignId == id && x.IsDeleted != true)
        //    //    .Include(x => x.Target)
        //    //    .Select(x => _mapper.Map<CampaignTargetDto>(x))
        //    //    .ToList();

        //    //response.CampaignTargetList = campaignTargetDtos;

        //    //achievements
        //    var achievements = _unitOfWork.GetRepository<CampaignAchievementEntity>()
        //                              .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
        //                              .Include(x => x.ActionOption)
        //                              .Include(x => x.AchievementType)
        //                              .OrderBy(x => x.CampaignChannelCode)
        //                              .ThenBy(x => x.Type).ToList();


        //    var channelsAndAchievementsList = new List<ChannelsAndAchievementsByCampaignDto>();

        //    var channels = (await _parameterService.GetCampaignChannelListAsync()).Data;

        //    foreach (var channel in channels)
        //    {
        //        var _achievements = achievements.Where(x => x.CampaignChannelCode == channel).Select(x => new CampaignAchievementListDto
        //        {
        //            Id = x.Id,
        //            AchievementType = x.AchievementType?.Name,
        //            Action = x.ActionOption?.Name,
        //            Type = Helpers.GetEnumDescription(x.Type),
        //            MaxUtilization = x.MaxUtilization,
        //            //CampaignId = x.CampaignId,
        //            TypeId = (int)x.Type,
        //            CurrencyId = x.CurrencyId,
        //            Amount = x.Amount,
        //            Rate = x.Rate,
        //            MaxAmount = x.MaxAmount,
        //            AchievementTypeId = x.AchievementTypeId,
        //            ActionOptionId = x.ActionOptionId,
        //            DescriptionTr = x.DescriptionTr,
        //            DescriptionEn = x.DescriptionEn,
        //            TitleTr = x.TitleTr,
        //            TitleEn = x.TitleEn,
        //        }).ToList();

        //        channelsAndAchievementsList.Add(new ChannelsAndAchievementsByCampaignDto
        //        {
        //            CampaignChannelCode = channel,
        //            CampaignChannelName = channel,
        //            HasAchievement = _achievements.Any(),
        //            AchievementList = _achievements
        //        });

        //    }

        //    response.ChannelsAndAchievementsList = channelsAndAchievementsList;

        //    return await BaseResponse<CampaignViewFormDto>.SuccessAsync(response);
        //}
    }
}
