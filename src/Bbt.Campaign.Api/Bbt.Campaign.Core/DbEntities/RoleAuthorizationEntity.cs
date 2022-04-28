using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class RoleAuthorizationEntity : AuditableEntity
    {
        [ForeignKey("RoleType")]
        public int RoleTypeId { get; set; }
        public RoleTypeEntity RoleType { get; set; }

        [ForeignKey("ModuleType")]
        public int ModuleTypeId { get; set; }
        public ModuleTypeEntity ModuleType { get; set; }

        [ForeignKey("ProcessType")]
        public int ProcessTypeId { get; set; }
        public ProcessTypeEntity ProcessType { get; set; }

    }
}
