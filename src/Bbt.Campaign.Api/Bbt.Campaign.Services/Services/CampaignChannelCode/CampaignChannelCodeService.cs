using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignChannelCode;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignChannelCode
{
    public class CampaignChannelCodeService : ICampaignChannelCodeService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDraftService _draftService;
        private static int moduleTypeId = (int)ModuleTypeEnum.Campaign;

        public CampaignChannelCodeService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            IAuthorizationService authorizationservice, IDraftService draftService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationservice;
            _draftService = draftService;
        }

        public async Task<BaseResponse<CampaignChannelCodeDto>> AddAsync(CampaignChannelCodeUpdateRequest request, UserRoleDto2 userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;

            await _authorizationService.CheckAuthorizationAsync2(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(request);

            CampaignChannelCodeDto response = new CampaignChannelCodeDto();

            await Update(request, userRole.UserId);

            response.CampaignId = request.CampaignId;
            response.CampaignChannelCodeList = await GetCampaignChannelCodeList(request.CampaignId);

            return await BaseResponse<CampaignChannelCodeDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<CampaignChannelCodeDto>> UpdateAsync(CampaignChannelCodeUpdateRequest request, UserRoleDto2 userRole) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationService.CheckAuthorizationAsync2(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(request);

            CampaignChannelCodeDto response = new CampaignChannelCodeDto();
            await Update(request, userRole.UserId);

            response.CampaignId = request.CampaignId;
            response.CampaignChannelCodeList = await GetCampaignChannelCodeList(request.CampaignId);

            return await BaseResponse<CampaignChannelCodeDto>.SuccessAsync(response);
        }
        private async Task Update(CampaignChannelCodeUpdateRequest request, string userid) 
        {
            int processTypeId = await _draftService.GetCampaignProcessType(request.CampaignId);
            if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
            {
                request.CampaignId = await _draftService.CreateCampaignDraftAsync(request.CampaignId, userid, (int)PageTypeEnum.CampaignChannelCode);
            }
            else
            {
                var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == request.CampaignId).FirstOrDefault();
                if (campaignUpdatePageEntity != null)
                {
                    campaignUpdatePageEntity.IsCampaignChannelCodeUpdated = true;
                    await _unitOfWork.GetRepository<CampaignUpdatePageEntity>().UpdateAsync(campaignUpdatePageEntity);
                }
            }

            foreach (var deleteEntity in _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
                .GetAll(x => x.CampaignId == request.CampaignId && x.IsDeleted != true))
            {
                await _unitOfWork.GetRepository<CampaignChannelCodeEntity>().DeleteAsync(deleteEntity);
            }

            request.CampaignChannelCodeList.ForEach(x =>
            {
                _unitOfWork.GetRepository<CampaignChannelCodeEntity>().AddAsync(new CampaignChannelCodeEntity()
                {
                    CampaignId = request.CampaignId,
                    ChannelCode = x,
                    CreatedBy = userid,
                });
            });
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<BaseResponse<CampaignChannelCodeInsertFormDto>> GetInsertFormAsync(int campaignId, UserRoleDto2 userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync2(userRole, moduleTypeId, authorizationTypeId);

            CampaignChannelCodeInsertFormDto response = new CampaignChannelCodeInsertFormDto();

            await FillForm(response, campaignId);

            return await BaseResponse<CampaignChannelCodeInsertFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CampaignChannelCodeUpdateFormDto>> GetUpdateFormAsync(int campaignId, UserRoleDto2 userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync2(userRole, moduleTypeId, authorizationTypeId);

            CampaignChannelCodeUpdateFormDto response = new CampaignChannelCodeUpdateFormDto();
            
            await FillForm(response, campaignId);

            response.CampaignChannelCodeList = await GetCampaignChannelCodeList(campaignId);

            return await BaseResponse<CampaignChannelCodeUpdateFormDto>.SuccessAsync(response);
        }
        public async Task<List<string>> GetCampaignChannelCodeList(int campaignId) 
        {
            return await _unitOfWork.GetRepository<CampaignChannelCodeEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.IsDeleted != true)
                .Select(x => x.ChannelCode)
                .ToListAsync();
        }

        public async Task<string> GetCampaignChannelCodesAsString(int campaignId) 
        {
            var campaignChannelCodeList = await GetCampaignChannelCodeList(campaignId);
            return string.Join(",", campaignChannelCodeList);
        }

        private async Task FillForm(CampaignChannelCodeInsertFormDto response, int campaignId)
        {
            response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

            CampaignProperty campaignProperty = await _draftService.GetCampaignProperties(campaignId);
            response.IsUpdatableCampaign = campaignProperty.IsUpdatableCampaign;
            response.IsInvisibleCampaign = campaignProperty.IsInvisibleCampaign;
        }
        async Task CheckValidationAsync(CampaignChannelCodeUpdateRequest request) 
        {
            if (request.CampaignId == 0)
                throw new Exception("Kampanya giriniz.");

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == request.CampaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            if (request.CampaignChannelCodeList == null || !request.CampaignChannelCodeList.Any())
                throw new Exception("Kanal kod listesi boş olamaz.");

            foreach (string code in request.CampaignChannelCodeList)
            {
                if (string.IsNullOrEmpty(code))
                    throw new Exception("Kanal kodu boş olamaz.");

                var channelCode = (await _parameterService.GetCampaignChannelListAsync())?.Data?.Any(x => x == code);
                if (!channelCode.GetValueOrDefault(false))
                    throw new Exception("Kanal kodu hatalı.");
            }

        }
    }
}
