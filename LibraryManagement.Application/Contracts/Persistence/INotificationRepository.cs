using LibraryManagement.Domain;

namespace LibraryManagement.Application.Contracts.Persistence
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IReadOnlyList<Notification>> GetActiveNotificationsByBookIdAsync(int bookId);
        Task<IReadOnlyList<Notification>> GetNotificationsByCustomerIdAsync(string customerId);
        Task<bool> IsNotificationUnique(int bookId, string customerId);
    }
}
