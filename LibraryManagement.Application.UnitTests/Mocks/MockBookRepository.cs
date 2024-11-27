using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Mocks
{
    public class MockBookRepository
    {
        public static Mock<IBookRepository> GetMockBooksRepository()
        {
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    IsReserved = false,
                    IsBorrowed = false,
                    ISBN = 123456789,
                    ReturnDate = null
                },
                new Book
                {
                    Id = 2,
                    Title = "Mastering ASP.NET Core",
                    IsReserved = true,
                    IsBorrowed = false,
                    ISBN = 23456789,
                    ReturnDate = null
                },
                new Book
                {
                    Id = 3,
                    Title = "Clean Architecture in Practice",
                    IsReserved = false,
                    IsBorrowed = true,
                    ISBN = 34567890,
                    ReturnDate = DateTime.UtcNow.AddDays(14) // Due in 14 days
                },
                new Book
                {
                    Id = 4,
                    Title = "Design Patterns Explained",
                    IsReserved = true,
                    IsBorrowed = true,
                    ISBN = 4567890,
                    ReturnDate = DateTime.UtcNow.AddDays(7) // Due in 7 days
                },
                new Book
                {
                    Id = 5,
                    Title = "Entity Framework Core Essentials",
                    IsReserved = false,
                    IsBorrowed = false,
                    ISBN = 567890123,
                    ReturnDate = null
                }
            };

            var mockRepo = new Mock<IBookRepository>();

            // Mock GetAllAsync
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(books);

            // Mock GetByIdAsync
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => books.FirstOrDefault(b => b.Id == id));

            // Mock CreateAsync
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Book>())).Callback((Book book) => books.Add(book));

            // Mock UpdateAsync
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Book>())).Callback((Book book) =>
            {
                var existingBook = books.FirstOrDefault(b => b.Id == book.Id);
                if (existingBook != null)
                {
                    books.Remove(existingBook);
                    books.Add(book);
                }
            });

            // Mock DeleteAsync
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Book>())).Callback((Book book) =>
            {
                var existingBook = books.FirstOrDefault(b => b.Id == book.Id);
                if (existingBook != null)
                {
                    books.Remove(existingBook);
                }
            });

            // Mock SearchBookAsync
            mockRepo.Setup(r => r.SearchBookAsync(It.IsAny<string>())).ReturnsAsync((string title) =>
                books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList());

            // Mock ReserveBookAsync
            mockRepo.Setup(r => r.ReserveBookAsync(It.IsAny<int>())).ReturnsAsync((int bookId) =>
            {
                var book = books.FirstOrDefault(b => b.Id == bookId);
                if (book != null && !book.IsReserved)
                {
                    book.IsReserved = true;
                }
                return book;
            });


            // Mock IsBookUnique
            mockRepo.Setup(r => r.IsBookUnique(It.IsAny<int>())).ReturnsAsync((int isbn) =>
                !books.Any(b => b.ISBN == isbn));

            return mockRepo;
        }
    }
}
