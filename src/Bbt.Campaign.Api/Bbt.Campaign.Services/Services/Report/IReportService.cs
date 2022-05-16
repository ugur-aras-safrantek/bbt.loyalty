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
        public Task<BaseResponse<CampaignReportFormDto>> FillCampaignFormAsync();
        public Task<BaseResponse<CustomerReportFormDto>> FillCustomerFormAsync();
        public Task<BaseResponse<CampaignReportListFilterResponse>> GetCampaignByFilterAsync(CampaignReportListFilterRequest request, string userid);
        public Task<BaseResponse<GetFileResponse>> GetCampaignReportExcelAsync(CampaignReportListFilterRequest request, string userid);
    }
}
