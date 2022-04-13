using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Core.Helper;
using Bbt.Campaign.EntityFrameworkCore.Context;
using Bbt.Campaign.Public.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.EntityFrameworkCore.Initializer
{
    public class CampaignDbInitializer
    {
        private readonly ModelBuilder modelBuilder;

        public CampaignDbInitializer(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

        public void Seed()
        {
            modelBuilder.Entity<BusinessLineEntity>().HasData(Helpers.EnumToObjectList<BusinessLineEnum>());

            modelBuilder.Entity<CustomerTypeEntity>().HasData(Helpers.EnumToObjectList<CustomerTypeEnum>());

            modelBuilder.Entity<AchievementTypeEntity>().HasData(Helpers.EnumToObjectList<AchievementTypeEnum>());

            modelBuilder.Entity<ActionOptionEntity>().HasData(Helpers.EnumToObjectList<ActionOptionsEnum>().Select(x => new ActionOptionEntity
            {
                Code = x.Id.ToString(),
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                Id = x.Id,
                IsDeleted = x.IsDeleted,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedOn = x.LastModifiedOn,
                Name = x.Name
            }).ToList());

            //modelBuilder.Entity<CampaignChannelEntity>().HasData(
            //    new CampaignChannelEntity() { Id = 1, Name = "Tümü", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 2, Name = "Batch", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 3, Name = "Bayi", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 4, Name = "Diğer", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 5, Name = "İnternet", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 6, Name = "Ptt", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 7, Name = "Remote", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 8, Name = "Sms", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 9, Name = "Şube", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 10, Name = "Tablet", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 11, Name = "Web", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 12, Name = "Web Bayi", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
            //    new CampaignChannelEntity() { Id = 13, Name = "Web Mevduat", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            //);

//            modelBuilder.Entity<BranchEntity>().HasData(
//    new BranchEntity() { Id = 1, Code = "9530", Name = "Merkez", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
//    new BranchEntity() { Id = 2, Code = "9531", Name = "Çamlıca Şubesi", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
//);


            //    modelBuilder.Entity<CustomerTypeEntity>().HasData(
            //    new CustomerTypeEntity() { Id = 1, Name = "Gerçek", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
            //    new CustomerTypeEntity() { Id = 2, Name = "Tüzel", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
            //    new CustomerTypeEntity() { Id = 3, Name = "Ortak", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
            //    new CustomerTypeEntity() { Id = 4, Name = "Reşit Olmayan", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
            //    new CustomerTypeEntity() { Id = 5, Name = "Adi Ortaklık", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            //);

            modelBuilder.Entity<LanguageEntity>().HasData(
                new LanguageEntity() { Id = 1, Code = "TR", Name = "Türkçe", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new LanguageEntity() { Id = 2, Code = "EN", Name = "İngilizce", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<SectorEntity>().HasData(
                new SectorEntity() { Id = 1, Code = "Akr", Name = "Akaryakıt", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new SectorEntity() { Id = 2, Code = "Chl", Name = "Giyim", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new SectorEntity() { Id = 3, Code = "Edu", Name = "Eğitim", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<ViewOptionEntity>().HasData(
                new ViewOptionEntity() { Id = 1, Code = "SK", Name = "Sürekli Kampanyalar", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new ViewOptionEntity() { Id = 2, Code = "DK", Name = "Dönemsel Kampanyalar", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new ViewOptionEntity() { Id = 3, Code = "AK", Name = "Genel Kampanyalar", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new ViewOptionEntity() { Id = 4, Code = "NG", Name = "Görüntülenmeyecek", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            //modelBuilder.Entity<ActionOptionEntity>().HasData(
            //    new ActionOptionEntity() { Id = 1, Code = "SK", Name = "Ödeme Cashback", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            //    new ActionOptionEntity() { Id = 2, Code = "SK", Name = "Fatura Cashback", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            //);

            modelBuilder.Entity<JoinTypeEntity>().HasData(
                new JoinTypeEntity() { Id = 1, Code = "SK", Name = "Tüm Müşteriler", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new JoinTypeEntity() { Id = 2, Code = "SK", Name = "Müşteri Özelinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new JoinTypeEntity() { Id = 3, Code = "SK", Name = "İş Kolu Özelinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new JoinTypeEntity() { Id = 4, Code = "SK", Name = "Şube Özelinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new JoinTypeEntity() { Id = 5, Code = "SK", Name = "Müşteri Tipi Özelinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );


            modelBuilder.Entity<CampaignStartTermEntity>().HasData(
                new CampaignStartTermEntity() { Id = 1, Name = "Katılım Anında", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new CampaignStartTermEntity() { Id = 2, Name = "Dönem Başlangıcı", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            );

            modelBuilder.Entity<TargetOperationEntity>().HasData(
                new TargetOperationEntity() { Id = 1, Name = "ve", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new TargetOperationEntity() { Id = 2, Name = "veya", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new TargetOperationEntity() { Id = 3, Name = "kesişim", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new TargetOperationEntity() { Id = 4, Name = "fark", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );



            modelBuilder.Entity<AchievementFrequencyEntity>().HasData(
                new AchievementFrequencyEntity() { Id = 1, Name = "Anlık", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new AchievementFrequencyEntity() { Id = 2, Name = "Aylık", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new AchievementFrequencyEntity() { Id = 3, Name = "Yıllık", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            );

            modelBuilder.Entity<CurrencyEntity>().HasData(
                new CurrencyEntity() { Id = 1, Name = "TRY", Code = "TRY", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new CurrencyEntity() { Id = 2, Name = "GBP", Code = "GBP", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new CurrencyEntity() { Id = 3, Name = "EUR", Code = "EUR", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false },
                new CurrencyEntity() { Id = 4, Name = "USD", Code = "USD", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false }
            );

            modelBuilder.Entity<ProgramTypeEntity>().HasData(
                new ViewOptionEntity() { Id = 1, Code = "SK", Name = "Sadakat", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new ViewOptionEntity() { Id = 2, Code = "GK", Name = "Kampanya", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
                new ViewOptionEntity() { Id = 3, Code = "AK", Name = "Kazanım", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<TargetViewTypeEntity>().HasData(
               new TargetViewTypeEntity() { Id = 1, Name = "Progress Bar", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new TargetViewTypeEntity() { Id = 2, Name = "Bilgi", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new TargetViewTypeEntity() { Id = 3, Name = "Görüntülenmeyecek", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<TriggerTimeEntity>().HasData(
               new TriggerTimeEntity() { Id = 1, Name = "Hedefe Ulaşıldığı Anda", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new TriggerTimeEntity() { Id = 2, Name = "Tamamlandıktan Sonra", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<VerificationTimeEntity>().HasData(
               new VerificationTimeEntity() { Id = 1, Name = "İlk Kontrol Edildiğinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new VerificationTimeEntity() { Id = 2, Name = "Her Kontrol Edildiğinde", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<TargetSourceEntity>().HasData(
               new TargetSourceEntity() { Id = 1, Name = "Akış", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new TargetSourceEntity() { Id = 2, Name = "Sorgu", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

            modelBuilder.Entity<ParticipationTypeEntity>().HasData(
               new ParticipationTypeEntity() { Id = 1, Name = "Otomatik Katılım", Code = "1", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new ParticipationTypeEntity() { Id = 2, Name = "Müşteri Seçimi", Code = "2", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, },
               new ParticipationTypeEntity() { Id = 3, Name = "Operatör Seçimi", Code = "3", CreatedBy = "1", CreatedOn = DateTime.Now, IsDeleted = false, }
            );

        }
    }
}
