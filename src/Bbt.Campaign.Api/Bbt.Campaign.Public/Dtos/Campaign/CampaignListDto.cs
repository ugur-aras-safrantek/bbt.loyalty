namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignListDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ContractId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public string ProgramType { get; set; }

    }
}
