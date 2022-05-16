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

namespace Bbt.Campaign.Services.Services.Report
{
    public class ReportService : IReportService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly IAuthorizationservice _authorizationservice;
        private static int moduleTypeId = (int)ModuleTypeEnum.Campaign;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, IAuthorizationservice authorizationservice)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _authorizationservice = authorizationservice;
        }

        private async Task<IQueryable<CampaignReportEntity>> GetCampaignQueryAsync(CampaignReportListFilterRequest request) 
        {
            var campaignQuery = _unitOfWork.GetRepository<CampaignReportEntity>().GetAll();

            if (!string.IsNullOrEmpty(request.Code))
            {
                int campaignId = -1;
                try
                {
                    campaignId = int.Parse(request.Code);
                }
                catch (Exception ex)
                {

                }
                campaignQuery = campaignQuery.Where(x => x.Id == campaignId);
            }
            if (!string.IsNullOrEmpty(request.Name))
                campaignQuery = campaignQuery.Where(x => x.Name.Contains(request.Name));
            if (request.ViewOptionId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ViewOptionId == request.ViewOptionId);
            if (!string.IsNullOrEmpty(request.StartDate))
                campaignQuery = campaignQuery.Where(x => x.StartDate.Date >= Convert.ToDateTime(request.StartDate));
            if (!string.IsNullOrEmpty(request.EndDate))
                campaignQuery = campaignQuery.Where(x => x.EndDate.Date <= Convert.ToDateTime(request.EndDate));
            if (request.IsActive.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsActive == request.IsActive.Value);
            if (request.IsBundle.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsBundle == request.IsBundle.Value);
            if (request.ProgramTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value);
            if (!string.IsNullOrEmpty(request.AchievementTypeId))
                campaignQuery = campaignQuery.Where(x => x.AchievementTypeId.Contains(request.AchievementTypeId));
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
                        campaignQuery = isDescending ? campaignQuery.OrderByDescending(x => x.Id) : campaignQuery.OrderBy(x => x.Id);
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

        public async Task<BaseResponse<CampaignReportListFilterResponse>> GetCampaignByFilterAsync(CampaignReportListFilterRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            CampaignReportListFilterResponse response = new CampaignReportListFilterResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CampaignReportEntity> campaignQuery = await GetCampaignQueryAsync(request);

            if (campaignQuery.Count() == 0)
                return await BaseResponse<CampaignReportListFilterResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignQuery.Count();
            campaignQuery = campaignQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var campaignList = campaignQuery.Select(x => new CampaignReportListDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Id.ToString(),
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartDateStr = x.StartDate.ToShortDateString(),
                EndDateStr = x.EndDate.ToShortDateString(),
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
                JoinTypeName = x.JoinTypeName
            }).ToList();

            response.ResponseList = campaignList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CampaignReportListFilterResponse>.SuccessAsync(response);
        }

        public async Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportListFilterRequest request, string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

            GetFileResponse response = new GetFileResponse();

            Helpers.ListByFilterCheckValidation(request);

            IQueryable<CampaignReportEntity> campaignQuery = await GetCampaignQueryAsync(request);

            if (campaignQuery.Count() == 0)
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Kampanya bulunamadı");

            var campaignList = campaignQuery.Select(x => new CampaignReportListDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Id.ToString(),
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartDateStr = x.StartDate.ToShortDateString(),
                EndDateStr = x.EndDate.ToShortDateString(),
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
                JoinTypeName = x.JoinTypeName
            }).ToList();

            byte[] data = ListFileOperations.GetCampaignReportListExcel(campaignList);

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

        public async Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync(string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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

        public async Task<BaseResponse<CustomerReportFormDto>> FillCustomerFormAsync(string userid)
        {
            int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            await _authorizationservice.CheckAuthorizationAsync(userid, moduleTypeId, authorizationTypeId);

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
    }
}
