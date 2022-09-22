using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("OnExtraDefinition", Schema = "str")]
    public class OnExtraDefinitionEntity
    {
        [Key]
        public Guid Id { get; set; }
        public int PendingUnconditionalMounth { get; set; }
        public decimal FirstMounthConditional { get; set; }
        public decimal SupportSpendingAmount { get; set; }
        public decimal CampaignJoinFirstMounthTarget { get; set; }
        public int OnExtraCampaignId { get; set; }
    }
}
