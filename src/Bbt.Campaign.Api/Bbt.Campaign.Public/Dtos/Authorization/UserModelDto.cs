using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserModelDto
    {
        public string Tckn { get; set; }
        public List<string> Credentials { get; set; }
    }

    public class Credentials
    {
        public bool IsLoyaltyCreator { get; set; } 
        public bool IsLoyaltyApprover { get; set; } 
        public bool IsLoyaltyReader { get; set; } 
        public bool IsLoyaltyRuleCreator { get; set; } 
        public bool IsLoyaltyRuleApprover { get; set; } 
    }

    public class UserModelDto2 
    {
        public string Tckn { get; set; }

        public Credentials Credentials { get; set; }

    }
}
