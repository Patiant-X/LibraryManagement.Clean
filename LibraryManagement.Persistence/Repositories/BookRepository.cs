using LibraryManagement.Application.Contracts.Logging;
using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using LibraryManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Persistence.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(LmDatabaseContext context, IAppLogger<Book> appLogger) : base(context, appLogger)
        {
        }

        public async Task<bool> IsBookUnique(int ISBN)
        {
            // (int isbn) => !books.Any(b => b.ISBN == isbn) mock implementation
            return await _context.books.AsNoTracking().AnyAsync(b => b.ISBN == ISBN) == false;
        }

        public async Task<Book> ReserveBookAsync(int bookId)
        {
            var book = await _context.books.FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            if (book.IsReserved)
            {
                throw new ApplicationException("The book is already reserved.");
            }

            book.IsReserved = true;
            book.IsBorrowed = false;

            await HandleDatabaseOperation(async () =>
            {
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            });

            return book;

        }

        public async Task<IReadOnlyList<Book>> SearchBookAsync(string title)
        {
            return await _context.books.AsNoTracking()
               .Where(b => EF.Functions.Like(b.Title, $"%{title}%"))
               .ToListAsync(); ;
        }
    }
}
