using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignDocument;
using Bbt.Campaign.Public.Models.CampaignTopLimit;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bbt.Campaign.Services.Services.CampaignTopLimit
{
    public class CampaignTopLimitService : ICampaignTopLimitService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;

        public CampaignTopLimitService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
        }

        public async Task<BaseResponse<TopLimitDto>> AddAsync(CampaignTopLimitInsertRequest campaignTopLimit)
        {
            await CheckValidationAsync(campaignTopLimit);
            var entity = _mapper.Map<TopLimitEntity>(campaignTopLimit);
            entity.IsDraft = true;
            entity.IsApproved = false;
            entity = await SetTopLimitChanges(entity);
            var campaignIds = campaignTopLimit.CampaignIds.Distinct().ToList();

            //Campaigns
            if (campaignTopLimit.CampaignIds is { Count: > 0 })
            {
                entity.TopLimitCampaigns = new List<CampaignTopLimitEntity>();
                campaignIds.ForEach(x =>
                {
                    entity.TopLimitCampaigns.Add(new CampaignTopLimitEntity()
                    {
                        CampaignId = x,
                    });
                });
            }

            await _unitOfWork.GetRepository<TopLimitEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            var mappedTopLimit = _mapper.Map<TopLimitDto>(entity);

            return await BaseResponse<TopLimitDto>.SuccessAsync(mappedTopLimit);

        }

        public async Task<BaseResponse<TopLimitDto>> UpdateAsync(CampaignTopLimitUpdateRequest request)
        {
            await CheckValidationAsync(request);

            if (request.Id <= 0)
                throw new Exception("Kampanya Çatı Limiti bulunamadı.");
            var entity = await _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => !x.IsDeleted && x.Id == request.Id)
                .Include(x => x.TopLimitCampaigns).FirstOrDefaultAsync();

            if (entity is null)
                throw new Exception("Kampanya Çatı Limiti bulunamadı.");

            var campaignIds = request.CampaignIds.Distinct().ToList();

            entity.AchievementFrequencyId = request.AchievementFrequencyId;
            entity.CurrencyId = request.CurrencyId;
            entity.IsActive = request.IsActive;
            entity.MaxTopLimitAmount = request.MaxTopLimitAmount;
            entity.MaxTopLimitRate = request.MaxTopLimitRate;
            entity.MaxTopLimitUtilization = request.MaxTopLimitUtilization;
            entity.Name = request.Name;
            entity.Type = request.Type;
            entity.IsDraft = true;
            entity.IsApproved = false;

            entity = await SetTopLimitChanges(entity);

            foreach (var requestEntityDelete in _unitOfWork.GetRepository<CampaignTopLimitEntity>().GetAll(x => !x.IsDeleted
                     && x.TopLimitId == request.Id))
            {
                await _unitOfWork.GetRepository<CampaignTopLimitEntity>().DeleteAsync(requestEntityDelete);
            }
            foreach (int campaignId in campaignIds)
            {
                await _unitOfWork.GetRepository<CampaignTopLimitEntity>().AddAsync(new CampaignTopLimitEntity()
                {
                    CampaignId = campaignId,
                    TopLimitId = request.Id
                });
            }

            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignTopLimitAsync(request.Id);
        }

        private async Task<TopLimitEntity> SetTopLimitChanges(TopLimitEntity entity)
        {
            if (entity.Type == TopLimitType.Amount)
            {
                entity.MaxTopLimitRate = null;
            }
            else if (entity.Type == TopLimitType.Rate)
            {
                entity.MaxTopLimitAmount = null;
                entity.CurrencyId = null;

                if (entity.MaxTopLimitRate > 100)
                    throw new Exception("Oran %100’den büyük bir değer girilemez.");
                if (entity.MaxTopLimitRate < 0)
                    throw new Exception("Oran %0’dan küçük bir değer girilemez");
            }
            entity.IsDeleted = false;
            return entity;
        }

        public async Task<BaseResponse<TopLimitDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TopLimitEntity>().GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Kampanya Çatı Limiti bulunamadı.");

            entity.IsDeleted = true;
            await _unitOfWork.GetRepository<TopLimitEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignTopLimitAsync(entity.Id);
        }

        public async Task<BaseResponse<TopLimitDto>> GetCampaignTopLimitAsync(int id)
        {
            var campaignTopLimitEntity = await _unitOfWork.GetRepository<TopLimitEntity>()
                                                          .GetAll(x => x.Id == id && x.IsDeleted == false)
                                                          .Include(x => x.TopLimitCampaigns).ThenInclude(x => x.Campaign)
                                                          .Include(x => x.Currency)
                                                          .Include(x => x.AchievementFrequency).FirstOrDefaultAsync();
            if (campaignTopLimitEntity != null)
            {
                TopLimitDto mappedCampaignTopLimit = new TopLimitDto
                {
                    AchievementFrequency = new Public.Dtos.ParameterDto { Id = campaignTopLimitEntity.AchievementFrequency.Id, Name = campaignTopLimitEntity.AchievementFrequency.Name },
                    AchievementFrequencyId = campaignTopLimitEntity.AchievementFrequencyId,
                    Campaigns = campaignTopLimitEntity.TopLimitCampaigns.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
                    {
                        Id = c.CampaignId,
                        Name = c.Campaign.Name
                    }).ToList(),
                    Currency = campaignTopLimitEntity.Currency is null ? null : new Public.Dtos.ParameterDto { Id = campaignTopLimitEntity.Currency?.Id??0, Code = campaignTopLimitEntity.Currency?.Code, Name = campaignTopLimitEntity.Currency?.Name },
                    CurrencyId = campaignTopLimitEntity.CurrencyId,
                    Id = campaignTopLimitEntity.Id,
                    IsActive = campaignTopLimitEntity.IsActive,
                    MaxTopLimitAmount = campaignTopLimitEntity.MaxTopLimitAmount,
                    MaxTopLimitRate = campaignTopLimitEntity.MaxTopLimitRate,
                    MaxTopLimitUtilization = campaignTopLimitEntity.MaxTopLimitUtilization,
                    MaxTopLimitAmountStr = Core.Helper.Helpers.ConvertNullablePriceString(campaignTopLimitEntity.MaxTopLimitAmount),
                    MaxTopLimitRateStr = Core.Helper.Helpers.ConvertNullablePriceString(campaignTopLimitEntity.MaxTopLimitRate),
                    MaxTopLimitUtilizationStr = Core.Helper.Helpers.ConvertNullablePriceString(campaignTopLimitEntity.MaxTopLimitUtilization),
                    Name = campaignTopLimitEntity.Name,
                    Type = campaignTopLimitEntity.Type
                };
                return await BaseResponse<TopLimitDto>.SuccessAsync(mappedCampaignTopLimit);
            }
            
            
            throw new Exception("Kampanya Çatı Limiti bulunamadı.");
        }

        public async Task<BaseResponse<CampaignTopLimitInsertFormDto>> GetInsertForm()
        {
            CampaignTopLimitInsertFormDto response = new CampaignTopLimitInsertFormDto();
            await FillForm(response);

            return await BaseResponse<CampaignTopLimitInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(CampaignTopLimitInsertFormDto response)
        {
            response.CurrencyList = (await _parameterService.GetCurrencyListAsync())?.Data;
            response.AchievementFrequencyList = (await _parameterService.GetAchievementFrequencyListAsync())?.Data;
            response.CampaignList = (await _campaignService.GetParameterListAsync())?.Data;
        }

        public async Task<BaseResponse<List<TopLimitDto>>> GetListAsync()
        {
            List<TopLimitDto> campaignTopLimitList = _unitOfWork.GetRepository<TopLimitEntity>()
                .GetAll()
                .Select(x => new TopLimitDto
                {
                AchievementFrequency = new Public.Dtos.ParameterDto { Id = x.AchievementFrequency.Id, Name = x.AchievementFrequency.Name },
                AchievementFrequencyId = x.AchievementFrequencyId,
                Campaigns = x.TopLimitCampaigns.Where(c => !c.IsDeleted).Select(c => new Public.Dtos.ParameterDto
                {
                    Id = c.CampaignId,
                    Name = c.Campaign.Name
                }).ToList(),
                Currency = new Public.Dtos.ParameterDto { Id = x.Currency.Id, Code = x.Currency.Code, Name = x.Currency.Name },
                CurrencyId = x.CurrencyId,
                Id = x.Id,
                IsActive = x.IsActive,
                MaxTopLimitAmount = x.MaxTopLimitAmount,
                MaxTopLimitRate = x.MaxTopLimitRate,
                MaxTopLimitUtilization = x.MaxTopLimitUtilization,
                Name = x.Name,
                Type = x.Type
            }).ToList();

            return await BaseResponse<List<TopLimitDto>>.SuccessAsync(campaignTopLimitList);
        }

        public async Task<BaseResponse<CampaignTopLimitUpdateFormDto>> GetUpdateForm(int id)
        {
            CampaignTopLimitUpdateFormDto response = new CampaignTopLimitUpdateFormDto();
            await FillForm(response);
            response.CampaignTopLimit = (await GetCampaignTopLimitAsync(id))?.Data;

            return await BaseResponse<CampaignTopLimitUpdateFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<CampaignTopLimitListFilterResponse>> GetByFilterAsync(CampaignTopLimitListFilterRequest request)
        {
            CampaignTopLimitListFilterResponse response = new CampaignTopLimitListFilterResponse();
            List<CampaignTopLimitListDto> campaignList = await GetFilteredCampaignTopLimitList(request); 
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignList.Count();
            if (!campaignList.Any())
                return await BaseResponse<CampaignTopLimitListFilterResponse>.SuccessAsync(response, "Çatı limiti bulunamadı");
            campaignList = campaignList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            response.ResponseList = campaignList;
            response.Paging = Core.Helper.Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CampaignTopLimitListFilterResponse>.SuccessAsync(response);
        }

        public async Task<BaseResponse<GetFileResponse>> GetExcelAsync(CampaignTopLimitListFilterRequest request)
        {
            var response = new GetFileResponse();

            try 
            {
                List<CampaignTopLimitListDto> campaignList = await GetFilteredCampaignTopLimitList(request);

                if (campaignList.Count() == 0)
                    return await BaseResponse<GetFileResponse>.FailAsync("Çatı Limiti bulunamadı");

                byte[] data = ListFileOperations.GetTopLimitListExcel(campaignList);

                response = new GetFileResponse()
                {
                    Document = new DocumentModel()
                    {
                        Data = Convert.ToBase64String(data, 0, data.Length),
                        DocumentName = "Çatı Limiti Listesi.xlsx",
                        DocumentType = DocumentTypePublicEnum.ExcelReport,
                        MimeType = MimeTypeExtensions.ToMimeType(".xlsx")
                    }
                };

                return await BaseResponse<GetFileResponse>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await BaseResponse<GetFileResponse>.FailAsync("Hata oluştu." + ex.Message);
            }
        }

        private async Task<List<CampaignTopLimitListDto>> GetFilteredCampaignTopLimitList(CampaignTopLimitListFilterRequest request) 
        {
            Core.Helper.Helpers.ListByFilterCheckValidation(request);

            var campaignTopLimitQuery = _unitOfWork.GetRepository<TopLimitEntity>().GetAll(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Name))
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.Name.ToLower().Contains(request.Name.ToLower()));
            if (request.AchievementFrequencyId.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.AchievementFrequencyId == request.AchievementFrequencyId.Value);
            if (request.CurrencyId.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.CurrencyId == request.CurrencyId);
            if (request.MaxTopLimitAmount.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.MaxTopLimitAmount == request.MaxTopLimitAmount.Value);
            if (request.MaxTopLimitRate.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.MaxTopLimitRate == request.MaxTopLimitRate.Value);
            if (request.MaxTopLimitUtilization.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.MaxTopLimitUtilization == request.MaxTopLimitUtilization);
            if (request.Type.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.Type == Enum.Parse<TopLimitType>(request.Type.ToString()));
            if (request.IsActive.HasValue)
                campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.IsActive == request.IsActive.Value);
            //if (request.IsDraft.HasValue)
            //    campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.IsDraft == request.IsDraft.Value);
            //if (request.IsApproved.HasValue)
            //    campaignTopLimitQuery = campaignTopLimitQuery.Where(x => x.IsApproved == request.IsApproved.Value);

            var campaignList = campaignTopLimitQuery.Select(x => new CampaignTopLimitListDto
            {
                Id = x.Id,
                AchievementFrequency = x.AchievementFrequency.Name,
                Amount = x.Type == Public.Enums.TopLimitType.Amount,
                Rate = x.Type == Public.Enums.TopLimitType.Rate,
                Name = x.Name,
                Currency = x.Currency == null ? null : x.Currency.Name,
                MaxTopLimitAmount = x.MaxTopLimitAmount,
                MaxTopLimitRate = x.MaxTopLimitRate,
                MaxTopLimitUtilization = x.MaxTopLimitUtilization,
                MaxTopLimitAmountStr = Core.Helper.Helpers.ConvertNullablePriceString(x.MaxTopLimitAmount),
                MaxTopLimitRateStr = Core.Helper.Helpers.ConvertNullablePriceString(x.MaxTopLimitRate),
                MaxTopLimitUtilizationStr = Core.Helper.Helpers.ConvertNullablePriceString(x.MaxTopLimitUtilization),
                IsActive = x.IsActive,
            }).ToList();

            if (string.IsNullOrEmpty(request.SortBy))
            {
                campaignList = campaignList.OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                if (request.SortBy.EndsWith("Str"))
                    request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

                bool isDescending = request.SortDir?.ToLower() == "desc";
                if (isDescending)
                    campaignList = campaignList.OrderByDescending(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
                else
                    campaignList = campaignList.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
            }

            return campaignList;
        }

        public async Task<BaseResponse<CampaignTopLimitFilterParameterResponse>> GetFilterParameterList()
        {
            CampaignTopLimitFilterParameterResponse response = new CampaignTopLimitFilterParameterResponse();
            await FillForm(response);
            response.TypeList = Core.Helper.Helpers.EnumToList<TopLimitType>().Select(x => new Public.Dtos.ParameterDto
            {
                Id = x.Key,
                Name = x.Value
            }).ToList();

            return await BaseResponse<CampaignTopLimitFilterParameterResponse>.SuccessAsync(response);
        }

        async Task CheckValidationAsync(CampaignTopLimitInsertBaseRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                throw new Exception("Çatı Limiti Adı girilmelidir.");

            if (input.AchievementFrequencyId <= 0)
                throw new Exception("Kazanım sıklığı seçilmelidir.");
            else
            {
                var achievementFrequency = (await _parameterService.GetAchievementFrequencyListAsync())?.Data?.Any(x => x.Id == input.AchievementFrequencyId);
                if (!achievementFrequency.GetValueOrDefault(false))
                    throw new Exception("Kazanım sıklığı seçilmelidir.");
            }

            if (input.Type == TopLimitType.Amount)
            {
                if (!input.CurrencyId.HasValue)
                    throw new Exception("Para Birimi seçiniz.");

                if (input.CurrencyId <= 0)
                    throw new Exception("Para Birimi hatalı.");

                var currency = (await _parameterService.GetCurrencyListAsync())?.Data?.Any(x => x.Id == input.CurrencyId);
                if (!currency.GetValueOrDefault(false))
                    throw new Exception("Para Birimi hatalı.");

                if (!input.MaxTopLimitAmount.HasValue)
                    throw new Exception("Çatı Max Tutar girilmelidir.");

                if (input.MaxTopLimitAmount <= 0)
                    throw new Exception("Çatı Max Tutar girilmelidir.");
            }
            else if (input.Type == TopLimitType.Rate)
            {
                if (!input.MaxTopLimitRate.HasValue)
                    throw new Exception("Çatı oranı girilmelidir.");

                if (input.MaxTopLimitRate > 100)
                    throw new Exception("“Çatı Oranı % 100’ün üzerinde bir değer girilemez.");

                if (input.MaxTopLimitRate <= 0)
                    throw new Exception("Çatı Oranı girilmelidir.");
            }
            else
                throw new Exception("Çatı limit tipi seçilmelidir.");

            if (input.CampaignIds == null || input.CampaignIds.Count < 2)
                throw new Exception("En az 2 Kampanya adı seçilmelidir.");
            else if (input.CampaignIds.Any(x => x <= 0))
                throw new Exception($"Hatalı Kampanya bilgisi (Kampanya Id: {input.CampaignIds.FirstOrDefault(x => x <= 0)}).");

            foreach(int campaignId in input.CampaignIds) 
            {
                var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
                if (campaignEntity == null)
                {
                    throw new Exception("Kampanya bulunamadı.");
                }
            }

        }
    }
}
