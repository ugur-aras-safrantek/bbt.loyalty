using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.Draft;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IDraftService _draftService;
        private static int moduleTypeId = (int)ModuleTypeEnum.Target;

        public TargetService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            IAuthorizationService authorizationservice, IDraftService draftService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationservice;
            _draftService = draftService;
        }

        public async Task<BaseResponse<TargetDto>> AddAsync(TargetInsertRequest Target, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(Target);

            DateTime now = DateTime.UtcNow;
            var entity = _mapper.Map<TargetEntity>(Target);
            entity.Code = Helpers.CreateCampaignCode();
            entity.StatusId = (int)StatusEnum.Draft;
            entity.CreatedBy = userRole.UserId;
            entity.CreatedOn = now;
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

        public async Task<TargetDto2> GetTargetDto2(int id) 
        {
            var entity = await _unitOfWork.GetRepository<TargetEntity>().GetByIdAsync(id);
            TargetDto2 targetDto = new TargetDto2();
            targetDto.Name = entity.Name;
            targetDto.Id = entity.Id;
            targetDto.Code = entity.Code;
            targetDto.Title = entity.Title;
            targetDto.IsActive = entity.IsActive;
            targetDto.ApprovedBy = entity.ApprovedBy;
            targetDto.ApprovedDate = entity.ApprovedDate;
            targetDto.StatusId = entity.StatusId;
            return targetDto;
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
                .GetAll(x => !x.IsDeleted)
                .Select(x => _mapper.Map<TargetDto>(x))
                .ToList();
            return await BaseResponse<List<TargetDto>>.SuccessAsync(mappedTargets);
        }

        public async Task<BaseResponse<List<TargetDto>>> GetDraftListAsync()
        {
            var mappedTargets = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => !x.IsDeleted)
                .Select(x => _mapper.Map<TargetDto>(x))
                .ToList();
            return await BaseResponse<List<TargetDto>>.SuccessAsync(mappedTargets);
        }

        public async Task<BaseResponse<TargetDto>> UpdateAsync(TargetUpdateRequest Target, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(Target);
            var entity = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll().Where(x => x.Id == Target.Id).FirstOrDefault();
            if (entity != null)
            {
                if(entity.IsActive && !Target.IsActive) 
                {
                    var campaignIdList = await _unitOfWork.GetRepository<CampaignTargetListEntity>()
                        .GetAll(x => x.TargetId == Target.Id && !x.IsDeleted)
                        .Select(x => x.CampaignId)
                        .ToListAsync();
                    if(campaignIdList.Any()) 
                    {
                        foreach(int campaignId in campaignIdList) 
                        {
                            bool isActiveCampaign = await _draftService.IsActiveCampaign(campaignId);
                            if (isActiveCampaign)
                                throw new Exception("Pasif hale getirilmek istenen Hedef Tanımı, aktif bir kampanyaya bağlıdır.");
                        }
                    }
                }

                bool isCreateDraft = false;
                int processTypeId = await _draftService.GetTargetProcessType(Target.Id);
                if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
                {
                    isCreateDraft = true;
                    entity = new TargetEntity();
                    entity.TargetDetail = new TargetDetailEntity();
                    entity = await _draftService.CopyTargetInfo(Target.Id, entity, userRole.UserId, false, false, false, true, false);
                }

                entity.Title= Target.Title;
                entity.Name= Target.Name;
                entity.IsActive = Target.IsActive;

                if(isCreateDraft)
                    await _unitOfWork.GetRepository<TargetEntity>().AddAsync(entity);
                else
                    await _unitOfWork.GetRepository<TargetEntity>().UpdateAsync(entity);

                await _unitOfWork.SaveChangesAsync();
                return await GetTargetAsync(entity.Id);
            }
            return await BaseResponse<TargetDto>.FailAsync("Hedef bulunamadı.");
        }

        async Task CheckValidationAsync(TargetInsertRequest input)
        {
            //if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrEmpty(input.Name))
            //    throw new Exception("Hedef Adı girilmelidir.");

            //file eklenmeli
        }

        public async Task<BaseResponse<TargetListFilterResponse>> GetByFilterAsync(TargetListFilterRequest request, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            var response = new TargetListFilterResponse();
            try 
            {
                var sentToApprovalEntity = _unitOfWork.GetRepository<TargetEntity>()
                    .GetAll(x => !x.IsDeleted && x.StatusId == (int)StatusEnum.SentToApprove)
                    .FirstOrDefaultAsync();
                response.IsSentToApprovalRecord = sentToApprovalEntity != null;

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

        public async Task<BaseResponse<GetFileResponse>> GetExcelAsync(TargetListFilterRequest request, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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
            if (request.StatusId.HasValue)
                targetQuery = targetQuery.Where(x => x.StatusId == request.StatusId);
            if (!string.IsNullOrWhiteSpace(request.Name))
                targetQuery = targetQuery.Where(x => x.Name.Contains(request.Name));
            if (request.TargetSourceId.HasValue)
                targetQuery = targetQuery.Where(x => x.TargetDetail.TargetSourceId == request.TargetSourceId.Value);
            if (request.IsActive.HasValue)
                targetQuery = targetQuery.Where(x => x.IsActive == request.IsActive.Value);
            if (request.TargetViewTypeId.HasValue)
                targetQuery = targetQuery.Where(x => x.TargetDetail.TargetViewTypeId == request.TargetViewTypeId.Value);

            targetQuery = targetQuery.Include(x => x.TargetDetail).OrderByDescending(x => x.Id);

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
