using ApiHelper.Domain.Entities.NewRelic;
using ApiHelper.Filters;
using ApiHelper.Trace;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using IntegrationHubApi.Configuration;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Services;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using IntegrationHubApi.ExternalServices.Connector;

namespace IntegrationHubApi
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuração dos serviços da aplicação
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            CustomConfig customConfig = new CustomConfig(Configuration);
            services.AddSingleton(customConfig);

            //CORS
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            //HttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //MediatR
            services.AddMediatR(typeof(BaseMediatR).Assembly);

            //Repositories
            services.AddRepositoryConfiguration();

            //Connectors
            services.AddConnectorConfiguration();

            //Services
            services.AddDomainConfiguration();
            

            //FluentMapper
            services.AddFluentMapperConfiguration();

            //Swagger
            services.AddSwaggerConfiguration();

            //SqlServer HelthCheck
            services.AddHealthChecks()
                    .AddSqlServer(customConfig.ConnectionStringintegrationHubRead, healthQuery: "SELECT 1;", name: "Sql Server - Leitura", failureStatus: HealthStatus.Unhealthy)
                    .AddSqlServer(customConfig.ConnectionStringintegrationHubWrite, healthQuery: "SELECT 1;", name: "Sql Server - Escrita", failureStatus: HealthStatus.Unhealthy);

            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15);
                opt.MaximumHistoryEntriesPerEndpoint(60);
                opt.SetApiMaxActiveRequests(1);

                opt.AddHealthCheckEndpoint("default api", "/healthcheck");
            }).AddInMemoryStorage();

            //Logs
            LogConfiguration newRelicLog = new LogConfiguration(customConfig.NewRelicUrl, customConfig.NewRelicKey, customConfig.NewRelicGuids, customConfig.NewRelicService);

            services.AddSingleton(newRelicLog);
            services.AddSingleton<NewRelicHelper>();

            //Controllers
            services.AddControllers(config =>
            {
                config.Filters.Add(new ExceptionHandlerFilter("infocast", newRelicLog, customConfig.DEBUG_MODE));
            });

            services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = false;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseSwaggerConfiguration();

            app.UseHealthChecksUI();

            app.UseEndpoints(endpoints =>
            {
                //adding endpoint of health check for the health check ui in UI format
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                //map healthcheck ui endpoing - default is /healthchecks-ui/
                endpoints.MapHealthChecksUI();
                endpoints.MapControllers();
            });
        }
    }
}