using Bbt.Campaign.Public.BaseResultModels;
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
        public Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync(string userid);
        public Task<BaseResponse<CustomerReportFormDto>> FillCustomerFormAsync(string userid);
        public Task<BaseResponse<CampaignReportResponse>> GetCampaignReportByFilterAsync(CampaignReportRequest request, string userid);
        public Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportRequest request, string userid);
        public Task<BaseResponse<CustomerReportResponse>> GetCustomerReportByFilterAsync(CustomerReportRequest request, string userid);
        public Task<BaseResponse<GetFileResponse>> GetCustomerReportExcelAsync(CustomerReportRequest request, string userid);
    }
}
