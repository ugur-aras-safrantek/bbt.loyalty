using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("CampaignIdentityListView")]
    public class CampaignIdentityListEntity
    {
        [Key]
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? IdentitySubTypeId { get; set; }
        public string? IdentitySubTypeName { get; set; }
        public string Identities { get; set; }
    }
}
