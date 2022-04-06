using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Target.Group
{
    public class TargetGroupInsertRequest
    {
        public string Name { get; set; }
        public List<int> TargetList { get; set; }
    }
}
