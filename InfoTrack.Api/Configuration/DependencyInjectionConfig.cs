using InfoTrack.Contracts.Interfaces;
using InfoTrack.Business.Services;
using InfoTrack.Data.Repositories;

namespace InfoTrack.Api.Configuration;

/// <summary>
/// Static class to register services using dependency injection
/// </summary>
public static class DependencyInjectionConfig
{
    /// <summary>
    /// Registers services required by the API in the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ISettlementService, SettlementService>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
    }
}