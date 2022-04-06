using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target.Group
{
    public class TargetGroupLineDto
    {
        public int Id { get; set; }
        
        public int TargetGroupId { get; set; }
        public ParameterDto TargetGroup { get; set; }
        
        public ParameterDto Target { get; set; }
        public int TargetId { get; set; }
    }
}
