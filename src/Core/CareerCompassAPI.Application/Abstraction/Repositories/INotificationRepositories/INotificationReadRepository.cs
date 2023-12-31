﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CareerCompassAPI.Domain.Entities;

namespace CareerCompassAPI.Application.Abstraction.Repositories.INotificationRepositories
{
    public interface INotificationReadRepository : IReadRepository<Notification>
    {
        Task<IQueryable<Notification>> GetByUserIdAsync(Guid userId);
    }
}
