using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using IntegrationHubApi.Configuration.Swagger;
using System.IO;

namespace IntegrationHubApi.Configuration
{
    /// <summary>
    /// Classe de configuração do Swagger
    /// </summary>
    public static class SwaggerConfiguration
    {

        /// <summary>
        /// Metodo de configuração do swagger
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "Integration Hub API",
                        Version = "v1",
                        Description = "Documentação da API do Integration Hub",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Name = "UOL EdTech"
                        }
                    });

                string caminhoAplicacao = PlatformServices.Default.Application.ApplicationBasePath;
                string nomeAplicacao = PlatformServices.Default.Application.ApplicationName;

                string caminhoXmlDoc = Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");

                c.IncludeXmlComments(caminhoXmlDoc);
                c.UseInlineDefinitionsForEnums();
            });

            services.ConfigureSwaggerGen(option =>
            {
                option.CustomSchemaIds(x => x.FullName);
                option.DocumentFilter<HealthChecksFilter>();
                option.OperationFilter<AddRequiredHeaderParameter>();
            });

        }

        /// <summary>
        /// Método que configura a aplicação para utilizar o swagger
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Sapiência Platform API");
            });
        }
    }
}