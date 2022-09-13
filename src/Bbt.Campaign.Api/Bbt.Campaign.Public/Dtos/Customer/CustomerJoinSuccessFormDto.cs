
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignDetail;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerJoinSuccessFormDto
    {
        public CampaignMinDto Campaign { get; set; }
        public string smsMessage { get; set; }
    }
}
