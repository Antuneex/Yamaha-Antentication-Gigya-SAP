using System;
using MediatR;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams.Enum;
using System.Collections.Generic;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using IntegrationHubApi.Services.Base;
using System.Linq;

namespace IntegrationHubApi.Services.MicrosoftTeams.Commands
{
    /// <summary>
    /// Command para criar nova reunião no Teams
    /// </summary>
    public class CreateOrUpdateTeamsMeetingCommand : ICommand, IRequest<Meeting>
    {
        /// <summary>
        /// Código da aula ao vivo.
        /// </summary>
        public virtual long? Id { get; set; }

        /// <summary>
        /// Nome da aula.
        /// </summary>
        public virtual string Name { get; set; }
     
        /// <summary>
        /// Apresentadores
        /// </summary>
        public virtual IEnumerable<string> Presenters { get; set; }

        /// <summary>
        /// Data de início da aula.
        /// </summary>
        public virtual DateTime StartOn { get; set; }

        /// <summary>
        /// Duração da aula em minutos.
        /// </summary>
        public virtual int Duration { get; set; }

        /// <summary>
        /// Identificador externo da aula.
        /// </summary>
        public virtual string MeetingIdentifier { get; set; }

        /// <summary>
        /// Quem pode pular a sala de espera?
        /// </summary>
        public virtual SkipLobby SkipLobby { get; set; }

        /// <summary>
        /// Notificar entrada e saída de participantes.
        /// </summary>
        public virtual bool AnnounceParticipants { get; set; }

        /// <summary>
        /// Permitir que participantes liguem a câmera.
        /// </summary>
        public virtual bool AllowCamera { get; set; }

        /// <summary>
        /// Permitir que participantes liguem o microfone.
        /// </summary>
        public virtual bool AllowMicrophone { get; set; }

        /// <summary>
        /// Permitir que participantes usem reações.
        /// </summary>
        public virtual bool AllowReactions { get; set; }

        /// <summary>
        /// Token de autenticação
        /// </summary>
        public string Token { get; set; }

        public bool IsValid()
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones();

            var local = timezones.FirstOrDefault(e => e.Id == "America/Sao_Paulo" || e.Id == "America/Bahia") ??
                timezones.FirstOrDefault(e => e.StandardName == "E. South America Standard Time");

            var now = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, local.Id);

            return !(string.IsNullOrWhiteSpace(Name)
               || StartOn < now
               || Duration < 1);
        }
    }
}