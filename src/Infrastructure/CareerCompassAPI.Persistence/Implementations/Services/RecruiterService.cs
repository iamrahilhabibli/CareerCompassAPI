﻿using CareerCompassAPI.Application.Abstraction.Repositories.IRecruiterRepositories;
using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Recruiter_DTOs;
using CareerCompassAPI.Domain.Entities;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IRecruiterReadRepository _recruiterReadRepository;
        public RecruiterService(IRecruiterReadRepository recruiterReadRepository)
        {
            _recruiterReadRepository = recruiterReadRepository;
        }
        public async Task<RecruiterGetDto> GetRecruiterByUserId(Guid userId)
        {
            Recruiter recruiter = await _recruiterReadRepository.GetByUserIdAsync(userId);
            if (recruiter is not Recruiter)
            {
                throw new ArgumentNullException();
            }
            Guid subscriptionId = recruiter.Subscription.Id;

            RecruiterGetDto recruiterGetDto = new(
                AppUserId: recruiter.AppUserId,
                CompanyId: recruiter.CompanyId,
                FirstName: recruiter.FirstName,
                LastName: recruiter.LastName,
                JobLocationId: recruiter.JobLocationId,
                SubscriptionId: subscriptionId
            );
            return recruiterGetDto;
        }

    }
}