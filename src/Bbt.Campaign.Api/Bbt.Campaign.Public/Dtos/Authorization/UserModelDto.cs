using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserModelDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public AuthorityModel Authority { get; set; }
    }

    public class AuthorityModel
    {
        public bool IsLoyaltyCreator { get; set; } 
        public bool IsLoyaltyApprover { get; set; } 
        public bool IsLoyaltyReader { get; set; } 
        public bool IsLoyaltyRuleCreator { get; set; } 
        public bool IsLoyaltyRuleApprover { get; set; } 
    }

}
