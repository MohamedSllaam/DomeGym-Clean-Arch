using DomeGym.Application.Common.Interfaces;
using DomeGym.Domain.AdminAggregate.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomeGym.Application.Subscriptions.Events;

public class SubscriptionSetEventHandler : INotificationHandler<SubscriptionSetEvent>
{
    private readonly ISubscriptionsRepository _subscriptionsRepository;
public SubscriptionSetEventHandler(ISubscriptionsRepository subscriptionsRepository)
    {
        _subscriptionsRepository = subscriptionsRepository;
    }

    public async Task Handle(SubscriptionSetEvent subscriptionSetEvent, CancellationToken cancellationToken)
    {
        await _subscriptionsRepository.AddSubscriptionAsync(subscriptionSetEvent.Subscription);
             
    }
}
