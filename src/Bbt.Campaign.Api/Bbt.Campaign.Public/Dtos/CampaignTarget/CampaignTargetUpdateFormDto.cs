namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class CampaignTargetUpdateFormDto : CampaignTargetInsertFormDto
    {
        public bool IsInvisibleCampaign { get; set; }
        public CampaignTargetDto CampaignTargetList { get; set; }
    }
}
