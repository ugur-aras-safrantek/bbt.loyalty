using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserRoleDto2
    {
        public string UserId { get; set; }
        public List<int> RoleTypeIdList { get; set; }
    }
}
