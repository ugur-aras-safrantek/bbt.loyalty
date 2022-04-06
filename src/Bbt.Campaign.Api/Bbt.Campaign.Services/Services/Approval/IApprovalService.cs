using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Dtos.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.Approval
{
    public interface IApprovalService
    {
        public Task<BaseResponse<CampaignDto>> ApproveCampaignAsync(int id);
        //public Task<BaseResponse<TargetDto>> ApproveTargetAsync(int id);
        //public Task<BaseResponse<TopLimitDto>> ApproveTopLimitAsync(int id);
        public Task<BaseResponse<TargetApproveFormDto>> GetTargetApprovalFormAsync(int id);
        public Task<BaseResponse<TopLimitApproveFormDto>> GetTopLimitApprovalFormAsync(int id);
        public Task<BaseResponse<CampaignApproveFormDto>> GetCampaignApprovalFormAsync(int id);
        public Task<BaseResponse<CampaignViewFormDto>> GetCampaignViewFormAsync(int campaignId);
        public Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int campaignId);
        public Task<BaseResponse<TopLimitDto>> TopLimitCopyAsync(int topLimitId);
        public Task<BaseResponse<TargetDto>> TargetCopyAsync(int targetId);
    }
}
