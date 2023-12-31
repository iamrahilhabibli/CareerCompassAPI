﻿using CareerCompassAPI.Application.Abstraction.Repositories.ICompanyRepositories;
using CareerCompassAPI.Application.Abstraction.Repositories.IFollowerRepositories;
using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Follower_DTOs;
using CareerCompassAPI.Domain.Entities;
using CareerCompassAPI.Domain.Identity;
using CareerCompassAPI.Persistence.Contexts;
using CareerCompassAPI.Persistence.Exceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class FollowerService : IFollowerService
    {
        private readonly CareerCompassDbContext _context;
        private readonly ICompanyReadRepository _companyReadRepository;
        private readonly IFollowerReadRepository _followerReadRepository;
        private readonly IFollowerWriteRepository _followerWriteRepository;
        private readonly INotificationService _notificationService;

        public FollowerService(CareerCompassDbContext context,
                               ICompanyReadRepository companyReadRepository,
                               IFollowerWriteRepository followerWriteRepository,
                               IFollowerReadRepository followerReadRepository,
                               INotificationService notificationService)
        {
            _context = context;
            _companyReadRepository = companyReadRepository;
            _followerWriteRepository = followerWriteRepository;
            _followerReadRepository = followerReadRepository;
            _notificationService = notificationService;
        }

        public async Task CreateAsync(FollowerCreateDto followerCreateDto)
        {
            if (followerCreateDto is null)
            {
                throw new ArgumentNullException("Empty value can not be passed as an argument");
            }
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerCreateDto.appUserId);
            if (user is not AppUser) { throw new NotFoundException("User does not exist"); }
            Company company = await _companyReadRepository.GetByIdAsync(followerCreateDto.companyId);
            if (company is not Company) { throw new NotFoundException("Company does not exist"); }
            var existingFollower = await _context.Followers
            .FirstOrDefaultAsync(f => f.User == user && f.Company == company);

            if (existingFollower != null)
            {
                throw new InvalidOperationException("User is already following this company.");
            }
            Follower newFollower = new()
            {
                User = user,
                UserEmail = user.Email,
                Company = company,
            };
            await _followerWriteRepository.AddAsync(newFollower);
            await _followerWriteRepository.SaveChangesAsync();

            var userId = Guid.Parse(followerCreateDto.appUserId);  
            var title = $"Successfully followed {company.Name}";
            var message = "You will now receive updates from this company.";
            BackgroundJob.Schedule<INotificationService>(x => x.CreateAsync(userId, title, message), TimeSpan.FromSeconds(5));
        }

        public async Task<List<GetAllFollowersDto>> GetAllFollowersByCompanyId(Guid companyId)
        {
            var followersForCompany = _followerReadRepository.GetAll()
                .Where(f => f.Company.Id == companyId)
                .Select(f => new GetAllFollowersDto(f.UserEmail))
                .ToList();

            if (!followersForCompany.Any())
            {
                return new List<GetAllFollowersDto>();
            }
            return followersForCompany;
        }


        public async Task<List<FollowerGetFollowedCompaniesDto>> GetFollowedCompanies(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var followersQuery = _followerReadRepository.GetAllByExpression(f => f.User.Id == userId, take: 10000, skip: 0);

            if (!followersQuery.Any())
            {
                return new List<FollowerGetFollowedCompaniesDto>();
            }
            var followedCompanies = followersQuery.Select(f => new FollowerGetFollowedCompaniesDto(f.Company.Id)).ToList();
            return await Task.FromResult(followedCompanies);
        }


        public async Task Remove(FollowerRemoveDto followerRemoveDto)
        {
            if (followerRemoveDto is null)
            {
                throw new ArgumentNullException("Empty value may not be passed as an argument");
            }
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerRemoveDto.appUserId);
            if (user is not AppUser)
            {
                throw new NotFoundException("User does not exist");
            }
            Company company = await _companyReadRepository.GetByIdAsync(followerRemoveDto.companyId);
            if (company is not Company)
            {
                throw new NotFoundException("Company does not exist");
            }
            var existingFollower = await _context.Followers
                .FirstOrDefaultAsync(f => f.User == user && f.Company == company);

            if (existingFollower == null)
            {
                throw new NotFoundException("Follower relationship not found.");
            }

            _followerWriteRepository.Remove(existingFollower);
            await _followerWriteRepository.SaveChangesAsync();

            var userId = Guid.Parse(followerRemoveDto.appUserId);
            var title = $"Successfully unfollowed {company.Name}";
            var message = "You will no longer receive updates from this company.";
            BackgroundJob.Schedule<INotificationService>(x => x.CreateAsync(userId, title, message), TimeSpan.FromSeconds(5));
        }
    }
}
