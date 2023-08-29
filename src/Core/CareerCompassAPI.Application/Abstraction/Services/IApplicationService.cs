﻿using CareerCompassAPI.Application.DTOs.Application_DTOs;
using CareerCompassAPI.Domain.Entities;
using CareerCompassAPI.Domain.Enums;

namespace CareerCompassAPI.Application.Abstraction.Services
{
    public interface IApplicationService
    {
        Task<int> CreateAsync(ApplicationCreateDto applicationCreateDto);
        Task<List<ApplicantsGetDto>> GetApplicationsByAppUserId(string appUserId);
        Task UpdateAsync(ApplicationStatusUpdateDto applicationStatusUpdateDto);
    }
}