namespace IntegrationHubApi.Domain.Entities.MicrosoftTeams
{
    public class LobbySettings
    {
          /// <summary>
        /// Especifica se ligações podem ou não ignorar o lobby
        /// </summary>
        public bool IsDialInBypassEnabled { get; set; }


        /// <summary>
        /// Especifica o tipo de participantes que são automaticamente admitidos
        /// </summary>
        public string Scope { get; set; } = "everyone";
    }
}