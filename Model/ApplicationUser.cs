using Microsoft.AspNetCore.Identity;

namespace BackEnd.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockoutEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
