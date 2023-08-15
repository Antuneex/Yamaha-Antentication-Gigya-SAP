using Microsoft.Extensions.DependencyInjection;
using IntegrationHubApi.Domain.Interfaces.Repositories;
using IntegrationHubApi.Infra.Repositories;

namespace IntegrationHubApi.Configuration
{
    /// <summary>
    /// Classe para Injeção de dependencia dos repositórios
    /// </summary>
    public static class RepositoryConfiguration
    {
        /// <summary>
        /// Método de extensão de Injeção de dependencia dos repositórios
        /// </summary>
        public static void AddRepositoryConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<ITeamsMeetingRepository, TeamsMeetingRepository>();
        }
    }
}