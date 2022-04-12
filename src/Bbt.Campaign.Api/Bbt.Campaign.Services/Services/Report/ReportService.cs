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

        public async Task<BaseResponse<CampaignReportListFilterResponse>> GetCampaignByFilterAsync(CampaignReportListFilterRequest request) 
        {
            var campaignList = await GetCampaignFilteredList(request);
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = campaignList.Count();

            if(string.IsNullOrEmpty(request.SortBy))
                campaignList = campaignList.OrderByDescending(x => x.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            else 
            {
                var pi = typeof(CampaignEntity).GetProperty(request.SortBy);
                var orderByAddress = campaignList.OrderBy(x => pi.GetValue(x, null));


                //campaignList.OrderBy(s => s.GetType().GetProperty(request.SortBy).GetValue(s));
            }
            
            
            
            var campaignListFiltered = campaignList.Select(x => new CampaignReportListDto
            {
                Id = x.Id,
                Code = x.Id.ToString(),
                StartDate = x.StartDate.ToShortDateString(),
                EndDate = x.EndDate.ToShortDateString(),
                ContractId = x.ContractId,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                SectorName = x.SectorId == null ? null : Helpers.GetEnumDescription<SectorsEnum>(x.SectorId ?? 0),
                ViewOptionName = x.ViewOptionId == null ? null : Helpers.GetEnumDescription<ViewOptionsEnum>(x.ViewOptionId ?? 0),
                ProgramTypeName = Helpers.GetEnumDescription<ProgramTypeEnum>(x.ProgramTypeId),
                AchievementTypeName = x.Achievement == null ? null : Helpers.GetEnumDescription<AchievementTypeEnum>(x.Achievement.AchievementTypeId),
                JoinTypeName = x.CampaignRule == null ? null : Helpers.GetEnumDescription<JoinTypeEnum>(x.CampaignRule.JoinTypeId),
                Name = x.Name
            }).ToList();

            CampaignReportListFilterResponse response = new CampaignReportListFilterResponse();
            response.ResponseList = campaignListFiltered;
            response.Paging = Core.Helper.Helpers.Paging(totalItems, pageNumber, pageSize);

            return await BaseResponse<CampaignReportListFilterResponse>.SuccessAsync(response);
        }


        private async Task<List<CampaignEntity>> GetCampaignFilteredList(CampaignReportListFilterRequest request)
        {
            Core.Helper.Helpers.ListByFilterCheckValidation(request);


            

             var campaignList = _unitOfWork.GetRepository<CampaignEntity>()
               .GetAll(x => !x.IsDeleted && x.Id == 15)
               .Include(x => x.CampaignRule)
               .Include(x => x.Achievement)
               .ToList();

            if (request.IsBundle.HasValue)
                campaignList = campaignList.Where(x => x.IsBundle == request.IsBundle.Value).ToList();
            if (!string.IsNullOrEmpty(request.CampaignCode) && !string.IsNullOrWhiteSpace(request.CampaignCode))
            {
                int campaignId = -1;
                try
                {
                    campaignId = int.Parse(request.CampaignCode);
                }
                catch (Exception ex){}

                campaignList = campaignList.Where(x => x.Id == campaignId).ToList();
            }

            if (!string.IsNullOrEmpty(request.CampaignName) && !string.IsNullOrWhiteSpace(request.CampaignName))
                campaignList = campaignList.Where(x => x.Name.Contains(request.CampaignName)).ToList();
            if (request.ContractId.HasValue)
                campaignList = campaignList.Where(x => x.ContractId == request.ContractId.Value).ToList();
            if (request.IsActive.HasValue)
                campaignList = campaignList.Where(x => x.IsActive == request.IsActive.Value).ToList();
            if (request.ProgramTypeId.HasValue)
                campaignList = campaignList.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value).ToList();
            if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrWhiteSpace(request.StartDate))
                campaignList = campaignList.Where(x => x.StartDate.Date >= Convert.ToDateTime(request.StartDate)).ToList();
            if (!string.IsNullOrEmpty(request.EndDate) && !string.IsNullOrWhiteSpace(request.EndDate))
                campaignList = campaignList.Where(x => x.EndDate.Date <= Convert.ToDateTime(request.EndDate)).ToList();
            if (request.IsApproved.HasValue)
                campaignList = campaignList.Where(x => x.IsApproved == request.IsApproved.Value).ToList();
            if (request.SectorId.HasValue)
                campaignList = campaignList.Where(x => x.SectorId == request.SectorId.Value).ToList();
            if (request.ViewOptionId.HasValue)
                campaignList = campaignList.Where(x => x.ViewOptionId == request.ViewOptionId.Value).ToList();           
            if (request.ProgramTypeId.HasValue)
                campaignList = campaignList.Where(x => x.ProgramTypeId == request.ProgramTypeId.Value).ToList();
            if (request.AchievementTypeId.HasValue)
                campaignList = campaignList.Where(x => x.Achievement?.AchievementTypeId == request.AchievementTypeId.Value).ToList();
            if (request.JoinTypeId.HasValue)
                campaignList = campaignList.Where(x => x.CampaignRule?.JoinTypeId == request.JoinTypeId.Value).ToList();

            return campaignList;
        }

        public async Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportListFilterRequest request) 
        {
            GetFileResponse response = new GetFileResponse();

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
                catch (Exception ex){}
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
            if (request.IsApproved.HasValue)
                campaignQuery = campaignQuery.Where(x => x.IsApproved == request.IsApproved.Value);

            campaignQuery = campaignQuery.OrderByDescending(x => x.Id);


            campaignQuery = campaignQuery.OrderByDescending(x => x.CreatedOn);
            var totalItems = campaignQuery.Count();
            if (totalItems == 0)
            {
                return await BaseResponse<GetFileResponse>.SuccessAsync(response);
            }

            campaignQuery = campaignQuery.OrderByDescending(x => x.CreatedOn);

            var campaignList = campaignQuery.Select(x => new CampaignListDto
            {
                Id = x.Id,
                Code = x.Id.ToString(),
                StartDate = DateTime.Parse(x.StartDate.ToShortDateString()),
                EndDate = DateTime.Parse(x.EndDate.ToShortDateString()),
                ContractId = x.ContractId,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                ProgramType = x.ProgramType.Name,
                Name = x.Name
            }).ToList();

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
    }
}
