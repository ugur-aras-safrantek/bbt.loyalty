using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignChannelCode;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.CampaignTopLimit;
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Services.Services.Report;
using Bbt.Campaign.Services.Services.Target.Detail;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Target.Services.Services.Target;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Bbt.Campaign.Services.Services.Approval
{
    public class ApprovalService : IApprovalService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRuleService _campaignRuleService;
        private readonly ICampaignChannelCodeService _campaignChannelCodeService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDraftService _draftService;
        private readonly IReportService _reportService;
        private readonly ICampaignAchievementService _campaignAchievementService;
        private readonly ICampaignTopLimitService _campaignTopLimitService;
        private readonly ITargetService _targetService;
        private readonly ITargetDetailService _targetDetailService;

        public ApprovalService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            ICampaignService campaignService, 
            ICampaignRuleService campaignRuleService, 
            ICampaignChannelCodeService campaignChannelCodeService,
            ICampaignTargetService campaignTargetService, 
            IAuthorizationService authorizationservice, 
            IReportService reportService, 
            IDraftService draftService,
            ICampaignAchievementService campaignAchievementService,
            ICampaignTopLimitService campaignTopLimitService,
            ITargetService targetService,
            ITargetDetailService targetDetailService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignRuleService = campaignRuleService;
            _campaignChannelCodeService = campaignChannelCodeService;
            _campaignTargetService = campaignTargetService;
            _authorizationService = authorizationservice;
            _reportService = reportService;
            _draftService = draftService;
            _campaignAchievementService = campaignAchievementService;
            _campaignTopLimitService = campaignTopLimitService;
            _targetService = targetService;
            _targetDetailService = targetDetailService;
        }

        #region campaign


        public DateTime ConvertWithInvariantCulture(string date, string format) 
        {
            return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
        }

        public DateTime ConvertWithCulture(string date, string format, string culture)
        {
            IFormatProvider iFormatProvider = new CultureInfo(culture);
            return DateTime.ParseExact(date, format, iFormatProvider);
        }

        public DateTime ConvertWithNewDateTime(string dateStr) 
        {
            string[] dateArray = dateStr.Split('-');
            int day = int.Parse(dateArray[0]);
            int month = int.Parse(dateArray[1]);    
            int year = int.Parse(dateArray[2]);
            return new DateTime(year, month, day);  
        }

        public async Task<BaseResponse<CampaignDto>> ApproveCampaignAsync(int id, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Approve;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            var draftCampaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == id && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (draftCampaignEntity == null)
                throw new Exception("Kampanya bulunamadı");

            if (draftCampaignEntity.CreatedBy == userRole.UserId)
                throw new Exception("Kampanya kaydını oluşturan kullanıcı ile onaylayan kullanıcı aynı kişi olamaz.");

            var approvedCampaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Code == draftCampaignEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (approvedCampaignEntity == null)
            {
                return await ApproveCampaignAddAsync(id, userRole.UserId);
            }
            else 
            {
                return await ApproveCampaignUpdateAsync(id, approvedCampaignEntity.Id, userRole.UserId);
            }
        }
        public async Task<BaseResponse<CampaignDto>> DisApproveCampaignAsync(int id, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Approve;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == id && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı");

            campaignEntity.StatusId = (int)StatusEnum.Draft;
            campaignEntity.LastModifiedBy = userRole.UserId;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);
            await _unitOfWork.SaveChangesAsync();
            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }
        private async Task<BaseResponse<CampaignDto>> ApproveCampaignAddAsync(int id, string userid)
        { 
            DateTime now = DateTime.UtcNow;
            var draftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();
            if (draftEntity == null)
                throw new Exception("Kampanya bulunamadı.");
            if (draftEntity.CreatedBy == userid)
                throw new Exception("Kampanyayı oluşturan kullanıcı ile onaylayan kullanıcı aynı kişi olamaz.");

            //update campaign

            draftEntity.StatusId = (int)StatusEnum.Approved;
            draftEntity.ApprovedBy = userid;
            draftEntity.ApprovedDate = now;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(draftEntity);

            #region add history

            CampaignEntity historyEntity = new CampaignEntity();
            historyEntity.CampaignDetail = new CampaignDetailEntity();
            historyEntity = await _draftService.CopyCampaignInfo(historyEntity, id, userid, true, true, true, true, true, false);
            historyEntity.ApprovedBy = userid;
            historyEntity.ApprovedDate = now;
            historyEntity.StatusId = (int)StatusEnum.History;

            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(historyEntity);

            var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == id && !x.IsDeleted)
                .Include(x => x.RuleIdentities)
                .Include(x => x.Branches.Where(x => !x.IsDeleted))
                .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            if (campaignRuleDraftEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı.");
            CampaignRuleEntity campaignRuleHistoryEntity = new CampaignRuleEntity();
            campaignRuleHistoryEntity = await _draftService.CopyCampaignRuleInfo(campaignRuleDraftEntity, campaignRuleHistoryEntity, historyEntity, userid, false, false);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleHistoryEntity);
            if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                foreach (var campaignRuleIdentityEntity in await _draftService.CopyCampaignRuleIdentites(campaignRuleDraftEntity, campaignRuleHistoryEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);

            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            {
                foreach (var campaignRuleBusinessLineEntity in await _draftService.CopyCampaignRuleBusiness(campaignRuleDraftEntity, campaignRuleHistoryEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
            {
                foreach (var campaignRuleBranchEntity in await _draftService.CopyCampaignRuleBranches(campaignRuleDraftEntity, campaignRuleHistoryEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
            }
            else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                foreach (var campaignRuleCustomerTypeEntity in await _draftService.CopyCampaignRuleCustomerTypes(campaignRuleDraftEntity, campaignRuleHistoryEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
            }

            foreach (var campaignDocumentHistory in await _draftService.CopyCampaignDocumentInfo(id, historyEntity, userid, true))
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocumentHistory);

            foreach (var campaignTargetHistory in await _draftService.CopyCampaignTargetInfo(id, historyEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetHistory);

            foreach (var campaignChannelCodeHistory in await _draftService.CopyCampaignChannelCodeInfo(id, historyEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCodeHistory);

            foreach (var campaignAchievementHistory in await _draftService.CopyCampaignAchievementInfo(id, historyEntity, userid, true, true))
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievementHistory);

            #endregion

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(draftEntity);
            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }
        public async Task<BaseResponse<CampaignDto>> ApproveCampaignUpdateAsync(int draftId, int campaignId, string userid) 
        {
            DateTime now = DateTime.UtcNow;
            var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == draftId && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            if (campaignDraftEntity == null || campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }

            if (campaignDraftEntity.CreatedBy == userid)
                throw new Exception("Kampanyayı oluşturan kullanıcı ile onaylayan kullanıcı aynı kişi olamaz.");

            var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == draftId).FirstOrDefault();
            if(campaignUpdatePageEntity == null) 
                throw new Exception("Bu kayıt için güncelleme kaydı yoktur.");

            #region add history kapatıldı

            //CampaignEntity historyEntity = new CampaignEntity();
            //historyEntity.CampaignDetail = new CampaignDetailEntity();
            //historyEntity = await _draftService.CopyCampaignInfo(historyEntity, campaignId, userid, true, true, true, true, true, false);
            //historyEntity.StatusId = (int)StatusEnum.History;
            //await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(historyEntity);

            //var campaignRuleDraftHistoryEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
            //    .GetAll(x => x.CampaignId == draftId && !x.IsDeleted)
            //    .Include(x => x.RuleIdentities)
            //    .Include(x => x.Branches.Where(x => !x.IsDeleted))
            //    .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
            //    .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
            //    .FirstOrDefaultAsync();
            //if (campaignRuleDraftHistoryEntity == null)
            //    throw new Exception("Kampanya kuralı bulunamadı.");
            //CampaignRuleEntity campaignRuleHistoryEntity = new CampaignRuleEntity();
            //campaignRuleHistoryEntity = await _draftService.CopyCampaignRuleInfo(campaignRuleDraftHistoryEntity, campaignRuleHistoryEntity, historyEntity, userid, false, false);
            //await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleHistoryEntity);
            //if (campaignRuleDraftHistoryEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            //{
            //    foreach (var campaignRuleIdentityEntity in await _draftService.CopyCampaignRuleIdentites(campaignRuleDraftHistoryEntity, campaignRuleHistoryEntity, userid, true, true))
            //        await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);

            //}
            //else if (campaignRuleDraftHistoryEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            //{
            //    foreach (var campaignRuleBusinessLineEntity in await _draftService.CopyCampaignRuleBusiness(campaignRuleDraftHistoryEntity, campaignRuleHistoryEntity, userid, true, true))
            //        await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
            //}
            //else if (campaignRuleDraftHistoryEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
            //{
            //    foreach (var campaignRuleBranchEntity in await _draftService.CopyCampaignRuleBranches(campaignRuleDraftHistoryEntity, campaignRuleHistoryEntity, userid, true, true))
            //        await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
            //}
            //else if (campaignRuleDraftHistoryEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            //{
            //    foreach (var campaignRuleCustomerTypeEntity in await _draftService.CopyCampaignRuleCustomerTypes(campaignRuleDraftHistoryEntity, campaignRuleHistoryEntity, userid, true, true))
            //        await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
            //}

            //foreach (var campaignDocumentHistory in await _draftService.CopyCampaignDocumentInfo(draftId, historyEntity, userid, true))
            //    await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocumentHistory);

            //foreach (var campaignTargetHistory in await _draftService.CopyCampaignTargetInfo(draftId, historyEntity, userid, true, true))
            //    await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetHistory);

            //foreach (var campaignChannelCodeHistory in await _draftService.CopyCampaignChannelCodeInfo(draftId, historyEntity, userid, true, true))
            //    await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCodeHistory);

            //foreach (var campaignAchievementHistory in await _draftService.CopyCampaignAchievementInfo(draftId, historyEntity, userid, true, true))
            //    await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievementHistory);

            #endregion

            if (campaignUpdatePageEntity.IsCampaignUpdated)
            {
                campaignEntity = await _draftService.CopyCampaignInfo(campaignEntity, draftId, userid, true, true, false, true, true, false);
            }
            campaignEntity.StatusId = (int)StatusEnum.Approved;
            campaignEntity.ApprovedDate = DateTime.UtcNow;
            campaignEntity.ApprovedBy = userid;
            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            if (campaignUpdatePageEntity.IsCampaignRuleUpdated) 
            {
                var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                    .GetAll(x => x.CampaignId == draftId && !x.IsDeleted)
                    .Include(x=>x.RuleIdentities)
                    .Include(x => x.Branches.Where(x => !x.IsDeleted))
                    .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                    .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                    .FirstOrDefaultAsync();
                if (campaignRuleDraftEntity == null) throw new Exception("Kazanım kanalı bulunamadı");
                var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                    .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                    .Include(x => x.RuleIdentities)
                    .Include(x => x.Branches.Where(x => !x.IsDeleted))
                    .Include(x => x.BusinessLines.Where(x => !x.IsDeleted))
                    .Include(x => x.CustomerTypes.Where(x => !x.IsDeleted))
                    .FirstOrDefaultAsync();
                if (campaignRuleEntity == null) throw new Exception("Kazanım kanalı bulunamadı");
                //delete previous data
                foreach (var x in campaignRuleEntity.Branches)
                    await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().DeleteAsync(x);
                foreach (var x in campaignRuleEntity.CustomerTypes)
                    await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().DeleteAsync(x);
                foreach (var x in campaignRuleEntity.BusinessLines)
                    await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().DeleteAsync(x);
                foreach (var x in campaignRuleEntity.RuleIdentities)
                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().DeleteAsync(x);
                campaignRuleEntity = await _draftService.CopyCampaignRuleInfo(campaignRuleDraftEntity, campaignRuleEntity, campaignEntity, userid, true, true);
                await _unitOfWork.GetRepository<CampaignRuleEntity>().UpdateAsync(campaignRuleEntity);
                if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer) 
                {
                    foreach (var campaignRuleIdentityEntity in await _draftService.CopyCampaignRuleIdentites(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                        await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);

                }
                else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine) 
                {
                    foreach (var campaignRuleBusinessLineEntity in await _draftService.CopyCampaignRuleBusiness(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                        await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
                }
                else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch) 
                {
                    foreach (var campaignRuleBranchEntity in await _draftService.CopyCampaignRuleBranches(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                        await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
                }
                else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType) 
                {
                    foreach (var campaignRuleCustomerTypeEntity in await _draftService.CopyCampaignRuleCustomerTypes(campaignRuleDraftEntity, campaignRuleEntity, userid, true, true))
                        await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
                }
            }

            if (campaignUpdatePageEntity.IsCampaignTargetUpdated) 
            {
                foreach (var x in _unitOfWork.GetRepository<CampaignTargetEntity>().GetAll(x => !x.IsDeleted && x.CampaignId == campaignId).ToList())
                    await _unitOfWork.GetRepository<CampaignTargetEntity>().DeleteAsync(x);
                foreach (var campaignTarget in await _draftService.CopyCampaignTargetInfo(draftId, campaignEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTarget);
            }

            if (campaignUpdatePageEntity.IsCampaignChannelCodeUpdated) 
            {
                foreach (var campaignChannelCodeDelete in _unitOfWork.GetRepository<CampaignChannelCodeEntity>().GetAll(x => !x.IsDeleted && x.CampaignId == campaignId).ToList())
                    await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().DeleteAsync(campaignChannelCodeDelete);
                foreach (var campaignChannelCode in await _draftService.CopyCampaignChannelCodeInfo(draftId, campaignEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);
            }

            if (campaignUpdatePageEntity.IsCampaignAchievementUpdated) 
            {
                foreach (var campaignAchievementDelete in _unitOfWork.GetRepository<CampaignAchievementEntity>().GetAll(x => !x.IsDeleted && x.CampaignId == campaignId).ToList())
                    await _unitOfWork.GetRepository<CampaignAchievementEntity>().DeleteAsync(campaignAchievementDelete);
                foreach (var campaignAchievement in await _draftService.CopyCampaignAchievementInfo(draftId, campaignEntity, userid, true, true))
                    await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);
            }

            campaignDraftEntity.ApprovedBy = userid;
            campaignDraftEntity.ApprovedDate = now;
            campaignDraftEntity.StatusId = (int)StatusEnum.History;
            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignDraftEntity);

            try { await _unitOfWork.SaveChangesAsync(); }
            catch(Exception ex) { throw new Exception(ex.ToString()); }

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);

            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }
        public async Task<BaseResponse<CampaignApproveFormDto>> GetCampaignApprovalFormAsync(int draftId, UserRoleDto userRoleDto) 
        {
            CampaignApproveFormDto response = new CampaignApproveFormDto();

            var campaignQuery = _unitOfWork.GetRepository<CampaignReportEntity>().GetAll().Where(x =>x.Id == draftId);
            if (!campaignQuery.Any()) 
                throw new Exception("Kampanya bulunamadı");

            var draftCampaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == draftId && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                .Include(x=>x.CampaignDetail)
                .FirstOrDefaultAsync();
            if (draftCampaignEntity == null)
                throw new Exception("Kampanya bulunamadı");

            var approvedCampaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Code == draftCampaignEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .Include(x => x.CampaignDetail)
                .FirstOrDefaultAsync();

            response.CampaignDetail = _mapper.Map<CampaignDetailDto>(draftCampaignEntity.CampaignDetail);
            response.isNewRecord = approvedCampaignEntity == null;
            response.Campaign = _reportService.ConvertCampaignReportList(campaignQuery)[0];
            response.CampaignRule = await _campaignRuleService.GetCampaignRuleDto(draftId);
            response.CampaignTargetList = await _campaignTargetService.GetCampaignTargetDto(draftId, false);
            response.CampaignChannelCodeList = await _campaignChannelCodeService.GetCampaignChannelCodesAsString(draftId);
            response.CampaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(draftId);

            CampaignUpdatePages campaignUpdatePages = new CampaignUpdatePages();
            var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == draftId).FirstOrDefault();
            if (campaignUpdatePageEntity != null)
            {
                campaignUpdatePages.IsCampaignUpdated = campaignUpdatePageEntity.IsCampaignUpdated;
                campaignUpdatePages.IsCampaignRuleUpdated = campaignUpdatePageEntity.IsCampaignRuleUpdated;
                campaignUpdatePages.IsCampaignTargetUpdated = campaignUpdatePageEntity.IsCampaignTargetUpdated;
                campaignUpdatePages.IsCampaignChannelCodeUpdated = campaignUpdatePageEntity.IsCampaignChannelCodeUpdated;
                campaignUpdatePages.IsCampaignAchievementUpdated = campaignUpdatePageEntity.IsCampaignAchievementUpdated;
            }
            response.CampaignUpdatePages = campaignUpdatePages;

            CampaignUpdateFields campaignUpdateFields = new CampaignUpdateFields();
            if (!response.isNewRecord) 
            {
                if (campaignUpdatePages.IsCampaignUpdated) 
                {
                    campaignUpdateFields.IsNameUpdated = draftCampaignEntity.Name != approvedCampaignEntity.Name;
                    campaignUpdateFields.IsDescriptionTrUpdated = draftCampaignEntity.DescriptionTr != approvedCampaignEntity.DescriptionTr;
                    campaignUpdateFields.IsDescriptionEnUpdated = draftCampaignEntity.DescriptionEn != approvedCampaignEntity.DescriptionEn;
                    campaignUpdateFields.IsTitleTrUpdated = draftCampaignEntity.TitleTr != approvedCampaignEntity.TitleTr;
                    campaignUpdateFields.IsTitleEnUpdated = draftCampaignEntity.TitleEn != approvedCampaignEntity.TitleEn;
                    campaignUpdateFields.IsStartDateUpdated = draftCampaignEntity.StartDate != approvedCampaignEntity.StartDate;
                    campaignUpdateFields.IsEndDateUpdated = draftCampaignEntity.EndDate != approvedCampaignEntity.EndDate;
                    campaignUpdateFields.IsOrderUpdated = draftCampaignEntity.Order != approvedCampaignEntity.Order;
                    campaignUpdateFields.IsMaxNumberOfUserUpdated = draftCampaignEntity.MaxNumberOfUser != approvedCampaignEntity.MaxNumberOfUser;
                    campaignUpdateFields.IsIsActiveUpdated = draftCampaignEntity.IsActive != approvedCampaignEntity.IsActive;
                    campaignUpdateFields.IsIsBundleUpdated = draftCampaignEntity.IsBundle != approvedCampaignEntity.IsBundle;
                    campaignUpdateFields.IsIsContractUpdated = draftCampaignEntity.IsContract != approvedCampaignEntity.IsContract;
                    campaignUpdateFields.IsContractIdUpdated = draftCampaignEntity.ContractId != approvedCampaignEntity.ContractId;
                    campaignUpdateFields.IsSectorIdUpdated = draftCampaignEntity.SectorId != approvedCampaignEntity.SectorId;
                    campaignUpdateFields.IsViewOptionIdUpdated = draftCampaignEntity.ViewOptionId != approvedCampaignEntity.ViewOptionId;
                    campaignUpdateFields.IsProgramTypeIdUpdated = draftCampaignEntity.ProgramTypeId != approvedCampaignEntity.ProgramTypeId;
                    campaignUpdateFields.IsParticipationTypeIdUpdated = draftCampaignEntity.ParticipationTypeId != approvedCampaignEntity.ParticipationTypeId;

                    campaignUpdateFields.IsCampaignListImageUrlUpdated = draftCampaignEntity.CampaignDetail.CampaignListImageUrl != approvedCampaignEntity.CampaignDetail.CampaignListImageUrl;
                    campaignUpdateFields.IsCampaignDetailImageUrlUpdated = draftCampaignEntity.CampaignDetail.CampaignDetailImageUrl != approvedCampaignEntity.CampaignDetail.CampaignDetailImageUrl;
                    campaignUpdateFields.IsSummaryTrUpdated = draftCampaignEntity.CampaignDetail.SummaryTr != approvedCampaignEntity.CampaignDetail.SummaryTr;
                    campaignUpdateFields.IsSummaryEnUpdated = draftCampaignEntity.CampaignDetail.SummaryEn != approvedCampaignEntity.CampaignDetail.SummaryEn;
                    campaignUpdateFields.IsContentTrUpdated = draftCampaignEntity.CampaignDetail.ContentTr != approvedCampaignEntity.CampaignDetail.ContentTr;
                    campaignUpdateFields.IsContentEnUpdated = draftCampaignEntity.CampaignDetail.ContentEn != approvedCampaignEntity.CampaignDetail.ContentEn;
                    campaignUpdateFields.IsDetailTrUpdated = draftCampaignEntity.CampaignDetail.DetailTr != approvedCampaignEntity.CampaignDetail.DetailTr;
                    campaignUpdateFields.IsDetailEnUpdated = draftCampaignEntity.CampaignDetail.DetailEn != approvedCampaignEntity.CampaignDetail.DetailEn;
                }

                if (campaignUpdatePages.IsCampaignRuleUpdated) 
                {
                    var draftRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>().GetAll(x => x.CampaignId == draftCampaignEntity.Id && !x.IsDeleted).FirstOrDefaultAsync();
                    var approvedRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>().GetAll(x => x.CampaignId == approvedCampaignEntity.Id && !x.IsDeleted).FirstOrDefaultAsync();
                    
                    campaignUpdateFields.IsCampaignRuleStartTermIdUpdated = draftRuleEntity.CampaignStartTermId != approvedRuleEntity.CampaignStartTermId;
                    campaignUpdateFields.IsCampaignRuleJoinTypeIdUpdated = draftRuleEntity.JoinTypeId != approvedRuleEntity.JoinTypeId;
                    campaignUpdateFields.IsIsEmployeeIncludedUpdated = draftRuleEntity.IsEmployeeIncluded != approvedRuleEntity.IsEmployeeIncluded;
                    campaignUpdateFields.IsIsPrivateBankingUpdated = draftRuleEntity.IsPrivateBanking != approvedRuleEntity.IsPrivateBanking;

                    if(draftRuleEntity.JoinTypeId != approvedRuleEntity.JoinTypeId) 
                    {
                        switch (draftRuleEntity.JoinTypeId) 
                        {
                            case ((int)JoinTypeEnum.BusinessLine):
                                campaignUpdateFields.IsRuleBusinessLinesUpdated = true;
                                break;
                            case ((int)JoinTypeEnum.CustomerType):
                                campaignUpdateFields.IsRuleCustomerTypesUpdated = true;
                                break;
                            case ((int)JoinTypeEnum.Branch):
                                campaignUpdateFields.IsRuleBranchesUpdated = true;
                                break;
                            case ((int)JoinTypeEnum.Customer):
                                //campaignUpdateFields.
                                break; 
                            default:
                                break;
                        }
                    }
                    else 
                    {
                        switch (draftRuleEntity.JoinTypeId)
                        {
                            case ((int)JoinTypeEnum.BusinessLine):
                                campaignUpdateFields.IsRuleBusinessLinesUpdated = draftRuleEntity.BusinessLines.Select(x => x.BusinessLineId).ToList()
                                    .Except(approvedRuleEntity.BusinessLines.Select(x => x.BusinessLineId)).ToList().Any();
                                break;
                            case ((int)JoinTypeEnum.CustomerType):
                                campaignUpdateFields.IsRuleCustomerTypesUpdated = draftRuleEntity.CustomerTypes.Select(x=> x.CustomerTypeId).ToList()
                                    .Except(approvedRuleEntity.CustomerTypes.Select(x => x.CustomerTypeId)).ToList().Any();
                                break;
                            case ((int)JoinTypeEnum.Branch):
                                campaignUpdateFields.IsRuleBranchesUpdated = draftRuleEntity.Branches.Select(x => x.BranchCode).ToList()
                                    .Except(approvedRuleEntity.Branches.Select(x => x.BranchCode)).ToList().Any(); 
                                break;
                            case ((int)JoinTypeEnum.Customer):
                                //campaignUpdateFields.
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            response.CampaignUpdateFields = campaignUpdateFields;

            response.HistoryList = new List<HistoryApproveDto>();
            foreach (var campaignHistory in _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => x.Code == draftCampaignEntity.Code && x.StatusId == (int)StatusEnum.History && !x.IsDeleted).ToList())
            {
                HistoryApproveDto historyApproveDto = new HistoryApproveDto();
                historyApproveDto.ApprovedBy = campaignHistory.ApprovedBy;
                historyApproveDto.ApprovedDate = campaignHistory.ApprovedDate;
                DateTime _approvedDate = campaignHistory.ApprovedDate ?? DateTime.MinValue;
                if(_approvedDate != DateTime.MinValue)
                    historyApproveDto.ApprovedDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(_approvedDate);
                response.HistoryList.Add(historyApproveDto);
            }

            return await BaseResponse<CampaignApproveFormDto>.SuccessAsync(response);
        }

        #endregion

        #region approve toplimit
        public async Task<BaseResponse<TopLimitDto>> ApproveTopLimitAsync(int id, bool isApproved, UserRoleDto userRole) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Approve;
            int moduleTypeId = (int)ModuleTypeEnum.TopLimit;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);
            string userid = userRole.UserId;
            if (isApproved) 
            {
                var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                    .GetAll(x => x.Id == id && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (draftEntity == null)
                    throw new Exception("Çatı limiti bulunamadı");

                if (draftEntity.CreatedBy == userid)
                    throw new Exception("Çatı limiti kaydını oluşturan kullanıcı ile onaylayan kullanıcı aynı kişi olamaz.");

                var approvedEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                    .GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (approvedEntity == null)
                {
                    return await ApproveTopLimitAddAsync(id, userid);
                }
                else
                {
                    return await ApproveTopLimitUpdateAsync(id, approvedEntity.Id, userRole.UserId);
                }
            }
            else 
            {
                return await this.DisApproveTopLimitAsync(id, userRole.UserId);
            }
        }
        private async Task<BaseResponse<TopLimitDto>> ApproveTopLimitAddAsync(int id, string userid) 
        {
            DateTime now = DateTime.UtcNow;
            var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (draftEntity == null)
                throw new Exception("Çatı limiti bulunamadı.");

            //add history
            var historyEntity = new TopLimitEntity();
            historyEntity = await _draftService.CopyTopLimitInfo(id, historyEntity, userid, true, true, true, true, true);
            historyEntity.StatusId = (int)StatusEnum.History;
            historyEntity.ApprovedBy = userid;
            historyEntity.ApprovedDate = now;
            await _unitOfWork.GetRepository<TopLimitEntity>().AddAsync(historyEntity);

            foreach (var campaignTopLimit in await _draftService.CopyCampaignTopLimits(id, historyEntity, userid, true, true)) 
            {
                await _unitOfWork.GetRepository<CampaignTopLimitEntity>().AddAsync(campaignTopLimit);
            }

            //update draft top limit
            draftEntity.StatusId = (int)StatusEnum.Approved;
            draftEntity.ApprovedBy = userid;
            draftEntity.ApprovedDate = now;
            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(draftEntity);

            await _unitOfWork.SaveChangesAsync();

            return await _campaignTopLimitService.GetCampaignTopLimitAsync(draftEntity.Id);
        }
        private async Task<BaseResponse<TopLimitDto>> ApproveTopLimitUpdateAsync(int draftId, int topLimitId, string userid) 
        {
            DateTime now = DateTime.UtcNow;
            var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => x.Id == draftId && !x.IsDeleted).FirstOrDefaultAsync();
            var approvedEntity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => x.Id == topLimitId && !x.IsDeleted).FirstOrDefaultAsync();

            if (draftEntity == null || approvedEntity == null) 
                throw new Exception("Çatı limiti bulunamadı.");
            

            approvedEntity = await _draftService.CopyTopLimitInfo(draftId, approvedEntity, userid, true, true, true, true, true);
            approvedEntity.StatusId = (int)StatusEnum.Approved;
            approvedEntity.ApprovedDate = DateTime.UtcNow;
            approvedEntity.ApprovedBy = userid;
            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(approvedEntity);

            draftEntity.ApprovedBy = userid;
            draftEntity.ApprovedDate = now;
            draftEntity.StatusId = (int)StatusEnum.History;
            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(draftEntity);

            await _unitOfWork.SaveChangesAsync();
            var mappedEntity = _mapper.Map<TopLimitDto>(approvedEntity);
            return await BaseResponse<TopLimitDto>.SuccessAsync(mappedEntity);

        }
        private async Task<BaseResponse<TopLimitDto>> DisApproveTopLimitAsync(int id, string userid)
        {
            var entity = await _unitOfWork.GetRepository<TopLimitEntity>().GetByIdAsync(id); 
            if (entity is null)
                throw new Exception("Kampanya Çatı Limiti bulunamadı.");

            entity.StatusId = (int)StatusEnum.Draft;
            entity.LastModifiedBy = userid;

            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            var mappedEntity = _mapper.Map<TopLimitDto>(entity);
            return await BaseResponse<TopLimitDto>.SuccessAsync(mappedEntity);
        }
        public async Task<BaseResponse<TopLimitApproveFormDto>> GetTopLimitApprovalFormAsync(int id, UserRoleDto userRoleDto) 
        {
            TopLimitApproveFormDto response = new TopLimitApproveFormDto();

            var draftEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                .GetAll(x => x.Id == id && !x.IsDeleted && x.StatusId == (int)StatusEnum.SentToApprove)
                .Include(x => x.TopLimitCampaigns.Where(x => !x.IsDeleted)).FirstOrDefaultAsync();
            if(draftEntity == null)
                throw new Exception("Çatı limiti bulunamadı");
            var approvedEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                .GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .Include(x => x.TopLimitCampaigns.Where(x => !x.IsDeleted)).FirstOrDefaultAsync();

            response.isNewRecord = approvedEntity == null;

            response.TopLimit = await _campaignTopLimitService.GetTopLimitDto(id);

            TopLimitUpdateFields topLimitUpdateFields = new TopLimitUpdateFields();
            if (!response.isNewRecord) 
            {
                topLimitUpdateFields.IsNameUpdated = draftEntity.Name != approvedEntity.Name;
                topLimitUpdateFields.IsActiveUpdated = draftEntity.IsActive != approvedEntity.IsActive;
                topLimitUpdateFields.IsAchievementFrequencyIdUpdated = draftEntity.AchievementFrequencyId != approvedEntity.AchievementFrequencyId;
                topLimitUpdateFields.IsTypeIdUpdated = draftEntity.Type != approvedEntity.Type;
                topLimitUpdateFields.IsMaxTopLimitAmountUpdated = draftEntity.MaxTopLimitAmount != approvedEntity.MaxTopLimitAmount;
                topLimitUpdateFields.IsCurrencyIdUpdated = draftEntity.CurrencyId != approvedEntity.CurrencyId;
                topLimitUpdateFields.IsMaxTopLimitRateUpdated = draftEntity.MaxTopLimitRate != approvedEntity.MaxTopLimitRate;
                topLimitUpdateFields.IsMaxTopLimitUtilizationUpdated = draftEntity.MaxTopLimitUtilization != approvedEntity.MaxTopLimitUtilization;

                if (draftEntity.TopLimitCampaigns.Count != approvedEntity.TopLimitCampaigns.Count)
                    topLimitUpdateFields.IsTopLimitCampaignsUpdated = true;
                else 
                { 
                    foreach(var draftTopLimitCampaign in draftEntity.TopLimitCampaigns) 
                    { 
                        var approvedTopLimitCampaign = approvedEntity.TopLimitCampaigns.Where(x=>x.CampaignId == draftTopLimitCampaign.Id);
                        if(approvedTopLimitCampaign == null) 
                        {
                            topLimitUpdateFields.IsTopLimitCampaignsUpdated = true;
                            break;
                        }
                    }
                }
            }
            response.TopLimitUpdateFields = topLimitUpdateFields;

            response.HistoryList = new List<HistoryApproveDto>();
            foreach (var campaignHistory in _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.History && !x.IsDeleted).ToList())
            {
                HistoryApproveDto historyApproveDto = new HistoryApproveDto();
                historyApproveDto.ApprovedBy = campaignHistory.ApprovedBy;
                historyApproveDto.ApprovedDate = campaignHistory.ApprovedDate;
                DateTime _approvedDate = campaignHistory.ApprovedDate ?? DateTime.MinValue;
                if (_approvedDate != DateTime.MinValue)
                    historyApproveDto.ApprovedDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(_approvedDate);
                response.HistoryList.Add(historyApproveDto);
            }

            return await BaseResponse<TopLimitApproveFormDto>.SuccessAsync(response);
        }
        
        #endregion

        #region target
        public async Task<BaseResponse<TargetDto>> ApproveTargetAsync(int id, bool isApproved, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Approve;
            int moduleTypeId = (int)ModuleTypeEnum.Target;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            if (isApproved)
            {
                var draftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                    .GetAll(x => x.Id == id && x.StatusId == (int)StatusEnum.SentToApprove && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (draftEntity == null)
                    throw new Exception("Hedef bulunamadı");

                if (draftEntity.CreatedBy == userRole.UserId)
                    throw new Exception("Hedef kaydını oluşturan kullanıcı ile onaylayan kullanıcı aynı kişi olamaz.");

                var approvedEntity = await _unitOfWork.GetRepository<TargetEntity>()
                    .GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                    .FirstOrDefaultAsync();

                if (approvedEntity == null)
                {
                    return await ApproveTargetAddAsync(id, userRole.UserId);
                }
                else
                {
                    return await ApproveTargetUpdateAsync(id, approvedEntity.Id, userRole.UserId);
                }
            }
            else
            {
                return await this.DisApproveTargetAsync(id, userRole.UserId);
            }

        }

        private async Task<BaseResponse<TargetDto>> DisApproveTargetAsync(int id, string userid)
        {
            var entity = await _unitOfWork.GetRepository<TargetEntity>().GetByIdAsync(id);
            if (entity is null)
                throw new Exception("Hedef bulunamadı.");

            entity.StatusId = (int)StatusEnum.Draft;
            entity.LastModifiedOn = DateTime.UtcNow;
            entity.LastModifiedBy = userid;

            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            var mappedEntity = _mapper.Map<TargetDto>(entity);
            return await BaseResponse<TargetDto>.SuccessAsync(mappedEntity);
        }

        private async Task<BaseResponse<TargetDto>> ApproveTargetAddAsync(int id, string userid)
        {
            DateTime now = DateTime.UtcNow;
            var draftEntity = await _unitOfWork.GetRepository<TargetEntity>().GetAll(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (draftEntity == null)
                throw new Exception("Çatı limiti bulunamadı.");

            //add history
            var historyEntity = new TargetEntity();
            historyEntity.TargetDetail = new TargetDetailEntity();
            historyEntity = await _draftService.CopyTargetInfo(id, historyEntity, userid, true, true, true, true, true);
            historyEntity.StatusId = (int)StatusEnum.History;
            historyEntity.ApprovedBy = userid;
            historyEntity.ApprovedDate = now;
            await _unitOfWork.GetRepository<TargetEntity>().AddAsync(historyEntity);

            // update draft 
            draftEntity.StatusId = (int)StatusEnum.Approved;
            draftEntity.ApprovedBy = userid;
            draftEntity.ApprovedDate = now;
            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(draftEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedEntity = _mapper.Map<TargetDto>(draftEntity);

            return await BaseResponse<TargetDto>.SuccessAsync(mappedEntity);
        }
        private async Task<BaseResponse<TargetDto>> ApproveTargetUpdateAsync(int draftId, int targetId, string userid)
        {
            DateTime now = DateTime.UtcNow;
            var draftEntity = await _unitOfWork.GetRepository<TargetEntity>().GetAll(x => x.Id == draftId && !x.IsDeleted).FirstOrDefaultAsync();
            var approvedEntity = await _unitOfWork.GetRepository<TargetEntity>().GetAll(x => x.Id == targetId && !x.IsDeleted)
                .Include(x=>x.TargetDetail).FirstOrDefaultAsync();

            if (draftEntity == null || approvedEntity == null)
                throw new Exception("Hedef bulunamadı.");

            approvedEntity = await _draftService.CopyTargetInfo(draftId, approvedEntity, userid, true, true, true, true, true);
            approvedEntity.StatusId = (int)StatusEnum.Approved;
            approvedEntity.ApprovedDate = DateTime.UtcNow;
            approvedEntity.ApprovedBy = userid;
            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(approvedEntity);

            draftEntity.ApprovedBy = userid;
            draftEntity.ApprovedDate = now;
            draftEntity.StatusId = (int)StatusEnum.History;
            await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(draftEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedEntity = _mapper.Map<TargetDto>(draftEntity);

            return await BaseResponse<TargetDto>.SuccessAsync(mappedEntity);
        }
        public async Task<BaseResponse<TargetApproveFormDto>> GetTargetApprovalFormAsync(int id, UserRoleDto userRole)
        {
            TargetApproveFormDto response = new TargetApproveFormDto();

            var draftEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Id == id && !x.IsDeleted && x.StatusId == (int)StatusEnum.SentToApprove)
                .Include(x => x.TargetDetail).FirstOrDefaultAsync();
            if (draftEntity == null)
                throw new Exception("hedeef bulunamadı");
            var approvedEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .Include(x => x.TargetDetail).FirstOrDefaultAsync();
            response.isNewRecord = approvedEntity == null;

            response.Target = await _targetService.GetTargetDto2(id);
            response.TargetDetail = await _targetDetailService.GetTargetDetailDto(id);

            TargetUpdateFields targetUpdateFields = new TargetUpdateFields();
            if (!response.isNewRecord) 
            {
                targetUpdateFields.IsNameUpdated = draftEntity.Name != approvedEntity.Name;
                targetUpdateFields.IsTitleUpdated = draftEntity.Title != approvedEntity.Title;
                targetUpdateFields.IsIsActiveUpdated = draftEntity.IsActive != approvedEntity.IsActive;
                targetUpdateFields.IsTargetSourceIdUpdated = draftEntity.TargetDetail.TargetSourceId != approvedEntity.TargetDetail.TargetSourceId;
                targetUpdateFields.IsTargetViewTypeIdUpdated = draftEntity.TargetDetail.TargetViewTypeId != approvedEntity.TargetDetail.TargetViewTypeId;
                targetUpdateFields.IsTriggerTimeIdUpdated = draftEntity.TargetDetail.TriggerTimeId != approvedEntity.TargetDetail.TriggerTimeId;
                targetUpdateFields.IsVerificationTimeIdUpdated = draftEntity.TargetDetail.VerificationTime != approvedEntity.TargetDetail.VerificationTime;
                targetUpdateFields.IsFlowNameUpdated = draftEntity.TargetDetail.VerificationTimeId != approvedEntity.TargetDetail.VerificationTimeId;
                targetUpdateFields.IsTargetDetailEnUpdated = draftEntity.TargetDetail.TargetDetailEn != approvedEntity.TargetDetail.TargetDetailEn;
                targetUpdateFields.IsTargetDetailTrUpdated = draftEntity.TargetDetail.TargetDetailTr != approvedEntity.TargetDetail.TargetDetailTr;
                targetUpdateFields.IsDescriptionEnUpdated = draftEntity.TargetDetail.DescriptionEn != approvedEntity.TargetDetail.DescriptionEn;
                targetUpdateFields.IsDescriptionTrUpdated = draftEntity.TargetDetail.DescriptionTr != approvedEntity.TargetDetail.DescriptionTr;
                targetUpdateFields.IsTotalAmountUpdated = draftEntity.TargetDetail.TotalAmount != approvedEntity.TargetDetail.TotalAmount;
                targetUpdateFields.IsNumberOfTransactionUpdated = draftEntity.TargetDetail.NumberOfTransaction != approvedEntity.TargetDetail.NumberOfTransaction;
                targetUpdateFields.IsFlowFrequencyUpdated = draftEntity.TargetDetail.FlowFrequency != approvedEntity.TargetDetail.FlowFrequency;
                targetUpdateFields.IsAdditionalFlowTimeUpdated = draftEntity.TargetDetail.AdditionalFlowTime != approvedEntity.TargetDetail.AdditionalFlowTime;
                targetUpdateFields.IsQueryUpdated = draftEntity.TargetDetail.Query != approvedEntity.TargetDetail.Query;
                targetUpdateFields.IsConditionUpdated = draftEntity.TargetDetail.Condition != approvedEntity.TargetDetail.Condition;
            }
            response.TargetUpdateFields = targetUpdateFields;

            List <HistoryApproveDto> historyList = new List<HistoryApproveDto>();
            foreach (var historyEntity in _unitOfWork.GetRepository<TargetEntity>().GetAll(x => x.Code == draftEntity.Code && x.StatusId == (int)StatusEnum.History && !x.IsDeleted).ToList())
            {
                HistoryApproveDto historyApproveDto = new HistoryApproveDto();
                historyApproveDto.ApprovedBy = historyEntity.ApprovedBy;
                historyApproveDto.ApprovedDate = historyEntity.ApprovedDate;
                DateTime _approvedDate = historyEntity.ApprovedDate ?? DateTime.MinValue;
                if (_approvedDate != DateTime.MinValue)
                    historyApproveDto.ApprovedDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(_approvedDate);
                historyList.Add(historyApproveDto);
            }
            response.HistoryList = historyList;

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

        public async Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int id, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            return await _draftService.CreateCampaignCopyAsync(id, userRole.UserId);
        }
        

        public async Task<BaseResponse<TopLimitDto>> TopLimitCopyAsync(int refId, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.TopLimit;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);
            
            TopLimitEntity entity = new TopLimitEntity();
            entity = await _draftService.CopyTopLimitInfo(refId, entity, userRole.UserId, false, false, false, false, false);
            entity.Name = entity.Name + "-Copy";
            await _unitOfWork.GetRepository<TopLimitEntity>().AddAsync(entity);

            foreach (var campaignTopLimit in await _draftService.CopyCampaignTopLimits(refId, entity, userRole.UserId, true, true))
            {
                await _unitOfWork.GetRepository<CampaignTopLimitEntity>().AddAsync(campaignTopLimit);
            }


            await _unitOfWork.SaveChangesAsync();

            var mappedTopLimit = _mapper.Map<TopLimitDto>(entity);
            return await BaseResponse<TopLimitDto>.SuccessAsync(mappedTopLimit);
        }

        public async Task<BaseResponse<TargetDto>> TargetCopyAsync(int refId, UserRoleDto userRole) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Target;
            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            TargetEntity entity = new TargetEntity();
            entity.TargetDetail = new TargetDetailEntity();
            entity = await _draftService.CopyTargetInfo(refId, entity, userRole.UserId, false, false, false, false, false);
            entity.Name = entity.Name + "-Copy";

            await _unitOfWork.GetRepository<TargetEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var mappedEntity = _mapper.Map<TargetDto>(entity);

            return await BaseResponse<TargetDto>.SuccessAsync(mappedEntity);
        }

        #endregion

    }
}
