using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Target.Services.Services.Target
{
    public class TargetService : ITargetService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;

        public TargetService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
        }

        public async Task<BaseResponse<TargetDto>> AddAsync(TargetInsertRequest Target)
        {
            await CheckValidationAsync(Target);

            var entity = _mapper.Map<TargetEntity>(Target);
            entity.IsApproved = false;
            entity.IsDraft = true;
            entity = await _unitOfWork.GetRepository<TargetEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var mappedTarget = _mapper.Map<TargetDto>(entity);
            return await BaseResponse<TargetDto>.SuccessAsync(mappedTarget);
        }

        public async Task<BaseResponse<TargetDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<TargetEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<TargetEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetTargetAsync(entity.Id);
            }
            return await BaseResponse<TargetDto>.FailAsync("Kampanya bulunamadı.");
        }

        public async Task<BaseResponse<TargetDto>> GetTargetAsync(int id)
        {
            var targetDto = await GetTargetDto(id);
            {
                if (targetDto != null)
                {
                    return await BaseResponse<TargetDto>.SuccessAsync(targetDto);
                }
            }
            return await BaseResponse<TargetDto>.FailAsync("Hedef bulunamadı.");
        }

        private async Task<TargetDto> GetTargetDto(int id)
        {
            var targetEntity = await _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.TargetDetail)
                .Select(x => _mapper.Map<TargetDto>(x))
                .FirstOrDefaultAsync();
            if (targetEntity == null)
            {
                return null;
            }
            var mappedTarget = _mapper.Map<TargetDto>(targetEntity);
            return  mappedTarget;
        }

        public async Task<BaseResponse<TargetViewFormDto>> GetTargetViewFormAsync(int id) 
        {
            var response = new TargetViewFormDto();

            response.Target = await GetTargetDto(id);

            response.TargetSourceList = (await _parameterService.GetTargetSourceListAsync())?.Data;
            response.TriggerTimeList = (await _parameterService.GetTriggerTimeListAsync())?.Data;
            response.TargetViewTypeList = (await _parameterService.GetTargetViewTypeListAsync())?.Data;
            response.VerificationTimeList = (await _parameterService.GetVerificationTimeListAsync())?.Data;

            return await BaseResponse<TargetViewFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<List<TargetDto>>> GetListAsync()
        {
            var mappedTargets = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => !x.IsDeleted && !x.IsDraft)
                .Select(x => _mapper.Map<TargetDto>(x))
                .ToList();
            return await BaseResponse<List<TargetDto>>.SuccessAsync(mappedTargets);
        }

        public async Task<BaseResponse<List<TargetDto>>> GetDraftListAsync()
        {
            var mappedTargets = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => !x.IsDeleted && x.IsDraft)
                .Select(x => _mapper.Map<TargetDto>(x))
                .ToList();
            return await BaseResponse<List<TargetDto>>.SuccessAsync(mappedTargets);
        }

        public async Task<BaseResponse<TargetDto>> UpdateAsync(TargetUpdateRequest Target)
        {
            await CheckValidationAsync(Target);
            var entity = _unitOfWork.GetRepository<TargetEntity>().GetAllIncluding(x => x.TargetDetail).Where(x => x.Id == Target.Id).FirstOrDefault();
            if (entity != null)
            {
                entity.Title= Target.Title;
                entity.Name= Target.Name;
                entity.IsActive = Target.IsActive;
                entity.IsApproved = false;
                entity.IsDraft = true; 
                await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetTargetAsync(Target.Id);
            }
            return await BaseResponse<TargetDto>.FailAsync("Kampanya bulunamadı.");
        }

        async Task CheckValidationAsync(TargetInsertRequest input)
        {
            //if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrEmpty(input.Name))
            //    throw new Exception("Hedef Adı girilmelidir.");

            //file eklenmeli
        }

        public async Task<BaseResponse<TargetListFilterResponse>> GetByFilterAsync(TargetListFilterRequest request)
        {
            var response = new TargetListFilterResponse();
            try 
            {
                Helpers.ListByFilterCheckValidation(request);
                List<TargetListDto> targetList = await GetFilteredTargetList(request);
                if (!targetList.Any())
                    return await BaseResponse<TargetListFilterResponse>.SuccessAsync(response, "Hedef bulunamadı"); 
                var totalItems = targetList.Count();
                var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
                var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
                targetList = targetList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
               
                response.ResponseList = targetList;
                response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);

                return await BaseResponse<TargetListFilterResponse>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await BaseResponse<TargetListFilterResponse>.FailAsync("Hata oluştu." + ex.Message);
            } 
        }

        public async Task<BaseResponse<GetFileResponse>> GetExcelAsync(TargetListFilterRequest request)
        {
            var response = new GetFileResponse();

            try 
            {
                List<TargetListDto> targetList = await GetFilteredTargetList(request);
                if (!targetList.Any())
                    return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Hedef bulunamadı");
                byte[] data = ListFileOperations.GetTargetListExcel(targetList);

                response = new GetFileResponse()
                {
                    Document = new Campaign.Public.Models.CampaignDocument.DocumentModel()
                    {
                        Data = Convert.ToBase64String(data, 0, data.Length),
                        DocumentName = "Hedef Listesi.xlsx",
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

        private  async Task<List<TargetListDto>> GetFilteredTargetList(TargetListFilterRequest request) 
        {
            Helpers.ListByFilterCheckValidation(request);

            var targetQuery = _unitOfWork.GetRepository<TargetEntity>().GetAll(x => !x.IsDeleted);

            if (request.Id.HasValue)
                targetQuery = targetQuery.Where(x => x.Id == request.Id);
            if (!string.IsNullOrWhiteSpace(request.Name))
                targetQuery = targetQuery.Where(x => x.Name.Contains(request.Name));
            if (request.TargetSourceId.HasValue)
                targetQuery = targetQuery.Where(x => x.TargetDetail.TargetSourceId == request.TargetSourceId.Value);
            if (request.IsActive.HasValue)
                targetQuery = targetQuery.Where(x => x.IsActive == request.IsActive.Value);
            if (request.TargetViewTypeId.HasValue)
                targetQuery = targetQuery.Where(x => x.TargetDetail.TargetViewTypeId == request.TargetViewTypeId.Value);
            //if (request.IsDraft.HasValue)
            //    targetQuery = targetQuery.Where(x => x.IsDraft == request.IsDraft.Value);
            //if (request.IsApproved.HasValue)
            //    targetQuery = targetQuery.Where(x => x.IsApproved == request.IsApproved.Value);

            targetQuery = targetQuery
                .Include(x => x.TargetDetail)
                .OrderByDescending(x => x.Id);

            var targetList = targetQuery.Select(x => new TargetListDto
            {
                Id = x.Id,
                TargetId = x.Id,
                Flow = x.TargetDetail.TargetSourceId == (int)TargetSourceEnum.Flow,
                Query = x.TargetDetail.TargetSourceId == (int)TargetSourceEnum.Query,
                Name = x.Name,
                IsActive = x.IsActive,
                TargetViewType = x.TargetDetail.TargetViewType.Name
            }).ToList();

            if (string.IsNullOrEmpty(request.SortBy))
            {
                targetList = targetList.OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                if (request.SortBy.EndsWith("Str"))
                    request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

                bool isDescending = request.SortDir?.ToLower() == "desc";
                if (isDescending)
                    targetList = targetList.OrderByDescending(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
                else
                    targetList = targetList.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
            }

            return targetList;
        }
    }
}
