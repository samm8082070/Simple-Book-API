﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication4.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
