using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CampaignReportFormDto
    {
        public List<ParameterDto> SectorList { get; set; }
        public List<ParameterDto> ViewOptionList { get; set; }
        public List<ParameterDto> ActionOptionList { get; set; }
        public List<ParameterDto> ProgramTypeList { get; set; }

        public List<ParameterDto> JoinTypeList { get; set; }

        public List<ParameterDto> AchievementTypes { get; set; }
    }
}
