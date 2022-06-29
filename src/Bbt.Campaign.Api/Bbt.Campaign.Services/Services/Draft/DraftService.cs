using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignTarget;
using Bbt.Campaign.Public.Models.Draft;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Draft
{
    public class DraftService : IDraftService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationService _authorizationService;

        public DraftService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationService;
        }
        public async Task<int> CreateCampaignDraftAsync(int campaignId, string userid, int pageTypeId)
        {
            await CheckValidationCampaignCopy(campaignId);

            //campaign
            CampaignEntity campaignEntity = new CampaignEntity();
            campaignEntity.CampaignDetail = new CampaignDetailEntity();
            campaignEntity = await CopyCampaignInfo(campaignEntity, campaignId, userid, false, false, false, true, true, false);
            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            //campaign rule
            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                .Include(x => x.RuleIdentities)
                .Include(x => x.Branches.Where(x => !x.IsDeleted))
                .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            if (campaignRuleDraftEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı.");
            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();
            campaignRuleEntity = await CopyCampaignRuleInfo(campaignRuleDraftEntity, campaignRuleEntity, campaignEntity, userid, false, false);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);
            if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                foreach (var campaignRuleIdentityEntity in await CopyCampaignRuleIdentites(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);

            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            {
                foreach (var campaignRuleBusinessLineEntity in await CopyCampaignRuleBusiness(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
            {
                foreach (var campaignRuleBranchEntity in await CopyCampaignRuleBranches(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                foreach (var campaignRuleCustomerTypeEntity in await CopyCampaignRuleCustomerTypes(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
            }

            foreach (var campaignDocument in await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid, true))
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            foreach (var campaignTarget in await CopyCampaignTargetInfo(campaignId, campaignEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTarget);

            foreach (var campaignChannelCode in await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            foreach (var campaignAchievement in await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            CampaignUpdatePageEntity campaignUpdatePageEntity = new CampaignUpdatePageEntity();
            campaignUpdatePageEntity.Campaign = campaignEntity;
            campaignUpdatePageEntity.IsCampaignUpdated = pageTypeId == (int)PageTypeEnum.Campaign;
            campaignUpdatePageEntity.IsCampaignRuleUpdated = pageTypeId == (int)PageTypeEnum.CampaignRule;
            campaignUpdatePageEntity.IsCampaignTargetUpdated = pageTypeId == (int)PageTypeEnum.CampaignTarget;
            campaignUpdatePageEntity.IsCampaignChannelCodeUpdated = pageTypeId == (int)PageTypeEnum.CampaignChannelCode;
            campaignUpdatePageEntity.IsCampaignAchievementUpdated = pageTypeId == (int)PageTypeEnum.CampaignAchievement;
            await _unitOfWork.GetRepository<CampaignUpdatePageEntity>().AddAsync(campaignUpdatePageEntity);

            await _unitOfWork.SaveChangesAsync();

            return campaignEntity.Id;
        }


        public async Task<BaseResponse<CampaignDto>> CreateCampaignCopyAsync(int campaignId, string userid)
        {
            await CheckValidationCampaignCopy(campaignId);

            //campaign
            CampaignEntity campaignEntity = new CampaignEntity();
            campaignEntity.CampaignDetail = new CampaignDetailEntity();
            campaignEntity = await CopyCampaignInfo(campaignEntity, campaignId, userid, false, false, false, false, false, false);
            campaignEntity.Name = string.Concat(campaignEntity.Name, "-Copy");

            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            //campaign rule
            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                .Include(x => x.RuleIdentities)
                .Include(x => x.Branches.Where(x => !x.IsDeleted))
                .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            if (campaignRuleDraftEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı.");
            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();
            campaignRuleEntity = await CopyCampaignRuleInfo(campaignRuleDraftEntity, campaignRuleEntity, campaignEntity, userid, false, false);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);
            if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                foreach (var campaignRuleIdentityEntity in await CopyCampaignRuleIdentites(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);

            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            {
                foreach (var campaignRuleBusinessLineEntity in await CopyCampaignRuleBusiness(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
            {
                foreach (var campaignRuleBranchEntity in await CopyCampaignRuleBranches(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                foreach (var campaignRuleCustomerTypeEntity in await CopyCampaignRuleCustomerTypes(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
            }

            //campaign document
            foreach (var campaignDocument in await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid, false))
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);
            //campaign target
            foreach (var campaignTarget in await CopyCampaignTargetInfo(campaignId, campaignEntity, userid, false, false))
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTarget);
            //campaign channel code
            foreach (var campaignChannelCode in await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid, false, false))
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);
            //campaign achievement
            foreach (var campaignAchievement in await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid, false, false))
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);

            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }

        private async Task CheckValidationCampaignCopy(int campaignId)
        {
            var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                .Where(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .FirstOrDefault();
            if (approvedCampaign == null)
                throw new Exception("Kampanya uygun statüde değildir.");
        }

        public async Task<CampaignEntity> CopyCampaignInfo(CampaignEntity targetEntity, int sourceCampaignId, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo,
            bool isIncludeOrder, bool isIncludeCode, bool isIncludeStatusId)
        {
            var sourceEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == sourceCampaignId && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (sourceEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            //campaign
            targetEntity.ContractId = sourceEntity.ContractId;
            targetEntity.ProgramTypeId = sourceEntity.ProgramTypeId;
            targetEntity.SectorId = sourceEntity.SectorId;
            targetEntity.ViewOptionId = sourceEntity.ViewOptionId;
            targetEntity.IsBundle = sourceEntity.IsBundle;
            targetEntity.IsActive = sourceEntity.IsActive;
            targetEntity.IsContract = sourceEntity.IsContract;
            targetEntity.DescriptionTr = sourceEntity.DescriptionTr;
            targetEntity.DescriptionEn = sourceEntity.DescriptionEn;
            targetEntity.StartDate = sourceEntity.StartDate;
            targetEntity.EndDate = sourceEntity.EndDate;
            targetEntity.Name = sourceEntity.Name;
            targetEntity.TitleTr = sourceEntity.TitleTr;
            targetEntity.TitleEn = sourceEntity.TitleEn;
            targetEntity.MaxNumberOfUser = sourceEntity.MaxNumberOfUser;
            targetEntity.ParticipationTypeId = sourceEntity.ParticipationTypeId;
            targetEntity.StatusId = isIncludeStatusId ? sourceEntity.StatusId : (int)StatusEnum.Draft;
            targetEntity.CreatedBy = isIncludeCreateInfo ? sourceEntity.CreatedBy : userid;
            targetEntity.CreatedOn = isIncludeCreateInfo ? sourceEntity.CreatedOn : DateTime.UtcNow;

            if (isIncludeApproveInfo)
            {
                targetEntity.ApprovedBy = sourceEntity.ApprovedBy;
                targetEntity.ApprovedDate = sourceEntity.ApprovedDate;
            }
            targetEntity.Code = isIncludeCode ? sourceEntity.Code : Helpers.CreateCampaignCode();
            targetEntity.Order = isIncludeOrder ? sourceEntity.Order : null;

            //campaign detail
            targetEntity.CampaignDetail.DetailEn = sourceEntity.CampaignDetail.DetailEn;
            targetEntity.CampaignDetail.DetailTr = sourceEntity.CampaignDetail.DetailTr;
            targetEntity.CampaignDetail.SummaryEn = sourceEntity.CampaignDetail.SummaryEn;
            targetEntity.CampaignDetail.SummaryTr = sourceEntity.CampaignDetail.SummaryTr;
            targetEntity.CampaignDetail.CampaignDetailImageUrl = sourceEntity.CampaignDetail.CampaignDetailImageUrl;
            targetEntity.CampaignDetail.CampaignListImageUrl = sourceEntity.CampaignDetail.CampaignListImageUrl;
            targetEntity.CampaignDetail.ContentTr = sourceEntity.CampaignDetail.ContentTr;
            targetEntity.CampaignDetail.ContentEn = sourceEntity.CampaignDetail.ContentEn;
            targetEntity.CampaignDetail.CreatedBy = isIncludeCreateInfo ? sourceEntity.CampaignDetail.CreatedBy : userid;
            targetEntity.CampaignDetail.CreatedOn = isIncludeCreateInfo ? sourceEntity.CampaignDetail.CreatedOn : DateTime.UtcNow;
            targetEntity.CampaignDetail.LastModifiedBy = isIncludeUpdateInfo ? sourceEntity.CampaignDetail.LastModifiedBy : userid;
            targetEntity.CampaignDetail.LastModifiedOn = isIncludeUpdateInfo ? sourceEntity.CampaignDetail.LastModifiedOn : DateTime.UtcNow;

            return targetEntity;

        }
        
        public async Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
        {
            List<CampaignDocumentEntity> campaignDocumentlist = new List<CampaignDocumentEntity>();

            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            foreach (var x in documents)
            {
                campaignDocumentlist.Add(new CampaignDocumentEntity()
                {
                    Campaign = campaignEntity,
                    Content = x.Content,
                    DocumentName = x.DocumentName,
                    DocumentType = x.DocumentType,
                    MimeType = x.MimeType,
                    CreatedBy = isIncludeUpdateInfo ? x.CreatedBy : userid,
                    CreatedOn = isIncludeUpdateInfo ? x.CreatedOn : DateTime.UtcNow,
                    LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid,
                    LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow,
                });
            }

            return campaignDocumentlist;
        }
        public async Task<List<CampaignTargetEntity>> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignTargetEntity> campaignTargetList = new List<CampaignTargetEntity>();
            List<CampaignTargetEntity> sourceList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                     .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
                     .ToList();
            if (sourceList.Count == 0)
                throw new Exception("Kampanya hedefleri bulunamadı.");

            foreach (var sourceEntity in sourceList)
            {
                CampaignTargetEntity targetEntity = new CampaignTargetEntity();
                targetEntity.Id = 0;
                targetEntity.Campaign = campaignEntity;
                targetEntity.TargetId = sourceEntity.TargetId;
                targetEntity.TargetGroupId = sourceEntity.TargetGroupId;
                targetEntity.TargetOperationId = sourceEntity.TargetOperationId;
                targetEntity.CreatedBy = isIncludeCreateInfo ? sourceEntity.CreatedBy : userid;
                targetEntity.CreatedOn = isIncludeCreateInfo ? sourceEntity.CreatedOn : DateTime.UtcNow;
                campaignTargetList.Add(targetEntity);
            }

            return campaignTargetList;
        }
        public async Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignChannelCodeEntity> campaignChannelCodeList = new List<CampaignChannelCodeEntity>();
            var campaignChannelCodes = _unitOfWork.GetRepository<CampaignChannelCodeEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            if (!campaignChannelCodes.Any())
                throw new Exception("Kampanya kanal kodu bulunamadı.");
            foreach (var x in campaignChannelCodes)
            {
                CampaignChannelCodeEntity campaignChannelCodeEntity = new CampaignChannelCodeEntity();
                campaignChannelCodeEntity.Campaign = campaignEntity;
                campaignChannelCodeEntity.ChannelCode = x.ChannelCode;
                campaignChannelCodeEntity.CreatedBy = isIncludeCreateInfo ? x.CreatedBy : userid;
                campaignChannelCodeEntity.CreatedOn = isIncludeCreateInfo ? x.CreatedOn : DateTime.UtcNow;
                campaignChannelCodeList.Add(campaignChannelCodeEntity);
            }
            return campaignChannelCodeList;
        }
        public async Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignAchievementEntity> campaignAchievementList = new List<CampaignAchievementEntity>();

            var achievementDraftList = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .ToListAsync();
            if (!achievementDraftList.Any())
                throw new Exception("Kampanya kazanımları bulunamadı.");
            foreach (var sourceEntity in achievementDraftList)
            {
                var targetEntity = new CampaignAchievementEntity();
                targetEntity.Id = 0;
                targetEntity.Campaign = campaignEntity;
                targetEntity.CurrencyId = sourceEntity.CurrencyId;
                targetEntity.Amount = sourceEntity.Amount;
                targetEntity.Rate = sourceEntity.Rate;
                targetEntity.MaxAmount = sourceEntity.MaxAmount;
                targetEntity.MaxUtilization = sourceEntity.MaxUtilization;
                targetEntity.AchievementTypeId = sourceEntity.AchievementTypeId;
                targetEntity.ActionOptionId = sourceEntity.ActionOptionId;
                targetEntity.DescriptionTr = sourceEntity.DescriptionTr;
                targetEntity.DescriptionEn = sourceEntity.DescriptionEn;
                targetEntity.TitleTr = sourceEntity.TitleTr;
                targetEntity.TitleEn = sourceEntity.TitleEn;
                targetEntity.Type = sourceEntity.Type;
                targetEntity.XKAMPCode = sourceEntity.XKAMPCode;
                targetEntity.CreatedBy = isIncludeCreateInfo ? sourceEntity.CreatedBy : userid;
                targetEntity.CreatedOn = isIncludeCreateInfo ? sourceEntity.CreatedOn : DateTime.UtcNow;
                campaignAchievementList.Add(targetEntity);
            }
            return campaignAchievementList;
        }



        public async Task<CampaignRuleEntity> CopyCampaignRuleInfo(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            campaignRuleEntity.Campaign = campaignEntity;
            campaignRuleEntity.CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId;
            campaignRuleEntity.JoinTypeId = campaignRuleDraftEntity.JoinTypeId;
            campaignRuleEntity.IsEmployeeIncluded = campaignRuleDraftEntity.IsEmployeeIncluded;
            campaignRuleEntity.IsPrivateBanking = campaignRuleDraftEntity.IsPrivateBanking;
            campaignRuleEntity.CreatedBy = isIncludeCreateInfo ? campaignRuleDraftEntity.CreatedBy : userid;
            campaignRuleEntity.CreatedOn = isIncludeCreateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.UtcNow;
            return campaignRuleEntity;
        }
        public async Task<List<CampaignRuleBranchEntity>> CopyCampaignRuleBranches(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignRuleBranchEntity> campaignRuleBranchList = new List<CampaignRuleBranchEntity>();
            foreach (var x in campaignRuleDraftEntity.Branches)
            {
                CampaignRuleBranchEntity campaignRuleBranchEntity = new CampaignRuleBranchEntity();
                campaignRuleBranchEntity.CampaignRule = campaignRuleEntity;
                campaignRuleBranchEntity.BranchCode = x.BranchCode;
                campaignRuleBranchEntity.BranchName = x.BranchName;
                campaignRuleBranchEntity.CreatedBy = isIncludeCreateInfo ? x.CreatedBy : userid;
                campaignRuleBranchEntity.CreatedOn = isIncludeCreateInfo ? x.CreatedOn : DateTime.UtcNow;
                campaignRuleBranchList.Add(campaignRuleBranchEntity);
            }
            return campaignRuleBranchList;
        }
        public async Task<List<CampaignRuleBusinessLineEntity>> CopyCampaignRuleBusiness(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignRuleBusinessLineEntity> campaignRuleBusinessLineList = new List<CampaignRuleBusinessLineEntity>();
            foreach (var x in campaignRuleDraftEntity.BusinessLines)
            {
                CampaignRuleBusinessLineEntity campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity();
                campaignRuleBusinessLineEntity.CampaignRule = campaignRuleEntity;
                campaignRuleBusinessLineEntity.BusinessLineId = x.BusinessLineId;
                campaignRuleBusinessLineEntity.CreatedBy = isIncludeCreateInfo ? x.CreatedBy : userid;
                campaignRuleBusinessLineEntity.CreatedOn = isIncludeCreateInfo ? x.CreatedOn : DateTime.UtcNow;
                campaignRuleBusinessLineList.Add(campaignRuleBusinessLineEntity);
            }
            return campaignRuleBusinessLineList;
        }
        public async Task<List<CampaignRuleCustomerTypeEntity>> CopyCampaignRuleCustomerTypes(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignRuleCustomerTypeEntity> campaignRuleCustomerTypeList = new List<CampaignRuleCustomerTypeEntity>();
            foreach (var x in campaignRuleDraftEntity.CustomerTypes)
            {
                CampaignRuleCustomerTypeEntity campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity();
                campaignRuleCustomerTypeEntity.CampaignRule = campaignRuleEntity;
                campaignRuleCustomerTypeEntity.CustomerTypeId = x.CustomerTypeId;
                campaignRuleCustomerTypeEntity.CreatedBy = isIncludeCreateInfo ? x.CreatedBy : userid;
                campaignRuleCustomerTypeEntity.CreatedOn = isIncludeCreateInfo ? x.CreatedOn : DateTime.UtcNow;
                campaignRuleCustomerTypeList.Add(campaignRuleCustomerTypeEntity);
            }
            return campaignRuleCustomerTypeList;
        }
        public async Task<List<CampaignRuleIdentityEntity>> CopyCampaignRuleIdentites(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            List<CampaignRuleIdentityEntity> campaignRuleIdentityList = new List<CampaignRuleIdentityEntity>();
            foreach (var x in campaignRuleDraftEntity.RuleIdentities)
            {
                CampaignRuleIdentityEntity campaignRuleIdentityEntity = new CampaignRuleIdentityEntity();
                campaignRuleIdentityEntity.CampaignRule = campaignRuleEntity;
                campaignRuleIdentityEntity.Identities = x.Identities;
                campaignRuleIdentityEntity.CreatedBy = isIncludeCreateInfo ? x.CreatedBy : userid;
                campaignRuleIdentityEntity.CreatedOn = isIncludeCreateInfo ? x.CreatedOn : DateTime.UtcNow;
                campaignRuleIdentityList.Add(campaignRuleIdentityEntity);
            }
            return campaignRuleIdentityList;
        }

        

        public async Task<TopLimitEntity> CopyTopLimitInfo(int topLimitId, TopLimitEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo, 
            bool isIncludeCode, bool isIncludeStatusId) 
        {
            var sourceEntity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => !x.IsDeleted && x.Id == topLimitId)
                .Include(x => x.TopLimitCampaigns)
                .FirstOrDefaultAsync();
            if (sourceEntity == null)
                throw new Exception("Çatı limiti bulunamadı.");
            if (sourceEntity.StatusId == (int)StatusEnum.Draft)
                throw new Exception("İşlem yapılmak istenen bu kayıt uygun statüde değildir.");

            targetEntity.AchievementFrequencyId = sourceEntity.AchievementFrequencyId;
            targetEntity.CurrencyId = sourceEntity.CurrencyId;
            targetEntity.IsActive = sourceEntity.IsActive;
            targetEntity.MaxTopLimitAmount = sourceEntity.MaxTopLimitAmount;
            targetEntity.MaxTopLimitRate = sourceEntity.MaxTopLimitRate;
            targetEntity.MaxTopLimitUtilization = sourceEntity.MaxTopLimitUtilization;
            targetEntity.Name = sourceEntity.Name;
            targetEntity.Type = sourceEntity.Type;
            targetEntity.StatusId = isIncludeStatusId ? sourceEntity.StatusId : (int)StatusEnum.Draft; 
            targetEntity.Code = isIncludeCode ? sourceEntity.Code : Helpers.CreateCampaignCode();
            targetEntity.CreatedBy = isIncludeCreateInfo ? sourceEntity.CreatedBy : userid;
            targetEntity.CreatedOn = isIncludeCreateInfo ? sourceEntity.CreatedOn : DateTime.UtcNow;
            if (isIncludeApproveInfo)
            {
                targetEntity.ApprovedBy = sourceEntity.ApprovedBy;
                targetEntity.ApprovedDate = sourceEntity.ApprovedDate;
            }
            return targetEntity;
        }

        public async Task<TargetEntity> CopyTargetInfo(int targetId, TargetEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo,
            bool isIncludeCode, bool isIncludeStatusId)
        {
            var sourceEntity = await _unitOfWork.GetRepository<TargetEntity>().GetAll(x => !x.IsDeleted && x.Id == targetId)
                .Include(x => x.TargetDetail)
                .FirstOrDefaultAsync();
            if (sourceEntity == null)
                throw new Exception("Hedef bulunamadı.");
            if(sourceEntity.StatusId == (int)StatusEnum.Draft)
                throw new Exception("İşlem yapılmak istenen bu kayıt uygun statüde değildir.");

            targetEntity.Name = sourceEntity.Name;
            targetEntity.Title = sourceEntity.Title;
            targetEntity.IsActive = sourceEntity.IsActive; 
            targetEntity.Code = isIncludeCode ? sourceEntity.Code : Helpers.CreateCampaignCode();
            targetEntity.StatusId = isIncludeStatusId ? sourceEntity.StatusId : (int)StatusEnum.Draft;
            targetEntity.CreatedBy = isIncludeCreateInfo ? sourceEntity.CreatedBy : userid;
            targetEntity.CreatedOn = isIncludeCreateInfo ? sourceEntity.CreatedOn : DateTime.UtcNow;
            if (isIncludeApproveInfo)
            {
                targetEntity.ApprovedBy = sourceEntity.ApprovedBy;
                targetEntity.ApprovedDate = sourceEntity.ApprovedDate;
            }

            targetEntity.TargetDetail.AdditionalFlowTime = sourceEntity.TargetDetail.AdditionalFlowTime;
            targetEntity.TargetDetail.FlowFrequency = sourceEntity.TargetDetail.FlowFrequency;
            targetEntity.TargetDetail.TotalAmount = sourceEntity.TargetDetail.TotalAmount;
            targetEntity.TargetDetail.Condition = sourceEntity.TargetDetail.Condition;
            targetEntity.TargetDetail.DescriptionEn = sourceEntity.TargetDetail.DescriptionEn;
            targetEntity.TargetDetail.DescriptionTr = sourceEntity.TargetDetail.DescriptionTr;
            targetEntity.TargetDetail.FlowName = sourceEntity.TargetDetail.FlowName;
            targetEntity.TargetDetail.NumberOfTransaction = sourceEntity.TargetDetail.NumberOfTransaction;
            targetEntity.TargetDetail.Query = sourceEntity.TargetDetail.Query;
            targetEntity.TargetDetail.TargetDetailEn = sourceEntity.TargetDetail.TargetDetailEn;
            targetEntity.TargetDetail.TargetDetailTr = sourceEntity.TargetDetail.TargetDetailTr;
            targetEntity.TargetDetail.TargetSourceId = sourceEntity.TargetDetail.TargetSourceId;
            targetEntity.TargetDetail.TargetViewTypeId = sourceEntity.TargetDetail.TargetViewTypeId;
            targetEntity.TargetDetail.TriggerTimeId = sourceEntity.TargetDetail.TriggerTimeId;
            targetEntity.TargetDetail.VerificationTimeId = sourceEntity.TargetDetail.VerificationTimeId;
            targetEntity.TargetDetail.CreatedBy = isIncludeCreateInfo ? sourceEntity.TargetDetail.CreatedBy : userid;
            targetEntity.TargetDetail.CreatedOn = isIncludeCreateInfo ? sourceEntity.TargetDetail.CreatedOn : DateTime.UtcNow;

            return targetEntity;
        }

        public async Task<List<CampaignTopLimitEntity>> CopyCampaignTopLimits(int topLimitId, TopLimitEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo) 
        {
            List<CampaignTopLimitEntity> campaignTopLimitList = new List<CampaignTopLimitEntity>();
            var sourceEntity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => !x.IsDeleted && x.Id == topLimitId)
                .Include(x => x.TopLimitCampaigns)
                .FirstOrDefaultAsync();
            if (sourceEntity == null)
                throw new Exception("Çatı limiti bulunamadı.");
            foreach (var item in sourceEntity.TopLimitCampaigns)
            {
                CampaignTopLimitEntity campaignTopLimitEntity = new CampaignTopLimitEntity();
                campaignTopLimitEntity.TopLimit = targetEntity;
                campaignTopLimitEntity.CampaignId = item.CampaignId;
                campaignTopLimitEntity.CreatedBy = isIncludeCreateInfo ? item.CreatedBy : userid;
                campaignTopLimitEntity.CreatedOn = isIncludeCreateInfo ? item.CreatedOn : DateTime.UtcNow;
                campaignTopLimitList.Add(campaignTopLimitEntity);
            }
            return campaignTopLimitList;
        }
        public async Task<bool> IsActiveCampaign(int campaignId)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && x.StatusId == (int)StatusEnum.Approved && 
                            !x.IsDeleted && x.EndDate > DateTime.UtcNow.AddDays(-1))
                .FirstOrDefaultAsync();
            return campaignEntity != null;
        }
        public async Task<CampaignProperty> GetCampaignProperties(int campaignId)
        {
            CampaignProperty campaignProperty = new CampaignProperty();

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı");

            int viewOptionId = campaignEntity.ViewOptionId ?? 0;
            campaignProperty.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;

            campaignProperty.IsActiveCampaign = campaignEntity.IsActive && campaignEntity.EndDate > DateTime.UtcNow.AddDays(-1) && !campaignEntity.IsDeleted;

            campaignProperty.IsUpdatableCampaign = campaignEntity.StatusId == (int)StatusEnum.Draft;

            return campaignProperty;
        }


        #region ProcessType

        public async Task<int> GetCampaignProcessType(int campaignId)
        {
            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (entity == null)
                throw new Exception("Kampanya bulunamadı.");
            var sentToApproveEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Code == entity.Code && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (sentToApproveEntity != null)
                throw new Exception("Bu kampanya için onayda bekleyen bir kayıt vardır. Güncelleme yapılamaz.");

            return GetProcessId(entity.StatusId);
        }
        public async Task<int> GetTopLimitProcessType(int topLimitId)
        {
            var entity = await _unitOfWork.GetRepository<TopLimitEntity>().GetByIdAsync(topLimitId);
            if (entity == null)
                throw new Exception("Çatı limiti bulunamadı.");

            var sentToApproveEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                .GetAll(x => x.Code == entity.Code && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (sentToApproveEntity != null)
                throw new Exception("Bu çatı limiti için onayda bekleyen bir kayıt vardır. Güncelleme yapılamaz.");

            return GetProcessId(entity.StatusId);
        }
        public async Task<int> GetTargetProcessType(int targetId)
        {
            var entity = await _unitOfWork.GetRepository<TargetEntity>().GetByIdAsync(targetId);
            if (entity == null)
                throw new Exception("Hedef bulunamadı.");

            var sentToApproveEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Code == entity.Code && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (sentToApproveEntity != null)
                throw new Exception("Bu hedef için onayda bekleyen bir kayıt vardır. Güncelleme yapılamaz.");

            return GetProcessId(entity.StatusId);
        }
        private int GetProcessId(int statusId)
        {
            int retVal = (int)ProcessTypesEnum.Update;
            if (statusId == (int)StatusEnum.Draft)
                retVal = (int)ProcessTypesEnum.Update;
            else if (statusId == (int)StatusEnum.SentToApprove || statusId == (int)StatusEnum.History)
                throw new Exception("Bu kayıt güncellenmek için uygun statüde değildir.");
            else if (statusId == (int)StatusEnum.Approved)
                retVal = (int)ProcessTypesEnum.CreateDraft;
            return retVal;
        }

        #endregion
    }
}
