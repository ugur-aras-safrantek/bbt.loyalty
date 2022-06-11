using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class TargetUpdateFields
    {
        public bool IsNameUpdated { get; set; } = false;
        public bool IsTitleUpdated { get; set; } = false;
        public bool IsIsActiveUpdated { get; set; } = false;

        public bool IsTargetSourceIdUpdated { get; set; } = false;
        public bool IsTargetViewTypeIdUpdated { get; set; } = false;
        public bool IsTriggerTimeIdUpdated { get; set; } = false;
        public bool IsVerificationTimeIdUpdated { get; set; } = false;
        public bool IsFlowNameUpdated { get; set; } = false;
        public bool IsTargetDetailEnUpdated { get; set; } = false;
        public bool IsTargetDetailTrUpdated { get; set; } = false;
        public bool IsDescriptionEnUpdated { get; set; } = false;
        public bool IsDescriptionTrUpdated { get; set; } = false;
        public bool IsTotalAmountUpdated { get; set; } = false;
        public bool IsNumberOfTransactionUpdated { get; set; } = false;
        public bool IsFlowFrequencyUpdated { get; set; } = false;
        public bool IsAdditionalFlowTimeUpdated { get; set; } = false;
        public bool IsQueryUpdated { get; set; } = false;
        public bool IsConditionUpdated { get; set; } = false;
    }
}
