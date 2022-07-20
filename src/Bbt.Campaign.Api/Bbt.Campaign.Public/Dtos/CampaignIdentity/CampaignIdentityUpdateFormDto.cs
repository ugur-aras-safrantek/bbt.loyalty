using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignIdentity
{
    public class CampaignIdentityUpdateFormDto
    {
        public List<ParameterDto> CampaignList { get; set; }
        public List<ParameterDto> IdentitySubTypeList { get; set; }
    }
}
