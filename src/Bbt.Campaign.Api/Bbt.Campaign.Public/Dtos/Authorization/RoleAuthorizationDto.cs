using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class RoleAuthorizationDto
    {
        public int RoleTypeId { get; set; }
        public int ModuleTypeId { get; set; }
        public int ProcessTypeId { get; set; }
    }
}
