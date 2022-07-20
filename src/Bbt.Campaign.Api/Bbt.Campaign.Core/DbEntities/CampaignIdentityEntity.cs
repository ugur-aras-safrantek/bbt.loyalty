using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignIdentityEntity : AuditableEntity
    {
        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }

        
        [MaxLength(11), Required]
        public string Identities { get; set; }


        [ForeignKey("IdentitySubType")]
        public int? IdentitySubTypeId { get; set; }
        public IdentitySubTypeEntity? IdentitySubType { get; set; }
    }
}
