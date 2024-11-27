using LibraryManagement.Domain;

namespace LibraryManagement.Application.Contracts.Persistence
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<Reservation> GetActiveReservationsByBookIdAsync(int bookId);
        Task<IReadOnlyList<Reservation>> GetExpiredReservationsAsync();
        Task<IReadOnlyList<Reservation>> GetReservationsByCustomerIdAsync(string customerId);
    }
}
