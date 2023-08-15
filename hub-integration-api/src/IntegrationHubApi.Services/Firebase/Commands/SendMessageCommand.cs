using MediatR;

namespace IntegrationHubApi.Domain.Entities.Firebase.Commands
{
    public class SendMessageCommand : IRequest<string>
    {
        public bool ToAll { get; set; }
        public string Device { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public FirebaseData Data { get; set; }
    }
}
