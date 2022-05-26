using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
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
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignTarget;
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
        private readonly ICampaignService _campaignService;
        private readonly ICampaignRuleService _campaignRuleService;
        private readonly ICampaignTargetService _campaignTargetService;

        public DraftService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
        }
        public async Task<int> CreateCampaignDraft(
            int campaignId,
            int pageTypeId,
            string userid,
            CampaignUpdateRequest campaignUpdateRequest, 
            AddCampaignRuleRequest campaignRuleUpdateRequest, 
            CampaignTargetInsertRequest campaignTargetInsertRequest, 
            CampaignAchievementInsertRequest campaignAchievementInsertRequest
            ) 
        {
            Dictionary<string, int> returnList = new Dictionary<string, int>();

            //campaign
            CampaignEntity campaignEntity = new CampaignEntity();
            CampaignDetailEntity campaignDetailEntity = new CampaignDetailEntity();
            bool isCampaignUpdate = pageTypeId == (int)PageTypesEnum.Campaign;
            var campaignReferenceEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                    .Include(x => x.CampaignDetail)
                    .FirstOrDefaultAsync();
            if(campaignReferenceEntity == null) 
            { 
                if(pageTypeId != (int)PageTypesEnum.Campaign)
                    throw new Exception("Kampanya bulunamadı.");
            }

            //campaign detail
            campaignDetailEntity.DetailEn = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.DetailEn : campaignReferenceEntity.CampaignDetail.DetailEn;
            campaignDetailEntity.DetailTr = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.DetailTr : campaignReferenceEntity.CampaignDetail.DetailTr;
            campaignDetailEntity.SummaryEn = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.SummaryEn : campaignReferenceEntity.CampaignDetail.SummaryEn;
            campaignDetailEntity.SummaryTr = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.SummaryTr : campaignReferenceEntity.CampaignDetail.SummaryTr;
            campaignDetailEntity.CampaignDetailImageUrl = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.CampaignDetailImageUrl : campaignReferenceEntity.CampaignDetail.CampaignDetailImageUrl;
            campaignDetailEntity.CampaignListImageUrl = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.CampaignListImageUrl : campaignReferenceEntity.CampaignDetail.CampaignListImageUrl;
            campaignDetailEntity.ContentTr = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.ContentTr : campaignReferenceEntity.CampaignDetail.ContentTr;
            campaignDetailEntity.ContentEn = isCampaignUpdate ? campaignUpdateRequest.CampaignDetail.ContentEn : campaignReferenceEntity.CampaignDetail.ContentEn;
            campaignDetailEntity.CreatedBy = userid;

            campaignEntity.CampaignDetail = campaignDetailEntity;
            campaignEntity.ContractId = isCampaignUpdate ? campaignUpdateRequest.ContractId : campaignReferenceEntity.ContractId;
            campaignEntity.ProgramTypeId = isCampaignUpdate ? campaignUpdateRequest.ProgramTypeId : campaignReferenceEntity.ProgramTypeId;
            campaignEntity.SectorId = isCampaignUpdate ? campaignUpdateRequest.SectorId : campaignReferenceEntity.SectorId;
            campaignEntity.ViewOptionId = isCampaignUpdate ? campaignUpdateRequest.ViewOptionId : campaignReferenceEntity.ViewOptionId;
            campaignEntity.IsBundle = isCampaignUpdate ? campaignUpdateRequest.IsBundle : campaignReferenceEntity.IsBundle;
            campaignEntity.IsActive = isCampaignUpdate ? campaignUpdateRequest.IsActive : campaignReferenceEntity.IsActive;
            campaignEntity.IsContract = isCampaignUpdate ? campaignUpdateRequest.IsContract : campaignReferenceEntity.IsContract;
            campaignEntity.DescriptionTr = isCampaignUpdate ? campaignUpdateRequest.DescriptionTr : campaignReferenceEntity.DescriptionTr;
            campaignEntity.DescriptionEn = isCampaignUpdate ? campaignUpdateRequest.DescriptionEn : campaignReferenceEntity.DescriptionEn;
            campaignEntity.EndDate = isCampaignUpdate ? DateTime.Parse(campaignUpdateRequest.EndDate) : campaignReferenceEntity.EndDate;
            campaignEntity.StartDate = isCampaignUpdate ? DateTime.Parse(campaignUpdateRequest.StartDate) : campaignReferenceEntity.StartDate;
            campaignEntity.Name = isCampaignUpdate ? campaignUpdateRequest.Name : campaignReferenceEntity.Name;
            campaignEntity.Order = isCampaignUpdate ? campaignUpdateRequest.Order : campaignReferenceEntity.Order;
            campaignEntity.TitleTr = isCampaignUpdate ? campaignUpdateRequest.TitleTr : campaignReferenceEntity.TitleTr;
            campaignEntity.TitleEn = isCampaignUpdate ? campaignUpdateRequest.TitleEn : campaignReferenceEntity.TitleEn;
            campaignEntity.MaxNumberOfUser = isCampaignUpdate ? campaignUpdateRequest.MaxNumberOfUser : campaignReferenceEntity.MaxNumberOfUser;
            campaignEntity.Code = isCampaignUpdate ? campaignUpdateRequest.Id.ToString() : campaignReferenceEntity.Code;
            campaignEntity.IsDraft = true;
            campaignEntity.IsApproved = false;
            campaignEntity.ParticipationTypeId = isCampaignUpdate ? campaignUpdateRequest.ParticipationTypeId : campaignReferenceEntity.ParticipationTypeId;
            campaignEntity.CreatedBy = userid;

            if(isCampaignUpdate)
                campaignEntity = await SetCampaignDefaults(campaignEntity);

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

           
            
            //campaign rule
            var campaignRuleReferenceEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .Include(x => x.Branches.Where(t => t.IsDeleted != true))
                .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
                .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
                .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();
            bool isRuleUpdate = pageTypeId == (int)PageTypesEnum.CampaignRule;
            if (campaignRuleReferenceEntity == null)
            {
                if (pageTypeId != (int)PageTypesEnum.CampaignRule)
                    throw new Exception("Kampanya kuralı bulunamadı.");
            }

            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();
            campaignRuleEntity.Campaign = campaignEntity;
            campaignRuleEntity.CampaignStartTermId = isRuleUpdate ? campaignRuleUpdateRequest.StartTermId : campaignRuleReferenceEntity.CampaignStartTermId;
            campaignRuleEntity.JoinTypeId = isRuleUpdate ? campaignRuleUpdateRequest.JoinTypeId : campaignRuleReferenceEntity.JoinTypeId;
            campaignRuleEntity.CreatedBy = userid;


            try { await _unitOfWork.SaveChangesAsync(); }
            catch(Exception ex) 
            { 
                string s = ex.ToString();
            }
            

            //CampaignRuleEntity campaignRuleEntity = await CreateCampaignRule(campaignId, campaignPageId, campaignEntity, CreateCampaignRuleRequest);

            return campaignEntity.Id;
        }


        public async Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int campaignId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            int moduleTypeId = (int)ModuleTypeEnum.Campaign;

            //await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignEntity campaignEntity  = await CreateCampaign(campaignId, userid);
            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            CampaignRuleEntity campaignRuleEntity = await CreateCampaignRule(campaignId, campaignEntity, userid);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            List<CampaignDocumentEntity> campaignDocumentlist = await CreateCampaignDocument(campaignId, campaignEntity, userid);
            foreach(var campaignDocument in campaignDocumentlist)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            CampaignTargetEntity campaignTargetEntity = await CreateCampaignTarget(campaignId, campaignEntity, userid);
            await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetEntity);

            List<CampaignChannelCodeEntity> campaignChannelCodeList = await CreateCampaignChannelCode(campaignId, campaignEntity, userid);
            foreach(var campaignChannelCode in campaignChannelCodeList)
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            await CreateCampaignAchievement(campaignId, campaignEntity, userid);

            await _unitOfWork.SaveChangesAsync();

            campaignEntity.Code = campaignEntity.Id.ToString();
            //campaignEntity.campaignId = campaignEntity.Id;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);

            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }

        private async Task<CampaignEntity> CreateCampaign(int campaignId, string userid) 
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
            campaignEntity.Name = campaignDraftEntity.Name + "-Copy";
            campaignEntity.Code = string.Empty;
            campaignEntity.Order = null;
            campaignEntity.IsApproved = false;
            campaignEntity.IsDraft = true;
            campaignEntity.RefId = null;
            campaignEntity.CreatedBy = userid;

            //campaign detail
            var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
            var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
            campaignDetailEntity.Id = 0;
            campaignDetailEntity.CreatedBy = userid;

            campaignEntity.CampaignDetail = campaignDetailEntity;

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            return campaignEntity;

        }
        private async Task<CampaignRuleEntity> CreateCampaignRule(int campaignId, CampaignEntity campaignEntity, string userid)
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

            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity()
            {
                Campaign = campaignEntity,
                CampaignStartTermId = campaignRuleDraftEntity.CampaignStartTermId,
                JoinTypeId = campaignRuleDraftEntity.JoinTypeId,
                CreatedBy = userid,
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
                            BranchName = branch.BranchName,
                            CreatedBy = userid,
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
                            CreatedBy = userid,
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
                            CreatedBy = userid,
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
                        CreatedBy = userid,
                    });
                }
            }

            

            return campaignRuleEntity;
        }
        private async Task<List<CampaignDocumentEntity>> CreateCampaignDocument(int campaignId, CampaignEntity campaignEntity, string userid)
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
                    CreatedBy = userid,
                });
            }

            return campaignDocumentlist;
        }
        private async Task<CampaignTargetEntity> CreateCampaignTarget(int campaignId, CampaignEntity campaignEntity, string userid)
        {
            CampaignTargetEntity campaignTargetEntity = new CampaignTargetEntity();
            List<CampaignTargetEntity> campaignTargetDraftList = _unitOfWork.GetRepository<CampaignTargetEntity>()
                     .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
                     .ToList();
            if (campaignTargetDraftList.Count == 0)
                throw new Exception("Kampanya hedefleri bulunamadı.");

            foreach (var campaignTargetDraftEntity in campaignTargetDraftList)
            {
                var campaignTargetDto = _mapper.Map<CampaignTargetDto>(campaignTargetDraftEntity);
                campaignTargetEntity = _mapper.Map<CampaignTargetEntity>(campaignTargetDto);
                campaignTargetEntity.Id = 0;
                campaignTargetEntity.Campaign = campaignEntity;
                campaignTargetEntity.TargetId = campaignTargetDraftEntity.TargetId;
                campaignTargetEntity.TargetGroupId = campaignTargetDraftEntity.TargetGroupId;
                campaignTargetEntity.TargetOperationId = campaignTargetDraftEntity.TargetOperationId;
                campaignTargetEntity.CreatedBy = userid;
            }

            return campaignTargetEntity;
        }
        
        private async Task<List<CampaignChannelCodeEntity>> CreateCampaignChannelCode(int campaignId, CampaignEntity campaignEntity, string userid)
        {
            List <CampaignChannelCodeEntity> campaignChannelCodeList = new List <CampaignChannelCodeEntity>();
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
                    CreatedBy = userid,
                });
            }

            return campaignChannelCodeList;
        }
        private async Task<SuccessDto> CreateCampaignAchievement(int campaignId, CampaignEntity campaignEntity, string userid)
        {
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
                campaignAchievementEntity.CreatedBy = userid;

                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievementEntity);

            }
            return new SuccessDto() { IsSuccess = true };
        }



        private async Task<CampaignEntity> CreateCampaign(int refCampaignId, int campaignPageId, CampaignUpdateRequest request) 
        {
            CampaignEntity campaignEntity = new CampaignEntity();
            if (campaignPageId == (int)PageTypesEnum.Campaign)
            {
                var campaignDto = _mapper.Map<CampaignDto>(request);
                campaignDto.Id = 0;
                campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);
            }
            else
            {
                var campaignDraftEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == refCampaignId && !x.IsDeleted)
                    .Include(x => x.CampaignDetail)
                    .FirstOrDefaultAsync();
                if (campaignDraftEntity == null)
                    throw new Exception("Kampanya bulunamadı.");

                var campaignDto = _mapper.Map<CampaignDto>(campaignDraftEntity);
                campaignDto.Id = 0;
                campaignDto.Name = campaignDraftEntity.Name;
                campaignDto.Code = string.Empty;
                campaignDto.Order = null;
                campaignDto.IsApproved = false;
                campaignDto.IsDraft = true;
                campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);

                //campaign detail
                var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
                var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
                campaignDetailEntity.Id = 0;
                campaignEntity.CampaignDetail = campaignDetailEntity;
            }

            campaignEntity = await SetCampaignDefaults(campaignEntity);

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            return campaignEntity;
        }

        //private async Task<CampaignRuleEntity> CreateCampaignRule(int refCampaignId, int campaignPageId, CampaignEntity campaignEntity, CreateCampaignRuleRequest request)
        //{
        //    CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();

        //    if (campaignPageId == (int)PageTypesEnum.CampaignRule) //update sayfasından geliyorsa, requesti kopyala
        //    {
        //        campaignRuleEntity.Campaign = campaignEntity;
        //        campaignRuleEntity.CampaignStartTermId = request.StartTermId;
        //        campaignRuleEntity.JoinTypeId = request.JoinTypeId;

        //        if (request.JoinTypeId == (int)JoinTypeEnum.Customer)
        //        {
        //            if (request.IsSingleIdentity)
        //            {
        //                var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
        //                {
        //                    Identities = request.Identity.Trim(),
        //                    CampaignRule = campaignRuleEntity,
        //                };

        //                await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
        //            }
        //            else if (request.File != null)
        //            {
        //                byte[] bytesList = System.Convert.FromBase64String(request.File);

        //                var memoryStream = new MemoryStream(bytesList);

        //                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
        //                {
        //                    Campaign = campaignEntity,
        //                    DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN,
        //                    MimeType = MimeTypeExtensions.ToMimeType(".xlsx"),
        //                    Content = bytesList,
        //                    DocumentName = request.Identity
        //                });


        //                using (var excelWorkbook = new XLWorkbook(memoryStream))
        //                {
        //                    var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

        //                    //List<string> identityList = new List<string>();

        //                    foreach (var dataRow in nonEmptyDataRows)
        //                    {
        //                        string identity = dataRow.Cell(1).Value == null ? string.Empty : dataRow.Cell(1).Value.ToString().Trim();

        //                        //if(identityList.Contains(identity))
        //                        //    throw new Exception("Dosya içerisinde bazı kayıtlar çoklanmış.");

        //                        //identityList.Add(identity);

        //                        //CheckSingleIdentiy(identity);
        //                        if (!await IsValidIdentiy(identity))
        //                            continue;

        //                        var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
        //                        {
        //                            Identities = identity,
        //                            CampaignRule = campaignRuleEntity,
        //                        };

        //                        await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
        //                    }
        //                }
        //            }
        //        }
        //        else if (request.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
        //        {
        //            if (request.BusinessLines is { Count: > 0 })
        //            {
        //                request.BusinessLines.ForEach(x =>
        //                {
        //                    var campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity()
        //                    {
        //                        CampaignRule = campaignRuleEntity,
        //                        BusinessLineId = x,
        //                    };
        //                    _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
        //                });
        //            }
        //        }
        //        else if (request.JoinTypeId == (int)JoinTypeEnum.Branch)
        //        {
        //            if (request.Branches.Any())
        //            {
        //                request.Branches.ForEach(x =>
        //                {
        //                    var campaignRuleBranchEntity = new CampaignRuleBranchEntity()
        //                    {
        //                        CampaignRule = campaignRuleEntity,
        //                        BranchCode = x,
        //                        BranchName = ""
        //                    };
        //                    _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
        //                });
        //            }
        //        }
        //        else if (request.JoinTypeId == (int)JoinTypeEnum.CustomerType)
        //        {
        //            if (request.CustomerTypes is { Count: > 0 })
        //            {
        //                request.CustomerTypes.ForEach(x =>
        //                {
        //                    var campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity()
        //                    {
        //                        CampaignRule = campaignRuleEntity,
        //                        CustomerTypeId = x,
        //                    };
        //                    _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
        //                });
        //            }
        //        }
        //    }
        //    else 
        //    {
        //        var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
        //            .GetAll(x => x.CampaignId == refCampaignId && x.IsDeleted != true)
        //            .Include(x => x.Branches.Where(t => t.IsDeleted != true))
        //            .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
        //            .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
        //            .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
        //            .FirstOrDefaultAsync();
        //        if (campaignRuleDraftEntity == null)
        //            throw new Exception("Kampanya kuralı bulunamadı.");

        //        if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Branch)
        //        {
        //            if (campaignRuleDraftEntity.Branches.Any())
        //            {
        //                campaignRuleEntity.Branches = new List<CampaignRuleBranchEntity>();
        //                foreach (var branch in campaignRuleDraftEntity.Branches)
        //                {
        //                    campaignRuleEntity.Branches.Add(new CampaignRuleBranchEntity()
        //                    {
        //                        BranchCode = branch.BranchCode,
        //                        BranchName = branch.BranchName
        //                    });
        //                }
        //            }
        //        }
        //        else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.CustomerType)
        //        {
        //            if (campaignRuleDraftEntity.CustomerTypes.Any())
        //            {
        //                campaignRuleEntity.CustomerTypes = new List<CampaignRuleCustomerTypeEntity>();
        //                foreach (var customerType in campaignRuleDraftEntity.CustomerTypes)
        //                {
        //                    campaignRuleEntity.CustomerTypes.Add(new CampaignRuleCustomerTypeEntity()
        //                    {
        //                        CustomerTypeId = customerType.CustomerTypeId
        //                    });
        //                }
        //            }
        //        }
        //        else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
        //        {
        //            if (campaignRuleDraftEntity.BusinessLines.Any())
        //            {
        //                campaignRuleEntity.BusinessLines = new List<CampaignRuleBusinessLineEntity>();
        //                foreach (var businessLine in campaignRuleDraftEntity.BusinessLines)
        //                {
        //                    campaignRuleEntity.BusinessLines.Add(new CampaignRuleBusinessLineEntity()
        //                    {
        //                        BusinessLineId = businessLine.BusinessLineId
        //                    });
        //                }
        //            }
        //        }
        //        else if (campaignRuleDraftEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
        //        {
        //            campaignRuleEntity.RuleIdentities = new List<CampaignRuleIdentityEntity>();
        //            foreach (var ruleIdentity in campaignRuleDraftEntity.RuleIdentities)
        //            {
        //                campaignRuleEntity.RuleIdentities.Add(new CampaignRuleIdentityEntity()
        //                {
        //                    Identities = ruleIdentity.Identities,
        //                });
        //            }
        //        }
        //    }
        //    await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

        //    return campaignRuleEntity;
        //}

        private async Task<bool> IsValidIdentiy(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrEmpty(identity))
                return false;

            identity = identity.Trim();

            if (identity.Trim().Length > 11 || identity.Trim().Length < 10)
                return false;

            return identity.Length == 11 ? Core.Helper.Helpers.TcAuthentication(identity) : Core.Helper.Helpers.FirmaVergiKontrol(identity);
        }

        public async Task<CampaignEntity> SetCampaignDefaults(CampaignEntity entity)
        {
            if (entity.ProgramTypeId == (int)ProgramTypeEnum.Loyalty)
            {
                entity.ViewOptionId = null;
                entity.SectorId = null;
            }

            if (entity.ViewOptionId == (int)ViewOptionsEnum.InvisibleCampaign)
            {
                entity.TitleTr = null; //Başlık(Türkçe)
                entity.TitleEn = null;

                //İçerik
                entity.CampaignDetail.ContentTr = null;
                entity.CampaignDetail.ContentEn = null;

                //detay
                entity.CampaignDetail.DetailTr = null;
                entity.CampaignDetail.DetailEn = null;

                //liste ve detay görseli
                entity.CampaignDetail.CampaignListImageUrl = null;
                entity.CampaignDetail.CampaignDetailImageUrl = null;
            }

            //ContentTr boş ise, ContentEn boştur
            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.ContentTr) || string.IsNullOrEmpty(entity.CampaignDetail.ContentTr))
            {
                entity.CampaignDetail.ContentTr = null;
                entity.CampaignDetail.ContentEn = null;
            }

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.DetailTr) || string.IsNullOrEmpty(entity.CampaignDetail.DetailTr))
            {
                entity.CampaignDetail.DetailEn = null;
                entity.CampaignDetail.DetailTr = null;
            }

            if (entity.IsBundle || !entity.IsActive)
                entity.Order = null;

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.CampaignListImageUrl) ||
                string.IsNullOrEmpty(entity.CampaignDetail.CampaignListImageUrl))
                entity.CampaignDetail.CampaignListImageUrl = StaticValues.CampaignListImageUrlDefault;

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.CampaignDetailImageUrl) ||
                string.IsNullOrEmpty(entity.CampaignDetail.CampaignDetailImageUrl))
                entity.CampaignDetail.CampaignDetailImageUrl = StaticValues.CampaignDetailImageUrlDefault;

            return entity;
        }

    }
}
