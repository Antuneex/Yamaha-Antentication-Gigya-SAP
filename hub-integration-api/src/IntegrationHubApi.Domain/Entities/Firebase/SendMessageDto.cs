using MediatR;

namespace IntegrationHubApi.Domain.Entities.Firebase
{
    public class SendMessageDTO : IRequest<string>
    {
        public string to { get; set; }
        public FirebaseNotification Notification { get; set; }
        public FirebaseData Data { get; set; }
    }


}

