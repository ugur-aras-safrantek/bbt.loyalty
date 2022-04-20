using Bbt.Campaign.Public.Dtos.Target.Group;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class CampaignTargetDto
    {
        public CampaignTargetDto() 
        {
            TargetGroupList = new List<TargetGroupDto>();
        }
        public int CampaignId { get; set; }
        public int GroupCount { get; set; }
        public List<TargetGroupDto> TargetGroupList { get; set; }
    }
}
