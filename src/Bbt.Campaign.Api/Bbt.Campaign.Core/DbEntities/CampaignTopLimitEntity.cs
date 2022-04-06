using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignTopLimitEntity : AuditableEntity
    {
        [ForeignKey("CampaignTopLimit")]
        public int TopLimitId { get; set; }
        public TopLimitEntity TopLimit { get; set; }

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }


    }
}
