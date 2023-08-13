﻿using CareerCompassAPI.Domain.Entities.Common;

namespace CareerCompassAPI.Domain.Entities
{
    public class Company:BaseEntity
    {
        public string Name { get; set; } = null!;
        public CompanyDetails? Details { get; set; }
    }
}