using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class AchievementFrequencyEntity : AuditableEntity
    {
        [MaxLength(250), Required]
        public string Name { get; set; }
    }
}
