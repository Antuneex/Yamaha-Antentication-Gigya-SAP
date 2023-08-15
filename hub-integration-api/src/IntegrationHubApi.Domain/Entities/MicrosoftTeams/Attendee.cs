using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IntegrationHubApi.Domain.Entities.MicrosoftTeams
{
    public class Attendee
    {
         /// <summary>
        /// Identificador do participante
        /// </summary>
        public string Upn { get; set; }

        /// <summary>
        /// Especifica qual a função do participante na reunião
        /// </summary>
        public string Role { get; set; }
    }

    public class MeetingAttendees
    {
        public IEnumerable<Atendee> AttendanceRecords { get; set; }
    }
    public class Atendee
    {
        public string EmailAddress { get; set; }
        public int TotalAttendanceInSeconds { get; set; }
        public string Role { get; set; }
        public Identity Identity { get; set; }
        public IEnumerable<AttendanceInterval> AttendanceIntervals { get; set; }
    }

    public class Identity
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string TenantId { get; set; }
    }
    public class AttendanceInterval
    {
        public DateTime JoinDateTime { get; set; }
        public DateTime LeaveDateTime { get; set; }
        public int DurationInSeconds { get; set; }
    }
}