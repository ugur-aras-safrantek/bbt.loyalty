using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class UserCredentialEntity : AuditableEntity
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey("CredentialType")]
        public int CredentialTypeId { get; set; }
        public CredentialTypeEntity CredentialType { get; set; }
    }
}
