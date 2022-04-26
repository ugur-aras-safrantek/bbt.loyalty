using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target
{
    public class TargetParameterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public decimal? UsedAmount { get; set; }
        public string? UsedAmountStr { get; set; }
        public int? UsedNumberOfTransaction { get; set; }
        public int? Percent { get; set; }
    }
}
