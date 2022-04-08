using Bbt.Campaign.Public.Dtos.Target.TriggerTime;
using Bbt.Campaign.Public.Dtos.Target.VerificationTime;
using Bbt.Campaign.Public.Dtos.Target.ViewType;

namespace Bbt.Campaign.Public.Models.Target.Detail
{
    public class TargetDetailInsertRequest
    {
        public int TargetId { get; set; }
        public int TargetSourceId { get; set; }
        public int TargetViewTypeId { get; set; }
        public int? TriggerTimeId { get; set; }
        public int? VerificationTimeId { get; set; }

        //akışa özel alanlar
        public string? FlowName { get; set; }
        public decimal? TotalAmount { get; set; } //Toplam Tutar 1.000,00
        public int? NumberOfTransaction { get; set; }//8 İşlem Adedi

        public string? FlowFrequency { get; set; }//Akış Frekansı cron format
        public string? AdditionalFlowTime { get; set; } //Timespan formatındadır

        //sorguya özel alanlar
        public string? Query { get; set; }
        public string? Condition { get; set; }

        //ortak alanlar
        public string? TargetDetailEn { get; set; }
        public string? TargetDetailTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionTr { get; set; }





        
           

    }
}
