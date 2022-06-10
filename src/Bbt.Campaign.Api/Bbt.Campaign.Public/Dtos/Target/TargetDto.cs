using Bbt.Campaign.Public.Dtos.Target.Detail;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Public.Dtos.Target
{
    public class TargetDto
    {
        public int Id { get; set; }
        public string Code { get; set; }

        [MaxLength(250), Required]
        public string Name { get; set; }
        [MaxLength(250), Required]
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int StatusId { get; set; }

        public TargetDetailDto? TargetDetail { get; set; }
    }
}
