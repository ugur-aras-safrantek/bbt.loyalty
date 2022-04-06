using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Target.Group
{
    public class TargetGroupUpdateRequest: TargetGroupInsertRequest
    {
        public int Id { get; set; }
    }
}
