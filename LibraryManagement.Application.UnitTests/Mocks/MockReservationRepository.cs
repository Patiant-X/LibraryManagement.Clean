using LibraryManagement.Application.Contracts.Persistence;
using LibraryManagement.Domain;
using Moq;

namespace LibraryManagement.Application.UnitTests.Mocks
{
    public class MockReservationRepository
    {
        public static Mock<IReservationRepository> GetMockReservationRepository()
        {
            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = 1,
                    BookId = 1,
                    CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
                    DateCreated = DateTime.UtcNow.AddHours(-23), // Not expired
    
                },
                new Reservation
                {
                    Id = 2,
                    BookId = 2,
                    CustomerId = "417732bf-58b9-45ad-a416-28446c65b7c1",
                    DateCreated = DateTime.UtcNow.AddHours(-25) // Expired
                },
                new Reservation
                {
                    Id = 3,
                    BookId = 1,
                    CustomerId = "12345678-1234-1234-1234-abcdefabcdef",
                    DateCreated = DateTime.UtcNow.AddHours(-10) // Not expired
                }
            };

            var mockRepo = new Mock<IReservationRepository>();

            // Mock GetActiveReservationsByBookIdAsync
            mockRepo.Setup(r => r.GetActiveReservationsByBookIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int bookId) =>
                {
                    return reservations.FirstOrDefault(r => r.BookId == bookId && !r.IsExpired);
                });

            // Mock GetExpiredReservationsAsync
            mockRepo.Setup(r => r.GetExpiredReservationsAsync())
                .ReturnsAsync(() =>
                {
                    return reservations.Where(r => r.IsExpired).ToList();
                });

            // Mock GetReservationsByCustomerIdAsync
            mockRepo.Setup(r => r.GetReservationsByCustomerIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string customerId) =>
                {
                    return reservations.Any(r => r.CustomerId == customerId) ? reservations.Where(r => r.CustomerId == customerId).ToList()
    : null;
                });

            // Mock CreateAsync
            mockRepo.Setup(r => r.CreateAsync(It.IsAny<Reservation>())).Callback((Reservation reservation) =>
            {
                reservation.Id = reservations.Max(r => r.Id) + 1; // Auto-increment Id
                reservation.DateCreated = DateTime.Now;
                reservations.Add(reservation);
            });

            // Mock UpdateAsync
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Callback((Reservation reservation) =>
            {
                var existingReservation = reservations.FirstOrDefault(r => r.Id == reservation.Id);
                if (existingReservation != null)
                {
                    reservations.Remove(existingReservation);
                    reservations.Add(reservation);
                }
            });

            // Mock DeleteAsync
            mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Reservation>())).Callback((Reservation reservation) =>
            {
                var existingReservation = reservations.FirstOrDefault(r => r.Id == reservation.Id);
                if (existingReservation != null)
                {
                    reservations.Remove(existingReservation);
                }
            });

            return mockRepo;
        }
    }
}
