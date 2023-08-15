using IntegrationHubApi.Domain.Entities.Firebase;
using System.Threading.Tasks;

namespace IntegrationHubApi.Domain.Interfaces.Connectors
{
    public interface IFirebaseApiConnector
    {
        Task<string> SendMessage(SendMessageDTO sendMessageCaseFalseCommand);
    }
}
