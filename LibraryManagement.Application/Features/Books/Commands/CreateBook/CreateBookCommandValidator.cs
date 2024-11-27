using FluentValidation;
using LibraryManagement.Application.Contracts.Persistence;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook
{
    public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        private readonly IBookRepository _bookRepository;

        public CreateBookCommandValidator(IBookRepository bookRepository)
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .GreaterThan(0).WithMessage("{PropertyName} must be 10 digits");


            RuleFor(x => x)
               .MustAsync(BookUnique).WithMessage("Book already exists");

            RuleFor(x => x)
               .Must(BookMustBeAvailable).WithMessage("Book Availability Clash; IsReserved and IsBorrowed cannot both be true");

            _bookRepository = bookRepository;
        }

        private bool BookMustBeAvailable(CreateBookCommand command)
        {
            if (command.IsReserved && !command.IsBorrowed && command.ReturnDate != null)
                return true;
            else if (!command.IsReserved && command.IsBorrowed && command.ReturnDate != null)
                return true;
            else if (command.IsReserved && command.IsBorrowed)
                return false;
            else if (!command.IsReserved && !command.IsBorrowed && command.ReturnDate == null)
                return true;
            else return false;
        }

        private async Task<bool> BookUnique(CreateBookCommand command, CancellationToken token)
        {
            var book = await _bookRepository.IsBookUnique(command.ISBN);
            return book;
        }
    }
}
