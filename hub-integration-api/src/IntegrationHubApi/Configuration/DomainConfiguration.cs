using Microsoft.Extensions.DependencyInjection;

namespace IntegrationHubApi.Configuration
{
    /// <summary>
    /// Classe para Injeção de dependencia dos objetos de domínio
    /// </summary>
    public static class DomainConfiguration
    {
        /// <summary>
        /// Método de extensão para Injeção de dependencia dos objetos de domínio
        /// </summary>
        public static void AddDomainConfiguration(this IServiceCollection services)
        {
            //services.AddSingleton<AuthDomain>();
        }
    }
}