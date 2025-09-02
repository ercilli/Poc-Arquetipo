using BgbaArquetipoCanales.Middleware;
using BgbaArquetipoCanales.Enrichers;
using BgbaArquetipoHttp;
using Logging.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BgbaArquetipoCanales.Extensions
{
    /// <summary>
    /// Extension methods for integrating canal logging with dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds canal logging services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddCanalLogging(this IServiceCollection services, LoggingConfiguration? configuration = null)
        {
            configuration ??= new LoggingConfiguration();

            // Create filter based on configuration
            var logFilter = new DefaultLogFilter(configuration);

            // Create base logger if not already registered
            if (!services.Any(s => s.ServiceType == typeof(ILogger)))
            {
                var baseLogger = new Logging.Core.Logger();
                baseLogger.AddWriter(new ConsoleLogWriter(configuration.WriteIndented, logFilter, configuration));
                services.AddSingleton<ILogger>(baseLogger);
                services.AddSingleton(configuration);
                services.AddSingleton<ILogFilter>(logFilter);
            }

            // Add canal-specific logger (for backward compatibility)
            services.AddSingleton<CanalLogger>();

            return services;
        }

        /// <summary>
        /// Adds canal enrichment to HTTP logging instead of separate middleware
        /// </summary>
        public static IServiceCollection AddCanalEnrichment(this IServiceCollection services)
        {
            // Register the canal HTTP log enricher
            services.AddSingleton<IHttpLogEnricher, CanalHttpLogEnricher>();

            return services;
        }

        /// <summary>
        /// Adds canal logging middleware to the application pipeline (DEPRECATED - use AddCanalEnrichment instead)
        /// </summary>
        [Obsolete("Use AddCanalEnrichment() instead to enrich HTTP logs rather than creating separate canal logs")]
        public static IApplicationBuilder UseCanalLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CanalLoggingMiddleware>();
        }
    }
}
