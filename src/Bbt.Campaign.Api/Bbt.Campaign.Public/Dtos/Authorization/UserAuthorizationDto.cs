using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserAuthorizationDto
    {
        public UserAuthorizationDto() 
        {
            ProcessList = new List<int>();
        }
        public int ModuleId { get; set; }
        public List<int> ProcessList { get; set; }
    }
}
