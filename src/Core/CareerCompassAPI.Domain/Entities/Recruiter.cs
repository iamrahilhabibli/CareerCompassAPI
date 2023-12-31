﻿using CareerCompassAPI.Domain.Entities.Common;
using CareerCompassAPI.Domain.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerCompassAPI.Domain.Entities
{
    public class Recruiter : BaseEntity
    {
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        [ForeignKey("Company")]
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Subscriptions Subscription { get; set; } = null!;
        public ICollection<Vacancy>? Vacancies { get; set; }
        public int CurrentPostCount { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
    }
}
