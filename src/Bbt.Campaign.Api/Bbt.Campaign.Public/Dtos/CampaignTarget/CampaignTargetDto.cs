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

        public string? TargetAmountStr { get; set; }
        public string? TargetAmountCurrencyCode { get; set; }
        public string? RemainAmountStr { get; set; }
        public string? RemainAmountCurrencyCode { get; set; }

        public List<TargetGroupDto> TargetGroupList { get; set; }
    }
}
