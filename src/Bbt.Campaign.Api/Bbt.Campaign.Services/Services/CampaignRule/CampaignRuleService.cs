using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignRule
{
    public class CampaignRuleService : ICampaignRuleService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;

        public CampaignRuleService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
        }

        public async Task<BaseResponse<CampaignRuleDto>> AddAsync(AddCampaignRuleRequest campaignRule)
        {
            await CheckValidationsAsync(campaignRule, false);

            var entity = new CampaignRuleEntity()
            {
                CampaignId = campaignRule.CampaignId,
                CampaignStartTermId = campaignRule.StartTermId,
                JoinTypeId = campaignRule.JoinTypeId
            };

            if (campaignRule.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                if (campaignRule.IsSingleIdentity)
                {
                    entity.RuleIdentities = new List<CampaignRuleIdentityEntity>();
                    entity.RuleIdentities.Add(new CampaignRuleIdentityEntity()
                    {
                        Identities = campaignRule.Identity.Trim(),
                    });
                }
                else if (campaignRule.File != null)
                {                   
                    byte[] bytesList = System.Convert.FromBase64String(campaignRule.File);

                    var memoryStream = new MemoryStream(bytesList);

                    await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                    {
                        CampaignId = campaignRule.CampaignId,
                        DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN,
                        MimeType = MimeTypeExtensions.ToMimeType(".xlsx"),
                        Content = bytesList,
                        DocumentName = campaignRule.Identity
                    });

                    using (var excelWorkbook = new XLWorkbook(memoryStream))
                    {
                        entity.RuleIdentities = new List<CampaignRuleIdentityEntity>();

                        var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

                        //List<string> identityList = new List<string>();

                        foreach (var dataRow in nonEmptyDataRows)
                        {
                            string identity = dataRow.Cell(1).Value == null ? string.Empty : dataRow.Cell(1).Value.ToString().Trim();

                            //if (identityList.Contains(identity))
                            //    throw new Exception("Dosya içerisinde bazı kayıtlar çoklanmış.");

                            //identityList.Add(identity);

                            //CheckSingleIdentiy(identity);

                            if (!await IsValidIdentiy(identity))
                                continue;

                            entity.RuleIdentities.Add(new CampaignRuleIdentityEntity()
                            {
                                Identities = identity,
                            });
                        }
                    }
                }
                else
                {
                    throw new Exception("TCKN girilmelidir veya dosya seçilmelidir.");
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.BusinessLine)
            {
                if (campaignRule.BusinessLines is { Count: > 0 })
                {
                    entity.BusinessLines = new List<CampaignRuleBusinessLineEntity>();
                    campaignRule.BusinessLines.ForEach(x =>
                    {
                        entity.BusinessLines.Add(new CampaignRuleBusinessLineEntity()
                        {
                            BusinessLineId = x
                        });
                    });
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.Branch) 
            { 
                if (campaignRule.Branches.Any())
                {
                    entity.Branches = new List<CampaignRuleBranchEntity>();
                    campaignRule.Branches.ForEach(x =>
                    {
                        entity.Branches.Add(new CampaignRuleBranchEntity()
                        {
                            BranchCode = x,
                            BranchName = ""
                        });
                    });
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                if (campaignRule.CustomerTypes is { Count: > 0 })
                {
                    entity.CustomerTypes = new List<CampaignRuleCustomerTypeEntity>();
                    campaignRule.CustomerTypes.ForEach(x =>
                    {

                        entity.CustomerTypes.Add(new CampaignRuleCustomerTypeEntity()
                        {
                            CustomerTypeId = x
                        });
                    });
                }
            }         
                             
            entity = await _unitOfWork.GetRepository<CampaignRuleEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            var mappedCampaignRuleDto = _mapper.Map<CampaignRuleDto>(entity);

            return await BaseResponse<CampaignRuleDto>.SuccessAsync(mappedCampaignRuleDto);
        }

        public async Task<BaseResponse<CampaignRuleDto>> UpdateAsync(AddCampaignRuleRequest campaignRule)
        {
            await CheckValidationsAsync(campaignRule, true);

            var entity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignRule.CampaignId && x.IsDeleted != true)
                .Include(x => x.Branches.Where(t => t.IsDeleted != true))
                .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
                .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
                .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();

            if (entity == null) 
                return await AddAsync(campaignRule);

            int campaignRuleId = entity.Id;


            entity.CampaignStartTermId = campaignRule.StartTermId;
            entity.JoinTypeId = campaignRule.JoinTypeId;

            foreach (var x in entity.Branches)
            {
                await _unitOfWork.GetRepository<CampaignRuleBranchEntity>().DeleteAsync(x);
            }
            foreach (var x in entity.CustomerTypes)
            {
                await _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().DeleteAsync(x);
            }
            foreach (var x in entity.BusinessLines)
            {
                await _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().DeleteAsync(x);
            }
            foreach (var x in entity.RuleIdentities)
            {
                await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().DeleteAsync(x);
            }
            var ruleDocument = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                       .GetAll(x => x.CampaignId == entity.CampaignId
                           && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                           && !x.IsDeleted)
                       .FirstOrDefaultAsync();
            if(ruleDocument != null)
                await _unitOfWork.GetRepository<CampaignDocumentEntity>().DeleteAsync(ruleDocument);

            if (campaignRule.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                if (campaignRule.IsSingleIdentity)
                {
                    var campaignRuleIdentityEntity = new CampaignRuleIdentityEntity()
                    {
                        Identities = campaignRule.Identity.Trim(),
                        CampaignRule = entity,
                    };

                    await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                }
                else if (campaignRule.File != null)
                {
                    byte[] bytesList = System.Convert.FromBase64String(campaignRule.File);

                    var memoryStream = new MemoryStream(bytesList);

                    await _unitOfWork.GetRepository<CampaignDocumentEntity>().AddAsync(new CampaignDocumentEntity()
                    {
                        CampaignId = campaignRule.CampaignId,
                        DocumentType = Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN,
                        MimeType = MimeTypeExtensions.ToMimeType(".xlsx"),
                        Content = bytesList,
                        DocumentName = campaignRule.Identity
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
                                CampaignRuleId = campaignRuleId,
                            };

                            await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>().AddAsync(campaignRuleIdentityEntity);
                        }
                    }
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.BusinessLine) 
            {
                if (campaignRule.BusinessLines is { Count: > 0 })
                {
                    campaignRule.BusinessLines.ForEach(x =>
                    {
                        var campaignRuleBusinessLineEntity = new CampaignRuleBusinessLineEntity()
                        {
                            CampaignRuleId = campaignRuleId,
                            BusinessLineId = x,
                        };
                        _unitOfWork.GetRepository<CampaignRuleBusinessLineEntity>().AddAsync(campaignRuleBusinessLineEntity);
                    });
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.Branch)
            { 
                if (campaignRule.Branches.Any())
                {
                    campaignRule.Branches.ForEach(x =>
                    {
                        var campaignRuleBranchEntity = new CampaignRuleBranchEntity()
                        {
                            CampaignRuleId = campaignRuleId,
                            BranchCode = x,
                            BranchName = ""
                        };
                        _unitOfWork.GetRepository<CampaignRuleBranchEntity>().AddAsync(campaignRuleBranchEntity);
                    });
                }
            }
            else if (campaignRule.JoinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                if (campaignRule.CustomerTypes is { Count: > 0 })
                {
                    campaignRule.CustomerTypes.ForEach(x =>
                    {
                        var campaignRuleCustomerTypeEntity = new CampaignRuleCustomerTypeEntity()
                        {
                            CampaignRuleId = campaignRuleId,
                            CustomerTypeId = x,
                        };
                        _unitOfWork.GetRepository<CampaignRuleCustomerTypeEntity>().AddAsync(campaignRuleCustomerTypeEntity);
                    });
                }
            }

            await _unitOfWork.GetRepository<CampaignRuleEntity>().UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignRuleAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignRuleDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignRuleEntity>().GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<CampaignRuleEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignRuleAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignRuleDto>> GetCampaignRuleAsync(int id)
        {
            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>().GetByIdAsync(id);
            if (campaignRuleEntity != null)
            {
                CampaignRuleDto campaignRuleDto = _mapper.Map<CampaignRuleDto>(campaignRuleEntity);
                return await BaseResponse<CampaignRuleDto>.SuccessAsync(campaignRuleDto);

            }
            return null;
        }

        public async Task<BaseResponse<CampaignRuleInsertFormDto>> GetInsertForm()
        {
            CampaignRuleInsertFormDto response = new CampaignRuleInsertFormDto();
            await FillForm(response);

            return await BaseResponse<CampaignRuleInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignRuleInsertFormDto response)
        {
            response.CampaignStartTermList = (await _parameterService.GetCampaignStartTermListAsync())?.Data;
            response.BusinessLineList = (await _parameterService.GetBusinessLineListAsync())?.Data;
            response.BranchList = (await _parameterService.GetBranchListAsync())?.Data;
            response.CustomerTypeList = (await _parameterService.GetCustomerTypeListAsync())?.Data;
            response.JoinTypeList = (await _parameterService.GetJoinTypeListAsync())?.Data;

            
        }

        public async Task<BaseResponse<List<CampaignRuleDto>>> GetListAsync()
        {
            List<CampaignRuleDto> campaignRules = _unitOfWork.GetRepository<CampaignRuleEntity>().GetAll().Select(x => _mapper.Map<CampaignRuleDto>(x)).ToList();
            return await BaseResponse<List<CampaignRuleDto>>.SuccessAsync(campaignRules);
        }

        public async Task<BaseResponse<CampaignRuleUpdateFormDto>> GetUpdateForm(int campaignId)
        {
            CampaignRuleUpdateFormDto response = new CampaignRuleUpdateFormDto();
            
            await FillForm(response);

            response.IsInvisibleCampaign = await _campaignService.IsInvisibleCampaign(campaignId);

            var campaignRuleDto = await GetCampaignRuleDto(campaignId);

            if (campaignRuleDto != null)
            {
                response.CampaignRule = campaignRuleDto;  
            }

            return await BaseResponse<CampaignRuleUpdateFormDto>.SuccessAsync(response);
        }

        public async Task<CampaignRuleDto> GetCampaignRuleDto(int campaignId)
        {
            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
               .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
               .Include(x => x.Branches.Where(t => t.IsDeleted != true))
               .Include(x => x.BusinessLines.Where(t => t.IsDeleted != true))
               .Include(x => x.CustomerTypes.Where(t => t.IsDeleted != true))
               .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
               .FirstOrDefaultAsync();
            if (campaignRuleEntity == null)
            {
                return null;
            }

            string documentName = null;
            string identityNumber = null;
            bool isSingleIdentity = false;
            if (campaignRuleEntity.JoinTypeId == (int)JoinTypeEnum.Customer)
            {
                var identityList = await _unitOfWork.GetRepository<CampaignRuleIdentityEntity>()
                    .GetAll(x => x.CampaignRuleId == campaignRuleEntity.Id && !x.IsDeleted)
                    .Select(x => x.Identities)
                    .ToListAsync();
                if (campaignRuleEntity.RuleIdentities.Count == 1)
                {
                    isSingleIdentity = true;
                    identityNumber = identityList[0];
                }
                else 
                {
                    var ruleDocument = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                       .GetAll(x => x.CampaignId == campaignId
                           && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                           && !x.IsDeleted)
                       .FirstOrDefaultAsync();
                    if (ruleDocument != null) 
                    {
                        documentName = ruleDocument.DocumentName;
                    }
                }
            }
            CampaignRuleDto campaignRuleDto = new CampaignRuleDto()
            {
                Id = campaignRuleEntity.Id,
                CampaignId = campaignRuleEntity.CampaignId,
                JoinTypeId = campaignRuleEntity.JoinTypeId,
                CampaignStartTermId = campaignRuleEntity.CampaignStartTermId,
                IdentityNumber = identityNumber,
                IsSingleIdentity = isSingleIdentity,
                DocumentName = documentName,
                RuleBusinessLines = campaignRuleEntity.BusinessLines.Where(c => !c.IsDeleted).Select(c => c.BusinessLineId).ToList(),
                RuleCustomerTypes = campaignRuleEntity.CustomerTypes.Where(c => !c.IsDeleted).Select(c => c.CustomerTypeId).ToList(),
                RuleBranches = campaignRuleEntity.Branches.Where(c => !c.IsDeleted).Select(c => c.BranchCode).ToList()
            };

            return campaignRuleDto;
        }

        async Task CheckValidationsAsync(AddCampaignRuleRequest input, bool isUpdate)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == input.CampaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            int joinTypeId = input.JoinTypeId;

            if (joinTypeId <= 0)
                throw new Exception("Dahil Olma Şekli seçilmelidir.");

            var joinType = (await _parameterService.GetJoinTypeListAsync())?.Data?.Any(x => x.Id == joinTypeId);
            if (!joinType.GetValueOrDefault(false))
                throw new Exception("Dahil Olma Şekli hatalı.");

            if (input.StartTermId <= 0)
                throw new Exception("Kampanya Başlama Dönemi seçilmelidir.");
            else
            {
                var startTerm = (await _parameterService.GetCampaignStartTermListAsync())?.Data?.Any(x => x.Id == input.StartTermId);
                if (!startTerm.GetValueOrDefault(false))
                    throw new Exception("Kampanya Başlama Dönemi seçilmelidir.");
            }

            if(joinTypeId == (int)JoinTypeEnum.BusinessLine) 
            {
                if (input.BusinessLines == null || input.BusinessLines is { Count: < 1 })
                    throw new Exception("İş Kolu seçilmelidir.");

                foreach(var id in input.BusinessLines) 
                {
                    var businessLine = (await _parameterService.GetBusinessLineListAsync())?.Data?.Any(x => x.Id == id);
                    if (!businessLine.GetValueOrDefault(false))
                        throw new Exception("İş Kolu hatalı.");
                }
            }
            else if (joinTypeId == (int)JoinTypeEnum.Customer) 
            {
                if (input.IsSingleIdentity)
                    await CheckSingleIdentiy(input.Identity ?? "");
                else 
                {
                    if (string.IsNullOrEmpty(input.File))
                        throw new Exception("Dosya boş olamaz.");
                    if(string.IsNullOrEmpty(input.Identity))
                        throw new Exception("Dosya adı boş olamaz.");
                }
            }
            else if (joinTypeId == (int)JoinTypeEnum.Branch) 
            {
                if (input.Branches == null || input.Branches is { Count: < 1 })
                    throw new Exception("Şube seçilmelidir.");

                foreach (var code in input.Branches)
                {
                    var branch = (await _parameterService.GetBranchListAsync())?.Data?.Any(x => x.Code == code);
                    if (!branch.GetValueOrDefault(false))
                        throw new Exception("Şube bilgisi hatalı.");
                }
            }
            else if (joinTypeId == (int)JoinTypeEnum.CustomerType)
            {
                if (input.CustomerTypes == null || input.CustomerTypes is { Count: < 1 })
                    throw new Exception("Müşteri Tipi seçilmelidir.");

                foreach (var id in input.CustomerTypes)
                {
                    var customerType = (await _parameterService.GetCustomerTypeListAsync())?.Data?.Any(x => x.Id == id);
                    if (!customerType.GetValueOrDefault(false))
                        throw new Exception("Müşteri Tipi hatalı.");
                }
            }
        }

        public async Task<BaseResponse<GetFileResponse>> GetRuleIdentityFileAsync(int campaignId)
        {
            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .Include(x => x.RuleIdentities.Where(t => t.IsDeleted != true))
                .FirstOrDefaultAsync();
            if(campaignRuleEntity == null)
                throw new Exception("Kampanya kuralı bulunamadı.");

            if (campaignRuleEntity.JoinTypeId != (int)JoinTypeEnum.Customer)
                throw new Exception("Kampanya kuralı VKN/TCKN dosyası bulunamadı.");

            if (campaignRuleEntity.RuleIdentities.Count < 2)
                throw new Exception("Kampanya kuralı VKN/TCKN dosyası bulunamadı.");

            var ruleDocument = await _unitOfWork.GetRepository<CampaignDocumentEntity>()
                       .GetAll(x => x.CampaignId == campaignId
                           && x.DocumentType == Core.Enums.DocumentTypeDbEnum.CampaignRuleTCKN
                           && !x.IsDeleted)
                       .FirstOrDefaultAsync();
            if(ruleDocument == null)
                throw new Exception("Kampanya kuralı VKN/TCKN dosyası bulunamadı.");

            var getFileResponse = new GetFileResponse()
            {
                Document = new Public.Models.CampaignDocument.DocumentModel()
                {
                    Data = Convert.ToBase64String(ruleDocument.Content, 0, ruleDocument.Content.Length),
                    DocumentName = campaignId.ToString() + "-KampanyaKuralıTCKN.xlsx",
                    DocumentType = DocumentTypePublicEnum.CampaignRuleTCKN,
                    MimeType = MimeTypeExtensions.ToMimeType(".xlsx")
                }
            };
            return await BaseResponse<GetFileResponse>.SuccessAsync(getFileResponse);
        }

        private async Task CheckSingleIdentiy(string identity)
        {
            if (string.IsNullOrWhiteSpace(identity) || string.IsNullOrEmpty(identity))
            { throw new Exception("TCKN/VKN boş olamaz."); }

            if (!Core.Helper.Helpers.IsNumeric(identity)) 
            { throw new Exception("TCKN/VKN bilgisi hatalı."); }

            identity = identity.Trim();

            if (identity.Trim().Length > 11 || identity.Trim().Length < 10)
            { throw new Exception("TCKN/VKN bilgisinin uzunluğu hatalı."); }

            if(identity.Trim().Length == 11) 
            {
                if (!Core.Helper.Helpers.TcAuthentication(identity))
                { throw new Exception("TCKN bilgisi doğrulanamadı."); }
            }
            else if (identity.Trim().Length == 10) 
            {
                if (!Core.Helper.Helpers.FirmaVergiKontrol(identity))
                { throw new Exception("VKN bilgisi doğrulanamadı."); }
            }
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
    }
}
