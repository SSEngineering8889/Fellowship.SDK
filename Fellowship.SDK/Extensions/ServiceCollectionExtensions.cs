using Fellowship.SDK.Api;
using Fellowship.SDK.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fellowship.SDK.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Fellowship SDK services to the DI container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFellowshipSdk(this IServiceCollection services, Action<FellowshipSdkOptions> configure)
    {
        var options = new FellowshipSdkOptions();
        configure(options);

        // Register concrete implementations with API key
        services.AddSingleton<IMoviesClient>(provider =>
        {
            var logger = provider.GetService<ILogger<MoviesClient>>() ?? 
                        Microsoft.Extensions.Logging.Abstractions.NullLogger<MoviesClient>.Instance;
            return new MoviesClient(options.ApiKey, logger);
        });

        services.AddSingleton<IQuotesClient>(provider =>
        {
            var logger = provider.GetService<ILogger<QuotesClient>>() ?? 
                        Microsoft.Extensions.Logging.Abstractions.NullLogger<QuotesClient>.Instance;
            return new QuotesClient(options.ApiKey, logger);
        });

        // Register interfaces
        //services.AddSingleton(provider => provider.GetRequiredService<MoviesClient>());
        //services.AddSingleton(provider => provider.GetRequiredService<QuotesClient>());
        services.AddSingleton<IFellowshipClient, FellowshipClient>();

        return services;
    }

    /// <summary>
    /// Adds Fellowship SDK services to the DI container with API key
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="apiKey">The One API key</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFellowshipSdk(this IServiceCollection services, string apiKey)
    {
        return services.AddFellowshipSdk(options => options.ApiKey = apiKey);
    }
}

public class FellowshipSdkOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

