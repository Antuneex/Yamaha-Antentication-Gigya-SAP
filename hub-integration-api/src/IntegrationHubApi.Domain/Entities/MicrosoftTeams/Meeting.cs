namespace IntegrationHubApi.Domain.Entities.MicrosoftTeams
{
    /// <summary>
    /// Reunião do Teams
    /// </summary>
    public class Meeting
    {
        /// <summary>
        /// O identificador do Teams.
        /// </summary>
        public string Id { get; set; }

         /// <summary>
        /// O assunto do Teams.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Inicio da reunião em UTC
        /// </summary>
        public string StartDateTime { get; set; }

        /// <summary>
        /// Fim da reunião em UTC
        /// </summary>
        public string EndDateTime { get; set; }
        
        /// <summary>
        /// Url da meeting
        /// </summary>
        public string JoinUrl { get; set; }

        /// <summary>
        /// Especifica se participantes podem habilitar a camera
        /// </summary>
        public bool AllowAttendeeToEnableCamera { get; set; }

        /// <summary>
        /// Especifica se participantes podem habilitar o microfone
        /// </summary>
        public bool AllowAttendeeToEnableMic { get; set; }

        /// <summary>
        /// Especifica se participantes podem utilizar as reações do Teams
        /// </summary>
        public bool AllowTeamworkReactions { get; set; }

        /// <summary>
        /// Especifica se participantes podem utilizar as reações do Teams
        /// </summary>
        public bool IsEntryExitAnnounced { get; set; }

        /// <summary>
        /// Os Participantes associados a reunião
        /// </summary>
        public Participants Participants { get; set; }

        /// <summary>
        /// Especifica as configurações de lobby para a reunião
        /// </summary>
        public LobbySettings LobbyBypassSettings { get; set; }
    }
}