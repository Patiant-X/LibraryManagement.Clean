using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.Application.Features.Books.Queries.GetBookDetails;
using LibraryManagement.Application.Features.Books.Queries.GetBooksBySearch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // GET: api/<BooksController>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<BookDto>))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<BookDto>>> Get()
        {
            var books = await _mediator.Send(new GetAllBooksQuery());
            return Ok(books);
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(BookDetailsDto))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<BookDetailsDto>> GetById(int id)
        {
            var book = await _mediator.Send(new GetBookDetailsQuery(id));
            return Ok(book);
        }

        // GET api/<BooksController>/search
        [HttpGet("search")]
        [ProducesResponseType(200, Type = typeof(List<BookSearchDto>))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<BookSearchDto>>> Search(string title)
        {
            var books = await _mediator.Send(new GetBookBySearchQuery(title));
            return Ok(books);
        }


        // POST api/<BooksController>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateBookDto))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<CreateBookDto>> Post(CreateBookCommand bookCommand)
        {
            var book = await _mediator.Send(bookCommand);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        // PUT api/<BooksController>/5
        [Authorize(Roles = "Administrator")]
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(UpdateBookDto))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<UpdateBookDto>> Put(UpdateBookCommand bookCommand)
        {
            var book = await _mediator.Send(bookCommand);
            return Ok(book);
        }

        // DELETE api/<BooksController>/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(400, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteBookCommand { Id = id });
            return NoContent();
        }
    }
}
