using Api.Infrastructure;
using Application.Routes.GetSafeRoute;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("route")]
    public class RouteController : ControllerBase
    {
        private readonly ILogger<RouteController> _logger;
        private readonly IMediator _mediator;

        public RouteController(ILogger<RouteController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("get-safe-route")]
        public async Task<ActionResult<GetSafeRouteQueryResult>> GetSafeRoute([FromBody] GetSafeRouteQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Got request: {query}", query);

            return (await _mediator.Send(query, cancellationToken)).GetApiResponse(_logger);
        }
    }
}