using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Campaign.Public.Models.Target.Group;

namespace Bbt.Campaign.Services.Services.Target
{
    public interface ITargetGroupService
    {
        public Task<BaseResponse<TargetGroupDto>> GetTargetGroupAsync(int id);
        public Task<BaseResponse<List<TargetGroupDto>>> GetListAsync();
        public Task<BaseResponse<TargetGroupDto>> AddAsync(TargetGroupInsertRequest request);
        public Task<BaseResponse<TargetGroupDto>> UpdateAsync(TargetGroupUpdateRequest request);       
        public Task<BaseResponse<TargetGroupDto>> DeleteAsync(int id);
        public Task<BaseResponse<TargetGroupDto>> DeleteLineAsync(int id);
        public Task<BaseResponse<TargetGroupInsertFormDto>> GetInsertForm();
        public Task<BaseResponse<TargetGroupUpdateFormDto>> GetUpdateForm(int id);
    }
}
