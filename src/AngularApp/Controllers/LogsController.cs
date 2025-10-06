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
        public async Task<IActionResult> GetLogs([FromQuery] bool? success, [FromQuery] int? page,
            CancellationToken cancellationToken)
        {
            var request = new LogRequest
            {
                Success = success,
                Page = page
            };
            var result = await _queryDispatcher.Dispatch<LogRequest, Result<LogResponse>>(request, cancellationToken);

            if (result.Success && result.Value != null) return Ok(result.Value);

            return BadRequest(new { Errors = result.Errors });
        }
        //
    }
}
