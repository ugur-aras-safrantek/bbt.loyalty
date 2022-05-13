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
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.Services.Services.Report
{
    public class ReportService : IReportService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
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
            if (request.AchievementTypeId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.AchievementTypeId == request.AchievementTypeId.Value);
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
                if (request.SortBy.Equals("Code"))
                {
                    if (isDescending)
                        campaignQuery = campaignQuery.OrderByDescending(x => x.Id);
                    else
                        campaignQuery = campaignQuery.OrderBy(x => x.Id);
                }
                else
                {
                    if (isDescending)
                        campaignQuery = campaignQuery.OrderByDescending(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null));
                    else
                        campaignQuery = campaignQuery.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s, null));
                }
            }

            return campaignQuery;          
        }

        public async Task<BaseResponse<CampaignReportListFilterResponse>> GetCampaignByFilterAsync(CampaignReportListFilterRequest request)
        {
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
                ContractId = x.ContractId,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                SectorName = x.SectorName,
                ViewOptionName = x.ViewOptionName,
                ProgramTypeName = x.ProgramTypeName,
                AchievementTypeName = x.AchievementTypeName,
                JoinTypeName = x.JoinTypeName

            }).ToList();

            response.ResponseList = campaignList;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CampaignReportListFilterResponse>.SuccessAsync(response);
        }

        //public async Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportListFilterRequest request) 
        //{
        //    GetFileResponse response = new GetFileResponse();

        //    //Core.Helper.Helpers.ListByFilterCheckValidation(request);

        //    //var campaignQuery = _unitOfWork.GetRepository<CampaignEntity>().GetAll(x => !x.IsDeleted);

        //    //if (request.IsBundle.HasValue)
        //    //    campaignQuery = campaignQuery.Where(x => x.IsBundle == request.IsBundle.Value);
        //    //if (!string.IsNullOrEmpty(request.CampaignCode) && !string.IsNullOrWhiteSpace(request.CampaignCode))
        //    //{
        //    //    int campaignId = -1;
        //    //    try
        //    //    {
        //    //        campaignId = int.Parse(request.CampaignCode);
        //    //    }
        //    //    catch (Exception ex){}
        //    //    campaignQuery = campaignQuery.Where(x => x.Id == campaignId);
        //    //}

        //    //if (!string.IsNullOrEmpty(request.CampaignName) && !string.IsNullOrWhiteSpace(request.CampaignName))
        //    //    campaignQuery = campaignQuery.Where(x => x.Name.Contains(request.CampaignName));
        //    //if (request.ContractId.HasValue)
        //    //    campaignQuery = campaignQuery.Where(x => x.ContractId == request.ContractId.Value);
        //    //if (request.IsActive.HasValue)
        //    //    campaignQuery = campaignQuery.Where(x => x.IsActive == request.IsActive.Value);
        //    //if (request.ProgramTypeId.HasValue)
        //    //    campaignQuery = campaignQuery.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value);
        //    //if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrWhiteSpace(request.StartDate))
        //    //    campaignQuery = campaignQuery.Where(x => x.StartDate.Date >= Convert.ToDateTime(request.StartDate));
        //    //if (!string.IsNullOrEmpty(request.EndDate) && !string.IsNullOrWhiteSpace(request.EndDate))
        //    //    campaignQuery = campaignQuery.Where(x => x.EndDate.Date <= Convert.ToDateTime(request.EndDate));
        //    //if (request.IsApproved.HasValue)
        //    //    campaignQuery = campaignQuery.Where(x => x.IsApproved == request.IsApproved.Value);

        //    //campaignQuery = campaignQuery.OrderByDescending(x => x.Id);


        //    campaignQuery = campaignQuery.OrderByDescending(x => x.CreatedOn);
        //    var totalItems = campaignQuery.Count();
        //    if (totalItems == 0)
        //    {
        //        return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        //    }

        //    campaignQuery = campaignQuery.OrderByDescending(x => x.CreatedOn);

        //    var campaignList = campaignQuery.Select(x => new CampaignListDto
        //    {
        //        Id = x.Id,
        //        Code = x.Id.ToString(),
        //        StartDate = DateTime.Parse(x.StartDate.ToShortDateString()),
        //        EndDate = DateTime.Parse(x.EndDate.ToShortDateString()),
        //        ContractId = x.ContractId,
        //        IsActive = x.IsActive,
        //        IsBundle = x.IsBundle,
        //        ProgramType = x.ProgramType.Name,
        //        Name = x.Name
        //    }).ToList();

        //    byte[] data = ListFileOperations.GetCampaignListExcel(campaignList);

        //    response = new GetFileResponse()
        //    {
        //        Document = new Public.Models.CampaignDocument.DocumentModel()
        //        {
        //            Data = Convert.ToBase64String(data, 0, data.Length),
        //            DocumentName = "Kampanya Listesi.xlsx",
        //            DocumentType = DocumentTypePublicEnum.ExcelReport,
        //            MimeType = MimeTypeExtensions.ToMimeType(".xlsx")
        //        }
        //    };
        //    return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        //}

        public async Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync()
        {
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

        public async Task<BaseResponse<CustomerReportFormDto>> FillCustomerFormAsync()
        {
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
