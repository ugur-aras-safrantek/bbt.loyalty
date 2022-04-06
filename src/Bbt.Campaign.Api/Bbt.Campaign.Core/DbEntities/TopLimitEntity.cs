using Bbt.Campaign.Core.BaseEntities;
using Bbt.Campaign.Public.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class TopLimitEntity : AuditableEntity
    {
        [MaxLength(250), Required]
        public string Name { get; set; } //adı

        public bool IsActive { get; set; }

        [ForeignKey("AchievementFrequency")]  //Kazanım Sıklığı
        public int AchievementFrequencyId { get; set; }
        public AchievementFrequencyEntity AchievementFrequency { get; set; }
        public TopLimitType Type { get; set; }

        //tutar
        public decimal? MaxTopLimitAmount { get; set; } = 0;  //Çatı Max Tutar

        [ForeignKey("Currency")]   //Para Birimi
        public int? CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }


        //oran

        public decimal? MaxTopLimitRate { get; set; }  //çatı oranı

        //genel

        [MaxLength(250)]
        public decimal? MaxTopLimitUtilization { get; set; }  //Çatı Max Yararlanma


        //Approve
        public bool IsApproved { get; set; }
        public bool IsDraft { get; set; }
        public int? RefId { get; set; }

        public virtual ICollection<CampaignTopLimitEntity> TopLimitCampaigns { get; set; }
    }
}
