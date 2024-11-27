using FluentValidation;
using LibraryManagement.Application.Contracts.Persistence;

namespace LibraryManagement.Application.Features.Notifications.Commands.CreateNotification
{
    public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
    {
        private readonly IBookRepository _bookRepository;
        private readonly INotificationRepository _notificationRepository;

        public CreateNotificationCommandValidator(IBookRepository bookRepository, INotificationRepository notificationRepository)
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull();


            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .MustAsync(BooKMustExist).WithMessage("{PropertyName} resource not Found or not Available");

            RuleFor(x => x)
                .MustAsync(NotificationMustBeUnique).WithMessage("Notification already exists");
            _bookRepository = bookRepository;
            _notificationRepository = notificationRepository;
        }


        private async Task<bool> NotificationMustBeUnique(CreateNotificationCommand command, CancellationToken token)
        {
            var notification = await _notificationRepository.IsNotificationUnique(command.BookId, command.CustomerId);
            return notification;
        }

        private async Task<bool> BooKMustExist(int BookId, CancellationToken token)
        {
            var book = await _bookRepository.GetByIdAsync(BookId);
            if (book == null)
                return false;
            if (book.IsReserved || book.IsBorrowed)
                return true;
            else return false;
        }
    }
}
