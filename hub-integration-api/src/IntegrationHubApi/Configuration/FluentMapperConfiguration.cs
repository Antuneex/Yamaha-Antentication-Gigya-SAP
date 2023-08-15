using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
namespace IntegrationHubApi.Configuration
{
    /// <summary>
    /// Classe para Mapeamento de objetos no FluentMapper
    /// </summary>
    public static class FluentMapperConfiguration
    {

          /// <summary>
        /// Método de extensão para Mapeamento de objetos no FluentMapper
        /// </summary>
        public static void AddFluentMapperConfiguration(this IServiceCollection services)
        {
            FluentMapper.Initialize(config =>
            {
               
            });
        }
    }
}