using Application.Common.Dispatchers.Interfaces;
using Application.Common.Results;
using Application.Logs.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        public LogsController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] bool? success, CancellationToken cancellationToken)
        {
            var request = new LogRequest
            {

            };
            var result = await _queryDispatcher.Dispatch<LogRequest, Result<List<LogResponse>>>(request, cancellationToken);

            if (result.Success && result.Value != null) return Ok(result.Value);

            return BadRequest(new { Errors = result.Errors });
        }
    }
}
