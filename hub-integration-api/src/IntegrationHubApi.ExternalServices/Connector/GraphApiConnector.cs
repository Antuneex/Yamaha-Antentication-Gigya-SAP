using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Entities.MicrosoftTeams;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Linq;

namespace IntegrationHubApi.ExternalServices.Connector
{
    /// <summary>
    /// Classe que se conecta com a api Graph da Microsoft
    /// </summary>
    public class GraphApiConnector : IGraphApiConnector
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly CustomConfig _customConfig;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="customConfig"></param>
        public GraphApiConnector(IHttpClientFactory clientFactory, CustomConfig customConfig)
        {
            _clientFactory = clientFactory;
            _customConfig = customConfig;
        }

        /// <summary>
        /// Cria uma nova reunião no Teams
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public async Task<Meeting> Create(Meeting meeting, TeamsConfiguration configuration, string token)
        {
            if (configuration.Default)
                token = await GetAuthTokenWithUserAndPass(configuration);

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Application could not estabilsh connection with microsoft graph api.");

            GraphServiceClient graphClient = CreateClient(configuration, token);



            var onlineMeeting = new OnlineMeeting
            {
                StartDateTime = DateTimeOffset.Parse(meeting.StartDateTime),
                EndDateTime = DateTimeOffset.Parse(meeting.EndDateTime),
                Subject = meeting.Subject,
                IsEntryExitAnnounced = meeting.IsEntryExitAnnounced,
                AllowedPresenters = OnlineMeetingPresenters.Everyone,
                AllowAttendeeToEnableMic = meeting.AllowAttendeeToEnableMic,
                AllowAttendeeToEnableCamera = meeting.AllowAttendeeToEnableCamera,
                AllowTeamworkReactions = meeting.AllowTeamworkReactions,
                LobbyBypassSettings = new LobbyBypassSettings
                {
                    IsDialInBypassEnabled = false,
                    Scope = DefineLobbyScope(meeting)
                },
            };

            if (meeting.Participants?.Attendees?.Count() > 0)
            {
                onlineMeeting.AllowedPresenters = OnlineMeetingPresenters.RoleIsPresenter;
                onlineMeeting.Participants = new MeetingParticipants
                {
                    Attendees = meeting.Participants.Attendees.ToList().Select(e =>
                    {
                        return new MeetingParticipantInfo
                        {
                            Role = e.Role.ToUpper().Equals("PRESENTER") ? OnlineMeetingRole.Presenter : OnlineMeetingRole.Attendee,
                            Upn = e.Upn
                        };
                    })
                };
            }

            var newMeeting = await graphClient.Me.OnlineMeetings
                .Request()
                .AddAsync(onlineMeeting);

            if (newMeeting == null || string.IsNullOrEmpty(newMeeting.Id))
                throw new HttpRequestException("Could not stablish connection with Microsoft Graph Api.");

            meeting.Id = newMeeting.Id;
            meeting.JoinUrl = newMeeting.JoinWebUrl;

            return meeting;
        }

        /// <summary>
        /// Atualiza uma reunião no Teams
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="configuration"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Meeting> Update(Meeting meeting, TeamsConfiguration configuration, string token)
        {
            if (configuration.Default)
                token = await GetAuthTokenWithUserAndPass(configuration);

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Application could not estabilsh connection with microsoft graph api.");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var content = JsonConvert.SerializeObject(meeting, settings);

            using var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await SendRequest(meeting.Id, token, bodyContent);

            string resultContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(resultContent);

            var data = JsonConvert.DeserializeObject<Meeting>(resultContent);

            if (data.JoinUrl == null)
                throw new Exception(resultContent, default);

            meeting.Id = data.Id;
            meeting.JoinUrl = data.JoinUrl;

            return meeting;
        }

        /// <summary>
        /// Adquire token de autenticação com a microsoft graph api
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetAuthToken(TeamsConfiguration configuration, string code, string redirectUri)
        {
            var param = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "client_id", configuration.ClientId },
            };

            var data = $"{configuration.ClientId}:{configuration.Secret}";
            var dataAsBytes = Encoding.ASCII.GetBytes(data);
            var base64 = Convert.ToBase64String(dataAsBytes);

            var urlToken = string.Format(_customConfig.MicrosoftTeamsTokenUrl, configuration.TenantId);
            var request = new HttpRequestMessage(HttpMethod.Post, urlToken)
            {
                Content = new FormUrlEncodedContent(param),
            };

            request.Headers.Add("Authorization", $"Basic {base64}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Application could not authenticate with Microsoft Graph Api. Try again.");

            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

            return result["access_token"].ToString();
        }

        /// <summary>
        /// Lista os participantes de uma reunião do Teams
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="configuration"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<Atendee>> GetAttendees(string meetingId, TeamsConfiguration configuration, string token)
        {
            if (configuration.Default)
                token = await GetAuthTokenWithUserAndPass(configuration);

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Application could not estabilsh connection with microsoft graph api.");

            var id = await GetAttendanceReportId(meetingId, token);

            if (string.IsNullOrEmpty(id))
                return null;

            var url = $"{_customConfig.MicrosoftTeamsCreateUrl}/{meetingId}/attendanceReports/{id}/attendanceRecords";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"Bearer {token}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Could not estabilsh connection with microsoft graph api to retrieve meeting attendees.");

            var content = await response.Content.ReadAsStringAsync();

            var result = JToken.Parse(content)["value"].ToObject<IEnumerable<Atendee>>();

            return result;
        }

        public async Task<string> GetAttendanceReportId(string meetingId, string token)
        {
            var url = $"{_customConfig.MicrosoftTeamsCreateUrl}/{meetingId}/meetingAttendanceReport";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"Bearer {token}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Could not estabilsh connection with microsoft graph api to retrieve meeting attendees.");

            var id = JToken.Parse(await response.Content.ReadAsStringAsync())["id"].ToObject<string>();

            return id;
        }

        public async Task<object> GetMeeting(string meetingId, TeamsConfiguration configuration, string token)
        {
            if (configuration.Default)
                token = await GetAuthTokenWithUserAndPass(configuration);

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Application could not estabilsh connection with microsoft graph api.");

            var url = $"{_customConfig.MicrosoftTeamsCreateUrl}/{meetingId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"Bearer {token}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Could not estabilsh connection with microsoft graph api to retrieve meeting attendees.");

            var meeting = JToken.Parse(await response.Content.ReadAsStringAsync()).ToObject<dynamic>();

            return meeting;
        }

        private LobbyBypassScope DefineLobbyScope(Meeting meeting)
        {
            switch (meeting.LobbyBypassSettings.Scope.ToUpper())
            {
                case "EVERYONE":
                    return LobbyBypassScope.Everyone;
                case "ORGANIZER":
                    return LobbyBypassScope.Organizer;
                case "ORGANIZATION":
                    return LobbyBypassScope.Organization;
                case "ORGANIZATIONANDFEDERATED":
                    return LobbyBypassScope.OrganizationAndFederated;
                case "UNKNOWNFUTUREVALUE":
                    return LobbyBypassScope.UnknownFutureValue;
                case "INVITED":
                    return LobbyBypassScope.Invited;
                case "ORGANIZATIONEXCLUDINGGUESTS":
                    return LobbyBypassScope.OrganizationExcludingGuests;
                default:
                    return LobbyBypassScope.Everyone;
            }
        }

        private async Task<string> GetAuthTokenWithUserAndPass(TeamsConfiguration configuration)
        {
            var auth = new Uri(string.Format(_customConfig.MicrosoftTeamsTokenUrl, configuration.TenantId));

            using var client = new HttpClient();
            var result = await client.PostAsync(auth, new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("client_id", configuration.ClientId),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", configuration.OrganizerLogin),
                    new KeyValuePair<string, string>("password", configuration.OrganizerPassword),
                    new KeyValuePair<string, string>("scope", "OnlineMeetings.ReadWrite User.Read"),
                    new KeyValuePair<string, string>("client_secret", configuration.Secret),
                }));

            var content = await result.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

            return data["access_token"].ToString();
        }

        private async Task<HttpResponseMessage> SendRequest(string identifier, string token, StringContent content)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);

            if (string.IsNullOrWhiteSpace(identifier))
                return await client.PostAsync(_customConfig.MicrosoftTeamsCreateUrl, content);

            var request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                $"{_customConfig.MicrosoftTeamsCreateUrl}/{identifier}")
            {
                Content = content
            };
            return await client.SendAsync(request);
        }

        private GraphServiceClient CreateClient(TeamsConfiguration config, string token = "")
        {
            var scopes = new string[] { "https://graph.microsoft.com/.default" };

            var confidentialClient = ConfidentialClientApplicationBuilder
               .Create(config.ClientId)
               .WithAuthority($"https://login.microsoftonline.com/{config.TenantId}/v2.0")
               .WithClientSecret(config.Secret)
               .Build();

            var client = new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                _ = await confidentialClient.AcquireTokenForClient(scopes).ExecuteAsync();

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }));

            return client;
        }
    }
}