﻿namespace Bbt.Campaign.Public.Dtos.CampaignRule
{
    public class CampaignRuleUpdateFormDto : CampaignRuleInsertFormDto
    {

        public bool IsInvisibleCampaign { get; set; }

        public bool IsUpdatableCampaign { get; set; }
        public CampaignRuleDto CampaignRule { get; set; }
    }
}
