namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignListDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ContractId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public string ProgramType { get; set; }

    }
}
