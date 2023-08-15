using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationHubApi.Domain.Interfaces.Repositories
{
    public interface ITeamsMeetingRepository
    {
        Task<TeamsConfiguration> GetConfigurationByConsumer(string consumer);
        Task Update(int tokenId, int userId);

    }
}