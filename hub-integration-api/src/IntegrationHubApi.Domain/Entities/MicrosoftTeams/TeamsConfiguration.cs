namespace IntegrationHubApi.Domain.Entities.MicrosoftTeams
{
    public class TeamsConfiguration
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string OrganizerLogin { get; set; }
        public string OrganizerPassword { get; set; }
        public string RedirectUri { get; set; }
        public string AuthUrl { get; set; }
        public bool Default { get; set; }
    }
}