using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target.Detail
{
    public class TargetDetailInsertFormDto
    {
        public List<ParameterDto> TargetSourceList { get; set; }
        public List<ParameterDto> TriggerTimeList { get; set; }
        public List<ParameterDto> TargetViewTypeList { get; set; }
        public List<ParameterDto> VerificationTimeList { get; set; }
    }
}
