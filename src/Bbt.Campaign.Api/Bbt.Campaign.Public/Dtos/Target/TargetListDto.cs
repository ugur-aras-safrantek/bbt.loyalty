using Bbt.Campaign.Public.Dtos.Target.Detail;

namespace Bbt.Campaign.Public.Dtos.Target
{
    public class TargetListDto
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string TargetViewType { get; set; }
        public bool Flow { get; set; }
        public bool Query { get; set; }
        public TargetDetailDto TargetDetail  { get; set; }

    }
}
