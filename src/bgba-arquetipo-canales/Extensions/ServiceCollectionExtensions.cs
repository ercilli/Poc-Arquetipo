using BgbaArquetipoCanales.Middleware;
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

            // Add canal-specific logger
            services.AddSingleton<CanalLogger>();

            return services;
        }

        /// <summary>
        /// Adds canal logging middleware to the application pipeline
        /// </summary>
        public static IApplicationBuilder UseCanalLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CanalLoggingMiddleware>();
        }
    }
}
