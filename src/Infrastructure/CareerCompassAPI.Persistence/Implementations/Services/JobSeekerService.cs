﻿using CareerCompassAPI.Application.Abstraction.Repositories.IJobSeekerRepositories;
using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.JobSeeker_DTOs;
using CareerCompassAPI.Domain.Entities;
using CareerCompassAPI.Domain.Enums;
using CareerCompassAPI.Persistence.Contexts;
using CareerCompassAPI.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly IJobSeekerReadRepository _jobSeekerReadRepository;
        private readonly IJobSeekerWriteRepository _jobSeekerWriteRepository;
        private readonly CareerCompassDbContext _context;
        private readonly ILogger<JobSeekerDetails> _logger;
        public JobSeekerService(IJobSeekerReadRepository jobSeekerReadRepository,
                                CareerCompassDbContext context,
                                IJobSeekerWriteRepository jobSeekerWriteRepository,
                                ILogger<JobSeekerDetails> logger)
        {
            _jobSeekerReadRepository = jobSeekerReadRepository;
            _context = context;
            _jobSeekerWriteRepository = jobSeekerWriteRepository;
            _logger = logger;
        }

        public async Task CreateAsync(JobSeekerCreateDto dto, string jobseekerAppUserId)
        {
            var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(j => j.AppUserId == jobseekerAppUserId);
            if (jobSeeker is not JobSeeker)
            {
                throw new NotFoundException($"JobSeeker with ID {jobseekerAppUserId} not found.");
            }

            EducationLevel educationLevel = await _context.EducationLevels.FirstOrDefaultAsync(el => el.Id == dto.educationLevelId);
            if (educationLevel is not EducationLevel)
            {
                throw new NotFoundException($"EducationLevel with ID {dto.educationLevelId} not found.");
            }

            JobSeekerDetails jobSeekerDetails = new()
            {
                JobSeeker = jobSeeker,
                PhoneNumber = dto.phoneNumber,
                Experience = dto.experience,
                EducationLevel = educationLevel,
                Description = dto.description
            };
            await _context.JobSeekerDetails.AddAsync(jobSeekerDetails);
            await _context.SaveChangesAsync();
        }
        public async Task<List<JobseekerApprovedGetDto>> GetApprovedPositionsByAppUserId(string appUserId)
        {
            var jobSeeker = await _context.JobSeekers
                .Include(js => js.JobApplications) 
                .ThenInclude(ja => ja.Vacancy)    
                .ThenInclude(v => v.Recruiter)  
                .ThenInclude(r => r.Company)    
                .FirstOrDefaultAsync(js => js.AppUserId == appUserId);

            if (jobSeeker is not JobSeeker)
            {
                throw new NotFoundException("JobSeeker not found");
            }

            var approvedPositions = jobSeeker.JobApplications
                .Where(ja => ja.Status == ApplicationStatus.Approved && ja.IsDeleted == false)
                .Select(ja => new JobseekerApprovedGetDto(
                    ja.Id,
                    ja.Vacancy.Recruiter.FirstName,
                    ja.Vacancy.Recruiter.LastName,
                    ja.Vacancy.Recruiter.Company.Name,
                    ja.Vacancy.Recruiter.AppUserId
                ))
                .ToList();
            return approvedPositions;
        }

        public async Task<JobSeekerGetDto> GetByUserId(Guid userId)
        {
            var response = await _jobSeekerReadRepository.GetByUserIdAsync(userId);
            var userResponse = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());
            JobSeekerGetDto jobSeeker = new(
                id: response.Id,
                firstName: response.FirstName,
                lastName: response.LastName,
                email: userResponse.Email,
                phoneNumber: userResponse.PhoneNumber);
            return jobSeeker;
        }

        public async Task UploadLogoAsync(string appUserId, JobseekerAvatarUploadDto avatarUploadDto)
        {
            if (avatarUploadDto is null)
            {
                throw new ArgumentNullException("Empty value passed as an argument");
            }
            JobSeeker jobseeker = await _jobSeekerReadRepository.GetByUserIdAsync(Guid.Parse(appUserId));
            if (jobseeker is not JobSeeker)
            {
                throw new NotFoundException("Jobseeker not found");
            }
            jobseeker.LogoUrl = avatarUploadDto.url;
            _jobSeekerWriteRepository.Update(jobseeker);
            await _jobSeekerWriteRepository.SaveChangesAsync();
        }
    }
}
