﻿using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.FileOperations;
using Bbt.Campaign.Services.Services.Parameter;
using Bbt.Campaign.Shared.Extentions;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Bbt.Campaign.Services.Services.Remote;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Dtos;

namespace Bbt.Campaign.Services.Services.Report
{
    public class ReportService : IReportService, IScopedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IParameterService _parameterService;
        private readonly ICampaignTargetService _campaignTargetService;
        private readonly IRemoteService _remoteService;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IParameterService parameterService, ICampaignTargetService campaignTargetService, IRemoteService remoteService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _parameterService = parameterService;
            _campaignTargetService = campaignTargetService;
            _remoteService = remoteService;
        }

        #region Campaign Report
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
            if (request.StatusId.HasValue)
                campaignQuery = campaignQuery.Where(x => x.StatusId == request.StatusId.Value);

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
                DescriptionTr = x.DescriptionTr,
                DescriptionEn = x.DescriptionEn,
                TitleTr = x.TitleTr,
                TitleEn = x.TitleEn,
                ParticipationTypeId = x.ParticipationTypeId,
                ParticipationTypeName = x.ParticipationTypeName
            }).ToList();

            return campaignList;
        }
        public async Task<BaseResponse<CampaignReportResponse>> GetCampaignReportByFilterAsync(CampaignReportRequest request)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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
        public async Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportRequest request)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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
        public async Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync()
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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

        #endregion

        #region Earning Report
        public async Task<BaseResponse<CustomerReportFormDto>> FillEarningFormAsync()
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

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
        public async Task<BaseResponse<CustomerReportResponse>> GetEarningReportByFilterAsync(CustomerReportRequest request)
        {
            CustomerReportResponse response = new CustomerReportResponse();
            ReportListAndTotalCount customerReportList = await GetEarningReportData(request);
            if (!customerReportList.CustomerReportList.Any())
                return await BaseResponse<CustomerReportResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");
            response.CustomerCampaignList = customerReportList.CustomerReportList;
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = customerReportList.TotalCount;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CustomerReportResponse>.SuccessAsync(response);
        }
        private async Task<ReportListAndTotalCount> GetEarningReportData(CustomerReportRequest request)
        {
            ReportListAndTotalCount reportAndCount = new ReportListAndTotalCount() { CustomerReportList = new List<CustomerReportListDto>(), TotalCount = 0 };

            if (StaticValues.IsDevelopment)
            {
                Helpers.ListByFilterCheckValidation(request);

                IQueryable<CustomerReportEntity> query = await GetCustomerQueryAsync(request);

                if (query.Count() == 0)
                    return reportAndCount;

                var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
                var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
                var totalItems = query.Count();
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                reportAndCount.CustomerReportList = await this.ConvertCustomerReportList(query);
                reportAndCount.TotalCount = reportAndCount.CustomerReportList.Count;
            }
            else
            {
                var getCampaignReport = await _remoteService.GetCustomerReportData(request);
                if (getCampaignReport != null && getCampaignReport.ReportData != null && getCampaignReport.ReportData.Any())
                {
                    reportAndCount.TotalCount = getCampaignReport.TotalCount;
                    foreach (var x in getCampaignReport.ReportData)
                    {
                        CustomerReportListDto customerReportListDto = new CustomerReportListDto();
                        customerReportListDto.CampaignCode = x.CampaignCode;
                        customerReportListDto.CampaignName = x.CampaignName;
                        customerReportListDto.IsActive = x.IsActive;
                        customerReportListDto.IsBundle = x.IsBundle;
                        customerReportListDto.Term = x.Term;
                        string joinDateStr = x.CustomerJoinDate ?? string.Empty;
                        if (!string.IsNullOrEmpty(joinDateStr))
                        {
                            string[] joinDateArray = joinDateStr.Split('T');
                            if (joinDateArray.Length == 2)
                            {
                                joinDateArray = joinDateArray[0].Split('-');
                                if (joinDateArray.Length == 3)
                                {
                                    customerReportListDto.JoinDateStr = joinDateArray[2] + "-" + joinDateArray[1] + "-" + joinDateArray[0];
                                    customerReportListDto.JoinDate = Helpers.ConvertUIDateTimeStringForBackEnd(customerReportListDto.JoinDateStr);
                                }

                            }
                        }

                        customerReportListDto.CustomerIdentifier = x.CustomerNumber;
                        customerReportListDto.CustomerCode = x.CustomerId;

                        string earningReachDateStr = x.EarningReachDate ?? string.Empty;
                        if (!string.IsNullOrEmpty(earningReachDateStr))
                        {
                            string[] earningReachDateArray = earningReachDateStr.Split('T');
                            if (earningReachDateArray.Length == 2)
                            {
                                earningReachDateArray = earningReachDateArray[0].Split('-');
                                if (earningReachDateArray.Length == 3)
                                {
                                    customerReportListDto.EarningReachDateStr = earningReachDateArray[2] + "-" + earningReachDateArray[1] + "-" + earningReachDateArray[0];
                                    customerReportListDto.EarningReachDate = Helpers.ConvertUIDateTimeStringForBackEnd(customerReportListDto.EarningReachDateStr);
                                }

                            }
                        }

                        customerReportListDto.AchievementAmountStr = Helpers.ConvertNullablePriceString(x.EarningAmount == null ? null : (decimal)x.EarningAmount);
                        customerReportListDto.AchievementRateStr = Helpers.ConvertNullablePriceString(x.EarningRate == null ? null : (decimal)x.EarningRate);
                        customerReportListDto.CustomerTypeName = x.CustomerType;
                        customerReportListDto.BranchCode = x.BranchCode;
                        customerReportListDto.BranchName = x.BranchCode;
                        customerReportListDto.BusinessLineName = x.BusinessLine;
                        customerReportListDto.AchievementTypeName = x.EarningType;

                        string earningUsedDateStr = x.EarningUsedDate ?? string.Empty;
                        if (!string.IsNullOrEmpty(earningUsedDateStr))
                        {
                            string[] earningUsedDateArray = earningUsedDateStr.Split('T');
                            if (earningUsedDateArray.Length == 2)
                            {
                                earningUsedDateArray = earningUsedDateArray[0].Split('-');
                                if (earningUsedDateArray.Length == 3)
                                {
                                    customerReportListDto.AchievementDateStr = earningUsedDateArray[2] + "-" + earningUsedDateArray[1] + "-" + earningUsedDateArray[0];
                                    customerReportListDto.AchievementDate = Helpers.ConvertUIDateTimeStringForBackEnd(customerReportListDto.AchievementDateStr);
                                }
                            }
                        }
                        reportAndCount.CustomerReportList.Add(customerReportListDto);
                    }
                }
            }

            return reportAndCount;
        }
        public async Task<BaseResponse<GetFileResponse>> GetEarningReportExcelAsync(CustomerReportRequest request)
        {
            GetFileResponse response = new GetFileResponse();

            Helpers.ListByFilterCheckValidation(request);

            ReportListAndTotalCount customerReportList = await GetEarningReportData(request);
            if (!customerReportList.CustomerReportList.Any())
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");

            byte[] data = ReportFileOperations.GetCustomerReportListExcel(customerReportList.CustomerReportList);

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
                AchievementAmountStr = "",
            }).ToList();

            var branchList = (await _parameterService.GetBranchListAsync())?.Data;
            foreach (var customerCampaign in customerCampaignList)
            {
                if (customerCampaign.JoinTypeId == (int)JoinTypeEnum.Branch)
                {
                    if (!string.IsNullOrEmpty(customerCampaign.CampaignCode))
                    {
                        string branchName = string.Empty;
                        string[] branchCodeArray = customerCampaign.BranchCode.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string branchCode in branchCodeArray)
                        {
                            var branch = branchList.Where(x => x.Code == branchCode).FirstOrDefault();
                            if (branch != null)
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
        public async Task<BaseResponse<CustomerReportDetailDto>> GetCustomerReportDetailAsync(string customerCode, string campaignCode, string term = null)
        {
            //int authorizationTypeId = (int)AuthorizationTypeEnum.View;

            //await _authorizationService.CheckAuthorizationAsync(userRole, moduleTypeId, authorizationTypeId);

            CustomerReportDetailDto response = new CustomerReportDetailDto();

            var approvedCampaign = _unitOfWork.GetRepository<CampaignEntity>().GetAll()
                   .Where(x => !x.IsDeleted && x.Code == campaignCode && x.StatusId == (int)StatusEnum.Approved)
                   .FirstOrDefault();
            if (approvedCampaign == null)
                throw new Exception("Kampanya bulunamadı.");

            if (StaticValues.IsDevelopment)
            {
                decimal usedAmount = 1000;
                int usedNumberOfTransaction = 0;
                var campaignTargetDto = await _campaignTargetService.GetCampaignTargetDtoCustomer(approvedCampaign.Id, usedAmount, usedNumberOfTransaction);
                response.CampaignTarget = campaignTargetDto;
            }
            else
            {
                CampaignTargetDto campaignTargetDto = new CampaignTargetDto();

                campaignTargetDto.CampaignId = approvedCampaign.Id;
                campaignTargetDto.GroupCount = 0;
                List<TargetParameterDto2> targetList2 = new List<TargetParameterDto2>();
                CampaignTargetDto2 campaignTargetDto2 = await _campaignTargetService.GetCampaignTargetDtoCustomer2(approvedCampaign.Id, customerCode, "tr", false, term);
                if (campaignTargetDto2.Informationlist.Any() || campaignTargetDto2.ProgressBarlist.Any())
                {
                    foreach (var target in campaignTargetDto2.Informationlist)
                        targetList2.Add(target);
                    foreach (var target in campaignTargetDto2.ProgressBarlist)
                        targetList2.Add(target);

                    campaignTargetDto.TargetAmountStr = campaignTargetDto2.TargetAmountStr;
                    campaignTargetDto.RemainAmountStr = campaignTargetDto2.RemainAmountStr;
                    campaignTargetDto.TargetAmountCurrencyCode = campaignTargetDto2.TargetAmountCurrencyCode;
                    campaignTargetDto.RemainAmountCurrencyCode = campaignTargetDto2.RemainAmountCurrencyCode;
                    campaignTargetDto.UsedAmountStr = campaignTargetDto2.UsedAmountStr;
                    campaignTargetDto.UsedAmountCurrencyCode = campaignTargetDto2.UsedAmountCurrencyCode;

                    var groupList = targetList2.Select(x => x.TargetGroupId).Distinct().ToList();
                    if (groupList.Any())
                    {
                        campaignTargetDto.GroupCount = groupList.Count;

                        foreach (int groupId in groupList)
                        {
                            TargetGroupDto targetGroupDto = new TargetGroupDto();
                            {
                                foreach (var target in targetList2.Where(x => x.TargetGroupId == groupId))
                                {
                                    TargetParameterDto targetParameterDto = new TargetParameterDto();

                                    targetParameterDto.Name = target.Name;
                                    targetParameterDto.Title = target.Title;
                                    targetParameterDto.TargetViewTypeId = target.TargetViewTypeId;
                                    targetParameterDto.UsedAmountStr = target.UsedAmountStr;
                                    targetParameterDto.UsedAmountCurrencyCode = target.UsedAmountCurrencyCode;
                                    targetParameterDto.TargetAmountStr = target.TargetAmountStr;
                                    targetParameterDto.TargetAmountCurrencyCode = target.TargetAmountCurrencyCode;
                                    targetParameterDto.RemainAmountStr = target.RemainAmountStr;
                                    targetParameterDto.RemainAmountCurrencyCode = target.RemainAmountCurrencyCode;
                                    targetParameterDto.Percent = target.Percent;
                                    targetParameterDto.Description = target.Description;
                                    targetParameterDto.DescriptionTr = target.Description;
                                    targetGroupDto.TargetList.Add(targetParameterDto);
                                }
                            }
                            campaignTargetDto.TargetGroupList.Add(targetGroupDto);
                        }
                    }
                }
                response.CampaignTarget = campaignTargetDto;
            }

            return await BaseResponse<CustomerReportDetailDto>.SuccessAsync(response);
        }

        #endregion

        #region Customer Report
        public async Task<BaseResponse<CustomerCampaignReportResponse>> GetCustomerReportByFilterAsync(CustomerCampaignReportRequest request)
        {
            CustomerCampaignReportResponse response = new CustomerCampaignReportResponse();
            CustomerReportListAndTotalCount customerReportList = GetCustomerReportData(request);

            if (!customerReportList.CustomerReportList.Any())
                return await BaseResponse<CustomerCampaignReportResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");
            response.CustomerCampaignList = customerReportList.CustomerReportList;
            var totalItems = customerReportList.TotalCount;
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<CustomerCampaignReportResponse>.SuccessAsync(response);
        }

        public async Task<BaseResponse<GetFileResponse>> GetCustomerReportExcelAsync(CustomerCampaignReportRequest request)
        {
            GetFileResponse response = new GetFileResponse();

            Helpers.ListByFilterCheckValidation(request);

            CustomerReportListAndTotalCount customerReportList = GetCustomerReportData(request);
            if (!customerReportList.CustomerReportList.Any())
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");

            byte[] data = ReportFileOperations.GetCustomerCampaignReportListExcel(customerReportList.CustomerReportList);

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

        public CustomerReportListAndTotalCount GetCustomerReportData(CustomerCampaignReportRequest request)
        {
            var response = new CustomerReportListAndTotalCount() { TotalCount = 0, CustomerReportList = new List<CustomerCampaignReportListDto>() };
            var query = _unitOfWork.GetRepository<CustomerCampaignReportEntity>().GetAll();
            if (request.IsActive != null)
            {
                query = query.Where(x => x.IsActive == request.IsActive);
            }
            if (!String.IsNullOrWhiteSpace(request.CustomerIdentifier))
            {
                query = query.Where(x => x.CustomerId == request.CustomerIdentifier);
            }
            if (!String.IsNullOrWhiteSpace(request.CampaignCode))
            {
                query = query.Where(x => x.CampaignCode == request.CampaignCode);
            }
            if (request.SortDir == "desc")
            {
                switch (request.SortBy)
                {
                    case "CampaignCode":
                        query = query.OrderByDescending(x => x.CampaignCode);
                        break;
                    case "CampaignName":
                        query = query.OrderByDescending(x => x.CampaignName);
                        break;
                    case "IsActive":
                        query = query.OrderByDescending(x => x.IsActive);
                        break;
                    case "IsBundle":
                        query = query.OrderByDescending(x => x.IsBundle);
                        break;
                    case "CustomerId":
                        query = query.OrderByDescending(x => x.CustomerId);
                        break;
                    case "IsExited":
                        query = query.OrderByDescending(x => x.IsExited);
                        break;
                    case "StartDateStr":
                        query = query.OrderByDescending(x => x.CampaignStartDate);
                        break;
                    case "JoinDateStr":
                        query = query.OrderByDescending(x => x.CustomerJoinDate);
                        break;
                    case "ExitDateStr":
                        query = query.OrderByDescending(x => x.CustomerExitDate);
                        break;
                    default:
                        query = query.OrderByDescending(x => x.CampaignCode);
                        break;
                }
            }
            else
            {
                switch (request.SortBy)
                {
                    case "CampaignCode":
                        query = query.OrderBy(x => x.CampaignCode);
                        break;
                    case "CampaignName":
                        query = query.OrderBy(x => x.CampaignName);
                        break;
                    case "IsActive":
                        query = query.OrderBy(x => x.IsActive);
                        break;
                    case "IsBundle":
                        query = query.OrderBy(x => x.IsBundle);
                        break;
                    case "CustomerId":
                        query = query.OrderBy(x => x.CustomerId);
                        break;
                    case "IsExited":
                        query = query.OrderBy(x => x.IsExited);
                        break;
                    case "StartDateStr":
                        query = query.OrderBy(x => x.CampaignStartDate);
                        break;
                    case "JoinDateStr":
                        query = query.OrderBy(x => x.CustomerJoinDate);
                        break;
                    case "ExitDateStr":
                        query = query.OrderBy(x => x.CustomerExitDate);
                        break;
                    default:
                        query = query.OrderBy(x => x.CampaignCode);
                        break;
                }
            }
            response.TotalCount = query.Count();
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            response.CustomerReportList = query.Select(x => new CustomerCampaignReportListDto
            {
                CampaignCode = x.CampaignCode,
                CampaignName = x.CampaignName,
                CampaignStartDate = x.CampaignStartDate,
                CustomerId = x.CustomerId,
                CustomerExitDate = x.CustomerExitDate,
                CustomerJoinDate = x.CustomerJoinDate,
                StartDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.CampaignStartDate),
                JoinDateStr = Helpers.ConvertBackEndDateTimeToStringForUI(x.CustomerJoinDate),
                ExitDateStr = x.CustomerExitDate != null ? Helpers.ConvertBackEndDateTimeToStringForUI(x.CustomerExitDate.Value) : null,
                IsActive = x.IsActive,
                IsBundle = x.IsBundle,
                IsExited = x.IsExited
            }).ToList();
            return response;
        }
        #endregion

        #region Target Report
        public async Task<BaseResponse<TargetReportFormDto>> FillTargetFormAsync()
        {
            TargetReportFormDto response = new TargetReportFormDto();
            await FillTargetFormAsync(response);
            return await BaseResponse<TargetReportFormDto>.SuccessAsync(response);
        }
        private async Task FillTargetFormAsync(TargetReportFormDto response)
        {
            response.IdentitySubTypeList = (await _parameterService.GetIdentitySubTypeListAsync())?.Data;
            response.CampaignList = _unitOfWork.GetRepository<CampaignEntity>()
                .GetAll(x => x.IsActive && x.StatusId == (int)StatusEnum.Approved && !x.IsDeleted && (x.EndDate.AddDays(1) > DateTime.Now))
                .Select(x => _mapper.Map<ParameterDto>(x)).ToList();
            response.TargetList = _unitOfWork.GetRepository<TargetEntity>()
                .GetAll(x => x.IsActive && !x.IsDeleted && x.StatusId == (int)StatusEnum.Approved)
                .Select(x => _mapper.Map<ParameterDto>(x)).ToList();
        }
        public async Task<BaseResponse<TargetReportResponse>> GetTargetReportByFilterAsync(TargetReportRequest request)
        {
            TargetReportResponse response = new TargetReportResponse();

            TargetReportResp targetReportList = await GetTargetReportData(request);
            if (!targetReportList.TargetReportList.Any())
                return await BaseResponse<TargetReportResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");

            response.TargetReportList = targetReportList.TargetReportList;
            var pageNumber = request.PageNumber.GetValueOrDefault(1) < 1 ? 1 : request.PageNumber.GetValueOrDefault(1);
            var pageSize = request.PageSize.GetValueOrDefault(0) == 0 ? 25 : request.PageSize.Value;
            var totalItems = targetReportList.TotalCount;
            response.Paging = Helpers.Paging(totalItems, pageNumber, pageSize);
            return await BaseResponse<TargetReportResponse>.SuccessAsync(response);
        }
        private async Task<TargetReportResp> GetTargetReportData(TargetReportRequest request)
        {
            TargetReportResp targetReportResponse = new TargetReportResp() { TargetReportList = new List<TargetReportListDto>(), TotalCount = 0 };
            if (StaticValues.IsDevelopment)
            {
                Helpers.ListByFilterCheckValidation(request);

                TargetReportListDto targetReportListDto = new TargetReportListDto();
                targetReportListDto.TargetName = "1000 TL Harcama Yap";
                targetReportListDto.CampaignName = "Kıyafet Harcamalarına 150 TL İade";
                targetReportListDto.IsJoin = true;
                targetReportListDto.CustomerCode = "1234567890";
                targetReportListDto.IdentitySubTypeName = "Harcama Koşulsuz Dönem";
                targetReportListDto.TargetAmount = 1000;
                targetReportListDto.TargetAmountStr = "1000";
                targetReportListDto.IsTargetSuccess = true;
                targetReportListDto.RemainAmount = 0;
                targetReportListDto.RemainAmountStr = "0";
                targetReportListDto.TargetSuccessDateStr = "25-07-2022";
                targetReportResponse.TargetReportList.Add(targetReportListDto);

                targetReportListDto = new TargetReportListDto();
                targetReportListDto.TargetName = "23";
                targetReportListDto.CampaignName = "Test 50";
                targetReportListDto.IsJoin = true;
                targetReportListDto.CustomerCode = "1234567892";
                targetReportListDto.IdentitySubTypeName = "Harcama Koşulsuz Dönem";
                targetReportListDto.TargetAmount = 44;
                targetReportListDto.TargetAmountStr = "44";
                targetReportListDto.IsTargetSuccess = false;
                targetReportListDto.RemainAmount = 20;
                targetReportListDto.RemainAmountStr = "20";
                targetReportListDto.TargetSuccessDateStr = null;
                targetReportResponse.TargetReportList.Add(targetReportListDto);

                targetReportResponse.TotalCount = 2;
            }
            else
            {
                var getTargetReport = await _remoteService.GetTargetReportData(request);
                if (getTargetReport != null && getTargetReport.ReportData != null && getTargetReport.ReportData.Any())
                {
                    foreach (var x in getTargetReport.ReportData)
                    {
                        TargetReportListDto targetReportListDto = new TargetReportListDto();

                        targetReportListDto.TargetName = x.TargetName;
                        targetReportListDto.CampaignName = x.CampaignName;
                        targetReportListDto.IsJoin = x.IsJoined;
                        targetReportListDto.CustomerCode = x.CustomerNumber;
                        targetReportListDto.Term = x.Term;

                        int identitySubTypeId = x.SubSegment ?? 0;
                        if (identitySubTypeId > 0)
                            targetReportListDto.IdentitySubTypeName = Helpers.GetEnumDescription<IdentitySubTypeEnum>(identitySubTypeId);

                        targetReportListDto.TargetAmount = x.TargetAmount;
                        targetReportListDto.TargetAmountStr = Helpers.ConvertNullablePriceString(x.TargetAmount == null ? null : (decimal)x.TargetAmount);
                        targetReportListDto.TargetAmountCurrency = x.TargetAmountCurrency;

                        targetReportListDto.IsTargetSuccess = x.IsCompleted;

                        targetReportListDto.RemainAmount = x.RemainingAmount;
                        targetReportListDto.RemainAmountStr = Helpers.ConvertNullablePriceString(x.RemainingAmount == null ? null : (decimal)x.RemainingAmount);
                        targetReportListDto.RemainAmountCurrency = x.RemainingAmountCurrency;

                        string targetSuccessDateStr = x.CompletedAt == null ? null : x.CompletedAt.ToString();
                        if (!string.IsNullOrEmpty(targetSuccessDateStr))
                        {
                            string[] targetSuccessDateArray = targetSuccessDateStr.Split(' ');
                            if (targetSuccessDateArray.Length == 2)
                            {
                                targetSuccessDateStr = targetSuccessDateArray[0].Replace(".", "-");
                                //targetSuccessDateArray = targetSuccessDateArray[0].Split('-');
                                //if (targetSuccessDateArray.Length == 3)
                                //{
                                //    targetSuccessDateStr = targetSuccessDateArray[2] + "-" + targetSuccessDateArray[1] + "-" + targetSuccessDateArray[0];
                                //}
                            }
                        }
                        targetReportListDto.TargetSuccessDateStr = targetSuccessDateStr;

                        targetReportResponse.TargetReportList.Add(targetReportListDto);
                    }
                }
                targetReportResponse.TotalCount = getTargetReport.TotalCount;
            }

            return targetReportResponse;
        }
        public async Task<BaseResponse<GetFileResponse>> GetTargetReportExcelAsync(TargetReportRequest request)
        {
            GetFileResponse response = new GetFileResponse();

            TargetReportResp targetReportList = await GetTargetReportData(request);
            if (!targetReportList.TargetReportList.Any())
                return await BaseResponse<GetFileResponse>.SuccessAsync(response, "Uygun kayıt bulunamadı");

            byte[] data = ReportFileOperations.GetTargetReportListExcel(targetReportList.TargetReportList);

            response = new GetFileResponse()
            {
                Document = new Public.Models.CampaignDocument.DocumentModel()
                {
                    Data = Convert.ToBase64String(data, 0, data.Length),
                    DocumentName = "Hedef Raporu.xlsx",
                    DocumentType = DocumentTypePublicEnum.ExcelReport,
                    MimeType = ".xlsx"
                }
            };
            return await BaseResponse<GetFileResponse>.SuccessAsync(response);
        }

        #endregion
    }
}
