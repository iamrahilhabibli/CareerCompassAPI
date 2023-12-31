﻿using CareerCompassAPI.Application.DTOs.Company_DTOs;

namespace CareerCompassAPI.Application.Abstraction.Services
{
    public interface ICompanyService
    {
        Task CreateAsync(CompanyCreateDto companyCreateDto, string userId);
        Task Remove(Guid companyId);
        Task<CompanyGetDto> GetCompanyDetailsById(Guid companyId);
        Task<List<CompanyDetailsGetDto>> GetCompanyBySearchAsync(string companyName);
        Task UploadLogoAsync(Guid companyId, CompanyLogoUploadDto logoUploadDto);
        Task<List<HighestRatedCompanyGetDto>> GetHighestRated();
        Task CompanyDetailsUpdate(CompanyDetailsUpdateDto companyDetailsUpdateDto);
    }
}
