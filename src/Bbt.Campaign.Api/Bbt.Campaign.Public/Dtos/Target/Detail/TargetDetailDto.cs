using Bbt.Campaign.Public.Dtos.Target.Source;
using Bbt.Campaign.Public.Dtos.Target.TriggerTime;
using Bbt.Campaign.Public.Dtos.Target.VerificationTime;
using Bbt.Campaign.Public.Dtos.Target.ViewType;

namespace Bbt.Campaign.Public.Dtos.Target.Detail
{
    public class TargetDetailDto
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public int TargetSourceId { get; set; }
        public ParameterDto  TargetSource { get; set; }
        public int TargetViewTypeId { get; set; }
        public ParameterDto TargetViewType { get; set; }
        public int? TriggerTimeId { get; set; }
        public ParameterDto? TriggerTime { get; set; }     
        public int? VerificationTimeId { get; set; }
        public ParameterDto? VerificationTime { get; set; }
        public string? FlowName { get; set; }
        public string? TargetDetailEn { get; set; }
        public string? TargetDetailTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionTr { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? TotalAmountStr { get; set; }
        public int? NumberOfTransaction { get; set; }
        public string? FlowFrequency { get; set; }
        public string? AdditionalFlowTime { get; set; }
        public string? Query { get; set; }
        public string? Condition { get; set; }
    }
}
