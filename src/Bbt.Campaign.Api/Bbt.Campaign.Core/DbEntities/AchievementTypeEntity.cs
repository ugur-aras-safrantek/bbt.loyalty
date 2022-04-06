using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class AchievementTypeEntity : AuditableEntity
    {
        [MaxLength(250), Required]
        public string Name { get; set; }

    }
}
