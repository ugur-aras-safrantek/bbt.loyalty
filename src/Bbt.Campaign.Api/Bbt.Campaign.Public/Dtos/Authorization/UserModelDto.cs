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
        public bool IsLoyaltyCreator { get; set; } = false;
        public bool IsLoyaltyApprover { get; set; } = false;
        public bool IsLoyaltyReader { get; set; } = false;
        public bool IsLoyaltyRuleCreator { get; set; } = false;
        public bool IsLoyaltyRuleApprover { get; set; } = false;
    }

    public class UserModelDto2 
    {
        public UserModelDto2() 
        {
            Credentials = new Credentials();
        }
        public string Tckn { get; set; }

        public Credentials Credentials { get; set; }

    }
}
