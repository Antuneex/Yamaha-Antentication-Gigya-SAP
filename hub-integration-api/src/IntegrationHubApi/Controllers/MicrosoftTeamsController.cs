using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Services.MicrosoftTeams.Commands;
using IntegrationHubApi.Services.MicrosoftTeams.Queries;

namespace IntegrationHubApi.Controllers
{
    /// <summary>
    /// Controller para o Gestão do Microsoft Teams
    /// </summary>
    [Route("v1/[controller]")]
    [ApiController]
    public class MicrosoftTeamsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CustomConfig _config;

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="mediator"></param>
        public MicrosoftTeamsController(IMediator mediator, CustomConfig config)
        {
            _mediator = mediator;
            _config = config;
        }

        /// <summary>
        /// Lista todos os participantes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ListAttendees(string id, [FromQuery] string token)
        {
            GetMeetingAttendeesQuery query = new()
            {
                Id = id,
                Token = token
            };

            var attendees = await _mediator.Send(query);

            return Ok(attendees);
        }

        /// <summary>
        /// Lista todos os participantes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("meeting/{id}")]
        public async Task<IActionResult> GetMeeting(string id, [FromQuery] string token)
        {
            GetMeetingQuery query = new()
            {
                Id = id,
                Token = token
            };

            var attendees = await _mediator.Send(query);

            return Ok(attendees);
        }

        /// <summary>
        /// Lista todos os participantes.
        /// </summary>
        /// <returns></returns>
        [HttpGet("authorization")]
        public async Task<IActionResult> GetAuthUrl()
        {
            string url = await _mediator.Send(new GetAuthUrlQuery());

            return Ok(new { url });
        }

        /// <summary>
        /// Cria uma nova reunião no graph api 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewMeeting(CreateOrUpdateTeamsMeetingCommand command)
        {
            if (command == null || !command.IsValid())
                return BadRequest("Incorrect body state.");

            command.Token = (string)Request.Headers["token"];

            var @new = await _mediator.Send(command);

            return Ok(@new);
        }

        /// <summary>
        /// Atualiza uma nova reunião no graph api 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(CreateOrUpdateTeamsMeetingCommand command)
        {
            if (command == null || !command.IsValid())
                return BadRequest();

            command.Token = (string)Request.Headers["token"];

            var @new = await _mediator.Send(command);

            return Ok(@new);
        }

        /// <summary>
        /// Recuperar token de autenticação a partir do código
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("token")]
        public async Task<IActionResult> GetAuthToken([FromQuery] GetTokenCommand command)
        {
            if (command == null || !command.IsValid())
                return BadRequest();

            var token = await _mediator.Send(command);

            if (string.IsNullOrWhiteSpace(token))
                return StatusCode(500, new { Message = "Application could not get access token.\nTry again later." });

            return Ok(new { token });
        }

        /// <summary>
        /// Apenas para teste local
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("/signin-oidc")]
        public IActionResult JustToTests(string code) => Ok(new { code });
    }
}