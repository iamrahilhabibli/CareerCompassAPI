﻿using CareerCompassAPI.Application.Abstraction.Services;
using CareerCompassAPI.Application.DTOs.Company_DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CareerCompassAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] CompanyCreateDto companyCreateDto, [FromQuery] string userId)
        {
            await _companyService.CreateAsync(companyCreateDto, userId);
            return StatusCode((int)HttpStatusCode.Created);
        }


        [HttpDelete("[action]/{companyId}")]
        public async Task<IActionResult> Remove(Guid companyId)
        {
            await _companyService.Remove(companyId);
            return Ok();
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetDetails([FromQuery] Guid companyId) 
        {
            CompanyGetDto response = await _companyService.GetCompanyDetailsById(companyId);
            return Ok(response);
        }
    }
}
