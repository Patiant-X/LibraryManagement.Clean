using FluentValidation;
using LibraryManagement.Application.Contracts.Persistence;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookCommandValidator(IBookRepository bookRepository)
        {
            RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull()
            .MustAsync(BookMustExist).WithMessage("{PropertyName} resource does not exist");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than zero");

            RuleFor(p => p.IsBorrowed)
           .Must(x => x == false || x == true).WithMessage("{PropertyName} is required");

            RuleFor(p => p.IsReserved).
          Must(x => x == false || x == true).WithMessage("{PropertyName} is required");

            RuleFor(x => x)
                  .MustAsync(BookUnique).WithMessage("Ensure Book ISBN is Unique");
            RuleFor(x => x)
              .Must(BookAvailablityState).WithMessage("Book cannot be Borrowed and Reserved");


            _bookRepository = bookRepository;
        }

        private bool BookAvailablityState(UpdateBookCommand command)
        {
            if (command.IsReserved && command.IsBorrowed) return false;
            return true;
        }

        private async Task<bool> BookMustExist(int id, CancellationToken token)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book != null;
        }

        private async Task<bool> BookUnique(UpdateBookCommand command, CancellationToken token)
        {
            var book = await _bookRepository.GetByIdAsync(command.Id);

            if (book == null) return false;

            if (book.ISBN != command.ISBN)
            {
                var bookEsists = await _bookRepository.IsBookUnique(command.ISBN);
                return bookEsists;
            }
            else
            {
                return true;
            }
        }
    }
}
