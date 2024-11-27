using LibraryManagement.Application.Features.Notifications.Commands.CreateNotification;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByBookId;
using LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/<NotificationsController>/book/{bookId}
        [Authorize(Roles = "Administrator")]
        [HttpGet("book/{bookId}")]
        [ProducesResponseType(200, Type = typeof(List<NotificationDto>))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<NotificationDto>>> GetByBookId(int bookId)
        {
            var notifications = await _mediator.Send(new GetNotifcationsByBookIdQuery(bookId));
            return Ok(notifications);
        }

        // GET api/<NotificationsController>/customer/{customerId}
        [Authorize(Roles = "Administrator, Customer")]
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(200, Type = typeof(List<NotificationDto>))]
        [ProducesResponseType(401)]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<NotificationDto>>> GetByCustomerId(string customerId)
        {
            // Ensure the ids are the same
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && customerId != userId)
            {
                return Unauthorized("User provided an incorrect Id.");
            }
            var notifications = await _mediator.Send(new GetNotificationsByCustomerIdQuery(customerId));
            return Ok(notifications);
        }

        // POST api/<NotificationsController>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NotificationDto))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(401)]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationCommand command)
        {
            // Ensure the user is authenticated
            var userId = User.FindFirst("uid")?.Value;

            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }
            command.CustomerId = userId;
            var notification = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByBookId), new { bookId = notification.BookId }, notification);
        }
    }
}
