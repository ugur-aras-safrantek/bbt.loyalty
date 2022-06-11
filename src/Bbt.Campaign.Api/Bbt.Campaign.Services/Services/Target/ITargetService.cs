using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Models.File;
using Bbt.Campaign.Public.Models.Target;

namespace Bbt.Target.Services.Services.Target
{
    public interface ITargetService
    {
        public Task<BaseResponse<TargetDto>> AddAsync(TargetInsertRequest target, string userid);
        public Task<BaseResponse<TargetDto>> UpdateAsync(TargetUpdateRequest Target, string userid);
        public Task<BaseResponse<TargetListFilterResponse>> GetByFilterAsync(TargetListFilterRequest request, string userid);
        public Task<BaseResponse<TargetViewFormDto>> GetTargetViewFormAsync(int id);
        public Task<BaseResponse<TargetDto>> DeleteAsync(int id);
        public Task<BaseResponse<TargetDto>> GetTargetAsync(int id);
        public Task<BaseResponse<List<TargetDto>>> GetListAsync();
        public Task<BaseResponse<GetFileResponse>> GetExcelAsync(TargetListFilterRequest request, string userid);
        public Task<TargetDto2> GetTargetDto2(int id);
    }
}
