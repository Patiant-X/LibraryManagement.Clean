using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using LibraryManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(LmDatabaseContext context, IAppLogger<Notification> appLogger) : base(context, appLogger)
        {
        }

        public async Task<IReadOnlyList<Notification>> GetActiveNotificationsByBookIdAsync(int bookId)
        {
            return await _context.notifications
                .AsNoTracking()
                .Where(n => n.BookId == bookId && !n.IsNotified)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Notification>> GetNotificationsByCustomerIdAsync(string customerId)
        {
            return await _context.notifications
                .AsNoTracking()
                .Where(n => n.CustomerId == customerId)
                .ToListAsync(); ;
        }

        public async Task<bool> IsNotificationUnique(int bookId, string customerId)
        {
            return await _context.notifications
                .AsNoTracking()
                .AnyAsync(n => n.BookId == bookId && n.CustomerId == customerId) == false;
        }
    }
}
