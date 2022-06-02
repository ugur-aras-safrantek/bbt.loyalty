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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bbt.Campaign.Services.Services.CampaignTopLimit;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Core.Helper;
using System.Globalization;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public class CampaignService : ICampaignService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDraftService _draftService;
        private static int moduleTypeId = (int)ModuleTypeEnum.Campaign;
        CultureInfo provider = CultureInfo.InvariantCulture;
        private static string dateFormat = "d";

        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, IAuthorizationService authorizationService, IDraftService draftService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationService;
            _draftService = draftService;
        }

        public async Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);
            try
            {
            //    campaign.StartDate = Helpers.ConvertDotFormatDatetimeString(campaign.StartDate);


            //    campaign.EndDate = Helpers.ConvertDotFormatDatetimeString(campaign.EndDate);
                await CheckValidationAsync(campaign, 0);
                var entity = _mapper.Map<CampaignEntity>(campaign);
                entity = await SetDefaults(entity);
                entity.StatusId = (int)StatusEnum.Draft;
                entity.Code = Helpers.CreateCampaignCode();
                entity.CreatedBy = userid;
                entity.CampaignDetail.CreatedBy = userid;

                entity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            

            //entity.Code = entity.Id.ToString();
            //await _unitOfWork.SaveChangesAsync();

            return await GetCampaignAsync(entity.Id, userid);
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.ToString());
            }
            return null;
        }

        public async Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            campaign.StartDate = Helpers.ConvertDotFormatDatetimeString(campaign.StartDate);
            campaign.EndDate = Helpers.ConvertDotFormatDatetimeString(campaign.EndDate);
            await CheckValidationAsync(campaign, campaign.Id);

            var entity = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAllIncluding(x => x.CampaignDetail)
                .Where(x => x.Id == campaign.Id).FirstOrDefault();

            if(entity == null)
                return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");

            if (entity.IsActive && !campaign.IsActive && DateTime.Parse(campaign.EndDate) > DateTime.UtcNow.AddDays(-1))
            {
                var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                    .Where(x => !x.IsDeleted && x.Code == entity.Code && x.StatusId == (int)StatusEnum.Approved)
                    .FirstOrDefault();
                if(approvedCampaign != null) 
                {
                    var topLimitIdList = await _unitOfWork.GetRepository<CampaignTopLimitEntity>()
                        .GetAllIncluding(x => x.TopLimit)
                        .Where(x => x.CampaignId == approvedCampaign.Id && !x.IsDeleted && x.TopLimit.IsActive)
                        .Select(x => x.TopLimitId)
                        .ToListAsync();
                    if (!topLimitIdList.Contains(approvedCampaign.Id))
                        throw new Exception(@"Pasif hale getirilmek istenen Kampanya, Çatı Limitleri içerisinde bir adet kampanya ile birlikte tanımlıdır.");
                }
            }

            int processTypeId = await _draftService.GetProcessType(campaign.Id);
            if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
            {
                int id = await _draftService.CreateCampaignDraftAsync(campaign.Id, userid);
                entity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAllIncluding(x => x.CampaignDetail)
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
            }

            entity.CampaignDetail.DetailEn = campaign.CampaignDetail.DetailEn;
            entity.CampaignDetail.DetailTr = campaign.CampaignDetail.DetailTr;
            entity.CampaignDetail.SummaryEn = campaign.CampaignDetail.SummaryEn;
            entity.CampaignDetail.SummaryTr = campaign.CampaignDetail.SummaryTr;
            entity.CampaignDetail.CampaignDetailImageUrl = campaign.CampaignDetail.CampaignDetailImageUrl;
            entity.CampaignDetail.CampaignListImageUrl = campaign.CampaignDetail.CampaignListImageUrl;
            entity.CampaignDetail.ContentTr = campaign.CampaignDetail.ContentTr;
            entity.CampaignDetail.ContentEn = campaign.CampaignDetail.ContentEn;
            entity.CampaignDetail.LastModifiedBy = userid;

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
            entity.ParticipationTypeId = campaign.ParticipationTypeId;
            entity.LastModifiedBy = userid;

            entity = await SetDefaults(entity);

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignAsync(campaign.Id, userid);
        }

        public async Task<BaseResponse<CampaignDto>> CreateDraftAsync(int id, string userid) 
        {
            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Kampanya bulunamadı.");
            if (entity.StatusId != (int)StatusEnum.Approved)
                throw new Exception("Kampanya uygun statüde değil.");
            var draftEntity = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
               .Where(x => x.Code == entity.Code && x.StatusId == (int)StatusEnum.Draft && !x.IsDeleted)
               .FirstOrDefault();
            if(draftEntity != null)
                throw new Exception("Kampanya için taslak bulunmaktadır.");

            int campaignId = await _draftService.CreateCampaignDraftAsync(id, userid);
            return await GetCampaignAsync(campaignId, userid);
        }

        public async Task<BaseResponse<CampaignDto>> CopyAsync(int id, string userid)
        {
            return await _draftService.CreateCampaignCopyAsync(id, userid);
        }

        public async Task<BaseResponse<CampaignDto>> DeleteAsync(int id, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<CampaignEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCampaignAsync(entity.Id, userid);
            }
            return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");
        }

        public async Task<BaseResponse<CampaignDto>> GetCampaignAsync(int id, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;
            
            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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

            return  mappedCampaign;
        }

        public async Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync(string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignInsertFormDto response = new CampaignInsertFormDto();
            await FillFormAsync(response);
            return await BaseResponse<CampaignInsertFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<List<CampaignDto>>> GetListAsync(string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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

        public async Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignUpdateFormDto response = new CampaignUpdateFormDto();
            await FillFormAsync(response);
            response.Campaign = (await GetCampaignAsync(id, userid))?.Data;

            List<int> orderList = response.OrderList;
            if(response.Campaign != null) 
            {
                int order = response.Campaign.Order ?? 0;
                if(order > 0) 
                {
                    if (!orderList.Exists(p => p.Equals(order))) 
                    {
                        orderList.Add(order);
                        orderList.Sort();
                        response.OrderList = orderList;
                    }
                }
            }

            if(response.Campaign.IsContract && (response.Campaign.ContractId ?? 0) > 0)
                response.ContractFile = await GetContractFile(response.Campaign.ContractId ?? 0, contentRootPath);

            CampaignProperty campaignProperty = await _draftService.GetCampaignProperties(id);
            response.IsUpdatableCampaign = campaignProperty.IsUpdatableCampaign;

            return await BaseResponse<CampaignUpdateFormDto>.SuccessAsync(response);
        }
        
        private async Task FillFormAsync(CampaignInsertFormDto response)
        {
            response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
            response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
            response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
            response.ParticipationTypeList = (await _parameterService.GetParticipationTypeListAsync())?.Data;
            response.OrderList = await this.GetOrderListAsync(50);
            response.StatusList = (await _parameterService.GetStatusListAsync())?.Data;
        } 
        
        private async Task<CampaignEntity> SetDefaults(CampaignEntity entity)
        {
            if (entity.ProgramTypeId == (int)ProgramTypeEnum.Loyalty)
            {
                entity.ViewOptionId = null;
                entity.SectorId = null;
            }

            if (entity.ViewOptionId == (int)ViewOptionsEnum.InvisibleCampaign)
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

            //Katılım şekli
            if (input.ParticipationTypeId <= 0)
                throw new Exception("Katılım şekli seçilmelidir.");
            else
            {
                var participationType = (await _parameterService.GetParticipationTypeListAsync())?.Data?.Any(x => x.Id == input.ParticipationTypeId);
                if (!participationType.GetValueOrDefault(false))
                    throw new Exception("Katılım şekli hatalı.");
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
            DateTime startDate = DateTime.ParseExact(Helpers.ConvertDotFormatDatetimeString(input.StartDate), dateFormat, provider);
            DateTime endDate = DateTime.ParseExact(Helpers.ConvertDotFormatDatetimeString(input.EndDate), dateFormat, CultureInfo.InvariantCulture);
            DateTime previousDay  = DateTime.Now.AddDays(-1);

            if (startDate < previousDay)
                throw new Exception("Başlama Tarihi günün tarihinden küçük olamaz.");
            if (endDate < previousDay)
                throw new Exception("Bitiş Tarihi günün tarihinden küçük olamaz.");
            if (startDate > endDate)
                throw new Exception("Başlama Tarihi, Bitiş Tarihinden büyük olamaz”");

            //sıralama: birleştirilebilir ve  değilse zorunludur
            if (!input.IsBundle && input.IsActive)
            {
                DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
                int order = input.Order ?? 0;

                if (order <= 0)
                    throw new Exception("Sıralama girilmelidir.");
                string campaignCode = string.Empty;
                if(campaignId > 0) 
                {
                    var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
                    campaignCode = campaignEntity.Code;
                }

                var orderCampaignEntity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => !x.IsDeleted && x.Order != null && x.Order == order 
                        && x.EndDate >= today && x.Id != campaignId 
                        && x.Code != campaignCode
                        && x.StatusId == (int)StatusEnum.Approved)
                    .FirstOrDefault();
                if (orderCampaignEntity != null) 
                {
                    throw new Exception("Aynı Sıralama Girilemez.");
                }
            }

            if(input.MaxNumberOfUser.HasValue) 
            {
                if ((input.MaxNumberOfUser ?? 0) == 0)
                    throw new Exception("Max. Kullanıcı 0 olamaz");
            }
        }

        public async Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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

        public async Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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
            if (!string.IsNullOrEmpty(request.Code) && !string.IsNullOrWhiteSpace(request.Code)) 
                campaignQuery = campaignQuery.Where(x => x.Code.Contains(request.Code));
            if (!string.IsNullOrEmpty(request.Name) && !string.IsNullOrWhiteSpace(request.Name))
                campaignQuery = campaignQuery.Where(x => x.Name.Contains(request.Name));
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
            switch (request.StatusId) 
            {
                case (int)StatusEnum.History:
                    campaignQuery = campaignQuery.Where(x => x.StatusId == (int)StatusEnum.History || x.StatusId == (int)StatusEnum.Approved);
                    break;
                default:
                    campaignQuery = campaignQuery.Where(x => x.StatusId == request.StatusId);
                    break;
            }

            var campaignList = campaignQuery.Select(x => new CampaignListDto
            {
                Id = x.Id,
                Code = x.Code,
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
                if (isDescending)
                    campaignList = campaignList.OrderByDescending(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
                else
                    campaignList = campaignList.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null)).ToList();
            }
            return campaignList;
        }

        public async Task<BaseResponse<GetFileResponse>> GetContractFileAsync(int id, string contentRootPath) 
        {
            //GetFileResponse getFileResponse = await GetContractFile(id, contentRootPath);

            return await BaseResponse<GetFileResponse>.SuccessAsync(await GetContractFile(id, contentRootPath));
        }

        public async Task<GetFileResponse> GetContractFile(int id, string contentRootPath) 
        {
            var getFileResponse = new GetFileResponse();

            if (StaticValues.IsDevelopment)
            {
                byte[] data = null;
                var filePath = Path.Combine(contentRootPath, "Download", $"Kampanya.html");
                if (File.Exists(filePath))
                    data = File.ReadAllBytes(filePath);
                else
                    throw new Exception("Dosya bulunamadı");

                getFileResponse = new GetFileResponse()
                {
                    Document = new Public.Models.CampaignDocument.DocumentModel()
                    {
                        Data = Convert.ToBase64String(data, 0, data.Length),
                        DocumentName = id.ToString() + "-Sözleşme.html",
                        DocumentType = DocumentTypePublicEnum.Contract,
                        MimeType = MimeTypeExtensions.ToMimeType(".html")
                    }
                };
            }
            else
            {
                string accessToken = await _parameterService.GetAccessToken();


                using (var client = new HttpClient())
                {
                    //string baseAddress = await _parameterService.GetServiceConstantValue("BaseAddress");
                    //string apiAddress = await _parameterService.GetServiceConstantValue("Document");
                    //apiAddress = apiAddress.Replace("{key}", id.ToString());
                    //client.BaseAddress = new Uri(baseAddress); 
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    //var restResponse = await client.GetAsync(apiAddress);
                    //if (restResponse.IsSuccessStatusCode) 
                    //{ 
                    //    var responseContent = restResponse.Content.ReadAsStringAsync().Result;
                    //}
                    //else 
                    //{ 
                    
                    //}
                    
                }
            }

            return getFileResponse;

        }
        
        //public async Task<bool> IsInvisibleCampaign(int campaignId) 
        //{
        //    bool isInvisibleCampaign = false;
        //    var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
        //    if (campaignEntity != null)
        //    {
        //        int viewOptionId = campaignEntity.ViewOptionId ?? 0;
        //        isInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
        //    }
        //    return isInvisibleCampaign;
        //}

        public async Task<bool> IsActiveCampaign(int campaignId) 
        {
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
            if (campaignEntity == null)
                throw new Exception("Kampanya bulunamadı");
            return  campaignEntity.IsActive && 
                    campaignEntity.EndDate > DateTime.UtcNow.AddDays(-1) && 
                    !campaignEntity.IsDeleted;
        }

        private async Task<List<int>> GetOrderListAsync(int maxCount) 
        {
            List<int> orderList = new List<int>();

            DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
            List<int?> usedList = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive && x.EndDate >= today 
                            && !x.IsBundle && (x.Order ?? 0) > 0 && x.StatusId == (int)StatusEnum.Approved)
                .Select(x => x.Order)
                .ToList();

            int count = 0;
            for(int i = 1; i < int.MaxValue; i++) 
            {
                if(!usedList.Exists(p => p.Equals(i)))
                {
                    orderList.Add(i);
                    count++;
                }
                
                if(count == maxCount)
                    break;
            }
            return orderList;
        }
    }
}
