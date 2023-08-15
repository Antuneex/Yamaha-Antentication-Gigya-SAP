using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Entities.Firebase;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace IntegrationHubApi.ExternalServices.Connector
{
    public class FirebaseApiConnector : IFirebaseApiConnector
    {
        private readonly CustomConfig _customConfig;
        public FirebaseApiConnector(CustomConfig customConfig)
        {
            _customConfig = customConfig;
        }
        public async Task<string> SendMessage(SendMessageDTO sendMessageCaseFalseCommand)
        {
            var client = new RestClient(_customConfig.FIREBASE_URL);
            var request = new RestRequest();

            request.Method = Method.POST;
            request.AddHeader("Authorization", _customConfig.FIREBASE_TOKEN);
            var json = JsonConvert.SerializeObject(sendMessageCaseFalseCommand);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = client.Execute(request);

            return response.Content;

        }
    }
}
