using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target.Group
{
    public class TargetGroupDto
    {
        public TargetGroupDto() 
        {
            TargetList = new List<TargetParameterDto>();
        }
        public int Id { get; set; }
        public decimal? TargetAmount { get; set; }
        public string? TargetAmountStr { get; set; }
        public decimal? RemainAmount { get; set; }
        public string? RemainAmountStr { get; set; }
        public int? TargetNumberOfTransaction { get; set; }        
        public List<TargetParameterDto> TargetList { get; set; }
    }
}
