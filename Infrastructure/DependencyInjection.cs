using Domain.Routes;
using Domain.StationGraphNodes;
using Domain.StationGraphs;
using Infrastructure.Repositories.Routes;
using Infrastructure.Repositories.StationGraphNodes;
using Infrastructure.Repositories.StationGraphs;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IStationGraphRepository, OneTimeStationGraphRepository>();
        services.AddScoped<IStationGraphNodeRepository, OneTimeStationGraphNodeRepository>();
        services.AddScoped<IRouteRepository, OneTimeRouteRepository>();

        return services;
    }
}
