using LibraryManagement.Persistence.Middleware;
using Microsoft.AspNetCore.Builder;

namespace LibraryManagement.Persistence
{
    public static class RequestPipeline
    {
        public static IApplicationBuilder AddPersistenceMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<EventualConsistencyMiddleware>();
            return builder;
        }
    }
}
