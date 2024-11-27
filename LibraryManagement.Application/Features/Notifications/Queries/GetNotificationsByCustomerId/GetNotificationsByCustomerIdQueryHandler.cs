using AutoMapper;
using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Features.Notifications.Dto;
using MediatR;

namespace LibraryManagement.Application.Features.Notifications.Queries.GetNotificationsByCustomerId
{
    public class GetNotificationsByCustomerIdQueryHandler : IRequestHandler<GetNotificationsByCustomerIdQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<GetNotificationsByCustomerIdQueryHandler> _logger;

        public GetNotificationsByCustomerIdQueryHandler(INotificationRepository notificationRepository, IMapper mapper
            , IAppLogger<GetNotificationsByCustomerIdQueryHandler> logger)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<List<NotificationDto>> Handle(GetNotificationsByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetNotificationsByCustomerIdAsync(request.CustomerId);
            if (notifications == null || !notifications.Any())
            {
                _logger.LogWarning("No notifications found for CustomerId: {CustomerId}", request.CustomerId);
                throw new NotFoundException(nameof(Notifications), request.CustomerId);
            }
            var data = _mapper.Map<List<NotificationDto>>(notifications);
            _logger.LogInformation("Retrieved {Count} notifications for CustomerId: {CustomerId}", notifications.Count, request.CustomerId);
            return data;
        }
    }
}
