using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public class CampaignService : ICampaignService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
        }

        public async Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign)
        {
            await CheckValidationAsync(campaign, 0);

            var entity = _mapper.Map<CampaignEntity>(campaign);

            entity = await SetDefaults(entity);

            entity.Code = string.Empty;
            entity.IsDraft = true;
            entity.IsApproved = false;

            entity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<CampaignEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCampaignAsync(entity.Id);
            }
            return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");
        }

        public async Task<BaseResponse<CampaignDto>> GetCampaignAsync(int id)
        {
            //var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
            //    .GetAll(x => x.Id == id && !x.IsDeleted)
            //    .Include(x => x.CampaignDetail)
            //    .FirstOrDefaultAsync();
            //if (campaignEntity != null)
            //{
            //    var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            //    mappedCampaign.StartDate = campaignEntity.StartDate.ToShortDateString().Replace('.', '-');
            //    mappedCampaign.EndDate = campaignEntity.EndDate.ToShortDateString().Replace('.', '-');
            //    mappedCampaign.Code = mappedCampaign.Id.ToString();
            //    return await BaseResponse<CampaignDto>.SuccessAsync(mappedCampaign);
            //}

            var campaignDto = await GetCampaignDtoAsync(id);

            if(campaignDto != null) 
            {
                return await BaseResponse<CampaignDto>.SuccessAsync(campaignDto);
            }

            return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");
        }

        public async Task<CampaignDto> GetCampaignDtoAsync(int id) 
        {          
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == id && !x.IsDeleted)
                    .Include(x => x.CampaignDetail)
                    .FirstOrDefaultAsync();
            if(campaignEntity == null) 
            {
                return null;
            }

            var mappedCampaign = _mapper.Map<CampaignDto>(campaignEntity);
            mappedCampaign.StartDate = campaignEntity.StartDate.ToShortDateString().Replace('.', '-');
            mappedCampaign.EndDate = campaignEntity.EndDate.ToShortDateString().Replace('.', '-');
            mappedCampaign.Code = mappedCampaign.Id.ToString();

            return  mappedCampaign;
        }

        public async Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync()
        {
            CampaignInsertFormDto response = new CampaignInsertFormDto();
            await FillFormAsync(response);
            return await BaseResponse<CampaignInsertFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<List<CampaignDto>>> GetListAsync()
        {
            var mappedCampaigns = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted)
                .Select(x => _mapper.Map<CampaignDto>(x))
                .ToList();
            return await BaseResponse<List<CampaignDto>>.SuccessAsync(mappedCampaigns);
        }

        public async Task<BaseResponse<List<ParameterDto>>> GetParameterListAsync()
        {
            var existCampaings = _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => !x.IsDeleted).Select(x => _mapper.Map<ParameterDto>(x)).ToList();
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(existCampaings);
        }

        public async Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath)
        {
            CampaignUpdateFormDto response = new CampaignUpdateFormDto();
            await FillFormAsync(response);
            response.Campaign = (await GetCampaignAsync(id))?.Data;

            if(response.Campaign.IsContract && (response.Campaign.ContractId ?? 0) > 0)
                response.ContractFile = await GetContractFile(response.Campaign.ContractId ?? 0, contentRootPath);

            return await BaseResponse<CampaignUpdateFormDto>.SuccessAsync(response);
        }

        private async Task FillFormAsync(CampaignInsertFormDto response)
        {
            response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
            response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
            response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
        }

        public async Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign)
        {
            await CheckValidationAsync(campaign, campaign.Id);
            
            var entity = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAllIncluding(x => x.CampaignDetail)
                .Where(x => x.Id == campaign.Id).FirstOrDefault();
            if (entity != null)
            {
                if (entity.CampaignDetail == null)
                {
                    entity.CampaignDetail = await _unitOfWork.GetRepository<CampaignDetailEntity>().AddAsync(new CampaignDetailEntity()
                    {
                        CampaignId = campaign.Id,
                        DetailEn = campaign.CampaignDetail.DetailEn,
                        DetailTr = campaign.CampaignDetail.DetailTr,
                        SummaryEn = campaign.CampaignDetail.SummaryEn,
                        SummaryTr = campaign.CampaignDetail.SummaryTr,
                        CampaignDetailImageUrl = campaign.CampaignDetail.CampaignDetailImageUrl,
                        CampaignListImageUrl = campaign.CampaignDetail.CampaignListImageUrl,
                        ContentTr = campaign.CampaignDetail.ContentTr,
                        ContentEn = campaign.CampaignDetail.ContentEn
                    });
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    entity.CampaignDetail.DetailEn = campaign.CampaignDetail.DetailEn;
                    entity.CampaignDetail.DetailTr = campaign.CampaignDetail.DetailTr;
                    entity.CampaignDetail.SummaryEn = campaign.CampaignDetail.SummaryEn;
                    entity.CampaignDetail.SummaryTr = campaign.CampaignDetail.SummaryTr;
                    entity.CampaignDetail.CampaignDetailImageUrl = campaign.CampaignDetail.CampaignDetailImageUrl;
                    entity.CampaignDetail.CampaignListImageUrl = campaign.CampaignDetail.CampaignListImageUrl;
                    entity.CampaignDetail.ContentTr = campaign.CampaignDetail.ContentTr;
                    entity.CampaignDetail.ContentEn = campaign.CampaignDetail.ContentEn;
                }

                entity.ContractId = campaign.ContractId;
                entity.ProgramTypeId = campaign.ProgramTypeId;
                entity.SectorId = campaign.SectorId;
                entity.ViewOptionId = campaign.ViewOptionId;
                entity.IsBundle = campaign.IsBundle;
                entity.IsActive = campaign.IsActive;
                entity.IsContract = campaign.IsContract;
                entity.DescriptionTr = campaign.DescriptionTr;
                entity.DescriptionEn = campaign.DescriptionEn;
                entity.EndDate = DateTime.Parse(campaign.EndDate);
                entity.StartDate = DateTime.Parse(campaign.StartDate);
                entity.Name = campaign.Name;
                entity.Order = campaign.Order;
                entity.TitleTr = campaign.TitleTr;
                entity.TitleEn = campaign.TitleEn;
                entity.MaxNumberOfUser = campaign.MaxNumberOfUser;
                entity.Code = campaign.Id.ToString();
                entity.IsDraft = true;
                entity.IsApproved = false;

                entity = await SetDefaults(entity);

                await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCampaignAsync(campaign.Id);
            }

            return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");
        }

        private async Task<CampaignEntity> SetDefaults(CampaignEntity entity)
        {
            if (entity.ProgramTypeId == (int)ProgramTypeEnum.Loyalty) 
            {
                entity.ViewOptionId = null;
                entity.SectorId = null;
            }

            if(entity.ViewOptionId == (int)ViewOptionsEnum.InvisibleCampaign) 
            {
                entity.TitleTr = null; //Başlık(Türkçe)
                entity.TitleEn = null;
            
                 //İçerik
                entity.CampaignDetail.ContentTr = null;
                entity.CampaignDetail.ContentEn = null;
                
                //detay
                entity.CampaignDetail.DetailTr = null;
                entity.CampaignDetail.DetailEn = null;

                //liste ve detay görseli
                entity.CampaignDetail.CampaignListImageUrl = null;
                entity.CampaignDetail.CampaignDetailImageUrl = null;
            }

            //ContentTr boş ise, ContentEn boştur
            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.ContentTr) || string.IsNullOrEmpty(entity.CampaignDetail.ContentTr)) 
            {
                entity.CampaignDetail.ContentTr = null;
                entity.CampaignDetail.ContentEn = null;
            }

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.DetailTr) || string.IsNullOrEmpty(entity.CampaignDetail.DetailTr)) 
            {
                entity.CampaignDetail.DetailEn = null;
                entity.CampaignDetail.DetailTr = null;
            }

            if (entity.IsBundle || !entity.IsActive)
                entity.Order = null;

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.CampaignListImageUrl) || 
                string.IsNullOrEmpty(entity.CampaignDetail.CampaignListImageUrl))
                entity.CampaignDetail.CampaignListImageUrl = StaticValues.CampaignListImageUrlDefault;

            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.CampaignDetailImageUrl) ||
                string.IsNullOrEmpty(entity.CampaignDetail.CampaignDetailImageUrl))
                entity.CampaignDetail.CampaignDetailImageUrl = StaticValues.CampaignDetailImageUrlDefault;

            return entity;
        }

        async Task CheckValidationAsync(CampaignInsertRequest input, int campaignId)
        {
            //Kampanya Adı
            if (string.IsNullOrWhiteSpace(input.Name))
                throw new Exception("Kampanya Adı girilmelidir.");

            //Sözleşme
            if (input.IsContract) 
            {
                if((input.ContractId ?? 0) == 0)
                    throw new Exception("Sözleşme ID Giriniz.");
            }

            //Program Tipi
            if (input.ProgramTypeId <= 0)
                throw new Exception("Program Tipi seçilmelidir.");
            else
            {
                var programType = (await _parameterService.GetProgramTypeListAsync())?.Data?.Any(x => x.Id == input.ProgramTypeId);
                if (!programType.GetValueOrDefault(false))
                    throw new Exception("Program Tipi hatalı.");
            }

            if (input.ProgramTypeId != (int)ProgramTypeEnum.Loyalty)//Program Tipi sadakat değil ise
            {
                int viewOptionId = input.ViewOptionId ?? 0;
                if (viewOptionId <= 0)
                    throw new Exception("Görüntüleme seçilmelidir.");

                var viewOption = (await _parameterService.GetViewOptionListAsync())?.Data?.Any(x => x.Id == viewOptionId);
                if (!viewOption.GetValueOrDefault(false))
                    throw new Exception("Görüntüleme hatalı.");

                int sectorId = input.SectorId ?? 0;
                if(sectorId > 0) 
                {
                    var sector = (await _parameterService.GetSectorListAsync())?.Data?.Any(x => x.Id == sectorId);
                    if (!sector.GetValueOrDefault(false))
                        throw new Exception("Sektör hatalı.");
                }             
            }

            if (input.ViewOptionId != (int)ViewOptionsEnum.InvisibleCampaign) 
            {
                if (string.IsNullOrWhiteSpace(input.TitleTr) || string.IsNullOrEmpty(input.TitleTr))
                    throw new Exception("Başlık (Türkçe) girilmelidir.");
                if (string.IsNullOrWhiteSpace(input.TitleEn) || string.IsNullOrEmpty(input.TitleEn))
                    throw new Exception("Başlık (İngilizce) girilmelidir.");

                //ContentTr girildiyse ContentEn zorunludur
                if (!string.IsNullOrWhiteSpace(input.CampaignDetail.ContentTr) && !string.IsNullOrEmpty(input.CampaignDetail.ContentTr)) 
                {
                    if (string.IsNullOrWhiteSpace(input.CampaignDetail.ContentEn) || string.IsNullOrEmpty(input.CampaignDetail.ContentEn))
                        throw new Exception("İçerik (İngilizce) girilmelidir.");
                }

                //DetailTr girildiyse DetailTr zorunludur
                if (!string.IsNullOrWhiteSpace(input.CampaignDetail.DetailTr) && !string.IsNullOrEmpty(input.CampaignDetail.DetailTr)) 
                {
                    if (string.IsNullOrWhiteSpace(input.CampaignDetail.DetailEn) || string.IsNullOrEmpty(input.CampaignDetail.DetailEn))
                        throw new Exception("Detay (İngilizce) girilmelidir.");
                }
            }

            //liste ve detay url kontrolu
            if (!string.IsNullOrEmpty(input.CampaignDetail.CampaignListImageUrl) && 
                !string.IsNullOrWhiteSpace(input.CampaignDetail.CampaignListImageUrl)) 
            { 
                if(!Uri.IsWellFormedUriString(input.CampaignDetail.CampaignListImageUrl, UriKind.Absolute)) 
                {
                    throw new Exception("Kampanya Liste Görseli url formatı hatalı.");
                }
            }

            if (!string.IsNullOrEmpty(input.CampaignDetail.CampaignDetailImageUrl) &&
                !string.IsNullOrWhiteSpace(input.CampaignDetail.CampaignDetailImageUrl))
            {
                if (!Uri.IsWellFormedUriString(input.CampaignDetail.CampaignDetailImageUrl, UriKind.Absolute))
                {
                    throw new Exception("Kampanya Detay Görseli url formatı hatalı.");
                }
            }

            //startdate ve enddate
            DateTime startDate = DateTime.Parse(input.StartDate.ToString());
            DateTime endDate = DateTime.Parse(input.EndDate.ToString());
            DateTime now = DateTime.Parse(DateTime.Now.ToShortDateString());

            if (startDate < now)
                throw new Exception("Başlama Tarihi günün tarihinden küçük olamaz.");
            if (endDate < now)
                throw new Exception("Bitiş Tarihi günün tarihinden küçük olamaz.");
            if (startDate > endDate)
                throw new Exception("Başlama Tarihi, Bitiş Tarihinden büyük olamaz”");

            //sıralama: birleştirilebilir ve  değilse zorunludur
            if (!input.IsBundle || input.IsActive)
            {
                int order = input.Order ?? 0;

                if (order <= 0)
                    throw new Exception("Sıralama girilmelidir.");

                var orderCampaignEntity = _unitOfWork.GetRepository<CampaignEntity>().
                    GetAll(x => !x.IsDeleted && x.Order != null && x.Order == order)
                    .FirstOrDefault();
                if (orderCampaignEntity != null) 
                {
                    if (campaignId == 0 || campaignId != orderCampaignEntity.Id)
                        throw new Exception("Aynı Sıralama Girilemez.");
                }
            }

            if(input.MaxNumberOfUser.HasValue) 
            {
                if ((input.MaxNumberOfUser ?? 0) == 0)
                    throw new Exception("Max. Kullanıcı 0 olamaz");
            }
        }

        public async Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request)
        {
            CampaignListFilterResponse response = new CampaignListFilterResponse();
            List<CampaignListDto> campaignList = await this.GetFilteredCampaignList(request);
            var totalItems = campaignList.Count();
            if (totalItems == 0)
                return await BaseResponse<CampaignListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            campaignList = campaignList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            response.ResponseList = campaignList;
            response.Paging = Core.Helper.Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CampaignListFilterResponse>.SuccessAsync(response);
        }

        public async Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request)
        {
            GetFileResponse response = new GetFileResponse();
            List<CampaignListDto> campaignList = await this.GetFilteredCampaignList(request);
            var totalItems = campaignList.Count();
            if (totalItems == 0)
            {
                return await BaseResponse<GetFileResponse>.SuccessAsync(response);
            }

            byte[] data = ListFileOperations.GetCampaignListExcel(campaignList);

            response = new GetFileResponse()
            {
                Document = new Public.Models.CampaignDocument.DocumentModel()
                {
                    Data = Convert.ToBase64String(data, 0, data.Length),
                    DocumentName = "Kampanya Listesi.xlsx",
                    DocumentType = DocumentTypePublicEnum.ExcelReport,
                    MimeType = MimeTypeExtensions.ToMimeType(".xlsx")
                }
            };

            return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        }

        private async Task<List<CampaignListDto>> GetFilteredCampaignList(CampaignListFilterRequest request)
        {
            Core.Helper.Helpers.ListByFilterCheckValidation(request);

            var campaignQuery = _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => !x.IsDeleted);

            if (request.IsBundle.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsBundle == request.IsBundle.Value);
            if (!string.IsNullOrEmpty(request.CampaignCode) && !string.IsNullOrWhiteSpace(request.CampaignCode)) 
            {
                int campaignId = -1;
                try 
                { 
                    campaignId = int.Parse(request.CampaignCode);
                }
                catch (Exception ex) 
                { 
                
                }
                campaignQuery = campaignQuery.Where(x => x.Id == campaignId);
            } 
            if (!string.IsNullOrEmpty(request.CampaignName) && !string.IsNullOrWhiteSpace(request.CampaignName))
                campaignQuery = campaignQuery.Where(x => x.Name.Contains(request.CampaignName));
            if (request.ContractId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ContractId == request.ContractId.Value);
            if (request.IsActive.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsActive == request.IsActive.Value);
            if (request.ProgramTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value);
            if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrWhiteSpace(request.StartDate)) 
                campaignQuery = campaignQuery.Where(x => x.StartDate.Date >= Convert.ToDateTime(request.StartDate));              
            if (!string.IsNullOrEmpty(request.EndDate) && !string.IsNullOrWhiteSpace(request.EndDate))
                campaignQuery = campaignQuery.Where(x => x.EndDate.Date <= Convert.ToDateTime(request.EndDate));
            if (request.IsDraft.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsDraft == request.IsDraft.Value);
            if (request.IsApproved.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsApproved == request.IsApproved.Value);

            var campaignList = campaignQuery.Select(x => new CampaignListDto
            {
                Id = x.Id,
                Code = x.Id.ToString(),
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartDateStr = x.StartDate.ToShortDateString(),
                EndDateStr = x.EndDate.ToShortDateString(),
                ContractId = x.ContractId,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                ProgramType = x.ProgramType.Name,
                Name = x.Name
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
                if (request.SortBy.Equals("CampaignCode")) 
                {
                    if (isDescending)
                        campaignList = campaignList.OrderByDescending(x => x.Id).ToList();
                    else
                        campaignList = campaignList.OrderBy(x => x.Id).ToList();
                }
                else 
                {
                    if (isDescending)
                        campaignList = campaignList.OrderByDescending(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
                    else
                        campaignList = campaignList.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
                } 
            }
            return campaignList;
        }

        private async Task<List<CampaignListDto>> GetSortedCampaignListDto(List<CampaignListDto> campaignList, string sortBy, string sortDir) 
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                campaignList = campaignList.OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                bool isDescending = sortDir?.ToLower() == "desc";

                switch (sortBy)
                {
                    case "CampaignName":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.Name).ToList() : campaignList.OrderBy(x => x.Name).ToList();
                        break;
                    case "CampaignCode":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.Id).ToList() : campaignList.OrderBy(x => x.Id).ToList();
                        break;
                    case "ContractId":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.ContractId).ToList() : campaignList.OrderBy(x => x.ContractId).ToList();
                        break;
                    case "IsActive":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.IsActive).ToList() : campaignList.OrderBy(x => x.IsActive).ToList();
                        break;
                    case "IsBundle":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.IsBundle).ToList() : campaignList.OrderBy(x => x.IsBundle).ToList();
                        break;
                    case "StartDate":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.StartDate).ToList() : campaignList.OrderBy(x => x.StartDate).ToList();
                        break;
                    case "EndDate":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.EndDate).ToList() : campaignList.OrderBy(x => x.EndDate).ToList();
                        break;
                    case "ProgramType":
                        campaignList = isDescending ? campaignList.OrderByDescending(x => x.ProgramType).ToList() : campaignList.OrderBy(x => x.ProgramType).ToList();
                        break;
                    default:
                        break;
                }
            }
            return campaignList;
        }

        public async Task<BaseResponse<GetFileResponse>> GetContractFileAsync(int id, string contentRootPath) 
        {
            GetFileResponse getFileResponse = await GetContractFile(id, contentRootPath);

            return await BaseResponse<GetFileResponse>.SuccessAsync(getFileResponse);
        }

        private async Task<GetFileResponse> GetContractFile(int id, string contentRootPath) 
        {
            var getFileResponse = new GetFileResponse();

            if (StaticValues.IsDevelopment)
            {
                byte[] data = null;
                var filePath = Path.Combine(contentRootPath, "Download", $"Contract.pdf");
                if (File.Exists(filePath))
                    data = File.ReadAllBytes(filePath);
                else
                    throw new Exception("Dosya bulunamadı");

                getFileResponse = new GetFileResponse()
                {
                    Document = new Public.Models.CampaignDocument.DocumentModel()
                    {
                        Data = Convert.ToBase64String(data, 0, data.Length),
                        DocumentName = id.ToString() + "-Sözleşme.pdf",
                        DocumentType = DocumentTypePublicEnum.Contract,
                        MimeType = MimeTypeExtensions.ToMimeType(".pdf")
                    }
                };
            }
            else
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await httpClient.GetAsync(StaticValues.ContractServiceUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content != null)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var values = apiResponse.Split('\u002C');
                        }
                    }
                    else
                    {
                        throw new Exception("Servisten sözleşme bilgisi çekilemedi.");
                    }
                }
            }

            return getFileResponse;

        }
        public async Task<bool> IsInvisibleCampaign(int campaignId) 
        {
            bool isInvisibleCampaign = false;
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                isInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }
            return isInvisibleCampaign;
        }
    }
}
