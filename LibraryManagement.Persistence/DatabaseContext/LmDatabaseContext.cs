using LibraryManagement.Domain;
using LibraryManagement.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Persistence.DatabaseContext
{
    public class LmDatabaseContext : DbContext
    {
        private readonly IPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LmDatabaseContext(DbContextOptions<LmDatabaseContext> options, IPublisher publisher, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _publisher = publisher;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Book> books { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Notification> notifications { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
                .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
            {
                entry.Entity.DateModified = DateTime.Now;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DateCreated = DateTime.Now;
                }
            }

            // Collect domain events
            var domainEvents = ChangeTracker.Entries<BaseEntity>()
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            // Clear domain events
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                entry.Entity.ClearDomainEvents();
            }

            // Save changes to the database
            var result = await base.SaveChangesAsync(cancellationToken);

            // Handle domain events
            if (IsUserWaitingOnline())
            {
                AddDomainEventsToOfflineProcessingQueue(domainEvents);
            }
            else
            {
                await PublishDomainEvents(domainEvents, cancellationToken);
            }

            return result;
        }

        private async Task PublishDomainEvents(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            };
        }

        private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
        {
            // Fetch or initialize the queue
            var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
                .TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> existingQueue
                    ? existingQueue
                    : new Queue<IDomainEvent>();

            // Enqueue the events
            domainEvents.ForEach(domainEventsQueue.Enqueue);

            // Store the updated queue in HttpContext
            _httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;
        }

        private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmDatabaseContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
