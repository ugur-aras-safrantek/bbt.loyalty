using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Authorization
{
    public class UserAuthorizationResponseDto
    {
        public string AccessToken { get; set; }
        public List<UserAuthorizationDto> AuthorizationList { get; set; }
    }
}
