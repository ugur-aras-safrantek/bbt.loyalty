using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target.Group
{
    public class TargetGroupUpdateFormDto : TargetGroupInsertFormDto
    {
        public TargetGroupDto TargetGroup { get; set; }
    }
}
