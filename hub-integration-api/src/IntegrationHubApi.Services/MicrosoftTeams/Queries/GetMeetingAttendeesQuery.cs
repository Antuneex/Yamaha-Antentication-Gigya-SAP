using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using MediatR;
using System.Collections.Generic;

namespace IntegrationHubApi.Services.MicrosoftTeams.Queries
{
    public class GetMeetingAttendeesQuery : IRequest<IEnumerable<Atendee>>
    {
        public string Id { get; set; }
        public string Token { get; set; }
    }
}
