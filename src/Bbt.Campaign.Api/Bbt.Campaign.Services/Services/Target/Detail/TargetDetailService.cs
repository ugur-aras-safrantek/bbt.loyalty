using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Target.Detail;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Cronos;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.Target.Detail
{
    public class TargetDetailService : ITargetDetailService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationservice _authorizationservice;
        private static int moduleTypeId = (int)ModuleTypeEnum.Target;

        public TargetDetailService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, IAuthorizationservice authorizationservice)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationservice = authorizationservice;
        }
        public async Task<BaseResponse<TargetDetailDto>> AddAsync(TargetDetailInsertRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(request);

            var entity = _mapper.Map<TargetDetailEntity>(request);

            entity = await SetChanges(entity);
            entity.CreatedBy = userid;

            entity = await _unitOfWork.GetRepository<TargetDetailEntity>().AddAsync(entity);
            
            await _unitOfWork.SaveChangesAsync();
            
            var mappedTarget = _mapper.Map<TargetDetailDto>(entity);
            
            return await BaseResponse<TargetDetailDto>.SuccessAsync(mappedTarget);
        }

        private async Task<TargetDetailEntity> SetChanges(TargetDetailEntity entity)
        {
            if (entity.TargetSourceId == (int)TargetSourceEnum.Flow)
            {
                entity.Query = null;
                entity.Condition = null;
                entity.VerificationTimeId = null;
            }
            else
            {
                entity.FlowName = null;
                entity.TotalAmount = null;
                entity.NumberOfTransaction = null;
                entity.FlowFrequency = null;
                entity.AdditionalFlowTime = null;
                entity.TriggerTimeId = null;
            }
            return entity;
        }

        public async Task<BaseResponse<TargetDetailDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TargetDetailEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<TargetDetailEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetTargetDetailAsync(entity.Id);
            }
            return await BaseResponse<TargetDetailDto>.FailAsync("Kampanya bulunamadı.");
        }

        public Task<BaseResponse<TargetDetailListFilterResponse>> GetByFilterAsync(TargetDetailListFilterRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<TargetDetailDto>> GetTargetDetailAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TargetDetailEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                var mappedTarget = _mapper.Map<TargetDetailDto>(entity);
                mappedTarget.TotalAmountStr = Helpers.ConvertNullablePriceString(mappedTarget.TotalAmount);
                return await BaseResponse<TargetDetailDto>.SuccessAsync(mappedTarget);
            }
            return await BaseResponse<TargetDetailDto>.FailAsync("Hedef bulunamadı.");
        }

        public async Task<BaseResponse<TargetDetailDto>> GetByTargetAsync(int targetId)
        {
            var entity = await _unitOfWork.GetRepository<TargetDetailEntity>()
                .GetAll(x => x.TargetId == targetId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (entity != null)
            {
                var mappedTarget = _mapper.Map<TargetDetailDto>(entity);
                mappedTarget.TotalAmountStr = Helpers.ConvertNullablePriceString(mappedTarget.TotalAmount);
                return await BaseResponse<TargetDetailDto>.SuccessAsync(mappedTarget);
            }
            return await BaseResponse<TargetDetailDto>.FailAsync("Hedef bulunamadı.");
        }

        public async Task<BaseResponse<TargetDetailDto>> UpdateAsync(TargetDetailInsertRequest targetDetail, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(targetDetail);

            var entity = await _unitOfWork.GetRepository<TargetDetailEntity>()
                .GetAll(x => x.TargetId == targetDetail.TargetId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (entity != null)
            {
                entity.AdditionalFlowTime = targetDetail.AdditionalFlowTime;
                entity.FlowFrequency = targetDetail.FlowFrequency;
                entity.TotalAmount = targetDetail.TotalAmount;
                entity.Condition = targetDetail.Condition;
                entity.DescriptionEn = targetDetail.DescriptionEn;
                entity.DescriptionTr = targetDetail.DescriptionTr;
                entity.FlowName = targetDetail.FlowName;
                entity.NumberOfTransaction = targetDetail.NumberOfTransaction;
                entity.Query = targetDetail.Query;
                entity.TargetDetailEn = targetDetail.TargetDetailEn;
                entity.TargetDetailTr = targetDetail.TargetDetailTr;
                entity.TargetSourceId = targetDetail.TargetSourceId;
                entity.TargetViewTypeId = targetDetail.TargetViewTypeId;
                entity.TriggerTimeId = targetDetail.TriggerTimeId;
                entity.VerificationTimeId = targetDetail.VerificationTimeId;
                entity.LastModifiedBy = userid;

                entity = await SetChanges(entity);

                await _unitOfWork.GetRepository<TargetDetailEntity>().UpdateAsync(entity);

                await _unitOfWork.SaveChangesAsync();

                return await GetTargetDetailAsync(entity.Id);
            }
            else 
            {
                return await AddAsync(targetDetail, userid);
            }
        }

        public async Task<BaseResponse<TargetDetailInsertFormDto>> GetInsertFormAsync(string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            TargetDetailInsertFormDto response = new TargetDetailInsertFormDto();
            await FillForm(response);

            return await BaseResponse<TargetDetailInsertFormDto>.SuccessAsync(response);
        }

        private async Task FillForm(TargetDetailInsertFormDto response)
        {
            response.TargetSourceList = (await _parameterService.GetTargetSourceListAsync())?.Data;
            response.TriggerTimeList = (await _parameterService.GetTriggerTimeListAsync())?.Data;
            response.TargetViewTypeList = (await _parameterService.GetTargetViewTypeListAsync())?.Data;
            response.VerificationTimeList = (await _parameterService.GetVerificationTimeListAsync())?.Data;
        }

        public async Task<BaseResponse<TargetDetailUpdateFormDto>> GetUpdateFormAsync(int targetId, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            TargetDetailUpdateFormDto response = new TargetDetailUpdateFormDto();

            await FillForm(response);

            response.TargetDetail = (await GetByTargetAsync(targetId))?.Data;

            return await BaseResponse<TargetDetailUpdateFormDto>.SuccessAsync(response);
        }

        private async Task CheckValidationAsync(TargetDetailInsertRequest request) 
        {
            if (request.TargetId == 0)
                throw new Exception("Hedef seçiniz.");

            var targetSource = (await _parameterService.GetTargetSourceListAsync())?.Data?.Any(x => x.Id == request.TargetSourceId);
            if (!targetSource.GetValueOrDefault(false))
                throw new Exception("Hedef kaynağı hatalı.");

            var targetViewType = (await _parameterService.GetTargetViewTypeListAsync())?.Data?.Any(x => x.Id == request.TargetViewTypeId);
            if (!targetViewType.GetValueOrDefault(false))
                throw new Exception("Hedef gösterim tipi hatalı.");

            //DetailTr girildiyse DetailTr zorunludur
            if (!string.IsNullOrWhiteSpace(request.TargetDetailTr) && !string.IsNullOrEmpty(request.TargetDetailTr))
            {
                if (string.IsNullOrWhiteSpace(request.TargetDetailEn) || string.IsNullOrEmpty(request.TargetDetailEn))
                    throw new Exception("Hedef Detay (İngilizce) girilmelidir.");
            }

            //DetailTr girildiyse DetailTr zorunludur
            if (!string.IsNullOrWhiteSpace(request.DescriptionTr) && !string.IsNullOrEmpty(request.DescriptionTr))
            {
                if (string.IsNullOrWhiteSpace(request.DescriptionEn) || string.IsNullOrEmpty(request.DescriptionEn))
                    throw new Exception("Açıklama (İngilizce) girilmelidir.");
            }

            if (request.TargetSourceId == (int)TargetSourceEnum.Flow) 
            {
                var triggerTime = (await _parameterService.GetTriggerTimeListAsync())?.Data?.Any(x => x.Id == (request.TriggerTimeId ?? 0));
                if (!triggerTime.GetValueOrDefault(false))
                    throw new Exception("Tetiklenme Zamanı hatalı.");

                if (string.IsNullOrEmpty(request.FlowName) || string.IsNullOrWhiteSpace(request.FlowName))
                    throw new Exception("Akış ismi boş olamaz.");

                if (string.IsNullOrEmpty(request.FlowFrequency) || string.IsNullOrWhiteSpace(request.FlowFrequency))
                    throw new Exception("Akış frekansı boş olamaz.");

                try
                {
                    CronExpression expression = CronExpression.Parse(request.FlowFrequency);

                    DateTime? nextUtc = expression.GetNextOccurrence(DateTime.UtcNow);
                }
                catch (CronFormatException ex)
                {
                    //throw new Exception("Akış frekansı cron formatı hatalı. Hata detayı : " + ex.Message);
                    throw new Exception("Akış frekansı cron formatı hatalı.");
                }

                string[] flowFrequencyArray = request.FlowFrequency.Split(' ');
                if(flowFrequencyArray[4].Contains("0")) 
                {
                    throw new Exception("Akış frekansı cron formatı hatalı.");
                }

                if (!string.IsNullOrEmpty(request.AdditionalFlowTime) && !string.IsNullOrWhiteSpace(request.AdditionalFlowTime)) 
                {
                    try 
                    { 
                        TimeSpan ts = TimeSpan.Parse(request.AdditionalFlowTime);
                    }
                    catch(Exception ex) 
                    {
                        throw new Exception("Akış Ek Süresi timespan formatında olmalıdır.");
                    }
                }
                
                int numberOfTransaction = request.NumberOfTransaction ?? 0;
                if(numberOfTransaction == 0 && (request.TotalAmount ?? 0) == 0) 
                {
                    throw new Exception("Toplam tutar veya işlem adedi giriniz.");
                }

                if (numberOfTransaction > 0 && (request.TotalAmount ?? 0) > 0)
                {
                    throw new Exception("Toplam tutar ve işlem adedi aynı anda girilemez.");
                }
            }
            else if (request.TargetSourceId == (int)TargetSourceEnum.Query)
            {
                //VerificationTimeId
                var verificationTime = (await _parameterService.GetVerificationTimeListAsync())?.Data?.Any(x => x.Id == (request.VerificationTimeId ?? 0));
                if (!verificationTime.GetValueOrDefault(false))
                    throw new Exception("Kampanya Doğrulama Zamanı hatalı.");

                if (string.IsNullOrEmpty(request.Query) || string.IsNullOrWhiteSpace(request.Query))
                    throw new Exception("Sorgu boş olamaz.");

                if (string.IsNullOrEmpty(request.Condition) || string.IsNullOrWhiteSpace(request.Condition))
                    throw new Exception("Koşul boş olamaz.");
            }
        }
    }
}
