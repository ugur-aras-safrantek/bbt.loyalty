using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Detail;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class TargetApproveFormDto
    {
        public bool isNewRecord { get; set; }
        public TargetDto TargetDraft { get; set; }
        public TargetDetailDto TargetDraftDetail { get; set; }
        public TargetDto? Target { get; set; }
        public TargetDetailDto? TargetDetail { get; set; }
    }
}
