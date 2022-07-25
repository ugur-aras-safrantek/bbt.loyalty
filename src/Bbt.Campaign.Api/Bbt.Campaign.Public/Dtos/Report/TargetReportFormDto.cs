using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class TargetReportFormDto
    {
        public List<ParameterDto> CampaignList { get; set; }
        public List<ParameterDto> IdentitySubTypeList { get; set; }
        public List<ParameterDto> TargetList { get; set; }
    }
}
