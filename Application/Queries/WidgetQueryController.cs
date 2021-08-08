using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Eventuous;

namespace Eventuous.Sample.Application.Queries
{
    [ApiController]
    [Route("/api/widget")]
    public class WidgetQueryController : ControllerBase
    {

        WidgetQueryService _service;
        readonly ILogger<WidgetQueryController> _log;
        
        public WidgetQueryController(
            WidgetQueryService service,
            ILoggerFactory loggerFactory
        )
        {
            _service = service;
            _log = loggerFactory.CreateLogger<WidgetQueryController>();
        } 
        
        [HttpGet]
        public async Task<IActionResult> GetWidget(string widgetId)
        {
            try 
            {
                var widget = await _service.GetWidgetById(widgetId);
                if (widget == null)
                    return NotFound();

                return Ok(widget);
            }
            catch(Exception e)
            {
                _log.LogError($"{e.Message} - {e.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
