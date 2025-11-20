using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CliAsistan.Services;

namespace CliAsistan.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration
            services.AddSingleton(configuration);
            
            // AppSettings binding
            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton(appSettings);

            // Services
            services.AddSingleton<AIService>();
            services.AddSingleton<AgentService>();
            services.AddSingleton<MenuHandler>();

            return services;
        }
    }
}
