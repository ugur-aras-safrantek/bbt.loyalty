using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class GoalResultByCustomerAndCampaing
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }

        public Detail Detail { get; set; }
        public string Title { get; set; }
        public int TargetGroupId { get; set; }
    }

    public class Detail 
    {
        public string FlowName { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? NumberOfTransaction { get; set; }
        public int TargetId { get; set; }
        public bool IsDeleted { get; set; }
        public int TargetViewTypeId { get; set; }
        public string Description { get; set; }
        public StreamResult StreamResult { get; set; }

    }

    public class StreamResult 
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string StreamName { get; set; }
        public string Currency { get; set; }
        public string FlowName { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public decimal? Amount { get; set; }
        public int? Times { get; set; }
    }
}
