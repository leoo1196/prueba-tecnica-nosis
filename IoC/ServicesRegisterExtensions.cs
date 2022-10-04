using Business.Services;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IoC;
public static class ServicesRegisterExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IMovieService, MovieService>();

        return services;
    }
}
