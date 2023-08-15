using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams.Enum;
using IntegrationHubApi.Services.MicrosoftTeams.Commands;

namespace IntegrationHubApi.Services.MicrosoftTeams.Extensions
{
    public static class NewTeamsMeetingCommandExtension
    {
        public static Meeting ToDomainObject(this CreateOrUpdateTeamsMeetingCommand command)
        {
            var liveDate = new DateTime(command.StartOn.Ticks, DateTimeKind.Local);
            try
            {
                return new Meeting
                {
                    Subject = command.Name,
                    StartDateTime = liveDate.ToUniversalTime().ToString("O"),
                    EndDateTime = liveDate.AddMinutes(command.Duration).ToUniversalTime().ToString("O"),
                    AllowAttendeeToEnableCamera = command.AllowCamera,
                    AllowAttendeeToEnableMic = command.AllowMicrophone,
                    AllowTeamworkReactions = command.AllowReactions,
                    IsEntryExitAnnounced = command.AnnounceParticipants,
                    Participants = DefineParticipants(command.Presenters),
                    LobbyBypassSettings = new LobbySettings
                    {
                        IsDialInBypassEnabled = false,
                        Scope = command.SkipLobby.ToString("g").ToLower()
                    }
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static Participants DefineParticipants(IEnumerable<string> presenters)
        {
            if (presenters == null || !presenters.Any())
                return null;

            var role = OnlineMeetingRole.Presenter.ToString("g").ToLower();
            var attendees = presenters
                .Select(email => new Attendee { Upn = email, Role = role }).ToList();

            return new Participants { Attendees = attendees };
        }
    }
}