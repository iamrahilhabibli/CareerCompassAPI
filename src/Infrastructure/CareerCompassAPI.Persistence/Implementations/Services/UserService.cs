﻿using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Auth_DTOs;
using CareerCompassAPI.Persistence.Contexts;
using CareerCompassAPI.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class UserService : IUserService
    {
        private readonly CareerCompassDbContext _context;
        public UserService(CareerCompassDbContext context)
        {
            _context = context;
        }
        public async Task<UserDetailsGetDto> GetDetailsAsync(string userId)
        {
            var user = await _context.Users.Include(u => u.JobSeekers)
                                            .Include(u => u.Recruiters)
                                                .ThenInclude(r => r.Subscription)
                                            .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            var firstName = user.JobSeekers?.FirstName ?? user.Recruiters?.FirstName;
            var lastName = user.JobSeekers?.LastName ?? user.Recruiters?.LastName;
            var email = user.Email;
            var subscriptionName = user.Recruiters?.Subscription?.Name;

            return new UserDetailsGetDto(firstName, lastName, email, subscriptionName);
        }

    }
}
