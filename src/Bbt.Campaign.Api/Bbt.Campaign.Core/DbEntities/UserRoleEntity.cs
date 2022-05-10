using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class UserRoleEntity 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("RoleType")]
        public int RoleTypeId { get; set; }
        public RoleTypeEntity RoleType { get; set; }
        public DateTime LastProcessDate { get; set; }
    }
}
