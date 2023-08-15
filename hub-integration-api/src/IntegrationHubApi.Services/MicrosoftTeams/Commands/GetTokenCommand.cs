using IntegrationHubApi.Services.Base;
using MediatR;

namespace IntegrationHubApi.Services.MicrosoftTeams.Commands
{
    public class GetTokenCommand : ICommand, IRequest<string>
    {
        public string Code { get; set; }
        public string RedirectUri { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code);
        }
    }
}
