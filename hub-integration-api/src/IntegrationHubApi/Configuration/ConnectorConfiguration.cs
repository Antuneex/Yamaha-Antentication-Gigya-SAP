using Microsoft.Extensions.DependencyInjection;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using IntegrationHubApi.ExternalServices.Connector;

namespace IntegrationHubApi.Configuration
{
    /// <summary>
    /// Classe de Injeção de dependencia dos conectores
    /// </summary>
    public static class ConnectorConfiguration
    {

        /// <summary>
        /// Método de extensão de Injeção de dependencia dos conectores
        /// </summary>
        public static void AddConnectorConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IGraphApiConnector, GraphApiConnector>();
            services.AddScoped<IFirebaseApiConnector, FirebaseApiConnector>();
        }
    }
}