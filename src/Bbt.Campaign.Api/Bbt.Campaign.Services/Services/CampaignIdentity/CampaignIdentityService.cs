using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignIdentity;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignIdentity;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.CampaignIdentity
{
    public class CampaignIdentityService : ICampaignIdentityService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParameterService _parameterService;
        private readonly IMapper _mapper;

        public CampaignIdentityService(IUnitOfWork unitOfWork, IParameterService parameterService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _parameterService = parameterService;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<CampaignIdentityDto>>> UpdateAsync(UpdateCampaignIdentityRequest request, string userId) 
        {
            List<string>  identityList = await CheckValidationsAsync(request);

            //foreach (var x in await _unitOfWork.GetRepository<CampaignIdentityEntity>()
            //    .GetAll(x => x.CampaignId == request.CampaignId && x.IsDeleted != true).ToListAsync()) 
            //{
            //    await _unitOfWork.GetRepository<CampaignIdentityEntity>().DeleteAsync(x);
            //}

            foreach(string identity in identityList) 
            {
                var campaignIdentityEntity = new CampaignIdentityEntity()
                { 
                    CampaignId = request.CampaignId,
                    IdentitySubTypeId = request.IdentitySubTypeId,
                    Identities = identity,
                    CreatedBy = userId,
                };

                await _unitOfWork.GetRepository<CampaignIdentityEntity>().AddAsync(campaignIdentityEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetListAsync(request.CampaignId);
        }

        public async Task<BaseResponse<List<CampaignIdentityDto>>> DeleteAsync(CampaignIdentityDeleteRequest request, string userId) 
        {
            List<CampaignIdentityDto> campaignIdentityList = new List<CampaignIdentityDto>();

            if (request.IdList == null || !request.IdList.Any())
                throw new Exception("silinecek kayıt bulunamadı");

            foreach(int id in request.IdList) 
            {
                var entity = await _unitOfWork.GetRepository<CampaignIdentityEntity>().GetByIdAsync(id);
                if (entity == null)
                    throw new Exception("Kayıt bulunamadı");

                entity.IsDeleted = true;
                entity.LastModifiedOn = DateTime.UtcNow;
                entity.LastModifiedBy = userId;

                await _unitOfWork.GetRepository<CampaignIdentityEntity>().UpdateAsync(entity);
            }
            
            await _unitOfWork.SaveChangesAsync();

            foreach(int id in request.IdList) 
            {
                var entity = await _unitOfWork.GetRepository<CampaignIdentityEntity>().GetByIdAsync(id);
                var campaignIdentityDto = _mapper.Map<CampaignIdentityDto>(entity);
                campaignIdentityList.Add(campaignIdentityDto);
            }

            return await BaseResponse<List<CampaignIdentityDto>>.SuccessAsync(campaignIdentityList);
        }

        public async Task<BaseResponse<CampaignIdentityDto>> GetCampaignIdentityAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignIdentityEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                CampaignIdentityDto campaignIdentityDto = _mapper.Map<CampaignIdentityDto>(entity);
                return await BaseResponse<CampaignIdentityDto>.SuccessAsync(campaignIdentityDto);

            }
            return null;
        }

        public async Task<BaseResponse<List<CampaignIdentityDto>>> GetListAsync(int campaignId)
        {
            List<CampaignIdentityDto> campaignIdentityList = _unitOfWork.GetRepository<CampaignIdentityEntity>().GetAll()
                .Where(x => x.CampaignId == campaignId && !x.IsDeleted)
                .Select(x => _mapper.Map<CampaignIdentityDto>(x)).ToList();
            return await BaseResponse<List<CampaignIdentityDto>>.SuccessAsync(campaignIdentityList);
        }

        public async Task<BaseResponse<CampaignIdentityUpdateFormDto>> GetUpdateFormAsync() 
        {
            CampaignIdentityUpdateFormDto response = new CampaignIdentityUpdateFormDto();
            await FillForm(response);
            return await BaseResponse<CampaignIdentityUpdateFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<CampaignIdentityListFilterResponse>> GetByFilterAsync(CampaignIdentityListFilterRequest request) 
        {
            CampaignIdentityListFilterResponse response = new CampaignIdentityListFilterResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CampaignIdentityListEntity> query = await GetQueryAsync(request);

            if (query.Count() == 0)
                return await BaseResponse<CampaignIdentityListFilterResponse>.SuccessAsync(response, "Kayıt bulunamadı");

            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = query.Count();
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var campaignIdentityList = ConvertCampaignIdentityList(query);
            response.CampaignIdentityList = campaignIdentityList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);

            return await BaseResponse<CampaignIdentityListFilterResponse>.SuccessAsync(response);
        }

        public async Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignIdentityListFilterRequest request) 
        {
            GetFileResponse response = new GetFileResponse();
            IQueryable<CampaignIdentityListEntity> query = await GetQueryAsync(request);
            if (query.Count() == 0)
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Kayıt bulunamadı");
            
            var campaignIdentityList = ConvertCampaignIdentityList(query);
            byte[] data = ListFileOperations.GetCampaignIdentityListExcel(campaignIdentityList);

            response = new GetFileResponse()
            {
                Document = new Public.Models.CampaignDocument.DocumentModel()
                {
                    Data = Convert.ToBase64String(data, 0, data.Length),
                    DocumentName = "Kampanya TCKN Listesi.xlsx",
                    DocumentType = DocumentTypePublicEnum.ExcelReport,
                    MimeType = ".xlsx"
                }
            };
            return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        }

        private async Task<List<string>> CheckValidationsAsync(UpdateCampaignIdentityRequest input)
        {
            List<string> identityList = new List<string>();

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == input.CampaignId && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            int identitySubTypeId = input.IdentitySubTypeId ?? 0;
            if (identitySubTypeId > 0)
            {
                var identitySubType = (await _parameterService.GetIdentitySubTypeListAsync())?.Data?.Any(x => x.Id == identitySubTypeId);
                if (!identitySubType.GetValueOrDefault(false))
                    throw new Exception("Alt kırılım bilgisi hatalı.");
            }

            if (input.IsSingleIdentity)
            {
                await CheckSingleIdentiy(input.Identity ?? "");
                identityList.Add(input.Identity ?? string.Empty);
            }
            else
            {
                if (string.IsNullOrEmpty(input.File))
                    throw new Exception("Dosya boş olamaz.");

                byte[] bytesList = System.Convert.FromBase64String(input.File);
                var memoryStream = new MemoryStream(bytesList);
                using (var excelWorkbook = new XLWorkbook(memoryStream))
                {
                    var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();
                    foreach (var dataRow in nonEmptyDataRows)
                    {
                        string identity = dataRow.Cell(1).Value == null ? string.Empty : dataRow.Cell(1).Value.ToString().Trim();

                        await CheckSingleIdentiy(identity);

                        if (identityList.Contains(identity))
                            throw new Exception("Dosya içerisinde " + identity + " nolu Vkn/Tckn 1 den fazla mevcut.");

                        identityList.Add(identity);
                    }
                }
            }

            if (!identityList.Any())
                throw new Exception("Vkn/Tckn bilgisi boş olamaz.");

            return identityList;
        }
        
        private async Task FillForm(CampaignIdentityUpdateFormDto response)
        {
            response.IdentitySubTypeList = (await _parameterService.GetIdentitySubTypeListAsync())?.Data;
            response.CampaignList = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.IsActive && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted && (x.EndDate.AddDays(1) > DateTime.UtcNow))
                .Select(x => _mapper.Map<ParameterDto>(x)).ToList();
        }
        
        private List<CampaignIdentityListDto> ConvertCampaignIdentityList(IQueryable<CampaignIdentityListEntity> query) 
        {
            var list = query.Select(x => new CampaignIdentityListDto
            {
                Id = x.Id,
                CampaignId = x.CampaignId,
                CampaignName = x.CampaignName,
                IdentitySubTypeId = x.IdentitySubTypeId,
                IdentitySubTypeName = x.IdentitySubTypeName,
                Identities = x.Identities,
            }).ToList();
            return list;
        }

        private async Task<IQueryable<CampaignIdentityListEntity>> GetQueryAsync(CampaignIdentityListFilterRequest request) 
        {
            var query = _unitOfWork.GetRepository<CampaignIdentityListEntity>().GetAll();

            if (request.CampaignId.HasValue)
                query = query.Where(x => x.CampaignId == request.CampaignId);
            if (request.IdentitySubTypeId.HasValue)
                query = query.Where(x => x.IdentitySubTypeId == request.IdentitySubTypeId);
            if (!string.IsNullOrEmpty(request.Identities) && !string.IsNullOrWhiteSpace(request.Identities))
                query = query.Where(x => x.Identities.Contains(request.Identities));

            if (string.IsNullOrEmpty(request.SortBy))
            {
                query = query.OrderByDescending(x => x.Id);
            }
            else 
            {
                if (request.SortBy.EndsWith("Str"))
                    request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

                bool isDescending = request.SortDir?.ToLower() == "desc";
                switch (request.SortBy) 
                {
                    case "CampaignName":
                        query = isDescending ? query.OrderByDescending(x => x.CampaignName) : query.OrderBy(x => x.CampaignName);
                        break;
                    case "IdentitySubTypeName":
                        query = isDescending ? query.OrderByDescending(x => x.IdentitySubTypeName) : query.OrderBy(x => x.IdentitySubTypeName);
                        break;
                    case "Identities":
                        query = isDescending ? query.OrderByDescending(x => x.Identities) : query.OrderBy(x => x.Identities);
                        break;
                    default:
                        break;
                }
            }


            return query;
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

            if (identity.Trim().Length == 11)
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
    }
}
