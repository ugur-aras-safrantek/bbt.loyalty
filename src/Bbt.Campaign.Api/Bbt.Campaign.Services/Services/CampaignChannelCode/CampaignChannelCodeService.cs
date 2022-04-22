using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignChannelCode;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
using Bbt.Campaign.Services.Services.Campaign;
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
        private readonly ICampaignService _campaignService;

        public CampaignChannelCodeService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
        }

        public async Task<BaseResponse<CampaignChannelCodeDto>> UpdateAsync(CampaignChannelCodeUpdateRequest request) 
        {
            CampaignChannelCodeDto response = new CampaignChannelCodeDto();

            if (request.CampaignChannelCodeList == null || !request.CampaignChannelCodeList.Any())
                throw new Exception("Kanal kod listesi boş olamaz.");

            foreach (string code in request.CampaignChannelCodeList)
            {
                if(string.IsNullOrEmpty(code))
                    throw new Exception("Kanal kodu boş olamaz.");

                var channelCode = (await _parameterService.GetCampaignChannelListAsync())?.Data?.Any(x => x == code);
                if (!channelCode.GetValueOrDefault(false))
                    throw new Exception("Kanal kodu hatalı.");
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
                });
            });

            response.CampaignId = request.CampaignId;
            response.CampaignChannelCodeList = await GetCampaignChannelCodeList(request.CampaignId);

            return await BaseResponse<CampaignChannelCodeDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CampaignChannelCodeInsertFormDto>> GetInsertFormAsync(int campaignId)
        {
            CampaignChannelCodeInsertFormDto response = new CampaignChannelCodeInsertFormDto();

            await FillForm(response, campaignId);

            return await BaseResponse<CampaignChannelCodeInsertFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CampaignChannelCodeUpdateFormDto>> GetUpdateFormAsync(int campaignId)
        {
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
        private async Task FillForm(CampaignChannelCodeInsertFormDto response, int campaignId)
        {
            response.ChannelCodeList = (await _parameterService.GetCampaignChannelListAsync())?.Data;

            response.IsInvisibleCampaign = await _campaignService.IsInvisibleCampaign(campaignId);
        }
    }
}
