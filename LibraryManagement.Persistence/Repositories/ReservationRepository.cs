using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using LibraryManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Persistence.Repositories
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(LmDatabaseContext context, IAppLogger<Reservation> appLogger) : base(context, appLogger)
        {
        }

        public async Task<Reservation> GetActiveReservationsByBookIdAsync(int bookId)
        {
            return await _context.reservations
                .AsNoTracking()
                .Where(r => r.BookId == bookId && !r.IsExpired)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Reservation>> GetExpiredReservationsAsync()
        {
            return await _context.reservations
                .AsNoTracking()
                .Where(r => r.IsExpired)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Reservation>> GetReservationsByCustomerIdAsync(string customerId)
        {
            return await _context.reservations
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }
    }
}
