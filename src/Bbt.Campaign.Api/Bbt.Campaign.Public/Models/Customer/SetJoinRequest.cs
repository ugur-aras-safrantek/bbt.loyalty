
namespace Bbt.Campaign.Public.Models.Customer
{
    public class SetJoinRequest
    {
        public string CustomerCode { get; set; }
        public int CampaignId { get; set; }
        public bool IsJoin { get; set; }
    }
}
