using Microsoft.AspNetCore.Mvc;
using PassIn.Application.UseCases.CheckIns;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        [HttpPost]
        [Route("{attendeeId}")]
        [ProducesResponseType(typeof(ResponseRegisteredJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
        public IActionResult Checkin([FromRoute] Guid attendeeId)
        {

            var useCase = new DoAttendeeCheckIn();

            var response = useCase.Execute(attendeeId);

            return Created(string.Empty, response);
        }
    }
}
