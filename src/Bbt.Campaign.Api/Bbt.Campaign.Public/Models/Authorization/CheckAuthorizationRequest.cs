using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Authorization
{
    public class CheckAuthorizationRequest
    {
        public string UserId { get; set; }
        public int ModuleTypeId { get; set; }
        public int AuthorizationTypeId { get; set; }
    }
}
