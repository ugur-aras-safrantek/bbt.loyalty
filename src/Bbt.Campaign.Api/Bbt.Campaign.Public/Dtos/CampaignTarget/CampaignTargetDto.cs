using Bbt.Campaign.Public.Dtos.Target.Group;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class CampaignTargetDto
    {
        public CampaignTargetDto() 
        {
            TargetGroupList = new List<TargetGroupDto>();
        }

        //public int Id { get; set; }
        public int CampaignId { get; set; }

        public List<TargetGroupDto> TargetGroupList { get; set; }


        //public int TargetGroupId { get; set; }
        //public int TargetOperationId { get; set; }
        //public int TargetId { get; set; }
        //public ParameterDto Target { get; set; }
    }
}
