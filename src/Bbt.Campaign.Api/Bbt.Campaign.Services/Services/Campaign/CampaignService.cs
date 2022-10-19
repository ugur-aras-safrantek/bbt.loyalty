﻿using AutoMapper;
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
using Bbt.Campaign.Services.Services.Draft;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.Services.Services.Remote;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public class CampaignService : ICampaignService, IScopedService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IDraftService _draftService;
        private readonly IRemoteService _remoteService;


        public CampaignService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            IDraftService draftService, IRemoteService remoteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _draftService = draftService;
            _remoteService = remoteService;
        }

        public async Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            //await _authorizationService.CheckAuthorizationAsync(userId, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(campaign, 0);

            CampaignEntity entity = new CampaignEntity();

            entity.ContractId = campaign.ContractId;
            entity.ProgramTypeId = campaign.ProgramTypeId;
            entity.SectorId = campaign.SectorId;
            entity.ViewOptionId = campaign.ViewOptionId;
            entity.IsBundle = campaign.IsBundle;
            entity.IsActive = campaign.IsActive;
            entity.IsContract = campaign.IsContract;
            entity.DescriptionTr = campaign.DescriptionTr;
            entity.DescriptionEn = campaign.DescriptionEn;
            entity.StartDate = Helpers.ConvertUIDateTimeStringForBackEnd(campaign.StartDate);
            entity.EndDate = Helpers.ConvertUIDateTimeStringForBackEnd(campaign.EndDate);
            entity.Name = campaign.Name;
            entity.Order = campaign.Order;
            entity.TitleTr = campaign.TitleTr;
            entity.TitleEn = campaign.TitleEn;
            entity.MaxNumberOfUser = campaign.MaxNumberOfUser;
            entity.ParticipationTypeId = campaign.ParticipationTypeId;
            entity.StatusId = (int)StatusEnum.Draft;
            entity.Code = Helpers.CreateCampaignCode();
            entity.CreatedBy = userId;

            entity.CampaignDetail = new CampaignDetailEntity();
            entity.CampaignDetail.DetailEn = campaign.CampaignDetail.DetailEn;
            entity.CampaignDetail.DetailTr = campaign.CampaignDetail.DetailTr;
            entity.CampaignDetail.SummaryEn = campaign.CampaignDetail.SummaryEn;
            entity.CampaignDetail.SummaryTr = campaign.CampaignDetail.SummaryTr;
            entity.CampaignDetail.CampaignDetailImageUrl = campaign.CampaignDetail.CampaignDetailImageUrl;
            entity.CampaignDetail.CampaignListImageUrl = campaign.CampaignDetail.CampaignListImageUrl;
            entity.CampaignDetail.ContentTr = campaign.CampaignDetail.ContentTr;
            entity.CampaignDetail.ContentEn = campaign.CampaignDetail.ContentEn;
            entity.CampaignDetail.CreatedBy = userId; 
            entity.CampaignDetail.IsDeleted = false;

            entity = await SetDefaults(entity);
           
            entity = await _unitOfWork.GetRepository<CampaignEntity>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return await GetCampaignAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Update;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            await CheckValidationAsync(campaign, campaign.Id);

            var entity = _unitOfWork.GetRepository<CampaignEntity>().GetAllIncluding(x => x.CampaignDetail).Where(x => x.Id == campaign.Id).FirstOrDefault();

            if(entity == null)
                return await BaseResponse<CampaignDto>.FailAsync("Kampanya bulunamadı.");

            if (entity.IsActive && !campaign.IsActive && Helpers.ConvertUIDateTimeStringForBackEnd(campaign.EndDate) > DateTime.Now.AddDays(-1))
            {
                var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                    .Where(x => !x.IsDeleted && x.Code == entity.Code && x.StatusId == (int)StatusEnum.Approved)
                    .FirstOrDefault();
                if(approvedCampaign != null) 
                {
                    var topLimitList = await _unitOfWork.GetRepository<CampaignTopLimitEntity>()
                        .GetAllIncluding(x => x.TopLimit)
                        .Where(x => x.CampaignId == approvedCampaign.Id && !x.IsDeleted && x.TopLimit.IsActive)
                        .Select(x => x.CampaignId)
                        .ToListAsync();
                    if (topLimitList.Contains(approvedCampaign.Id))
                        throw new Exception(@"Pasif hale getirilmek istenen Kampanya, Çatı Limitleri içerisinde bir adet kampanya ile birlikte tanımlıdır.");
                }
            }

            int processTypeId = await _draftService.GetCampaignProcessType(campaign.Id);
            if (processTypeId == (int)ProcessTypesEnum.CreateDraft)
            {
                int id = await _draftService.CreateCampaignDraftAsync(campaign.Id, userId, (int)PageTypeEnum.Campaign);
                entity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAllIncluding(x => x.CampaignDetail)
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
            }
            else 
            {
                var campaignUpdatePageEntity = _unitOfWork.GetRepository<CampaignUpdatePageEntity>().GetAll().Where(x => x.CampaignId == entity.Id).FirstOrDefault();
                if (campaignUpdatePageEntity != null)
                {
                    campaignUpdatePageEntity.IsCampaignUpdated = true;
                    await _unitOfWork.GetRepository<CampaignUpdatePageEntity>().UpdateAsync(campaignUpdatePageEntity);
                }
            }

            entity.CampaignDetail.DetailEn = campaign.CampaignDetail.DetailEn;
            entity.CampaignDetail.DetailTr = campaign.CampaignDetail.DetailTr;
            entity.CampaignDetail.SummaryEn = campaign.CampaignDetail.SummaryEn;
            entity.CampaignDetail.SummaryTr = campaign.CampaignDetail.SummaryTr;
            entity.CampaignDetail.CampaignDetailImageUrl = campaign.CampaignDetail.CampaignDetailImageUrl;
            entity.CampaignDetail.CampaignListImageUrl = campaign.CampaignDetail.CampaignListImageUrl;
            entity.CampaignDetail.ContentTr = campaign.CampaignDetail.ContentTr;
            entity.CampaignDetail.ContentEn = campaign.CampaignDetail.ContentEn;
            //Bu alanlar önyüzden alındıgında requestten set edilecek.
            //Update işleminde db deki kayıt ezilmesin diye bu şekilde set edildi.
            entity.CampaignDetail.SummaryPopupTr = entity.CampaignDetail.SummaryPopupTr;
            entity.CampaignDetail.SummaryPopupEn = entity.CampaignDetail.SummaryPopupEn;


            entity.ContractId = campaign.ContractId;
            entity.ProgramTypeId = campaign.ProgramTypeId;
            entity.SectorId = campaign.SectorId;
            entity.ViewOptionId = campaign.ViewOptionId;
            entity.IsBundle = campaign.IsBundle;
            entity.IsActive = campaign.IsActive;
            entity.IsContract = campaign.IsContract;
            entity.DescriptionTr = campaign.DescriptionTr;
            entity.DescriptionEn = campaign.DescriptionEn;
            entity.StartDate = Helpers.ConvertUIDateTimeStringForBackEnd(campaign.StartDate);
            entity.EndDate = Helpers.ConvertUIDateTimeStringForBackEnd(campaign.EndDate);
            entity.Name = campaign.Name;
            entity.Order = campaign.Order;
            entity.TitleTr = campaign.TitleTr;
            entity.TitleEn = campaign.TitleEn;
            entity.MaxNumberOfUser = campaign.MaxNumberOfUser;
            entity.ParticipationTypeId = campaign.ParticipationTypeId;
            entity.LastModifiedOn = DateTime.Now;
            entity.LastModifiedBy = userId;

            entity = await SetDefaults(entity);

            await _unitOfWork.GetRepository<CampaignEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return await GetCampaignAsync(entity.Id);
        }

        public async Task<BaseResponse<CampaignDto>> CreateDraftAsync(int id, string userId) 
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.Insert;
            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);


            var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Kampanya bulunamadı.");
            if (entity.StatusId != (int)StatusEnum.Approved)
                throw new Exception("Kampanya uygun statüde değil.");
            //var draftEntity = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
            //   .Where(x => x.Code == entity.Code && x.StatusId == (int)StatusEnum.Draft && !x.IsDeleted)
            //   .FirstOrDefault();
            //if(draftEntity != null)
            //    throw new Exception("Kampanya için taslak bulunmaktadır.");

            int campaignId = await _draftService.CreateCampaignDraftAsync(id, userId, (int)PageTypeEnum.Campaign);
            return await GetCampaignAsync(campaignId);
        }

        public async Task<BaseResponse<CampaignDto>> CopyAsync(int id, string userid)
        {
            return await _draftService.CreateCampaignCopyAsync(id, userid);
        }

        public async Task<BaseResponse<CampaignDto>> DeleteAsync(int id, string userId)
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
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;
            
            //await _authorizationService.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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
            mappedCampaign.StartDate = Helpers.ConvertBackEndDateTimeToStringForUI(campaignEntity.StartDate);
            mappedCampaign.EndDate = Helpers.ConvertBackEndDateTimeToStringForUI(campaignEntity.EndDate);

            return  mappedCampaign;
        }

        public async Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync(string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignInsertFormDto response = new CampaignInsertFormDto();
            await FillFormAsync(response);
            return await BaseResponse<CampaignInsertFormDto>.SuccessAsync(response);
        }

        public async Task<BaseResponse<List<CampaignDto>>> GetListAsync(string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            var mappedCampaigns = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted)
                .Select(x => _mapper.Map<CampaignDto>(x))
                .ToList();
            return await BaseResponse<List<CampaignDto>>.SuccessAsync(mappedCampaigns);
        }

        public async Task<BaseResponse<List<ParameterDto>>> GetParameterListAsync()
        {
            var existCampaings = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted).Select(x => _mapper.Map<ParameterDto>(x)).ToList();
            return await BaseResponse<List<ParameterDto>>.SuccessAsync(existCampaings);
        }

        public async Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignUpdateFormDto response = new CampaignUpdateFormDto();
            await FillFormAsync(response);
            response.Campaign = (await GetCampaignAsync(id))?.Data;

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

            if (response.Campaign.IsContract && (response.Campaign.ContractId ?? 0) > 0) 
            {
                response.ContractFile = new GetFileResponse()
                {
                    Document = new Public.Models.CampaignDocument.DocumentModel()
                    {
                        Data = null,
                        DocumentName = response.Campaign.ContractId.ToString() + "-Sözleşme.html",
                        DocumentType = DocumentTypePublicEnum.Contract,
                        MimeType = MimeTypeExtensions.ToMimeType(".html")
                    }
                };
                
                //response.ContractFile = await GetContractFile(response.Campaign.ContractId ?? 0, contentRootPath);
            }
                
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
            { 
                entity.CampaignDetail.CampaignListImageUrl = await _parameterService.GetServiceConstantValue("CampaignListImageUrl");
            }
                
            if (string.IsNullOrWhiteSpace(entity.CampaignDetail.CampaignDetailImageUrl) ||
                string.IsNullOrEmpty(entity.CampaignDetail.CampaignDetailImageUrl)) 
            { 
                entity.CampaignDetail.CampaignDetailImageUrl = await _parameterService.GetServiceConstantValue("CampaignDetailImageUrl");
            }
                

            return entity;
        }

        async Task CheckValidationAsync(CampaignInsertRequest input, int campaignId)
        {
            //Kampanya Adı
            if (string.IsNullOrWhiteSpace(input.Name))
                throw new Exception("Kampanya Adı girilmelidir.");

            //kampanya adı mükerrer kontrolu
            bool isNameExists = false;
            var campaignList = await _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Name == input.Name && !x.IsDeleted && (x.StatusId == (int)StatusEnum.Approved || x.StatusId == (int)StatusEnum.SentToApprove))
                    .ToListAsync();
            if(campaignList.Any()) 
            { 
                if(campaignId == 0)
                    isNameExists = true;
                else 
                {
                    var entity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);
                    var campaign = campaignList.Where(x => x.Code != entity.Code).FirstOrDefault();
                    if (campaign != null)
                        isNameExists = true;
                }
            }
            if(isNameExists)
                throw new Exception("Aynı kampanya adı ile birden fazla kayıt oluşturulamaz.");


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
            DateTime startDate = Helpers.ConvertUIDateTimeStringForBackEnd(input.StartDate);
            DateTime endDate = Helpers.ConvertUIDateTimeStringForBackEnd(input.EndDate);
            DateTime previousDay  = DateTime.Now.AddDays(-1);

            if(campaignId == 0)//insert ise 
            {
                if (startDate < previousDay)
                    throw new Exception("Başlama Tarihi günün tarihinden küçük olamaz.");
            }
            if (endDate < previousDay)
                throw new Exception("Bitiş Tarihi günün tarihinden küçük olamaz.");
            if (startDate > endDate)
                throw new Exception("Başlama Tarihi, Bitiş Tarihinden büyük olamaz”");

            //sıralama: birleştirilebilir ve  değilse zorunludur
            if (!input.IsBundle && input.IsActive)
            {
                DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
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
                        && (x.StatusId == (int)StatusEnum.Approved || x.StatusId == (int)StatusEnum.SentToApprove))
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

        public async Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignListFilterResponse response = new CampaignListFilterResponse();

            var sentToApprovalEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.StatusId == (int)StatusEnum.SentToApprove)
                .FirstOrDefaultAsync();
            response.IsSentToApprovalRecord = sentToApprovalEntity != null;

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

        public async Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request, string userId)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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
                campaignQuery = campaignQuery.Where(x => x.StartDate.Date >= Helpers.ConvertUIDateTimeStringForBackEnd(request.StartDate));              
            if (!string.IsNullOrEmpty(request.EndDate) && !string.IsNullOrWhiteSpace(request.EndDate))
                campaignQuery = campaignQuery.Where(x => x.EndDate.Date <= Helpers.ConvertUIDateTimeStringForBackEnd(request.EndDate));
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
                StartDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.StartDate),
                EndDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.EndDate),
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
                var document = await _remoteService.GetDocument(id);
                if (document != null)
                {
                    if (document.Content != null)
                    {
                        getFileResponse = new GetFileResponse()
                        {
                            Document = new Public.Models.CampaignDocument.DocumentModel()
                            {
                                Data = document.Content,
                                DocumentName = id.ToString() + "-Sözleşme.html",
                                DocumentType = DocumentTypePublicEnum.Contract,
                                MimeType = "text/html"
                            }
                        };
                    }
                    else { throw new Exception("Doküman içeriği boş."); }
                }
            }

            return getFileResponse;

        }
        private async Task<List<int>> GetOrderListAsync(int maxCount) 
        {
            List<int> orderList = new List<int>();

            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
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
