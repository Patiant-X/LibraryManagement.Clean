using LibraryManagement.Domain.Common;
using LibraryManagement.Persistence.DatabaseContext;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement.Persistence.Middleware
{
    public class EventualConsistencyMiddleware
    {
        private readonly RequestDelegate _next;

        public EventualConsistencyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPublisher publisher, LmDatabaseContext dbContext, Application.Contracts.Logging.IAppLogger<EventualConsistencyMiddleware> appLogger)
        {
            // Start a database transaction
            var transaction = await dbContext.Database.BeginTransactionAsync();

            // Hook into the response lifecycle to process after response is sent
            context.Response.OnCompleted(async () =>
            {
                try
                {
                    // Check if Domain Events exist in HttpContext
                    if (context.Items.TryGetValue("DomainEventsQueue", out var value) &&
                        value is Queue<IDomainEvent> domainEventsQueue)
                    {
                        // Publish all domain events
                        while (domainEventsQueue.TryDequeue(out var domainEvent))
                        {
                            await publisher.Publish(domainEvent);
                        }
                    }

                    // Commit the database transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Notify admin/log error - Response to user already sent, but ensure event didn't process
                    appLogger.LogWarning("Response to user already sent, but ensure event didn't process", ex);
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            });

            // Pass control to the next middleware
            await _next(context);
        }
    }
}
