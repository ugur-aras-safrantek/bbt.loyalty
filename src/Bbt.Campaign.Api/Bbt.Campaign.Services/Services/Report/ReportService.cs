using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Services.Services.Authorization;
using Bbt.Campaign.Shared.Static;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Authorization;

namespace Bbt.Campaign.Services.Services.Report
{
    public class ReportService : IReportService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICampaignTargetService _campaignTargetService;
        private static int moduleTypeId = (int)ModuleTypeEnum.Campaign;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, 
            IAuthorizationService authorizationService, ICampaignTargetService campaignTargetService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationService = authorizationService;
            _campaignTargetService = campaignTargetService;
        }
        private async Task<IQueryable<CampaignReportEntity>> GetCampaignQueryAsync(CampaignReportRequest request) 
        {
            var campaignQuery = _unitOfWork.GetRepository<CampaignReportEntity>().GetAll();

            if (!string.IsNullOrEmpty(request.Code) && !string.IsNullOrWhiteSpace(request.Code))
                campaignQuery = campaignQuery.Where(x => x.Code.Contains(request.Code));
            if (!string.IsNullOrEmpty(request.Name))
                campaignQuery = campaignQuery.Where(x => x.Name.Contains(request.Name));
            if (request.ViewOptionId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ViewOptionId == request.ViewOptionId);
            if (!string.IsNullOrEmpty(request.StartDate))
                campaignQuery = campaignQuery.Where(x => x.StartDate.Date >= Helpers.ConvertUIDateTimeStringForBackEnd(request.StartDate));
            if (!string.IsNullOrEmpty(request.EndDate))
                campaignQuery = campaignQuery.Where(x => x.EndDate.Date <= Helpers.ConvertUIDateTimeStringForBackEnd(request.EndDate));
            if (request.IsActive.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsActive == request.IsActive.Value);
            if (request.IsBundle.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsBundle == request.IsBundle.Value);
            if (request.ProgramTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value);
            if (request.AchievementTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.AchievementTypeId.Contains(request.AchievementTypeId.ToString()));
            if (request.JoinTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.JoinTypeId == request.JoinTypeId.Value);
            if (request.SectorId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.SectorId == request.SectorId.Value);

            if (string.IsNullOrEmpty(request.SortBy))
            {
                campaignQuery = campaignQuery.OrderByDescending(x => x.Id);
            }
            else
            {
                if (request.SortBy.EndsWith("Str"))
                    request.SortBy = request.SortBy.Substring(0, request.SortBy.Length - 3);

                bool isDescending = request.SortDir?.ToLower() == "desc";

                switch (request.SortBy) 
                {
                    case "Name":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Name) : campaignQuery.OrderBy(x => x.Name);
                            break;
                    case "Code":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Code) : campaignQuery.OrderBy(x => x.Code);
                        break;
                    case "ContractId":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.ContractId) : campaignQuery.OrderBy(x => x.ContractId);
                        break;
                    case "ViewOptionName":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.ViewOptionName) : campaignQuery.OrderBy(x => x.ViewOptionName);
                        break;
                    case "StartDate":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.StartDate) : campaignQuery.OrderBy(x => x.StartDate);
                        break;
                    case "EndDate":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.EndDate) : campaignQuery.OrderBy(x => x.EndDate);
                        break;
                    case "IsActive":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.IsActive) : campaignQuery.OrderBy(x => x.IsActive);
                        break;
                    case "IsBundle":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.IsBundle) : campaignQuery.OrderBy(x => x.IsBundle);
                        break;
                    case "Order":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Order) : campaignQuery.OrderBy(x => x.Order);
                        break;
                    case "ProgramTypeName":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.ProgramTypeName) : campaignQuery.OrderBy(x => x.ProgramTypeName);
                        break;
                    case "AchievementTypeName":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.AchievementTypeName) : campaignQuery.OrderBy(x => x.AchievementTypeName);
                        break;
                    case "JoinTypeName":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.JoinTypeName) : campaignQuery.OrderBy(x => x.JoinTypeName);
                        break;
                    case "SectorName":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.SectorName) : campaignQuery.OrderBy(x => x.SectorName);
                        break;
                    case "CustomerCount":
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.CustomerCount) : campaignQuery.OrderBy(x => x.CustomerCount);
                        break;
                    default: 
                        break;
                }
            }

            return campaignQuery;          
        }
        public List<CampaignReportListDto> ConvertCampaignReportList(IQueryable<CampaignReportEntity> query) 
        {
            var campaignList = query.Select(x => new CampaignReportListDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.StartDate),
                EndDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.EndDate),
                IsContract = x.IsContract,
                ContractId = x.ContractId,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                Order = x.Order,
                SectorId = x.SectorId,
                SectorName = x.SectorName,
                ViewOptionId = x.ViewOptionId,
                ViewOptionName = x.ViewOptionName,
                ProgramTypeId = x.ProgramTypeId,
                ProgramTypeName = x.ProgramTypeName,
                AchievementTypeId = x.AchievementTypeId,
                AchievementTypeName = x.AchievementTypeName,
                AchievementAmount = x.AchievementAmount,
                AchievementRate = x.AchievementRate,
                TopLimitName = x.TopLimitName,
                CustomerCount = x.CustomerCount ?? 0,
                JoinTypeId = x.JoinTypeId,
                JoinTypeName = x.JoinTypeName,
                StatusId = x.StatusId,
                StatusName = x.StatusName,
                MaxNumberOfUser = x.MaxNumberOfUser,
            }).ToList();

            return campaignList;
        }
        public async Task<BaseResponse<CampaignReportResponse>> GetCampaignReportByFilterAsync(CampaignReportRequest request, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignReportResponse response = new CampaignReportResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CampaignReportEntity> campaignQuery = await GetCampaignQueryAsync(request);

            if (campaignQuery.Count() == 0)
                return await BaseResponse<CampaignReportResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignQuery.Count();
            campaignQuery = campaignQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var campaignList = ConvertCampaignReportList(campaignQuery);

            response.CampaignList = campaignList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CampaignReportResponse>.SuccessAsync(response);
        }
        public async Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportRequest request, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            GetFileResponse response = new GetFileResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CampaignReportEntity> campaignQuery = await GetCampaignQueryAsync(request);

            if (campaignQuery.Count() == 0)
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var campaignList = ConvertCampaignReportList(campaignQuery);

            byte[] data = ReportFileOperations.GetCampaignReportListExcel(campaignList);

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
        public async Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync(UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CampaignReportFormDto response = new CampaignReportFormDto();
            await FillCampaignFormAsync(response);
            return await BaseResponse<CampaignReportFormDto>.SuccessAsync(response);
        }
        private async Task FillCampaignFormAsync(CampaignReportFormDto response)
        {
            response.ActionOptionList = (await _parameterService.GetActionOptionListAsync())?.Data;
            response.ViewOptionList = (await _parameterService.GetViewOptionListAsync())?.Data;
            response.SectorList = (await _parameterService.GetSectorListAsync())?.Data;
            response.ProgramTypeList = (await _parameterService.GetProgramTypeListAsync())?.Data;
            response.JoinTypeList = (await _parameterService.GetJoinTypeListAsync())?.Data;
            response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
        }
        public async Task<BaseResponse<CustomerReportFormDto>> FillCustomerFormAsync(UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CustomerReportFormDto response = new CustomerReportFormDto();
            await FillCustomerFormAsync(response);
            return await BaseResponse<CustomerReportFormDto>.SuccessAsync(response);
        }
        private async Task FillCustomerFormAsync(CustomerReportFormDto response)
        {
            response.CampaignStartTermList = (await _parameterService.GetCampaignStartTermListAsync())?.Data;
            response.CustomerTypeList = (await _parameterService.GetCustomerTypeListAsync())?.Data;
            response.BusinessLineList = (await _parameterService.GetBusinessLineListAsync())?.Data;
            response.AchievementTypes = (await _parameterService.GetAchievementTypeListAsync())?.Data;
        }       
        public async Task<BaseResponse<CustomerReportResponse>> GetCustomerReportByFilterAsync(CustomerReportRequest request, UserRoleDto userRole)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CustomerReportResponse response = new CustomerReportResponse();



            if (StaticValues.IsDevelopment) 
            {
                Helpers.ListByFilterCheckValidation(request);

                IQueryable<CustomerReportEntity> query = await GetCustomerQueryAsync(request);

                if (query.Count() == 0)
                    return await BaseResponse<CustomerReportResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");

                var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
                var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
                var totalItems = query.Count();
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                var customerCampaignList = await this.ConvertCustomerReportList(query);

                response.CustomerCampaignList = customerCampaignList;
                response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            }
            else 
            { 
                
            }
            
            
            return await BaseResponse<CustomerReportResponse>.SuccessAsync(response);
        }
        public async Task<BaseResponse<GetFileResponse>> GetCustomerReportExcelAsync(CustomerReportRequest request, UserRoleDto userRole) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            GetFileResponse response = new GetFileResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CustomerReportEntity> query = await GetCustomerQueryAsync(request);

            if (query.Count() == 0)
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı.");

            var customerCampaignList = await this.ConvertCustomerReportList(query);

            byte[] data = ReportFileOperations.GetCustomerReportListExcel(customerCampaignList);

            response = new GetFileResponse()
            {
                Document = new Public.Models.CampaignDocument.DocumentModel()
                {
                    Data = Convert.ToBase64String(data, 0, data.Length),
                    DocumentName = "Müşteri Listesi.xlsx",
                    DocumentType = DocumentTypePublicEnum.ExcelReport,
                    MimeType = MimeTypeExtensions.ToMimeType(".xlsx")
                }
            };
            return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        }
        private async Task<IQueryable<CustomerReportEntity>> GetCustomerQueryAsync(CustomerReportRequest request)
        {
            var query = _unitOfWork.GetRepository<CustomerReportEntity>().GetAll();

            if (!string.IsNullOrEmpty(request.CustomerCode))
                query = query.Where(x => x.CustomerCode.Contains(request.CustomerCode));
            if (!string.IsNullOrEmpty(request.CustomerIdentifier))
                query = query.Where(x => x.CustomerIdentifier.Contains(request.CustomerIdentifier));
            if (request.CustomerTypeId.HasValue)
                query = query.Where(x => x.CustomerTypeId != null && x.CustomerTypeId.Contains(request.CustomerTypeId.ToString()));
            if (request.CampaignStartTermId.HasValue)
                query = query.Where(x => x.CampaignStartTermId == request.CampaignStartTermId);
            if (!string.IsNullOrEmpty(request.BranchCode))
                query = query.Where(x => x.BranchCode != null && x.BranchCode.Contains(request.BranchCode));
            if (request.AchievementTypeId.HasValue)
                query = query.Where(x => x.AchievementTypeId != null && x.AchievementTypeId.Contains(request.AchievementTypeId.ToString()));
            if (request.BusinessLineId.HasValue)
                query = query.Where(x => x.BusinessLineId != null && x.BusinessLineId.Contains(request.BusinessLineId.ToString()));
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            if (request.IsBundle.HasValue)
                query = query.Where(x => x.IsBundle == request.IsBundle.Value);

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
                    case "CustomerCode":
                        query = isDescending ? query.OrderByDescending(x => x.CustomerCode) : query.OrderBy(x => x.CustomerCode);
                        break;
                    //case "CustomerIdentifier":
                    //    query = isDescending ? query.OrderByDescending(x => x.CustomerIdentifier) : query.OrderBy(x => x.CustomerIdentifier);
                    //    break;
                    case "CustomerTypeName":
                        query = isDescending ? query.OrderByDescending(x => x.CustomerTypeName) : query.OrderBy(x => x.CustomerTypeName);
                        break;
                    case "AchievementTypeName":
                        query = isDescending ? query.OrderByDescending(x => x.AchievementTypeName) : query.OrderBy(x => x.AchievementTypeName);
                        break;
                    case "CampaignStartTermName":
                        query = isDescending ? query.OrderByDescending(x => x.CampaignStartTermName) : query.OrderBy(x => x.CampaignStartTermName);
                        break;
                    case "BranchName":
                        query = isDescending ? query.OrderByDescending(x => x.BranchName) : query.OrderBy(x => x.BranchName);
                        break;
                    case "BusinessLineName":
                        query = isDescending ? query.OrderByDescending(x => x.BusinessLineName) : query.OrderBy(x => x.BusinessLineName);
                        break;
                    //case "AchievementDate":
                    //    query = isDescending ? query.OrderByDescending(x => x.AchievementDate) : query.OrderBy(x => x.AchievementDate);
                    //    break;
                    case "CampaignCode":
                        query = isDescending ? query.OrderByDescending(x => x.CampaignCode) : query.OrderBy(x => x.CampaignCode);
                        break;
                    case "CampaignName":
                        query = isDescending ? query.OrderByDescending(x => x.CampaignName) : query.OrderBy(x => x.CampaignName);
                        break;
                    case "IsActive":
                        query = isDescending ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive);
                        break;
                    case "IsBundle":
                        query = isDescending ? query.OrderByDescending(x => x.IsBundle) : query.OrderBy(x => x.IsBundle);
                        break;
                    case "IsContinuingCampaign":
                        query = isDescending ? query.OrderByDescending(x => x.IsContinuingCampaign) : query.OrderBy(x => x.IsContinuingCampaign);
                        break;
                    case "JoinDate":
                        query = isDescending ? query.OrderByDescending(x => x.JoinDate) : query.OrderBy(x => x.JoinDate);
                        break;
                    default:
                        break;
                }
            }
            return query;
        }
        private async Task<List<CustomerReportListDto>> ConvertCustomerReportList(IQueryable<CustomerReportEntity> query) 
        {
            var customerCampaignList = query.Select(x => new CustomerReportListDto
            {
                Id = x.Id,
                CustomerCode = x.CustomerCode,
                CustomerIdentifier = x.CustomerIdentifier,
                JoinDate = x.JoinDate,
                JoinDateStr = x.JoinDateStr,
                CampaignId = x.CampaignId,
                CampaignCode = x.CampaignCode,
                CampaignName = x.CampaignName,
                IsContinuingCampaign = x.IsContinuingCampaign == 1 ? true : false,
                CampaignStartDate = x.CampaignStartDate,
                CampaignStartDateStr = x.CampaignStartDateStr,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                JoinTypeId = x.JoinTypeId,
                JoinTypeName = x.JoinTypeName,
                CustomerTypeId = x.CustomerTypeId,
                CustomerTypeName = x.CustomerTypeName,
                CampaignStartTermId = x.CampaignStartTermId,
                CampaignStartTermName = x.CampaignStartTermName,
                BusinessLineId = x.BusinessLineId,
                BusinessLineName = x.BusinessLineName,
                BranchCode = x.BranchCode,
                BranchName = x.BranchName,
                AchievementTypeId = x.AchievementTypeId,
                AchievementTypeName = x.AchievementTypeName,
            }).ToList();

            var branchList = (await _parameterService.GetBranchListAsync())?.Data;
            foreach (var customerCampaign in customerCampaignList) 
            { 
                if(customerCampaign.JoinTypeId == (int)JoinTypeEnum.Branch) 
                {
                    if (!string.IsNullOrEmpty(customerCampaign.CampaignCode)) 
                    {
                        string branchName = string.Empty;
                        string[] branchCodeArray = customerCampaign.BranchCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach(string branchCode in branchCodeArray) 
                        { 
                            var branch = branchList.Where(x=>x.Code == branchCode).FirstOrDefault();
                            if(branch != null) 
                            {
                                branchName += "," + branchCode + " - " + branch.Name;
                            }
                        }
                        if (!string.IsNullOrEmpty(branchName)) 
                        {
                            branchName = branchName.Remove(0, 1);
                            customerCampaign.BranchName = branchName;
                        }
                    }
                }
            }
            
            return customerCampaignList;
        }
        public async Task<BaseResponse<CustomerReportDetailDto>> GetCustomerReportDetailAsync(int id, UserRoleDto userRole) 
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CustomerReportDetailDto response = new CustomerReportDetailDto();
            CustomerReportDetailEntity customerReportDetailEntity = new CustomerReportDetailEntity();
            CustomerReportDetailDto customerReportDetailDto = new CustomerReportDetailDto();
            var query = _unitOfWork.GetRepository<CustomerReportEntity>().GetAll().Where(x => x.Id == id);
            if (query.Count() == 0)
                return await BaseResponse<CustomerReportDetailDto>.SuccessAsync(response, "Uygun kayıt bulunamadı");
            var customerCampaignList = await this.ConvertCustomerReportList(query);
            var customerCampaign = customerCampaignList.FirstOrDefault();
            decimal usedAmount = 0;
            int usedNumberOfTransaction = 0;

            if (StaticValues.IsDevelopment) 
            {
                customerReportDetailEntity.CampaignCode = customerCampaign.CampaignCode;
                customerReportDetailEntity.CampaignName = customerCampaign.CampaignName;
                customerReportDetailEntity.IsActive = customerCampaign.IsActive;
                customerReportDetailEntity.IsBundle = customerCampaign.IsBundle;
                customerReportDetailEntity.CustomerNumber = customerCampaign.CustomerCode;
                customerReportDetailEntity.CustomerId = "12345678910";
                customerReportDetailEntity.CustomerType = customerCampaign.CustomerTypeName;
                customerReportDetailEntity.BranchCode = customerCampaign.BranchCode;
                customerReportDetailEntity.BusinessLine = customerCampaign.BusinessLineName;
                customerReportDetailEntity.EarningType = customerCampaign.AchievementTypeName;
                customerReportDetailEntity.CustomerJoinDate = customerCampaign.JoinDate;
                customerReportDetailEntity.CustomerJoinDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(customerCampaign.JoinDate);
                customerReportDetailEntity.EarningAmount = 30;
                customerReportDetailEntity.EarningAmountStr = Helpers.ConvertNullablePriceString(customerReportDetailEntity.EarningAmount);
                customerReportDetailEntity.EarningRate = null;
                customerReportDetailEntity.EarningRateStr = Helpers.ConvertNullablePriceString(customerReportDetailEntity.EarningRate);
                customerReportDetailEntity.IsEarningUsed = true;
                customerReportDetailEntity.EarningUsedDate = Helpers.ConvertDateTimeToShortDate(DateTime.Now);
                customerReportDetailEntity.EarningUsedDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(DateTime.Now.AddDays(-15));
                customerReportDetailEntity.CampaignStartDate = customerCampaign.CampaignStartDate;
                customerReportDetailEntity.CampaignStartDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(customerCampaign.CampaignStartDate); 
                response = _mapper.Map<CustomerReportDetailDto>(customerReportDetailEntity);

                usedAmount = 1000;
                usedNumberOfTransaction = 2;
            }
            else 
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var serviceResponse = await httpClient.GetAsync(StaticValues.ChannelCodeServiceUrl);
                    if (serviceResponse.IsSuccessStatusCode)
                    {
                        if (serviceResponse.Content != null)
                        {
                            //string apiResponse = await serviceResponse.Content.ReadAsStringAsync();
                            //channelCodeList = JsonConvert.DeserializeObject<List<string>>((JObject.Parse(apiResponse)["data"]).ToString());
                            //if (channelCodeList != null && channelCodeList.Any()) { }
                            //else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                        }
                        else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                    }
                    else { throw new Exception("Kazanım kanalı servisinden veri çekilemedi."); }
                }

            }

            var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(customerCampaign.CampaignId, usedAmount, usedNumberOfTransaction);
            response.CampaignTarget = campaignTargetDto;

            return await BaseResponse<CustomerReportDetailDto>.SuccessAsync(response);
        }
    }
}
