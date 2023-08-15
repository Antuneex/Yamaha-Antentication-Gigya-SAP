using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MediatR;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using IntegrationHubApi.Services.MicrosoftTeams.Queries;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using IntegrationHubApi.Domain.Interfaces.Repositories;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Services.MicrosoftTeams.Extensions;
using System.Collections.Generic;

namespace IntegrationHubApi.Services.MicrosoftTeams.Handlers
{
    internal class GetMeetingQueryHandler : BaseMediatR, IRequestHandler<GetMeetingQuery, object>
    {
        private readonly IGraphApiConnector _graphApi;
        private readonly ITeamsMeetingRepository _repository;
        private readonly CustomConfig _customConfig;

        public GetMeetingQueryHandler(IHttpContextAccessor httpContextAccessor,
            IGraphApiConnector graphApi,
            ITeamsMeetingRepository repository,
            CustomConfig customConfig) : base(httpContextAccessor)
        {
            _graphApi = graphApi;
            _repository = repository;
            _customConfig = customConfig;
        }

        public async Task<object> Handle(GetMeetingQuery request, CancellationToken cancellationToken)
        {
            var config = await _repository.GetConfigurationByConsumer(_parameters.Consumer);

            if (config.Default)
                config = await _repository.GetConfigurationByConsumer(_customConfig.MicrosoftTeamsDefaultConsumer);

            config.Decrypt(_customConfig.MicrosoftTeamsKey);

            var attendees = await _graphApi.GetMeeting(request.Id, config, request.Token);
           
            return attendees;
        }
    }
}
