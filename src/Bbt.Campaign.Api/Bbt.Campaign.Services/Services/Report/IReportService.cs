﻿using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Report
{
    public interface IReportService
    {
        public Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync();
        public Task<BaseResponse<CustomerReportFormDto>> FillEarningFormAsync();
        public Task<BaseResponse<CampaignReportResponse>> GetCampaignReportByFilterAsync(CampaignReportRequest request);
        public Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportRequest request);
        public Task<BaseResponse<CustomerCampaignReportResponse>> GetCustomerReportByFilterAsync(CustomerCampaignReportRequest request);
        public Task<BaseResponse<GetFileResponse>> GetCustomerReportExcelAsync(CustomerCampaignReportRequest request);
        public Task<BaseResponse<CustomerReportResponse>> GetEarningReportByFilterAsync(CustomerReportRequest request);
        public Task<BaseResponse<GetFileResponse>> GetEarningReportExcelAsync(CustomerReportRequest request);
        public Task<BaseResponse<CustomerReportDetailDto>> GetCustomerReportDetailAsync(string customerCode, string campaignCode,string term = null);
        public List<CampaignReportListDto> ConvertCampaignReportList(IQueryable<CampaignReportEntity> query);
        public Task<BaseResponse<TargetReportFormDto>> FillTargetFormAsync();
        public Task<BaseResponse<TargetReportResponse>> GetTargetReportByFilterAsync(TargetReportRequest request);
        public Task<BaseResponse<GetFileResponse>> GetTargetReportExcelAsync(TargetReportRequest request);
    }
}
