using System.Collections.Generic;

namespace IntegrationHubApi.Domain.Entities.MicrosoftTeams
{
    public class Participants
    {
        /// <summary>
        /// Lista de participantes da reunião
        /// </summary>
        public IEnumerable<Attendee> Attendees { get; set; }
    }
}