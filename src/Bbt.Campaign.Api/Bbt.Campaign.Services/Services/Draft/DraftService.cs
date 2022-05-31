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
        public async Task<int> CreateCampaignDraftAsync(int campaignId, string userid)
        {
            var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                .Where(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .FirstOrDefault();
            if (approvedCampaign == null)
                throw new Exception("Kampanya bulunamadı");

            //copy campaign
            CampaignEntity campaignEntity = await CopyCampaignInfo(campaignId, userid, true);
            campaignEntity.Code = approvedCampaign.Code;
            campaignEntity.StatusId = (int)StatusEnum.Draft;

            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            CampaignRuleEntity campaignRuleEntity = await CopyCampaignRuleInfo(campaignId, campaignEntity, userid, true);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            List<CampaignDocumentEntity> campaignDocumentlist = await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid, true);
            foreach (var campaignDocument in campaignDocumentlist)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            List<CampaignTargetEntity> campaignTargetList = await CopyCampaignTargetInfo(campaignId, campaignEntity, userid, true);
            foreach (var campaignTarget in campaignTargetList)
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTarget);

            List<CampaignChannelCodeEntity> campaignChannelCodeList = await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid, true);
            foreach (var campaignChannelCode in campaignChannelCodeList)
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            List<CampaignAchievementEntity> campaignAchievementList = await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid, true);
            foreach (var campaignAchievement in campaignAchievementList)
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            try { await _unitOfWork.SaveChangesAsync(); }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

            return campaignEntity.Id;
        }


        public async Task<BaseResponse<CampaignDto>> CreateCampaignCopyAsync(int campaignId, string userid)
        {
            await CheckValidationCampaignCopy(campaignId, userid);

            CampaignEntity campaignEntity = await CopyCampaignInfo(campaignId, userid, false);
            campaignEntity.Name = string.Concat(campaignEntity.Name, "-Copy");
            campaignEntity.Code = Helpers.CreateCampaignCode();
            campaignEntity.Order = null;
            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            CampaignRuleEntity campaignRuleEntity = await CopyCampaignRuleInfo(campaignId, campaignEntity, userid, false);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            List<CampaignDocumentEntity> campaignDocumentlist = await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid, false);
            foreach (var campaignDocument in campaignDocumentlist)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            List<CampaignTargetEntity> campaignTargetList = await CopyCampaignTargetInfo(campaignId, campaignEntity, userid, false);
            foreach(var campaignTarget in campaignTargetList)
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTarget);

            List<CampaignChannelCodeEntity> campaignChannelCodeList = await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid, false);
            foreach (var campaignChannelCode in campaignChannelCodeList)
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            List<CampaignAchievementEntity> campaignAchievementList = await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid, false);
            foreach (var campaignAchievement in campaignAchievementList)
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            await _unitOfWork.SaveChangesAsync();

            //campaignEntity.Code = campaignEntity.Id.ToString();
            //campaignEntity.campaignId = campaignEntity.Id;

            //await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

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
                throw new Exception("Kampanya bulunamadı.");
            if (entity.StatusId == (int)StatusEnum.Draft)
            {
                var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                   .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                   .FirstOrDefaultAsync();
                if (campaignRuleEntity == null)
                {
                    throw new Exception("Kampanya kuralları giriniz.");
                }

                var campaignTargetEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>()
                    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (campaignTargetEntity == null)
                {
                    throw new Exception("Kampanya hedefleri giriniz.");
                }

                var campaignChannelCodeEntity = await _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
                    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (campaignChannelCodeEntity == null)
                {
                    throw new Exception("Kampanya kanal kodu giriniz.");
                }

                var campaignAchievementEntity = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (campaignAchievementEntity == null)
                {
                    throw new Exception("Kampanya kanal kodu giriniz.");
                }
            }
        }


        public async Task<CampaignEntity> CopyCampaignInfo(int campaignId, string userid, bool isIncludeUpdateInfo)
        {
            CampaignEntity campaignEntity = new CampaignEntity();
            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (campaignDraftEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            //campaign
            var campaignDto = _mapper.Map<CampaignDto>(campaignDraftEntity);
            campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);
            campaignEntity.Id = 0;
            campaignEntity.StatusId = (int)StatusEnum.Draft;
            if (isIncludeUpdateInfo) 
            {
                campaignEntity.CreatedBy = campaignDraftEntity.CreatedBy;
                campaignEntity.CreatedOn = campaignDraftEntity.CreatedOn;
                campaignEntity.LastModifiedBy = campaignDraftEntity.LastModifiedBy;
                campaignEntity.LastModifiedOn = campaignDraftEntity.LastModifiedOn;
            }
            else 
            { 
                campaignEntity.CreatedBy = userid;
            }
            

            //campaign detail
            var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
            var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
            campaignDetailEntity.Id = 0;
            if (isIncludeUpdateInfo)
            {
                campaignDetailEntity.CreatedBy = campaignDraftEntity.CreatedBy;
                campaignDetailEntity.CreatedOn = campaignDraftEntity.CreatedOn;
                campaignDetailEntity.LastModifiedBy = campaignDraftEntity.LastModifiedBy;
                campaignDetailEntity.LastModifiedOn = campaignDraftEntity.LastModifiedOn;
            }
            else
            {
                campaignDetailEntity.CreatedBy = userid;
            }

            campaignEntity.CampaignDetail = campaignDetailEntity;

            return campaignEntity;

        }
        public async Task<CampaignRuleEntity> CopyCampaignRuleInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
        {
            
            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .Include(x => x.Branches.Where(t => t.IsDeleted != true))
                .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
                .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
                .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();
            if (campaignRuleDraftEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı.");

            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();
            campaignRuleEntity.Campaign = campaignEntity;
            campaignRuleEntity.CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId;
            campaignRuleEntity.JoinTypeId = campaignRuleDraftEntity.JoinTypeId;
            if (isIncludeUpdateInfo)
            {
                campaignRuleEntity.CreatedBy = campaignRuleDraftEntity.CreatedBy;
                campaignRuleEntity.CreatedOn = campaignRuleDraftEntity.CreatedOn;
                campaignRuleEntity.LastModifiedBy = campaignRuleDraftEntity.LastModifiedBy;
                campaignRuleEntity.LastModifiedOn = campaignRuleDraftEntity.LastModifiedOn;
            }
            else
            {
                campaignRuleEntity.CreatedBy = userid;
            }

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
                            BranchName = branch.BranchName,
                            CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
                            CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
                            LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
                            LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
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
                            CustomerTypeId = customerType.CustomerTypeId,
                            CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
                            CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
                            LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
                            LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
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
                            BusinessLineId = businessLine.BusinessLineId,
                            CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
                            CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
                            LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
                            LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
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
                        CreatedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedBy : userid,
                        CreatedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.CreatedOn : DateTime.MinValue,
                        LastModifiedBy = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedBy : null,
                        LastModifiedOn = isIncludeUpdateInfo ? campaignRuleDraftEntity.LastModifiedOn : null,
                    });
                }
            }

            return campaignRuleEntity;
        }
        public async Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
        {
            List<CampaignDocumentEntity> campaignDocumentlist = new List<CampaignDocumentEntity>();

            var documents = _unitOfWork.GetRepository<CampaignDocumentEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
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
                    CreatedOn = isIncludeUpdateInfo ? x.CreatedOn : DateTime.MinValue,
                    LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : null,
                    LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : null,
                });
            }

            return campaignDocumentlist;
        }
        public async Task<List<CampaignTargetEntity>> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
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
                var campaignTargetDto = _mapper.Map<CampaignTargetDto>(campaignTargetDraftEntity);
                campaignTargetEntity = _mapper.Map<CampaignTargetEntity>(campaignTargetDto);
                campaignTargetEntity.Id = 0;
                campaignTargetEntity.Campaign = campaignEntity;
                campaignTargetEntity.TargetId = campaignTargetDraftEntity.TargetId;
                campaignTargetEntity.TargetGroupId = campaignTargetDraftEntity.TargetGroupId;
                campaignTargetEntity.TargetOperationId = campaignTargetDraftEntity.TargetOperationId;
                if (isIncludeUpdateInfo)
                {
                    campaignTargetEntity.CreatedBy = campaignTargetDraftEntity.CreatedBy;
                    campaignTargetEntity.CreatedOn = campaignTargetDraftEntity.CreatedOn;
                    campaignTargetEntity.LastModifiedBy = campaignTargetDraftEntity.LastModifiedBy;
                    campaignTargetEntity.LastModifiedOn = campaignTargetDraftEntity.LastModifiedOn;
                }
                else
                {
                    campaignTargetEntity.CreatedBy = userid;
                }
                campaignTargetList.Add(campaignTargetEntity);
            }

            return campaignTargetList;
        }
        public async Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
        {
            List<CampaignChannelCodeEntity> campaignChannelCodeList = new List<CampaignChannelCodeEntity>();
            var campaignChannelCodes = _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
                .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted);
            if (!campaignChannelCodes.Any())
                throw new Exception("Kampanya kanal kodu bulunamadı.");
            foreach (var x in campaignChannelCodes)
            {
                campaignChannelCodeList.Add(new CampaignChannelCodeEntity()
                {
                    Campaign = campaignEntity,
                    ChannelCode = x.ChannelCode,
                    CreatedBy = isIncludeUpdateInfo ? x.CreatedBy : userid,
                    CreatedOn = isIncludeUpdateInfo ? x.CreatedOn : DateTime.MinValue,
                    LastModifiedBy = isIncludeUpdateInfo ? x.LastModifiedBy : null,
                    LastModifiedOn = isIncludeUpdateInfo ? x.LastModifiedOn : null,
                });
            }

            return campaignChannelCodeList;
        }
        public async Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo)
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
                if (isIncludeUpdateInfo)
                {
                    campaignAchievementEntity.CreatedBy = achievementDraftEntity.CreatedBy;
                    campaignAchievementEntity.CreatedOn = achievementDraftEntity.CreatedOn;
                    campaignAchievementEntity.LastModifiedBy = achievementDraftEntity.LastModifiedBy;
                    campaignAchievementEntity.LastModifiedOn = achievementDraftEntity.LastModifiedOn;
                }
                else
                {
                    campaignAchievementEntity.CreatedBy = userid;
                }
                campaignAchievementList.Add(campaignAchievementEntity);
            }
            return campaignAchievementList;
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
            int retVal  = (int)ProcessTypesEnum.Update;

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
