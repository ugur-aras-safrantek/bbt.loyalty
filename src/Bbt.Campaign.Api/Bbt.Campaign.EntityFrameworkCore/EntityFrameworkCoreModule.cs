using Bbt.Campaign.EntityFrameworkCore.Context;
using Bbt.Campaign.EntityFrameworkCore.Repositories;
using Bbt.Campaign.EntityFrameworkCore.UnitOfWork;
using Bbt.Campaign.Shared.Static;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bbt.Campaign.EntityFrameworkCore
{
    public static class EntityFrameworkCoreModule
    {
        public static void Configure(IConfiguration configuration, IServiceCollection services)
        {
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddDbContext<CampaignDbContext>(options => options.UseSqlServer(StaticValues.Campaign_MsSql_ConStr), ServiceLifetime.Transient);
        }
    }
}
