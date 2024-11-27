using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByBookId
{
    public class GetNotificationsByBookIdQueryHandler : IRequestHandler<GetNotifcationsByBookIdQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<GetNotificationsByBookIdQueryHandler> _logger;

        public GetNotificationsByBookIdQueryHandler(INotificationRepository notificationRepository, IMapper mapper
            , IAppLogger<GetNotificationsByBookIdQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<List<NotificationDto>> Handle(GetNotifcationsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetActiveNotificationsByBookIdAsync(request.BookId);

            if (notifications == null || !notifications.Any())
            {
                _logger.LogWarning("No notifications found for BookId: {BookId}", request.BookId);
                throw new NotFoundException(nameof(Notifications), request.BookId);
            }

            _logger.LogInformation("Retrieved {Count} notifications for BookId: {BookId}", notifications.Count, request.BookId);
            var data = _mapper.Map<List<NotificationDto>>(notifications);

            return data;
        }
    }
}
