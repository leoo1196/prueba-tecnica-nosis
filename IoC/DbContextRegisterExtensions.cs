using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC;
public static class DbContextRegisterExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(configuration.GetConnectionString("NosisTestDb")));
    }
}

