using LibraryManagement.Application.Features.Reservations.Commands.CreateReservation;
using LibraryManagement.Application.Features.Reservations.Dto;
using LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByBookId;
using LibraryManagement.Application.Features.Reservations.Queries.GetReservationsByCustomerId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/<ReservationsController>/book/{bookId}
        [Authorize(Roles = "Administrator")]
        [HttpGet("book/{bookId}")]
        [ProducesResponseType(200, Type = typeof(ReservationDto))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ReservationDto>> GetByBookId(int bookId)
        {
            var reservation = await _mediator.Send(new GetReservationByBookIdQuery(bookId));
            return Ok(reservation);
        }


        // GET api/<ReservationsController>/customer/{customerId}
        [Authorize(Roles = "Administrator, Customer")]
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(200, Type = typeof(List<ReservationDto>))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<ReservationDto>>> GetByCustomerId(string customerId)
        {
            // Ensure the user is authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && customerId != userId)
            {
                return Unauthorized("User provided an incorrect Id.");
            }
            var reservations = await _mediator.Send(new GetCustomersReservationsQuery(customerId));
            return Ok(reservations);
        }

        // POST api/<ReservationsController>
        [Authorize(Roles = "Administrator, Customer")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ReservationDto))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationCommand command)
        {
            // Ensure the user is authenticated
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && command.CustomerId != userId)
            {
                return Unauthorized("User provided an incorrect Id.");
            }
            var reservation = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetByBookId), new { bookId = reservation.BookId }, reservation);
        }

    }
}
