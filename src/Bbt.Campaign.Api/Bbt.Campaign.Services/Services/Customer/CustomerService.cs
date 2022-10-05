using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Customer;
using Bbt.Campaign.Services.Services.Campaign;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Bbt.Campaign.Services.Services.Remote;
using System.Text;
using Bbt.Campaign.Public.Models.MessagingTemplate;
using Newtonsoft.Json;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using System.Globalization;

namespace Bbt.Campaign.Services.Services.Customer
{
    public class CustomerService : ICustomerService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignService _campaignService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly ICampaignAchievementService _campaignAchievementService;
        private readonly IRemoteService _remoteService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService,
            ICampaignService campaignService, ICampaignTargetService campaignTargetService,
            ICampaignAchievementService campaignAchievementService, IRemoteService remoteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignService = campaignService;
            _campaignTargetService = campaignTargetService;
            _campaignAchievementService = campaignAchievementService;
            _remoteService = remoteService;
        }

        public async Task<BaseResponse<CustomerJoinSuccessFormDto>> SetJoin(SetJoinRequest request)
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            if (request.IsJoin)
            {
                bool ismaxNumberOfUserReach = await IsMaxNumberOfUserReach(request.CampaignId);
                if (ismaxNumberOfUserReach)
                    throw new Exception("Bu kampanya için maximum kullanıcı sayısına ulaşılmıstır.");
            }

            CustomerJoinSuccessFormDto response = new CustomerJoinSuccessFormDto();

            bool isFavorite = false;
            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted)
               .OrderByDescending(x => x.Id)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                if (request.IsJoin && entity.IsJoin)
                    throw new Exception("Müşteri bu kampayaya daha önceki bir tarihte katılmış.");
                if (!request.IsJoin && !entity.IsJoin)
                    throw new Exception("Müşteri bu kampayaya henüz katılmamış.");

                isFavorite = entity.IsFavorite;
            }
            else
            {
                if (!request.IsJoin)
                    throw new Exception("Müşteri bu kampayaya henüz katılmamış.");
            }

            foreach (var deleteEntity in _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted)
               .ToList())
            {
                deleteEntity.LastModifiedOn = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().DeleteAsync(deleteEntity);
            }


            if (isFavorite || request.IsJoin)
            {
                var newEntity = new CustomerCampaignEntity();
                newEntity.CustomerCode = request.CustomerCode;
                newEntity.CampaignId = request.CampaignId;
                newEntity.IsFavorite = isFavorite;
                newEntity.IsJoin = request.IsJoin;
                newEntity.StartDate = request.IsJoin ? Helpers.ConvertDateTimeToShortDate(DateTime.Now) : null;
                newEntity.CreatedBy = request.CustomerCode;

                newEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(newEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            //response

            if (request.IsJoin)
            {
                var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == request.CampaignId && x.CustomerCode == request.CustomerCode && !x.IsDeleted)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();
                if (customerJoin == null)
                    throw new Exception("Müşteri kampanyaya katılmamış.");

                if (!customerJoin.IsJoin)
                    throw new Exception("Müşteri kampanyaya katılmamış.");

                var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                    .GetAll(x => x.Id == request.CampaignId && !x.IsDeleted);
                campaignQuery = campaignQuery.Take(1);

                //DYS servisine döküman gönderilecek.
                //#region DYS gönderimi
                //List<int> docList = new List<int>();
                //var campaign = campaignQuery.FirstOrDefault();
                //if (campaign != null && campaign.IsContract)
                //{
                //    docList.Add(campaign.ContractId.Value);
                //}
                //var infotext = Convert.ToInt32(_parameterService.GetServiceConstantValue("InformationText"));
                //var gdpr = Convert.ToInt32(_parameterService.GetServiceConstantValue("GDPR"));
                //docList.Add(infotext);
                //docList.Add(gdpr);
                //await _remoteService.SendDmsDocuments(request.CustomerCode, docList);
                //#endregion

                var campaignList = campaignQuery.Select(x => new CampaignMinDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    TitleEn = x.TitleEn,
                    TitleTr = x.TitleTr,
                    CampaignListImageUrl = x.CampaignListImageUrl,
                    CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                    EndDate = x.EndDate,
                    ContentEn = x.ContentEn,
                    ContentTr = x.ContentTr,
                }).ToList();

                if (!campaignList.Any())
                    throw new Exception("Kampanya bulunamadı.");

                response.Campaign = campaignList[0];
            }
            else
            {
                var deletedEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == request.CampaignId && x.CustomerCode == request.CustomerCode && x.IsDeleted)
                        .Include(x => x.Campaign)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();
                if (deletedEntity != null)
                {
                    CampaignMinDto campaignMinDto = new CampaignMinDto()
                    {
                        Id = deletedEntity.Campaign.Id,
                        Name = deletedEntity.Campaign.Name,
                        TitleTr = deletedEntity.Campaign.TitleTr,
                        TitleEn = deletedEntity.Campaign.TitleEn,
                        EndDate = deletedEntity.Campaign.EndDate,
                    };

                    response.Campaign = campaignMinDto;
                    // Kampanyadan ayrıldıgında o dönem verilen kazanımları silinir. 
                    var term = Utilities.GetTerm();
                    _remoteService.LeaveProgramAchievementDelete(request.CustomerCode, request.CampaignId, term);
                }
            }


            if (request.IsJoin)
            {

                #region sms gönderimi
                //Hoşgeldin bildirimi
                var targetAmount = await GetCustomerCampaignTargetAmountAsync(request.CampaignId, request.CustomerCode);
                if (targetAmount != null && !String.IsNullOrEmpty(targetAmount.Data.TargetAmount))
                {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("targetamount", targetAmount.Data.TargetAmount);
                    TemplateInfo template = new TemplateInfo()
                    {
                        templateName = "",
                        templateParameter = JsonConvert.SerializeObject(param)
                    };
                    _remoteService.SendSmsMessageTemplate(request.CustomerCode, request.CampaignId, 1, template);
                    //_remoteService.SendNotificationMessageTemplate(request.CustomerCode, request.CampaignId, 1, template);
                }
                #endregion

                #region koşulsuz dönem ve destek Harcama kontrolüne göre kazanım servisi çağırma

                var term = Utilities.GetTerm();
                var customerIdendity = _unitOfWork.GetRepository<CampaignIdentityEntity>()
                    .GetAll(x => x.Identities == request.CustomerCode && x.CampaignId == request.CampaignId && x.IsDeleted == false).ToList();

                if (customerIdendity.Count > 0)
                {
                    var result = await _remoteService.CustomerAchievementsAdd(request.CustomerCode, request.CampaignId, term);
                }
                #endregion
            }


            return await BaseResponse<CustomerJoinSuccessFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerCampaignDto>> SetFavorite(SetFavoriteRequest request)
        {
            await CheckValidationAsync(request.CustomerCode, request.CampaignId);

            bool isJoin = false;
            DateTime? startDate = null;
            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted)
               .OrderByDescending(x => x.Id)
               .FirstOrDefaultAsync();
            if (entity != null)
            {
                isJoin = entity.IsJoin;
                startDate = entity.StartDate;
            }

            foreach (var deleteEntity in _unitOfWork.GetRepository<CustomerCampaignEntity>()
               .GetAll(x => x.CustomerCode == request.CustomerCode && x.CampaignId == request.CampaignId && !x.IsDeleted).ToList())
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().DeleteAsync(deleteEntity);

            if (request.IsFavorite || isJoin)
            {
                var newEntity = new CustomerCampaignEntity();
                newEntity.CustomerCode = request.CustomerCode;
                newEntity.CampaignId = request.CampaignId;
                newEntity.IsFavorite = request.IsFavorite;
                newEntity.IsJoin = isJoin;
                newEntity.StartDate = startDate;
                newEntity.CreatedBy = request.CustomerCode;

                await _unitOfWork.GetRepository<CustomerCampaignEntity>().AddAsync(newEntity);
            }

            await _unitOfWork.SaveChangesAsync();

            var mappedCustomerCampaign = _mapper.Map<CustomerCampaignDto>(entity);

            return await BaseResponse<CustomerCampaignDto>.SuccessAsync(mappedCustomerCampaign);
        }
        private async Task<bool> IsMaxNumberOfUserReach(int campaignId)
        {
            bool isMaxNumberOfUserReach = false;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>().GetByIdAsync(campaignId);

            if (campaignEntity != null)
            {
                int maxNumberOfUser = campaignEntity.MaxNumberOfUser ?? 0;

                if (maxNumberOfUser > 0)
                {
                    var customerCount = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                       .GetAll(x => x.CampaignId == campaignId && x.IsJoin && !x.IsDeleted)
                       .CountAsync();
                    if (customerCount >= maxNumberOfUser)
                        isMaxNumberOfUserReach = true;
                }
            }
            return isMaxNumberOfUserReach;
        }
        private async Task CheckValidationAsync(string customerCode, int campaignId)
        {
            if (string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            if (campaignId <= 0)
                throw new Exception("Kampanya giriniz.");

            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignEntity = _unitOfWork.GetRepository<CampaignEntity>()
                    .GetAll(x => x.Id == campaignId && !x.IsDeleted && x.IsActive && x.EndDate >= today && x.StatusId == (int)StatusEnum.Approved)
                    .FirstOrDefault();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }
        }
        public async Task<BaseResponse<CustomerCampaignDto>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.GetRepository<CustomerCampaignEntity>().DeleteAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return await GetCustomerCampaignAsync(entity.Id);
            }
            return await BaseResponse<CustomerCampaignDto>.FailAsync("Kayıt bulunamadı.");
        }
        public async Task<BaseResponse<CustomerCampaignDto>> GetCustomerCampaignAsync(int id)
        {
            var customerCampaignEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>().GetByIdAsync(id);
            if (customerCampaignEntity != null)
            {
                CustomerCampaignDto customerCampaignDto = _mapper.Map<CustomerCampaignDto>(customerCampaignEntity);
                return await BaseResponse<CustomerCampaignDto>.SuccessAsync(customerCampaignDto);
            }
            return null;
        }
        public async Task<BaseResponse<CustomerCampaignListFilterResponse>> GetByFilterAsync(CustomerCampaignListFilterRequest request)
        {
            if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join || request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite)
            {
                if (string.IsNullOrEmpty(request.CustomerCode))
                    throw new Exception("Müşteri kodu giriniz.");
            }

            CustomerCampaignListFilterResponse response = new CustomerCampaignListFilterResponse();
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);

            IQueryable<CampaignDetailListEntity> campaignQuery = await GetCampaignQueryAsync(request);
            if (campaignQuery.Count() == 0)
                return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            //var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            //var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            //var totalItems = campaignQuery.Count();
            //campaignQuery = campaignQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            DateTime previousDay = DateTime.Now.AddDays(-1);
            var customerCampaignCountList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => x.IsJoin && !x.IsDeleted)
                .Include(x => x.Campaign)
                .Where(x => x.Campaign.IsActive && !x.Campaign.IsDeleted && ((x.Campaign.MaxNumberOfUser ?? 0) > 0) && x.Campaign.EndDate > previousDay)
                .GroupBy(x => x.CampaignId)
                .Select(group => Tuple.Create(group.Key, group.Count()))
                .ToListAsync();

            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
                MaxNumberOfUser = x.MaxNumberOfUser,
            }).ToList();

            var customerCampaignList = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => !x.IsDeleted && x.CustomerCode == (request.CustomerCode ?? string.Empty))
                .ToListAsync();

            List<CustomerCampaignMinListDto> returnList = new List<CustomerCampaignMinListDto>();
            foreach (var campaign in campaignList)
            {
                CustomerCampaignMinListDto customerCampaignListDto = new CustomerCampaignMinListDto();

                customerCampaignListDto.CampaignId = campaign.Id;
                customerCampaignListDto.Campaign = campaign;
                customerCampaignListDto.CustomerCode = request.CustomerCode;
                customerCampaignListDto.IsJoin = false;
                customerCampaignListDto.IsFavorite = false;

                var customerCampaign = customerCampaignList.Where(x => x.CampaignId == campaign.Id).FirstOrDefault();
                if (customerCampaign != null)
                {
                    customerCampaignListDto.Id = customerCampaign.Id;
                    customerCampaignListDto.IsJoin = customerCampaign.IsJoin;
                    customerCampaignListDto.IsFavorite = customerCampaign.IsFavorite;
                }

                if (!customerCampaignListDto.IsJoin && !customerCampaignListDto.IsFavorite)
                {
                    int maxNumberOfUser = campaign.MaxNumberOfUser ?? 0;
                    if (maxNumberOfUser > 0)
                    {
                        var customerCampaignCountEntity = customerCampaignCountList
                            .Where(x => x.Item1 == customerCampaignListDto.CampaignId).FirstOrDefault();
                        if (customerCampaignCountEntity != null)
                        {
                            int campaignUserCount = customerCampaignCountEntity.Item2;
                            if (campaignUserCount >= maxNumberOfUser)
                                continue;
                        }
                    }
                }

                if (campaign.EndDate > today)
                {
                    TimeSpan ts = campaign.EndDate - today;
                    customerCampaignListDto.DueDay = ts.Days + 1;
                }

                if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                {
                    if (campaign.EndDate >= today)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Join)
                {
                    if (customerCampaignListDto.IsJoin)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Favorite)
                {
                    if (customerCampaignListDto.IsFavorite)
                        returnList.Add(customerCampaignListDto);
                }
                else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                {
                    if (DateTime.Now > campaign.EndDate)
                        returnList.Add(customerCampaignListDto);
                }
            }

            response.CustomerCampaignList = returnList;
            //response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);

            return await BaseResponse<CustomerCampaignListFilterResponse>.SuccessAsync(response);
        }
        private async Task<IQueryable<CampaignDetailListEntity>> GetCampaignQueryAsync(CustomerCampaignListFilterRequest request)
        {
            DateTime today = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => !x.IsDeleted && x.IsActive
                        && (x.ViewOptionId != (int)ViewOptionsEnum.InvisibleCampaign || x.ViewOptionId == null)
                        && x.StatusId == (int)StatusEnum.Approved);

            if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.Campaign)
                campaignQuery = campaignQuery.Where(t => t.EndDate >= today);
            else if (request.PageTypeId == (int)CustomerCampaignListTypeEnum.OverDue)
                campaignQuery = campaignQuery.Where(t => t.EndDate < today);

            //sort

            if (string.IsNullOrEmpty(request.SortBy))
            {
                campaignQuery = campaignQuery.OrderByDescending(x => x.Id);
                return campaignQuery;
            }

            if (request.SortBy.EndsWith("Str"))
                request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

            bool isDescending = request.SortDir?.ToLower() == "desc";

            if (request.SortBy.Equals("Id") || request.SortBy.Equals("Code"))
            {
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Id) : campaignQuery = campaignQuery.OrderBy(x => x.Id);
            }
            else if (request.SortBy.Equals("Order"))
            {
                campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Order) : campaignQuery = campaignQuery.OrderBy(x => x.Order);
            }

            return campaignQuery;
        }
        public async Task<BaseResponse<CustomerViewFormMinDto>> GetCustomerViewFormAsync(int campaignId, string contentRootPath)
        {
            CustomerViewFormMinDto response = new CustomerViewFormMinDto();

            //campaign
            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            response.IsInvisibleCampaign = false;

            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }

            response.IsJoin = false;

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
            {
                response.IsContract = false;
                response.ContractFile = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
            }

            //target

            //var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDto(campaignId, true);

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer2(campaignId, string.Empty, "tr", true);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            response.CampaignAchievementList = campaignAchievementList;

            return await BaseResponse<CustomerViewFormMinDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerJoinFormDto>> GetCustomerJoinFormAsync(int campaignId, string customerCode, string contentRootPath)
        {
            if (string.IsNullOrEmpty(customerCode))
                throw new Exception("Müşteri kodu giriniz.");

            CustomerJoinFormDto response = new CustomerJoinFormDto() { ContractFiles = new List<Public.Models.File.GetFileResponse>() };

            //campaign
            response.CampaignId = campaignId;

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                if (campaignEntity == null) { throw new Exception("Kampanya bulunamadı."); }
            }

            int viewOptionId = campaignEntity.ViewOptionId ?? 0;
            response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;

            response.IsOverDue = DateTime.Now.AddDays(-1) > campaignEntity.EndDate;

            response.IsJoin = false;
            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .FirstOrDefaultAsync();
            if (customerJoin != null)
            {
                response.IsJoin = customerJoin.IsJoin;
            }

            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);

            //var campaignDto = campaignDtoAll

            response.Campaign = campaignDto;

            response.IsContract = false;
            if (campaignEntity.IsContract && (campaignEntity.ContractId ?? 0) > 0)
            {
                response.IsContract = false;

                var informationTextId = await _parameterService.GetServiceConstantValue("InformationText");
                var informationContract = await _campaignService.GetContractFile(Convert.ToInt32(informationTextId), contentRootPath);
                informationContract.ButtonTextTr = "Okudum";
                informationContract.ButtonTextEn = "Okudum";
                informationContract.UnderlineTextTr = $"{campaignDto.TitleTr} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Aydınlatma Metni'ni";
                informationContract.UnderlineTextEn = $"{campaignDto.TitleEn} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Aydınlatma Metni'ni";
                informationContract.DocumentTextTr = $"{campaignDto.TitleTr} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Aydınlatma Metni'ni okudum.";
                informationContract.DocumentTextEn = $"{campaignDto.TitleEn} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Aydınlatma Metni'ni okudum.";
                response.ContractFiles.Add(informationContract);

                var gdprTextId = await _parameterService.GetServiceConstantValue("GDPR");
                var gdprContract = await _campaignService.GetContractFile(Convert.ToInt32(gdprTextId), contentRootPath);
                gdprContract.ButtonTextTr = "Okudum, onaylıyorum";
                gdprContract.ButtonTextEn = "Okudum, onaylıyorum";
                gdprContract.UnderlineTextTr = $"{campaignDto.TitleTr} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Açık Rıza Beyanı'nı";
                gdprContract.UnderlineTextEn = $"{campaignDto.TitleEn} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Açık Rıza Beyanı'nı";
                gdprContract.DocumentTextTr = $"{campaignDto.TitleTr} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Açık Rıza Beyanı'nı okudum, onaylıyorum.";
                gdprContract.DocumentTextEn = $"{campaignDto.TitleEn} Programı Kapsamında Kişisel Verilerimin İşlenmesine İlişkin Açık Rıza Beyanı'nı okudum, onaylıyorum.";
                response.ContractFiles.Add(gdprContract);

                var campaignContract = await _campaignService.GetContractFile(campaignEntity.ContractId ?? 0, contentRootPath);
                if (campaignContract != null)
                {
                    campaignContract.ButtonTextTr = "Okudum, onaylıyorum";
                    campaignContract.ButtonTextEn = "Okudum, onaylıyorum";
                    campaignContract.UnderlineTextTr = $"{campaignDto.TitleTr} Program Sözleşmesi'ni";
                    campaignContract.UnderlineTextEn = $"{campaignDto.TitleEn} Program Sözleşmesi'ni";
                    campaignContract.DocumentTextTr = $"{campaignDto.TitleTr} Program Sözleşmesi'ni okudum ve onaylıyorum.";
                    campaignContract.DocumentTextEn = $"{campaignDto.TitleEn} Program Sözleşmesi'ni okudum ve onaylıyorum.";
                }
                response.ContractFiles.Add(campaignContract);
            }

            //target

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(campaignId, 0, 0);

            response.CampaignTarget = campaignTargetDto;

            //achievement
            //var campaignAchievementList = await _campaignAchievementService.GetCampaignAchievementListDto(campaignId);

            //response.CampaignAchievementList = campaignAchievementList;

            response.IsMaxNumberOfUserReach = await IsMaxNumberOfUserReach(campaignId);

            return await BaseResponse<CustomerJoinFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerAchievementFormDto>> GetCustomerAchievementFormAsync(int campaignId, string customerCode, string? language)
        {
            CustomerAchievementFormDto response = new CustomerAchievementFormDto();
           
            if (language == null)
                language = "tr";
            response.IsOnAccount =await _remoteService.GetAccounts(customerCode);
            if (response.IsOnAccount)
            {
                response.OnAccountTitle = language.ToLower() == "tr" ? "ON Hesap Ek Faiz Getirisi" : "ON Account Additional Interest Income";
                response.OnAccountDescription = language.ToLower() == "tr" ? "ON Hesap faizine ek %2 faiz getirisi avantajından faydalanabilirsin." : "You can take advantage of 2% interest income in addition to ON Account interest.";
            }
            else
            {
                response.OnAccountTitle = language.ToLower() == "tr" ? "Ek Faiz Getirisi için ON Hesap Aç!" : "Open ON Account for Additional Interest Income!";
                response.OnAccountDescription = language.ToLower() == "tr" ? "ON Hesap faizine ek %2 faiz getirisi avantajlarından faydalanmak için hemen ON Hesap aç." 
                    : "Open an ON Account now to take advantage of additional 2% interest income in addition to ON Account interest.";
            }
          
           

            //campaign
            response.CampaignId = campaignId;
            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (campaignEntity == null)
            {
                throw new Exception("Kampanya bulunamadı.");
            }

            //customerCampaign
            var customerCampaignEntity = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && x.IsJoin && !x.IsDeleted)
                .FirstOrDefaultAsync();
            if (customerCampaignEntity == null)
            {
                throw new Exception("Kampanya katılım bilgisi bulunamadı.");
            }
            var campaignJoinDate = customerCampaignEntity.StartDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            response.CampaignJoinMessage = language.ToLower() == "tr" ? $"{campaignJoinDate} tarihinden beri {campaignEntity.TitleTr}lısın"
                : $"You are {campaignEntity.TitleEn} since {campaignJoinDate}";
            response.IsInvisibleCampaign = false;
            if (campaignEntity != null)
            {
                int viewOptionId = campaignEntity.ViewOptionId ?? 0;
                response.IsInvisibleCampaign = viewOptionId == (int)ViewOptionsEnum.InvisibleCampaign;
            }
            var campaignDto = await _campaignService.GetCampaignDtoAsync(campaignId);
            response.Campaign = campaignDto;

            decimal? totalAchievement = 0;
            //decimal? previousMonthAchievement = 0;
            //decimal usedAmount = 0;
            //int usedNumberOfTransaction = 0;


            response.IsAchieved = false;
            string serviceUrl = string.Empty;

            if (StaticValues.IsDevelopment)
            {
                //totalAchievement = 190;
                //previousMonthAchievement = 120;
                //response.TotalAchievementStr = Helpers.ConvertNullablePriceString(totalAchievement);
                //response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);
                //response.TotalAchievementCurrencyCode = "TRY";
                //response.PreviousMonthAchievementCurrencyCode = "TRY";
            }
            else
            {
                var goalResultByCustomerIdAndMonthCount = await _remoteService.GetGoalResultByCustomerIdAndMonthCountData(customerCode, campaignId);
                if (goalResultByCustomerIdAndMonthCount != null)
                {
                    if (goalResultByCustomerIdAndMonthCount.Total != null)
                    {
                        response.TotalAchievementStr = Helpers.ConvertNullablePriceString(goalResultByCustomerIdAndMonthCount.Total.Amount);
                        response.TotalAchievementCurrencyCode = goalResultByCustomerIdAndMonthCount.Total.Currency == null ? null :
                                goalResultByCustomerIdAndMonthCount.Total.Currency == "TRY" ? "₺" :
                                goalResultByCustomerIdAndMonthCount.Total.Currency;
                    }
                    if (goalResultByCustomerIdAndMonthCount.Months != null && goalResultByCustomerIdAndMonthCount.Months.Any())
                    {
                        int month = DateTime.Now.Month;
                        int year = DateTime.Now.Year;

                        //this month
                        decimal? currentMonthAchievement = 0;
                        string currentMonthAchievementCurrencyCode = "TL";
                        var currentMonthAchievent = goalResultByCustomerIdAndMonthCount.Months.Where(x => x.Month == month).FirstOrDefault();
                        if (currentMonthAchievent != null)
                        {
                            currentMonthAchievement = currentMonthAchievent.Amount;
                            currentMonthAchievementCurrencyCode = currentMonthAchievent.Currency == null ? null :
                                currentMonthAchievent.Currency == "TRY" ? "TL" :
                                currentMonthAchievent.Currency;
                        }
                        response.CurrentMonthAchievementStr = Helpers.ConvertNullablePriceString(currentMonthAchievement);
                        response.CurrentMonthAchievementCurrencyCode = currentMonthAchievementCurrencyCode;

                        //previous month
                        decimal? previousMonthAchievement = 0;
                        string previousMonthAchievementCurrencyCode = "TL";
                        var previousMonthAchievent = goalResultByCustomerIdAndMonthCount.Months.Where(x => x.Month != month).FirstOrDefault();
                        if (previousMonthAchievent != null)
                        {
                            previousMonthAchievement = previousMonthAchievent.Amount;
                            previousMonthAchievementCurrencyCode = previousMonthAchievent.Currency == null ? null :
                                previousMonthAchievent.Currency == "TRY" ? "₺" :
                                previousMonthAchievent.Currency;
                        }
                        response.PreviousMonthAchievementStr = Helpers.ConvertNullablePriceString(previousMonthAchievement);
                        response.PreviousMonthAchievementCurrencyCode = previousMonthAchievementCurrencyCode;
                    }
                }
            }

            response.CampaignTarget = await _campaignTargetService.GetCampaignTargetDtoCustomer2(campaignId, customerCode, language, false);

            response.IsAchieved = response.CampaignTarget.IsAchieved;
            response.LastMonthIsAchieved = response.CampaignTarget.LastMonthIsAchieved;

            decimal usedAmount = 0;
            string usedAmountCurrencyCode = "TL";
            if (response.CampaignTarget.TotalUsed != null)
            {
                usedAmount = response.CampaignTarget.TotalUsed.Amount == null ? 0 : (decimal)response.CampaignTarget.TotalUsed.Amount;
                usedAmountCurrencyCode = response.CampaignTarget.TotalUsed.Currency == null ? "TL" :
                                    response.CampaignTarget.TotalUsed.Currency == "TRY" ? "TL" :
                                    response.CampaignTarget.TotalUsed.Currency;
            }
            response.UsedAmountStr = Helpers.ConvertNullablePriceString(usedAmount);
            response.UsedAmountCurrencyCode = usedAmountCurrencyCode;

            //targetResultDefinition
            string targetResultDefinition = string.Empty;
            var campaignTarget = response.CampaignTarget.ProgressBarlist.FirstOrDefault();
            if (campaignTarget != null)
            {
                string targetAmountStr = campaignTarget.TargetAmountStr ?? "";
                string remainAmountStr = campaignTarget.RemainAmountStr ?? "";
                string targetCurrencyCode = campaignTarget.TargetAmountCurrencyCode == null ? null :
                                    (campaignTarget.TargetAmountCurrencyCode == "TRY" || campaignTarget.TargetAmountCurrencyCode == "TL") ? "₺" :
                                    campaignTarget.TargetAmountCurrencyCode;
                string campaignName = campaignEntity.Name;
                string monthName = string.Empty;
                int month = DateTime.Now.Month + 1;
                if (month == 13)
                    month = 1;

                monthName = Helpers.GetEnumDescription<MonthsEnum>(month);

                if (response.IsAchieved)
                {
                    targetResultDefinition = string.Format(@"{0} {1} ve üzeri harcama yaparak hedefinizi tutturduğunuz için {2} ayında {3} avantajlarından faydalanabilirsiniz."
                                                , targetAmountStr
                                                , targetCurrencyCode
                                                , monthName
                                                , campaignName);
                }
                else
                {
                    targetResultDefinition = string.Format(@"Sadece {0} {1} harcama yaparak {2} ayında {3} avantajlarından faydalanmaya başlayabilirsiniz."
                                        , remainAmountStr
                                        , targetCurrencyCode
                                        , monthName
                                        , campaignName);
                }
            }

            response.TargetResultDefinition = targetResultDefinition;
            DateTime currentDate = DateTime.Now;
            response.CurrentMounthTitle = language.ToLower() == "tr" ? currentDate.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("tr")) : currentDate.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("en"));

            //campaignLeftDefinition

            //achievement
            var campaignAchievementList = await _campaignAchievementService.GetCustomerAchievementsAsync(campaignId, customerCode, language);
            foreach (var campaignAchievement in campaignAchievementList)
                campaignAchievement.IsAchieved = response.IsAchieved;
            response.CampaignAchievementList = campaignAchievementList;
            var previousMonth = currentDate.AddMonths(-1);
            var previousMonthName = language.ToLower() == "tr" ? previousMonth.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("tr"))
                    : previousMonth.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("en"));

            if (response.CampaignAchievementList.Count > 0)
            {

                response.CurrentMounthAchievementMessage = language.ToLower() == "tr" ? $"Tebrikler, {previousMonthName} ayı harcama hedefini tutturduğunuz için ON Plus avantajlarından faydalanabilirsin."
                    : $"Congratulations! You can enjoy ON Plus advantages for achieving your {previousMonthName} spending target.";
            }
            else
            {
                response.CurrentMounthAchievementMessage = language.ToLower() == "tr" ? $"Üzgünüz, {previousMonthName} ayında yaptığın harcamalar ON Plus avantajlarından faydalanman için yeterli değil."
                    : $"Sorry, your spendings in {previousMonthName} are not enough for you to benefit from ON Plus advantages.";
            }

            string campaignLeftDefinition = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(campaignEntity.Name + " programından ayrılma talebin bulunuyor. ");
            sb.Append("İşlemini onaylaman doğrultusunda ");
            for (int t = 0; t < campaignAchievementList.Count; t++)
            {
                if (campaignAchievementList.Count == 1)
                {
                    sb.Append(campaignAchievementList[t].Description + " ");
                }
                else
                {
                    if (t == campaignAchievementList.Count - 1)
                        sb.Append(campaignAchievementList[t].Description + " ");
                    else if (t == campaignAchievementList.Count - 2)
                        sb.Append(campaignAchievementList[t].Description + " ve ");
                    else
                        sb.Append(campaignAchievementList[t].Description + ", ");
                }
            }
            sb.Append("kazanamayacaksın. ");
            sb.Append(campaignEntity.Name + " programından ayrılma talebini onaylıyor musun?");
            campaignLeftDefinition = sb.ToString();
            response.CampaignLeftDefinition = campaignLeftDefinition;

            return await BaseResponse<CustomerAchievementFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerJoinSuccessFormDto>> GetCustomerJoinSuccessFormAsync(int campaignId, string customerCode)
        {
            CustomerJoinSuccessFormDto response = new CustomerJoinSuccessFormDto();

            //var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
            //            .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
            //            .OrderByDescending(x => x.Id)
            //            .FirstOrDefaultAsync();

            var campaignEntity = await _unitOfWork.GetRepository<CampaignEntity>()
                        .GetAll(x => x.Id == campaignId && !x.IsDeleted)
                        .Include(x => x.CampaignDetail)
                        .FirstOrDefaultAsync();

            if (campaignEntity == null)
                throw new Exception("kampanya bulunamadı.");

            var campaignMinDto = new CampaignMinDto()
            {
                Id = campaignEntity.Id,
                Name = campaignEntity.Name,
                TitleEn = campaignEntity.TitleEn,
                TitleTr = campaignEntity.TitleTr,
                CampaignListImageUrl = campaignEntity.CampaignDetail.CampaignListImageUrl,
                CampaignDetailImageUrl = campaignEntity.CampaignDetail.CampaignDetailImageUrl,
                EndDate = campaignEntity.EndDate,
                ContentEn = campaignEntity.CampaignDetail.ContentEn,
                ContentTr = campaignEntity.CampaignDetail.ContentTr,
            };

            response.Campaign = campaignMinDto;

            return await BaseResponse<CustomerJoinSuccessFormDto>.SuccessAsync(response);
        }
        public async Task<BaseResponse<CustomerJoinSuccessFormDto>> GetCustomerCampaignLeaveFormAsync(int campaignId, string customerCode)
        {
            CustomerJoinSuccessFormDto response = new CustomerJoinSuccessFormDto();

            var customerJoin = await _unitOfWork.GetRepository<CustomerCampaignEntity>()
                        .GetAll(x => x.CampaignId == campaignId && x.CustomerCode == customerCode && !x.IsDeleted)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();
            if (customerJoin == null)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            if (!customerJoin.IsJoin)
                throw new Exception("Müşteri kampanyaya katılmamış.");

            var campaignQuery = _unitOfWork.GetRepository<CampaignDetailListEntity>()
                .GetAll(x => x.Id == campaignId && !x.IsDeleted);
            campaignQuery = campaignQuery.Take(1);

            var campaignList = campaignQuery.Select(x => new CampaignMinDto
            {
                Id = x.Id,
                Name = x.Name,
                TitleEn = x.TitleEn,
                TitleTr = x.TitleTr,
                CampaignListImageUrl = x.CampaignListImageUrl,
                CampaignDetailImageUrl = x.CampaignDetailImageUrl,
                EndDate = x.EndDate,
                ContentEn = x.ContentEn,
                ContentTr = x.ContentTr,
            }).ToList();

            if (!campaignList.Any())
                throw new Exception("Kampanya bulunamadı.");

            response.Campaign = campaignList[0];

            return await BaseResponse<CustomerJoinSuccessFormDto>.SuccessAsync(response);

        }
        public async Task<BaseResponse<CustomerCampaignTargetResultDto>> GetCustomerCampaignTargetAmountAsync(int campaignId, string customerCode)
        {
            CustomerCampaignTargetResultDto response = new CustomerCampaignTargetResultDto();
            var onExtraDefinition = _unitOfWork.GetRepository<OnExtraDefinitionEntity>().GetAll(x => x.OnExtraCampaignId == campaignId).FirstOrDefault();
            var campaignEntity = await _unitOfWork.GetRepository<CampaignTargetEntity>()
                        .GetAll(x => x.CampaignId == campaignId && !x.IsDeleted)
                        .Include(x => x.Target.TargetDetail)
                        .FirstOrDefaultAsync();
            var targetAmount = campaignEntity.Target.TargetDetail.TotalAmount != null ? campaignEntity.Target.TargetDetail.TotalAmount : campaignEntity.Target.TargetDetail.NumberOfTransaction;
            response.TargetAmount = targetAmount.ToString();
            bool checkFirstMounthTarget = false;
            ///Kampanyaya katıldıgı ilk aya özel hedef kontrolü
            if (onExtraDefinition != null && onExtraDefinition?.CampaignJoinFirstMounthTarget > 0)
            {
                DateTime dateNow = DateTime.Now;
                var joinDate = _unitOfWork.GetRepository<CustomerCampaignEntity>()
                    .GetAll(x => x.CampaignId == campaignId && x.IsJoin == true && x.CustomerCode == customerCode)
                    .OrderBy(x => x.Id).Select(x => x.StartDate).FirstOrDefault();
                var checkFirstMounth = (joinDate?.Month == dateNow.Month && joinDate?.Year == dateNow.Year ? true : false);
                if (checkFirstMounth)
                {
                    response.TargetAmount = onExtraDefinition.CampaignJoinFirstMounthTarget.ToString();
                }
            }
            return await BaseResponse<CustomerCampaignTargetResultDto>.SuccessAsync(response);
        }
    }
}
