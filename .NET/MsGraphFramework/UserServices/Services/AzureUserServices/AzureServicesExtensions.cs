using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsGraph.Classes;

namespace UserServices.Services.AzureUserServices
{
    public static class AzureServicesExtensions
    {
        public static void AddAzureUserServices(this IServiceCollection services, IConfiguration configuration, string graphSectionName = "Graph")
        {
            try
            {
                var graphConfig = configuration.GetSection(graphSectionName).Get<GraphConfig>();
                GraphConfig.The = graphConfig;
                services.AddSingleton(graphConfig);
                services.AddSingleton<GraphJson>();
                services.AddSingleton<GraphClient>();
                services.AddSingleton<IPersistentUserStorageProvider, AzureB2CUserStorageProvider>();
                services.AddSingleton<IVolatileUserStorageProvider, UserCache>(); 
                services.AddHostedService<UserCacheInitialLoader>();
                services.AddScoped<IUserService, AzureB2CUserService>();
            }
            catch (Exception e)
            {
                throw new Exception($"Error: {e.Message}. Make sure 'Graph' node is present in appsettings.json");
            }
        }
    }
}
