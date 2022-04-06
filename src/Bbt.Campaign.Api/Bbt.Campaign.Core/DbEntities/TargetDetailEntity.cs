using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class TargetDetailEntity : AuditableEntity
    {
        [ForeignKey("Target")]
        public int TargetId { get; set; }
        public TargetEntity  Target { get; set; }

        [ForeignKey("TargetSource")]     //Hedef kaynağı. “Akış” ve “Sorgu” 
        public int TargetSourceId { get; set; }
        public TargetSourceEntity  TargetSource { get; set; }


        [ForeignKey("TargetViewType")]  //Hedef gösterim tipi. “Progress Bar”, “Bilgi” ve “Görüntülenmeyecek”
        public int TargetViewTypeId { get; set; }
        public TargetViewTypeEntity TargetViewType { get; set; }


        [ForeignKey("TriggerTime")]  // Tetiklenme Zamanı. Akış için zorunludur. “Hedefe Ulaşıldığı Anda” ve “Tamamlandıktan Sonra” 
        public int? TriggerTimeId { get; set; }
        public TriggerTimeEntity? TriggerTime { get; set; }

      
        [ForeignKey("VerificationTime")]  //Kampanya Doğrulama Zamanı. Sorgu için zorunludur. “İlk Kontrol Edildiğinde” ve “Her Kontrol Edildiğinde”  
        public int? VerificationTimeId { get; set; }
        public VerificationTimeEntity? VerificationTime { get; set; }

        //Akış a özel alanlar

        [MaxLength(500)]
        public string? FlowName { get; set; }  // Akış İsmi-zorunlu
        public decimal? TotalAmount { get; set; } //toplam tutar - 	“İşlem Adedi” alanı doldurulmamış ise zorunlu alandır. 
        public int? NumberOfTransaction { get; set; } //işlem adedi - "Toplam Tutar” alanı doldurulmamış ise zorunlu alandır. 

        [MaxLength(500)]      //Dakika 0-59, Saat 0-23, Gün 1-31,  Ay 1-12 ve Hafta 0-7 şeklinde tanımlanmıştır.
        public string? FlowFrequency { get; set; } //Akış frekansı - zorunlu
        public int? AdditionalFlowTime { get; set; } //Akış ek süresi

        //Sorgu ya özel alanlar
        public string? Query { get; set; } //
        public string? Condition { get; set; }  //koşul

        //ortak alanlar
        public string? TargetDetailEn { get; set; }
        public string? TargetDetailTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionTr { get; set; }
    }
}
