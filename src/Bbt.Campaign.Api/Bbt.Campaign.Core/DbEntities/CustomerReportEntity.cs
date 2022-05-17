
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("CustomerReportView")]
    public class CustomerReportEntity
    {
        [Key]
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerIdentifier { get; set; }
        public DateTime JoinDate { get; set; }
        public string JoinDateStr { get; set; }
        public int CampaignId { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public int IsContinuingCampaign { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public string CampaignStartDateStr { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }



        public int JoinTypeId { get; set; }
        public string? JoinTypeName { get; set; }
        public string? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? CampaignStartTermId { get; set; }
        public string? CampaignStartTermName { get; set; }
        public string? BusinessLineId { get; set; }
        public string? BusinessLineName { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }


        public string? AchievementTypeId { get; set; }
        public string? AchievementTypeName { get; set; }
    }
}
