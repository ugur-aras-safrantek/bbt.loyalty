using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Approval;
using Bbt.Campaign.Public.Dtos.Authorization;
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
        public Task<BaseResponse<CampaignDto>> ApproveCampaignAsync(int id,  UserRoleDto userRoleDto);
        public Task<BaseResponse<CampaignDto>> DisApproveCampaignAsync(int id, UserRoleDto userRoleDto);      
        public Task<BaseResponse<CampaignApproveFormDto>> GetCampaignApprovalFormAsync(int id, UserRoleDto userRoleDto);
        public Task<BaseResponse<CampaignViewFormDto>> GetCampaignViewFormAsync(int campaignId);
        
        
        


        public Task<BaseResponse<TopLimitDto>> ApproveTopLimitAsync(int id, bool isApproved, UserRoleDto userRoleDto);
        public Task<BaseResponse<TopLimitApproveFormDto>> GetTopLimitApprovalFormAsync(int id, UserRoleDto userRoleDto);


        public Task<BaseResponse<TargetDto>> ApproveTargetAsync(int id, bool isApproved, UserRoleDto userRoleDto);
        public Task<BaseResponse<TargetApproveFormDto>> GetTargetApprovalFormAsync(int id, UserRoleDto userRoleDto);



        public Task<BaseResponse<CampaignDto>> CampaignCopyAsync(int campaignId, UserRoleDto userRoleDto);
        public Task<BaseResponse<TopLimitDto>> TopLimitCopyAsync(int topLimitId, UserRoleDto userRoleDto);
        public Task<BaseResponse<TargetDto>> TargetCopyAsync(int targetId, UserRoleDto userRoleDto);




        public DateTime ConvertWithInvariantCulture(string date, string format);
        public DateTime ConvertWithCulture(string date, string format, string culture);
        public DateTime ConvertWithNewDateTime(string dateStr);
    }
}
