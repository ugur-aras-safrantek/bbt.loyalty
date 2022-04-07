using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignAchievement
{
    public class CampaignAchievementService : ICampaignAchievementService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;

        public CampaignAchievementService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
        }

        public async Task<BaseResponse<CampaignAchievementDto>> AddAsync(CampaignAchievementInsertRequest request)
        {
            await CheckValidationAsync(request);

            var entity = _mapper.Map<CampaignAchievementEntity>(request);

            entity = await SetDefaults(entity);

            if (request.CampaignChannelCodeList.Any())
            {
                entity.ChannelCodes = new List<CampaignAchievementChannelCodeEntity>();
                request.CampaignChannelCodeList.ForEach(x =>
                {
                    entity.ChannelCodes.Add(new CampaignAchievementChannelCodeEntity()
                    {
                        ChannelCode = x,
                    });
                });
            }

            entity = await _unitOfWork.GetRepository<CampaignAchievementEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignAchievementAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignAchievementDto>> UpdateAsync(CampaignAchievementInsertRequest request)
        {
            var entity = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                .GetAll(x => x.CampaignId == request.CampaignId && x.IsDeleted != true)
                .Include(x => x.ChannelCodes.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
            if (entity == null)
                return await AddAsync(request);

            await CheckValidationAsync(request);

            entity.CampaignId = request.CampaignId;
            entity.CurrencyId = request.CurrencyId;
            entity.Amount = request.Amount;
            entity.Rate = request.Rate;
            entity.MaxAmount = request.MaxAmount;
            entity.MaxUtilization = request.MaxUtilization;
            entity.AchievementTypeId = request.AchievementTypeId;
            entity.ActionOptionId = request.ActionOptionId;
            entity.DescriptionTr = request.DescriptionTr;
            entity.DescriptionEn = request.DescriptionEn;
            entity.TitleTr = request.TitleTr;
            entity.TitleEn = request.TitleEn;
            entity.Type = request.Type == (int)AchievementType.Amount ? AchievementType.Amount : AchievementType.Rate;

            entity = await SetDefaults(entity);

            if (entity.ChannelCodes.Any())
            {
                foreach (var x in entity.ChannelCodes)
                {
                    await _unitOfWork.GetRepository<CampaignAchievementChannelCodeEntity>().DeleteAsync(x);
                }
            }

            if (request.CampaignChannelCodeList.Any())
            {
                request.CampaignChannelCodeList.ForEach(x =>
                {
                    var channelCodeEntity = new CampaignAchievementChannelCodeEntity() 
                    { 
                        AchievementId = entity.Id,
                        ChannelCode = x
                        
                    };
                    _unitOfWork.GetRepository<CampaignAchievementChannelCodeEntity>().AddAsync(channelCodeEntity);
                });
            }

            await _unitOfWork.GetRepository<CampaignAchievementEntity>().UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignAchievementAsync(entity.Id);
        }

        private async Task<CampaignAchievementEntity> SetDefaults(CampaignAchievementEntity entity)
        {
            if (entity.Type == AchievementType.Amount)
            {
                entity.Rate = null;
            }
            else if (entity.Type == AchievementType.Rate)
            {
                entity.Amount = null;
                entity.CurrencyId = null;
                entity.MaxAmount = null;
            }

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(entity.CampaignId);
            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                if (viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign) 
                {
                    entity.DescriptionTr = null;
                    entity.DescriptionEn = null;
                    entity.TitleTr = null;
                    entity.TitleEn = null;
                }
            }

            return entity;
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
                    .Include(x => x.ChannelCodes.Where(t=> !t.IsDeleted))
                    .FirstOrDefaultAsync();
            if (entity != null)
            {
                CampaignAchievementDto mappedCampaignAchievement = _mapper.Map<CampaignAchievementDto>(entity);
                mappedCampaignAchievement.ChannelCodeList = new List<string>();
                foreach (var x in entity.ChannelCodes.Where(t => !t.IsDeleted))
                {
                    mappedCampaignAchievement.ChannelCodeList.Add(x.ChannelCode);
                }
                return await BaseResponse<CampaignAchievementDto>.SuccessAsync(mappedCampaignAchievement);
            }
            return await BaseResponse<CampaignAchievementDto>.FailAsync("Kampanya kazanımı bulunamadı.");
        }

        public async Task<CampaignAchievementDto> GetCampaignAchievementDto(int campaignId) 
        {
            var entity = await _unitOfWork.GetRepository<CampaignAchievementEntity>()
                    .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                    .Include(x => x.ChannelCodes.Where(x => !x.IsDeleted))
                    .Include(x => x.Campaign)
                    .Include(x=>x.Currency)
                    .Include(x => x.AchievementType)
                    .Include(x => x.ActionOption)
                    .FirstOrDefaultAsync();

            if (entity == null) 
            {
                return null;
            }

            CampaignAchievementDto mappedCampaignAchievement = _mapper.Map<CampaignAchievementDto>(entity);
            mappedCampaignAchievement.ChannelCodeList = new List<string>();
            foreach (var x in entity.ChannelCodes) 
            {
                mappedCampaignAchievement.ChannelCodeList.Add(x.ChannelCode);           
            }
            mappedCampaignAchievement.Campaign = new ParameterDto { Id = entity.Campaign.Id, Code = "", Name = entity.Campaign.Name };
            mappedCampaignAchievement.Rule = new ParameterDto { Id = mappedCampaignAchievement.Type, Code = "", 
                Name = Helpers.GetEnumDescription<AchievementType>(mappedCampaignAchievement.Type)
            };
            if (entity.Currency != null) 
            {
                mappedCampaignAchievement.Currency = new ParameterDto { Id = entity.Currency.Id, Code = "", Name = entity.Currency.Name };
            }
            mappedCampaignAchievement.AchievementType = new ParameterDto { Id = entity.AchievementType.Id, Code = "", Name = entity.AchievementType.Name };
            mappedCampaignAchievement.ActionOption = new ParameterDto { Id = entity.ActionOption.Id, Code = "", Name = entity.ActionOption.Name };

            return mappedCampaignAchievement;
        }

        public async Task<BaseResponse<CampaignAchievementInsertFormDto>> GetInsertFormAsync(int campaignId)
        {
            CampaignAchievementInsertFormDto response = new CampaignAchievementInsertFormDto();
            
            await FillForm(response, campaignId);

            return await BaseResponse<CampaignAchievementInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignAchievementInsertFormDto response, int campaignId)
        {
            response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
            response.ActionOptions = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

            response.IsInvisibleCampaign = await _campaignService.IsInvisibleCampaign(campaignId);
        }

        public async Task<BaseResponse<List<CampaignAchievementDto>>> GetListAsync()
        {
            List<CampaignAchievementDto> campaignAchievementList = _unitOfWork.GetRepository<CampaignAchievementEntity>().GetAll().Select(x => _mapper.Map<CampaignAchievementDto>(x)).ToList();
            return await BaseResponse<List<CampaignAchievementDto>>.SuccessAsync(campaignAchievementList);
        }

        public async Task<BaseResponse<CampaignAchievementUpdateFormDto>> GetUpdateFormAsync(int campaignId)
        {
            CampaignAchievementUpdateFormDto response = new CampaignAchievementUpdateFormDto();
            await FillForm(response, campaignId);
            response.CampaignAchievement = await GetCampaignAchievementDto(campaignId);

            return await BaseResponse<CampaignAchievementUpdateFormDto>.SuccessAsync(response);
        }

        async Task CheckValidationAsync(CampaignAchievementInsertRequest input)
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == input.CampaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            var campaignRuleEntity = await _unitOfWork.GetRepository<CampaignRuleEntity>()
               .GetAll(x => x.CampaignId == input.CampaignId && !x.IsDeleted)
               .FirstOrDefaultAsync();
            if (campaignRuleEntity == null)
            {
                throw new Exception("Kampanya kuralları girilmemiş.");
            }

            var campaignTargetEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>()
                .GetAll(x => x.CampaignId == input.CampaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignTargetEntity == null)
            {
                throw new Exception("Kampanya hedefleri girilmemiş.");
            }

            if (input.CampaignChannelCodeList== null || !input.CampaignChannelCodeList.Any())
                throw new Exception("Kanal kod listesi boş olamaz.");
            foreach(string code in input.CampaignChannelCodeList) 
            {
                var channelCode = (await _parameterService.GetCampaignChannelListAsync())?.Data?.Any(x => x == code);
                if (!channelCode.GetValueOrDefault(false))
                    throw new Exception("Kanal kodu hatalı.");
            }

            int viewOptionId = campaignEntity.ViewOptionId ?? 0;
            if(viewOptionId != (int)ViewOptionsEnum.InvisibleCampaign) 
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
            
            if (input.ActionOptionId <= 0)
                throw new Exception("Aksiyon seçilmelidir.");
            else
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
                if (!input.MaxAmount.HasValue || input.MaxAmount.Value <= 0)
                    throw new Exception("Max Tutar girilmelidir.");
            }
            else if (input.Type == (int)AchievementType.Rate)
            {
                if (!input.Rate.HasValue)
                    throw new Exception("Kazanım Oranı girilmelidir.");
                if (input.Rate > 100)
                    throw new Exception("Oran %100’den büyük bir değer girilemez.");
                if (input.Rate < 0)
                    throw new Exception("Oran %0’dan küçük bir değer girilemez");
            }
            else
                throw new Exception("Kazanım türü (Tutar/Oran) seçilmelidir.");
        }

        //public async Task<BaseResponse<ChannelsAndAchievementsByCampaignResponse>> GetListByCampaignAsync(int campaignId)
        //{
        //    ChannelsAndAchievementsByCampaignResponse response = new ChannelsAndAchievementsByCampaignResponse();

        //    //var achievements = _unitOfWork.GetRepository<CampaignAchievementEntity>()
        //    //                          .GetAll(x => !x.IsDeleted && x.CampaignId == campaignId)
        //    //                          .Include(x => x.ActionOption)
        //    //                          .Include(x => x.AchievementType)
        //    //                          //.OrderBy(x => x.CampaignChannelCode)
        //    //                          //.ThenBy(x => x.Type)
        //    //                          .ToList();

        //    //var channels = (await _parameterService.GetCampaignChannelListAsync()).Data;

        //    //foreach (var channel in channels)
        //    //{
        //    //    var _achievements = achievements.Where(x => x.CampaignChannelCode == channel).Select(x => new CampaignAchievementListDto
        //    //    {
        //    //        Id = x.Id,
        //    //        AchievementType = x.AchievementType?.Name,
        //    //        Action = x.ActionOption?.Name,
        //    //        Type = Helpers.GetEnumDescription(x.Type),
        //    //        MaxUtilization = x.MaxUtilization,
        //    //        //CampaignId = x.CampaignId,
        //    //        TypeId= (int)x.Type,
        //    //        CurrencyId = x.CurrencyId,
        //    //        Amount = x.Amount,
        //    //        Rate = x.Rate,
        //    //        MaxAmount = x.MaxAmount,
        //    //        AchievementTypeId = x.AchievementTypeId,
        //    //        ActionOptionId = x.ActionOptionId,
        //    //        DescriptionTr = x.DescriptionTr,
        //    //        DescriptionEn = x.DescriptionEn,
        //    //        TitleTr = x.TitleTr,
        //    //        TitleEn = x.TitleEn,
        //    //    }).ToList();

        //    //    response.ChannelsAndAchievements.Add(new ChannelsAndAchievementsByCampaignDto
        //    //    {
        //    //        CampaignChannelCode = channel,
        //    //        CampaignChannelName = channel,
        //    //        HasAchievement = _achievements.Any(),
        //    //        AchievementList = _achievements
        //    //    });

        //    //}

        //    return await BaseResponse<ChannelsAndAchievementsByCampaignResponse>.SuccessAsync(response);

        //}
    }
}
