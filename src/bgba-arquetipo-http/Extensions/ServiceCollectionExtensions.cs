using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using BgbaArquetipoHttp.Middleware;
using BgbaArquetipoHttp.Interceptors;
using Logging.Core;
using System;

namespace BgbaArquetipoHttp.Extensions
{
    /// <summary>
    /// Extension methods for configuring HTTP logging
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds HTTP logging services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddHttpLogging(this IServiceCollection services, LoggingConfiguration? config = null)
        {
            config ??= new LoggingConfiguration();

            // Create filter based on configuration
            var logFilter = new DefaultLogFilter(config);

            // Create enriched logger if enrichers are available
            services.AddSingleton<IHttpLogger>(provider =>
            {
                var enrichers = provider.GetServices<IHttpLogEnricher>();
                var httpContextAccessor = provider.GetService<IHttpContextAccessor>();

                if (enrichers.Any() && httpContextAccessor != null)
                {
                    return new HttpLoggerWithEnrichment(
                        new ConsoleLogWriter(config.WriteIndented, logFilter, config),
                        config,
                        enrichers,
                        httpContextAccessor);
                }
                else
                {
                    var logger = new HttpLogger();
                    logger.AddWriter(new ConsoleLogWriter(config.WriteIndented, logFilter, config));
                    return logger;
                }
            });

            services.AddSingleton(config);
            services.AddSingleton<ILogFilter>(logFilter);

            // Add HttpContextAccessor for transparent logging
            services.AddHttpContextAccessor();

            // Configure the static provider for simplified extensions
            services.AddSingleton<IHttpContextAccessor>(provider =>
            {
                var accessor = new HttpContextAccessor();
                HttpContextAccessorProvider.Current = accessor;
                return accessor;
            });

            // Configure HttpClient with logging handler
            services.AddHttpClient("LoggedHttpClient")
                .AddHttpMessageHandler<HttpLoggingHandler>();

            services.AddTransient<HttpLoggingHandler>();

            return services;
        }

        /// <summary>
        /// Adds enriched HTTP logging that automatically captures context from Microsoft ILogger calls
        /// </summary>
        public static IServiceCollection AddEnrichedHttpLogging(this IServiceCollection services, LoggingConfiguration? config = null)
        {
            // Add base HTTP logging
            services.AddHttpLogging(config);

            // Replace the default logger with our enriched version - SINGLETON para evitar conflictos de DI
            services.AddSingleton(typeof(ILogger<>), typeof(EnrichedHttpLogger<>));

            return services;
        }        /// <summary>
                 /// Adds HTTP logging services with a custom logger configuration
                 /// </summary>
        public static IServiceCollection AddHttpLogging(this IServiceCollection services, Action<HttpLogger> configure, LoggingConfiguration? config = null)
        {
            config ??= new LoggingConfiguration();

            // Create filter based on configuration
            var logFilter = new DefaultLogFilter(config);

            var logger = new HttpLogger();
            logger.AddWriter(new ConsoleLogWriter(config.WriteIndented, logFilter));
            configure(logger);

            services.AddSingleton<IHttpLogger>(logger);
            services.AddSingleton(config);
            services.AddSingleton<ILogFilter>(logFilter);

            // Configure HttpClient with logging handler
            services.AddHttpClient("LoggedHttpClient")
                .AddHttpMessageHandler<HttpLoggingHandler>();

            services.AddTransient<HttpLoggingHandler>();

            return services;
        }
    }

    /// <summary>
    /// Extension methods for configuring HTTP logging middleware
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds HTTP logging middleware to the request pipeline
        /// </summary>
        public static IApplicationBuilder UseBgbaHttpLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpLoggingMiddleware>();
        }
    }
}
