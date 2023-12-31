﻿using AutoMapper;
using CareerCompassAPI.Application.Abstraction.Repositories.ICompanyRepositories;
using CareerCompassAPI.Application.Abstraction.Repositories.IIndustryRepositories;
using CareerCompassAPI.Application.Abstraction.Repositories.IRecruiterRepositories;
using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Company_DTOs;
using CareerCompassAPI.Domain.Entities;
using CareerCompassAPI.Domain.Enums;
using CareerCompassAPI.Persistence.Contexts;
using CareerCompassAPI.Persistence.Exceptions;
using CareerCompassAPI.Persistence.Implementations.Repositories.CompanyRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CareerCompassAPI.Persistence.Implementations.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyWriteRepository _companyWriteRepository;
        private readonly IIndustryReadRepository _industryReadRepository;
        private readonly ICompanyReadRepository _companyReadRepository;
        private readonly CareerCompassDbContext _context;
        private readonly IRecruiterWriteRepository _recruiterWriteRepository;
        private readonly IMapper _mapper;
        public CompanyService(ICompanyWriteRepository companyWriteRepository,
                              IIndustryReadRepository industryReadRepository,
                              ICompanyReadRepository companyReadRepository,
                              CareerCompassDbContext context,
                              IRecruiterWriteRepository recruiterWriteRepository,
                              IMapper mapper)
        {
            _companyWriteRepository = companyWriteRepository;
            _industryReadRepository = industryReadRepository;
            _companyReadRepository = companyReadRepository;
            _context = context;
            _mapper = mapper;
            _recruiterWriteRepository = recruiterWriteRepository;
        }

        public async Task CompanyDetailsUpdate(CompanyDetailsUpdateDto companyDetailsUpdateDto)
        {
            var existingCompany = await _context.Companies
                .Include(c => c.Details)  
                .FirstOrDefaultAsync(c => c.Id == companyDetailsUpdateDto.id);

            if (existingCompany == null) { return; }

            existingCompany.Details.Ceo = companyDetailsUpdateDto.ceoName;
            existingCompany.Details.Link = companyDetailsUpdateDto.webLink;
            existingCompany.Name = companyDetailsUpdateDto.companyName;
            existingCompany.Details.Description = companyDetailsUpdateDto.description;
            existingCompany.Details.Address = companyDetailsUpdateDto.address;

            _context.Companies.Update(existingCompany);
            await _context.SaveChangesAsync();
        }


        public async Task CreateAsync(CompanyCreateDto companyCreateDto, string userId)
        {
            if (companyCreateDto is null)
            {
                throw new ArgumentNullException("Empty value passed as an argument");
            }
            var industry = await _industryReadRepository.GetByExpressionAsync(i => i.Id == companyCreateDto.industryId);
            if (industry is not Industry)
            {
                throw new NotFoundException("Matching industry is not found");
            }
            var jobLocation = await _context.JobLocations.FirstOrDefaultAsync(j => j.Id == companyCreateDto.locationId);
            if (jobLocation is not JobLocation)
            {
                throw new NotFoundException("Matching job location is not found");
            }
            var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.AppUserId == userId);
            if (recruiter is not Recruiter) { throw new NotFoundException("Recruiter is not found"); }

            CompanyDetails newCompanyDetails = new()
            {
                Ceo = companyCreateDto.ceoName,
                DateFounded = companyCreateDto.dateFounded,
                Industry = industry,
                CompanySize = companyCreateDto.companySize,
                Location = jobLocation,
                Link = companyCreateDto.websiteLink,
                Description = companyCreateDto.description,
                Address = companyCreateDto.address
            };
            Company newCompany = new()
            {
                Name = companyCreateDto.name,
                Details = newCompanyDetails
            };
            await _companyWriteRepository.AddAsync(newCompany);
            await _companyWriteRepository.SaveChangesAsync();

            recruiter.CompanyId = newCompany.Id;
            await _recruiterWriteRepository.SaveChangesAsync();
        }

        public async Task<List<CompanyDetailsGetDto>> GetCompanyBySearchAsync(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return new List<CompanyDetailsGetDto>();
            }
            var lowerCaseCompanyName = companyName.ToLower();
            var query = _context.Companies
                .Where(c => !c.IsDeleted && c.Name.ToLower().Contains(lowerCaseCompanyName))
                .Include(c => c.Details)
                    .ThenInclude(d => d.Location)
                .Include(c => c.Details)
                    .ThenInclude(d => d.Industry)
                .Take(10);

            var companies = await query.ToListAsync();

            return companies.Select(c => new CompanyDetailsGetDto(
                 c.Id,
                 c.Name,
                 c.Details.Description,
                 c.Details.Ceo,
                 c.Details.DateFounded,
                 Enum.GetName(typeof(CompanySizeEnum), c.Details.CompanySize),
                 c.Details.Industry.Name,
                 c.Details.Link,
                 c.Details.Location.Location,
                 c.LogoUrl
                 )).ToList();
        }



        public async Task<CompanyGetDto> GetCompanyDetailsById(Guid companyId)
        {
            var company = await _context.Companies
                 .Include(c => c.Details)
                 .ThenInclude(d => d.Industry)
                 .FirstOrDefaultAsync(c => c.Id == companyId);
            if (company is not Company)
            {
                throw new ArgumentNullException();
            }
            var details = company.Details;
            var industryName = details.Industry.Name;
            CompanyGetDto companyGetDto = new(
                company.Id,
                company.Name, details.Ceo, details.DateFounded, details.CompanySize, industryName, details.Link, details.Description, details.Address, company.LogoUrl
                );
            return companyGetDto;
        }

        public async Task<List<HighestRatedCompanyGetDto>> GetHighestRated()
        {
            //var companiesWithReviews = await _companyReadRepository.GetAll(isTracking: false, includes: new[] { "Reviews" })
            //                        .Select(c => new
            //                         {
            //                            c.Id,
            //                            c.Name,
            //                            c.LogoUrl,
            //                            Reviews = c.Reviews.Select(r => r.Rating)
            //                                           })
            //                                           .ToListAsync();
            var highestRatedCompanies = await _context.Companies 
        .Select(c => new
        {
            c.Id,
            c.Name,
            c.LogoUrl,
            ReviewCount = c.Reviews.Count(),
            AverageRating = c.Reviews.Average(r => r.Rating)
        })
        .OrderByDescending(dto => dto.ReviewCount)
        .ThenByDescending(dto => dto.AverageRating)
        .Take(9)
        .ToListAsync();
            var result = highestRatedCompanies.Select(c => new HighestRatedCompanyGetDto(
                c.Id,
                c.Name,
                c.LogoUrl,
                c.ReviewCount,
                c.AverageRating
            )).ToList();

            return result;
        }
        public async Task Remove(Guid companyId)
        {
            var company = await _companyReadRepository.GetByIdAsync(companyId);
            if (company is not Company)
            {
                throw new NotFoundException("Company not found");
            }
            _companyWriteRepository.Remove(company);
            await _companyWriteRepository.SaveChangesAsync();
        }

        public async Task UploadLogoAsync(Guid companyId, CompanyLogoUploadDto logoUploadDto)
        {
            if (logoUploadDto is null)
            {
                throw new ArgumentNullException("Empty value passed as an argument");
            }
            Company company = await _companyReadRepository.GetByIdAsync(companyId);
            if (company is not Company)
            {
                throw new NotFoundException("Company not found");
            }
            company.LogoUrl = logoUploadDto.url;
            _companyWriteRepository.Update(company);
            await _companyWriteRepository.SaveChangesAsync();
        }
    }
}
