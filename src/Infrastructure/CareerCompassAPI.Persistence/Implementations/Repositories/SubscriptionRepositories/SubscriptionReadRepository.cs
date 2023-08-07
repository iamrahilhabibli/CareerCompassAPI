﻿using CareerCompassAPI.Application.Abstraction.Repositories.ISubscriptionRepository;
using CareerCompassAPI.Domain.Entities;
using CareerCompassAPI.Persistence.Contexts;

namespace CareerCompassAPI.Persistence.Implementations.Repositories.SubscriptionRepositories
{
    public class SubscriptionReadRepository : ReadRepository<Subscriptions>, ISubscriptionReadRepository
    {
        public SubscriptionReadRepository(CareerCompassDbContext context) : base(context)
        {
        }
    }
}
