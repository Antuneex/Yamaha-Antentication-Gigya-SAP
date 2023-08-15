using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;

namespace IntegrationHubApi.Domain.Interfaces.Connectors
{
    /// <summary>
    /// Interface para a api do Graph
    /// </summary>
    public interface IGraphApiConnector
    {
        Task<Meeting> Create(Meeting meeting, TeamsConfiguration configuration, string token);
        Task<Meeting> Update(Meeting meeting, TeamsConfiguration configuration, string token);
        Task<string> GetAuthToken(TeamsConfiguration configuration, string code, string redirectUri);
        Task<IEnumerable<Atendee>> GetAttendees(string meetingId, TeamsConfiguration configuration, string token);
        Task<object> GetMeeting(string meetingId, TeamsConfiguration configuration, string token);
    }
}