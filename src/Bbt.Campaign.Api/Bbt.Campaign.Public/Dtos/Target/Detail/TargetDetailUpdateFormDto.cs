using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target.Detail
{
    public class TargetDetailUpdateFormDto : TargetDetailInsertFormDto
    {
        public TargetDetailDto TargetDetail { get; set; }
    }
}
