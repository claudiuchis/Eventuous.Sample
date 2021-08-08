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
    [Route("[controller]")]
    public class WidgetController : ControllerBase
    {

        WidgetService _service;
        readonly ILogger<WidgetController> _log;
        public WidgetController(
            WidgetService service,
            ILoggerFactory loggerFactory
        )
        {
            _service = service;
            _log = loggerFactory.CreateLogger<WidgetController>();
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
            catch(Exception e)
            {
                _log.LogError($"{e.Message} - {e.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
