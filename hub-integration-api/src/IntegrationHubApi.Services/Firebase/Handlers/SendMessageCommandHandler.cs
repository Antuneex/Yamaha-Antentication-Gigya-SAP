using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Entities.Firebase;
using IntegrationHubApi.Domain.Entities.Firebase.Commands;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationHubApi.Services.Firebase.Handlers
{
    public class SendMessageCommandHandler : BaseMediatR, IRequestHandler<SendMessageCommand, string>
    {
        private readonly CustomConfig _customConfig;
        private readonly IFirebaseApiConnector _firebaseApiConnector;

        public SendMessageCommandHandler(IHttpContextAccessor httpContextAccessor,
            CustomConfig customConfig,
            IFirebaseApiConnector firebaseApiConnector) : base(httpContextAccessor)
        {
            _customConfig = customConfig;
            _firebaseApiConnector = firebaseApiConnector;
        }

        public async Task<string> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var sendMessageCaseFalseCommand = new Domain.Entities.Firebase.SendMessageDTO()
            {
                to = request.ToAll ? "/topics/all" : request.Device,
                Notification = new FirebaseNotification { body = request.Body, title = request.Title },
                Data = new FirebaseData { contentId = request.Data.contentId }
            };

            var trueRequest = await _firebaseApiConnector.SendMessage(sendMessageCaseFalseCommand);
            return trueRequest;

        }
    }
}
