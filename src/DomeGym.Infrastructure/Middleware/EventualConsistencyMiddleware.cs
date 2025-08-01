using DomeGym.Domain.Common;
using DomeGym.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;

using DomeGym.Domain.Common;
namespace DomeGym.Infrastructure.Middleware
{
    public class EventualConsistencyMiddleware
    {
        public const string DomainEventsKey = "DomainEventsKey";

        private readonly RequestDelegate _next;

        public EventualConsistencyMiddleware(RequestDelegate next)
        {
            _next = _next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(
            HttpContext context,
            IPublisher publisher,
            DomeGymDbContext dbContext)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync();
            context.Response.OnCompleted(async () =>
            {
                try
                {
                    if (context.Items.TryGetValue(DomainEventsKey, out var value) 
                        && value is Queue<IDomainEvent> domainEvents)
                    {
                        while (domainEvents.TryDequeue(out var nextEvent))
                        {
                            await publisher.Publish(nextEvent);
                        }
                    }
                    await transaction.CommitAsync();


                }
                catch(EventualConsistencyException e)
                {
                    
                }
                finally
                {
                    await transaction.DisposeAsync();
                }

            });
              await _next(context);
        }
    }
}
