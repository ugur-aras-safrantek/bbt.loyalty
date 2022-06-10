using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.CampaignAchievement
{
    public class CampaignAchievementService : ICampaignAchievementService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDraftService _draftService;
        private static int moduleTypeId = (int)ModuleTypeEnum.Campaign;

        public CampaignAchievementService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, IAuthorizationService authorizationservice, IDraftService draftService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationservice;
            _draftService = draftService;
        }

        public async Task<BaseResponse<List<CampaignAchievementDto>>> AddAsync(CampaignAchievementInsertRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(request);

            await CheckValidationForSentToApproval(request.CampaignId);

            await Update(request, userid);

            List<CampaignAchievementDto> response = new List<CampaignAchievementDto>();

            response = await GetCampaignAchievementListDto(request.CampaignId);

            return await BaseResponse<List<CampaignAchievementDto>>.SuccessAsync(response);
        }

        public async Task<BaseResponse<List<CampaignAchievementDto>>> UpdateAsync(CampaignAchievementInsertRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(request);

            int processTypeId = await _draftService.GetCampaignProcessType(request.CampaignId);
            if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
            {
                request.CampaignId = await _draftService.CreateCampaignDraftAsync(request.CampaignId, userid, (int)PageTypeEnum.CampaignAchievement);
            }
            else
            {
                var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == request.CampaignId).FirstOrDefault();
                if (campaignUpdatePageEntity != null)
                {
                    campaignUpdatePageEntity.IsCampaignAchievementUpdated = true;
                    await _unitOfWork.GetRepository<CampaignUpdatePageEntity>().UpdateAsync(campaignUpdatePageEntity);
                }
            }

            await CheckValidationForSentToApproval(request.CampaignId);

            await Update(request, userid);

            List<CampaignAchievementDto> response = await GetCampaignAchievementListDto(request.CampaignId);

            return await BaseResponse<List<CampaignAchievementDto>>.SuccessAsync(response);
        }

        private async Task Update(CampaignAchievementInsertRequest request, string userid) 
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(request.CampaignId);
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunmadı.");

            foreach (var deleteEntity in _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == request.CampaignId && x.IsDeleted != true))
            {
                await _unitOfWork.GetRepository<CampaignAchievementEntity>().DeleteAsync(deleteEntity);
            }

            
            foreach (var x in request.CampaignAchievementList)
            {
                CampaignAchievementEntity entity = new CampaignAchievementEntity();

                entity.CampaignId = request.CampaignId;
                entity.CurrencyId = x.CurrencyId;
                entity.Amount = x.Amount;
                entity.Rate = x.Rate;
                entity.MaxAmount = x.MaxAmount;
                entity.MaxUtilization = x.MaxUtilization;
                entity.AchievementTypeId = x.AchievementTypeId;
                entity.ActionOptionId = x.ActionOptionId;
                entity.DescriptionTr = x.DescriptionTr;
                entity.DescriptionEn = x.DescriptionEn;
                entity.TitleTr = x.TitleTr;
                entity.TitleEn = x.TitleEn;
                entity.Type = x.Type == (int)AchievementType.Amount ? AchievementType.Amount : AchievementType.Rate;
                entity.CreatedBy = userid;
                entity.XKAMPCode = x.XKAMPCode;

                #region defaults

                if (entity.Type == AchievementType.Amount)
                {
                    entity.Rate = null;
                }
                else if (entity.Type == AchievementType.Rate)
                {
                    entity.Amount = null;
                    entity.CurrencyId = null;
                    if ((x.Rate ?? 0) > 0)
                        entity.XKAMPCode = null;
                    else if (!string.IsNullOrEmpty(x.XKAMPCode))
                        entity.Rate = null;
                }

                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                if (viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign)
                {
                    entity.DescriptionTr = null;
                    entity.DescriptionEn = null;
                    entity.TitleTr = null;
                    entity.TitleEn = null;
                }

                #endregion

                await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(entity);
            }

            campaignEntity.StatusId = (int)StatusEnum.SentToApprove;
            campaignEntity.LastModifiedBy = userid;
            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);

            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<BaseResponse<CampaignAchievementDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignAchievementEntity>().GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<CampaignAchievementEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignAchievementAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignAchievementDto>> GetCampaignAchievementAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                    .GetAll(x => x.Id == id && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (entity != null)
            {
                CampaignAchievementDto mappedCampaignAchievement = _mapper.Map<CampaignAchievementDto>(entity);
                return await BaseResponse<CampaignAchievementDto>.SuccessAsync(mappedCampaignAchievement);
            }
            return await BaseResponse<CampaignAchievementDto>.FailAsync("Kampanya kazanımı bulunamadı.");
        }

        public async Task<List<CampaignAchievementDto>> GetCampaignAchievementListDto(int campaignId) 
        {
            List<CampaignAchievementDto> response = new List<CampaignAchievementDto>();

            var achievementList = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                    .Include(x => x.Campaign)
                    .Include(x => x.Currency)
                    .Include(x => x.AchievementType)
                    .Include(x => x.ActionOption)
                    .ToListAsync();

            if (!achievementList.Any()) 
            {
                return response;
            }

            foreach (var achievementEntity in achievementList) 
            {
                response.Add(await GetCampaignAchievementDto(achievementEntity));           
            }

            return response;
        }

        private async Task<CampaignAchievementDto> GetCampaignAchievementDto(CampaignAchievementEntity entity) 
        {
            CampaignAchievementDto mappedCampaignAchievement = _mapper.Map<CampaignAchievementDto>(entity);

            mappedCampaignAchievement.Campaign = new ParameterDto { Id = entity.Campaign.Id, Code = entity.Campaign.Id.ToString(), Name = entity.Campaign.Name };
            mappedCampaignAchievement.Rule = new ParameterDto
            {
                Id = mappedCampaignAchievement.Type,
                Code = "",
                Name = Helpers.GetEnumDescription<AchievementType>(mappedCampaignAchievement.Type)
            };

            if (entity.Currency != null) 
            {
                mappedCampaignAchievement.Currency = new ParameterDto{ Id = entity.Currency.Id, Code = "", Name = entity.Currency.Name };
            }
            if (mappedCampaignAchievement.ActionOption != null)
            {
                mappedCampaignAchievement.ActionOption = new ParameterDto { Id = entity.ActionOption.Id, Code = "", Name = entity.ActionOption.Name };
            }
            mappedCampaignAchievement.AchievementType = new ParameterDto { Id = entity.AchievementType.Id, Code = "", Name = entity.AchievementType.Name };
            mappedCampaignAchievement.AmountStr = Helpers.ConvertNullablePriceString(mappedCampaignAchievement.Amount);
            mappedCampaignAchievement.RateStr = Helpers.ConvertNullablePriceString(mappedCampaignAchievement.Rate);
            mappedCampaignAchievement.MaxAmountStr = Helpers.ConvertNullablePriceString(mappedCampaignAchievement.MaxAmount);
            mappedCampaignAchievement.MaxUtilizationStr = Helpers.ConvertNullablePriceString(mappedCampaignAchievement.MaxUtilization);

            return mappedCampaignAchievement;
        }

        public async Task<BaseResponse<CampaignAchievementInsertFormDto>> GetInsertFormAsync(int campaignId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignAchievementInsertFormDto response = new CampaignAchievementInsertFormDto();
            
            await FillForm(response, campaignId);

            return await BaseResponse<CampaignAchievementInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignAchievementInsertFormDto response, int campaignId)
        {
            response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
            response.ActionOptions = (await _parameterService.GetActionOptionListAsync())?.Data;

            CampaignProperty campaignProperty = await _draftService.GetCampaignProperties(campaignId);
            response.IsUpdatableCampaign = campaignProperty.IsUpdatableCampaign;
            response.IsInvisibleCampaign = campaignProperty.IsInvisibleCampaign;
        }

        public async Task<BaseResponse<List<CampaignAchievementDto>>> GetListAsync()
        {
            List<CampaignAchievementDto> campaignAchievementList = _unitOfWork.GetRepository<CampaignAchievementEntity>().GetAll().Select(x => _mapper.Map<CampaignAchievementDto>(x)).ToList();
            return await BaseResponse<List<CampaignAchievementDto>>.SuccessAsync(campaignAchievementList);
        }

        public async Task<BaseResponse<CampaignAchievementUpdateFormDto>> GetUpdateFormAsync(int campaignId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignAchievementUpdateFormDto response = new CampaignAchievementUpdateFormDto();
            await FillForm(response, campaignId);            
            response.CampaignId = campaignId;
            response.CampaignAchievementList = await GetCampaignAchievementListDto(campaignId);
            return await BaseResponse<CampaignAchievementUpdateFormDto>.SuccessAsync(response);
        }

        public async Task CheckValidationAsync(CampaignAchievementInsertRequest request)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => x.Id == request.CampaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Draft).FirstOrDefaultAsync();
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            if (!request.CampaignAchievementList.Any())
                throw new Exception("Kazanım giriniz.");

            var groupedAchievementType = request.CampaignAchievementList
                .GroupBy(x => x.AchievementTypeId)
                .Where(grp => grp.Count() > 1)
                .FirstOrDefault();
            if(groupedAchievementType != null)
                throw new Exception("Kazanım Tipi çoklanamaz");

            foreach (var input in request.CampaignAchievementList) 
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                if (viewOptionId != (int)ViewOptionsEnum.InvisibleCampaign)
                {
                    if (string.IsNullOrWhiteSpace(input.DescriptionTr))
                        throw new Exception("Açıklama Detayı girilmelidir.");
                    if (string.IsNullOrWhiteSpace(input.DescriptionEn))
                        throw new Exception("Açıklama Detayı (İngilizce) girilmelidir.");
                    if (string.IsNullOrWhiteSpace(input.TitleTr))
                        throw new Exception("Başlık girilmelidir.");
                    if (string.IsNullOrWhiteSpace(input.TitleEn))
                        throw new Exception("Başlık (İngilizce) girilmelidir.");
                }

                if ((input.ActionOptionId ?? 0) > 0)
                {
                    var actionOption = (await _parameterService.GetActionOptionListAsync())?.Data?.Any(x => x.Id == input.ActionOptionId);
                    if (!actionOption.GetValueOrDefault(false))
                        throw new Exception("Aksiyon seçilmelidir.");
                }
                if (input.AchievementTypeId <= 0)
                    throw new Exception("Kazanım tipi seçilmelidir.");
                else
                {
                    var achievementFrequency = (await _parameterService.GetAchievementTypeListAsync())?.Data?.Any(x => x.Id == input.AchievementTypeId);
                    if (!achievementFrequency.GetValueOrDefault(false))
                        throw new Exception("Kazanım tipi seçilmelidir.");
                }

                if (input.Type == (int)AchievementType.Amount)
                {
                    if (!input.CurrencyId.HasValue || input.CurrencyId <= 0)
                        throw new Exception("Para Birimi seçilmelidir.");
                    else
                    {
                        var currency = (await _parameterService.GetCurrencyListAsync())?.Data?.Any(x => x.Id == input.CurrencyId);
                        if (!currency.GetValueOrDefault(false))
                            throw new Exception("Para Birimi seçilmelidir.");
                    }
                    if (!input.Amount.HasValue || input.Amount.Value <= 0)
                        throw new Exception("Kazanım Tutarı girilmelidir.");
                }
                else if (input.Type == (int)AchievementType.Rate)
                {
                    if (!input.Rate.HasValue)
                        throw new Exception("Kazanım Oranı girilmelidir.");
                    if (input.Rate > 100)
                        throw new Exception("“Kazanım Oranı % 100’ün üzerinde bir değer girilemez.");
                    if (input.Rate <= 0)
                        throw new Exception("Kazanım Oranı 0’dan küçük bir değer girilemez");
                }
                else
                    throw new Exception("Kazanım türü (Tutar/Oran) seçilmelidir.");
            }
        }

        private async Task CheckValidationForSentToApproval(int campaignId) 
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Draft).FirstOrDefaultAsync();
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı.");

            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted).FirstOrDefaultAsync();
            if (campaignRuleEntity == null)
                throw new Exception("Kampanya kuralları giriniz.");

            var campaignTargetEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted).FirstOrDefaultAsync();
            if (campaignTargetEntity == null)
                throw new Exception("Kampanya hedefleri giriniz.");

            var campaignChannelCodeEntity = await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().GetAll(x => x.CampaignId == campaignId && !x.IsDeleted).FirstOrDefaultAsync();
            if (campaignChannelCodeEntity == null)
                throw new Exception("Kampanya kanal kodu giriniz.");
        }

        public async Task<BaseResponse<bool>> SendToAppropval(int campaignId, string userid) 
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => x.Id == campaignId && !x.IsDeleted && x.StatusId == (int)StatusEnum.Draft).FirstOrDefaultAsync();
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı.");
            campaignEntity.StatusId = (int)StatusEnum.SentToApprove;
            campaignEntity.LastModifiedBy = userid;
            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(campaignEntity);
            return await BaseResponse<bool>.SuccessAsync(true);
        }

        public async Task<List<CustomerAchievement>> GetCustomerAchievementsAsync(int campaignId, string customerCode, string lang) 
        {
            List<CustomerAchievement> customerAchievementList = new List<CustomerAchievement>();

            if (StaticValues.IsDevelopment) 
            {
                var campaignAchievementList = await GetCampaignAchievementListDto(campaignId);
                if(campaignAchievementList.Any())
                {
                    foreach(var x in campaignAchievementList) 
                    {
                        CustomerAchievement customerAchievement= new CustomerAchievement();
                        customerAchievement.IsAchieved = false;
                        customerAchievement.AchievementTypeName = x.AchievementType.Name;
                        customerAchievement.Description = lang == "tr" ? x.DescriptionTr : x.DescriptionEn;
                        customerAchievement.Title = lang == "tr" ? x.TitleTr : x.TitleEn;
                        customerAchievement.AmountStr = x.AmountStr;
                        customerAchievement.CurrencyCode = x.Currency?.Name;
                        customerAchievementList.Add(customerAchievement);
                    }
                }
            }
            else 
            {
                using (var httpClient = new HttpClient()) 
                {
                    string accessToken = await _parameterService.GetAccessToken();
                    string serviceUrl = await _parameterService.GetServiceConstantValue("EarningByCustomerAndCampaing");
                    serviceUrl = serviceUrl.Replace("{customerId}", customerCode);
                    serviceUrl = serviceUrl.Replace("{campaignId}", campaignId.ToString());
                    serviceUrl = serviceUrl.Replace("{lang}", lang);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var restResponse = await httpClient.GetAsync(serviceUrl);
                    if (restResponse.IsSuccessStatusCode)
                    {
                        if (restResponse.Content != null)
                        {
                            var apiResponse = await restResponse.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(apiResponse))
                            {
                                var earningByCustomerAndCampaingList = JsonConvert.DeserializeObject<List<EarningByCustomerAndCampaing>>(apiResponse);
                                if (earningByCustomerAndCampaingList != null && earningByCustomerAndCampaingList.Any())
                                {
                                    foreach (var earning in earningByCustomerAndCampaingList)
                                    {
                                        CustomerAchievement customerAchievement = new CustomerAchievement();
                                        customerAchievement.IsAchieved = false;
                                        customerAchievement.AchievementTypeName = earning.EarningType;
                                        customerAchievement.Description = earning.AchivementDescription;
                                        customerAchievement.Title = earning.AchivementTitle;
                                        customerAchievement.AmountStr = Helpers.ConvertNullablePriceString(earning.Amount);
                                        customerAchievement.CurrencyCode = earning.Currency;
                                        customerAchievementList.Add(customerAchievement);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Müşteri kazanım servisinden hata alındı.");
                    }
                }


            }
            return customerAchievementList;
        }
    }
}
