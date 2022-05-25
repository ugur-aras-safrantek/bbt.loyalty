using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
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
            int campaignPageId,
            string userid,
            CampaignUpdateRequest campaignUpdateRequest, 
            AddCampaignRuleRequest addCampaignRuleRequest, 
            CampaignTargetInsertRequest campaignTargetInsertRequest, 
            CampaignAchievementInsertRequest campaignAchievementInsertRequest
            ) 
        {
            Dictionary<string, int> returnList = new Dictionary<string, int>();

            //campaign
            CampaignEntity campaignEntity;
            CampaignDetailEntity campaignDetailEntity;
            CampaignDto campaignDto;
            CampaignDetailDto campaignDetailDto;
            if (campaignPageId == (int)CampaignPagesEnum.Campaign)
            {
                campaignDto = _mapper.Map<CampaignDto>(campaignUpdateRequest);
                campaignDto.Id = 0;
                campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);

                campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignUpdateRequest.CampaignDetail);
                campaignDetailDto.Id = 0;
                campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
                campaignEntity.CampaignDetail = campaignDetailEntity;
            }
            else
            {
                var campaignReferenceEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                    .Include(x => x.CampaignDetail)
                    .FirstOrDefaultAsync();
                if (campaignReferenceEntity == null)
                    throw new Exception("Kampanya bulunamadı.");

                campaignDto = _mapper.Map<CampaignDto>(campaignReferenceEntity);
                campaignDto.Id = 0;
                campaignDto.Code = string.Empty;
                campaignDto.IsApproved = false;
                campaignDto.IsDraft = true;
                campaignEntity = _mapper.Map<CampaignEntity>(campaignDto);

                //campaign detail
                campaignDetailDto = _mapper.Map<CampaignDetailDto>(campaignReferenceEntity.CampaignDetail);
                campaignDetailDto.Id= 0;
                campaignDetailEntity = _mapper.Map<CampaignDetailEntity>(campaignDetailDto);
                campaignEntity.CampaignDetail = campaignDetailEntity;
            }

            campaignEntity = await SetCampaignDefaults(campaignEntity);

            campaignEntity.CreatedBy = userid;
            campaignEntity.LastModifiedBy = null;
            campaignEntity.LastModifiedOn = null;

            campaignDetailEntity.CreatedBy = userid;
            campaignDetailEntity.LastModifiedBy = null;
            campaignDetailEntity.LastModifiedOn = null;

            campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(campaignEntity);

            //CampaignRuleEntity campaignRuleEntity = await AddCampaignRule(campaignId, campaignPageId, campaignEntity, addCampaignRuleRequest);

            return campaignEntity.Id;
        }

        private async Task<CampaignEntity> AddCampaign(int refCampaignId, int campaignPageId, CampaignUpdateRequest request) 
        {
            CampaignEntity campaignEntity = new CampaignEntity();
            if (campaignPageId == (int)CampaignPagesEnum.Campaign)
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

        private async Task<CampaignRuleEntity> AddCampaignRule(int refCampaignId, int campaignPageId, CampaignEntity campaignEntity, AddCampaignRuleRequest request)
        {
            CampaignRuleEntity campaignRuleEntity = new CampaignRuleEntity();

            if (campaignPageId == (int)CampaignPagesEnum.CampaignRule) //update sayfasından geliyorsa, requesti kopyala
            {
                campaignRuleEntity.Campaign = campaignEntity;
                campaignRuleEntity.CampaignStartTermId = request.StartTermId;
                campaignRuleEntity.JoinTypeId = request.JoinTypeId;

                if (request.JoinTypeId == (int)JoinTypeEnum.Customer)
                {
                    if (request.IsSingleIdentity)
                    {
                        var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
                        {
                            Identities = request.Identity.Trim(),
                            CampaignRule = campaignRuleEntity,
                        };

                        await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                    }
                    else if (request.File != null)
                    {
                        byte[] bytesList = System.Convert.FromBase64String(request.File);

                        var memoryStream = new MemoryStream(bytesList);

                        await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                        {
                            Campaign = campaignEntity,
                            DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN,
                            MimeType = MimeTypeExtensions.ToMimeType(".xlsx"),
                            Content = bytesList,
                            DocumentName = request.Identity
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
                                };

                                await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                            }
                        }
                    }
                }
                else if (request.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
                {
                    if (request.BusinessLines is { Count: > 0 })
                    {
                        request.BusinessLines.ForEach(x =>
                        {
                            var campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                BusinessLineId = x,
                            };
                            _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
                        });
                    }
                }
                else if (request.JoinTypeId == (int)JoinTypeEnum.Branch)
                {
                    if (request.Branches.Any())
                    {
                        request.Branches.ForEach(x =>
                        {
                            var campaignRuleBranchEntity = new CampaignRuleBranchEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                BranchCode = x,
                                BranchName = ""
                            };
                            _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
                        });
                    }
                }
                else if (request.JoinTypeId == (int)JoinTypeEnum.CustomerType)
                {
                    if (request.CustomerTypes is { Count: > 0 })
                    {
                        request.CustomerTypes.ForEach(x =>
                        {
                            var campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity()
                            {
                                CampaignRule = campaignRuleEntity,
                                CustomerTypeId = x,
                            };
                            _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
                        });
                    }
                }
            }
            else 
            {
                var campaignRuleDraftEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                    .GetAll(x => x.CampaignId == refCampaignId && x.IsDeleted != true)
                    .Include(x => x.Branches.Where(t => t.IsDeleted != true))
                    .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
                    .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
                    .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                    .FirstOrDefaultAsync();
                if (campaignRuleDraftEntity == null)
                    throw new Exception("Kampanya kuralı bulunamadı.");

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
            }
            await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(campaignRuleEntity);

            return campaignRuleEntity;
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

        private async Task<CampaignEntity> SetCampaignDefaults(CampaignEntity entity)
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
    
        //public async Task<CampaignEntity> SetCampaignRowValues() 
        //{ 
            
        //}
    
    
    }
}
