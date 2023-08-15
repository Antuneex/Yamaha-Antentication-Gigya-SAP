using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MediatR;
using IntegrationHubApi.Domain.Interfaces.Repositories;
using IntegrationHubApi.Services.MicrosoftTeams.Commands;
using IntegrationHubApi.Services.MicrosoftTeams.Extensions;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using IntegrationHubApi.Domain.Configs;

namespace IntegrationHubApi.Services.MicrosoftTeams.Handlers
{

    public class NewMeetingCommandHandler : BaseMediatR, IRequestHandler<CreateOrUpdateTeamsMeetingCommand, Meeting>
    {
        private readonly ITeamsMeetingRepository _repository;
        private readonly IGraphApiConnector _graphApi;
        private readonly CustomConfig _customConfig;

        public NewMeetingCommandHandler(IHttpContextAccessor httpContextAccessor,
            ITeamsMeetingRepository repository,
            CustomConfig customConfig,
            IGraphApiConnector connector) : base(httpContextAccessor)
        {
            _repository = repository;
            _graphApi = connector;
            _customConfig = customConfig;
        }

        /// <summary>
        /// Listagem de caminhos das apis disponíveis
        /// </summary>
        /// <param name = "request" ></ param >
        /// < param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Meeting> Handle(CreateOrUpdateTeamsMeetingCommand request, CancellationToken cancellationToken)
        {
            var config = await _repository.GetConfigurationByConsumer(_parameters.Consumer);

            if (config == null || string.IsNullOrWhiteSpace(_parameters.Consumer))
                throw new BadHttpRequestException("Consumer cannot be empty");

            if (config.Default)
                config = await _repository.GetConfigurationByConsumer(_customConfig.MicrosoftTeamsDefaultConsumer);

            var newMeeting = request.ToDomainObject();

            config.Decrypt(_customConfig.MicrosoftTeamsKey);

            if (request.Id > 0)
                return await _graphApi.Update(newMeeting, config, request.Token);

            return await _graphApi.Create(newMeeting, config, request.Token);
        }
    }
}