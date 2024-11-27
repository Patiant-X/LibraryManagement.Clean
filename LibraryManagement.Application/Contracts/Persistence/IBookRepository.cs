using LibraryManagement.Domain;

namespace LibraryManagement.Application.Contracts.Persistence
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IReadOnlyList<Book>> SearchBookAsync(string title);

        Task<Book> ReserveBookAsync(int bookId);

        Task<bool> IsBookUnique(int ISBN);
    }
}
