﻿using CareerCompassAPI.Application.DTOs.AppSetting_DTOs;
using CareerCompassAPI.Application.DTOs.AppUser_DTOs;
using CareerCompassAPI.Application.DTOs.Dashboard_DTOs;
using CareerCompassAPI.Application.DTOs.ExperienceLevel_DTOs;
using CareerCompassAPI.Application.DTOs.JobType_DTOs;
using CareerCompassAPI.Application.DTOs.Location_DTOs;
using CareerCompassAPI.Application.DTOs.Resume_DTOs;
using CareerCompassAPI.Application.DTOs.Schedule_DTOs;
using CareerCompassAPI.Application.DTOs.Subscription_DTOs;
using CareerCompassAPI.Application.DTOs.TeamMember_DTOs;
using CareerCompassAPI.Domain.Concretes;
using CareerCompassAPI.Domain.Enums;

namespace CareerCompassAPI.Application.Abstraction.Services
{
    public interface IDashboardService
    {
        Task<PaginatedResponse<AppUserGetDto>> GetAllAsync(string searchQuery, int pageNumber, int pageSize);
        Task RemoveUser(string appUserId);
        Task ChangeUserRole(ChangeUserRoleDto changeUserRoleDto);
        Task<PaginatedResponse<CompaniesListGetDto>> GetAllCompaniesAsync(string? sortOrders, string? searchQuery, int page, int pageSize);
        Task RemoveCompany(Guid companyId);
        Task<List<PendingReviewsDto>> GetAllPendingReviews();
        Task UpdateReviewStatus(Guid reviewId, ReviewStatus newStatus);
        Task<List<UserRegistrationStatDto>> GetUserRegistrationStatsAsync(DateTime? startDate, DateTime? endDate);
        Task<List<EducationLevelsGetDto>> GetAllEducationLevelsAsync();
        Task<Guid> CreateEducationLevel(CreateEducationLevelDto createEducationLevelDto);
        Task RemoveEducationLevel(Guid levelId);
        Task UpdateEducationLevel(EducationLevelUpdateDto updateEducationLevelDto);
        Task<List<ExperienceLevelGetDto>> GetAllExperienceLevelsAsync();
        Task RemoveExperienceLevel(Guid levelId);
        Task<Guid> CreateExperienceLevel(ExperienceLevelCreateDto experienceLevelCreateDto);
        Task<List<LocationGetDto>> GetAllLocationsAsync(string? searchQuery);
        Task<Guid> CreateJobLocation(JobLocationCreateDto jobLocationCreateDto);
        Task RemoveJobLocation(Guid jobLocationId);
        Task<List<JobTypeGetDto>> GetAllTypesAsync();
        Task<Guid> CreateJobType(JobTypeCreateDto jobTypeCreateDto);
        Task RemoveJobType(Guid jobTypeId);
        Task<List<DTOs.Dashboard_DTOs.SubscriptionGetDto>> GetAllSubscriptionsAsync();
        Task<Guid> CreateSubscription(SubscriptionCreateDto subscriptionCreateDto);
        Task RemoveSubscription(Guid subscriptionId);
        Task UpdateSubscription(SubscriptionUpdateDto updateSubscriptionDto);
        Task<List<ShiftAndScheduleGetDto>> GetAllShiftsAsync();
        Task RemoveShift(Guid shiftId);
        Task<Guid> CreateShift(ShiftAndScheduleCreateDto shiftAndScheduleDto);
        Task<PaginatedResponse<PaymentsListGetDto>> GetAllPaymentsAsync(int page, int pageSize);
        Task<PaginatedResponse<UserFeedbacksGetDto>> GetAllFeedbacksAsync(int page, int pageSize);  
        Task RemoveFeedback(Guid feedbackId);
        Task SetIsActive(Guid feedbackId);
        Task<Guid> CreateMember(TeamMemberCreateDto teamMemberCreateDto);
        Task<List<TeamMembersGetDto>> GetAllTeamMembers();
        Task<List<AppSettingGetDto>> GetAppSettingListAsync();
        Task<List<ResumeGetDto>> GetAllResumes();
        Task RemoveResume(Guid resumeId);
        Task RemoveTeamMember(Guid memberId);
    }
}
