namespace InfoTrack.Api.Configuration;

/// <summary>
/// Static class to add api configuration
/// </summary>
public static class ApiConfig
{
    /// <summary>
    /// Adds API configuration services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection with added API services.</returns>
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
        });
        return services;
    }

    /// <summary>
    /// Configures the application pipeline for API requests.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <returns>The configured IApplicationBuilder.</returns>
    public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
