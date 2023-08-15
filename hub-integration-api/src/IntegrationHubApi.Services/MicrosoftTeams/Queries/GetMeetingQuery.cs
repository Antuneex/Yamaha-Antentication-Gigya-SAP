using MediatR;

namespace IntegrationHubApi.Services.MicrosoftTeams.Queries
{
    public class GetMeetingQuery : IRequest<object>
    {
        public string Id { get; set; }
        public string Token { get; set; }
    }
}
