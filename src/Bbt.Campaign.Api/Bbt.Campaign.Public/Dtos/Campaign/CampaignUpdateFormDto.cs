using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignUpdateFormDto : CampaignInsertFormDto
    {
        public CampaignDto Campaign { get; set; }

        public GetFileResponse ContractFile { get; set; }
    }
}
