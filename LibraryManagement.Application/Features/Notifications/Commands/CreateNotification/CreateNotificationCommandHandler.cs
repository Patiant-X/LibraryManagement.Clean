using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Dto;
using LibraryManagement.Domain;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Commands.CreateNotification
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationDto>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            INotificationRepository notificationRepository,
            IBookRepository bookRepository,
            IMapper mapper,
            IAppLogger<CreateNotificationCommandHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateNotificationCommandValidator(_bookRepository, _notificationRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Any())
            {
                _logger.LogWarning("Validation errors in notification request for BookId: {0}, CustomerId: {1}", request.BookId, request.CustomerId);
                throw new BadRequestException("Invalid Notification data", validationResult);
            }

            var notificationToCreate = _mapper.Map<Notification>(request);

            try
            {
                await _notificationRepository.CreateAsync(notificationToCreate);
                _logger.LogInformation("Notification created successfully for BookId: {0}, CustomerId: {1}", request.BookId, request.CustomerId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Database error occurred while creating notification for BookId: {0}. Exception: {1}", request.BookId, ex.Message);
                throw new DatabaseException("An error occurred while processing the request.", ex);
            }


            var data = _mapper.Map<NotificationDto>(notificationToCreate);
            _logger.LogInformation("Returning NotificationDto for BookId: {0}, CustomerId: {1}", request.BookId, request.CustomerId);


            return data;
        }
    }
}
