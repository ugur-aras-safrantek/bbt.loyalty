using Bbt.Campaign.Core.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.ListData
{
    public static class CampaignTargetListData
    {
        public static List<CampaignTargetListEntity> GetCampaignTargetList(IQueryable<CampaignTargetListEntity> campaignTargetQuery) 
        { 
            return campaignTargetQuery.Select(x => new CampaignTargetListEntity
            {
                Id = x.Id,
                CampaignId = x.CampaignId,
                TargetId = x.TargetId,
                TargetGroupId = x.TargetGroupId,
                TargetOperationId = x.TargetOperationId,
                IsDeleted = x.IsDeleted,
                Name = x.Name,
                Title = x.Title,
                TargetSourceId = x.TargetSourceId,
                TargetViewTypeId = x.TargetViewTypeId,
                TriggerTimeId = x.TriggerTimeId,
                VerificationTimeId = x.VerificationTimeId,
                FlowName = x.FlowName,
                TotalAmount = x.TotalAmount,
                NumberOfTransaction = x.NumberOfTransaction,
                Query = x.Query,
                Condition = x.Condition,
                DescriptionTr = x.DescriptionTr,
                DescriptionEn = x.DescriptionEn,
                TargetDetailTr= x.TargetDetailTr,
                TargetDetailEn = x.TargetDetailEn,
            }).ToList();

        }
    }
}
