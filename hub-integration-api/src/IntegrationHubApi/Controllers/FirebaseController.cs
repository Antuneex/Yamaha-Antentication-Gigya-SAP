using IntegrationHubApi.Domain.Configs;
using IntegrationHubApi.Domain.Entities.Firebase.Commands;
using IntegrationHubApi.Domain.Interfaces.Connectors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace IntegrationHubApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class FirebaseController : ControllerBase
    {

        private readonly IMediator _mediator;
        //private readonly CustomConfig _config;
        private readonly IFirebaseApiConnector _firebaseApiConnector;

        public FirebaseController(IFirebaseApiConnector firebaseApiConnector, IMediator mediator)
        {
            _firebaseApiConnector = firebaseApiConnector;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage([FromBody] SendMessageCommand firebaseCommand)
        {
            if (firebaseCommand == null)
                return BadRequest("Incorrect body state.");

            var @new = await _mediator.Send(firebaseCommand);

            return Ok(@new);
        }

    }
}
