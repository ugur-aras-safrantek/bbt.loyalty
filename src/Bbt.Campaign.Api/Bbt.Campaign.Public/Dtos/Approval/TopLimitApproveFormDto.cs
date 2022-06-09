using Bbt.Campaign.Public.Dtos.CampaignTopLimit;


namespace Bbt.Campaign.Public.Dtos.Approval
{
    public  class TopLimitApproveFormDto
    {
        public bool isNewRecord { get; set; }
        public TopLimitDto TopLimit { get; set; }
        public TopLimitUpdateFields TopLimitUpdateFields { get; set; }
    }
}
