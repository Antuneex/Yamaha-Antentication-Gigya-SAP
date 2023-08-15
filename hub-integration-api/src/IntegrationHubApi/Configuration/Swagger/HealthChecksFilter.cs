using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;

namespace IntegrationHubApi.Configuration.Swagger
{
    /// <summary>
    /// HealthChecks
    /// </summary>
    public class HealthChecksFilter : IDocumentFilter
    {
        private readonly string HealthCheckEndpoint = @"/healthcheck";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openApiDocument"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument openApiDocument, DocumentFilterContext context)
        {
            var pathItem = new OpenApiPathItem();

            var operation = new OpenApiOperation();
            operation.Tags.Add(new OpenApiTag { Name = "Health Checks" });

            Dictionary<string, OpenApiSchema> properties200 = Properties200();
            Dictionary<string, OpenApiSchema> properties503 = Properties503();

            operation.Responses.Add("200", CriarResponse(properties200));
            operation.Responses.Add("503", CriarResponse(properties503));
            pathItem.AddOperation(OperationType.Get, operation);
            openApiDocument?.Paths.Add(HealthCheckEndpoint, pathItem);
        }

        /// <summary>
        /// Response
        /// </summary>
        /// <param name="propertiesItem"></param>
        /// <returns></returns>
        private static OpenApiResponse CriarResponse(Dictionary<string, OpenApiSchema> propertiesItem)
        {

            var properties = new Dictionary<string, OpenApiSchema>();
            properties.Add("status", new OpenApiSchema() { Type = "string" });
            properties.Add("totalDuration", new OpenApiSchema() { Type = "string" });
            properties.Add("entries", new OpenApiSchema()
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>()
                {
                    {"Sql Server - Leitura", new OpenApiSchema() { Type = "object", Properties = propertiesItem } },
                    {"Sql Server - Escrita", new OpenApiSchema() { Type = "object", Properties = propertiesItem } }
                }
            });


            var response = new OpenApiResponse();
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    AdditionalPropertiesAllowed = false,
                    Properties = properties,
                }
            });
            return response;
        }

        private static Dictionary<string, OpenApiSchema> Properties200()
        {
            return new Dictionary<string, OpenApiSchema>()
            {
                {"data", new OpenApiSchema() { Type = "object" } },
                {"duration", new OpenApiSchema() { Type = "string" } },
                {"status", new OpenApiSchema() { Type = "string"} },
                {"tags", new OpenApiSchema() { Type = "array", Items = new OpenApiSchema() { Type = "string" }  } },
            };
        }

        private static Dictionary<string, OpenApiSchema> Properties503()
        {
            return new Dictionary<string, OpenApiSchema>()
            {
                {"data", new OpenApiSchema() { Type = "object" } },
                {"duration", new OpenApiSchema() { Type = "string" } },
                {"exception", new OpenApiSchema() { Type = "string"} },
                {"status", new OpenApiSchema() { Type = "string"} },
                {"tags", new OpenApiSchema() { Type = "array", Items = new OpenApiSchema() { Type = "string" }  } },
            };
        }
    }
}
