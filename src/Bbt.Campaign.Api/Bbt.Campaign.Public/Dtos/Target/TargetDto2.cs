using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Target
{
    public class TargetDto2
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int StatusId { get; set; }
    }
}
