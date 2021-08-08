using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Eventuous;
using static Eventuous.Sample.Application.Commands;

namespace Eventuous.Sample.Application
{
    [ApiController]
    [Route("/api/widget")]
    public class WidgetCommandController : ControllerBase
    {

        WidgetCommandService _service;
        readonly ILogger<WidgetCommandController> _log;
        public WidgetCommandController(
            WidgetCommandService service,
            ILoggerFactory loggerFactory
        )
        {
            _service = service;
            _log = loggerFactory.CreateLogger<WidgetCommandController>();
        } 
        
        [HttpPost]
        public async Task<IActionResult> CreateWidget(CreateWidget command)
        {
            try 
            {
                await _service.Handle(command, default);
                return Ok();
            }
            catch(DomainException e) 
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _log.LogError($"{e.Message} - {e.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
