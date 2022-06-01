using Bbt.Campaign.Public.Dtos.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class CampaignTargetDto2
    {
        public bool IsAchieved { get; set; }
        public List<TargetParameterDto> ProgressBarlist { get; set; }
        public List<TargetParameterDto> Informationlist { get; set; }
    }
}
