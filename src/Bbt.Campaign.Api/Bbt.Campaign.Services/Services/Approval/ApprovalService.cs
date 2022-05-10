using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.Approval
{
    public class ApprovalService : IApprovalService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRuleService _campaignRuleService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly IAuthorizationservice _authorizationservice;

        public ApprovalService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            ICampaignService campaignService, 
            ICampaignRuleService campaignRuleService, 
            ICampaignTargetService campaignTargetService,
            IAuthorizationservice authorizationservice)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignRuleService = campaignRuleService;
            _campaignTargetService = campaignTargetService;
            _authorizationservice = authorizationservice;
        }

        #region campaign
        public async Task<BaseResponse<CampaignDto>> ApproveCampaignAsync(int id)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => (x.RefId ?? 0) == id && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                return await ApproveCampaignAddAsync(id);
            }
            else 
            {
                return await ApproveCampaignUpdateAsync(id, campaignEntity.Id);
            }
        }
      
        private async Task<BaseResponse<CampaignDto>> ApproveCampaignAddAsync(int refId)
        {
            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();
            
            if(campaignDraftEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            //campaign Draft
            campaignDraftEntity.IsApproved = true;
            campaignDraftEntity.RefId = campaignDraftEntity.Id;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignDraftEntity);

            //campaign
            var campaignDto = _mapper.Map<CampaignDto>(campaignDraftEntity);
            var campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);
            campaignEntity.Id = 0;
            campaignEntity.Code = string.Empty;
            campaignEntity.IsApproved = true;
            campaignEntity.IsDraft = false;
            campaignEntity.RefId = refId;

            //campaign detail
            var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
            var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
            campaignDetailEntity.Id = 0;

            campaignEntity.CampaignDetail = campaignDetailEntity;

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            await AddCampaignDocument(refId, campaignEntity);

            await AddCampaignRule(refId, campaignEntity);

            await AddCampaignTarget(refId, campaignEntity);

            await AddCampaignAchievement(refId, campaignEntity);

            await _unitOfWork.SaveChangesAsync();

            campaignEntity.Code = campaignEntity.Id.ToString();

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }

        public async Task<BaseResponse<CampaignDto>> ApproveCampaignUpdateAsync(int refId, int campaignId) 
        {
            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
                .Include(x => x.CampaignDetail)
                .Where(x => x.Id == refId)
                .FirstOrDefaultAsync();
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (campaignDraftEntity == null || campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }

            //campaign Draft
            campaignDraftEntity.IsApproved = true;
            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignDraftEntity);

            //campaign
            campaignEntity.ContractId = campaignDraftEntity.ContractId;
            campaignEntity.ProgramTypeId = campaignDraftEntity.ProgramTypeId;
            campaignEntity.SectorId = campaignDraftEntity.SectorId;
            campaignEntity.ViewOptionId = campaignDraftEntity.ViewOptionId;
            campaignEntity.IsBundle = campaignDraftEntity.IsBundle;
            campaignEntity.IsContract = campaignDraftEntity.IsContract;
            campaignEntity.DescriptionTr = campaignDraftEntity.DescriptionTr;
            campaignEntity.DescriptionEn = campaignDraftEntity.DescriptionEn;
            campaignEntity.EndDate = campaignDraftEntity.EndDate;
            campaignEntity.StartDate = campaignDraftEntity.StartDate;
            campaignEntity.Name = campaignDraftEntity.Name;
            campaignEntity.Order = campaignDraftEntity.Order;
            campaignEntity.TitleTr = campaignDraftEntity.TitleTr;
            campaignEntity.TitleEn = campaignDraftEntity.TitleEn;
            campaignEntity.MaxNumberOfUser = campaignDraftEntity.MaxNumberOfUser;
            campaignEntity.IsDraft = false;
            campaignEntity.IsApproved = true;
            campaignEntity.RefId = refId;

            //campaign detail
            var campaignDraftDetailEntity = campaignDraftEntity.CampaignDetail;
            var campaignDetailEntity = campaignEntity.CampaignDetail;

            campaignDetailEntity.DetailEn = campaignDraftDetailEntity.DetailEn;
            campaignDetailEntity.DetailTr = campaignDraftDetailEntity.DetailTr;
            campaignDetailEntity.SummaryEn = campaignDraftDetailEntity.SummaryEn;
            campaignDetailEntity.SummaryTr = campaignDraftDetailEntity.SummaryTr;
            campaignDetailEntity.CampaignDetailImageUrl = campaignDraftDetailEntity.CampaignDetailImageUrl;
            campaignDetailEntity.CampaignListImageUrl = campaignDraftDetailEntity.CampaignListImageUrl;
            campaignDetailEntity.ContentTr = campaignDraftDetailEntity.ContentTr;
            campaignDetailEntity.ContentEn = campaignDraftDetailEntity.ContentEn;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            /**/

            //campaign rule
            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == refId && !x.IsDeleted)
                .Include(x => x.Branches.Where(x => !x.IsDeleted))
                .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .Include(x => x.Branches.Where(x => !x.IsDeleted))
                .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            int campaignRuleId = campaignRuleEntity.Id;

            campaignRuleEntity.CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId;
            campaignRuleEntity.JoinTypeId = campaignRuleDraftEntity.JoinTypeId;

            await _unitOfWork.GetRepository<CampaignRuleEntity>().UpdateAsync(campaignRuleEntity);

            //campaign rule branches
            foreach (var branchDeleteEntity in campaignRuleEntity.Branches)
            {
                await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().DeleteAsync(branchDeleteEntity);
            }
            if (campaignRuleDraftEntity.Branches.Any())
            {
                foreach (var branch in campaignRuleDraftEntity.Branches)
                {
                    var campaignRuleBranchEntity = new CampaignRuleBranchEntity()
                    {
                        CampaignRuleId = campaignRuleEntity.Id,
                        BranchCode = branch.BranchCode,
                        BranchName = ""
                    };
                    await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
                }
            }

            //campaign rule customerTypes
            foreach (var customerTypeDeleteEntity in campaignRuleEntity.CustomerTypes)
            {
                await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().DeleteAsync(customerTypeDeleteEntity);
            }
            if (campaignRuleDraftEntity.CustomerTypes.Any() )
            {
                foreach (var customerType in campaignRuleDraftEntity.CustomerTypes)
                {
                    var campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity()
                    {
                        CampaignRuleId = campaignRuleEntity.Id,
                        CustomerTypeId = customerType.CustomerTypeId,
                    };
                    await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
                }
            }

            //BusinessLines
            foreach (var businessLineDeleteEntity in campaignRuleEntity.BusinessLines.Where(t => !t.IsDeleted))
            {
                await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().DeleteAsync(businessLineDeleteEntity);
            }
            if (campaignRuleDraftEntity.BusinessLines.Any()) 
            {
                foreach (var businessLine in campaignRuleDraftEntity.BusinessLines)
                {
                    var campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity()
                    {
                        CampaignRuleId = campaignRuleEntity.Id,
                        BusinessLineId = businessLine.BusinessLineId,
                    };
                    await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
                }
            }

            //rule document-identities
            var ruleDraftDocumentEntity = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                .GetAll(x => x.CampaignId == refId
                            && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                            && !x.IsDeleted)
                .FirstOrDefaultAsync();
            var campaignDocumentEntity = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                .GetAll(x => x.CampaignId == campaignId
                            && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                            && !x.IsDeleted)
                .FirstOrDefaultAsync();
            /*
            bool isSingleIdentity = campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer &&
                campaignRuleDraftEntity.RuleIdentities != null &&
                !campaignRuleDraftEntity.RuleIdentities.IsDeleted &&
                !string.IsNullOrEmpty(campaignRuleDraftEntity.RuleIdentities.Identities);
            if(campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                if (campaignRuleEntity.RuleIdentities != null)
                {
                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().DeleteAsync(campaignRuleEntity.RuleIdentities);
                }
                if (campaignDocumentEntity != null)
                {
                    await _unitOfWork.GetRepository<CampaignDocumentEntity>().DeleteAsync(campaignDocumentEntity);
                }

                if (isSingleIdentity)
                {
                    var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity();
                    campaignRuleIdentityEntity.Identities = campaignRuleDraftEntity.RuleIdentities?.Identities ?? "";
                    campaignRuleEntity.RuleIdentities = campaignRuleIdentityEntity;
                }
                else
                {
                    var ruleDraftDocument = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                        .GetAll(x => x.CampaignId == campaignDraftEntity.Id
                            && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                            && !x.IsDeleted)
                        .FirstOrDefaultAsync();
                    if (ruleDraftDocument != null)
                    {
                        var campaignDocumentEntityNew = new CampaignDocumentEntity();
                        campaignDocumentEntityNew.Campaign = campaignEntity;
                        campaignDocumentEntityNew.DocumentType = ruleDraftDocument.DocumentType;
                        campaignDocumentEntityNew.MimeType = ruleDraftDocument.MimeType;
                        campaignDocumentEntityNew.Content = ruleDraftDocument.Content;
                        campaignDocumentEntityNew.DocumentName = ruleDraftDocument.DocumentName;
                        await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocumentEntityNew);
                    }
                }
            }
            */

            //target
            List<CampaignTargetEntity> targetDraftList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                 .GetAll(x => !x.IsDeleted && x.CampaignId == refId)
                 .ToList();
            List<CampaignTargetEntity> targetList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                 .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
                 .ToList();
            foreach(var targetDeleteEntity in targetList) 
            {
                await _unitOfWork.GetRepository<CampaignTargetEntity>().DeleteAsync(targetDeleteEntity);
            }
            foreach (var targetDraftEntity in targetDraftList)
            {
                var campaignTargetEntity = new CampaignTargetEntity();
                campaignTargetEntity.CampaignId = campaignId;
                campaignTargetEntity.TargetGroupId = targetDraftEntity.TargetGroupId;
                campaignTargetEntity.TargetOperationId = targetDraftEntity.TargetOperationId;
                campaignTargetEntity.TargetId = targetDraftEntity.TargetId;
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetEntity);
            }

            //achievement
            foreach(var achievementEntityDeleteEntity in _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted).ToList()) 
            {
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().DeleteAsync(achievementEntityDeleteEntity);
            }
            //achievement
            foreach (var achievementDraftEntity in _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == refId && !x.IsDeleted).ToList())
            {
                var campaignAchievementEntity = new CampaignAchievementEntity();

                //campaignAchievementEntity.CampaignChannelCode = achievementDraftEntity.CampaignChannelCode;
                campaignAchievementEntity.CampaignId = campaignId;
                campaignAchievementEntity.CurrencyId = achievementDraftEntity.CurrencyId;
                campaignAchievementEntity.Amount = achievementDraftEntity.Amount;
                campaignAchievementEntity.Rate = achievementDraftEntity.Rate;
                campaignAchievementEntity.MaxAmount = achievementDraftEntity.MaxAmount;
                campaignAchievementEntity.MaxUtilization = achievementDraftEntity.MaxUtilization;
                campaignAchievementEntity.AchievementTypeId = achievementDraftEntity.AchievementTypeId;
                campaignAchievementEntity.ActionOptionId = achievementDraftEntity.ActionOptionId;
                campaignAchievementEntity.DescriptionTr = achievementDraftEntity.DescriptionTr;
                campaignAchievementEntity.DescriptionEn = achievementDraftEntity.DescriptionEn;
                campaignAchievementEntity.TitleTr = achievementDraftEntity.TitleTr;
                campaignAchievementEntity.TitleEn = achievementDraftEntity.TitleEn; 

                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievementEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);

            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }

        public async Task<BaseResponse<CampaignApproveFormDto>> GetCampaignApprovalFormAsync(int refId) 
        {
            CampaignApproveFormDto response = new CampaignApproveFormDto();

            //var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
            //    .GetAllIncluding(x => x.CampaignDetail)
            //    .Where(x => x.Id == refId)
            //    .FirstOrDefaultAsync();

            //var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
            //    .GetAllIncluding(x => x.CampaignDetail)
            //    .Where(x => x.RefId == refId)
            //    .FirstOrDefaultAsync();
            //if(campaignEntity == null) 
            //{ 
            //    response.isNewRecord = true;
            //}

            //#region parameters

            //response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
            //response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
            //response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
            //response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
            //response.CampaignStartTermList = (await _parameterService.GetCampaignStartTermListAsync())?.Data;
            //response.BusinessLineList = (await _parameterService.GetBusinessLineListAsync())?.Data;
            //response.BranchList = (await _parameterService.GetBranchListAsync())?.Data;
            //response.CustomerTypeList = (await _parameterService.GetCustomerTypeListAsync())?.Data;
            //response.JoinTypeList = (await _parameterService.GetJoinTypeListAsync())?.Data;
            //response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            //response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
            //response.ActionOptions = (await _parameterService.GetActionOptionListAsync())?.Data;
            //response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

            //#endregion

            //#region draft

            //if (campaignDraftEntity != null)
            //{
            //    var mappedDraftCampaign = _mapper.Map<CampaignDto>(campaignDraftEntity);
            //    mappedDraftCampaign.Code = mappedDraftCampaign.Id.ToString();
            //    response.CampaignDraft = mappedDraftCampaign;
            //}

            //var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
            //   .GetAll(x => x.CampaignId == refId && x.IsDeleted != true)
            //   .Include(x => x.Branches.Where(t => t.IsDeleted != true))
            //   .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
            //   .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
            //   .FirstOrDefaultAsync();
            //if (campaignRuleDraftEntity != null)
            //{
            //    string identityNumber = string.Empty;
            //    bool isSingleIdentity = false;
            //    if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            //    {
            //        var campaignRuleIdentityEntity = _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().
            //            GetAll(x => x.CampaignRuleId == campaignRuleDraftEntity.Id && !x.IsDeleted).FirstOrDefault();
            //        identityNumber = campaignRuleDraftEntity == null ? string.Empty : campaignRuleIdentityEntity.Identities;
            //    }
            //    CampaignRuleDto campaignRuleDraftDto = new CampaignRuleDto()
            //    {
            //        Id = campaignRuleDraftEntity.Id,
            //        CampaignId = campaignRuleDraftEntity.CampaignId,
            //        JoinTypeId = campaignRuleDraftEntity.JoinTypeId,
            //        CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId,
            //        IdentityNumber = identityNumber,
            //        //BusinessLines = campaignRuleDraftEntity.BusinessLines.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //        //{
            //        //    Id = c.BusinessLineId,
            //        //    Name = string.Empty,
            //        //}).ToList(),
            //        //CustomerTypes = campaignRuleDraftEntity.CustomerTypes.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //        //{
            //        //    Id = c.CustomerTypeId,
            //        //    Name = string.Empty,
            //        //}).ToList(),
            //        //Branches = campaignRuleDraftEntity.Branches.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //        //{
            //        //    Id = c.Id,
            //        //    Code = c.BranchCode,
            //        //    Name = c.BranchName,
            //        //}).ToList(),

            //    };
            //    response.CampaignRuleDraft = campaignRuleDraftDto;
            //}

            //List<CampaignTargetDto> campaignTargetDraftDtos =
            //    _unitOfWork.GetRepository<CampaignTargetEntity>()
            //    .GetAll(x => x.CampaignId == refId && x.IsDeleted != true)
            //    .Include(x => x.Target)
            //    .Select(x => _mapper.Map<CampaignTargetDto>(x))
            //    .ToList();

            //response.CampaignTargetListDraft = campaignTargetDraftDtos;


            ////achievements
            //ChannelsAndAchievementsByCampaignResponse achievementsDraftResponse = new ChannelsAndAchievementsByCampaignResponse();

            //var achievementsDraft = _unitOfWork.GetRepository<CampaignAchievementEntity>()
            //                          .GetAll(x => !x.IsDeleted && x.CampaignId == refId)
            //                          .Include(x => x.ActionOption)
            //                          .Include(x => x.AchievementType)
            //                          .OrderBy(x => x.CampaignChannelCode)
            //                          .ThenBy(x => x.Type).ToList();

            //var channels = (await _parameterService.GetCampaignChannelListAsync()).Data;

            //foreach (var channel in channels)
            //{
            //    var _achievementsDraft = achievementsDraft.Where(x => x.CampaignChannelCode == channel).Select(x => new CampaignAchievementListDto
            //    {
            //        Id = x.Id,
            //        AchievementType = x.AchievementType?.Name,
            //        Action = x.ActionOption?.Name,
            //        Type = Helpers.GetEnumDescription(x.Type),
            //        MaxUtilization = x.MaxUtilization
            //    }).ToList();

            //    //achievementsDraftResponse.ChannelsAndAchievements.Add(new ChannelsAndAchievementsByCampaignDto
            //    //{
            //    //    CampaignChannelCode = channel,
            //    //    CampaignChannelName = channel,
            //    //    HasAchievement = _achievementsDraft.Any(),
            //    //    AchievementList = _achievementsDraft
            //    //});

            //}

            //response.AchievementsListDraft = achievementsDraftResponse;

            //#endregion

            //#region campaign

            //if (!response.isNewRecord) 
            //{
            //    //campaign
            //    var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            //    mappedCampaign.Code = mappedCampaign.Id.ToString();
            //    response.CampaignDraft = mappedCampaign;

            //    //campaign rule
            //    var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
            //   .GetAll(x => x.CampaignId == campaignEntity.Id && x.IsDeleted != true)
            //   .Include(x => x.Branches.Where(t => t.IsDeleted != true))
            //   .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
            //   .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
            //   .FirstOrDefaultAsync();
            //    if (campaignRuleEntity != null)
            //    {
            //        string identityNumber = string.Empty;
            //        bool isSingleIdentity = false;
            //        if (campaignRuleEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            //        {
            //            var campaignRuleIdentityEntity = _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().
            //                GetAll(x => x.CampaignRuleId == campaignRuleEntity.Id && !x.IsDeleted).FirstOrDefault();
            //            identityNumber = campaignRuleEntity == null ? string.Empty : campaignRuleIdentityEntity.Identities;
            //        }
            //        CampaignRuleDto campaignRuleDto = new CampaignRuleDto()
            //        {
            //            Id = campaignRuleEntity.Id,
            //            CampaignId = campaignRuleEntity.CampaignId,
            //            JoinTypeId = campaignRuleEntity.JoinTypeId,
            //            CampaignStartTermId = campaignRuleEntity.CampaignStartTermId,
            //            IdentityNumber = identityNumber,
            //            //BusinessLines = campaignRuleEntity.BusinessLines.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //            //{
            //            //    Id = c.BusinessLineId,
            //            //    Name = string.Empty,
            //            //}).ToList(),
            //            //CustomerTypes = campaignRuleEntity.CustomerTypes.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //            //{
            //            //    Id = c.CustomerTypeId,
            //            //    Name = string.Empty,
            //            //}).ToList(),
            //            //Branches = campaignRuleEntity.Branches.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
            //            //{
            //            //    Id = c.Id,
            //            //    Code = c.BranchCode,
            //            //    Name = c.BranchName,
            //            //}).ToList(),
            //        };
            //        response.CampaignRule = campaignRuleDto;
            //    }

            //    List<CampaignTargetDto> campaignTargetDtos =
            //        _unitOfWork.GetRepository<CampaignTargetEntity>()
            //        .GetAll(x => x.CampaignId == campaignEntity.Id && x.IsDeleted != true)
            //        .Include(x => x.Target)
            //        .Select(x => _mapper.Map<CampaignTargetDto>(x))
            //        .ToList();

            //    response.CampaignTargetListDraft = campaignTargetDtos;

            //    //achievements
            //    ChannelsAndAchievementsByCampaignResponse achievementsResponse = new ChannelsAndAchievementsByCampaignResponse();

            //    var achievements = _unitOfWork.GetRepository<CampaignAchievementEntity>()
            //                              .GetAll(x => !x.IsDeleted && x.CampaignId == campaignEntity.Id)
            //                              .Include(x => x.ActionOption)
            //                              .Include(x => x.AchievementType)
            //                              //.OrderBy(x => x.CampaignChannelCode)
            //                              //.ThenBy(x => x.Type).
            //                              .ToList();

            //    foreach (var channel in channels)
            //    {
            //        var _achievements = achievements.Where(x => x.CampaignChannelCode == channel).
            //            Select(x => new CampaignAchievementListDto
            //        {
            //            Id = x.Id,
            //            AchievementType = x.AchievementType?.Name,
            //            Action = x.ActionOption?.Name,
            //            Type = Helpers.GetEnumDescription(x.Type),
            //            MaxUtilization = x.MaxUtilization
            //        }).ToList();

            //        //achievementsResponse.ChannelsAndAchievements.Add(new ChannelsAndAchievementsByCampaignDto
            //        //{
            //        //    CampaignChannelCode = channel,
            //        //    CampaignChannelName = channel,
            //        //    HasAchievement = _achievements.Any(),
            //        //    AchievementList = _achievements
            //        //});

            //    }
            //    response.AchievementsList = achievementsResponse;
            //}

            //#endregion

            return await BaseResponse<CampaignApproveFormDto>.SuccessAsync(response);
        }

        private async Task<SuccessDto> AddCampaignRule(int refId, CampaignEntity campaignEntity)
        {
            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == refId && x.IsDeleted != true)
                .Include(x => x.Branches.Where(t => t.IsDeleted != true))
                .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
                .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
                .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();
            if(campaignRuleDraftEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı."); 


            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity()
            {
                Campaign = campaignEntity,
                CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId,
                JoinTypeId = campaignRuleDraftEntity.JoinTypeId
            };

            if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
            {
                if (campaignRuleDraftEntity.Branches.Any())
                {
                    campaignRuleEntity.Branches = new List<CampaignRuleBranchEntity>();
                    foreach (var branch in campaignRuleDraftEntity.Branches)
                    {
                        campaignRuleEntity.Branches.Add(new CampaignRuleBranchEntity()
                        {
                            BranchCode = branch.BranchCode,
                            BranchName = branch.BranchName
                        });
                    }
                }
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                if (campaignRuleDraftEntity.CustomerTypes.Any())
                {
                    campaignRuleEntity.CustomerTypes = new List<CampaignRuleCustomerTypeEntity>();
                    foreach (var customerType in campaignRuleDraftEntity.CustomerTypes)
                    {
                        campaignRuleEntity.CustomerTypes.Add(new CampaignRuleCustomerTypeEntity()
                        {
                            CustomerTypeId = customerType.CustomerTypeId
                        });
                    }
                }
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            {
                if (campaignRuleDraftEntity.BusinessLines.Any())
                {
                    campaignRuleEntity.BusinessLines = new List<CampaignRuleBusinessLineEntity>();
                    foreach (var businessLine in campaignRuleDraftEntity.BusinessLines)
                    {
                        campaignRuleEntity.BusinessLines.Add(new CampaignRuleBusinessLineEntity()
                        {
                            BusinessLineId = businessLine.BusinessLineId
                        });
                    }
                }
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                campaignRuleEntity.RuleIdentities = new List<CampaignRuleIdentityEntity>();
                foreach (var ruleIdentity in campaignRuleDraftEntity.RuleIdentities)
                {
                    campaignRuleEntity.RuleIdentities.Add(new CampaignRuleIdentityEntity()
                    {
                        Identities = ruleIdentity.Identities,
                    });
                }
            }

            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            return new SuccessDto() { IsSuccess = true };
        }

        private async Task<SuccessDto> AddCampaignTarget(int refId, CampaignEntity campaignEntity) 
        {
            List<CampaignTargetEntity> campaignTargetDraftList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                     .GetAll(x => !x.IsDeleted && x.CampaignId == refId)
                     .ToList();
            if(campaignTargetDraftList.Count == 0)
                throw new Exception("Kampanya hedefleri bulunamadı."); 

            foreach (var campaignTargetDraftEntity in campaignTargetDraftList)
            {
                var campaignTargetDto = _mapper.Map<CampaignTargetDto>(campaignTargetDraftEntity);
                var campaignTargetEntity = _mapper.Map<CampaignTargetEntity>(campaignTargetDto);
                campaignTargetEntity.Id = 0;
                campaignTargetEntity.Campaign = campaignEntity;
                campaignTargetEntity.TargetId = campaignTargetDraftEntity.TargetId;
                campaignTargetEntity.TargetGroupId = campaignTargetDraftEntity.TargetGroupId;
                campaignTargetEntity.TargetOperationId = campaignTargetDraftEntity.TargetOperationId;
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetEntity);
            }

            return new SuccessDto() { IsSuccess = true };
        }

        private async Task<SuccessDto> AddCampaignDocument(int refId, CampaignEntity campaignEntity) 
        {
            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>()
                .GetAll(x => x.CampaignId == refId && !x.IsDeleted);
            foreach(var x in documents) 
            {
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                {
                    Campaign = campaignEntity,
                    Content = x.Content,
                    DocumentName = x.DocumentName,
                    DocumentType = x.DocumentType,
                    MimeType = x.MimeType
                });
            }

            return new SuccessDto() { IsSuccess = true };
        }

        private async Task<SuccessDto> AddCampaignChannelCode(int refId, CampaignEntity campaignEntity) 
        {
            var campaignChannelCodes = _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
                .GetAll(x => x.CampaignId == refId && !x.IsDeleted);
            if(!campaignChannelCodes.Any())
                throw new Exception("Kampanya kanal kodu bulunamadı.");
            foreach (var x in campaignChannelCodes)
            {
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(new CampaignChannelCodeEntity()
                {
                    Campaign = campaignEntity,
                    ChannelCode = x.ChannelCode,
                });
            }

            return new SuccessDto() { IsSuccess = true };
        }

    

        private async Task<SuccessDto> AddCampaignAchievement(int refId, CampaignEntity campaignEntity) 
        {
            var achievementDraftList = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == refId && x.IsDeleted != true)
                .ToListAsync();
            if(!achievementDraftList.Any())
                throw new Exception("Kampanya kazanımları bulunamadı.");

            foreach(var achievementDraftEntity in achievementDraftList) 
            {
                var campaignAchievementDto = _mapper.Map<CampaignAchievementDto>(achievementDraftEntity);
                var campaignAchievementEntity = _mapper.Map<CampaignAchievementEntity>(campaignAchievementDto);
                campaignAchievementEntity.Id = 0;
                campaignAchievementEntity.Campaign = campaignEntity;

                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievementEntity);

            }
            return new SuccessDto() { IsSuccess = true };
        }

        #endregion

        #region toplimit

        //public async Task<BaseResponse<TopLimitEntity>> ApproveTopLimitAsync(int id)
        //{
        //    var entity = await _unitOfWork.GetRepository<TopLimitEntity>()
        //        .GetAll(x => (x.RefId ?? 0) == id && !x.IsDeleted)
        //        .FirstOrDefaultAsync();
        //    return entity == null ? await ApproveTopLimitAddAsync(id) : 
        //                                    await ApproveTopLimitUpdateAsync(entity.RefId ?? 0, entity.Id);
        //}
        //private async Task<BaseResponse<TopLimitEntity>> ApproveTopLimitAddAsync(int refId)
        //{
        //    var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
        //        .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
        //        .Include(x => x.TopLimitCampaigns.Where(x => !x.IsDeleted))
        //        .FirstOrDefaultAsync();
        //    if (draftEntity == null) { throw new Exception("Çatı limit bulunamadı."); }

        //    draftEntity.IsApproved = true;
        //    await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(draftEntity);

        //    var campaignTopLimitDto = _mapper.Map<TopLimitEntity>(draftEntity);
        //    var entity = _mapper.Map<TopLimitEntity>(campaignTopLimitDto);
        //    entity.Id = 0;
        //    entity.IsApproved = true;
        //    entity.IsDraft = false;
        //    entity.RefId = refId;

        //    //Campaigns
        //    if (draftEntity.TopLimitCampaigns.Any())
        //    {
        //        entity.TopLimitCampaigns = new List<CampaignTopLimitEntity>();
        //        foreach (var campaignTopLimitDraft in draftEntity.TopLimitCampaigns)
        //        {
        //            entity.TopLimitCampaigns.Add(new CampaignTopLimitEntity()
        //            {
        //                CampaignId = campaignTopLimitDraft.CampaignId,
        //            });
        //        }
        //    }

        //    await _unitOfWork.GetRepository<TopLimitEntity>().AddAsync(entity);

        //    await _unitOfWork.SaveChangesAsync();

        //    var mappedTopLimit = _mapper.Map<TopLimitEntity>(entity);

        //    return await BaseResponse<TopLimitEntity>.SuccessAsync(mappedTopLimit);
        //}
        //private async Task<BaseResponse<TopLimitEntity>> ApproveTopLimitUpdateAsync(int refId, int id)
        //{
        //    var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
        //        .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
        //        .Include(x => x.TopLimitCampaigns.Where(x => !x.IsDeleted))
        //        .FirstOrDefaultAsync();
        //    var entity = await _unitOfWork.GetRepository<TopLimitEntity>().GetByIdAsync(id);

        //    if (draftEntity == null || entity == null) { throw new Exception("Çatı limit bulunamadı."); }

        //    draftEntity.IsApproved = true;
        //    await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(draftEntity);

        //    entity.AchievementFrequencyId = draftEntity.AchievementFrequencyId;
        //    entity.CurrencyId = draftEntity.CurrencyId;
        //    entity.IsActive = draftEntity.IsActive;
        //    entity.MaxTopLimitAmount = draftEntity.MaxTopLimitAmount;
        //    entity.MaxTopLimitRate = draftEntity.MaxTopLimitRate;
        //    entity.MaxTopLimitUtilization = draftEntity.MaxTopLimitUtilization;
        //    entity.Name = draftEntity.Name;
        //    entity.Type = draftEntity.Type;
        //    entity.IsDraft = false;
        //    entity.IsApproved = true;

        //    //delete campaigns
        //    foreach (var campaignDelete in _unitOfWork.GetRepository<CampaignTopLimitEntity>().
        //        GetAll(x => !x.IsDeleted && x.TopLimitId == id))
        //    {
        //        await _unitOfWork.GetRepository<CampaignTopLimitEntity>().DeleteAsync(campaignDelete);
        //    }

        //    //add campaigns
        //    if (draftEntity.TopLimitCampaigns.Any())
        //    {
        //        foreach (var campaignDraft in draftEntity.TopLimitCampaigns)
        //        {
        //            CampaignTopLimitEntity campaignTopLimitEntity = new CampaignTopLimitEntity()
        //            {
        //                CampaignId = campaignDraft.CampaignId,
        //                TopLimitId = id,
        //            };

        //            await _unitOfWork.GetRepository<CampaignTopLimitEntity>().AddAsync(campaignTopLimitEntity);
        //        }
        //    }

        //    await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(entity);

        //    await _unitOfWork.SaveChangesAsync();

        //    var mappedTopLimit = _mapper.Map<TopLimitEntity>(entity);

        //    return await BaseResponse<TopLimitEntity>.SuccessAsync(mappedTopLimit);

        //}
        public async Task<BaseResponse<TopLimitApproveFormDto>> GetTopLimitApprovalFormAsync(int refId) 
        {
            TopLimitApproveFormDto response = new TopLimitApproveFormDto();

            response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            response.AchievementFrequencyList = (await _parameterService.GetAchievementFrequencyListAsync())?.Data;
            response.CampaignList = (await _campaignService.GetParameterListAsync())?.Data;

            var topLimitDraftEntitiy = await _unitOfWork.GetRepository<TopLimitEntity>()
                                                          .GetAll(x => x.Id == refId && x.IsDeleted == false)
                                                          .Include(x => x.TopLimitCampaigns).ThenInclude(x => x.Campaign)
                                                          .Include(x => x.Currency)
                                                          .Include(x => x.AchievementFrequency).FirstOrDefaultAsync();

            if (topLimitDraftEntitiy == null) { throw new Exception("Çatı limit bulunamadı."); }

            var TopLimitEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                                                          .GetAll(x => x.RefId == topLimitDraftEntitiy.Id && x.IsDeleted == false)
                                                          .Include(x => x.TopLimitCampaigns).ThenInclude(x => x.Campaign)
                                                          .Include(x => x.Currency)
                                                          .Include(x => x.AchievementFrequency).FirstOrDefaultAsync();

            if (TopLimitEntity == null)
            {
                response.isNewRecord = true;
            }

            if (topLimitDraftEntitiy != null)
            {
                TopLimitDto mappedCampaignTopLimitDraft = new TopLimitDto
                {
                    AchievementFrequency = new Public.Dtos.ParameterDto { Id = topLimitDraftEntitiy.AchievementFrequency.Id, Name = topLimitDraftEntitiy.AchievementFrequency.Name },
                    AchievementFrequencyId = topLimitDraftEntitiy.AchievementFrequencyId,
                    Campaigns = topLimitDraftEntitiy.TopLimitCampaigns.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
                    {
                        Id = c.CampaignId,
                        Name = c.Campaign.Name
                    }).ToList(),
                    Currency = topLimitDraftEntitiy.Currency is null ? null : new Public.Dtos.ParameterDto { Id = topLimitDraftEntitiy.Currency?.Id ?? 0, Code = topLimitDraftEntitiy.Currency?.Code, Name = topLimitDraftEntitiy.Currency?.Name },
                    CurrencyId = topLimitDraftEntitiy.CurrencyId,
                    Id = topLimitDraftEntitiy.Id,
                    IsActive = topLimitDraftEntitiy.IsActive,
                    //MaxTopLimitAmount = topLimitDraftEntitiy.MaxTopLimitAmount,
                    //MaxTopLimitRate = topLimitDraftEntitiy.MaxTopLimitRate,
                    //MaxTopLimitUtilization = topLimitDraftEntitiy.MaxTopLimitUtilization,
                    Name = topLimitDraftEntitiy.Name,
                    Type = topLimitDraftEntitiy.Type
                };
                response.TopLimitDraft = mappedCampaignTopLimitDraft;
            }

            if (TopLimitEntity != null) 
            {
                TopLimitDto mappedCampaignTopLimit = new TopLimitDto
                {
                    AchievementFrequency = new Public.Dtos.ParameterDto { Id = TopLimitEntity.AchievementFrequency.Id, Name = TopLimitEntity.AchievementFrequency.Name },
                    AchievementFrequencyId = TopLimitEntity.AchievementFrequencyId,
                    Campaigns = TopLimitEntity.TopLimitCampaigns.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
                    {
                        Id = c.CampaignId,
                        Name = c.Campaign.Name
                    }).ToList(),
                    Currency = TopLimitEntity.Currency is null ? null : new Public.Dtos.ParameterDto { Id = TopLimitEntity.Currency?.Id ?? 0, Code = TopLimitEntity.Currency?.Code, Name = TopLimitEntity.Currency?.Name },
                    CurrencyId = TopLimitEntity.CurrencyId,
                    Id = TopLimitEntity.Id,
                    IsActive = TopLimitEntity.IsActive,
                    //MaxTopLimitAmount = TopLimitEntity.MaxTopLimitAmount,
                    //MaxTopLimitRate = TopLimitEntity.MaxTopLimitRate,
                    //MaxTopLimitUtilization = TopLimitEntity.MaxTopLimitUtilization,
                    Name = TopLimitEntity.Name,
                    Type = TopLimitEntity.Type
                };
                response.TopLimit = mappedCampaignTopLimit;
            }

            return await BaseResponse<TopLimitApproveFormDto>.SuccessAsync(response);
        }
        
        #endregion

        #region target
        public async Task<BaseResponse<TargetDto>> ApproveTargetAsync(int id)
        {
            var targetEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => (x.RefId ?? 0) == id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            return targetEntity == null ? await ApproveTargetAddAsync(id) : await ApproveTargetUpdateAsync(id, targetEntity.Id);

        }
        private async Task<BaseResponse<TargetDto>> ApproveTargetAddAsync(int refId)
        {
            var draftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
                .Include(x => x.TargetDetail)
                .FirstOrDefaultAsync();

            if (draftEntity == null) { throw new Exception("Hedef bulunamadı."); }

            //draftEntity
            draftEntity.IsApproved = true;
            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(draftEntity);

            //entity
            var targetDto = _mapper.Map<TargetDto>(draftEntity);
            var entity = _mapper.Map<TargetEntity>(targetDto);
            entity.Id = 0;
            entity.IsApproved = true;
            entity.IsDraft = false;
            entity.RefId = refId;

            //targetDetailEntity
            var detailDto = _mapper.Map<TargetDetailDto>(draftEntity.TargetDetail);
            var detailEntity = _mapper.Map<TargetDetailEntity>(detailDto);
            detailEntity.Id = 0;
            entity.TargetDetail = detailEntity;

            entity = await _unitOfWork.GetRepository<TargetEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            var mappedTarget = _mapper.Map<TargetDto>(entity);
            return await BaseResponse<TargetDto>.SuccessAsync(mappedTarget);
        }
        private async Task<BaseResponse<TargetDto>> ApproveTargetUpdateAsync(int refId, int id)
        {
            var draftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Id == refId && !x.IsDeleted && x.IsDraft && !x.IsApproved)
                .Include(x => x.TargetDetail)
                .FirstOrDefaultAsync();
            var entity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAllIncluding(x => x.TargetDetail)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (draftEntity == null || entity == null) { throw new Exception("Hedef bulunamadı."); }

            //draftEntity
            draftEntity.IsApproved = true;
            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(draftEntity);

            //targetEntity
            entity.Title = draftEntity.Title;
            entity.Name = draftEntity.Name;
            entity.IsActive = draftEntity.IsActive;
            entity.IsApproved = true;
            entity.IsDraft = false;
            entity.RefId = draftEntity.Id;

            //targetDetailEntity
            var targetDetail = entity.TargetDetail;
            var draftTargetDetail = draftEntity.TargetDetail;

            targetDetail.AdditionalFlowTime = draftTargetDetail.AdditionalFlowTime;
            targetDetail.FlowFrequency = draftTargetDetail.FlowFrequency;
            targetDetail.TotalAmount = draftTargetDetail.TotalAmount;
            targetDetail.Condition = draftTargetDetail.Condition;
            targetDetail.DescriptionEn = draftTargetDetail.DescriptionEn;
            targetDetail.DescriptionTr = draftTargetDetail.DescriptionTr;
            targetDetail.FlowName = draftTargetDetail.FlowName;
            targetDetail.NumberOfTransaction = draftTargetDetail.NumberOfTransaction;
            targetDetail.Query = draftTargetDetail.Query;
            targetDetail.TargetDetailEn = draftTargetDetail.TargetDetailEn;
            targetDetail.TargetDetailTr = draftTargetDetail.TargetDetailTr;
            targetDetail.TargetSourceId = draftTargetDetail.TargetSourceId;
            targetDetail.TargetViewTypeId = draftTargetDetail.TargetViewTypeId;
            targetDetail.TriggerTimeId = draftTargetDetail.TriggerTimeId;
            targetDetail.VerificationTimeId = draftTargetDetail.VerificationTimeId;

            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            var mappedTarget = _mapper.Map<TargetDto>(entity);

            return await BaseResponse<TargetDto>.SuccessAsync(mappedTarget);
        }
        public async Task<BaseResponse<TargetApproveFormDto>> GetTargetApprovalFormAsync(int refId)
        {
            TargetApproveFormDto response = new TargetApproveFormDto();

            var targetDdraftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAllIncluding(x => x.TargetDetail)
                .Where(x => x.Id == refId)
                .FirstOrDefaultAsync();

            var targetEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAllIncluding(x => x.TargetDetail)
                .Where(x => (x.RefId ?? 0) == refId)
                .FirstOrDefaultAsync();

            response.isNewRecord = targetEntity == null ? true : false;

            response.TargetDraft = _mapper.Map<TargetDto>(targetDdraftEntity);
            response.TargetDraftDetail = _mapper.Map<TargetDetailDto>(targetDdraftEntity.TargetDetail);

            if (!response.isNewRecord)
            {
                response.Target = _mapper.Map<TargetDto>(targetEntity);
                response.TargetDetail = _mapper.Map<TargetDetailDto>(targetEntity?.TargetDetail);
            }

            return await BaseResponse<TargetApproveFormDto>.SuccessAsync(response);
        }

        #endregion

        #region view

        public async Task<BaseResponse<CampaignViewFormDto>> GetCampaignViewFormAsync(int campaignId)
        {
            CampaignViewFormDto response = new CampaignViewFormDto();

            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            #region parameters

            response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
            response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
            response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
            response.CampaignStartTermList = (await _parameterService.GetCampaignStartTermListAsync())?.Data;
            response.BusinessLineList = (await _parameterService.GetBusinessLineListAsync())?.Data;
            response.BranchList = (await _parameterService.GetBranchListAsync())?.Data;
            response.CustomerTypeList = (await _parameterService.GetCustomerTypeListAsync())?.Data;
            response.JoinTypeList = (await _parameterService.GetJoinTypeListAsync())?.Data;
            response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
            response.ActionOptions = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

            #endregion


            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            response.Campaign = campaignDto;

            var campaignRule = await _campaignRuleService.GetCampaignRuleDto(campaignId);

            response.CampaignRule = campaignRule;

            //var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(campaignId, 0, 0);

            //response.CampaignTargetList = campaignTargetDto;

            //campaign
            //var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            //mappedCampaign.StartDate = campaignEntity.StartDate.ToShortDateString().Replace('.', '-');
            //mappedCampaign.EndDate = campaignEntity.EndDate.ToShortDateString().Replace('.', '-');
            //mappedCampaign.Code = mappedCampaign.Id.ToString();
            //response.Campaign = mappedCampaign;

            //var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
            //    .GetAll(x => x.CampaignId == id && x.IsDeleted != true)
            //    .Include(x => x.Branches.Where(t => t.IsDeleted != true))
            //    .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
            //    .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
            //    .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
            //    .FirstOrDefaultAsync();
            //if (campaignRuleEntity != null)
            //{
            //    string identityNumber = string.Empty;
            //    bool isSingleIdentity = campaignRuleEntity.RuleIdentities.Count() == 1; ;

            //    CampaignRuleDto campaignRuleDto = new CampaignRuleDto()
            //    {
            //        Id = campaignRuleEntity.Id,
            //        CampaignId = campaignRuleEntity.CampaignId,
            //        JoinTypeId = campaignRuleEntity.JoinTypeId,
            //        CampaignStartTermId = campaignRuleEntity.CampaignStartTermId,
            //        IdentityNumber = isSingleIdentity ? campaignRuleEntity.RuleIdentities.ElementAt(0).Identities : "",
            //        RuleBusinessLines = campaignRuleEntity.BusinessLines.Where(c => !c.IsDeleted).Select(c => c.BusinessLineId).ToList(),
            //        RuleCustomerTypes = campaignRuleEntity.CustomerTypes.Where(c => !c.IsDeleted).Select(c => c.CustomerTypeId).ToList(),
            //        RuleBranches = campaignRuleEntity.Branches.Where(c => !c.IsDeleted).Select(c => c.BranchCode).ToList(),
            //    };
            //    response.CampaignRule = campaignRuleDto;
            //}

            //targets
            //List<CampaignTargetDto> campaignTargetDtos =
            //    _unitOfWork.GetRepository<CampaignTargetEntity>()
            //    .GetAll(x => x.CampaignId == id && x.IsDeleted != true)
            //    .Include(x => x.Target)
            //    .Select(x => _mapper.Map<CampaignTargetDto>(x))
            //    .ToList();

            //response.CampaignTargetList = campaignTargetDtos;

            //achievements
            //var achievements = _unitOfWork.GetRepository<CampaignAchievementEntity>()
            //                          .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
            //                          .Include(x => x.ActionOption)
            //                          .Include(x => x.AchievementType)
            //                          //.OrderBy(x => x.CampaignChannelCode)
            //                          .ThenBy(x => x.Type).ToList();
            
        
            //var channelsAndAchievementsList = new List<ChannelsAndAchievementsByCampaignDto>();

            var channels = (await _parameterService.GetCampaignChannelListAsync()).Data;

            //foreach (var channel in channels)
            //{
            //    var _achievements = achievements.Where(x => x.CampaignChannelCode == channel).Select(x => new CampaignAchievementListDto
            //    {
            //        Id = x.Id,
            //        AchievementType = x.AchievementType?.Name,
            //        Action = x.ActionOption?.Name,
            //        Type = Helpers.GetEnumDescription(x.Type),
            //        MaxUtilization = x.MaxUtilization,
            //        //CampaignId = x.CampaignId,
            //        TypeId = (int)x.Type,
            //        CurrencyId = x.CurrencyId,
            //        Amount = x.Amount,
            //        Rate = x.Rate,
            //        MaxAmount = x.MaxAmount,
            //        AchievementTypeId = x.AchievementTypeId,
            //        ActionOptionId = x.ActionOptionId,
            //        DescriptionTr = x.DescriptionTr,
            //        DescriptionEn = x.DescriptionEn,
            //        TitleTr = x.TitleTr,
            //        TitleEn = x.TitleEn,
            //    }).ToList();

            //    channelsAndAchievementsList.Add(new ChannelsAndAchievementsByCampaignDto
            //    {
            //        CampaignChannelCode = channel,
            //        CampaignChannelName = channel,
            //        HasAchievement = _achievements.Any(),
            //        AchievementList = _achievements
            //    });

            //}
                
            //response.ChannelsAndAchievementsList = channelsAndAchievementsList;

            return await BaseResponse<CampaignViewFormDto>.SuccessAsync(response);
        }

        #endregion

        #region copy

        //taslak bir kampanya oluşturur
        public async Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int refId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == refId && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (campaignDraftEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            //campaign
            var campaignDto = _mapper.Map<CampaignDto>(campaignDraftEntity);
            var campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);
            campaignEntity.Id = 0;
            campaignEntity.Name = campaignDraftEntity.Name + "-Copy";
            campaignEntity.Code = string.Empty;
            campaignEntity.Order = null;
            campaignEntity.IsApproved = false;
            campaignEntity.IsDraft = true;
            campaignEntity.RefId = null;

            //campaign detail
            var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
            var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
            campaignDetailEntity.Id = 0;

            campaignEntity.CampaignDetail = campaignDetailEntity;

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            await AddCampaignRule(refId, campaignEntity);
            
            await AddCampaignDocument(refId, campaignEntity);

            await AddCampaignTarget(refId, campaignEntity);

            await AddCampaignChannelCode(refId, campaignEntity);

            await AddCampaignAchievement(refId, campaignEntity);

            await _unitOfWork.SaveChangesAsync();
            
            campaignEntity.Code = campaignEntity.Id.ToString();
            campaignEntity.RefId = campaignEntity.Id;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }

        public async Task<BaseResponse<TopLimitDto>> TopLimitCopyAsync(int refId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.TopLimit;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            var topLimitDraftEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                                                          .GetAll(x => x.Id == refId && x.IsDeleted == false)
                                                          .Include(x => x.TopLimitCampaigns.Where(x=>!x.IsDeleted))
                                                          .FirstOrDefaultAsync();
            if (topLimitDraftEntity == null)
                throw new Exception("Çatı limiti bulunamadı.");

            //topLimitEntity

            TopLimitEntity topLimitEntity = new TopLimitEntity();
            topLimitEntity.AchievementFrequencyId = topLimitDraftEntity.AchievementFrequencyId;
            topLimitEntity.CurrencyId = topLimitDraftEntity.CurrencyId;
            topLimitEntity.IsActive = topLimitDraftEntity.IsActive;
            topLimitEntity.MaxTopLimitAmount = topLimitDraftEntity.MaxTopLimitAmount;
            topLimitEntity.MaxTopLimitRate = topLimitDraftEntity.MaxTopLimitRate;
            topLimitEntity.MaxTopLimitUtilization = topLimitDraftEntity.MaxTopLimitUtilization;
            topLimitEntity.Name = topLimitDraftEntity.Name + "-Copy";
            topLimitEntity.Type = topLimitDraftEntity.Type;
            topLimitEntity.IsApproved = false;
            topLimitEntity.IsDraft = true;
            topLimitEntity.RefId = null;

            topLimitEntity.TopLimitCampaigns = new List<CampaignTopLimitEntity>();
            foreach (var campaign in topLimitDraftEntity.TopLimitCampaigns)
            {
                topLimitEntity.TopLimitCampaigns.Add(new CampaignTopLimitEntity()
                {
                    CampaignId = campaign.CampaignId,
                });
            }

            await _unitOfWork.GetRepository<TopLimitEntity>().AddAsync(topLimitEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedTopLimit = _mapper.Map<TopLimitDto>(topLimitEntity);

            return await BaseResponse<TopLimitDto>.SuccessAsync(mappedTopLimit);
        }

        public async Task<BaseResponse<TargetDto>> TargetCopyAsync(int refId, string userid) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Target;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            var targetDraftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                                                              .GetAll(x => x.Id == refId && x.IsDeleted == false)
                                                              .Include(x=>x.TargetDetail)
                                                              .FirstOrDefaultAsync();
            if (targetDraftEntity == null)
                throw new Exception("Hedef bulunamadı.");

            if (targetDraftEntity.TargetDetail == null)
                throw new Exception("Hedef detayı bulunamadı.");

            //target
            var targetDto = _mapper.Map<TargetDto>(targetDraftEntity);
            targetDto.Id = 0;
            var targetEntity = _mapper.Map<TargetEntity>(targetDto);
            //targetEntity.Id = 0;
            targetEntity.Name = targetDraftEntity.Name + "-Copy";
            targetEntity.IsApproved = false;
            targetEntity.IsDraft = true;
            targetEntity.RefId = null;
            targetEntity.TargetDetail = null;

             await _unitOfWork.GetRepository<TargetEntity>().AddAsync(targetEntity);

            if (targetDraftEntity.TargetDetail != null) 
            {
                var targetDetailDto = _mapper.Map<TargetDetailDto>(targetDraftEntity.TargetDetail);
                targetDetailDto.Id = 0;
                var targetDetailEntity = _mapper.Map<TargetDetailEntity>(targetDetailDto);
                //targetDetailEntity.Id = 0;
                targetDetailEntity.Target = targetEntity;

                await _unitOfWork.GetRepository<TargetDetailEntity>().AddAsync(targetDetailEntity);

                //targetEntity.TargetDetail = targetDetailEntity;
            }
            
           

            await _unitOfWork.SaveChangesAsync();

            var mappedTarget = _mapper.Map<TargetDto>(targetEntity);

            return await BaseResponse<TargetDto>.SuccessAsync(mappedTarget);
        }

        #endregion

    }
}
