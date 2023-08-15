using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using IntegrationHubApi.Domain.Interfaces.Repositories;
using IntegrationHubApi.Services.MicrosoftTeams.Commands;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;

namespace IntegrationHubApi.Services.MicrosoftTeams.Handlers
{
    public class GetAuthUrlCommandHandler : BaseMediatR, IRequestHandler<GetAuthUrlQuery, string>, IRequestHandler<GetTokenCommand, string>
    {
        private readonly ITeamsMeetingRepository _repository;
        private readonly IGraphApiConnector _connector;
        private readonly CustomConfig _customConfig;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="repository"></param>
        /// <param name="connector"></param>
        /// <param name="customConfig"></param>
        public GetAuthUrlCommandHandler(IHttpContextAccessor httpContextAccessor,
            ITeamsMeetingRepository repository,
            IGraphApiConnector connector,
            CustomConfig customConfig) : base(httpContextAccessor)
        {
            _repository = repository;
            _connector = connector;
            _customConfig = customConfig;
        }

        /// <summary>
        /// Retorna url de autorização da Microsoft
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<string> Handle(GetAuthUrlQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_parameters.Consumer))
                throw new BadHttpRequestException("Consumer cannot be empty");

            var config = await GetConfiguration();

            var url = _customConfig.MicrosoftTeamsAuthorizeUrl;

            return string.Format(url, config.TenantId, config.ClientId, config.RedirectUri, _customConfig.MicrosoftTeamsScope);
        }

        /// <summary>
        /// Retorna token de autenticação da Microsoft
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<string> Handle(GetTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_parameters.Consumer))
                throw new BadHttpRequestException("Consumer cannot be empty");

            var config = await GetConfiguration();

            var token = await _connector.GetAuthToken(config, request.Code, request.RedirectUri);

            return token;
        }

        private async Task<TeamsConfiguration> GetConfiguration()
        {
            var config = await _repository.GetConfigurationByConsumer(_parameters.Consumer);

            if (config == null)
                throw new BadHttpRequestException("Consumer doesn't exists");

            if (config.Default)
                config = await _repository.GetConfigurationByConsumer(_customConfig.MicrosoftTeamsDefaultConsumer);

            return config;
        }
    }
}
