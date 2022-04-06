using Bbt.Campaign.EntityFrameworkCore;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bbt.Campaign.Services
{
    public static class ServiceModule
    {
        public static void Configure(IConfiguration configuration, IServiceCollection services)
        {
            IocLoader.UseIocLoader(services);
            EntityFrameworkCoreModule.Configure(configuration, services);
            services.AddAutoMapper(typeof(ServiceModule));
        }
    }
}
