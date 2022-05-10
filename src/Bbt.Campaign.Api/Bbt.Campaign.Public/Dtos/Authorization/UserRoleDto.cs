using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleTypeId { get; set; }
        public DateTime LastProcessDate { get; set; }
    }
}
