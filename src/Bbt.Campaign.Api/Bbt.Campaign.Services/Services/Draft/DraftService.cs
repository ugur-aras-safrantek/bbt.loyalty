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
        public async Task<int> CreateCampaignDraft(int campaignId, int pageTypeId, string userid, CampaignUpdateRequest campaignUpdateRequest, AddCampaignRuleRequest campaignRuleUpdateRequest, CampaignTargetInsertRequest campaignTargetInsertRequest,
            CampaignChannelCodeUpdateRequest campaignChannelCodeUpdateRequest, CampaignAchievementInsertRequest campaignAchievementInsertRequest
            )
        {
            var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                .Where(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .FirstOrDefault();
            if (approvedCampaign == null)
                throw new Exception("Kampanya bulunamadı");

            //copy campaign
            CampaignEntity campaignEntity;
            if (pageTypeId == (int)PageTypesEnum.Campaign)
            {
                campaignEntity = new CampaignEntity();
                campaignEntity = SetCampaignUpdateRequest(campaignEntity, campaignUpdateRequest);
                campaignEntity.Code = approvedCampaign.Code;
                campaignEntity.CampaignDetail.CreatedBy = userid;
                campaignEntity.CreatedBy = userid;
                campaignEntity = SetCampaignDefaults(campaignEntity);
            }
            else
            {
                campaignEntity = await CopyCampaignInfo(campaignId, userid);
            }
            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            CampaignRuleEntity campaignRuleEntity;
            if (pageTypeId == (int)PageTypesEnum.CampaignRule)
            {
                campaignRuleEntity = new CampaignRuleEntity();

                campaignRuleEntity.Campaign = campaignEntity;
                campaignRuleEntity.CampaignStartTermId = campaignRuleUpdateRequest.StartTermId;
                campaignRuleEntity.JoinTypeId = campaignRuleUpdateRequest.JoinTypeId;
                campaignRuleEntity.CreatedBy = userid;

                if (campaignRuleUpdateRequest.JoinTypeId == (int)JoinTypeEnum.Customer)
                {
                    if (campaignRuleUpdateRequest.IsSingleIdentity)
                    {
                        var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
                        {
                            Identities = campaignRuleUpdateRequest.Identity.Trim(),
                            CampaignRule = campaignRuleEntity,
                            CreatedBy = userid,
                        };

                        await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                    }
                    else if (campaignRuleUpdateRequest.File != null)
                    {
                        byte[] bytesList = System.Convert.FromBase64String(campaignRuleUpdateRequest.File);

                        var memoryStream = new MemoryStream(bytesList);

                        await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                        {
                            Campaign = campaignEntity,
                            DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN,
                            MimeType = MimeTypeExtensions.ToMimeType(".xlsx"),
                            Content = bytesList,
                            DocumentName = campaignRuleUpdateRequest.Identity,
                            CreatedBy = userid,
                        });


                        using (var excelWorkbook = new XLWorkbook(memoryStream))
                        {
                            var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

                            //List<string> identityList = new List<string>();

                            foreach (var dataRow in nonEmptyDataRows)
                            {
                                string identity = dataRow.Cell(1).Value == null ? string.Empty : dataRow.Cell(1).Value.ToString().Trim();

                                //if(identityList.Contains(identity))
                                //    throw new Exception("Dosya içerisinde bazı kayıtlar çoklanmış.");

                                //identityList.Add(identity);

                                //CheckSingleIdentiy(identity);
                                if (!await IsValidIdentiy(identity))
                                    continue;

                                var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
                                {
                                    Identities = identity,
                                    CampaignRule = campaignRuleEntity,
                                    CreatedBy = userid,
                                };

                                await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                            }
                        }
                    }
                }
                else if (campaignRuleUpdateRequest.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
                {
                    if (campaignRuleUpdateRequest.BusinessLines is { Count: > 0 })
                    {
                        campaignRuleUpdateRequest.BusinessLines.ForEach(x =>
                        {
                            var campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                BusinessLineId = x,
                                CreatedBy = userid,
                            };
                            _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
                        });
                    }
                }
                else if (campaignRuleUpdateRequest.JoinTypeId == (int)JoinTypeEnum.Branch)
                {
                    if (campaignRuleUpdateRequest.Branches.Any())
                    {
                        campaignRuleUpdateRequest.Branches.ForEach(x =>
                        {
                            var campaignRuleBranchEntity = new CampaignRuleBranchEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                BranchCode = x,
                                BranchName = "",
                                CreatedBy = userid,
                            };
                            _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
                        });
                    }
                }
                else if (campaignRuleUpdateRequest.JoinTypeId == (int)JoinTypeEnum.CustomerType)
                {
                    if (campaignRuleUpdateRequest.CustomerTypes is { Count: > 0 })
                    {
                        campaignRuleUpdateRequest.CustomerTypes.ForEach(x =>
                        {
                            var campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                CustomerTypeId = x,
                                CreatedBy = userid,
                                LastModifiedBy = userid,
                            };
                            _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
                        });
                    }
                }

            }
            else
            {
                campaignRuleEntity = await CopyCampaignRuleInfo(campaignId, campaignEntity, userid);
            }
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            List<CampaignDocumentEntity> campaignDocumentlist = await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid);
            foreach (var campaignDocument in campaignDocumentlist)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            CampaignTargetEntity campaignTargetEntity = await CopyCampaignTargetInfo(campaignId, campaignEntity, userid);
            await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetEntity);

            List<CampaignChannelCodeEntity> campaignChannelCodeList;
            if (pageTypeId == (int)PageTypesEnum.ChannelCode)
            {
                campaignChannelCodeList = new List<CampaignChannelCodeEntity>();
                campaignChannelCodeUpdateRequest.CampaignChannelCodeList.ForEach(x =>
                {
                    campaignChannelCodeList.Add(new CampaignChannelCodeEntity()
                    {
                        Campaign = campaignEntity,
                        ChannelCode = x,
                        CreatedBy = userid,
                    });
                });
            }
            else
            {
                campaignChannelCodeList = await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid);
            }
            foreach (var campaignChannelCode in campaignChannelCodeList)
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            List<CampaignAchievementEntity> campaignAchievementList = await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid);
            if (pageTypeId == (int)PageTypesEnum.CampaignAchievement)
            {
                foreach (var x in campaignAchievementInsertRequest.CampaignAchievementList)
                {
                    CampaignAchievementEntity campaignAchievementEntity = new CampaignAchievementEntity();

                    campaignAchievementEntity.Campaign = campaignEntity;
                    campaignAchievementEntity.CurrencyId = x.CurrencyId;
                    campaignAchievementEntity.Amount = x.Amount;
                    campaignAchievementEntity.Rate = x.Rate;
                    campaignAchievementEntity.MaxAmount = x.MaxAmount;
                    campaignAchievementEntity.MaxUtilization = x.MaxUtilization;
                    campaignAchievementEntity.AchievementTypeId = x.AchievementTypeId;
                    campaignAchievementEntity.ActionOptionId = x.ActionOptionId;
                    campaignAchievementEntity.DescriptionTr = x.DescriptionTr;
                    campaignAchievementEntity.DescriptionEn = x.DescriptionEn;
                    campaignAchievementEntity.TitleTr = x.TitleTr;
                    campaignAchievementEntity.TitleEn = x.TitleEn;
                    campaignAchievementEntity.Type = x.Type == (int)AchievementType.Amount ? AchievementType.Amount : AchievementType.Rate;
                    campaignAchievementEntity.CreatedBy = userid;

                    //#region defaults

                    //if (entity.Type == AchievementType.Amount)
                    //{
                    //    entity.Rate = null;
                    //}
                    //else if (entity.Type == AchievementType.Rate)
                    //{
                    //    entity.Amount = null;
                    //    entity.CurrencyId = null;
                    //    entity.MaxAmount = null;
                    //}

                    //var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(entity.CampaignId);
                    //if ((campaignEntity.ViewOptionId ?? 0))
                    //{
                    //    int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                    //    if (viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign)
                    //    {
                    //        entity.DescriptionTr = null;
                    //        entity.DescriptionEn = null;
                    //        entity.TitleTr = null;
                    //        entity.TitleEn = null;
                    //    }
                    //}

                    //#endregion

                    campaignAchievementList.Add(campaignAchievementEntity);
                }

            }
            else
            {
                campaignAchievementList = await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid);
            }
            foreach (var campaignAchievement in campaignAchievementList)
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            try { await _unitOfWork.SaveChangesAsync(); }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

            return campaignEntity.Id;
        }


        public async Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int campaignId, string userid)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            //int moduleTypeId = (int)ModuleTypeEnum.Campaign;

            //await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignEntity campaignEntity = await CopyCampaignInfo(campaignId, userid);
            campaignEntity.Name = string.Concat(campaignEntity.Name, "-Copy");
            campaignEntity.Code = Helpers.CreateCampaignCode();
            campaignEntity.Order = null;
            await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            CampaignRuleEntity campaignRuleEntity = await CopyCampaignRuleInfo(campaignId, campaignEntity, userid);
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            List<CampaignDocumentEntity> campaignDocumentlist = await CopyCampaignDocumentInfo(campaignId, campaignEntity, userid);
            foreach (var campaignDocument in campaignDocumentlist)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(campaignDocument);

            CampaignTargetEntity campaignTargetEntity = await CopyCampaignTargetInfo(campaignId, campaignEntity, userid);
            await _unitOfWork.GetRepository<CampaignTargetEntity>().AddAsync(campaignTargetEntity);

            List<CampaignChannelCodeEntity> campaignChannelCodeList = await CopyCampaignChannelCodeInfo(campaignId, campaignEntity, userid);
            foreach (var campaignChannelCode in campaignChannelCodeList)
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(campaignChannelCode);

            List<CampaignAchievementEntity> campaignAchievementList = await CopyCampaignAchievementInfo(campaignId, campaignEntity, userid);
            foreach (var campaignAchievement in campaignAchievementList)
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(campaignAchievement);

            await _unitOfWork.SaveChangesAsync();

            campaignEntity.Code = campaignEntity.Id.ToString();
            //campaignEntity.campaignId = campaignEntity.Id;

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);

            return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
        }
        private async Task<CampaignEntity> CopyCampaignInfo(int campaignId, string userid)
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
            campaignEntity.CreatedBy = userid;

            //campaign detail
            var campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignDraftEntity.CampaignDetail);
            var campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
            campaignDetailEntity.Id = 0;
            campaignDetailEntity.CreatedBy = userid;

            campaignEntity.CampaignDetail = campaignDetailEntity;

            return campaignEntity;

        }
        private async Task<CampaignRuleEntity> CopyCampaignRuleInfo(int campaignId, CampaignEntity campaignEntity, string userid)
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
        private async Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid)
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
        private async Task<CampaignTargetEntity> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid)
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
        private async Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid)
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
                    CreatedBy = userid,
                });
            }

            return campaignChannelCodeList;
        }
        private async Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid)
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
                campaignAchievementEntity.CreatedBy = userid;
                campaignAchievementList.Add(campaignAchievementEntity);
            }
            return campaignAchievementList;
        }


        private async Task<bool> IsValidIdentiy(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrEmpty(identity))
                return false;

            identity = identity.Trim();

            if (identity.Trim().Length > 11 || identity.Trim().Length < 10)
                return false;

            return identity.Length == 11 ? Core.Helper.Helpers.TcAuthentication(identity) : Core.Helper.Helpers.FirmaVergiKontrol(identity);
        }

        public CampaignEntity SetCampaignDefaults(CampaignEntity entity)
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

        public async Task<int> GetProcessType(int canpaignId)
        {
            int retVal  = (int)ProcessTypesEnum.Update;

            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(canpaignId);

            if (entity.StatusId == (int)StatusEnum.Draft || entity.StatusId == (int)StatusEnum.Updating)
            {
                retVal = (int)ProcessTypesEnum.Update;
            }
            else if (entity.StatusId == (int)StatusEnum.SentToApprove || entity.StatusId == (int)StatusEnum.History)
            {
                throw new Exception("Bu kampanya güncellenmek için uygun statüde değildir.");
            }
            else if (entity.StatusId == (int)StatusEnum.Approved)
            {
                var usingEntity = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                    .Where(x => !x.IsDeleted && x.Code == entity.Code &&
                                (x.StatusId == (int)StatusEnum.SentToApprove || x.StatusId == (int)StatusEnum.Updating))
                    .FirstOrDefault();
                if (usingEntity == null) 
                    retVal = (int)ProcessTypesEnum.CreateDraft;
                else
                    throw new Exception("Bu kampanya güncellenmek için uygun statüde değildir.");
            }
            return retVal;
        }
        public CampaignEntity SetCampaignUpdateRequest(CampaignEntity campaignEntity, CampaignUpdateRequest campaignUpdateRequest)
        {
            campaignEntity.CampaignDetail.DetailEn = campaignUpdateRequest.CampaignDetail.DetailEn;
            campaignEntity.CampaignDetail.DetailTr = campaignUpdateRequest.CampaignDetail.DetailTr;
            campaignEntity.CampaignDetail.SummaryEn = campaignUpdateRequest.CampaignDetail.SummaryEn;
            campaignEntity.CampaignDetail.SummaryTr = campaignUpdateRequest.CampaignDetail.SummaryTr;
            campaignEntity.CampaignDetail.CampaignDetailImageUrl = campaignUpdateRequest.CampaignDetail.CampaignDetailImageUrl;
            campaignEntity.CampaignDetail.CampaignListImageUrl = campaignUpdateRequest.CampaignDetail.CampaignListImageUrl;
            campaignEntity.CampaignDetail.ContentTr = campaignUpdateRequest.CampaignDetail.ContentTr;
            campaignEntity.CampaignDetail.ContentEn = campaignUpdateRequest.CampaignDetail.ContentEn;

            campaignEntity.ContractId = campaignUpdateRequest.ContractId;
            campaignEntity.ProgramTypeId = campaignUpdateRequest.ProgramTypeId;
            campaignEntity.SectorId = campaignUpdateRequest.SectorId;
            campaignEntity.ViewOptionId = campaignUpdateRequest.ViewOptionId;
            campaignEntity.IsBundle = campaignUpdateRequest.IsBundle;
            campaignEntity.IsActive = campaignUpdateRequest.IsActive;
            campaignEntity.IsContract = campaignUpdateRequest.IsContract;
            campaignEntity.DescriptionTr = campaignUpdateRequest.DescriptionTr;
            campaignEntity.DescriptionEn = campaignUpdateRequest.DescriptionEn;
            campaignEntity.EndDate = DateTime.Parse(campaignUpdateRequest.EndDate);
            campaignEntity.StartDate = DateTime.Parse(campaignUpdateRequest.StartDate);
            campaignEntity.Name = campaignUpdateRequest.Name;
            campaignEntity.Order = campaignUpdateRequest.Order;
            campaignEntity.TitleTr = campaignUpdateRequest.TitleTr;
            campaignEntity.TitleEn = campaignUpdateRequest.TitleEn;
            campaignEntity.MaxNumberOfUser = campaignUpdateRequest.MaxNumberOfUser;
            campaignEntity.ParticipationTypeId = campaignUpdateRequest.ParticipationTypeId;

            return campaignEntity;
        }
    }
}
