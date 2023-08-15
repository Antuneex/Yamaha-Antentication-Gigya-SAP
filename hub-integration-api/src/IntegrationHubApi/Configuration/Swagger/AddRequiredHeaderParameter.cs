using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace IntegrationHubApi.Configuration.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// Incluí parâmetros necessários no header
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-authenticated-userid",
                In = ParameterLocation.Header,
                Description = "IdUsuario",
                Required = true
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Consumer-Username",
                In = ParameterLocation.Header,
                Description = "consumer",
                Required = true
            });
        }
    }
}