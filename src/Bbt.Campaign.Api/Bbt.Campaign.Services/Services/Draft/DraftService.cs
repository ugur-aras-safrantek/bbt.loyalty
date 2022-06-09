using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
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
            var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                .Where(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .FirstOrDefault();
            if (approvedCampaign == null)
                throw new Exception("Kampanya bulunamadı");

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
            await CheckValidationCampaignCopy(campaignId, userid);

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

        private async Task CheckValidationCampaignCopy(int campaignId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;
            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (entity == null)
                throw new Exception("Kampanya bulunamadı");
            if (entity.StatusId != (int)StatusEnum.Approved)
                throw new Exception("Bu kampanya kopyalanmak için uygun statude değildir.");



            //if (entity.StatusId != (int)StatusEnum.Approved)
            //{
            //    //var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
            //    //   .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
            //    //   .FirstOrDefaultAsync();
            //    //if (campaignRuleEntity == null)
            //    //{
            //    //    throw new Exception("Kampanya kuralları giriniz.");
            //    //}

            //    //var campaignTargetEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>()
            //    //    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
            //    //    .FirstOrDefaultAsync();
            //    //if (campaignTargetEntity == null)
            //    //{
            //    //    throw new Exception("Kampanya hedefleri giriniz.");
            //    //}

            //    //var campaignChannelCodeEntity = await _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
            //    //    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
            //    //    .FirstOrDefaultAsync();
            //    //if (campaignChannelCodeEntity == null)
            //    //{
            //    //    throw new Exception("Kampanya kanal kodu giriniz.");
            //    //}

            //    //var campaignAchievementEntity = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
            //    //    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
            //    //    .FirstOrDefaultAsync();
            //    //if (campaignAchievementEntity == null)
            //    //{
            //    //    throw new Exception("Kampanya kanal kodu giriniz.");
            //    //}
            //}
        }


        public async Task<CampaignEntity> CopyCampaignInfo(CampaignEntity campaignEntity, int campaignId, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo,
            bool isIncludeOrder, bool isIncludeCode, bool isIncludeStatusId)
        {
            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (campaignDraftEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            //campaign
            campaignEntity.ContractId = campaignDraftEntity.ContractId;
            campaignEntity.ProgramTypeId = campaignDraftEntity.ProgramTypeId;
            campaignEntity.SectorId = campaignDraftEntity.SectorId;
            campaignEntity.ViewOptionId = campaignDraftEntity.ViewOptionId;
            campaignEntity.IsBundle = campaignDraftEntity.IsBundle;
            campaignEntity.IsActive = campaignDraftEntity.IsActive;
            campaignEntity.IsContract = campaignDraftEntity.IsContract;
            campaignEntity.DescriptionTr = campaignDraftEntity.DescriptionTr;
            campaignEntity.DescriptionEn = campaignDraftEntity.DescriptionEn;
            campaignEntity.StartDate = campaignDraftEntity.StartDate;
            campaignEntity.EndDate = campaignDraftEntity.EndDate;
            campaignEntity.Name = campaignDraftEntity.Name;
            campaignEntity.TitleTr = campaignDraftEntity.TitleTr;
            campaignEntity.TitleEn = campaignDraftEntity.TitleEn;
            campaignEntity.MaxNumberOfUser = campaignDraftEntity.MaxNumberOfUser;
            campaignEntity.ParticipationTypeId = campaignDraftEntity.ParticipationTypeId;
            campaignEntity.StatusId = isIncludeStatusId ? campaignDraftEntity.StatusId : (int)StatusEnum.Draft;
            campaignEntity.CreatedBy = isIncludeCreateInfo ? campaignDraftEntity.CreatedBy : userid;
            campaignEntity.CreatedOn = isIncludeCreateInfo ? campaignDraftEntity.CreatedOn : DateTime.UtcNow;
            campaignEntity.LastModifiedBy = isIncludeUpdateInfo ? campaignDraftEntity.LastModifiedBy : userid;
            campaignEntity.LastModifiedOn = isIncludeUpdateInfo ? campaignDraftEntity.LastModifiedOn : DateTime.UtcNow;
            if (isIncludeApproveInfo)
            {
                campaignEntity.ApprovedBy = campaignDraftEntity.ApprovedBy;
                campaignEntity.ApprovedDate = campaignDraftEntity.ApprovedDate;
            }
            campaignEntity.Code = isIncludeCode ? campaignDraftEntity.Code : Helpers.CreateCampaignCode(); 
            campaignEntity.Order = isIncludeOrder ? campaignDraftEntity.Order : null;

            //campaign detail
            campaignEntity.CampaignDetail.DetailEn = campaignDraftEntity.CampaignDetail.DetailEn;
            campaignEntity.CampaignDetail.DetailTr = campaignDraftEntity.CampaignDetail.DetailTr;
            campaignEntity.CampaignDetail.SummaryEn = campaignDraftEntity.CampaignDetail.SummaryEn;
            campaignEntity.CampaignDetail.SummaryTr = campaignDraftEntity.CampaignDetail.SummaryTr;
            campaignEntity.CampaignDetail.CampaignDetailImageUrl = campaignDraftEntity.CampaignDetail.CampaignDetailImageUrl;
            campaignEntity.CampaignDetail.CampaignListImageUrl = campaignDraftEntity.CampaignDetail.CampaignListImageUrl;
            campaignEntity.CampaignDetail.ContentTr = campaignDraftEntity.CampaignDetail.ContentTr;
            campaignEntity.CampaignDetail.ContentEn = campaignDraftEntity.CampaignDetail.ContentEn;
            campaignEntity.CampaignDetail.CreatedBy = isIncludeCreateInfo ? campaignDraftEntity.CampaignDetail.CreatedBy : userid;
            campaignEntity.CampaignDetail.CreatedOn = isIncludeCreateInfo ? campaignDraftEntity.CampaignDetail.CreatedOn : DateTime.UtcNow;
            campaignEntity.CampaignDetail.LastModifiedBy = isIncludeUpdateInfo ? campaignDraftEntity.CampaignDetail.LastModifiedBy : userid;
            campaignEntity.CampaignDetail.LastModifiedOn = isIncludeUpdateInfo ? campaignDraftEntity.CampaignDetail.LastModifiedOn : DateTime.UtcNow;

            return campaignEntity;

        }
        //public async Task<CampaignRuleEntity> CopyCampaignRuleInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
        //{

        //    var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
        //        .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
        //        .Include(x => x.Branches.Where(t => t.IsDeleted != true))
        //        .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
        //        .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
        //        .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
        //        .FirstOrDefaultAsync();
        //    if (campaignRuleDraftEntity == null)
        //        throw new Exception("Kampanya kuralı bulunamadı.");

        //    CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();
        //    campaignRuleEntity.Campaign = campaignEntity;
        //    campaignRuleEntity.CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId;
        //    campaignRuleEntity.JoinTypeId = campaignRuleDraftEntity.JoinTypeId;
        //    if (isIncludeUpdateInfo)
        //    {
        //        campaignRuleEntity.CreatedBy = campaignRuleDraftEntity.CreatedBy;
        //        campaignRuleEntity.CreatedOn = campaignRuleDraftEntity.CreatedOn;
        //        campaignRuleEntity.LastModifiedBy = campaignRuleDraftEntity.LastModifiedBy;
        //        campaignRuleEntity.LastModifiedOn = campaignRuleDraftEntity.LastModifiedOn;
        //    }
        //    else
        //    {
        //        campaignRuleEntity.CreatedBy = userid;
        //    }

        //    if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
        //    {
        //        if (campaignRuleDraftEntity.Branches.Any())
        //        {
        //            campaignRuleEntity.Branches = new List<CampaignRuleBranchEntity>();
        //            foreach (var branch in campaignRuleDraftEntity.Branches)
        //            {
        //                campaignRuleEntity.Branches.Add(new CampaignRuleBranchEntity()
        //                {
        //                    BranchCode = branch.BranchCode,
        //                    BranchName = branch.BranchName,
        //                    CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
        //                    CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
        //                    LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
        //                    LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
        //                });
        //            }
        //        }
        //    }
        //    else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
        //    {
        //        if (campaignRuleDraftEntity.CustomerTypes.Any())
        //        {
        //            campaignRuleEntity.CustomerTypes = new List<CampaignRuleCustomerTypeEntity>();
        //            foreach (var customerType in campaignRuleDraftEntity.CustomerTypes)
        //            {
        //                campaignRuleEntity.CustomerTypes.Add(new CampaignRuleCustomerTypeEntity()
        //                {
        //                    CustomerTypeId = customerType.CustomerTypeId,
        //                    CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
        //                    CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
        //                    LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
        //                    LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
        //                });
        //            }
        //        }
        //    }
        //    else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
        //    {
        //        if (campaignRuleDraftEntity.BusinessLines.Any())
        //        {
        //            campaignRuleEntity.BusinessLines = new List<CampaignRuleBusinessLineEntity>();
        //            foreach (var businessLine in campaignRuleDraftEntity.BusinessLines)
        //            {
        //                campaignRuleEntity.BusinessLines.Add(new CampaignRuleBusinessLineEntity()
        //                {
        //                    BusinessLineId = businessLine.BusinessLineId,
        //                    CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
        //                    CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
        //                    LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
        //                    LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
        //                });
        //            }
        //        }
        //    }
        //    else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
        //    {
        //        campaignRuleEntity.RuleIdentities = new List<CampaignRuleIdentityEntity>();
        //        foreach (var ruleIdentity in campaignRuleDraftEntity.RuleIdentities)
        //        {
        //            campaignRuleEntity.RuleIdentities.Add(new CampaignRuleIdentityEntity()
        //            {
        //                Identities = ruleIdentity.Identities,
        //                CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
        //                CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
        //                LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
        //                LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
        //            });
        //        }
        //    }

        //    return campaignRuleEntity;
        //}
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
            List<CampaignTargetEntity> campaignTargetDraftList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                     .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
                     .ToList();
            if (campaignTargetDraftList.Count == 0)
                throw new Exception("Kampanya hedefleri bulunamadı.");

            foreach (var campaignTargetDraftEntity in campaignTargetDraftList)
            {
                CampaignTargetEntity campaignTargetEntity = new CampaignTargetEntity();
                campaignTargetEntity.Id = 0;
                campaignTargetEntity.Campaign = campaignEntity;
                campaignTargetEntity.TargetId = campaignTargetDraftEntity.TargetId;
                campaignTargetEntity.TargetGroupId = campaignTargetDraftEntity.TargetGroupId;
                campaignTargetEntity.TargetOperationId = campaignTargetDraftEntity.TargetOperationId;
                campaignTargetEntity.CreatedBy = isIncludeCreateInfo ? campaignTargetDraftEntity.CreatedBy : userid;
                campaignTargetEntity.CreatedOn = isIncludeCreateInfo ? campaignTargetDraftEntity.CreatedOn : DateTime.UtcNow;
                campaignTargetEntity.LastModifiedBy = isIncludeUpdateInfo ? campaignTargetDraftEntity.LastModifiedBy : userid;
                campaignTargetEntity.LastModifiedOn = isIncludeUpdateInfo ? campaignTargetDraftEntity.LastModifiedOn : DateTime.UtcNow;
                campaignTargetList.Add(campaignTargetEntity);
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
                campaignChannelCodeEntity.LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid;
                campaignChannelCodeEntity.LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow;
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
            foreach (var achievementDraftEntity in achievementDraftList)
            {
                var campaignAchievementDto = _mapper.Map<CampaignAchievementDto>(achievementDraftEntity);
                var campaignAchievementEntity = _mapper.Map<CampaignAchievementEntity>(campaignAchievementDto);
                campaignAchievementEntity.Id = 0;
                campaignAchievementEntity.Campaign = campaignEntity;
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
                campaignAchievementEntity.Type = achievementDraftEntity.Type;
                campaignAchievementEntity.CreatedBy = isIncludeCreateInfo ? achievementDraftEntity.CreatedBy : userid;
                campaignAchievementEntity.CreatedOn = isIncludeCreateInfo ? achievementDraftEntity.CreatedOn : DateTime.UtcNow;
                campaignAchievementEntity.LastModifiedBy = isIncludeUpdateInfo ? achievementDraftEntity.LastModifiedBy : userid;
                campaignAchievementEntity.LastModifiedOn = isIncludeUpdateInfo ? achievementDraftEntity.LastModifiedOn : DateTime.UtcNow;
                campaignAchievementList.Add(campaignAchievementEntity);
            }
            return campaignAchievementList;
        }



        public async Task<CampaignRuleEntity> CopyCampaignRuleInfo(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        {
            campaignRuleEntity.Campaign = campaignEntity;
            campaignRuleEntity.CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId;
            campaignRuleEntity.JoinTypeId = campaignRuleDraftEntity.JoinTypeId;
            campaignRuleEntity.CreatedBy = isIncludeCreateInfo ? campaignRuleDraftEntity.CreatedBy : userid;
            campaignRuleEntity.CreatedOn = isIncludeCreateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.UtcNow;
            campaignRuleEntity.LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : userid;
            campaignRuleEntity.LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : DateTime.UtcNow;
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
                campaignRuleBranchEntity.LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid;
                campaignRuleBranchEntity.LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow;
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
                campaignRuleBusinessLineEntity.LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid;
                campaignRuleBusinessLineEntity.LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow;
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
                campaignRuleCustomerTypeEntity.LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid;
                campaignRuleCustomerTypeEntity.LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow;
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
                campaignRuleIdentityEntity.LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : userid;
                campaignRuleIdentityEntity.LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : DateTime.UtcNow;
                campaignRuleIdentityList.Add(campaignRuleIdentityEntity);
            }
            return campaignRuleIdentityList;
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
        public async Task<int> GetProcessType(int canpaignId)
        {
            int retVal = (int)ProcessTypesEnum.Update;

            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(canpaignId);

            if (entity.StatusId == (int)StatusEnum.Draft)
            {
                retVal = (int)ProcessTypesEnum.Update;
            }
            else if (entity.StatusId == (int)StatusEnum.SentToApprove || entity.StatusId == (int)StatusEnum.History)
            {
                throw new Exception("Bu kampanya güncellenmek için uygun statüde değildir.");
            }
            else if (entity.StatusId == (int)StatusEnum.Approved)
            {
                retVal = (int)ProcessTypesEnum.CreateDraft;

                //var usingEntity = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                //    .Where(x => !x.IsDeleted && x.Code == entity.Code && x.StatusId == (int)StatusEnum.SentToApprove)
                //    .FirstOrDefault();
                //if (usingEntity == null) 
                //    retVal = (int)ProcessTypesEnum.CreateDraft;
                //else
                //    throw new Exception("Bu kampanya güncellenmek için uygun statüde değildir.");
            }
            return retVal;
        }
    }
}
