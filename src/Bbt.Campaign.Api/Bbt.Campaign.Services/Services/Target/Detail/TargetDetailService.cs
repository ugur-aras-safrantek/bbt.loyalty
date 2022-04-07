using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Target.Detail;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Bbt.Campaign.Services.Services.Target.Detail
{
    public class TargetDetailService : ITargetDetailService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;

        public TargetDetailService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
        }
        public async Task<BaseResponse<TargetDetailDto>> AddAsync(TargetDetailInsertRequest request)
        {
            await CheckValidationAsync(request);

            var entity = _mapper.Map<TargetDetailEntity>(request);
            entity = await _unitOfWork.GetRepository<TargetDetailEntity>().AddAsync(entity);
            try 
            { 
            await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex) 
            { 
                string s = ex.ToString();
            }
            

            var mappedTarget = _mapper.Map<TargetDetailDto>(entity);
            return await BaseResponse<TargetDetailDto>.SuccessAsync(mappedTarget);
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
                return await BaseResponse<TargetDetailDto>.SuccessAsync(mappedTarget);
            }
            return await BaseResponse<TargetDetailDto>.FailAsync("Hedef bulunamadı.");
        }

        public async Task<BaseResponse<TargetDetailDto>> UpdateAsync(TargetDetailInsertRequest targetDetail)
        {
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

                await _unitOfWork.GetRepository<TargetDetailEntity>().UpdateAsync(entity);

                await _unitOfWork.SaveChangesAsync();

                return await GetTargetDetailAsync(entity.Id);
            }
            else 
            {
                return await AddAsync(targetDetail);
            }
        }

        public async Task<BaseResponse<TargetDetailInsertFormDto>> GetInsertFormAsync()
        {
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

        public async Task<BaseResponse<TargetDetailUpdateFormDto>> GetUpdateFormAsync(int targetId)
        {
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

            if(request.TargetSourceId == (int)TargetSourceEnum.Flow) 
            {
                var triggerTime = (await _parameterService.GetTriggerTimeListAsync())?.Data?.Any(x => x.Id == (request.TriggerTimeId ?? 0));
                if (!triggerTime.GetValueOrDefault(false))
                    throw new Exception("Tetiklenme Zamanı hatalı.");

                if (string.IsNullOrEmpty(request.FlowName) || string.IsNullOrWhiteSpace(request.FlowName))
                    throw new Exception("Akış ismi boş olamaz.");

                if (string.IsNullOrEmpty(request.FlowFrequency) || string.IsNullOrWhiteSpace(request.FlowFrequency))
                    throw new Exception("Akış frekansı boş olamaz.");

                //bool isValidCron = Quartz.CronExpression.IsValidExpression(request.FlowFrequency);
                //if (!isValidCron)
                //    throw new Exception("Akış frekansı cron formatı hatalı.");

                //Regex regex = new Regex(Helpers.cronRegex);
                //if (!regex.IsMatch(request.FlowFrequency))
                //    throw new Exception("Akış frekansı cron formatı hatalı.");


                decimal totalAmount = request.TotalAmount ?? 0;
                int numberOfTransaction = request.NumberOfTransaction ?? 0;
                if(totalAmount == 0 && numberOfTransaction == 0) 
                {
                    throw new Exception("Toplam tutar veya işlem adedi giriniz.");
                }
            }
            else if (request.TargetSourceId == (int)TargetSourceEnum.Query)
            {
                //VerificationTimeId
                var verificationTime = (await _parameterService.GetVerificationTimeListAsync())?.Data?.Any(x => x.Id == (request.VerificationTimeId ?? 0));
                if (!verificationTime.GetValueOrDefault(false))
                    throw new Exception("Kampanya Doğrulama Zamanı hatalı.");
            }

        }


    }
}
