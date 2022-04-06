using Bbt.Campaign.Core.BaseEntities;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.EntityFrameworkCore.Initializer;
using Microsoft.EntityFrameworkCore;

namespace Bbt.Campaign.EntityFrameworkCore.Context
{
    public partial class CampaignDbContext : DbContext
    {
        public CampaignDbContext()
        {
        }

        public CampaignDbContext(DbContextOptions<CampaignDbContext> options)
            : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, 
                //you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            OnModelCreatingPartial(modelBuilder);
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            new CampaignDbInitializer(modelBuilder).Seed();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public bool HasChanges => ChangeTracker.HasChanges();
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "1";
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = DateTime.UtcNow; 
                        entry.Entity.LastModifiedBy = "2";
                        break;

                }
            }

            //return _authenticatedUser.UserId == null
            //    ? await base.SaveChangesAsync(cancellationToken)
            //    : await base.SaveChangesAsync(_authenticatedUser.UserId);
            return await base.SaveChangesAsync(cancellationToken);
        }

        
        
        public DbSet<ActionOptionEntity> ActionOptions { get; set; }
        //public DbSet<BranchEntity> Branches { get; set; }
        public DbSet<BusinessLineEntity> BusinessLines { get; set; }

        //Campaign     
        public DbSet<CampaignEntity> Campaigns { get; set; }
        public DbSet<CampaignDetailEntity> CampaignDetails { get; set; }
        public DbSet<CampaignRuleEntity>  CampaignRules { get; set; }
        public DbSet<CampaignRuleIdentityEntity>  CampaignRuleIdentities { get; set; }
        public DbSet<CampaignRuleCustomerTypeEntity>  CampaignRuleCustomerTypes { get; set; }
        public DbSet<CampaignRuleBusinessLineEntity>  CampaignRuleBusinesses { get; set; }
        public DbSet<CampaignRuleBranchEntity>  CampaignRuleBranches { get; set; }      
        public DbSet<CampaignAchievementEntity> CampaignAchievements { get; set; }
        public DbSet<CampaignAchievementChannelCodeEntity> CampaignAchievementChannelCodes { get; set; }
        public DbSet<CampaignTargetEntity> CampaignTargets { get; set; }
        public DbSet<CampaignTopLimitEntity> CampaignTopLimits { get; set; }
        public DbSet<CampaignDocumentEntity> CampaignDocuments { get; set; }
        public DbSet<CampaignStartTermEntity> CampaignStartTerms { get; set; }
        public DbSet<CustomerTypeEntity> CustomerTypes { get; set; }
        public DbSet<JoinTypeEntity> JoinTypes { get; set; }
        public DbSet<LanguageEntity> Languages { get; set; }
        public DbSet<SectorEntity> Sectors { get; set; }
        public DbSet<ViewOptionEntity> ViewOptions { get; set; }
        public DbSet<ProgramTypeEntity>  ProgramTypes { get; set; }
        public DbSet<AchievementFrequencyEntity> AchievementFrequencies { get; set; }    
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<TargetDefinitionEntity> TargetDefinitions { get; set; }
        public DbSet<TargetOperationEntity> TargetOperations { get; set; }
        public DbSet<TargetEntity> Targets { get; set; }
        public DbSet<TargetSourceEntity> TargetSources { get; set; }
        public DbSet<TargetDetailEntity> TargetDetails { get; set; }
        public DbSet<TargetViewTypeEntity> TargetViewTypes { get; set; }
        public DbSet<TriggerTimeEntity> TriggerTimes { get; set; }
        public DbSet<VerificationTimeEntity> VerificationTimes { get; set; }
        public DbSet<AchievementTypeEntity> AchievementTypes { get; set; }
        public DbSet<TargetGroupEntity> TargetGroups { get; set; }
        public DbSet<TargetGroupLineEntity> TargetGroupLines { get; set; }
        public DbSet<TopLimitEntity> TopLimits { get; set; }
    }
}
