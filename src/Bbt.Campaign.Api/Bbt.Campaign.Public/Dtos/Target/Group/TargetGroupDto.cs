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
            TargetList = new List<ParameterDto>();
        }

        public int Id { get; set; }
        [MaxLength(250), Required]

        public List<ParameterDto> TargetList { get; set; }

        //public string Name { get; set; }
        //public int TargetId { get; set; }
        //public ParameterDto Target { get; set; }

        ////public ParameterDto TargetGroupLines { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public string? LastModifiedBy { get; set; }
        //public DateTime? LastModifiedOn { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
